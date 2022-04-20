using System.Globalization;
using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fakestagram.Data.DTOs.Tokens;
using Fakestagram.Data.Repositories.Contracts;
using Fakestagram.Exceptions;

namespace Fakestagram.Services.Providers
{
    public class JwtAuthProvider : IAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly TimeSpan accessTokenLifetime;
        private readonly TimeSpan refreshTokenLifetime;
        private readonly SymmetricSecurityKey tokenSecurityKey;
        private const int SEQUENCE_THRESHOLD = 20;

        public JwtAuthProvider(IConfiguration configuration, IRefreshTokenRepository tokenRepository,
                                TokenValidationParameters tokenValidationParameters, IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            _tokenValidationParameters = tokenValidationParameters;
            _httpContextAccessor = httpContextAccessor;
            accessTokenLifetime = TimeSpan.Parse(_configuration.GetSection("JWTConfig").GetSection("AccessTokenLifetime").Value, CultureInfo.InvariantCulture);
            refreshTokenLifetime = TimeSpan.Parse(_configuration.GetSection("JWTConfig").GetSection("RefreshTokenLifetime").Value, CultureInfo.InvariantCulture);
            tokenSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWTConfig").GetSection("TokenSecret").Value));
        }
        private string CreateAccessToken(User user, Guid jti)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Typ, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jti.ToString())
            };

            var creds = new SigningCredentials(tokenSecurityKey, SecurityAlgorithms.HmacSha512);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddTicks(accessTokenLifetime.Ticks),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public Dictionary<string, string> GetClaims(ClaimsPrincipal userHttpContext)
        {
            Dictionary<string, string> claimList = new Dictionary<string, string>(3);

            claimList.Add("userName", userHttpContext?.FindFirstValue(ClaimTypes.Name));
            claimList.Add("userId", userHttpContext?.FindFirstValue(ClaimTypes.NameIdentifier));
            claimList.Add("role", userHttpContext?.FindFirstValue(JwtRegisteredClaimNames.Typ));
            claimList.Add("jwtId", userHttpContext?.FindFirstValue(JwtRegisteredClaimNames.Jti));

            return claimList;
        }

        public bool isUserAdmin(ClaimsPrincipal userHttpContext)
        {
            if (userHttpContext?.FindFirstValue(JwtRegisteredClaimNames.Typ) == UserRoles.Administrator.ToString())
            {
                return true;
            }

            return false;
        }

        private ClaimsPrincipal GetPrincipalFromToken(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                
                var principal =
                    tokenHandler.ValidateToken(accessToken, _tokenValidationParameters, out var validatedToken);

                _tokenValidationParameters.ValidateLifetime = true;

                if (!isJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool isJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return (validatedToken is JwtSecurityToken jwtSecurityToken) &&
                    jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512,
                        StringComparison.InvariantCultureIgnoreCase);
        }

        private string CreateNewRefreshToken(Guid userId, Guid jti, Guid? parentId)
        {
            string ip = _httpContextAccessor.HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();

            var refreshToken = new RefreshToken()
            {
                IAT = DateTime.UtcNow,
                UserId = userId,
                ParentRefreshTokenId = parentId,
                ExpirationDateTime = DateTime.UtcNow.AddTicks(refreshTokenLifetime.Ticks),
                RefreshJwtToken = $"{Guid.NewGuid()}.{Guid.NewGuid()}.{Guid.NewGuid()}.{Guid.NewGuid()}",
                JwtId = jti,
                IPAddress = ip
            };

            _tokenRepository.Create(refreshToken);

            return refreshToken.RefreshJwtToken;

        }

        public TokenAuthDTO Refresh(TokenAuthDTO authDto)
        {
            var validatedToken = GetPrincipalFromToken(authDto.AccessToken);

            if (validatedToken == null)
            {
                throw new InvalidRefreshTokenException("Invalid access token.");
            }

            var expirationDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expirationDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                throw new InvalidRefreshTokenException("The provided access token hasnt expired yet.");
            }

            var accessTokenJti = validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            var storedRefreshToken = _tokenRepository.GetToken(authDto.RefreshToken);

            if (accessTokenJti != storedRefreshToken.JwtId.ToString())
            {
                throw new InvalidRefreshTokenException("This refresh token doesnt match the access token.");
            }

            DetectRefreshTokenReuse(storedRefreshToken);

            if (DateTime.UtcNow > storedRefreshToken.ExpirationDateTime)
            {
                throw new InvalidRefreshTokenException("This refresh token has expired.");
            }


            TokenAuthDTO newTokenPair = IssueNewTokenPair(storedRefreshToken.User,
                storedRefreshToken.ParentRefreshTokenId.HasValue
                    ? storedRefreshToken.ParentRefreshTokenId
                    : storedRefreshToken.Id);


            _tokenRepository.PopTokenSequence(storedRefreshToken.Id, SEQUENCE_THRESHOLD);

            return newTokenPair;
        }

        private void DetectRefreshTokenReuse(RefreshToken refreshToken)
        {
            bool isRotated = _tokenRepository.IsTokenAlreadyRotated(refreshToken.Id);

            if (isRotated)
            {
                _tokenRepository.DeleteTokenSequence(refreshToken.ParentRefreshTokenId.HasValue ? refreshToken.ParentRefreshTokenId.Value : refreshToken.Id);

                throw new InvalidRefreshTokenException("The provided refresh token has been already used.");
            }
        }

        public TokenAuthDTO IssueNewTokenPair(User user, Guid? parentId = null)
        {
            Guid jti = Guid.NewGuid();

            return new TokenAuthDTO()
            {
                AccessToken = CreateAccessToken(user, jti),
                RefreshToken = CreateNewRefreshToken(user.Id, jti, parentId)
            };
        }

        private void InvalidateToken(RefreshToken refreshToken)
        {
            refreshToken.ExpirationDateTime = DateTime.UtcNow.Subtract(new TimeSpan(0,10,0));

            _tokenRepository.Save(refreshToken);
        }

        public void RevokeRefreshToken(string refreshToken, User user)
        {
            var storedRefreshToken = _tokenRepository.GetToken(refreshToken);

            if (storedRefreshToken.User.Id != user.Id)
            {
                throw new InvalidRefreshTokenException("This refresh token doesnt match the user sending the request.");
            }

            if (_tokenRepository.IsTokenAlreadyRotated(storedRefreshToken.Id))
            {
                throw new InvalidRefreshTokenException("This refresh token is already rotated.");
            }

            InvalidateToken(storedRefreshToken);
        }
    }
}

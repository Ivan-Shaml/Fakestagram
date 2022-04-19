using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Fakestagram.Data.DTOs.Tokens;
using Fakestagram.Data.Repositories.Contracts;

namespace Fakestagram.Services.Providers
{
    public class JwtAuthProvider : IAuthProvider
    {
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenRepository _tokenRepository;
        private readonly TimeSpan accessTokenLifetime;
        private readonly TimeSpan refreshTokenLifetime;
        private readonly SymmetricSecurityKey tokenSecurityKey;

        public JwtAuthProvider(IConfiguration configuration, IRefreshTokenRepository tokenRepository)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;
            accessTokenLifetime = TimeSpan.Parse(_configuration.GetSection("JWTConfig").GetSection("AccessTokenLifetime").Value);
            refreshTokenLifetime = TimeSpan.Parse(_configuration.GetSection("JWTConfig").GetSection("RefreshTokenLifetime").Value);
            tokenSecurityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("JWTConfig").GetSection("TokenSecret").Value));
        }
        public string CreateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var creds = new SigningCredentials(tokenSecurityKey, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddTicks(accessTokenLifetime.Ticks),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public Dictionary<string, string> GetClaims(ClaimsPrincipal userHttpContext)
        {
            Dictionary<string, string> claimList = new Dictionary<string, string>(3);

            claimList.Add("userName", userHttpContext?.FindFirst(ClaimTypes.Name).Value);
            claimList.Add("userId", userHttpContext?.FindFirst(ClaimTypes.NameIdentifier).Value);
            claimList.Add("role", userHttpContext?.FindFirst(ClaimTypes.Role).Value);

            return claimList;
        }

        public bool isUserAdmin(ClaimsPrincipal userHttpContext)
        {
            if (userHttpContext?.FindFirst(ClaimTypes.Role).Value == UserRoles.Administrator.ToString())
            {
                return true;
            }

            return false;
        }

        public string CreateRefreshToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        public TokenAuthDTO IssueTokenPair(TokenAuthDTO authDto)
        {
            throw new NotImplementedException();
        }
    }
}

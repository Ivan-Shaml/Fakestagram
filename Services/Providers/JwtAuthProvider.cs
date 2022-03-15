using Fakestagram.Models;
using Fakestagram.Services.Contracts;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Fakestagram.Services.Providers
{
    public class JwtAuthProvider : IAuthProvider
    {
        private readonly IConfiguration _configuration;

        public JwtAuthProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
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
    }
}

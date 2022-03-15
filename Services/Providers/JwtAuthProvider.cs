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

        public List<Claim> GetClaims(string token)
        {
            throw new NotImplementedException();
        }

        public bool isUserAdmin(string token)
        {
            throw new NotImplementedException();
        }

        public bool VerifyToken(string token)
        {
            throw new NotImplementedException();
        }
    }
}

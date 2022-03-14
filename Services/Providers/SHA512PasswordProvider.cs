using Fakestagram.Services.Contracts;
using System.Security.Cryptography;

namespace Fakestagram.Services.Providers
{
    public class SHA512PasswordProvider : IPasswordProvider
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
            }
        }

        public bool VerifyPasswordHash(string plaintextPassword, string passwordHash, string passwordSalt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(passwordSalt)))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(plaintextPassword));
                return computedHash.SequenceEqual(Convert.FromBase64String(passwordHash));
            }
        }
    }
}

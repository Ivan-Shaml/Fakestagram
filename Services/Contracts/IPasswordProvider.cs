namespace Fakestagram.Services.Contracts
{
    public interface IPasswordProvider
    {
        bool VerifyPasswordHash(string plaintextPassword, string passwordHash, string passwordSalt);
        void CreatePasswordHash(string password, out string passwordHash, out string passwordSalt);
    }
}

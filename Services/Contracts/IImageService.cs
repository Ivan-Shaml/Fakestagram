namespace Fakestagram.Services.Contracts
{
    public interface IImageService
    {
        string BuildPath(string directory, string filePath);
        void SaveFile(IFormFile file, string filePath);
        void DeleteFile(string filePath);
    }
}

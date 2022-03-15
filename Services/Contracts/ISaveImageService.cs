namespace Fakestagram.Services.Contracts
{
    public interface ISaveImageService
    {
        string BuildPath(string directory, string filePath);
        void SaveFile(IFormFile file, string filePath);
    }
}

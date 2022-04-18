namespace Fakestagram.Services.Contracts;

public interface IOffSiteBackupProvider
{
    Task UploadFileAsync(IFormFile file, string workingDirectory, string newFileName);
    void CreateDirectory(string workingDirectory);
    void DeleteFile(string filePath);
}
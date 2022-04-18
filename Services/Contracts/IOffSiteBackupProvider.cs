namespace Fakestagram.Services.Contracts;

public interface IOffSiteBackupProvider
{
    Task UploadFileAsync(IFormFile file, string workingDirectory, string newFileName);
    void DeleteFile(string filePath);
}
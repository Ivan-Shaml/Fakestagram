using Fakestagram.Services.Contracts;

namespace Fakestagram.Services.Helpers
{
    public class ImageService : IImageService
    {
        public string BuildPath(string directory, string filePath)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, filePath);
        }

        public void DeleteFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified file was not found.");
            }
            File.Delete(filePath);
        }

        public void SaveFile(IFormFile file, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(fs);
            }
        }
    }
}

using Fakestagram.Services.Contracts;

namespace Fakestagram.Services.Helpers
{
    public class SaveImageService : ISaveImageService
    {
        public string BuildPath(string directory, string filePath)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            return Path.Combine(directory, filePath);
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

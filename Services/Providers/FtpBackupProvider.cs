using System.Diagnostics;
using System.Net;
using Fakestagram.Services.Contracts;

namespace Fakestagram.Services.Providers
{
    public class FtpBackupProvider : IOffSiteBackupProvider
    {
        private readonly IConfiguration _confirugration;
        private readonly NetworkCredential ftpCreds;
        private readonly string ftpServerUrl;

        public FtpBackupProvider(IConfiguration configuration)
        {
            _confirugration = configuration;

            string userName = _confirugration.GetSection("FTPConfig").GetSection("userName").Value;
            string password = _confirugration.GetSection("FTPConfig").GetSection("password").Value;

            ftpServerUrl = _confirugration.GetSection("FTPConfig").GetSection("url").Value;
            ftpCreds = new NetworkCredential(userName, password);
        }

        private void CreateDirectory(string workingDirectory)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{workingDirectory}/");

            request.Method = WebRequestMethods.Ftp.MakeDirectory;

            request.Credentials = ftpCreds;

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Debug.WriteLine($"Mkdir operation completed with status code {(int)response.StatusCode}");
        }

        public async Task UploadFileAsync(IFormFile file, string workingDirectory, string newFileName)
        {
            try
            {
                CreateDirectory(workingDirectory);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{workingDirectory}/{newFileName}");

            request.Method = WebRequestMethods.Ftp.UploadFile;

            request.Credentials = ftpCreds;

            await using (Stream requestStream = request.GetRequestStream())
            {
                await file.CopyToAsync(requestStream);
            }

            using FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Debug.WriteLine($"Upload file operation completed with status code {(int)response.StatusCode}");
        }

        public void DeleteFile(string filePath)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{filePath.Substring(filePath.IndexOf('/') + 1)}");

            request.Method = WebRequestMethods.Ftp.DeleteFile;

            request.Credentials = ftpCreds;
            try
            {
                using FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Debug.WriteLine($"Delete file operation completed with status code {(int)response.StatusCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class ShareFileUploader : IBackupUploader
    {
        private readonly string _uploadUrl;
        private readonly string _authToken;

        public ShareFileUploader(string uploadUrl, string authToken)
        {
            //here we have to pass the creditials
            _uploadUrl = uploadUrl;
            _authToken = authToken;
        }

        public async Task UploadAsync(string localZipPath)
        {
            if (!File.Exists(localZipPath))
            {
                Console.WriteLine("❌ ZIP file not found.");
                return;
            }

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _authToken);

            using var content = new MultipartFormDataContent();
            var fileStream = File.OpenRead(localZipPath);
            content.Add(new StreamContent(fileStream), "file", Path.GetFileName(localZipPath));

            var response = await client.PostAsync(_uploadUrl, content);
            response.EnsureSuccessStatusCode();

            Console.WriteLine("✅ ShareFile upload complete.");
        }
    }
}

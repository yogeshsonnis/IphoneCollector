using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class AzureUploader : IBackupUploader
    {
        private readonly BlobContainerClient _containerClient;

        public AzureUploader(string connectionString, string containerName)
        {
            _containerClient = new BlobContainerClient(connectionString, containerName);
            _containerClient.CreateIfNotExists();
        }

        public async Task UploadAsync(string localZipPath)
        {
            if (!File.Exists(localZipPath))
            {
                Console.WriteLine("❌ ZIP file not found.");
                return;
            }

            var fileName = Path.GetFileName(localZipPath);
            var blobClient = _containerClient.GetBlobClient($"iPhoneBackups/{DateTime.Now:yyyyMMdd_HHmmss}/{fileName}");

            using var fileStream = File.OpenRead(localZipPath);
            await blobClient.UploadAsync(fileStream, overwrite: true);

            Console.WriteLine("✅ Azure upload complete.");
        }
    }
}

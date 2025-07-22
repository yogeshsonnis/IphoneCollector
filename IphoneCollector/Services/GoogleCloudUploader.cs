using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;

namespace IphoneCollector.Services
{
    public class GoogleCloudUploader : IBackupUploader
    {
        private readonly string _bucketName;
        private readonly StorageClient _storageClient;

        public GoogleCloudUploader(string bucketName, string serviceAccountJsonPath)
        {
            _bucketName = bucketName;

            // Use the JSON file to authenticate
            var credentials = GoogleCredential.FromFile(serviceAccountJsonPath);
            _storageClient = StorageClient.Create(credentials);
        }

        public async Task UploadAsync(string localZipPath)
        {
            if (!File.Exists(localZipPath))
            {
                Console.WriteLine("❌ ZIP file not found.");
                return;
            }

            var fileName = Path.GetFileName(localZipPath);

            using var fileStream = File.OpenRead(localZipPath);

            Console.WriteLine($"📤 Uploading ZIP to GCS: {fileName}");

            await _storageClient.UploadObjectAsync(
                bucket: _bucketName,
                objectName: $"iPhoneBackups/{DateTime.Now:yyyyMMdd_HHmmss}/{fileName}",
                contentType: "application/zip",
                source: fileStream
            );

            Console.WriteLine("✅ GCS upload complete.");
        }
    }
}
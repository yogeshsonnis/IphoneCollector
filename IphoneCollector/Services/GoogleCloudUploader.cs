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
            var credentials = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile(serviceAccountJsonPath);
            _storageClient = StorageClient.Create(credentials);
        }

        public async Task UploadAsync(string localBackupPath)
        {
            if (!Directory.Exists(localBackupPath))
            {
                Console.WriteLine("Backup folder does not exist.");
                return;
            }

            var files = Directory.GetFiles(localBackupPath, "*.*", SearchOption.AllDirectories);
            foreach (var filePath in files)
            {
                var relativePath = Path.GetRelativePath(localBackupPath, filePath).Replace("\\", "/");
                using var fileStream = File.OpenRead(filePath);

                Console.WriteLine($"Uploading: {relativePath}");

                await _storageClient.UploadObjectAsync(
                    bucket: _bucketName,
                    objectName: relativePath,
                    contentType: "application/octet-stream",
                    source: fileStream
                );
            }

            Console.WriteLine("✅ Google Cloud upload complete.");
        }
    }
}
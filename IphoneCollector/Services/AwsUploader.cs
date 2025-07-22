using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class AwsUploader : IBackupUploader
    {
        private readonly string _bucketName;
        private readonly IAmazonS3 _s3Client;

        public AwsUploader(string bucketName, string accessKey, string secretKey, string region)
        {
            _bucketName = bucketName;
            _s3Client = new AmazonS3Client(accessKey, secretKey, Amazon.RegionEndpoint.GetBySystemName(region));
        }

        public async Task UploadAsync(string localZipPath)
        {
            if (!File.Exists(localZipPath))
            {
                Console.WriteLine("❌ ZIP file not found.");
                return;
            }

            var fileName = Path.GetFileName(localZipPath);
            var key = $"iPhoneBackups/{DateTime.Now:yyyyMMdd_HHmmss}/{fileName}";

            using var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(localZipPath, _bucketName, key);

            Console.WriteLine("✅ AWS S3 upload complete.");
        }
    }
}
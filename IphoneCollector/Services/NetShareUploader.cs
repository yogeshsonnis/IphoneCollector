using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class NetShareUploader : IBackupUploader
    {
        private readonly string _networkPath;

        public NetShareUploader(string networkPath)
        {
            if (!Directory.Exists(networkPath))
                throw new DirectoryNotFoundException($"Network path not found: {networkPath}");

            _networkPath = networkPath;
        }

        public async Task UploadAsync(string localZipPath)
        {
            if (!File.Exists(localZipPath))
            {
                Console.WriteLine("❌ ZIP file not found.");
                return;
            }

            var timestampFolder = Path.Combine(_networkPath, "iPhoneBackups", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(timestampFolder);

            var destPath = Path.Combine(timestampFolder, Path.GetFileName(localZipPath));
            await Task.Run(() => File.Copy(localZipPath, destPath, overwrite: true));

            Console.WriteLine("✅ Network share upload complete.");
        }
    }
}

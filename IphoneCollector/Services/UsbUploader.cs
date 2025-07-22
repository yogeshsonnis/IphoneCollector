using System;
using System.IO;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class UsbUploader : IBackupUploader
    {
        private readonly string _usbDrivePath;

        public UsbUploader(string usbDrivePath)
        {
            if (string.IsNullOrWhiteSpace(usbDrivePath) || !Directory.Exists(usbDrivePath))
                throw new DirectoryNotFoundException($"USB path not found: {usbDrivePath}");

            _usbDrivePath = usbDrivePath;
        }

        public async Task UploadAsync(string localBackupZipPath)
        {
            if (!File.Exists(localBackupZipPath))
            {
                Console.WriteLine("❌ Backup ZIP file does not exist.");
                return;
            }

            string timestampFolder = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string destinationDir = Path.Combine(_usbDrivePath, "iPhoneBackups", timestampFolder);
            Directory.CreateDirectory(destinationDir);

            string destFile = Path.Combine(destinationDir, Path.GetFileName(localBackupZipPath));

            Console.WriteLine($"📁 Copying ZIP to USB: {destFile}");

            await Task.Run(() =>
            {
                File.Copy(localBackupZipPath, destFile, overwrite: true);
            });

            Console.WriteLine("✅ USB ZIP upload complete.");
        }
    }
}

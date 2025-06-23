using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task UploadAsync(string localBackupPath)
        {
            if (!Directory.Exists(localBackupPath))
            {
                Console.WriteLine("Backup folder does not exist.");
                return;
            }

            var destinationPath = Path.Combine(_usbDrivePath, "iPhoneBackups", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(destinationPath);

            Console.WriteLine($"Copying to USB: {destinationPath}");

            await Task.Run(() => CopyDirectory(localBackupPath, destinationPath));

            Console.WriteLine("✅ USB upload (copy) complete.");
        }

        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dir.Replace(sourceDir, destinationDir));
            }

            foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                var destFile = file.Replace(sourceDir, destinationDir);
                File.Copy(file, destFile, overwrite: true);
            }
        }
    }
}

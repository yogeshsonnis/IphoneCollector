using IphoneCollector.Helper;
using IphoneCollector.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public class AndroidDeviceService
    {
        string backupRoot = Path.Combine(AppContext.BaseDirectory, "Backup");
        private string GetAdbPath()
        {
            return Path.Combine(AppContext.BaseDirectory, "Tools", "adb", "adb.exe");
        }

        public ConnectedDevice GetConnectedDevice()
        {
            var adbPath = GetAdbPath();

            //var output = RunAdbCommand(GetAdbPath());
            //Console.WriteLine(output);

            if (!File.Exists(adbPath))
                throw new FileNotFoundException("ADB not found at: " + adbPath);

            string deviceId = RunAdbCommand("get-serialno").Trim();
            if (string.IsNullOrWhiteSpace(deviceId) || deviceId == "unknown")
                return null; // No device

            var device = new ConnectedDevice
            {
                UniqueDeviceID = RunAdbCommand("get-serialno").Trim(),
                ProductType = RunAdbCommand("shell getprop ro.product.model").Trim(),
                DeviceName = RunAdbCommand("shell getprop ro.product.device").Trim(),
                ComponyName = RunAdbCommand("shell getprop ro.product.manufacturer").Trim(),
                Platform = ConnectedDevice.MobilePlatform.Android
            };

            return device;
        }

        private string RunAdbCommand(string arguments)
        {
            var psi = new ProcessStartInfo
            {
                FileName = GetAdbPath(),
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);
            return process?.StandardOutput.ReadToEnd() ?? string.Empty;
        }

            
        public void AutoBackupUserFiles(List<string> desiredFolders)
        {
            var folders = GetSdcardFolders();

            Directory.CreateDirectory(backupRoot);

            //string[] desiredFolders = new[] {
            //    "DCIM", "Pictures", "Download", "Documents", "Music", "Movies", "WhatsApp"
            //};


            foreach (var folder in folders)
            {
                if (!desiredFolders.Contains(folder, StringComparer.OrdinalIgnoreCase))
                    continue;

                string sourcePath = $"/sdcard/{folder}";
                string localTarget = Path.Combine(backupRoot, folder);
                Directory.CreateDirectory(localTarget);

                Debug.WriteLine($"📥 Pulling {sourcePath}...");
                RunAdbCommand($"pull {sourcePath} \"{localTarget}\"");
            }

            Debug.WriteLine("✅ All selected folders pulled.");
        }


        public List<string> GetSdcardFolders()
        {
            string output = RunAdbCommand("shell ls /sdcard");
            var folders = output.Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                .Select(f => f.Trim())
                                .Where(f => !string.IsNullOrWhiteSpace(f))
                                .ToList();
            return folders;
        }

        public void CompressBackup()
        {
            string backupDir = Path.Combine(AppContext.BaseDirectory, "Backup");
            string zipPath = Path.Combine(AppContext.BaseDirectory, "Backup.zip");

            if (File.Exists(zipPath))
                File.Delete(zipPath);

            ZipFile.CreateFromDirectory(backupDir, zipPath);
            Debug.WriteLine("📦 Backup compressed to Backup.zip");
        }


        public async Task UploadToAllPlatforms()
        {
            string localBackupPath = backupRoot;
            string bucketName = "your-gcs-bucket";
            string gcpJsonKeyPath = @"C:\Secrets\your-service-account.json";

            var uploaders = new List<IBackupUploader>
            {
                //new GoogleCloudUploader(bucketName, gcpJsonKeyPath)
            };

            string? usbPath = UsbHelper.GetFirstUsbDrive();

            if (!string.IsNullOrEmpty(usbPath))
            {
                uploaders.Add(new UsbUploader(usbPath));
            }
            else
            {
                Console.WriteLine("⚠️ No USB drive detected. Skipping USB upload.");
            }
            //var uploaders = new List<IBackupUploader>
            //{
            //    new GoogleCloudUploader(bucketName, gcpJsonKeyPath),
            //    new UsbUploader(usbPath),

            //    // new AwsUploader(...), // ← Future expansion
            //    // new AzureUploader(...),
            //    // new UsbUploader(...),
            //};

            foreach (var uploader in uploaders)
            {
                try
                {
                    await uploader.UploadAsync(localBackupPath);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"❌ Upload failed: {ex.Message}");
                }
            }
        }
    }
}

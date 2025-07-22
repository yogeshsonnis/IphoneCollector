using iMobileDevice.iDevice;
using iMobileDevice;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iMobileDevice.Lockdown;
using IphoneCollector.MVVM.Model;
using System.Diagnostics;
using static IphoneCollector.MVVM.Model.ConnectedDevice;
using IphoneCollector.Helper;
using System.Text.RegularExpressions;
using System.IO.Compression;

namespace IphoneCollector.Services
{
    public class IOSDeviceService
    {
        public List<string> ConnectedDeviceNames { get; private set; } = new();
        //private readonly string toolPath = Path.Combine(AppContext.BaseDirectory, "Tools", "idevicebackup2.exe");
        private readonly string toolPath = Path.Combine(AppContext.BaseDirectory, "Tools", "idevice", "idevicebackup2.exe");

        private readonly string _backupOutputPath = Path.Combine(AppContext.BaseDirectory, "iPhoneBackup", DateTime.Now.ToString("yyyyMMdd_HHmmss"));

        public string EstimatedTimeDisplay { get; private set; } = "Calculating...";

        public string? udid { get; set; }

        public ConnectedDevice GetConnectedDevice()
        {
            ReadOnlyCollection<string> udids;
            int count = 0;

            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;

            var ret = idevice.idevice_get_device_list(out udids, ref count);

            if (ret == iDeviceError.NoDevice || udids == null || udids.Count == 0)
            {
                return null;
            }

            ret.ThrowOnError();

            udid = udids[0]; // Return the first connected device (similar to ADB behavior)

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();

            LockdownClientHandle lockdownHandle;
            lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "Quamotion").ThrowOnError();


            lockdown.lockdownd_get_device_name(lockdownHandle, out string deviceName).ThrowOnError();
            lockdown.lockdownd_get_value(lockdownHandle, null, "ProductVersion", out var versionPlist).ThrowOnError();
            lockdown.lockdownd_get_value(lockdownHandle, null, "SerialNumber", out var serialPlist).ThrowOnError();
            lockdown.lockdownd_get_value(lockdownHandle, null, "ProductType", out var typePlist).ThrowOnError();

            var connectedDevice = new ConnectedDevice
            {
                UniqueDeviceID = udid,
                DeviceName = deviceName,
                SerialNumber = serialPlist?.ToString(),
                ProductVersion = versionPlist?.ToString(),
                ProductType = typePlist?.ToString(),
                ComponyName = "Apple",
                Platform = MobilePlatform.Ios
            };

            deviceHandle.Dispose();
            lockdownHandle.Dispose();

            return connectedDevice;
        }

        private string GetUniqueBackupPath(string deviceName, string storageLocation)
        {
            string baseBackupPath = Path.Combine(storageLocation, "Backup");
            Directory.CreateDirectory(baseBackupPath);

            string deviceFolderPath = Path.Combine(baseBackupPath, deviceName);
            int count = 1;

            while (Directory.Exists(deviceFolderPath))
            {
                deviceFolderPath = Path.Combine(baseBackupPath, $"{deviceName}_{count}");
                count++;
            }

            string finalPath = Path.Combine(deviceFolderPath, DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            Directory.CreateDirectory(finalPath);

            return finalPath;
        }

        //public async Task<bool> TriggerBackupAsync(string deviceName, IProgress<int> progress, string encryptionPassword)
        //{
        //    try
        //    {
        //        if (!File.Exists(toolPath))
        //        {
        //            Debug.WriteLine("Backup tool not found.");
        //            return false;
        //        }

        //        string backupOutputPath = GetUniqueBackupPath(deviceName);

        //        string arguments;

        //        if (!string.IsNullOrEmpty(encryptionPassword))
        //        {
        //            arguments = $"backup --password \"{encryptionPassword}\" \"{backupOutputPath}\"";
        //        }
        //        else
        //        {
        //            arguments = $"backup \"{backupOutputPath}\"";
        //        }

        //        //var processStartInfo = new ProcessStartInfo
        //        //{
        //        //    FileName = toolPath,
        //        //    Arguments = $"backup \"{backupOutputPath}\"",
        //        //    RedirectStandardOutput = true,
        //        //    RedirectStandardError = true,
        //        //    UseShellExecute = false,
        //        //    CreateNoWindow = true
        //        //};



        //        var processStartInfo = new ProcessStartInfo
        //        {
        //            FileName = toolPath,
        //            Arguments = arguments,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };


        //        //Arguments = $"backup --password " + "123456" + "\"{backupOutputPath}\"",
        //        using var process = new Process { StartInfo = processStartInfo };
        //        process.Start();


        //        var outputTask = Task.Run(async () =>
        //        {
        //            while (!process.StandardOutput.EndOfStream)
        //            {
        //                var line = await process.StandardOutput.ReadLineAsync();
        //                // Debug.WriteLine("Line Output => "+line);


        //                int percent = ParseProgressFromLine(line);
        //                if (percent >= 0)
        //                {
        //                    progress?.Report(percent);
        //                }
        //            }
        //        });

        //        var errorTask = Task.Run(async () =>
        //        {
        //            while (!process.StandardError.EndOfStream)
        //            {
        //                var line = await process.StandardError.ReadLineAsync();
        //                Debug.WriteLine("ERROR: " + line);
        //            }
        //        });

        //        await Task.WhenAll(outputTask, errorTask);
        //        process.WaitForExit();

        //        return process.ExitCode == 0;

        //        //string output = await process.StandardOutput.ReadToEndAsync();
        //        //string error = await process.StandardError.ReadToEndAsync();

        //        //process.WaitForExit();

        //        ////Debug.WriteLine("Backup Output:\n" + output);
        //        //return process.ExitCode == 0;

        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine("Backup failed: " + ex.Message);
        //        return false;
        //    }
        //}


        public async Task<bool> TriggerBackupAsync(string deviceName, IProgress<BackupProgressInfo> progress, string encryptionPassword, string storageLocation)
        {
            try
            {
                if (!File.Exists(toolPath))
                {
                    Debug.WriteLine("Backup tool not found.");
                    return false;
                }

                string backupOutputPath = GetUniqueBackupPath(deviceName, storageLocation);

                // Step 1: Estimate files to be backed up
                int estimatedFiles = await EstimateTotalFilesToBackupAsync();

                // Step 2: Check encryption state
                bool encryptionEnabled = await IsBackupEncryptionEnabledAsync();
                string arguments;

                if (encryptionEnabled)
                {
                    await App.Current.MainPage.DisplayAlert("Password Required", "This device has encrypted backups enabled. Please enter the backup password in Your Device", "OK");
                    arguments = $"backup --password \"{encryptionPassword}\" \"{backupOutputPath}\"";
                }
                else
                {
                    if (!string.IsNullOrEmpty(encryptionPassword))
                    {
                        await App.Current.MainPage.DisplayAlert("Enabling Encryption", "Device does not have encryption enabled. Enabling now...", "OK");
                        bool encryptionSet = await EnableBackupEncryptionAsync(udid, encryptionPassword);

                        if (!encryptionSet)
                        {
                            await App.Current.MainPage.DisplayAlert("Encryption Failed", "Failed to enable backup encryption. Backup canceled.", "OK");
                            return false;
                        }

                        await App.Current.MainPage.DisplayAlert("Encryption Enabled", "✅ Backup encryption enabled successfully.", "OK");
                        arguments = $"backup --password \"{encryptionPassword}\" \"{backupOutputPath}\"";
                    }
                    else
                    {
                        Debug.WriteLine("⚠️ Encryption is OFF and no password provided. Backup will be unencrypted.");
                        await App.Current.MainPage.DisplayAlert("Unencrypted Backup", "No encryption password provided. Backup will be created without encryption.", "OK");
                        arguments = $"backup \"{backupOutputPath}\"";
                    }
                }

                // Step 3: Start backup
                var processStartInfo = new ProcessStartInfo
                {
                    FileName = toolPath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = processStartInfo };
                process.Start();

                // Step 4: Real-time tracking task
                var speedTrackingTask = Task.Run(async () =>
                {
                    long lastSize = 0;
                    DateTime lastTime = DateTime.Now;

                    while (!process.HasExited)
                    {
                        long currentSize = GetDirectorySize(backupOutputPath);
                        int fileCount = GetFileCount(backupOutputPath);
                        DateTime currentTime = DateTime.Now;

                        double elapsedSeconds = (currentTime - lastTime).TotalSeconds;
                        double speedMBps = elapsedSeconds > 0 ? (currentSize - lastSize) / 1024.0 / 1024.0 / elapsedSeconds : 0;

                        int percent = estimatedFiles > 0 ? (int)((fileCount / (double)estimatedFiles) * 100) : 0;

                        progress?.Report(new BackupProgressInfo
                        {
                            Percent = percent,
                            Size = ConvertSizeToReadable(currentSize),
                            FilesWritten = fileCount,
                            EstimatedFiles = estimatedFiles,
                            SpeedMBps = speedMBps
                        });

                        lastSize = currentSize;
                        lastTime = currentTime;
                        await Task.Delay(1000);
                    }
                });

                // Step 5: Output parsing task
                var outputTask = Task.Run(async () =>
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = await process.StandardOutput.ReadLineAsync();

                        if (line.Contains("ErrorCode 105", StringComparison.OrdinalIgnoreCase))
                        {
                            MainThread.BeginInvokeOnMainThread(async () =>
                            {
                                await App.Current.MainPage.DisplayAlert("Backup Failed", "Insufficient Space", "OK");
                            });
                            process.Kill();
                            break;
                        }
                    }
                });

                // Step 6: Error stream monitor
                var errorTask = Task.Run(async () =>
                {
                    while (!process.StandardError.EndOfStream)
                    {
                        var line = await process.StandardError.ReadLineAsync();
                        Debug.WriteLine("ERROR: " + line);
                    }
                });

                await Task.WhenAll(outputTask, errorTask, speedTrackingTask);
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Failed", "iPhone Backup failed.", "OK");
                Debug.WriteLine("Backup failed: " + ex.Message);
                return false;
            }
        }


        private async Task<int> EstimateTotalFilesToBackupAsync()
        {
            int count = 0;
            try
            {
                var listProcess = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = toolPath,
                        Arguments = "list",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                listProcess.Start();

                while (!listProcess.StandardOutput.EndOfStream)
                {
                    var line = await listProcess.StandardOutput.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line))
                        count++;
                }

                listProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("EstimateTotalFilesToBackupAsync failed: " + ex.Message);
            }

            return count;
        }


        //private long GetDirectorySize(string path)
        //{
        //    return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
        //        .Sum(file => new FileInfo(file).Length);
        //}


        //private int GetFileCount(string path)
        //{
        //    return Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories).Count();
        //}

        private long GetDirectorySize(string path)
        {
            long totalSize = 0;
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    totalSize += new FileInfo(file).Length;
                }
                catch (FileNotFoundException)
                {
                    // File disappeared between enumeration and reading; skip it safely
                    Debug.WriteLine($"Skipped missing file during size calc: {file}");
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Skipped inaccessible file during size calc: {file}");
                }
            }
            return totalSize;
        }


        private int GetFileCount(string path)
        {
            int count = 0;
            foreach (var file in Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories))
            {
                try
                {
                    // Accessing file to catch potential FileNotFound
                    var _ = new FileInfo(file).Exists;
                    count++;
                }
                catch (FileNotFoundException)
                {
                    Debug.WriteLine($"Skipped missing file during count: {file}");
                }
                catch (UnauthorizedAccessException)
                {
                    Debug.WriteLine($"Skipped inaccessible file during count: {file}");
                }
            }
            return count;
        }


        private string ConvertSizeToReadable(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }



        public async Task<bool> EnableBackupEncryptionAsync(string udid, string newPassword)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "idevicebackup2",
                Arguments = $"encryption on \"{newPassword}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = psi };
            process.Start();

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            Debug.WriteLine("Encryption Set Output: " + output);
            Debug.WriteLine("Encryption Set Error: " + error);

            return process.ExitCode == 0; // Success if exit code is 0
        }


        private async Task<bool> IsBackupEncryptionEnabledAsync()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = toolPath, // adjust this path
                    Arguments = "-k BackupKeyBag",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = psi };
                process.Start();

                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                process.WaitForExit();

                // If output is not empty, encryption is ON
                return !string.IsNullOrWhiteSpace(output);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking encryption: {ex.Message}");
                return false; // assume encryption is off if error occurs
            }
        }


        int currentPhaseIndex = 0;
        double currentPhasePercent = 0;
        int estimatedPhases = 5; // guess or set based on observation

        Stopwatch stopwatch = new Stopwatch();
        double lastEstimatedTotalSeconds = 0;

        int ParseProgressFromLine(string line)
        {
            var match = Regex.Match(line, @"\]\s+(\d+)%");
            if (match.Success)
            {
                int percent = int.Parse(match.Groups[1].Value);

                if (percent < currentPhasePercent && currentPhasePercent > 90)
                {
                    currentPhaseIndex++;
                    currentPhaseIndex = Math.Min(currentPhaseIndex, estimatedPhases - 1); // Prevent exceeding max phases
                }

                currentPhasePercent = percent;

                double totalProgress = (currentPhaseIndex + (percent / 100.0)) / estimatedPhases;
                int totalProgressPercent = (int)(totalProgress * 100);

                totalProgressPercent = Math.Min(totalProgressPercent, 100);

                if (!stopwatch.IsRunning)
                    stopwatch.Start();

                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;

                double progressFraction = totalProgressPercent / 100.0;

                if (progressFraction > 0.05 && elapsedSeconds > 0)
                {
                    double estimatedTotalSeconds = elapsedSeconds / progressFraction;
                    double remainingSeconds = estimatedTotalSeconds - elapsedSeconds;

                    lastEstimatedTotalSeconds = estimatedTotalSeconds;

                    TimeSpan remaining = TimeSpan.FromSeconds(remainingSeconds);
                    EstimatedTimeDisplay = remaining.ToString(@"mm\:ss");
                }


                return totalProgressPercent;
            }

            return -1;
        }


        public async Task UploadToAllPlatformsAsync(bool uploadToGCP, bool uploadToAWS, bool uploadToAzure, bool uploadToUSB)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string localBackupFolder = _backupOutputPath;
            string zipFileName = $"iPhoneBackup_{timestamp}.zip";
            string zipFilePath = Path.Combine(Path.GetTempPath(), zipFileName);

            // 1. Zip the backup
            if (File.Exists(zipFilePath))
                File.Delete(zipFilePath);

            ZipFile.CreateFromDirectory(localBackupFolder, zipFilePath, CompressionLevel.Optimal, includeBaseDirectory: false);

            var uploaders = new List<IBackupUploader>();

            if (uploadToGCP)
            {
                string gcpJsonPath = @"C:\Secrets\your-service-account.json";
                string bucket = "your-bucket";
                uploaders.Add(new GoogleCloudUploader(bucket, gcpJsonPath));
            }

            if (uploadToAWS)
            {
                string awsAccessKey = "your-access-key";
                string awsSecretKey = "your-secret-key";
                string region = "us-east-1";
                string bucketName = "your-aws-bucket";

                var awsUploader = new AwsUploader(awsAccessKey, awsSecretKey, region, bucketName);
                uploaders.Add(awsUploader);
            }

            // 📦 Azure Blob
            if (uploadToAzure)
            {
                string azureConnectionString = "your-azure-connection-string";
                string containerName = "your-container-name";

                var azureUploader = new AzureUploader(azureConnectionString, containerName);
                uploaders.Add(azureUploader);
            }

            // 💾 USB Drive
            if (uploadToUSB)
            {
                string? usbPath = UsbHelper.GetFirstUsbDrive();
                if (!string.IsNullOrEmpty(usbPath))
                {
                    uploaders.Add(new UsbUploader(usbPath));
                }
                else
                {
                    Console.WriteLine("⚠️ No USB drive detected.");
                }
            }

            // 🚀 Upload to All Enabled Platforms
            foreach (var uploader in uploaders)
            {
                try
                {
                    await uploader.UploadAsync(zipFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Upload failed: {ex.Message}");
                }
            }

            Console.WriteLine("✅ All uploads complete.");
        }



    }
}

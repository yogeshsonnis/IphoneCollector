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

        private string GetUniqueBackupPath(string deviceName,string storageLocation)
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


        public async Task<bool> TriggerBackupAsync(string deviceName, IProgress<int> progress, string encryptionPassword, string storageLocation)
        {
            try
            {
                if (!File.Exists(toolPath))
                {
                    Debug.WriteLine("Backup tool not found.");
                    return false;
                }

                string backupOutputPath = GetUniqueBackupPath(deviceName, storageLocation);

                // Check if device encryption is enabled
                bool encryptionEnabled = await IsBackupEncryptionEnabledAsync();

                string arguments;

                if (encryptionEnabled)
                {
                    // Encryption is ON and password is provided
                    arguments = $"backup --password \"{encryptionPassword}\" \"{backupOutputPath}\"";

                }
                else
                {
                    if (!string.IsNullOrEmpty(encryptionPassword))
                    {
                        Debug.WriteLine("ℹ️ Backup encryption is OFF — attempting to enable it.");

                        bool encryptionSet = await EnableBackupEncryptionAsync(udid, encryptionPassword);

                        if (!encryptionSet)
                        {
                            Debug.WriteLine("❌ Failed to enable backup encryption.");
                            return false;
                        }

                        Debug.WriteLine("✅ Backup encryption enabled successfully.");
                        // Now encryption is ON — Use password for backup
                        arguments = $"backup --password \"{encryptionPassword}\" \"{backupOutputPath}\"";
                    }
                    else
                    {
                        // Encryption OFF and no password to set — proceed without encryption
                        Debug.WriteLine("⚠️ Encryption is OFF and no password provided. Backup will be unencrypted.");
                        arguments = $"backup \"{backupOutputPath}\"";
                    }
                }

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

                var outputTask = Task.Run(async () =>
                {
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = await process.StandardOutput.ReadLineAsync();
                        int percent = ParseProgressFromLine(line);
                        if (percent >= 0)
                        {
                            progress?.Report(percent);
                            Debug.WriteLine("Line here: " + line);
                        }
                    }

                });

                var errorTask = Task.Run(async () =>
                {
                    while (!process.StandardError.EndOfStream)
                    {
                        var line = await process.StandardError.ReadLineAsync();
                        Debug.WriteLine("ERROR: " + line);
                    }
                });

                await Task.WhenAll(outputTask, errorTask);
                process.WaitForExit();

                return process.ExitCode == 0;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Backup failed: " + ex.Message);
                return false;
            }
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

                if (progressFraction > 0.05)
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


        public async Task UploadToAllPlatforms()
        {
            string localBackupPath = $"backup \"{_backupOutputPath}\"";
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

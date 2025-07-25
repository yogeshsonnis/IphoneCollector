﻿
using CommunityToolkit.Maui.Views;
using IphoneCollector.Data;
using IphoneCollector.Helper;
using IphoneCollector.MVVM.Model;
using IphoneCollector.MVVM.View;
using IphoneCollector.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;
using static IphoneCollector.MVVM.Model.ConnectedDevice;

namespace IphoneCollector.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        private readonly AndroidDeviceService _deviceService = new();

        private readonly IOSDeviceService _iosService = new();

        private readonly LocalDbService _dbServices;

        public string OSProfile { get; set; }

        private ConnectedDevice _connectedDevice;
        public ConnectedDevice ConnectedDevice
        {
            get => _connectedDevice;
            set
            {
                _connectedDevice = value;
                OnPropertyChanged();
            }
        }

        private ContentView _currentView;
        public ContentView CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
                IsStartCollectionView = _currentView is StartCollectionView;
            }
        }

        private bool _isStartCollectionView;
        public bool IsStartCollectionView
        {
            get => _isStartCollectionView;
            private set
            {
                if (_isStartCollectionView != value)
                {
                    _isStartCollectionView = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<string> SettingsOptions { get; set; } = new()
        {
            "Google",
            "AWS S3",
            "AWS Fed RAMP",
            "Azure",
            "ShareFile"
        };

        public ObservableCollection<ConnectedDevice> ConnectedDevicesList { get; set; } = new ObservableCollection<ConnectedDevice>();


        private ObservableCollection<SelectedFolder> _deviceFolders;
        public ObservableCollection<SelectedFolder> DeviceFolders
        {
            get => _deviceFolders;
            set
            {
                _deviceFolders = value;
                OnPropertyChanged();
            }
        }



        #region Propeties


        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(); // Make sure your ViewModel implements INotifyPropertyChanged
            }
        }

        private bool _isDeviceConnected;

        public bool IsDeviceConnected
        {
            get { return _isDeviceConnected; }
            set
            {
                _isDeviceConnected = value;
                OnPropertyChanged();
            }
        }



        private bool _isPickerVisible;
        public bool IsPickerVisible
        {
            get => _isPickerVisible;
            set
            {
                _isPickerVisible = value;
                OnPropertyChanged();
            }
        }

        private string _caseName;

        public string CaseName
        {
            get { return _caseName; }
            set
            {
                _caseName = value;
                OnPropertyChanged();
            }
        }

        private string _examinerName;

        public string ExaminerName
        {
            get { return _examinerName; }
            set
            {
                _examinerName = value;
                OnPropertyChanged();
            }
        }

        private string _matterNumber;

        public string MatterNumber
        {
            get { return _matterNumber; }
            set
            {
                _matterNumber = value;
                OnPropertyChanged();
            }
        }

        private string _storageLocation;

        public string StorageLocation
        {
            get { return _storageLocation; }
            set
            {
                _storageLocation = value;
                OnPropertyChanged();
                DataTransferredTo = _storageLocation;
            }
        }

        private string _selectedSettingOption;

        public string SelectedSettingOption
        {
            get { return _selectedSettingOption; }
            set
            {
                _selectedSettingOption = value;
                OnPropertyChanged();
            }
        }


        private bool _isGoogleCloudSelected;

        public bool IsGoogleCloudSelected
        {
            get { return _isGoogleCloudSelected; }
            set
            {
                _isGoogleCloudSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isAwsS3Selected;

        public bool IsAwsS3Selected
        {
            get { return _isAwsS3Selected; }
            set
            {
                _isAwsS3Selected = value;
                OnPropertyChanged();
            }
        }

        private bool _isAzureSelected;

        public bool IsAzureSelected
        {
            get { return _isAzureSelected; }
            set
            {
                _isAzureSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isNetShareSelected;

        public bool IsNetShareSelected
        {
            get { return _isNetShareSelected; }
            set
            {
                _isNetShareSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isShareFileSelected;

        public bool IsShareFileSelected
        {
            get { return _isShareFileSelected; }
            set
            {
                _isShareFileSelected = value;
                OnPropertyChanged();
            }
        }

        private bool _isUSBConnected;

        public bool IsUSBConnected
        {
            get { return _isUSBConnected; }
            set
            {
                _isUSBConnected = value;
                OnPropertyChanged();
            }
        }
        private string _deviceInfo;

        public string DeviceInfo
        {
            get { return _deviceInfo; }
            set
            {
                _deviceInfo = value;
                OnPropertyChanged();
            }
        }
        private string _deviceWarningMsg;

        public string DeviceWarningMsg
        {
            get { return _deviceWarningMsg; }
            set
            {
                _deviceWarningMsg = value;
                OnPropertyChanged();
            }
        }

        private string _estimatedTimeDisplay = "Estimated Time Remaining: Calculating...";

        public string EstimatedTimeDisplay
        {
            get { return _estimatedTimeDisplay; }
            set
            {
                _estimatedTimeDisplay = value;
                OnPropertyChanged();
            }
        }


        private string _encryptionPassword;

        public string EncryptionPassword
        {
            get { return _encryptionPassword; }
            set
            {
                _encryptionPassword = value;
                OnPropertyChanged();
            }
        }


        private string _backupSize;
        public string BackupSize
        {
            get => _backupSize;
            set
            {
                _backupSize = value;
                OnPropertyChanged();
            }
        }

        private int _filesWritten;
        public int FilesWritten
        {
            get => _filesWritten;
            set
            {
                _filesWritten = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilesProgressDisplay));
            }
        }

        private int _estimatedFiles;
        public int EstimatedFiles
        {
            get => _estimatedFiles;
            set
            {
                _estimatedFiles = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(FilesProgressDisplay));
            }
        }



        public string FilesProgressDisplay => $"{FilesWritten / 1000} / {EstimatedFiles} files";

        private double _transferSpeed;
        public double TransferSpeed
        {
            get => _transferSpeed;
            set
            {
                _transferSpeed = value;
                OnPropertyChanged();
            }
        }
        public string ProgressDisplay => $"{ProgressPercent:F0}% out of 100%";

        private double _progressPercent;
        public double ProgressPercent
        {
            get => _progressPercent;
            set
            {
                _progressPercent = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProgressDisplay));
                OnPropertyChanged(nameof(ProgressValue));

            }
        }

        public double ProgressValue => ProgressPercent / 100.0;


        private string _hardDriveInventory;

        public string HardDriveInventory
        {
            get { return _hardDriveInventory; }
            set
            {
                _hardDriveInventory = value;
                OnPropertyChanged();
            }
        }

        private string _hardDrivePreserved;

        public string HardDrivePreserved
        {
            get { return _hardDrivePreserved; }
            set
            {
                _hardDrivePreserved = value;
                OnPropertyChanged();
            }
        }

        private string _custodianName;

        public string CustodianName
        {
            get { return _custodianName; }
            set
            {
                _custodianName = value;
                OnPropertyChanged();
            }
        }

        private string _custodianEmail;
        public string CustodianEmail
        {
            get { return _custodianEmail; }
            set
            {
                _custodianEmail = value;
                OnPropertyChanged();
            }
        }

        private string _collectionMode = "Remote Collection";
        public string CollectionMode
        {
            get { return _collectionMode; }
            set
            {
                _collectionMode = value;
                OnPropertyChanged();
            }
        }

        private string _custodianType = "Full Disk";
        public string CustodianType
        {
            get { return _custodianType; }
            set
            {
                _custodianType = value;
                OnPropertyChanged();
            }
        }

        private string _dataSize;
        public string DataSize
        {
            get { return _dataSize; }
            set
            {
                _dataSize = value;
                OnPropertyChanged();
            }
        }

        private string _hash;
        public string Hash
        {
            get { return _hash; }
            set
            {
                _hash = value;
                OnPropertyChanged();
            }
        }

        private string _custodianDate;
        public string CustodianDate
        {
            get { return _custodianDate; }
            set
            {
                _custodianDate = value;
                OnPropertyChanged();
            }
        }

        private string _dataTransferredTo;
        public string DataTransferredTo
        {
            get { return _dataTransferredTo; }
            set
            {
                _dataTransferredTo = value;
                OnPropertyChanged();
            }
        }

        private bool _backupSuccess;

        public bool BackupSuccess
        {
            get { return _backupSuccess; }
            set
            {
                _backupSuccess = value;
                OnPropertyChanged();
            }
        }



        #endregion


        #region Commands

        public ICommand StartUploadCommand { get; }
        public ICommand StartUploadPrevCommand { get; }
        public ICommand SelectFileCommand { get; }
        public ICommand RescanDeviceCommand { get; }
        public ICommand StartCollectionNextBtnCommand { get; }
        public ICommand StartCollectionPrevBtnCommand { get; }
        public ICommand BackupDataCommand { get; }
        public ICommand NewCollectionNextBtnCommand { get; }
        public ICommand StorageOptionsPrevCommand { get; }
        public ICommand StorageOptionsNextCommand { get; }

        public ICommand ShowPickerCommand { get; set; }
        public ICommand DeviceCredentialsPrevBtnCommand { get; set; }
        public ICommand DeviceCredentialsNextBtnCommand { get; set; }
        public ICommand SummaryNextBtnCommand { get; set; }
        public ICommand SummaryPrevBtnCommand { get; set; }


        #endregion

        public MainViewModel(LocalDbService dbService)
        {
            _dbServices = dbService;
            DetectAndLoadDevice();
            LoadSystemInfo();

            //ConnectedDevicesList.Add(new ConnectedDevice
            //{
            //    DeviceName = "iPhone 14",
            //    ProductType = "iPhone14,2",
            //    SerialNumber = "XYZ123"
            //});

            CurrentView = new NewCollectionView();
            NewCollectionNextBtnCommand = new Command(async () => await ExecuteStartNewCollectionNextBtnCommand());
            StorageOptionsPrevCommand = new RelayCommand(ExecuteStartStorageOptionsPrevCommand);
            StorageOptionsNextCommand = new RelayCommand(ExecuteStorageOptionsNextCommand);
            StartUploadCommand = new RelayCommand(ExecuteStartUploadCommand);
            StartUploadPrevCommand = new RelayCommand(ExecuteStartUploadPrevCommand);
            SelectFileCommand = new RelayCommand(ExecuteSelectFileCommand);
            RescanDeviceCommand = new RelayCommand(ExecuteRescanDeviceCommand);
            StartCollectionNextBtnCommand = new RelayCommand(ExecuteStartCollectionNextCommand);
            StartCollectionPrevBtnCommand = new RelayCommand(ExecuteStartCollectionPrevBtnCommand);
            DeviceCredentialsPrevBtnCommand = new RelayCommand(ExecuteDeviceCredentialsPrevBtnCommand);
            DeviceCredentialsNextBtnCommand = new RelayCommand(ExecuteDeviceDeviceCredentialsNextBtnCommand);
            BackupDataCommand = new RelayCommand(ExecuteBackupDataCommand);
            ShowPickerCommand = new RelayCommand(ExecuteShowPickerCommand);
            SummaryNextBtnCommand = new RelayCommand(ExecuteSummaryNextBtnCommand);
            SummaryPrevBtnCommand = new RelayCommand(ExecuteSummarySummaryPrevBtnCommand);

        }

        private void ExecuteSummaryNextBtnCommand()
        {
            CurrentView = new StorageOptionsView();
        }

        private void ExecuteSummarySummaryPrevBtnCommand()
        {
            CurrentView = new DeviceCredentialsView();
        }

        private void ExecuteDeviceDeviceCredentialsNextBtnCommand()
        {
            // CurrentView = new StorageOptionsView();
            CurrentView = new SummaryView();
        }

        private void ExecuteDeviceCredentialsPrevBtnCommand()
        {
            CurrentView = new StartCollectionView();
        }

        private void ExecuteStartUploadPrevCommand()
        {
            CurrentView = new StorageOptionsView();
        }

        private void ExecuteStartCollectionPrevBtnCommand()
        {
            CurrentView = new NewCollectionView();
        }

        private void ExecuteStartStorageOptionsPrevCommand()
        {
            CurrentView = new SummaryView();
        }

        private void ExecuteShowPickerCommand()
        {
            IsPickerVisible = !IsPickerVisible;
        }

        private async void ExecuteBackupDataCommand()
        {
            if (ConnectedDevice.IsAndroid)
            {
                await BackupAndUploadAndroidData();
            }
            else if (ConnectedDevice.IsIos)
            {
                await BackupAndUploadAsync();
            }
        }

        public async Task BackupAndUploadAsync()
        {
            var popup = new LoadingPopup(this);
            try
            {
                Application.Current.MainPage.ShowPopup(popup);

                var progress = new Progress<BackupProgressInfo>(info =>
                {
                    BackupSize = info.Size;
                    FilesWritten = info.FilesWritten;
                    EstimatedFiles = info.EstimatedFiles;
                    TransferSpeed = info.SpeedMBps;
                    ProgressPercent = info.Percent / 1000.0;
                });

                BackupSuccess = await _iosService.TriggerBackupAsync(
                   ConnectedDevice.DeviceName,
                   progress,
                   EncryptionPassword,
                   StorageLocation);


                if (!BackupSuccess)
                {
                    Debug.WriteLine("❌ IPhone Backup failed. Upload aborted.");
                    return;
                }
                App.Current.MainPage.DisplayAlert("Success", "IPhone Backup completed.", "OK");
                Debug.WriteLine("✅ IPhone Backup completed. Starting upload...");

                // await _iosService.UploadToAllPlatforms();
                // Debug.WriteLine("✅ Upload finished.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"❌ Exception during backup: {ex.Message}");
            }
            finally
            {
                popup.Close();  // Ensure popup always closes
            }
        }
        private async Task BackupAndUploadAndroidData()
        {
            if (ConnectedDevice == null)
            {
                ConnectedDevice = _deviceService.GetConnectedDevice();
            }

            if (ConnectedDevice != null)
            {
                var popup = new LoadingPopup(this);


                try
                {
                    var desiredFolders = DeviceFolders
                        .Where(folder => folder.IsSelected)
                        .Select(folder => folder.Name)
                        .ToList();

                    if (desiredFolders == null || !desiredFolders.Any())
                    {
                        await Application.Current.MainPage.DisplayAlert("Alert", "Please Select The Folder First", "Cancel");
                        return; // Exit early to avoid backup with no folders
                    }

                    Application.Current.MainPage.ShowPopup(popup);

                    await Task.Run(() =>
                    {
                        _deviceService.AutoBackupUserFiles(desiredFolders);
                        _deviceService.CompressBackup();
                    });
                    //await Task.Delay(5000); // Simulate some loading
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("❌ Error: " + ex.Message);
                }
                finally
                {
                    popup.Close();
                }
            }
            else
            {
                Debug.WriteLine("❌ No Android device connected.");
            }
        }

        private void SetDeviceFolders(List<string> folders)
        {
            DeviceFolders = new ObservableCollection<SelectedFolder>(
                folders.Select(f => new SelectedFolder { Name = f, IsSelected = false }));
        }

        private void ExecuteStorageOptionsNextCommand()
        {
            CurrentView = new CloudUploadView();
        }

        private async Task ExecuteStartNewCollectionNextBtnCommand()
        {
            try
            {
                //if (string.IsNullOrWhiteSpace(StorageLocation) || !Directory.Exists(StorageLocation))
                //{
                //    App.Current.MainPage.DisplayAlert("Not Found", " Invalid Storage Location Path", "OK");
                //}

                // Validate inputs
                bool isCaseNameEmpty = string.IsNullOrWhiteSpace(CaseName);
                bool isExaminerEmpty = string.IsNullOrWhiteSpace(ExaminerName);
                bool isMatterNumberEmpty = string.IsNullOrWhiteSpace(MatterNumber);

                // Show general message if all fields are empty
                if (isCaseNameEmpty && isMatterNumberEmpty && isExaminerEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please fill out all fields before adding the case.", "OK");
                    return;
                }

                // Show specific messages
                if (isCaseNameEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Case Name.", "OK");
                    return;
                }

                if (isExaminerEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Examiner name.", "OK");
                    return;
                }
                if (isMatterNumberEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Matter Number.", "OK");
                    return;
                }

                if (string.IsNullOrWhiteSpace(StorageLocation))
                {
                    await App.Current.MainPage.DisplayAlert("Error", " Storage Location cannot be empty.", "OK");
                    return;
                }
                if (!Directory.Exists(StorageLocation))
                {
                    await App.Current.MainPage.DisplayAlert("Not Found", " Storage Location Not Found", "OK");
                    return;
                }

                var newCase = new Case
                {
                    CaseName = CaseName.Trim(),
                    ExaminerName = ExaminerName.Trim(),
                    MatterNumber = MatterNumber.Trim(),
                    StorageLocation = StorageLocation,
                };

                await _dbServices.CreateCase(newCase);

                await App.Current.MainPage.DisplayAlert("Sucess", " Case Added Succesfully", "OK");

                CaseName = "";
                ExaminerName = "";
                MatterNumber = "";


                CurrentView = new StartCollectionView();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Something went wrong:\n{ex.Message}", "OK");
            }
        }

        private void ExecuteStartCollectionNextCommand()
        {
            CurrentView = new DeviceCredentialsView();
        }

        private void ExecuteRescanDeviceCommand()
        {
            DetectAndLoadDevice();
        }

        private void DetectAndLoadDevice()
        {
            // Try Android first
            var androidDevice = _deviceService.GetConnectedDevice();

            if (androidDevice != null && androidDevice.IsAndroid)
            {
                ConnectedDevice = androidDevice;
                GetDeviceConnection();
                return;
            }

            // Try iOS
            var iosDevice = _iosService.GetConnectedDevice();

            if (iosDevice != null && iosDevice.IsIos)
            {
                ConnectedDevice = iosDevice;
                ConnectedDevicesList.Clear();
                ConnectedDevicesList.Add(ConnectedDevice);
                LoadDeviceInfo();
                return;
            }

            // No device found
            ConnectedDevice = new ConnectedDevice
            {
                Platform = MobilePlatform.Unknown
            };

            DeviceInfo = "📡 Waiting for device connection...";
            DeviceWarningMsg = "Please connect the iPhone via USB. Once connected, the profile will appear below.";
            IsDeviceConnected = false;
            DeviceFolders = new ObservableCollection<SelectedFolder> { };
        }

        private void GetDeviceConnection()
        {
            ConnectedDevice = _deviceService.GetConnectedDevice();

            if (ConnectedDevice != null)
            {
                //App.Current.MainPage.DisplayAlert("No Device", "No device is connected.", "OK");
                var folders = _deviceService.GetSdcardFolders();
                SetDeviceFolders(folders);
                IsDeviceConnected = true;
            }
        }

        private void LoadDeviceInfo()
        {
            ConnectedDevice = _iosService.GetConnectedDevice();

            if (ConnectedDevice != null)
            {
                //App.Current.MainPage.DisplayAlert("No Device", "No device is connected.", "OK");
                //var folders = _deviceService.GetSdcardFolders();
                //SetDeviceFolders(folders);

                foreach (var name in _iosService.ConnectedDeviceNames)
                {
                    ConnectedDevice.DeviceName = name;
                    Debug.WriteLine("Connected iPhone: " + name);
                }
                IsDeviceConnected = true;


            }

        }

        private void ExecuteSelectFileCommand()
        {

        }

        private async void ExecuteStartUploadCommand()
        {
            if (!BackupSuccess)
            {

                await _iosService.UploadToAllPlatformsAsync(uploadToGCP: IsGoogleCloudSelected, uploadToAWS: IsAwsS3Selected, uploadToAzure: IsAzureSelected, uploadToUSB: IsUSBConnected);
                await App.Current.MainPage.DisplayAlert("", "Upload finished.", "OK");
                Debug.WriteLine("✅ Upload finished.");

            }
        }

        private void LoadSystemInfo()
        {
            // OS Info
            OSProfile = RuntimeInformation.OSDescription;
            CustodianName = $"{Environment.UserDomainName}\\{Environment.UserName}";

            // Drive Info
            var drives = DriveInfo.GetDrives();
            var readyDrives = drives.Where(d => d.IsReady && d.DriveType == DriveType.Fixed).ToList();

            // Example: "Macintosh HD (2TB), External SSD (500GB)"
            HardDriveInventory = string.Join(", ", readyDrives.Select(d =>
            {
                string label = string.IsNullOrWhiteSpace(d.VolumeLabel) ? d.Name.TrimEnd('\\') : d.VolumeLabel;
                string sizeGB = $"{d.TotalSize / (1024L * 1024 * 1024)}GB";
                return $"{label} ({sizeGB})";
            }));

            bool preserved = readyDrives.Any(d => d.DriveType == DriveType.Fixed || d.DriveType == DriveType.Network);
            HardDrivePreserved = preserved ? "Yes" : "Not Preserved";

            // Total size of all preserved drives
            long totalSizeBytes = readyDrives.Sum(d => d.TotalSize);
            DataSize = $"{totalSizeBytes / (1024 * 1024 * 1024)} GB";

            // Set a dummy hash (real hash would come from hashing actual files/folders)
            Hash = GenerateDummyHash();

            CustodianDate = DateTime.Now.ToString("yyyy-MM-dd");

        }

        private string GenerateDummyHash()
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(CustodianName + CustodianDate));
            return BitConverter.ToString(hash).Replace("-", "").Substring(0, 16);
        }


    }
    public class SelectedFolder
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

}

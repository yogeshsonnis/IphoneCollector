
using CommunityToolkit.Maui.Views;
using IphoneCollector.Data;
using IphoneCollector.Helper;
using IphoneCollector.MVVM.Model;
using IphoneCollector.MVVM.View;
using IphoneCollector.Services;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using static IphoneCollector.MVVM.Model.ConnectedDevice;

namespace IphoneCollector.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel
    {

        private readonly AndroidDeviceService _deviceService = new();

        private readonly IOSDeviceService _iosService = new();

        private readonly LocalDbService _dbServices;


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


        #endregion

        public MainViewModel(LocalDbService dbService)
        {
            _dbServices = dbService;
            DetectAndLoadDevice();

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

        }

        private void ExecuteDeviceDeviceCredentialsNextBtnCommand()
        {
            CurrentView = new StorageOptionsView();
        }

        private void ExecuteDeviceCredentialsPrevBtnCommand()
        {
            CurrentView = new NewCollectionView();
        }

        private void ExecuteStartUploadPrevCommand()
        {
            CurrentView = new StartCollectionView();
        }

        private void ExecuteStartCollectionPrevBtnCommand()
        {
            CurrentView = new StorageOptionsView();
        }

        private void ExecuteStartStorageOptionsPrevCommand()
        {
            CurrentView = new DeviceCredentialsView();
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

                bool backupSuccess = await _iosService.TriggerBackupAsync(
                    ConnectedDevice.DeviceName,
                    progress,
                    EncryptionPassword,
                    StorageLocation);


                if (!backupSuccess)
                {
                    Debug.WriteLine("❌ IPhone Backup failed. Upload aborted.");
                    return;
                }
                App.Current.MainPage.DisplayAlert("Success", "IPhone Backup completed.", "OK");
                Debug.WriteLine("✅ IPhone Backup completed. Starting upload...");

                //await _iosService.UploadToAllPlatforms();
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
            CurrentView = new StartCollectionView();
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
                    CaseName = CaseName.Trim() ,
                    ExaminerName = ExaminerName.Trim() ,
                    MatterNumber = MatterNumber.Trim() ,
                    StorageLocation = StorageLocation,
                };

                await _dbServices.CreateCase(newCase);

                await App.Current.MainPage.DisplayAlert("Sucess", " Case Added Succesfully", "OK");

                CaseName = "";
                ExaminerName = "";
                MatterNumber = "";
             

                CurrentView = new DeviceCredentialsView();

            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Something went wrong:\n{ex.Message}", "OK");
            }
        }

        private void ExecuteStartCollectionNextCommand()
        {
            CurrentView = new CloudUploadView();
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

        private void ExecuteStartUploadCommand()
        {

        }

    }
    public class SelectedFolder
    {
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }

}

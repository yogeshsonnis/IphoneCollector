using IphoneCollector.Data;
using IphoneCollector.Helper;
using IphoneCollector.MVVM.Model;
using IphoneCollector.MVVM.View;
using IphoneCollector.MVVM.View.CollectionViewUC;
using IphoneCollector.MVVM.View.CollectorViews;
using IphoneCollector.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using static IphoneCollector.MVVM.Model.ConnectedDevice;


namespace IphoneCollector.MVVM.ViewModel
{
    public class MainViewModelVM : BaseViewModel
    {
        private readonly AndroidDeviceService _deviceService = new();

        private readonly IOSDeviceService _iosService = new();

        private readonly LocalDbService _dbServices;

        #region new properties 

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

        private string _iphoneModel;

        public string IphoneModel
        {
            get { return _iphoneModel; }
            set
            {
                _iphoneModel = value;
                OnPropertyChanged();
            }
        }

        private string _phoneNumber;

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                _phoneNumber = value;
                OnPropertyChanged();
            }
        }

        private string _email;

        public string Email
        {
            get { return _email; }
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        private string _notes;
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                OnPropertyChanged();
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

        #endregion
        private ContentView _currentView;
        public ContentView CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();

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
        public ObservableCollection<Collector> Collector { get; set; }
        public ObservableCollection<CollectorWizard> CollectorWizard { get; set; } = new();
        public ObservableCollection<CollectorStorage> CollectorStorage { get; set; } = new();
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


        private ContentView _collectionView;
        public ContentView CollectionView
        {
            get => _collectionView;
            set
            {
                _collectionView = value;
                OnPropertyChanged();
            }
        }
        private bool _isAllSelected;
        public bool IsAllSelected
        {
            get => _isAllSelected;
            set
            {
                if (_isAllSelected != value)
                {
                    _isAllSelected = value;
                    //   PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsAllSelected)));
                    SelectAllItems(_isAllSelected);
                }
            }
        }
        private void SelectAllItems(bool isSelected)
        {
            foreach (var item in Collector)
            {
                item.IsSelected = isSelected;
            }
        }
        public ICommand AWSS3BtnCommand { get; set; }
        public ICommand AWSGovCloudBtnCommand { get; set; }
        public ICommand GoogleCloudBtnCommand { get; set; }
        public ICommand AzureBtnCommand { get; set; }
        public ICommand FileShareBtnCommand { get; set; }
        public ICommand FTPBtnCommand { get; set; }
        public ICommand CollectionWizardCommand { get; set; }
        public ICommand CastPageCommand { get; set; }
        public ICommand StorageWizardCommand { get; set; }
        public ICommand ImagingWizardCommand { get; set; }
        #region new command 
        public ICommand NewCollectionNextBtnCommand { get; }
        public ICommand DeviceCredentialsNextBtnCommand { get; set; }
        public ICommand SummaryNextBtnCommand { get; set; }
        public ICommand StorageOptionsNextCommand { get; }
        public ICommand SaveAddDeviceCommand { get; }
        #endregion

        public MainViewModelVM(LocalDbService dbService)
        {
            _dbServices = dbService;
            _ = GetCollectorWizardData();
            
            DetectAndLoadDevice();

            CurrentView = new NewCollectionView();
            CollectionView = new AwsS3View();

            Collector = new ObservableCollection<Collector>
        {
            new Collector { Custodian="John Doe", PhoneNumber = "(555) 123-4567", iPhoneModel="iPhone 13 Pro", SavedTo="Drive A", Action= "Notes" },
            new Collector { Custodian="Jane Smith", PhoneNumber = "(555) 987-6543", iPhoneModel="iPhone 12", SavedTo="Drive B", Action= "Notes" },
            new Collector { Custodian="Alan Poe", PhoneNumber = "(555) 333-4444", iPhoneModel="iPhone 11", SavedTo="Drive C", Action= "Notes" },
            new Collector { Custodian="Mary Lane", PhoneNumber = "(555) 222-1111", iPhoneModel="iPhone X", SavedTo="Drive D", Action= "Notes" },
            new Collector { Custodian="Chris Evans", PhoneNumber = "(555) 444-5555", iPhoneModel="iPhone 14", SavedTo="Drive E", Action= "Notes" },
            new Collector { Custodian="Susan White", PhoneNumber = "(555) 666-7777", iPhoneModel="iPhone 13", SavedTo="Drive F", Action= "Notes" },
            new Collector { Custodian="Bob Grey", PhoneNumber = "(555) 888-9999", iPhoneModel="iPhone SE", SavedTo="Drive G", Action= "Notes" },

        };

            //    CollectorWizard = new ObservableCollection<CollectorWizard>
            //{
            //    new CollectorWizard { iPhoneModel="iPhone 13 Pro",Custodian="john Doe", Email = "johndoe@gmail.com",Notes="Recently upgraded device"},
            //    new CollectorWizard { iPhoneModel="iPhone 12",Custodian="Jane Smith", Email = "janesmith@example.com",Notes="Primary collector"},
            //    new CollectorWizard { iPhoneModel="iPhone 11",Custodian="Alan Poe", Email = "alan.poe@example.com",Notes="Handles monthly backups"},
            //    new CollectorWizard {iPhoneModel = "iPhone X",Custodian = "Mary Lane",  Email = "mary.lane@example.com",Notes="Responsible for field testing"},
            //    new CollectorWizard {iPhoneModel = "iPhone 14",Custodian = "Chris Evans",  Email = "chris.evans@example.com",Notes="Collector for QA team"},
            //    new CollectorWizard {iPhoneModel = "iPhone 13", Custodian = "Susan White", Email = "susan.white@example.com",Notes="Recently upgraded device"},
            //    new CollectorWizard {iPhoneModel = "iPhone SE", Custodian = "Bob Grey", Email = "bob.grey@example.com",Notes="Lightweight test device"},

            //};


            CollectorStorage = new ObservableCollection<CollectorStorage>
        {
            new CollectorStorage { Label="Example Label", Platform = "Windows", Path="C:/Example/Path", TextConnection="Text" },
            new CollectorStorage { Label="Another Label", Platform = "Mac", Path="/Users/example/path", TextConnection="Text" },


        };

            AWSS3BtnCommand = new RelayCommand(ExecuteAWSS3BtnCommand);
            AWSGovCloudBtnCommand = new RelayCommand(ExecuteAWSGovCloudBtnCommand);
            GoogleCloudBtnCommand = new RelayCommand(ExecuteGoogleCloudBtnCommand);
            AzureBtnCommand = new RelayCommand(ExecuteAzureBtnCommand);
            FileShareBtnCommand = new RelayCommand(ExecuteFileShareBtnCommand);
            FTPBtnCommand = new RelayCommand(ExecuteFTPBtnCommand);
            // CurrentView = new CollectionWizardView();
            CollectionWizardCommand = new RelayCommand(ExecuteCollectionWizardCommand);
            CastPageCommand = new RelayCommand(ExecuteCastPageCommand);
            StorageWizardCommand = new RelayCommand(ExecuteStorageWizardCommand);
            ImagingWizardCommand = new RelayCommand(ExecuteImagingWizardCommand);

            #region new execute cmd                   
            NewCollectionNextBtnCommand = new Command(ExecuteStartNewCollectionNextBtnCommand);
            DeviceCredentialsNextBtnCommand = new RelayCommand(ExecuteDeviceDeviceCredentialsNextBtnCommand);
            SummaryNextBtnCommand = new RelayCommand(ExecuteSummaryNextBtnCommand);
            StorageOptionsNextCommand = new RelayCommand(ExecuteStorageOptionsNextCommand);
            SaveAddDeviceCommand = new Command(async () => await ExecuteSaveAddDeviceCommand());
            #endregion
        }

        #region new commands


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

        //    DeviceInfo = "📡 Waiting for device connection...";
         //   DeviceWarningMsg = "Please connect the iPhone via USB. Once connected, the profile will appear below.";
          //  IsDeviceConnected = false;
            DeviceFolders = new ObservableCollection<SelectedFolder> { };
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
             //   IsDeviceConnected = true;


            }

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
        private void SetDeviceFolders(List<string> folders)
        {
            DeviceFolders = new ObservableCollection<SelectedFolder>(
                folders.Select(f => new SelectedFolder { Name = f, IsSelected = false }));
        }

        public async Task GetCollectorWizardData()
        {
            try
            {

                var devices = await _dbServices.GetDevices();

                // Clear existing data before adding new
                CollectorWizard.Clear();

                if (devices != null && devices.Any())
                {
                    foreach (var device in devices)
                    {
                        var collector = new CollectorWizard
                        {
                            Custodian = device.CustodianName,
                            iPhoneModel = device.IphoneModel,
                            Email = device.Email,
                            Notes = device.Notes
                        };

                        CollectorWizard.Add(collector);
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Info", "No devices found in the database.", "OK");
                }
            }
            catch (Exception ex)
            {

                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void ExecuteStartNewCollectionNextBtnCommand()
        {
            CurrentView = new ImagingWizardView();
        }

        private async Task ExecuteSaveAddDeviceCommand()
        {
            try
            {
                #region forCase


                //if (string.IsNullOrWhiteSpace(StorageLocation) || !Directory.Exists(StorageLocation))
                //{
                //    App.Current.MainPage.DisplayAlert("Not Found", " Invalid Storage Location Path", "OK");
                //}

                //// Validate inputs
                //bool isCaseNameEmpty = string.IsNullOrWhiteSpace(CaseName);
                //bool isExaminerEmpty = string.IsNullOrWhiteSpace(ExaminerName);
                //bool isMatterNumberEmpty = string.IsNullOrWhiteSpace(MatterNumber);

                //// Show general message if all fields are empty
                //if (isCaseNameEmpty && isMatterNumberEmpty && isExaminerEmpty)
                //{
                //    await App.Current.MainPage.DisplayAlert("Warning", "Please fill out all fields before adding the case.", "OK");
                //    return;
                //}

                //// Show specific messages
                //if (isCaseNameEmpty)
                //{
                //    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Case Name.", "OK");
                //    return;
                //}

                //if (isExaminerEmpty)
                //{
                //    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Examiner name.", "OK");
                //    return;
                //}
                //if (isMatterNumberEmpty)
                //{
                //    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Matter Number.", "OK");
                //    return;
                //}

                //if (string.IsNullOrWhiteSpace(StorageLocation))
                //{
                //    await App.Current.MainPage.DisplayAlert("Error", " Storage Location cannot be empty.", "OK");
                //    return;
                //}
                //if (!Directory.Exists(StorageLocation))
                //{
                //    await App.Current.MainPage.DisplayAlert("Not Found", " Storage Location Not Found", "OK");
                //    return;
                //}

                //var newCase = new Case
                //{
                //    CaseName = CaseName.Trim(),
                //    ExaminerName = ExaminerName.Trim(),
                //    MatterNumber = MatterNumber.Trim(),
                //    StorageLocation = StorageLocation,
                //};

                //await _dbServices.CreateCase(newCase);

                //await App.Current.MainPage.DisplayAlert("Sucess", " Case Added Succesfully", "OK");

                //CaseName = "";
                //ExaminerName = "";
                //MatterNumber = "";
                #endregion

                #region DeviceInfo


                //if (string.IsNullOrWhiteSpace(StorageLocation) || !Directory.Exists(StorageLocation))
                //{
                //    App.Current.MainPage.DisplayAlert("Not Found", " Invalid Storage Location Path", "OK");
                //}

                // Validate inputs
                bool isCustodianNameEmpty = string.IsNullOrWhiteSpace(CustodianName);
                bool isIphoneModelEmpty = string.IsNullOrWhiteSpace(IphoneModel);
                bool isPhoneNumberEmpty = string.IsNullOrWhiteSpace(PhoneNumber);
                bool isEmailEmpty = string.IsNullOrWhiteSpace(Email);
                bool isNotesEmpty = string.IsNullOrWhiteSpace(Notes);

                // Show general message if all fields are empty
                if (isCustodianNameEmpty && isIphoneModelEmpty && isPhoneNumberEmpty && isEmailEmpty && isNotesEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please fill out all fields before adding the device info.", "OK");
                    return;
                }

                // Show specific messages
                if (isCustodianNameEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Custodian Name.", "OK");
                    return;
                }

                if (isIphoneModelEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the iPhone Model.", "OK");
                    return;
                }

                if (isPhoneNumberEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Phone Number.", "OK");
                    return;
                }

                if (isEmailEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter the Email address.", "OK");
                    return;
                }

                if (isNotesEmpty)
                {
                    await App.Current.MainPage.DisplayAlert("Warning", "Please enter some Notes.", "OK");
                    return;
                }


                var newDeviceInfo = new Data.DeviceInfo
                {
                    CustodianName = CustodianName.Trim(),
                    IphoneModel = IphoneModel.Trim(),
                    PhoneNumber = PhoneNumber.Trim(),
                    Email = Email.Trim(),
                    Notes = Notes.Trim(),

                };

                await _dbServices.CreateDevice(newDeviceInfo);

                await App.Current.MainPage.DisplayAlert("Sucess", " Device Added Succesfully", "OK");

                await GetCollectorWizardData();

                CustodianName = "";
                IphoneModel = "";
                PhoneNumber = "";
                Email = "";
                Notes = "";

                #endregion 




            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Something went wrong:\n{ex.Message}", "OK");
            }
        }

        private void ExecuteDeviceDeviceCredentialsNextBtnCommand()
        {
            // CurrentView = new StorageOptionsView();
            CurrentView = new SummaryView();
        }

        private void ExecuteSummaryNextBtnCommand()
        {
            CurrentView = new StorageOptionsView();
        }

        private void ExecuteStorageOptionsNextCommand()
        {
            CurrentView = new CloudUploadView();
        }

        #endregion

        private void ExecuteImagingWizardCommand()
        {
            CurrentView = new DeviceCredentialsView();
        }

        private void ExecuteStorageWizardCommand()
        {
            CurrentView = new CastPageView();
        }

        private void ExecuteCastPageCommand()
        {
            CurrentView = new StorageWizardView();
        }

        private void ExecuteCollectionWizardCommand()
        {
            CurrentView = new CastPageView();

        }
        private void ExecuteFTPBtnCommand()
        {
            CollectionView = new FTPView();
        }

        private void ExecuteFileShareBtnCommand()
        {
            CollectionView = new FileShareVIew();
        }

        private void ExecuteAzureBtnCommand()
        {
            CollectionView = new AzureView();
        }

        private void ExecuteGoogleCloudBtnCommand()
        {
            CollectionView = new GoogleCloudView();
        }

        private void ExecuteAWSS3BtnCommand()
        {
            CollectionView = new AwsS3View();
        }

        private void ExecuteAWSGovCloudBtnCommand()
        {
            CollectionView = new AwsGovCloudView();
        }
    }
    public class Collector : INotifyPropertyChanged
    {
        public string Custodian { get; set; }
        public string PhoneNumber { get; set; }
        public string iPhoneModel { get; set; }
        public string SavedTo { get; set; }
        public string Action { get; set; }
        public string Notes { get; set; }


        private bool _isSelected;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
    public class CollectorWizard : INotifyPropertyChanged
    {
        public string Email { get; set; }
        public string Custodian { get; set; }
        public string iPhoneModel { get; set; }
        public string Notes { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
    public class CollectorStorage : INotifyPropertyChanged
    {

        public string Label { get; set; }
        public string Platform { get; set; }
        public string Path { get; set; }
        public string TextConnection { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}


using IphoneCollector.Helper;
using IphoneCollector.MVVM.View.CollectionViewUC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace IphoneCollector.MVVM.ViewModel
{
    public class NewCollectionViewModel:BaseViewModel
    {
        public ObservableCollection<Collector> Collector { get; set; }
        public ObservableCollection<CollectorWizard> CollectorWizard { get; set; } = new();
        public ObservableCollection<CollectorStorage> CollectorStorage { get; set; } = new();

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

        public NewCollectionViewModel()
        {
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

            CollectorWizard = new ObservableCollection<CollectorWizard>
        {
            new CollectorWizard { iPhoneModel="iPhone 13 Pro",Custodian="john Doe", Email = "johndoe@gmail.com",Notes="Recently upgraded device"},
            new CollectorWizard { iPhoneModel="iPhone 12",Custodian="Jane Smith", Email = "janesmith@example.com",Notes="Primary collector"},
            new CollectorWizard { iPhoneModel="iPhone 11",Custodian="Alan Poe", Email = "alan.poe@example.com",Notes="Handles monthly backups"},
            new CollectorWizard {iPhoneModel = "iPhone X",Custodian = "Mary Lane",  Email = "mary.lane@example.com",Notes="Responsible for field testing"},
            new CollectorWizard {iPhoneModel = "iPhone 14",Custodian = "Chris Evans",  Email = "chris.evans@example.com",Notes="Collector for QA team"},
            new CollectorWizard {iPhoneModel = "iPhone 13", Custodian = "Susan White", Email = "susan.white@example.com",Notes="Recently upgraded device"},
            new CollectorWizard {iPhoneModel = "iPhone SE", Custodian = "Bob Grey", Email = "bob.grey@example.com",Notes="Lightweight test device"},

        };

            CollectorStorage = new ObservableCollection<CollectorStorage>
        {
            new CollectorStorage { Label="Example Label", Platform = "Windows", Path="C:/Example/Path", TextConnection="Text" },
            new CollectorStorage { Label="Another Label", Platform = "Mac", Path="/Users/example/path", TextConnection="Text" },


        };

            AWSS3BtnCommand = new Helper.RelayCommand(ExecuteAWSS3BtnCommand);
            AWSGovCloudBtnCommand = new Helper.RelayCommand(ExecuteAWSGovCloudBtnCommand);
            GoogleCloudBtnCommand = new Helper.RelayCommand(ExecuteGoogleCloudBtnCommand);
            AzureBtnCommand = new Helper.RelayCommand(ExecuteAzureBtnCommand);
            FileShareBtnCommand = new Helper.RelayCommand(ExecuteFileShareBtnCommand);
            FTPBtnCommand = new Helper.RelayCommand(ExecuteFTPBtnCommand);
            
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
    //public class Collector : INotifyPropertyChanged
    //{
    //    public string Custodian { get; set; }
    //    public string PhoneNumber { get; set; }
    //    public string iPhoneModel { get; set; }
    //    public string SavedTo { get; set; }
    //    public string Action { get; set; }
    //    public string Notes { get; set; }


    //    private bool _isSelected;

    //    public bool IsSelected
    //    {
    //        get => _isSelected;
    //        set
    //        {
    //            if (_isSelected != value)
    //            {
    //                _isSelected = value;
    //                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsSelected)));
    //            }
    //        }
    //    }

    //    public event PropertyChangedEventHandler PropertyChanged;
    //}
    //public class CollectorWizard : INotifyPropertyChanged
    //{
    //    public string Email { get; set; }
    //    public string Custodian { get; set; }
    //    public string iPhoneModel { get; set; }
    //    public string Notes { get; set; }

    //    public event PropertyChangedEventHandler? PropertyChanged;
    //}
    //public class CollectorStorage : INotifyPropertyChanged
    //{

    //    public string Label { get; set; }
    //    public string Platform { get; set; }
    //    public string Path { get; set; } 
    //    public string TextConnection { get; set; }

    //    public event PropertyChangedEventHandler? PropertyChanged;
    //}
}

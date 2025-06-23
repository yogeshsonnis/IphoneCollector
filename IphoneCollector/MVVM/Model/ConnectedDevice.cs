using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.MVVM.Model
{
    public class ConnectedDevice
    {
        public string DeviceName { get; set; }
        public string ProductType { get; set; }
        public string SerialNumber { get; set; }
        public string UniqueDeviceID { get; set; }
        public string ProductVersion { get; set; }
        public string ComponyName { get; set; }
        public bool IsConnected => !string.IsNullOrEmpty(DeviceName);

        public enum MobilePlatform
        {
            Unknown,
            Android,
            Ios
        }

        public MobilePlatform Platform { get; set; } = MobilePlatform.Unknown;

        public bool IsAndroid => Platform == MobilePlatform.Android;
        public bool IsIos => Platform == MobilePlatform.Ios;
    }
}

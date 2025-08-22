using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.CloudConfigHelper
{
    public class CloudConfig
    {
        public GoogleCloudConfig GoogleCloud { get; set; }
        public AwsS3Config AwsS3 { get; set; }
        public AzureBlobConfig AzureBlob { get; set; }
        public string UsbPath { get; set; }
    }

    public class GoogleCloudConfig
    {
        public string BucketName { get; set; }
        public string ServiceAccountJsonPath { get; set; }
    }

    public class AwsS3Config
    {
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string Region { get; set; }
    }

    public class AzureBlobConfig
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
    public class NetShareConfig
    {
        public string NetworkPath { get; set; }
       
    }
    public class ShareFileConfig
    {
        public string uploadUrl { get; set; }
        public string AuthToken { get; set; }
       
    }

}

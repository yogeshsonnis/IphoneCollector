using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.Services
{
    public interface IBackupUploader
    {
        Task UploadAsync(string zipFilePath); // Instead of folder path
    }
}

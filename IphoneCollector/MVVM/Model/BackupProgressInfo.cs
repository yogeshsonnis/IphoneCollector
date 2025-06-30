using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IphoneCollector.MVVM.Model
{
    public class BackupProgressInfo
    {
        public int Percent { get; set; }
        public string Size { get; set; }
        public int FilesWritten { get; set; }
        public int EstimatedFiles { get; set; }
        public double SpeedMBps { get; set; }
    }

}

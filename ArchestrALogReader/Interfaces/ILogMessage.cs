using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    interface ILogMessage
    {
        ulong id { get; set; }
        DateTime timeStamp { get; set; }
        int processID { get; set; }
        int threadID { get; set; }
        string logFlag { get; set; }
        string component { get; set; }
        string message { get; set; }
        int lastRecordRead { get; set; }
    }
}

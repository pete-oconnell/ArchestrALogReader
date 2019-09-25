using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    public class LogMessage : ILogMessage
    {
        public ulong id { get; set; }
        public DateTime timeStamp { get; set; } 
        public int processID { get; set; }
        public int threadID { get; set; }
        public string logFlag { get; set; }
        public string component { get; set; }
        public string message { get; set; }
        public int lastRecordRead { get; set; }
    }
}

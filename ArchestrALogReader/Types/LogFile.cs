using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    public class LogFile : ILogFile
    {
        public string fileName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int firstMessageOffset { get; set; }
        public int lastMessageOffset { get; set; }
        public int lastRead { get; set; }
    }
}

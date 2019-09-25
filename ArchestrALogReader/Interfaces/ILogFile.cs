using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    interface ILogFile
    {
        string fileName { get; set; }
        DateTime startDate { get; set; }
        DateTime endDate { get; set; }
        int firstMessageOffset { get; set; }
        int lastMessageOffset { get; set; }
        int lastRead { get; set; }
    }
}

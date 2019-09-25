using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ArchestrALogReader
{
    public class _LogReader
    {
        public string logPath;

        public List<LogFile> getLogFiles(string filePath, DateTime startDate, DateTime endDate)
        {
            List<LogFile> _logFiles = new List<LogFile>();
            DirectoryInfo d = new DirectoryInfo(filePath);

            foreach (FileInfo logfile in d.GetFiles("*.aaLOG"))
            {
                using (FileStream fsSource = new FileStream(logfile.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite ))
                {
                    int readResult;
                    int workingOffset = 0;
                    byte[] byteArray = new byte[1];
                    int fieldLength;

                    fsSource.Seek((long)0, SeekOrigin.Begin);

                    byteArray = new byte[12];

                    readResult = fsSource.Read(byteArray, 0, 12);

                    int headerLength = BitConverter.ToInt32(byteArray, 8);

                    fsSource.Seek((long)0, SeekOrigin.Begin);

                    byteArray = new byte[checked(headerLength + 1)];

                    readResult = fsSource.Read(byteArray, 0, headerLength);

                    var startMsg = BitConverter.ToUInt64(byteArray, 20);

                    var msgCount = (ulong)BitConverter.ToInt32(byteArray, 28);

                    var fileTime1 = BitConverter.ToUInt32(byteArray, 32);
                    var fileTime2 = BitConverter.ToUInt32(byteArray, checked(32 + 4));

                    ulong fileDT;

                    fileDT = (((ulong)fileTime2) << 32) | fileTime1;

                    DateTimeOffset startTime = DateTimeOffset.FromFileTime((long)fileDT);

                    fileTime1 = BitConverter.ToUInt32(byteArray, 40);
                    fileTime2 = BitConverter.ToUInt32(byteArray, checked(40 + 4));

                    fileDT = (((ulong)fileTime2) << 32) | fileTime1;

                    DateTimeOffset endTime = DateTimeOffset.FromFileTime((long)fileDT);

                    int firstRecordOffset = BitConverter.ToInt32(byteArray, 48);
                    int lastRecordOffset = BitConverter.ToInt32(byteArray, 52);

                    workingOffset = 56;
                    string computerName = byteArray.GetString(workingOffset, out fieldLength);

                    workingOffset += fieldLength + 2;
                    string session = byteArray.GetString(workingOffset, out fieldLength);

                    workingOffset += fieldLength + 2;
                    string prevFileName = byteArray.GetString(workingOffset, out fieldLength);

                    if ((startTime >= startDate) && (endTime <= endDate))
                    {
                        LogFile _logfile = new LogFile();
                        _logfile.fileName = logfile.FullName;
                        _logfile.startDate = startTime.DateTime;
                        _logfile.endDate = endTime.DateTime;
                        _logFiles.Add(_logfile);
                    }
                }
            }
            return _logFiles;
        }
    }

    public struct FileTimeStruct
    {
        public uint dwLowDateTime;
        public uint dwHighDateTime;

        public ulong @value
        {
            get
            {
                ulong lDt;
                lDt = (((ulong)dwHighDateTime) << 32) | dwLowDateTime;
                return lDt;
            }
        }
    }

    public class _LogFile
    {
        public string fileName { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    public class LogReader
    {
        public LogFile ReadLogHeader(string path)
        {
            LogFile _myLog = new LogFile();
            try
            {
                using (FileStream logSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int result;
                    byte[] byteArray = new byte[1];

                    logSource.Seek((long)0, SeekOrigin.Begin);

                    byteArray = new byte[12];

                    result = logSource.Read(byteArray, 0, 12);

                    int headerLength = BitConverter.ToInt32(byteArray, 8);

                    logSource.Seek((long)0, SeekOrigin.Begin);

                    byteArray = new byte[checked(headerLength + 1)];

                    result = logSource.Read(byteArray, 0, headerLength);

                    var startMsg = BitConverter.ToUInt64(byteArray, 20);

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

                    _myLog.fileName = path;
                    _myLog.startDate = startTime.UtcDateTime;
                    _myLog.endDate = endTime.UtcDateTime;
                    _myLog.firstMessageOffset = firstRecordOffset;
                    _myLog.lastMessageOffset = lastRecordOffset;

                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to read log file {0}", ex);
            }

            return _myLog;
        }

        public List<LogFile> GetLogFiles(string path)
        {
            List<LogFile> _logFiles = new List<LogFile>();

            try
            {
                DirectoryInfo logDir = new DirectoryInfo(path);
                foreach (FileInfo logfile in logDir.GetFiles("*.aaLOG"))
                {
                    _logFiles.Add(ReadLogHeader(logfile.FullName));
                }
            }
            catch(Exception ex)
            {
                throw new Exception("Could not read log files", ex);
            }

            return _logFiles;
        }

        public List<LogFile> GetLogFiles(string path, DateTime startDate, DateTime endDate)
        {
            List<LogFile> _logFiles = new List<LogFile>();
            LogFile log;
            try
            {
                DirectoryInfo logDir = new DirectoryInfo(path);
                foreach (FileInfo logFile in logDir.GetFiles("*.aaLOG"))
                {
                    log = ReadLogHeader(logFile.FullName);
                    if ((log.startDate >= startDate) && (log.endDate <= endDate))
                    {
                        _logFiles.Add(log);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Could not read log files", ex);
            }

            return _logFiles;
        }

        public void GetLogMessages(LogFile logFile)
        {
            List<LogMessage> _logMessages = new List<LogMessage>();

            /*int recordLength = 0;
            byte[] byteArray = new byte[1];
            int workingOffset = 0;
            int fieldLength;

            workingOffset = 0;



            using (FileStream logSource = new FileStream(logFile.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                logSource.Seek((long)logFile.firstMessageOffset, SeekOrigin.Begin);
                byteArray = new byte[8];

                if (logSource.Read(byteArray, 0, 8) == 0)
                {

                }

                recordLength = BitConverter.ToInt32(byteArray, 4);

                if (recordLength <= 0)
                {
                    
                }

                logSource.Seek((long)logFile.firstMessageOffset, SeekOrigin.Begin);

                byteArray = new byte[checked(recordLength + 1)];

                logSource.Read(byteArray, 0, recordLength);

                DateTime _test = DateTimeOffset.FromFileTime((long)byteArray.GetFileTime(24)).DateTime;

            }*/

            int lastread = 0;
            while (lastread < logFile.lastMessageOffset)
            {
                ReadLogMessage(logFile, lastread, out lastread);
            }
            
        }

        public LogMessage ReadLogMessage(LogFile logFile, int startEnd, out int offset)
        {
            LogMessage _logMessage = new LogMessage();

            int recordLength = 0;
            byte[] byteArray = new byte[1];
            int workingOffset = 0;


            using (FileStream logSource = new FileStream(logFile.fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                if (startEnd <= logFile.firstMessageOffset)
                {
                    logSource.Seek((long)logFile.firstMessageOffset, SeekOrigin.Begin);
                    startEnd = logFile.firstMessageOffset;
                }
                else
                {
                    logSource.Seek((long)startEnd, SeekOrigin.Begin);
                }
                byteArray = new byte[8];

                if (logSource.Read(byteArray, 0, 8) == 0)
                {

                }

                recordLength = BitConverter.ToInt32(byteArray, 4);

                

                if (recordLength <= 0)
                {

                }

                startEnd += recordLength;

                logSource.Seek((long)logFile.firstMessageOffset, SeekOrigin.Begin);

                byteArray = new byte[checked(recordLength + 1)];

                logSource.Read(byteArray, 0, recordLength);

                _logMessage.timeStamp = DateTimeOffset.FromFileTime((long)byteArray.GetFileTime(24)).DateTime;
                _logMessage.processID = (int)byteArray.GetUInt32(16);
                _logMessage.threadID = (int)byteArray.GetUInt32(20);
                _logMessage.logFlag = byteArray.GetString(32, out recordLength);
                workingOffset += recordLength + 2;
                _logMessage.component = byteArray.GetString(workingOffset, out recordLength);
                workingOffset += recordLength + 2;
                _logMessage.message = byteArray.GetString(workingOffset, out recordLength);


                offset = checked(workingOffset + recordLength);
                if (offset < startEnd)
                {
                    offset = startEnd;
                }

                _logMessage.lastRecordRead = offset;

            }

            return _logMessage;
        }
    }
}

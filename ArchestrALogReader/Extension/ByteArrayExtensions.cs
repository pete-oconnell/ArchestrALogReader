using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArchestrALogReader
{
    public static class ByteArrayExtensions
    {
        public static int GetInt(this byte[] bytes, int offset = 0)
        {
            return BitConverter.ToInt32(bytes, offset);
        }

        public static uint GetUInt32(this byte[] bytes, int offset = 0)
        {
            return BitConverter.ToUInt32(bytes, offset);
        }

        public static ulong GetFileTime(this byte[] bytes, int offset = 0)
        {
            var localFileTimeStruct = new FileTimeStruct
            {
                dwLowDateTime = BitConverter.ToUInt32(bytes, offset),
                dwHighDateTime = BitConverter.ToUInt32(bytes, checked(offset + 4))
            };
            return localFileTimeStruct.value;
        }

        public static string GetString(this byte[] bytes, int offset, out int length)
        {
            length = bytes.GetStringLength(offset);
            return Encoding.Unicode.GetString(bytes, offset, length);
        }

        public static int GetStringLength(this byte[] bytes, int offset)
        {
            var length = 0;
            var index = offset;
            while (true)
            {
                var value = BitConverter.ToUInt16(bytes, index);
                if (value == 0) break;
                length += 2;
                index += 2;
            }
            return length;
        }

        public static ulong GetULong(this byte[] bytes, int offset)
        {
            return BitConverter.ToUInt64(bytes, offset);
        }

        public static string GetSessionID(this byte[] bytes, int offset)
        {
            try
            {
                return string.Format("{0}.{1}.{2}.{3}", bytes[offset + 3], bytes[offset + 2], bytes[offset + 1], bytes[offset]);
            }
            catch
            {
                return "0.0.0.0";
            }
        }
    }
}

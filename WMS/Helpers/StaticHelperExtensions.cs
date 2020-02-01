using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace WMS.Helpers
{
    public static class StaticHelperExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            var bytes = new List<byte>();

            int b;
            while ((b = stream.ReadByte()) != -1)
                bytes.Add((byte)b);

            return bytes.ToArray();
        }

        public static DateTime? AsDateTime(this string input)
        {
            DateTime? result = null;
            DateTime outDate;
            bool success = DateTime.TryParse(input, out outDate);
            if (success) result = outDate;
            return result;
        }
        public static int AsInt(this string input)
        {
            int result = 0;
            int outInt;
            bool success = Int32.TryParse(input, out outInt);
            if (success) result = outInt;
            return result;
        }

        
    }
}
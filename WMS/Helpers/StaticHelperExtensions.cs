using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        public static decimal AsDecimal(this string input, int roundingDigits = 2)
        {
            decimal result = 0;
            decimal outInt;
            bool success = decimal.TryParse(input, out outInt);
            if (success) result = outInt;
            return Math.Round(result, roundingDigits);
        }
        public static decimal AsRoundedDecimal(this decimal input, int roundingDigits = 2)
        {
            return Math.Round(input, roundingDigits);
        }

        //usage: var lst =  Enum<myenum>.GetSelectList();
        public static List<SelectListItem> GetSelectList<T>()
        {
            return Enum.GetValues(typeof(T))
                .Cast<object>()
                .Select(i => new SelectListItem()
                {
                    Value = ((int)i).ToString(),
                    Text = i.ToString()
                })
                .ToList();
        }


    }
}
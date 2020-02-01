using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Barcoding
{

    public class GS128Decoder
    {
        static int _lotNumberLength = 20;
        static int _serialNumberLength = 20;
        static string _aiLeftIdentifier = "(";
        static string _aiRightIdentifier = ")";
        static bool _status = false;

        bool _lotRes = Int32.TryParse(ConfigurationManager.AppSettings.Get("GS128LotNumberLength"), out _lotNumberLength);
        bool _serialRes = Int32.TryParse(ConfigurationManager.AppSettings.Get("GS128SerialNumberLength"), out _serialNumberLength);


        GS128BarcodeAI _aiSSCC = new GS128BarcodeAI { AiCode = GetAiCode("00"), AiCodeLength = GetAiCodeLength("00"), AiMaxLength = 18, Type = AiTypeEnum.Numeric };
        GS128BarcodeAI _aiGTIN = new GS128BarcodeAI { AiCode = GetAiCode("01"), AiCodeLength = GetAiCodeLength("01"), AiMaxLength = 14, Type = AiTypeEnum.Numeric };
        GS128BarcodeAI _aiGTINB = new GS128BarcodeAI { AiCode = GetAiCode("02"), AiCodeLength = GetAiCodeLength("02"), AiMaxLength = 14, Type = AiTypeEnum.Numeric };
        GS128BarcodeAI _aiLotNumber = new GS128BarcodeAI { AiCode = GetAiCode("10"), AiCodeLength = GetAiCodeLength("10"), AiMaxLength = _lotNumberLength, Type = AiTypeEnum.ALphaNumeric };
        GS128BarcodeAI _aiDateProduction = new GS128BarcodeAI { AiCode = GetAiCode("11"), AiCodeLength = GetAiCodeLength("11"), AiMaxLength = 6, Type = AiTypeEnum.Date };
        GS128BarcodeAI _aiDateDue = new GS128BarcodeAI { AiCode = GetAiCode("12"), AiCodeLength = GetAiCodeLength("12"), AiMaxLength = 6, Type = AiTypeEnum.Date };
        GS128BarcodeAI _aiDatePacking = new GS128BarcodeAI { AiCode = GetAiCode("13"), AiCodeLength = GetAiCodeLength("13"), AiMaxLength = 6, Type = AiTypeEnum.Date };
        GS128BarcodeAI _aiDateBestBefore = new GS128BarcodeAI { AiCode = GetAiCode("15"), AiCodeLength = GetAiCodeLength("15"), AiMaxLength = 6, Type = AiTypeEnum.Date };
        GS128BarcodeAI _aiDateExpiry = new GS128BarcodeAI { AiCode = GetAiCode("17"), AiCodeLength = GetAiCodeLength("17"), AiMaxLength = 6, Type = AiTypeEnum.Date };
        GS128BarcodeAI _aiSerialNumber = new GS128BarcodeAI { AiCode = GetAiCode("21"), AiCodeLength = GetAiCodeLength("21"), AiMaxLength = _serialNumberLength, Type = AiTypeEnum.ALphaNumeric };

        public GS128DecodeResult GS128Decode(string barcode)
        {
            _status = false;
            var res = new GS128DecodeResult
            {
                SSCC = Decoder(_aiSSCC, barcode),
                GTIN = Decoder(_aiGTIN, barcode),
                GTINB = Decoder(_aiGTINB, barcode),
                LotNumber = Decoder(_aiLotNumber, barcode),
                DateProduction = Decoder(_aiDateProduction, barcode),
                DateDue = Decoder(_aiDateDue, barcode),
                DatePacking = Decoder(_aiDatePacking, barcode),
                DateBestBefore = Decoder(_aiDateBestBefore, barcode),
                DateExpiry = Decoder(_aiDateExpiry, barcode),
                SerialNumber = Decoder(_aiSerialNumber, barcode),
                Status = _status
            };

            return res;

        }

        /// <summary>
        /// returns value (if exist) of specified GS128 Identifier
        /// Returns empty string if type is not available in passed string
        /// </summary>
        /// <param name="barcode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GS128DecodeByType(string barcode, GS128DecodeType type)
        {
            string res = "";

            var ai = new GS128BarcodeAI();
            switch (type)
            {
                case GS128DecodeType.SSCC:
                    res = Decoder(_aiSSCC, barcode);
                    break;
                case GS128DecodeType.GTIN:
                    res = Decoder(_aiGTIN, barcode);
                    break;
                case GS128DecodeType.GTINB:
                    res = Decoder(_aiGTINB, barcode);
                    break;
                case GS128DecodeType.LotNumber:
                    res = Decoder(_aiLotNumber, barcode);
                    break;
                case GS128DecodeType.DateProduction:
                    res = Decoder(_aiDateProduction, barcode);
                    break;
                case GS128DecodeType.DateDue:
                    res = Decoder(_aiDateDue, barcode);
                    break;
                case GS128DecodeType.DatePacking:
                    res = Decoder(_aiDatePacking, barcode);
                    break;
                case GS128DecodeType.DateBestBefore:
                    res = Decoder(_aiDateBestBefore, barcode);
                    break;
                case GS128DecodeType.DateExpiry:
                    res = Decoder(_aiDateExpiry, barcode);
                    break;
                case GS128DecodeType.SerialNumber:
                    res = Decoder(_aiSerialNumber, barcode);
                    break;

            }

            return res;


        }
        /// <summary>
        /// if GTIN value does exist in GS128 barcode, that GTIN will be returned
        /// otherwise same passed barcode string will be returned
        /// </summary>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public string GS128DecodeGTINOrDefault(string barcode)
        {
            string res = "";
            res = Decoder(_aiGTIN, barcode);

            if (!string.IsNullOrWhiteSpace(res)) { return res; }
            else { return barcode; }

        }

        private string Decoder(GS128BarcodeAI ai, string barcode)
        {
            string code = "";

            if (barcode.Contains(ai.AiCode))
            {
                int startIndex = barcode.IndexOf(ai.AiCode) + ai.AiCodeLength;
                code = barcode.Substring(startIndex, ai.AiMaxLength);
                _status = true;
            }

            return code;

        }

        private static string GetAiCode(string code)
        {
            return _aiLeftIdentifier + code + _aiRightIdentifier;
        }

        private static int GetAiCodeLength(string code)
        {
            string codeString = _aiLeftIdentifier + code + _aiRightIdentifier;

            return codeString.Length;
        }

        private class GS128BarcodeAI
        {
            public string AiCode { get; set; }
            public int AiMinLength { get; set; }
            public int AiMaxLength { get; set; }
            public string AiName { get; set; }
            public int AiCodeLength { get; set; }
            public AiTypeEnum Type { get; set; }

        }

        private enum AiTypeEnum
        {
            Numeric = 1,
            ALphaNumeric = 2,
            Date = 3
        }

    }

    public class GS128DecodeResult
    {
        public string SSCC { get; set; }
        public string GTIN { get; set; }
        public string GTINB { get; set; }
        public string LotNumber { get; set; }
        public string DateProduction { get; set; }
        public string DateDue { get; set; }
        public string DatePacking { get; set; }
        public string DateBestBefore { get; set; }
        public string DateExpiry { get; set; }
        public string SerialNumber { get; set; }
        public bool Status { get; set; }
    }

    public enum GS128DecodeType
    {
        SSCC,
        GTIN,
        GTINB,
        LotNumber,
        DateProduction,
        DateDue,
        DatePacking,
        DateBestBefore,
        DateExpiry,
        SerialNumber
    }
}
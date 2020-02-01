using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class LogResponse
    {
        public int LogId { get; set; }
        public int RecievedRecords { get; set; }
        public int ProcessedRecodrds { get; set; }
        public int InsertedIntoDB { get; set; }
    }
    public class StockEnquiry
    {
        public string SkuCode { get; set; }
        public string ShortDesc { get; set; }
        public decimal InStock { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
    }

    public class TerminalsListViewModel
    {
        public int TerminalId { get; set; }
        public string Warehouse { get; set; }
        public string TerminalName { get; set; }
        public string TermainlSerial { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
        public bool IsActive { get; set; }

    }



}
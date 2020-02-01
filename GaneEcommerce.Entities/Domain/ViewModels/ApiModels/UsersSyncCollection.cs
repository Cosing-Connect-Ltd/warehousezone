using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class UserSync
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Boolean IsActive { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? DateUpdated { get; set; }
        public Boolean PurchaseOrderPerm { get; set; }
        public Boolean SalesOrderPerm { get; set; }
        public Boolean TransferOrderPerm { get; set; }
        public Boolean WorksOrderPerm { get; set; }
        public Boolean GoodsReturnPerm { get; set; }
        public Boolean StockTakePerm { get; set; }
        public Boolean PalletingPerm { get; set; }
        public Boolean WastagesPerm { get; set; }
        public Boolean DirectSalesPerm { get; set; }
        public bool PODPerm { get; set; }
        public bool MarketRoutesPerm { get; set; }
        public bool RandomJobsPerm { get; set; }
        public bool StockEnquiryPerm { get; set; }
        public bool EndOfDayPerm { get; set; }
        public bool HolidaysPerm { get; set; }
        public bool GeneratePalletLabelsPerm { get; set; }
        public bool GoodsReceiveCountPerm { get; set; }
        public bool HandheldOverridePerm { get; set; }
        public bool IsResource { get; set; }
        public int ResourceId { get; set; }
        public bool AddProductsOnScan { get; set; }
        public bool ExchangeOrdersPerm { get; set; }
        public bool AllowModifyPriceInTerminal { get; set; }
        public bool PendingOrdersPerm { get; set; }
        public bool PrintBarcodePerm { get; set; }
    }

    public class UsersSyncCollection
    {
        public int Count { get; set; }
        public Guid TerminalLogId { get; set; }
        public List<UserSync> Users { get; set; }
    }
}
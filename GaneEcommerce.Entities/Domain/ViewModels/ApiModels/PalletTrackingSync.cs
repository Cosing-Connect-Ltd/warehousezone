﻿using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class PalletTrackingSync
    {
        public int PalletTrackingId { get; set; }
        public int ProductId { get; set; }
        public string PalletSerial { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public decimal RemainingCases { get; set; }
        public decimal TotalCases { get; set; }
        public string BatchNo { get; set; }
        public string Comments { get; set; }
        public PalletTrackingStatusEnum Status { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateUpdated { get; set; }
    }

    public class PalletTrackingSyncCollection
    {
        public PalletTrackingSyncCollection()
        {
            PalletTrackingSync = new List<PalletTrackingSync>();
        }
        public Guid TerminalLogId { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
        public int Count { get; set; }
        public List<PalletTrackingSync> PalletTrackingSync { get; set; }
    }
    public class VerifyPalletTrackingSync
    {
        public string SerialNo { get; set; }
        public string PalletSerial { get; set; }
        public int? OrderId { get; set; }
        public int? ProductId { get; set; }
        public int InventoryTransactionType { get; set; }
        public int TenantId { get; set; }
        public int WarehouseId { get; set; }
    }

    public class PalleTrackingProcess
    {
        public int PalletTrackingId { get; set; }
        public decimal ProcessedQuantity { get; set; }
    }
    public class SubmitPalletSerial
    {
        public List<string> PalletSerial { get; set; }

        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public int OrderDetailID { get; set; }

        public int UserId { get; set; }

        public InventoryTransactionTypeEnum InventoryTransactionType { get; set; }

        public int ShopId { get; set; }
    }
    public class OrderProcessFull
    {
        public int OrderDetailId { get; set; }

        public int ProductId { get; set; }

        public int OrderId { get; set; }

        public Decimal Quantity { get; set; }

        public int ShopId { get; set; }

        public int UserId { get; set; }
    }


}
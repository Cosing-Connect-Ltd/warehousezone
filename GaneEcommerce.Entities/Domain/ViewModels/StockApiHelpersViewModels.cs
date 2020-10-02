using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;

namespace Ganedata.Core.Entities.Domain
{
    public class StockTakeProductCodeScanRequest : StockTakeProductCodeScanModel
    {
        public string SerialNo { get; set; }
        public Guid TransactionLogId { get; set; }

        public StockTakeScanLog LoadLog(StockTakeScanLog logger)
        {
            logger.RequestCurrentTenantId = CurrentTenantId;
            logger.RequestWarehouseId = WarehouseId;
            logger.RequestAuthUserId = AuthUserId;
            logger.RequestStockTakeId = StockTakeId;
            logger.RequestProductCode = ProductCode;
            logger.RequestProductSerial = ProductSerial;
            logger.RequestPalletSerial = PalletSerial;
            return logger;
        }
    }

    public class StockTakeProductCodeScanModel
    {
        public int CurrentTenantId { get; set; }
        public int WarehouseId { get; set; }
        public int AuthUserId { get; set; }
        public int StockTakeId { get; set; }
        public string LocationCode { get; set; }
        public string ProductCode { get; set; }
        public string ProductSerial { get; set; }
        public int ScannedQuantity { get; set; }
        public string PalletSerial { get; set; }
        public bool NotExistingItem { get; set; }
        public string NewProductBarcode { get; set; }
        public string NewProductBarcode2 { get; set; }
        public string NewProductName { get; set; }
        public bool IsSerialised { get; set; }
        public bool BatchRequired { get; set; }

        public bool IsProcessByPallet { get; set; }
        public int ProductId { get; set; }
        public string BatchNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
    }

    public class StockAdjustSerialsRequest
    {
        public int ProductId { get; set; }
        public InventoryTransactionTypeEnum InventoryTransactionTypeId { get; set; }
        public int? Quantity { get; set; }
        public string InventoryTransactionRef { get; set; }

        public List<string> SerialItems { get; set; }
    }


    public class StockTakeProductLookupRequest
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
    }

    public class StockTakeReportResponse
    {
        public StockTakeReportResponse()
        {
            StockTakeReportResponseItems = new List<StockTakeReportResponseItem>();
        }
        public List<StockTakeReportResponseItem> StockTakeReportResponseItems { get; set; }

        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }
        public bool AllowApplyChanges { get; set; }
        public int CurrentStockTakeId { get; set; }
        public int StockTakeStatusId { get; set; }
        public string StockTakeStatusString { get; set; }
        public string CurrentStockTakeRef { get; set; }
        public string CurrentStockTakeDesc { get; set; }
        public string CurrentStockTakeDate { get; set; }
    }

    public class StockTakeReportResponseItem
    {
        public int InventoryStockId { get; set; }
        public string ProductCode { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsSerialised { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public string ProductDescription { get; set; }

    }

    public class StockTakeProductCodeScanResponse : StockTakeProductCodeScanModel
    {
        public StockTakeProductCodeScanResponse()
        {
            Response = new ResponseObject();
        }

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public ResponseObject Response { get; set; }
        public decimal InStock { get; set; }
        public decimal Allocated { get; set; }
        public decimal Available { get; set; }
        public bool SerialRequired { get; set; }
        public bool PalletSerialRequired { get; set; }
        public int StockTakeDetailId { get; set; }
        public int StockDetailQuantity { get; set; }

        public StockTakeScanLog LoadLog(StockTakeScanLog logger)
        {
            logger.ResponseProductName = ProductName;
            logger.ResponseProductName = ProductDescription;
            logger.ResponseInStock = InStock;
            logger.ResponseAllocated = Allocated;
            logger.ResponseAvailable = Available;
            logger.ResponseSerialRequired = SerialRequired;

            logger.ResponseSuccess = Response.Success;
            logger.ResponseFailureMessage = Response.FailureMessage;
            logger.ResponseHasWarning = Response.HasWarning;
            logger.ResponseWarningMessage = Response.WarningMessage;

            return logger;
        }
    }

    public class ResponseObject
    {
        public ResponseObject()
        {
            Success = true;
            ResponseTime = DateTime.UtcNow;
        }
        public bool Success { get; set; }

        public string FailureMessage { get; set; }

        public DateTime ResponseTime { get; set; }

        public bool HasWarning { get; set; }

        public string WarningMessage { get; set; }

        public bool SerialRequired { get; set; }

        public int ProductId { get; set; }
        public bool MoveToNextProduct { get; set; }
        public bool ProductDontExist { get; set; }
        public bool SerialInsteadProduct { get; set; }

        public string ProductCode { get; set; }

        public string ProductGroup { get; set; }
    }
    public class StockTakeApplyChangeRequest
    {
        public StockTakeApplyChangeRequest()
        {
            RequestItems = new List<StockTakeApplyChangeRequestItem>();
        }
        public List<StockTakeApplyChangeRequestItem> RequestItems { get; set; }

        public int StockTakeId { get; set; }

        public static StockTakeApplyChangeRequest MapFromFormCollection(FormCollection formCollection)
        {
            var result = new StockTakeApplyChangeRequest { StockTakeId = int.Parse(formCollection["StockTakeId"]) };
            var checkedInventories = formCollection.AllKeys.Where(m => m.Contains("InventoryStock_") && formCollection[m] == "C").ToList();
            foreach (var item in checkedInventories)
            {
                var inventoryId = item.Replace("InventoryStock_", string.Empty);
                var values = formCollection["InventoryStockv_" + inventoryId];
                if (!string.IsNullOrEmpty(values) && values.Split(',').Length == 3)
                {
                    var valueCollection = values.Split(',');
                    var responseItem = new StockTakeApplyChangeRequestItem()
                    {
                        InventoryStockId = int.Parse(inventoryId),
                        ApplyChange = true,
                        PreviousQuantity = decimal.Parse(valueCollection[1]),
                        CurrentQuantity = decimal.Parse(valueCollection[2])
                    };
                    result.RequestItems.Add(responseItem);
                }
            }
            return result;
        }
    }
    public class StockTakeApplyChangeRequestItem
    {
        public int InventoryStockId { get; set; }
        public bool ApplyChange { get; set; }
        public decimal PreviousQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
    }


    public class ProductDetailRequest
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public bool IsSerialised { get; set; }
        public string SerialCode { get; set; }
        public bool IsNewProduct { get; set; }
        public int TenantId { get; set; }
        public int AccountId { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public bool ThresholdAcknowledged { get; set; }
        public string ProcessByType { get; set; }
        public string PageSessionToken { get; set; }
        public bool IsTransferAdd { get; set; }

        public decimal? CaseQuantity { get; set; }

        public int? TaxIds { get; set; }

        public int? ProductGroupId { get; set; }
        public int? ProductDepartmentId { get; set; }

        public string ProductDesc { get; set; }

    }
    public class StockDetailQuantityUpdateRequest
    {
        public int StockTakeDetailId { get; set; }
        public decimal NewQuantity { get; set; }
        public string BatchNumber { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        [DataType(DataType.Date)]
        public DateTime? ExpiryDate { get; set; }
        public Guid TransactionLogId { get; set; }
        public string SerialNo { get; set; }
    }
    public class StockDetailDeleteRequest
    {
        public int StockTakeDetailId { get; set; }
        public int WarehouseId { get; set; }
    }

    public class OrderRecipientInfo
    {
        public string CustomRecipients { get; set; }
        public string CustomCCRecipients { get; set; }
        public string CustomBCCRecipients { get; set; }

        public int? orderId { get; set; }
        public bool SendEmailWithAttachment { get; set; }

        public bool SendEmailWithAccount { get; set; }
        public int?[] AccountEmailContacts { get; set; }

        public string ShipmentDestination { get; set; }

        public string CustomMessage { get; set; }

        public int? AccountId { get; set; }
        public DateTime?  StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string ShipmentAddressLine1 { get; set; }
        public string ShipmentAddressLine2 { get; set; }
        public string ShipmentAddressLine3 { get; set; }
        public string ShipmentAddressTown { get; set; }
        public string ShipmentAddressPostcode { get; set; }
        public int? AccountAddressId { get; set; }
        public string TenantAddressID { get; set; }
        public int PPropertyID { get; set; }

        public string[] PropertyTenantIds { get; set; }

        public string PageSessionToken { get; set; }
        public bool? AddAddressToAccount { get; set; }

        public int? InventoryTransactionType { get; set; }
        public bool AttachDeliveryNote { get; set; }

    }

    public class StockTakeSnapshotsViewModel
    {
        public int ProductId { get; set; }
        public string ShDesc { get; set; }
        public string ReceivedSku { get; set; }
        public decimal PreviousQuantity { get; set; }
    }

    public class StockTakeDetailsViewModel
    {
        public int ProductId { get; set; }
        public decimal Quantity { get; set; }

        public DateTime DateScanned { get; set; }

        public int StockTakeDetailId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReceivedSku { get; set; }
        public bool Serialisable { get; set; }
    }

    public class OrderProcessSerialResponse
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public bool IsSerialised { get; set; }
        public string SerialCode { get; set; }
        public bool IsNewProduct { get; set; }
        public int TenantId { get; set; }
        public int AccountId { get; set; }
        public int InventoryTransactionTypeId { get; set; }
        public bool ThresholdAcknowledged { get; set; }
    }
}
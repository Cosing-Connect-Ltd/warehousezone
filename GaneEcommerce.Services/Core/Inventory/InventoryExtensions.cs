using System.Collections.Generic;
using Ganedata.Core.Entities.Enums;

namespace Ganedata.Core.Services
{
    public class InventoryExtensions
    {
        public static List<InventoryTransactionTypeEnum> StockOutTransactionTypeList
        {
            get
            {
                return new List<InventoryTransactionTypeEnum>()
                {
                    InventoryTransactionTypeEnum.AdjustmentOut, 
                    InventoryTransactionTypeEnum.DirectSales,
                    InventoryTransactionTypeEnum.Loan, 
                    InventoryTransactionTypeEnum.SalesOrder,
                    InventoryTransactionTypeEnum.Samples,
                    InventoryTransactionTypeEnum.TransferOut,
                    InventoryTransactionTypeEnum.WorksOrder,
                    InventoryTransactionTypeEnum.Wastage
                };
            }
        }

        public static List<InventoryTransactionTypeEnum> StockInTransactionTypeList {
            get
            {
                return new List<InventoryTransactionTypeEnum>()
                {
                    InventoryTransactionTypeEnum.AdjustmentIn,
                    InventoryTransactionTypeEnum.PurchaseOrder,
                    InventoryTransactionTypeEnum.Returns,
                    InventoryTransactionTypeEnum.TransferIn
                }; 
            }
        }
    }
}
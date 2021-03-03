using System.Collections.Generic;
using System.Linq;
using Ganedata.Core.Entities.Domain;
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

        public static decimal GetTotalInStock(IQueryable<InventoryCalculationViewModel> query)
        {
            decimal totalIn;
            decimal totalOut;
            decimal inStock;
            decimal adjustmentIn;
            decimal adjustmentOut;
            decimal totalReturns;
            decimal transferIn;
            decimal transferOut;
            decimal worksOut;
            decimal samples;
            decimal directSales;
            decimal exchnageOut;
            decimal wastages;

            totalIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.PurchaseOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            totalOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.SalesOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            totalReturns = query.Where(e => e.Type == InventoryTransactionTypeEnum.Returns).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            adjustmentIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.AdjustmentIn)
                .Select(I => I.Quantity).DefaultIfEmpty(0).Sum();
            adjustmentOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.AdjustmentOut)
                .Select(I => I.Quantity).DefaultIfEmpty(0).Sum();
            transferIn = query.Where(e => e.Type == InventoryTransactionTypeEnum.TransferIn).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            transferOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.TransferOut).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            worksOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.WorksOrder).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            samples = query.Where(e => e.Type == InventoryTransactionTypeEnum.Samples).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            directSales = query.Where(e => e.Type == InventoryTransactionTypeEnum.DirectSales).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            exchnageOut = query.Where(e => e.Type == InventoryTransactionTypeEnum.Exchange).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();
            wastages = query.Where(e => e.Type == InventoryTransactionTypeEnum.Wastage).Select(I => I.Quantity)
                .DefaultIfEmpty(0).Sum();

            inStock = (totalIn + totalReturns + adjustmentIn + transferIn) - adjustmentOut - totalOut - transferOut -
                      worksOut - samples - directSales - exchnageOut - wastages;

            return inStock;
        }

    }
}
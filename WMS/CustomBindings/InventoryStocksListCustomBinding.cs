using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class InventoryStocksListCustomBinding
    {

        public static GridViewModel CreateInventoryStocksListGridViewModel()
        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "ProductId";
            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("WarehouseId");
            viewModel.Columns.Add("WarehouseName");
            viewModel.Columns.Add("SkuCode");
            viewModel.Columns.Add("Barcode");
            viewModel.Columns.Add("InStock");
            viewModel.Columns.Add("Allocated");
            viewModel.Columns.Add("Available");
            viewModel.Columns.Add("OnOrder");
            viewModel.Columns.Add("PalletProduct");



            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void InventoryStocksListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int InventoryId)
        {

            var transactions = GetInventoryStocksListDataset(tenantId, InventoryId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                transactions = transactions.OrderBy(sortString);
            }
            else
            {
                transactions = transactions.OrderBy("ProductName Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        private static IQueryable<InventoryStockViewModel> GetInventoryStocksListDataset(int tenantId, int InventoryId)
      {
            var InventoryStocks = DependencyResolver.Current.GetService<IProductServices>();
            var transactions = InventoryStocks.GetAllInventoryStocksList(tenantId, InventoryId);
             
            return transactions;
        }

        public static void LoadStockLevelFilters(List<InventoryStockViewModel> stocks, int tenantId)
        {
            var service = DependencyResolver.Current.GetService<ITenantLocationServices>();

            var stockLevels = service.GetAllStockLevelsForTenant(tenantId).ToList();
            foreach (var stock in stocks)
            {
                var stockLevel = stockLevels.FirstOrDefault(m => m.ProductID == stock.ProductId);//RH: This has to be filtered with WarehouseId based on ProductStockLevel table, however leaving with what is done currently
                if (stockLevel != null && stockLevel.MinStockQuantity > 0)
                {
                    if (stock.Available < stockLevel.MinStockQuantity)
                    {
                        stock.StockLevelColor = "#F08080";
                    }
                    if (stock.InStock > stockLevel.MinStockQuantity && stock.InStock < stockLevel.MinStockQuantity * 1.1m)
                    {
                        stock.StockLevelColor = "#FFAA80";
                    }
                }
            }
        }

        public static void InventoryStocksListGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int InventoryId)
        {
            var transactions = GetInventoryStocksListDataset(tenantId, InventoryId);
             
            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                transactions = transactions.OrderBy(sortString);
            }
            else
            {
                transactions = transactions.OrderBy("ProductName Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var data = transactions.ToList();

            var lookupService = DependencyResolver.Current.GetService<ILookupServices>();

            data.ForEach(m =>
            {
                if (m.PalletProduct == true)
                {
                    m.StockIssue = lookupService.CheckStockIssue(m.ProductId, m.InStock, (m.SerialProduct ?? false), (m.PalletProduct ?? true));
                }
            });

            LoadStockLevelFilters(data, tenantId);

            e.Data = data;
        }
    }
}
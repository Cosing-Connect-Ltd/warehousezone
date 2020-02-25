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
            // var results = new List<InventoryStockViewModel>();
            data.ForEach(m =>
            {
                if (m.PalletProduct == true)
                {
                    m.StockIssue = lookupService.CheckStockIssue(m.ProductId, m.InStock, (m.SerialProduct ?? false), (m.PalletProduct ?? true));
                }
                //else if (m.SerialProduct == true)
                //{
                //    m.SerialProduct = lookupService.CheckStockIssue(m.ProductId, m.InStock, (m.SerialProduct ?? true), (m.PalletProduct ?? false));
                //}
            });
            e.Data = data;
        }
    }
}
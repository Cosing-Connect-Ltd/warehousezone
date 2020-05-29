using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class InventoryListCustomBinding
    {

        private static IQueryable<object> GetInventoryDataset(int tenantId, int warehouseId)
        {
            var productServices = DependencyResolver.Current.GetService<IProductServices>();

            var transactions = from p in productServices.GetAllInventoryTransactions(tenantId, warehouseId)
                               select new
                               {

                                   InventoryTransactionId = p.InventoryTransactionId,
                                   InventoryTransactionTypeName = p.InventoryTransactionTypeId,
                                   SerialNo = p.ProductSerial != null ? p.ProductSerial.SerialNo : "",
                                   PalletSerial = p.PalletTracking != null ? p.PalletTracking.PalletSerial : "",
                                   Name = p.ProductMaster.Name,
                                   ProductGroup = p.ProductMaster.ProductGroup.ProductGroup ?? null,
                                   DepartmentName = p.ProductMaster.TenantDepartment.DepartmentName ?? null,
                                   SKU = p.ProductMaster.SKUCode,
                                   Quantity = p.Quantity,
                                   ExpiryDate = p.ExpiryDate ?? null,
                                   DateCreated = p.DateCreated,
                                   OrderNumber = p.Order != null ? p.Order.OrderNumber : "",
                                   Property = p.Order != null && p.Order.PProperties != null ? p.Order.PProperties.PropertyCode + " - "
                                             + p.Order.PProperties.AddressLine1 + " - " + p.Order.PProperties.AddressPostcode : "",
                                   InStock = p.LastQty,
                                   PalletId = p.PalletTrackingId,
                                   DontMonitorStock = p.DontMonitorStock,
                                   PalletExpiry = p.PalletTracking.ExpiryDate ?? null,
                                   TerminalName = p.Terminals.TerminalName,
                                   CreatedBy = p.AuthUsers.UserName

                               };

            return transactions;
        }

        public static void InventoryGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetInventoryDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("DateCreated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void InventoryGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetInventoryDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("DateCreated Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);



            e.Data = transactions.ToList();
        }


        public static GridViewModel CreateInventoryGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "InventoryTransactionId";
            viewModel.Columns.Add("InventoryTransactionTypeName");
            viewModel.Columns.Add("SerialNo");
            viewModel.Columns.Add("PalletExpiry");
            viewModel.Columns.Add("PalletId");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Quantity");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("ProductGroup");
            viewModel.Columns.Add("ExpiryDate");
            viewModel.Columns.Add("DepartmentName");
            viewModel.Columns.Add("Property");
            viewModel.Columns.Add("InStock");
            viewModel.Columns.Add("DontMonitorStock");
            viewModel.Columns.Add("TerminalName");
            viewModel.Columns.Add("CreatedBy");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }

}
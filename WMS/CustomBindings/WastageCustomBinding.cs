using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class WastageCustomBinding
    {

        private static IQueryable<object> WastageDataset(int tenantId, int warehouseId)
        {
            var productServices = DependencyResolver.Current.GetService<IProductServices>();

            var transactions = from p in productServices.GetAllInventoryTransactions(tenantId, warehouseId).Where(x => x.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage)
                               select new
                               {
                                   InventoryTransactionId = p.InventoryTransactionId,
                                   SerialNo = p.ProductSerial != null ? p.ProductSerial.SerialNo : "",
                                   PalletSerial = p.PalletTracking != null ? p.PalletTracking.PalletSerial : "",
                                   Name = p.ProductMaster.Name,
                                   ProductGroup = p.ProductMaster.ProductGroup.ProductGroup ?? null,
                                   DepartmentName = p.ProductMaster.TenantDepartment.DepartmentName ?? null,
                                   SKU = p.ProductMaster.SKUCode,
                                   Quantity = p.Quantity,
                                   DateCreated = p.DateCreated
                               };

            return transactions;

        }

        public static void WastageDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = WastageDataset(tenantId, warehouseId);

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

        public static void WastageData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = WastageDataset(tenantId, warehouseId);

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


        public static GridViewModel CreateGoodsReturnGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "InventoryTransactionId";

            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("SKU");
            viewModel.Columns.Add("ProductGroup");
            viewModel.Columns.Add("DepartmentName");
            viewModel.Columns.Add("Quantity");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("SerialNo");
            viewModel.Columns.Add("PalletSerial");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }

}
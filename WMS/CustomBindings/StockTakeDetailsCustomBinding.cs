using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class StockTakeDetailsCustomBinding
    {

        private static IQueryable<object> StockTakeDetailsDataset(int tenantId, int warehouseId, int stockTakeId)
        {
            var StockTakeApiService = DependencyResolver.Current.GetService<IStockTakeApiService>();

            var transactions = StockTakeApiService.GetDetialStock(tenantId, warehouseId, stockTakeId);

            return transactions;
        }

        public static void StockTakeDetailsDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId, int stockTakeId)
        {
            var transactions = StockTakeDetailsDataset(tenantId, warehouseId, stockTakeId);

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
                transactions = transactions.OrderBy("DateScanned Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void StockTakeDetailsData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId, int stockTakeId)
        {
            var transactions = StockTakeDetailsDataset(tenantId, warehouseId, stockTakeId);

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
                transactions = transactions.OrderBy("DateScanned Desc");
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


        public static GridViewModel CreateStockTakeDetailsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "StockTakeDetailId";

            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("ReceivedSku");
            viewModel.Columns.Add("Quantity");
            viewModel.Columns.Add("DateScanned");
            viewModel.Columns.Add("DateApplied");
            viewModel.Columns.Add("PalletSerial");
            viewModel.Columns.Add("SerialNumber");
            viewModel.Columns.Add("BatchNumber");
            viewModel.Columns.Add("ExpiryDate");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
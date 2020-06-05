using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class WebsiteWarehousesCustomBinding
    {
        private static IQueryable<object> GetWebsiteWarehousesDataset(int tenantId, int SiteId)
        {
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var transactions = tenantWebsiteService.GetAllValidWebsiteWarehouses(tenantId, SiteId);
            return transactions;
        }

        public static void WarehouseGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int SiteId)
        {
            var warehouses = GetWebsiteWarehousesDataset(tenantId, SiteId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                warehouses = warehouses.OrderBy(sortString);
            }
            else
            {
                warehouses = warehouses.OrderBy("WarehouseName");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                warehouses = warehouses.Where(filterString);

            }

            warehouses = warehouses.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            e.Data = warehouses.ToList();
        }

        public static void WarehouseGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int SiteId)
        {

            var warehouses = GetWebsiteWarehousesDataset(tenantId, SiteId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                warehouses = warehouses.OrderBy(sortString);
            }
            else
            {
                warehouses = warehouses.OrderBy("WarehouseName");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                warehouses = warehouses.Where(filterString);
            }

            e.DataRowCount = warehouses.Count();

        }

        public static GridViewModel CreateWebsiteWarehousesViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("WarehouseId");
            viewModel.Columns.Add("SiteID");
            viewModel.Columns.Add("WarehouseName");
            viewModel.Columns.Add("WarehouseCity");
            viewModel.Columns.Add("WarehouseAddress");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
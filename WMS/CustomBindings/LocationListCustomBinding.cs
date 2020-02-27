using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class LocationListCustomBinding
    {

        private static IQueryable<WarehouseProductLevelViewModel> GetLocationDataset(int tenantId, int warehouseId)
        {
            var tenantLocation = DependencyResolver.Current.GetService<ITenantLocationServices>();
            var transactions = tenantLocation.GetAllStockLevelsForWarehouse(warehouseId);
            return transactions;
        }

        public static void LocationGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetLocationDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("SKUCode");
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

        public static void LocationGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetLocationDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("SKUCode");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }


        public static GridViewModel CreateLocationGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "SKUCode";
            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("SKUCode");
            viewModel.Columns.Add("ReOrderQuantity");
            viewModel.Columns.Add("MinStockQuantity");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
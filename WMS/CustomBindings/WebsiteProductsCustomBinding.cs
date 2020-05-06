using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
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
    public class WebsiteProductsCustomBinding
    {
        private static IQueryable<object> GetWebsiteProductsDataset(int tenantId, int SiteId)
        {
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var transactions = tenantWebsiteService.GetAllValidWebsiteProducts(tenantId, SiteId);
            return transactions;
        }

        private static IQueryable<object> GetWebsiteProductsMapsDataset(int tenantId, int SiteId)
        {
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var transactions = tenantWebsiteService.GetAllValidWebsiteProductsMap(tenantId, SiteId);
            return transactions;
        }

        public static void ProductGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int SiteId,bool selected=false)
        {
            var transactions = GetWebsiteProductsDataset(tenantId, SiteId);
            if (selected)
            {
                transactions = GetWebsiteProductsMapsDataset(tenantId, SiteId);
            }

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

        public static void ProductGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int SiteId, bool selected = false)
        {

            var transactions = GetWebsiteProductsDataset(tenantId, SiteId);
            if (selected)
            {
                transactions = GetWebsiteProductsMapsDataset(tenantId, SiteId);
            }
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

        public static GridViewModel CreateWebsiteProductsViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("ProductId");
            viewModel.Columns.Add("SiteID");
            viewModel.Columns.Add("SKUCode");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("DepartmentName");
            viewModel.Columns.Add("ProductGroupName");
            viewModel.Columns.Add("ProductCategoryName");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static GridViewModel CreateWebsiteDiscountCodeProductsViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("ProductId");
            viewModel.Columns.Add("SiteID");
            viewModel.Columns.Add("SKUCode");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
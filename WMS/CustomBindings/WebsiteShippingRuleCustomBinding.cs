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
    public class WebsiteShippingRuleCustomBinding
    {
        private static IQueryable<object> GetWebsiteShippingRulesDataset(int tenantId, int SiteId)
        {
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var transactions = tenantWebsiteService.GetAllValidWebsiteShippingRules(tenantId, SiteId).Select(u => new
            {
                Id = u.Id,
                SiteName = u.TenantWebsites == null ? "" : u.TenantWebsites.SiteName,
                CountryName = u.GlobalCountry == null ? "" : u.GlobalCountry.CountryName,
                Courier=u.Courier,
                Region=u.Region,
                PostalArea=u.PostalArea,
                WeightinGrams=u.WeightinGrams,
                Price=u.Price,
                IsActive=u.IsActive,
                SortOrder=u.SortOrder

            });
            return transactions;
        }

        public static void WebsiteShippingRulesGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int SiteId)
        {
            var transactions = GetWebsiteShippingRulesDataset(tenantId, SiteId);

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
                transactions = transactions.OrderBy("SortOrder");
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

        public static void WebsiteShippingRulesDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int SiteId)
        {

            var transactions = GetWebsiteShippingRulesDataset(tenantId, SiteId);

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
                transactions = transactions.OrderBy("SortOrder");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static GridViewModel CreateWebsiteShippingRulesViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("SiteName");
            viewModel.Columns.Add("CountryName");
            viewModel.Columns.Add("Courier");
            viewModel.Columns.Add("Region");
            viewModel.Columns.Add("PostalArea");
            viewModel.Columns.Add("WeightinGrams");
            viewModel.Columns.Add("Price");
            viewModel.Columns.Add("SortOrder");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
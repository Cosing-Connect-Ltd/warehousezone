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
    public class WebsiteVouchersCustomBinding
    {
        private static IQueryable<object> GetWebsiteVouchersDataset(int tenantId, int SiteId)
        {
            var tenantWebsiteService = DependencyResolver.Current.GetService<ITenantWebsiteService>();
            var transactions = tenantWebsiteService.GetAllValidWebsiteVoucher(tenantId, SiteId).Select(u => new
            {
                Id = u.Id,
                SiteName = u.TenantWebsites == null ? "" : u.TenantWebsites.SiteName,
                u.Code,
                u.Value,
                u.Shared,
                UserName=u.AuthUser ==null?"": u.AuthUser.UserLastName + ", " + u.AuthUser.UserFirstName,
                IsActive=u.IsActive,

            });
            return transactions;
        }

        public static void WebsiteVouchersGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int SiteId)
        {
            var transactions = GetWebsiteVouchersDataset(tenantId, SiteId);

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
                transactions = transactions.OrderBy("Id");
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

        public static void WebsiteVouchersDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int SiteId)
        {

            var transactions = GetWebsiteVouchersDataset(tenantId, SiteId);

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
                transactions = transactions.OrderBy("Id");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static GridViewModel CreateWebsiteVouchersViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("SiteName");
            viewModel.Columns.Add("Code");
            viewModel.Columns.Add("Value");
            viewModel.Columns.Add("Shared");
            viewModel.Columns.Add("UserName");
            viewModel.Columns.Add("IsActive");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
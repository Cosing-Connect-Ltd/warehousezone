using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ApiCredentialListCustomBinding
    {

        private static IQueryable<object> GetApiCredentialDataset(int tenantId)
        {
            var ApiCredentialServices = DependencyResolver.Current.GetService<IApiCredentialServices>();
            var transactions = ApiCredentialServices.GetAll(tenantId)
                .Select(t => new ApiCredentialsViewModel {
                    ApiKey = t.ApiKey,
                    ApiUrl = t.ApiUrl,
                    Location = t.Warehouse.WarehouseName,
                    AccountNumber = t.AccountNumber,
                    ApiTypes = t.ApiTypes,
                    DefaultWarehouseId = t.DefaultWarehouseId,
                    ExpiryDate = t.ExpiryDate,
                    Id = t.Id,
                    Password = t.Password,
                    SiteID = t.SiteID,
                    SiteName = t.TenantWebsites.SiteName,
                    UserName = t.UserName
                });
            return transactions;
        }

        public static void ApiCredentialGetData(GridViewCustomBindingGetDataArgs e, int tenantId)
        {
            var transactions = GetApiCredentialDataset(tenantId);

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

        public static void ApiCredentialGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = GetApiCredentialDataset(tenantId);

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

        public static GridViewModel CreateApiCredentialGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";
            viewModel.Columns.Add("UserName");
            viewModel.Columns.Add("ApiUrl");
            viewModel.Columns.Add("ApiKey");
            viewModel.Columns.Add("ApiTypes");
            viewModel.Columns.Add("ExpiryDate");
            viewModel.Columns.Add("AccountNumber");
            viewModel.Columns.Add("SiteName");
            viewModel.Columns.Add("Location");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }
}
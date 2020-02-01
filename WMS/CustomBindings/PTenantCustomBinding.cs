using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class PTenantCustomBinding
    {
        private static IQueryable<PTenant> GetPTenantDataset(int? id)
        {
            
            var PropertyService = DependencyResolver.Current.GetService<IPropertyService>();
            var transactions = PropertyService.GetAllPropertyTenants(id);

            return transactions;
        }


        public static void PTenantGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e,int? id)
        {

            var transactions = GetPTenantDataset(id);

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
                transactions = transactions.OrderBy("PTenantId");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void PTenantGetData(GridViewCustomBindingGetDataArgs e,int? id)
        {

            var transactions = GetPTenantDataset(id);

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
                transactions = transactions.OrderBy("PTenantId");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }
            if (!id.HasValue)
            {
                transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            }
            e.Data = transactions.ToList();
        }

        public static GridViewModel CreatePTenantGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "PTenantId";

            viewModel.Columns.Add("TenantCode");
            viewModel.Columns.Add("TenancyStatus");
            viewModel.Columns.Add("CurrentPropertyCode");
            viewModel.Columns.Add("TenantFullName");
            viewModel.Columns.Add("Email");
            viewModel.Columns.Add("AddressLine1");
            viewModel.Columns.Add("AddressLine2");
            viewModel.Columns.Add("AddressPostcode");
            viewModel.Columns.Add("HomeTelephone");
            viewModel.Columns.Add("IsCurrentTenant");
            viewModel.Columns.Add("IsFutureTenant");
            


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
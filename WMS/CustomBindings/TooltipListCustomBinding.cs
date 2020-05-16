using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class TooltipListCustomBinding
    {

        private static IQueryable<object> GetTooltipDataset(int tenantId, bool isSuperUser)
        {
            var TooltipServices = DependencyResolver.Current.GetService<ITooltipServices>();
            var transactions = TooltipServices.GetAll(tenantId, isSuperUser);
            return transactions;
        }

        public static void TooltipGetData(GridViewCustomBindingGetDataArgs e, int tenantId, bool isSuperUser)
        {
            var transactions = GetTooltipDataset(tenantId, isSuperUser);

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
                transactions = transactions.OrderBy("Key");
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

        public static void TooltipGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, bool isSuperUser)
        {

            var transactions = GetTooltipDataset(tenantId, isSuperUser);

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
                transactions = transactions.OrderBy("Key");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static GridViewModel CreateTooltipGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "TooltipId";
            viewModel.Columns.Add("Key");
            viewModel.Columns.Add("Title");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("Localization");
            viewModel.Columns.Add("TenantId");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }
}
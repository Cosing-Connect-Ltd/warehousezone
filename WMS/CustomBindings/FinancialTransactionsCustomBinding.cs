using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;

namespace WMS.CustomBindings
{
    public class FinancialTransactionsCustomBinding
    {
        public static void FinancialTransactionsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId, int? accountId)
        {
            var accountServices = DependencyResolver.Current.GetService<IAccountServices>();

            var transactions = accountServices.GetTenantAccountTransactions(tenantId,accountId??0);
             
            if (e.State.SortedColumns.Any())
            {
                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                transactions = transactions.OrderBy(sortString);
            }

            if (e.FilterExpression != string.Empty)
            {
                var op = CriteriaOperator.Parse(e.FilterExpression);

                var filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void FinancialTransactionsGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId, int? accountId)
        {
            var accountServices = DependencyResolver.Current.GetService<IAccountServices>();

            var transactions = accountServices.GetTenantAccountTransactions(tenantId,accountId??0);

            if (e.State.SortedColumns.Any())
            {

                var sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                transactions = transactions.OrderBy(sortString);
            }

            if (e.FilterExpression != string.Empty)
            {
                var op = CriteriaOperator.Parse(e.FilterExpression);

                var filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);
            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            e.Data = transactions.ToList();
        }


        public static GridViewModel CreateFinancialTransactionsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "AccountTransactionId";

            viewModel.Columns.Add("AccountTransactionId");
            viewModel.Columns.Add("Notes");
            viewModel.Columns.Add("Amount");
            viewModel.Columns.Add("FinalBalance");
            viewModel.Columns.Add("AccountPaymentMode");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("AccountCode");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }
}
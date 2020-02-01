using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class AccountCustomBinding
    {
        private static IQueryable<object> GetAccountDataset(int CurrentTenantId,int? marketId)
        {
            var accountServices = DependencyResolver.Current.GetService<IAccountServices>();
            var marketservce = DependencyResolver.Current.GetService<IMarketServices>();
            var mid = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["Mid"]) ? HttpContext.Current.Request.Params["Mid"] : "0");
            if (marketId>0)
            {
                mid = marketId??0;
            }
            if (mid > 0)
            {
                var marketaccount = marketservce.GetMarketCustomersById(mid, CurrentTenantId, null);
                if (marketaccount != null && marketaccount.SelectedCustomers != null)
                {
                    var res = marketaccount.SelectedCustomers.ToList();
                    var result = res.Select(u => u.AccountId).ToList();
                    return accountServices.GetAllValidAccountbyList(result).Select(p => new
                    {
                        AccountID = p.AccountID,
                        AccountCode = p.AccountCode,
                        CompanyName = p.CompanyName,
                        CountryName = p.GlobalCountry.CountryName,
                        RegNo = p.RegNo,
                        VATNo = p.VATNo,
                        AccountEmail = p.AccountEmail,
                        Telephone = p.Telephone,
                        Currency = p.GlobalCurrency.CurrencyName,
                        AccountStatus = p.GlobalAccountStatus.AccountStatus,
                        Comments = p.Comments,
                        Fax = p.Fax,
                        Mobile = p.Mobile,
                        Web = p.website,
                        IsCustomer = p.AccountTypeCustomer,
                        IsSupplier = p.AccountTypeSupplier,
                        IsEndUser = p.AccountTypeEndUser,
                        p.FinalBalance,
                        MarketId = mid
                        

                    });
                }

            }
            var transactions = from p in accountServices.GetAllValidAccountsCustom(CurrentTenantId)
                               select new
                               {
                                   AccountID = p.AccountID,
                                   AccountCode = p.AccountCode,
                                   CompanyName = p.CompanyName,
                                   CountryName = p.GlobalCountry.CountryName,
                                   RegNo = p.RegNo,
                                   VATNo = p.VATNo,
                                   AccountEmail = p.AccountEmail,
                                   Telephone = p.Telephone,
                                    Currency = p.GlobalCurrency.CurrencyName,
                                   AccountStatus = p.GlobalAccountStatus.AccountStatus,
                                   Comments = p.Comments,
                                   Fax = p.Fax,
                                   Mobile = p.Mobile,
                                   Web = p.website,
                                   IsCustomer = p.AccountTypeCustomer,
                                   IsSupplier = p.AccountTypeSupplier,
                                   IsEndUser = p.AccountTypeEndUser,
                                   p.FinalBalance,
                                   MarketId=0

                               };


            return transactions;
        }


        public static void AccountGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int? marketId)
        {

            var transactions = GetAccountDataset(tenantId,marketId);

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
                transactions = transactions.OrderBy("AccountCode");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void AccountGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int? marketId)
        {

            var transactions = GetAccountDataset(tenantId,marketId);

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
                transactions = transactions.OrderBy("AccountCode");
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

        public static GridViewModel CreateAccountGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "AccountID";

            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("RegNo");
            viewModel.Columns.Add("VATNo");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("CountryName");
            viewModel.Columns.Add("Currency");
            viewModel.Columns.Add("AccountEmail");
            viewModel.Columns.Add("Telephone");
            viewModel.Columns.Add("AccountStatus");
            viewModel.Columns.Add("Comments");
            viewModel.Columns.Add("Market");
            viewModel.Columns.Add("Fax");
            viewModel.Columns.Add("Mobile");
            viewModel.Columns.Add("MarketId");
            viewModel.Columns.Add("Web");
            viewModel.Columns.Add("IsCustomer");
            viewModel.Columns.Add("IsSupplier");
            viewModel.Columns.Add("IsEndUser");
            viewModel.Columns.Add("FinalBalance");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
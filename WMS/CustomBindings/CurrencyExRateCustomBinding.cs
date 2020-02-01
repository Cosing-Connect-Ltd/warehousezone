using DevExpress.Data.Filtering;
using DevExpress.DataProcessing;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class CurrencyExRateCustomBinding
    {
        public static GridViewModel CreateCurrencyExRateListViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "CurrencyID";
            viewModel.Columns.Add("CurrencyID");
            viewModel.Columns.Add("CurrencyName");
            viewModel.Columns.Add("CurrencySymbol");
            viewModel.Columns.Add("ExchangeRate");
            viewModel.Columns.Add("DateUpdated");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        public static void CurrencyExRateGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {


            var rates = GetCurrencyExRateListDataset(tenantId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                rates = rates.OrderBy(sortString);
            }
            else
            {
                rates = rates.OrderByDescending(d => Convert.ToDateTime(d.DateUpdated));
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                rates = rates.Where(filterString);
            }

            e.DataRowCount = rates.Count();

        }

        private static IQueryable<CurrencyExRateViewModel> GetCurrencyExRateListDataset(int tenantId)
        {
            var CurrencyExRateServices = DependencyResolver.Current.GetService<ITenantsCurrencyRateServices>();
            var currenciesExRate = CurrencyExRateServices.GetTenantCurrenciesExRates(tenantId);
            var currencyRate = currenciesExRate
                .Select(x => new CurrencyExRateViewModel
                 {
                     CurrencyId = x.TenantCurrencyID,
                     CurrencyName = x.TenantCurrencies.GlobalCurrency.CurrencyName,
                     CurrencySymbol = x.TenantCurrencies.GlobalCurrency.Symbol,
                     ExchangeRate = x.Rate,
                     DateUpdated = x.DateUpdated
                 });

            return currencyRate.AsQueryable();
        }

        public static void CurrencyExRateGetData(GridViewCustomBindingGetDataArgs e, int tenantId)
        {
            var rates = GetCurrencyExRateListDataset(tenantId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                rates = rates.OrderBy(sortString);
            }
            else
            {
                rates = rates.OrderByDescending(d => Convert.ToDateTime(d.DateUpdated)); 
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                rates = rates.Where(filterString);

            }

            rates = rates.Skip(e.StartDataRowIndex).Take(e.DataRowCount);



            e.Data = rates.ToList();
        }
    }
}
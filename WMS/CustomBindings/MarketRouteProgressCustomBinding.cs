using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class MarketRouteProgressCustomBinding
    {
        private static IQueryable<object> MarketRouteProgressDataset(int teanantId)
        {
            

            var marketServices = DependencyResolver.Current.GetService<IMarketServices>();
            var chosenDate = DateTime.MinValue;
            var marketrouteId = 0;

            var orderServices = DependencyResolver.Current.GetService<IOrderService>();

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["id"]) && HttpContext.Current.Request.Params["id"] != "Select date...")
            {
                chosenDate = DateTime.Parse(HttpContext.Current.Request.Params["id"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["marketrouteId"]) && int.Parse(HttpContext.Current.Request.Params["marketrouteId"]) > 0)
            {
                marketrouteId = int.Parse(HttpContext.Current.Request.Params["marketrouteId"]);
            }

            var transactions = marketServices.GetMarketRouteProgresses(marketrouteId, chosenDate, teanantId).Where(u => (u.MarketRouteId == marketrouteId || marketrouteId == 0) && (DbFunctions.TruncateTime(u.DateCreated) == chosenDate.Date || chosenDate == DateTime.MinValue))
                .Select(u => new
                {
                    EmployeeId = u.RouteProgressId,
                    MarketName = u.Market.Name ?? "",
                    MarketRoute = u.MarketRoute.Name ?? "",
                    AccountName = u.Account.CompanyName ?? "",
                    Comment = u.Comment,
                    DateCreated = u.DateCreated,
                   
                });
            
                              
                               
            return transactions;
        }


        public static void MarketRouteProgressGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int teanantId)
        {

            var transactions = MarketRouteProgressDataset(teanantId);

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
                transactions = transactions.OrderBy("DateCreated");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void MarketRouteProgressGetData(GridViewCustomBindingGetDataArgs e, int teanantId)
        {

            var transactions = MarketRouteProgressDataset(teanantId);

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
                transactions = transactions.OrderBy("DateCreated");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            var employeeLists = transactions.ToList();
            e.Data = employeeLists.ToList();
        }

        public static GridViewModel MarketRouteProgressGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "RouteProgressId";
            viewModel.Columns.Add("MarketName");
            viewModel.Columns.Add("MarketRoute");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("Comment");
            viewModel.Columns.Add("DateCreated");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
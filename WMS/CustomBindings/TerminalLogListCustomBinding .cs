using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class TerminalLogListCustomBinding
    {

        public static GridViewModel CreateTerminalLogListGridViewModel()
        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "TerminalId";
            viewModel.Columns.Add("Warehouse");
            viewModel.Columns.Add("TerminalName");
            viewModel.Columns.Add("TermainlSerial");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("IsActive");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void TerminalLogListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int terminalId)
        {


            var transactions = GetTerminalLogListDataset(terminalId);

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
                transactions = transactions.OrderBy("DateRequest Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        private static IQueryable<TerminalsLog> GetTerminalLogListDataset(int TerminalId)
        {
            var terminalServices = DependencyResolver.Current.GetService<ITerminalServices>();
            var transactions = terminalServices.GetTerminalLogById(TerminalId);
            return transactions;
        }
        public static void TerminalLogListGetData(GridViewCustomBindingGetDataArgs e, int terminalId)
        {
            var transactions = GetTerminalLogListDataset(terminalId);

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
                transactions = transactions.OrderBy("DateCreated Desc");
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
    }
}
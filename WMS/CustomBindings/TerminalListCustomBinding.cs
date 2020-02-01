using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
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
    public class TerminalListCustomBinding
    {
        public static GridViewModel CreateTerminalListGridViewModel()
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
        public static void TerminalListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetTerminalListDataset(tenantId, warehouseId);

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

            e.DataRowCount = transactions.Count();

        }

        private static IQueryable<TerminalsListViewModel> GetTerminalListDataset(int tenantId, int warehouseId)
        {
            var terminalServices = DependencyResolver.Current.GetService<ITerminalServices>();
            var transactions = terminalServices.GetAllTerminalsForGrid(tenantId, warehouseId);
            var transtion = transactions
                 .Select(x => new TerminalsListViewModel
                 {
                     TerminalId = x.TerminalId,
                     Warehouse = x.TenantWarehous.WarehouseName,
                     TerminalName = x.TerminalName,
                     TermainlSerial = x.TermainlSerial,
                     DateCreated = x.DateCreated,
                     DateUpdated = x.DateUpdated,
                     IsActive = x.IsActive
                 });

            return transtion;
        }
        public static void TerminalListGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetTerminalListDataset(tenantId, warehouseId);

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
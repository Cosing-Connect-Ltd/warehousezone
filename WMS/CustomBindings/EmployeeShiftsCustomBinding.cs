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
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class EmployeeShiftsCustomBinding
    {
        private static IQueryable<ResourceShifts> EmployeeShiftsDataset(DateTime? searchdate, int teanantId, int warehouseId)
        {
            var shiftServices = DependencyResolver.Current.GetService<EmployeeShiftsServices>();
            var transactions = shiftServices.SearchByDate(searchdate, teanantId, warehouseId);
            return transactions;
        }


        public static void EmployeeShiftsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, DateTime? searchdate, int teanantId, int warehouseId)
        {

            var transactions = EmployeeShiftsDataset(searchdate, teanantId, warehouseId);

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
                transactions = transactions.OrderBy("TimeStamp Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void EmployeeShiftsGetData(GridViewCustomBindingGetDataArgs e, DateTime? searchdate, int teanantId, int warehouseId)
        {

            var transactions = EmployeeShiftsDataset(searchdate, teanantId, warehouseId);
            var _mapper = DependencyResolver.Current.GetService<IMapper>();


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
                transactions = transactions.OrderBy("TimeStamp Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            var result = transactions.ToList();
            var data = _mapper.Map<List<EmployeeShiftsViewModel>>(result);

            e.Data = data.ToList();
        }

        public static GridViewModel CreateEmployeeShiftsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";

            viewModel.Columns.Add("ResourceId");
            viewModel.Columns.Add("Resources.FirstName");
            viewModel.Columns.Add("Resources.SurName");
            viewModel.Columns.Add("Resources.EmployeeRoles.Roles.RoleName");
            viewModel.Columns.Add("Date");
            viewModel.Columns.Add("TimeStamp");
            viewModel.Columns.Add("StatusType");
            viewModel.Columns.Add("Terminals.TerminalName");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
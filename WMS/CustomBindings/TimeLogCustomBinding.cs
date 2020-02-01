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
    public class TimeLogCustomBinding
    {
        private static IQueryable<EmployeesTimeLogsViewModel> TimeLogDataset(int teanantId, int locationId, int weekNumber,int year)
        {
            
            var shiftServices = DependencyResolver.Current.GetService<EmployeeServices>();
            var transactions = shiftServices.GetAllEmployeesByLocation(teanantId, locationId).Select(u=> new EmployeesTimeLogsViewModel()
                {
                    EmployeeId = u.ResourceId,
                    PayrollEmployeeNo = u.PayrollEmployeeNo,
                    FirstName = u.FirstName,
                    SurName = u.SurName,
                    WeekNumber = weekNumber,
                    years = year,
                    EmployeeRole = u.EmployeeRoles.Count() <= 0 ? "" : u.EmployeeRoles.Where(x => x.IsDeleted != true).FirstOrDefault().Roles.RoleName
                });
            
                              
                               
            return transactions;
        }


        public static void TimeLogGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int teanantId, int locationId, int weekNumber, int year)
        {

            var transactions = TimeLogDataset(teanantId,locationId,weekNumber,year);

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
                transactions = transactions.OrderBy("EmployeeId");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void TimeLogGetData(GridViewCustomBindingGetDataArgs e, int teanantId, int locationId, int weekNumber, int year)
        {

            var transactions = TimeLogDataset(teanantId,locationId,weekNumber,year);

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
                transactions = transactions.OrderBy("EmployeeId");
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

        public static GridViewModel TimeLogGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "EmployeeId";
            viewModel.Columns.Add("FirstName");
            viewModel.Columns.Add("SurName");
            viewModel.Columns.Add("EmployeeRole");
            viewModel.Columns.Add("PayrollEmployeeNo");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
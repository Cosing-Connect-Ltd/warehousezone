using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ResourceCustomBinding
    {
        private static IQueryable<ResourceRequests> ResourceRequestsJobsDataset(int CurrentTenantId)
        {

            var employeeServices = DependencyResolver.Current.GetService<IEmployeeServices>();

            var transactions = employeeServices.GetAllResourceRequests(CurrentTenantId);

            var requestStatus = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["RequestStatus"]) ? HttpContext.Current.Request.Params["RequestStatus"] : "0");

            var RequestType = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["RequestType"]) ? HttpContext.Current.Request.Params["RequestType"] : "0");

            if (RequestType > 0)
            {
                transactions = transactions.Where(o => (int)o.RequestType == RequestType);
            }
            if (requestStatus > 0)
            {
                transactions = transactions.Where(o => (int)o.RequestStatus == requestStatus);
            }

            return transactions;
        }


        public static void ResourceRequestsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = ResourceRequestsJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("RequestedDate Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void ResourceRequestsGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = ResourceRequestsJobsDataset(tenantId);
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
                transactions = transactions.OrderBy("RequestedDate Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            List<ResourceRequests> data = transactions.ToList();

            var results = new List<ResourceRequestsViewModel>();

            foreach (var item in data)
            {
                var result = _mapper.Map(item, new ResourceRequestsViewModel());
                result.ResourceName = item.Resources.Name;
                results.Add(result);
            }

            e.Data = results;

        }

        public static GridViewModel CreateUnallocatedJobsGridViewModel()

        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";

            viewModel.Columns.Add("ResourceId");
            viewModel.Columns.Add("StartDate");
            viewModel.Columns.Add("EndDate");
            viewModel.Columns.Add("HolidayReason");
            viewModel.Columns.Add("RequestedDate");
            viewModel.Columns.Add("Label");
            viewModel.Columns.Add("Location");
            viewModel.Columns.Add("AllDay");
            viewModel.Columns.Add("EventType");
            viewModel.Columns.Add("RecurrenceInfo");
            viewModel.Columns.Add("OrderNotes");
            viewModel.Columns.Add("ReminderInfo");
            viewModel.Columns.Add("ResourceIDs");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("IsAccepted");
            viewModel.Columns.Add("IsAnnualHoliday");
            viewModel.Columns.Add("IsCanceled");
            viewModel.Columns.Add("CancelReason");
            viewModel.Columns.Add("RequestType");
            viewModel.Columns.Add("Notes");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }





        private static IQueryable<Resources> ResourceListDataset(int CurrentTenantId, int CurrentWarehouseId)
        {

            var employeeServices = DependencyResolver.Current.GetService<IEmployeeServices>();

            var transactions = employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId).Where(x => x.EmployeeShifts_Stores.Any(a => a.WarehouseId == CurrentWarehouseId) || x.EmployeeShifts_Stores.Count == 0);


            return transactions;
        }


        public static void ResourceListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int CurrentWarehouseId)
        {

            var transactions = ResourceListDataset(tenantId, CurrentWarehouseId);

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
                transactions = transactions.OrderBy("ResourceId Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void ResourceListGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId, string exportRowType)
        {
            var transactions = ResourceListDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("ResourceId Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            if (exportRowType != "All")
            {
                transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            }

            e.Data = transactions.ToList();

        }

        public static GridViewModel CreateResourceListGridViewModel()

        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "ResourceId";

            viewModel.Columns.Add("PayrollEmployeeNo");
            viewModel.Columns.Add("FirstName");
            viewModel.Columns.Add("SurName");
            viewModel.Columns.Add("Gender");
            viewModel.Columns.Add("HomeNumber");
            viewModel.Columns.Add("MobileNumber");
            viewModel.Columns.Add("Fax");
            viewModel.Columns.Add("EmailAddress");
            viewModel.Columns.Add("InternalStaff");
            viewModel.Columns.Add("IsActive");
            viewModel.Columns.Add("Jtypes");
            viewModel.Columns.Add("JobTypes");



            viewModel.Pager.PageSize = 10;
            return viewModel;
        }












    }
}
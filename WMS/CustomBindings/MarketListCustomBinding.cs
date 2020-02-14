using AutoMapper;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
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
    public class MarketListCustomBinding
    {
        private static IQueryable<MarketJob> MarketJobsDataset(int CurrentTenantId)
        {

            var employeeServices = DependencyResolver.Current.GetService<IMarketServices>();
            var RequestType = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["RequestType"]) ? HttpContext.Current.Request.Params["RequestType"] : "0");
            var transactions = employeeServices.GetAllValidMarketJobs(CurrentTenantId, (MarketJobStatusEnum)RequestType);
            return transactions;
        }


        public static void MarketGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = MarketJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("Id asc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void MarketGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = MarketJobsDataset(tenantId);
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
                transactions = transactions.OrderBy("Id asc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            List<MarketJob> data = transactions.ToList();
            var results = new List<MarketJobViewModel>();

            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();


            foreach (var item in data)
            {
                var model = _mapper.Map(item, new MarketJobViewModel());
                model.DisplayText = item.MarketRoute != null ? item.MarketRoute.Description : "";
                var jobAllocation = _currentDbContext.MarketJobAllocations.FirstOrDefault(x => x.MarketJobId == item.Id && (x.MarketJobStatusId != (int)MarketJobStatusEnum.Cancelled));
                model.MarketJobStatusEnum = (MarketJobStatusEnum)item.LatestJobStatusId;
                if (jobAllocation != null)
                {
                    model.ResourceName = jobAllocation.Resource.Name;
                    model.ResourceID = jobAllocation.ResourceId;
                    model.Reason = jobAllocation.Reason;
                }
                results.Add(model);
            }

            e.Data = results;

        }

        public static GridViewModel CreateMarketGridViewModel()

        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "Id";

            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("TenantId");
            viewModel.Columns.Add("DisplayText");
            viewModel.Columns.Add("ResourceName");
            viewModel.Columns.Add("MarketJobStatusEnum");
            viewModel.Columns.Add("DateCreated");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
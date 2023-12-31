﻿using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ConsignmentCustomBinding
    {

        private static IQueryable<DelieveryViewModel> ConsignmentDataset(int tenantId, int warehouseId, OrderProcessStatusEnum? orderStatusId)
        {
            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();

            var transactions = orderServices.GetAllSalesConsignments(tenantId, warehouseId, orderstatusId: orderStatusId)
                                            .OrderBy(u => u.Order.SLAPriorityId != null)
                                            .ThenBy(u => u.Order.SLAPriority.SortOrder)
                                            .ThenByDescending(x => x.DateCreated)
                        .Select(ops => new DelieveryViewModel
                        {
                            DeliveryNO = ops.DeliveryNO,
                            OrderID = ops.OrderID,
                            DateCreated = ops.DateCreated,
                            OrderProcessID = ops.OrderProcessID,
                            OrderNumber = ops.Order.OrderNumber,
                            AccountCode = ops.Order.Account.AccountCode,
                            CompanyName = ops.Order.Account.CompanyName,
                            Status = ops.OrderProcessStatusId,
                            orderstatus = ops.Order.OrderStatusID,
                            PickContainerCode = ops.PickContainerCode,
                            SLAPrioritySortOrder = ops.Order.SLAPriorityId != null ? (ops.Order.SLAPriority.SortOrder != null ? ops.Order.SLAPriority.SortOrder.Value : 999) : 999,
                        });

            return transactions;
        }

        public static void ConsignmentDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId, OrderProcessStatusEnum? orderProcessStatus)
        {

            var transactions = ConsignmentDataset(tenantId, warehouseId, orderProcessStatus);

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
                var op = CriteriaOperator.Parse(e.FilterExpression);
                var filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void ConsignmentData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId, OrderProcessStatusEnum? orderProcessStatus)
        {
            var transactions = ConsignmentDataset(tenantId, warehouseId, orderProcessStatus);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += "SLAPrioritySortOrder," + column.FieldName + " " + column.SortOrder;
                }
                transactions = transactions.OrderBy(sortString);
            }
            else
            {
                transactions = transactions.OrderBy("SLAPrioritySortOrder,DateCreated Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                var op = CriteriaOperator.Parse(e.FilterExpression);

                var filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var result = transactions.ToList();
            result.ForEach(u => u.EmailCount = context.TenantEmailNotificationQueues.Count(c => c.OrderId == u.OrderID && c.TenantEmailTemplatesId == 4));
            e.Data = result.ToList();
        }


        public static GridViewModel CreateConsignmentGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderProcessID";


            viewModel.Columns.Add("PickContainerCode");
            viewModel.Columns.Add("DeliveryNO");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("EmailCount");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }



    }

}
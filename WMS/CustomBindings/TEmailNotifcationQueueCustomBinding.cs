using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class TEmailNotifcationQueueCustomBinding
    {
        private static IQueryable<object> GetNotificationQueueDataset(int tenantId)
        {
            var lookupServices = DependencyResolver.Current.GetService<ILookupServices>();
            var transactions = lookupServices.GetEmailNotifcationQueue(tenantId).Select(u => new {

                TenantEmailNotificationQueueId = u.TenantEmailNotificationQueueId,
                OrderNumber = u.Order==null?"":u.Order.OrderNumber,
                EmailSubject = u.EmailSubject,
                CustomEmailMessage = u.CustomEmailMessage,
                CustomRecipients = u.CustomRecipients,
                TenantEmailTemplate =u.TenantEmailTemplates.NotificationType,
                AppointmentSubject=u.Appointment==null?"": u.Appointment.Subject,
                IsNotificationCancelled=u.IsNotificationCancelled,
                ScheduledProcessingTime= u.ScheduledProcessingTime,
                InvoiceNumber=u.InvoiceMaster==null?"":u.InvoiceMaster.InvoiceNumber

            });
            
            return transactions;
        }

        public static void NotificationQueueGetData(GridViewCustomBindingGetDataArgs e, int tenantId)
        {
            var transactions = GetNotificationQueueDataset(tenantId);

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
                //transactions = transactions.OrderBy("ScheduledProcessingTime");
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

        public static void NotificationQueueGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = GetNotificationQueueDataset(tenantId);

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
                //transactions = transactions.OrderBy("ScheduledProcessingTime");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }


        public static GridViewModel CreateNotificationQueueGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "TenantEmailNotificationQueueId";
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("EmailSubject");
            viewModel.Columns.Add("CustomEmailMessage");
            viewModel.Columns.Add("CustomRecipients");
            viewModel.Columns.Add("TenantEmailTemplate");
            viewModel.Columns.Add("AppointmentSubject");
            viewModel.Columns.Add("IsNotificationCancelled");
            viewModel.Columns.Add("ScheduledProcessingTime");
            viewModel.Columns.Add("InvoiceNumber");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
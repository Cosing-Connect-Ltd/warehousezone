using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
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
    public class PickListCustomBinding
    {
        public static DateTime recurrenceDate = DateTime.MinValue;
        public static DateTime? expectedDate;

        public static GridViewModel CreatePickListGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("OrderTypeId");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("Property");
            viewModel.Columns.Add("ResourceName");
            viewModel.Columns.Add("Notes");
            viewModel.Columns.Add("POStatus").ForceType(typeof(string));
            viewModel.Columns.Add("Account");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

        private static IQueryable<PendingListDto> GetPickListDataset(int tenantId, int warehouseId, int UserId)
        {
            DateTime picklistDate;
            expectedDate = null;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["id"]))
            {
                var validDate = DateTime.TryParse(HttpContext.Current.Request.Params["id"], out picklistDate);
                if (validDate)
                {
                    expectedDate = Convert.ToDateTime(HttpContext.Current.Request.Params["id"]);
                }

            }
            else { expectedDate = DateTime.Today; }

            bool success = DateTime.TryParse(expectedDate.ToString(), out recurrenceDate);
            var productServices = DependencyResolver.Current.GetService<IOrderService>();
            var transactions = productServices.GetAllPendingOrdersForProcessingForDate();
            var PendingListDto = transactions.Where(o => ((o.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder && (!expectedDate.HasValue ||
                                 DbFunctions.TruncateTime(o.ExpectedDate) == expectedDate) && o.OrderStatusID == (int)OrderStatusEnum.Active)
                                 ||
                                 (o.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && (!expectedDate.HasValue ||
                                 DbFunctions.TruncateTime(o.Appointmentses.Max(m => m.StartTime)) == expectedDate || o.Appointmentses.Max(x => x.RecurrenceInfo) != null)
                                 && (o.OrderStatusID == (int)OrderStatusEnum.Scheduled))) && o.TenentId == tenantId && o.IsDeleted != true && (o.PickerId==UserId || o.PickerId==null))
                               .Include(m => m.OrderStatus)
                               .Include(m => m.TransactionType)
                               .Include(m => m.Account)
                               .Include(m => m.Account.GlobalAccountStatus)
                               .Include(m => m.Account.AccountAddresses)
                               .Include(m => m.Appointmentses)
                               .Include(m => m.Appointmentses.Select(n => n.AppointmentResources))
                               .Include(m => m.OrderNotes)
                               .Include(m => m.PProperties)
                               .Include(m => m.OrderDetails).Select(p => new PendingListDto
                               {
                                   OrderID = p.OrderID,
                                   WarehouseID = p.WarehouseId ?? 0,
                                   OrderNumber = p.OrderNumber,
                                   IssueDate = p.IssueDate,
                                   DateUpdated = p.DateUpdated,
                                   ResourceName = (p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault() != null
                          && p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault().AppointmentResources != null)
                          ? p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault().AppointmentResources.FirstName
                          + " " + p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault().AppointmentResources.SurName : String.Empty,
                                   POStatus = p.OrderStatus.Status,
                                   Account = p.Account != null ? p.Account.AccountCode : "",
                                   OrderTypeId = p.InventoryTransactionTypeId,
                                   OrderType = p.TransactionType.OrderType,
                                   AccountStatus = (p.Account != null && p.Account.GlobalAccountStatus != null) ? p.Account.GlobalAccountStatus.AccountStatus : "",
                                   OrderTotal = p.OrderTotal,
                                   Property = p.PProperties != null ? p.PProperties.AddressLine1 : "",
                                   OrderNotesList = p.OrderNotes.Where(m => m.IsDeleted != true).Select(s => new OrderNotesViewModel()
                                   {
                                       OrderNoteId = s.OrderNoteId,
                                       Notes = s.Notes,
                                       NotesByName = s.User.UserName ?? "",
                                       NotesDate = s.DateCreated
                                   }),
                                   OrderNotes = p.OrderNotes.Where(m => m.IsDeleted != true).Where(a => a.IsDeleted != true && a.TenantId == tenantId)
                              .Select(a => a.Notes),
                                   CreditLimit = (p.Account != null && p.Account.CreditLimit != null) ? p.Account.CreditLimit : 0,
                                   AddTypeShipping = (p.Account != null && p.Account.AccountAddresses.Count() > 0) ? p.Account.AccountAddresses.Any(a => a.AddTypeShipping == true) : false,
                                   RecurrenceInfo = p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault() != null ?
                              p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault().RecurrenceInfo : "",
                                   ScheduledStartTime = p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault() != null ?
                          p.Appointmentses.Where(m => !m.IsCanceled).OrderByDescending(m => m.StartTime).FirstOrDefault().StartTime : DateTime.MinValue,
                                   OrderDetails = p.OrderDetails.Where(x => x.IsDeleted != true)

                               });



            return PendingListDto;
        }

        public static void PickListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId,int UserId)
        {

            var transactions = GetPickListDataset(tenantId, warehouseId,UserId);

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
                transactions = transactions.OrderBy("IssueDate Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            var filter = transactions.ToList().Where(x => x.RecurrenceInfoStartDate?.Date <= recurrenceDate.Date
                && x.RecurrenceInfoEndDate?.Date >= recurrenceDate.Date || !expectedDate.HasValue || x.ScheduledStartTime.Value.Date == expectedDate);

            e.DataRowCount = transactions.Count();

        }

        public static void PickListGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId,int UserId)
        {
            var transactions = GetPickListDataset(tenantId, warehouseId,UserId);

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
                transactions = transactions.OrderBy("IssueDate Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            var filter = transactions.ToList();

            //filter = filter.Where(x => x.RecurrenceInfoStartDate?.Date <= recurrenceDate.Date
            //    && x.RecurrenceInfoEndDate?.Date >= recurrenceDate.Date || !expectedDate.HasValue || x.ScheduledStartTime.Value.Date == expectedDate).ToList();

            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();

            foreach (var item in filter)
            {
                item.FullShip = true;
                if (item.OrderDetails.Count() > 0)
                {
                    bool fullShip = true;
                    foreach (var detail in item.OrderDetails)
                    {
                        var available = _currentDbContext.InventoryStocks.AsNoTracking().Where(x => x.TenantId == tenantId && x.ProductId == detail.ProductId && x.WarehouseId == warehouseId).FirstOrDefault()?.Available ?? 0;
                        var qtyToProcess = detail.ProcessedQty - detail.ReturnedQty;
                        if (available < qtyToProcess)
                        {
                            fullShip = false;
                            break;
                        }
                    }

                    item.FullShip = fullShip;
                }
            }

            e.Data = filter.ToList();
        }


    }
}
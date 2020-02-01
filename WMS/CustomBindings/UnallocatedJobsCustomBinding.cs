using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class UnallocatedJobsCustomBinding
    {
        private static IQueryable<object> GetUnallocatedJobsDataset(int CurrentTenantId)
        {

            var orderServices = DependencyResolver.Current.GetService<IOrderService>();

            var selectedJobType = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["UASelectedJobType"]) ? HttpContext.Current.Request.Params["UASelectedJobType"] : "0");

            if (selectedJobType > 0)
            {
                var transactions = from p in orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId).Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.NotScheduled && o.JobTypeId == selectedJobType)
                                   select new
                                   {
                                       OrderID = p.OrderID,
                                       OrderNumber = p.OrderNumber,
                                       IssueDate = p.IssueDate,
                                       JobNotes = p.Note,
                                       JobTypeName = p.JobType.Name ?? "",
                                       JobSubTypeName = p.JobSubType.Description ?? "",
                                       DateUpdated = p.DateUpdated,
                                       POStatus = p.OrderStatus.Status ?? "",
                                       Account = p.Account.AccountCode ?? "",
                                       Property = p.PProperties.AddressLine1 ?? "",
                                       OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                                       OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
                                       OrderType = p.TransactionType.OrderType
                                   };
                return transactions;


            }
            else
            {
                var transactions = from p in orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId).Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.NotScheduled)
                                   select new
                                   {
                                       OrderID = p.OrderID,
                                       OrderNumber = p.OrderNumber,
                                       IssueDate = p.IssueDate,
                                       JobNotes = p.Note,
                                       JobTypeName = p.JobType.Name ?? "",
                                       JobSubTypeName = p.JobSubType.Description ?? "",
                                       DateUpdated = p.DateUpdated,
                                       POStatus = p.OrderStatus.Status ?? "",
                                       Account = p.Account.AccountCode ?? "",
                                       Property = p.PProperties.AddressLine1 ?? "",
                                       OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                                       OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
                                       OrderType = p.TransactionType.OrderType

                                   };



                return transactions;


            }

        }

        public static void UnallocatedJobsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = GetUnallocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void UnallocatedJobsGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetUnallocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
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

        public static GridViewModel CreateUnallocatedJobsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";

            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("IssueDate");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Note");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("AddressLine1");
            viewModel.Columns.Add("OrderNotes");
            viewModel.Columns.Add("OrderType");


            viewModel.Pager.PageSize = 20;
            return viewModel;
        }

        private static IQueryable<object> GetAllocatedJobsDataset(int CurrentTenantId)
        {
            var orderServices = DependencyResolver.Current.GetService<IOrderService>();

            var transactions = from p in orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId)
            .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.Scheduled)
                               select new
                               {
                                   OrderID = p.OrderID,
                                   OrderNumber = p.OrderNumber,
                                   IssueDate = p.IssueDate,
                                   JobNotes = p.Note,
                                   JobTypeName = p.JobType.Name ?? "",
                                   JobSubTypeName = p.JobSubType.Description ?? "",
                                   DateUpdated = p.DateUpdated,
                                   POStatus = p.OrderStatus.Status ?? "",
                                   Account = p.Account.AccountCode ?? "",
                                   Property = p.PProperties.AddressLine1 ?? "",
                                   OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                                   OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
                                   OrderType = p.TransactionType.OrderType,
                                   Appointment = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true)
                               };
            return transactions;





        }
        public static void AllocatedJobsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = GetAllocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void AllocatedJobsGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetUnallocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
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
        public static GridViewModel CreateAllocatedJobsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("IssueDate");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Note");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("AddressLine1");
            viewModel.Columns.Add("InventoryTransactionTypeId");
            viewModel.Columns.Add("OrderNotes");
            viewModel.Columns.Add("OrderType");
            viewModel.Columns.Add("Appointment");


            viewModel.Pager.PageSize = 20;
            return viewModel;


            //var model = OrderService.GetAllOrdersIncludingNavProperties(CurrentTenantId)
            //    .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.Scheduled)
            //    .Select(p => new WorksOrdersScheduledGridViewModel
            //    {
            //        OrderID = p.OrderID,
            //        OrderNumber = p.OrderNumber,
            //        IssueDate = p.IssueDate,
            //        JobNotes = p.Note,
            //        JobTypeName = p.JobType.Name ?? "",
            //        JobSubTypeName = p.JobSubType.Description ?? "",
            //        DateUpdated = p.DateUpdated,
            //        POStatus = p.OrderStatus.Status ?? "",
            //        Account = p.Account.AccountCode ?? "",
            //        Property = p.PProperties.AddressLine1 ?? "",
            //        OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
            //        OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
            //        OrderType = p.TransactionType.OrderType,
            //        Appointment = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true)
            //    }).ToList();

            //if (!string.IsNullOrEmpty(Request.Params["id"]) && Request.Params["id"] != "Select date...")
            //{
            //    var chosenDate = DateTime.Parse(Request.Params["id"]);
            //    model = model.Where(x => x.ScheduledStartTime.Value.Date == chosenDate.Date).ToList();
            //}

            //var resourceId = int.Parse(!string.IsNullOrEmpty(Request.Params["resourceId"]) ? Request.Params["resourceId"] : "0");

            //if (resourceId > 0)
            //{
            //    model = model.Where(x => x.ResourceId == resourceId).ToList();
            //}

            //var resources = _employeeServices.GetAllActiveAppointmentResourceses(CurrentTenantId).Select(x => new SelectListItem() { Text = x.Name, Value = x.ResourceId.ToString() }).ToList();
            //resources.Insert(0, new SelectListItem() { Text = "All", Value = "0" });
            //ViewBag.WorksResources = resources;

            //return PartialView("_AllocatedJobsPartial", model);
        }

        private static IQueryable<object> GetReallocatedJobsDataset(int CurrentTenantId)
        {
            var orderServices = DependencyResolver.Current.GetService<IOrderService>();

            //var transactions = from p in orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId)
            //.Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.Scheduled)
            //                   select new
            //                   {
            //                       OrderID = p.OrderID,
            //                       OrderNumber = p.OrderNumber,
            //                       IssueDate = p.IssueDate,
            //                       JobNotes = p.Note,
            //                       JobTypeName = p.JobType.Name ?? "",
            //                       JobSubTypeName = p.JobSubType.Description ?? "",
            //                       DateUpdated = p.DateUpdated,
            //                       POStatus = p.OrderStatus.Status ?? "",
            //                       Account = p.Account.AccountCode ?? "",
            //                       Property = p.PProperties.AddressLine1 ?? "",
            //                       OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
            //                       OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
            //                       OrderType = p.TransactionType.OrderType,
            //                       Appointment = p.Appointmentses.OrderByDescending(x => x.StartTime).FirstOrDefault(x => x.IsCanceled != true)
            //                   };

            var selectedJobType = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["RASelectedJobType"]) ? HttpContext.Current.Request.Params["RASelectedJobType"] : "0");

            if (selectedJobType > 0)
            {
                var transactions = orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId)
                    .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.ReAllocationRequired && o.JobTypeId == selectedJobType)
                    .Select(p => new WorksOrdersGridViewModel
                    {
                        OrderID = p.OrderID,
                        OrderNumber = p.OrderNumber,
                        IssueDate = p.IssueDate,
                        JobNotes = p.Note,
                        JobTypeName = p.JobType.Name ?? "",
                        JobSubTypeName = p.JobSubType.Description ?? "",
                        DateUpdated = p.DateUpdated,
                        POStatus = p.OrderStatus.Status ?? "",
                        Account = p.Account.AccountCode ?? "",
                        Property = p.PProperties.AddressLine1 ?? "",
                        OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                        OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
                        OrderType = p.TransactionType.OrderType

                    });

                return transactions;
            }
            else
            {
                var transactions = orderServices.GetAllOrdersIncludingNavProperties(CurrentTenantId)
                    .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.ReAllocationRequired)
                    .Select(p => new WorksOrdersGridViewModel
                    {
                        OrderID = p.OrderID,
                        OrderNumber = p.OrderNumber,
                        IssueDate = p.IssueDate,
                        JobNotes = p.Note,
                        JobTypeName = p.JobType.Name ?? "",
                        JobSubTypeName = p.JobSubType.Description ?? "",
                        DateUpdated = p.DateUpdated,
                        POStatus = p.OrderStatus.Status ?? "",
                        Account = p.Account.AccountCode ?? "",
                        Property = p.PProperties.AddressLine1 ?? "",
                        OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
                        OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
                        OrderType = p.TransactionType.OrderType

                    });

                return transactions;


            }


        }
        public static void ReallocatedJobsGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId)
        {

            var transactions = GetReallocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void ReallocatedJobsGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetReallocatedJobsDataset(tenantId);

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
                transactions = transactions.OrderBy("DateUpdated Desc");
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
        public static GridViewModel CreateReallocationJobsGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("IssueDate");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Note");
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Description");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("AddressLine1");
            viewModel.Columns.Add("InventoryTransactionTypeId");
            viewModel.Columns.Add("OrderNotes");
            viewModel.Columns.Add("OrderType");
            viewModel.Columns.Add("Appointment");
            viewModel.Pager.PageSize = 20;
           return viewModel;
            //var selectedJobType = int.Parse(!string.IsNullOrEmpty(Request.Params["RASelectedJobType"]) ? Request.Params["RASelectedJobType"] : "0");

            //if (selectedJobType > 0)
            //{
            //    var model = OrderService.GetAllOrdersIncludingNavProperties(CurrentTenantId)
            //        .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.ReAllocationRequired && o.JobTypeId == selectedJobType)
            //        .Select(p => new WorksOrdersGridViewModel
            //        {
            //            OrderID = p.OrderID,
            //            OrderNumber = p.OrderNumber,
            //            IssueDate = p.IssueDate,
            //            JobNotes = p.Note,
            //            JobTypeName = p.JobType.Name ?? "",
            //            JobSubTypeName = p.JobSubType.Description ?? "",
            //            DateUpdated = p.DateUpdated,
            //            POStatus = p.OrderStatus.Status ?? "",
            //            Account = p.Account.AccountCode ?? "",
            //            Property = p.PProperties.AddressLine1 ?? "",
            //            OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
            //            OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
            //            OrderType = p.TransactionType.OrderType

            //        });

            //    return PartialView("_ReallocationJobsPartial", model.ToList());
            //}
            //else
            //{
            //    var model = OrderService.GetAllOrdersIncludingNavProperties(CurrentTenantId)
            //        .Where(o => o.TransactionType.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.WorksOrder && o.OrderStatusID == (int)OrderStatusEnum.ReAllocationRequired)
            //        .Select(p => new WorksOrdersGridViewModel
            //        {
            //            OrderID = p.OrderID,
            //            OrderNumber = p.OrderNumber,
            //            IssueDate = p.IssueDate,
            //            JobNotes = p.Note,
            //            JobTypeName = p.JobType.Name ?? "",
            //            JobSubTypeName = p.JobSubType.Description ?? "",
            //            DateUpdated = p.DateUpdated,
            //            POStatus = p.OrderStatus.Status ?? "",
            //            Account = p.Account.AccountCode ?? "",
            //            Property = p.PProperties.AddressLine1 ?? "",
            //            OrderTypeId = p.TransactionType.InventoryTransactionTypeId,
            //            OrderNotesList = p.OrderNotes.Where(x => x.IsDeleted != true).Select(s => new OrderNotesViewModel() { OrderNoteId = s.OrderNoteId, Notes = s.Notes, NotesByName = s.User.UserName, NotesDate = s.DateCreated }),
            //            OrderType = p.TransactionType.OrderType

            //        });


        }
    }
}
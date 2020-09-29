using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class OrdersCustomBinding
    {
        private static IQueryable<PurchaseOrderViewModel> GetPurchaseOrdersInProgressDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            var purchaseServices = DependencyResolver.Current.GetService<IPurchaseOrderService>();
            var transactions = purchaseServices.GetAllPurchaseOrdersInProgress(CurrentTenantId, CurrentWarehouseId);
            return transactions;
        }
        public static void PurchaseOrdersInProgressGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetPurchaseOrdersInProgressDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void PurchaseOrdersInProgressGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetPurchaseOrdersInProgressDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var podata = transactions.ToList();
            podata.ForEach(u => u.Qty = context.OrderDetail.Where(o => o.OrderID == u.OrderID && o.IsDeleted != true).Select(q => (decimal?)q.Qty).Sum() ?? 0);

            podata.ForEach(u =>
               u.ProcessedQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Returns && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Wastage
                && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.WastedReturn)?.SelectMany(h => h.OrderProcessDetail)?.Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            podata.ForEach(u =>
                u.ReturnQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && (d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage
                || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn))?.SelectMany(m => m.OrderProcessDetail).Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            e.Data = podata;
        }
        public static IQueryable<PurchaseOrderViewModel> GetPurchaseOrdersDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            OrderStatusEnum? type = OrderStatusEnum.Complete;
            if (HttpContext.Current.Request.Params.AllKeys.Contains("selectedStatus"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["selectedStatus"].ToString()))
                {
                    type = (OrderStatusEnum)int.Parse(HttpContext.Current.Request.Params["selectedStatus"].ToString());
                    if (type == 0)
                    {
                        type = null;
                    }
                }
            }

            var purchaseServices = DependencyResolver.Current.GetService<IPurchaseOrderService>();
            var transactions = purchaseServices.GetAllPurchaseOrdersCompleted(CurrentTenantId, CurrentWarehouseId, type);
            return transactions;
        }
        public static void PurchaseOrdersGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {


            var transactions = GetPurchaseOrdersDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void PurchaseOrdersGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetPurchaseOrdersDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var podata = transactions.ToList();
            podata.ForEach(u => u.Qty = context.OrderDetail.Where(o => o.OrderID == u.OrderID && o.IsDeleted != true).Select(q => (decimal?)q.Qty).Sum() ?? 0);

            podata.ForEach(u =>
               u.ProcessedQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Returns && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Wastage
                && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.WastedReturn)?.SelectMany(h => h.OrderProcessDetail)?.Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            podata.ForEach(u =>
                u.ReturnQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && (d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage
                || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn))?.SelectMany(m => m.OrderProcessDetail).Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            e.Data = podata;
        }
        public static GridViewModel CreatePurchaseOrdersGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("IssueDate");
            viewModel.Columns.Add("Account");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("POStatus").ForceType(typeof(string));
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("Currecny");
            viewModel.Columns.Add("InvoiceNo");
            viewModel.Columns.Add("AutoAllowProcess");
            viewModel.Columns.Add("InvoiceDetails");
            viewModel.Columns.Add("OrderCost");
            viewModel.Columns.Add("OrderTotal");
            viewModel.Columns.Add("Property");
            viewModel.Columns.Add("OrderNoteId");
            viewModel.Columns.Add("Notes");
            viewModel.Columns.Add("NotesByName");
            viewModel.Columns.Add("NotesDate");
            viewModel.Columns.Add("OrderNotesListText");
            viewModel.Columns.Add("Qty");
            viewModel.Columns.Add("QtyReturned");
            viewModel.Columns.Add("QtyProcessed");
            viewModel.Columns.Add("PickerName");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        private static IQueryable<SalesOrderViewModel> GetSalesOrderCompletedDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            OrderStatusEnum? type = OrderStatusEnum.Complete;
            if (HttpContext.Current.Request.Params.AllKeys.Contains("selectedStatus"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["selectedStatus"].ToString()))
                {
                    type = (OrderStatusEnum)int.Parse(HttpContext.Current.Request.Params["selectedStatus"].ToString());
                    if (type == 0)
                    {
                        type = null;
                    }
                }
            }
            var orderServices = DependencyResolver.Current.GetService<ISalesOrderService>();
            var transactions = orderServices.GetAllCompletedSalesOrdersIq(CurrentTenantId, CurrentWarehouseId, type);

            return transactions;
        }

        public static void SalesOrderCompletedGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderCompletedDataset(tenantId, warehouseId);

            SortFilterAndSetDataCount(e, transactions);

        }

        private static void SortFilterAndSetDataCount(GridViewCustomBindingGetDataRowCountArgs e, IQueryable<SalesOrderViewModel> transactions)
        {
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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();
        }

        public static void SalesOrderCompletedGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderCompletedDataset(tenantId, warehouseId);

            SortFilterAndSetData(e, transactions);
        }

        public static void SalesOrderPickerAssignedGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderPickerAssignedDataset(tenantId, warehouseId);

            SortFilterAndSetDataCount(e, transactions);

        }

        public static void SalesOrderPickerAssignedGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderPickerAssignedDataset(tenantId, warehouseId);

            SortFilterAndSetData(e, transactions);
        }

        private static IQueryable<SalesOrderViewModel> GetSalesOrderPickerAssignedDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            OrderStatusEnum? type = OrderStatusEnum.Active;

            var orderServices = DependencyResolver.Current.GetService<ISalesOrderService>();
            var transactions = orderServices.GetAllPickerAssignedSalesOrders(CurrentTenantId, CurrentWarehouseId, type);

            return transactions;
        }

        private static IQueryable<SalesOrderViewModel> GetSalesOrderPickerUnassignedDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            OrderStatusEnum? type = OrderStatusEnum.Active;
            var orderServices = DependencyResolver.Current.GetService<ISalesOrderService>();
            var transactions = orderServices.GetAllPickerUnassignedSalesOrders(CurrentTenantId, CurrentWarehouseId, type);

            return transactions;
        }

        public static void SalesOrderPickerUnassignedGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderPickerUnassignedDataset(tenantId, warehouseId);

            SortFilterAndSetDataCount(e, transactions);

        }

        public static void SalesOrderPickerUnassignedGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderPickerUnassignedDataset(tenantId, warehouseId);

            SortFilterAndSetData(e, transactions);
        }

        private static void SortFilterAndSetData(GridViewCustomBindingGetDataArgs e, IQueryable<SalesOrderViewModel> transactions)
        {
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
                transactions = transactions.OrderBy("OrderID Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var sodata = transactions.ToList();
            sodata.ForEach(u => u.Qty = context.OrderDetail.Where(o => o.OrderID == u.OrderID).Select(q => (decimal?)q.Qty).Sum() ?? 0);

            sodata.ForEach(u =>
               u.ProcessedQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Returns && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.Wastage
                && d.InventoryTransactionTypeId != InventoryTransactionTypeEnum.WastedReturn)?.SelectMany(h => h.OrderProcessDetail)?.Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            sodata.ForEach(u =>
                u.ReturnQty = context.OrderProcess.Where(d => d.OrderID == u.OrderID && d.IsDeleted != true && (d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Returns || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.Wastage
                || d.InventoryTransactionTypeId == InventoryTransactionTypeEnum.WastedReturn))?.SelectMany(m => m.OrderProcessDetail).Where(d => d.IsDeleted != true).Select(c => c.QtyProcessed).DefaultIfEmpty(0).Sum());

            e.Data = sodata;
        }

        private static IQueryable<SalesOrderViewModel> GetSalesOrderAwaitingDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            var orderServices = DependencyResolver.Current.GetService<ISalesOrderService>();
            var transactions = orderServices.GetAllActiveSalesOrdersIq(CurrentTenantId, CurrentWarehouseId, OrderStatusEnum.AwaitingAuthorisation);

            return transactions;
        }
        public static void SalesOrderAwaitingGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderAwaitingDataset(tenantId, warehouseId);

            SortFilterAndSetDataCount(e, transactions);

        }
        public static void SalesOrderAwaitingGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderAwaitingDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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
        private static IQueryable<SalesOrderViewModel> GetSalesOrderActiveDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            var orderServices = DependencyResolver.Current.GetService<ISalesOrderService>();
            var transactions = orderServices.GetAllActiveSalesOrdersIq(CurrentTenantId, CurrentWarehouseId);
            return transactions;
        }
        public static void SalesOrderActiveGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderActiveDataset(tenantId, warehouseId);

            SortFilterAndSetDataCount(e, transactions);

        }

        public static void SalesOrderActiveGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetSalesOrderActiveDataset(tenantId, warehouseId);

            SortFilterAndSetData(e, transactions);
        }
        private static IQueryable<object> GetDirectSalesOrderDataset(int CurrentTenantId, int CurrentWarehouseId)
        {

            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();
            var transactions = orderServices.GetAllPaidDirectSalesOrdersIq(CurrentTenantId, CurrentWarehouseId);

            return transactions;
        }
        public static void DirectSalesOrderGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetDirectSalesOrderDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void DirectSalesOrderGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetDirectSalesOrderDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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

        //------------------  return orders---------------------------------------------------------------

        private static IQueryable<object> GetReturnOrderDataset(int CurrentTenantId, int CurrentWarehouseId)
        {

            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();
            var transactions = orderServices.GetAllReturnOrders(CurrentTenantId, CurrentWarehouseId);

            return transactions;
        }

        public static void ReturnOrderGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetReturnOrderDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void ReturnOrderGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetReturnOrderDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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



        //---------------------------------------------------------------------------------------------------







        public static GridViewModel CreateSalesOrdersGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("IssueDate");
            viewModel.Columns.Add("Account");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("POStatus");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("Currecny");
            viewModel.Columns.Add("InvoiceNo");
            viewModel.Columns.Add("InvoiceDetails");
            viewModel.Columns.Add("OrderCost");
            viewModel.Columns.Add("OrderTotal");
            viewModel.Columns.Add("Property");
            viewModel.Columns.Add("OrderNoteId");
            viewModel.Columns.Add("Notes");
            viewModel.Columns.Add("SaleNotes");
            viewModel.Columns.Add("NotesByName");
            viewModel.Columns.Add("NotesDate");
            viewModel.Columns.Add("OrderNotesListText");
            viewModel.Columns.Add("OrderTypeId");
            viewModel.Columns.Add("OrderType");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("Qty");
            viewModel.Columns.Add("QtyReturned");
            viewModel.Columns.Add("QtyProcessed");
            viewModel.Columns.Add("DeliveryMethod");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        private static IQueryable<TransferOrderViewModel> GetTransferOutDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            OrderStatusEnum? type = OrderStatusEnum.Active;
            if (HttpContext.Current.Request.Params.AllKeys.Contains("selectedStatusTO"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["selectedStatusTO"].ToString()))
                {
                    type = (OrderStatusEnum)int.Parse(HttpContext.Current.Request.Params["selectedStatusTO"].ToString());
                    if (type == 0)
                    {
                        type = null;
                    }
                }
            }

            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();
            var transactions = orderServices.GetTransferOutOrderViewModelDetailsIq(CurrentWarehouseId, CurrentTenantId, type);

            return transactions;
        }
        public static void TransferOutGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetTransferOutDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void TransferOutGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetTransferOutDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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
        public static IQueryable<WorksOrderViewModel> GetWorksOrdersDataset(int CurrentTenantId, int? propertyId)
        {
            var workOrderService = DependencyResolver.Current.GetService<IWorksOrderService>();
            var transactions = workOrderService.GetAllPendingWorksOrdersIq(CurrentTenantId, null, propertyId);
            return transactions;
        }
        public static void WorksOrdersGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int? propertyId)
        {
            var transactions = GetWorksOrdersDataset(tenantId, propertyId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void WorksOrdersGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int? propertyId)
        {

            var transactions = GetWorksOrdersDataset(tenantId, propertyId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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
        public static IQueryable<WorksOrderViewModel> GetWorksOrdersCompletedDataset(int CurrentTenantId, int? propertyId)
        {
            int? type = 2;
            if (HttpContext.Current.Request.Params.AllKeys.Contains("selectedStatus"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["selectedStatus"].ToString()))
                {
                    type = int.Parse(HttpContext.Current.Request.Params["selectedStatus"].ToString());
                    if (type == 0)
                    {
                        type = null;
                    }
                }
            }
            var workOrderService = DependencyResolver.Current.GetService<IWorksOrderService>();
            var transactions = workOrderService.GetAllCompletedWorksOrdersIq(CurrentTenantId, propertyId, type);
            return transactions;
        }
        public static void WorksOrdersCompletedGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int? propertyId)
        {

            var transactions = GetWorksOrdersCompletedDataset(tenantId, propertyId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void WorksOrdersCompletedGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int? propertyId)
        {

            var transactions = GetWorksOrdersCompletedDataset(tenantId, propertyId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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
        private static IQueryable<TransferOrderViewModel> GetTransferInDataset(int CurrentTenantId, int CurrentWarehouseId)
        {
            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();
            OrderStatusEnum? type = OrderStatusEnum.Active;
            if (HttpContext.Current.Request.Params.AllKeys.Contains("selectedStatus"))
            {
                if (!string.IsNullOrEmpty(HttpContext.Current.Request.Params["selectedStatus"].ToString()))
                {
                    type = (OrderStatusEnum)int.Parse(HttpContext.Current.Request.Params["selectedStatus"].ToString());
                    if (type == 0)
                    {
                        type = null;
                    }
                }
            }

            var transactions = orderServices.GetTransferInOrderViewModelDetails(CurrentWarehouseId, CurrentTenantId, type);
            return transactions;
        }
        public static void TransferInGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetTransferInDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static void TransferInGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetTransferInDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("OrderID Descending");
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
        public static GridViewModel CreateTransferInGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderID";
            viewModel.Columns.Add("Account");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("TransferWarehouse");
            viewModel.Columns.Add("TransType");
            viewModel.Columns.Add("Status");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }


    }
}
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
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
    public class InvoiceCustomBinding
    {

        private static IQueryable<OrderProcessViewModel> InvoiceDataset(string type)
        {

            var OrderService = DependencyResolver.Current.GetService<IOrderService>();
            if (type == "PO")
            {
                return OrderService.GetAllOrderProcesses(null, null, OrderProcessStatusEnum.Complete, InventoryTransactionTypeEnum.PurchaseOrder).
                Select(m => new OrderProcessViewModel()
                {
                    DateCreated = m.DateCreated,
                    DeliveryNO = m.DeliveryNO,
                    OrderProcessID = m.OrderProcessID,
                    PONumber = m.Order.OrderNumber,
                    Supplier = m.Order.Account != null ? m.Order.Account.CompanyName : "",
                    InvoiceNumber = m.InvoiceNo != null ? m.InvoiceNo : "",
                    InvoiceTotal = 0,
                    AccountId = m.Order.AccountID,
                    InvoiceDate = m.InvoiceDate ?? DateTime.UtcNow



                });

            }
            else if (type == "VI")
            {
                return OrderService.GetAllOrderProcesses(null, null, OrderProcessStatusEnum.PostedToAccounts, InventoryTransactionTypeEnum.PurchaseOrder).
               Select(m => new OrderProcessViewModel()
               {
                   DateCreated = m.DateCreated,
                   DeliveryNO = m.DeliveryNO,
                   OrderProcessID = m.OrderProcessID,
                   PONumber = m.Order.OrderNumber,
                   Supplier = m.Order.Account != null ? m.Order.Account.CompanyName : "",
                   InvoiceNumber = m.Order.InvoiceNo != null ? m.Order.InvoiceNo : "",
                   InvoiceTotal = 0,
                   AccountId = m.Order.AccountID,
                   InvoiceDate = m.InvoiceDate ?? DateTime.UtcNow,

               });

            }

            var transactions = OrderService.GetAllOrderProcesses(null, null, (type == "Active" ? OrderProcessStatusEnum.Dispatched : OrderProcessStatusEnum.Invoiced), InventoryTransactionTypeEnum.DirectSales).
                Select(m => new OrderProcessViewModel()
                {
                    DateCreated = m.DateCreated,
                    DeliveryNO = m.DeliveryNO,
                    OrderProcessID = m.OrderProcessID,
                    PONumber = m.Order.OrderNumber,
                    Supplier = m.Order.Account != null ? m.Order.Account.CompanyName : "",
                    InvoiceTotal = 0,
                    AccountId = m.Order.AccountID,
                    InvoiceDate = m.InvoiceDate ?? DateTime.UtcNow

                });
            return transactions;
        }
        public static void InvoiceGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, string type)
        {

            var transactions = InvoiceDataset(type);

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
                transactions = transactions.OrderBy("OrderProcessID Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static GridViewModel CreateInvoiceGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderProcessID";
            viewModel.Columns.Add("Supplier");
            viewModel.Columns.Add("PONumber");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DeliveryNO");
            viewModel.Columns.Add("InvoiceNumber");
            viewModel.Columns.Add("InvoiceTotal");
            viewModel.Columns.Add("Email");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void InvoiceGetData(GridViewCustomBindingGetDataArgs e, string type)
        {

            var transactions = InvoiceDataset(type);

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
                transactions = transactions.OrderBy("OrderProcessID Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var invoices = transactions.ToList();
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            var _currentDbContext = DependencyResolver.Current.GetService<IApplicationContext>();
            invoices.ForEach(m =>
            {
                m.InvoiceTotal = InvoiceService.LoadInvoiceProductValuesByOrderProcessId(m.OrderProcessID).InvoiceTotal;
                m.Email = string.Join(";", _currentDbContext.AccountContacts.Where(u => u.AccountID == m.AccountId && u.IsDeleted != true && u.ConTypeInvoices == true).Select(u => u.ContactEmail).ToList());
            });
            e.Data = invoices;

        }



        private static IQueryable<InvoiceViewModel> InvoiceCompletedDataset()
        {
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            int currentId = caCurrent.CurrentTenant().TenantId;
            var transactions = InvoiceService.GetAllInvoiceMasters(currentId).Select(u => new InvoiceViewModel()
            {
                InvoiceMasterId = u.InvoiceMasterId,
                InvoiceNumber = u.InvoiceNumber,
                OrderNumber = u.OrderProcess.Order.OrderNumber,
                NetAmount = u.NetAmount,
                TaxAmount = u.TaxAmount,
                WarrantyAmount = u.WarrantyAmount,
                CardCharges = u.CardCharges,
                PostageCharges = u.PostageCharges,
                InvoiceTotal = u.InvoiceTotal,
                InvoiceDate = u.InvoiceDate,
                AccountName = u.Account.CompanyName

            });

            return transactions;
        }
        public static void InvoiceCompletedGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            var transactions = InvoiceCompletedDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static GridViewModel CreateInvoiceCompletedGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "InvoiceMasterId";
            viewModel.Columns.Add("InvoiceNumber");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("NetAmount");
            viewModel.Columns.Add("TaxAmount");
            viewModel.Columns.Add("WarrantyAmount");
            viewModel.Columns.Add("CardCharges");
            viewModel.Columns.Add("PostageCharges");
            viewModel.Columns.Add("InvoiceTotal");
            viewModel.Columns.Add("InvoiceDate");
            viewModel.Columns.Add("Actions");
            viewModel.Columns.Add("EmailCount");
            viewModel.Columns.Add("Emails");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void InvoiceCompletedGetData(GridViewCustomBindingGetDataArgs e)
        {

            var transactions = InvoiceCompletedDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var data = transactions.ToList();
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            var results = new List<InvoiceViewModel>();
            data.ForEach(m =>
            {
                results.Add(InvoiceService.GetInvoiceMasterById(m.InvoiceMasterId));
            });


            e.Data = results.ToList();
        }



        ///// invoiceView
        private static IQueryable<InvoiceViewModel> InvoiceViewDataset()
        {
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            int CurrentId = caCurrent.CurrentTenant().TenantId;
            var transactions = InvoiceService.GetAllInvoiceViews(CurrentId).Select(u => new InvoiceViewModel()
            {
                InvoiceMasterId = u.InvoiceMasterId,
                InvoiceNumber = u.InvoiceNumber,
                OrderNumber = u.OrderProcess.Order.OrderNumber,
                NetAmount = u.NetAmount,
                TaxAmount = u.TaxAmount,
                WarrantyAmount = u.WarrantyAmount,
                CardCharges = u.CardCharges,
                PostageCharges = u.PostageCharges,
                InvoiceTotal = u.InvoiceTotal,
                InvoiceDate = u.InvoiceDate,
                AccountName = u.Account.CompanyName

            });

            return transactions;
        }
        public static void InvoiceViewGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            var transactions = InvoiceViewDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static GridViewModel CreateInvoiceViewGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "InvoiceMasterId";
            viewModel.Columns.Add("InvoiceNumber");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("NetAmount");
            viewModel.Columns.Add("TaxAmount");
            viewModel.Columns.Add("WarrantyAmount");
            viewModel.Columns.Add("CardCharges");



            viewModel.Columns.Add("PostageCharges");
            viewModel.Columns.Add("InvoiceTotal");
            viewModel.Columns.Add("InvoiceDate");
            viewModel.Columns.Add("Actions");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void InvoiceViewGetData(GridViewCustomBindingGetDataArgs e)
        {

            var transactions = InvoiceViewDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var data = transactions.ToList();
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            var results = new List<InvoiceViewModel>();
            data.ForEach(m =>
            {
                results.Add(InvoiceService.GetInvoiceMasterById(m.InvoiceMasterId));
            });


            e.Data = results.ToList();
        }



        ///// PurchaseinvoiceView
        private static IQueryable<InvoiceViewModel> PurchaseInvoiceViewDataset()
        {
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            int CurrentTenantId = caCurrent.CurrentTenant().TenantId;
            var transactions = InvoiceService.GetAllInvoiceViews(CurrentTenantId).Select(u => new InvoiceViewModel()
            {
                InvoiceMasterId = u.InvoiceMasterId,
                InvoiceNumber = u.InvoiceNumber,
                OrderNumber = u.OrderProcess.Order.OrderNumber,
                NetAmount = u.NetAmount,
                TaxAmount = u.TaxAmount,
                WarrantyAmount = u.WarrantyAmount,
                CardCharges = u.CardCharges,
                PostageCharges = u.PostageCharges,
                InvoiceTotal = u.InvoiceTotal,
                InvoiceDate = u.InvoiceDate,
                AccountName = u.Account.CompanyName

            });

            return transactions;
        }
        public static void PurchaseInvoiceViewGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            var transactions = InvoiceViewDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }
        public static GridViewModel CreatePurchaseInvoiceViewGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "InvoiceMasterId";
            viewModel.Columns.Add("InvoiceNumber");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("NetAmount");
            viewModel.Columns.Add("TaxAmount");
            viewModel.Columns.Add("WarrantyAmount");
            viewModel.Columns.Add("CardCharges");



            viewModel.Columns.Add("PostageCharges");
            viewModel.Columns.Add("InvoiceTotal");
            viewModel.Columns.Add("InvoiceDate");
            viewModel.Columns.Add("Actions");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void PurchaseInvoiceViewGetData(GridViewCustomBindingGetDataArgs e)
        {

            var transactions = InvoiceViewDataset();

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
                transactions = transactions.OrderBy("InvoiceMasterId Descending");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var data = transactions.ToList();
            var InvoiceService = DependencyResolver.Current.GetService<IInvoiceService>();
            var results = new List<InvoiceViewModel>();
            data.ForEach(m =>
            {
                results.Add(InvoiceService.GetInvoiceMasterById(m.InvoiceMasterId));
            });


            e.Data = results.ToList();
        }



    }
}
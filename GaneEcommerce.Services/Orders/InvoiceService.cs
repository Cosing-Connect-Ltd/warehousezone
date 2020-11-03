using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;
        private readonly IProductPriceService _productPriceService;

        public InvoiceService(IApplicationContext currentDbContext, IMapper mapper, IProductPriceService productPriceService)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
            _productPriceService = productPriceService;
        }

        public IQueryable<InvoiceMaster> GetAllInvoiceMasters(int TenantId)
        {
            return _currentDbContext.InvoiceMasters.Where(u => u.InvoiceStatus != InvoiceStatusEnum.PostedToAccounts && u.IsDeleted != true && u.TenantId == TenantId);
        }

        public IQueryable<InvoiceMaster> GetAllInvoiceMastersWithAllStatus(int tenantId, int[] accountIds)
        {
            accountIds = accountIds ?? new int[] { };
            return _currentDbContext.InvoiceMasters.Where(u => u.IsDeleted != true && u.TenantId == tenantId && (accountIds.Count() == 0 || accountIds.Contains(u.AccountId)));
        }

        public IQueryable<InvoiceMaster> GetAllInvoiceViews(int TenantId)
        {
            return _currentDbContext.InvoiceMasters.Where(u => u.InvoiceStatus == InvoiceStatusEnum.PostedToAccounts && u.IsDeleted != true && u.TenantId == TenantId);
        }

        public InvoiceViewModel GetInvoiceMasterByOrderProcessId(int orderProcessId)
        {
            var m = _currentDbContext.InvoiceMasters.FirstOrDefault(s => s.OrderProcessId == orderProcessId && s.IsDeleted != true);
            if (m == null) return null;
            var item = _mapper.Map<InvoiceMaster, InvoiceViewModel>(m);
            item.AccountName = m.Account.CompanyName;

            return item;
        }

        public InvoiceViewModel GetInvoiceMasterById(int invoiceId)
        {
            var m = _currentDbContext.InvoiceMasters.FirstOrDefault(s => s.InvoiceMasterId == invoiceId && s.IsDeleted != true);

            if (m == null) return null;
            var item = _mapper.Map<InvoiceMaster, InvoiceViewModel>(m);
            item.OrderNumber = m.OrderProcess?.Order?.OrderNumber;
            item.AccountName = m.Account.CompanyName;
            item.EmailCount = _currentDbContext.TenantEmailNotificationQueues.Count(u => u.InvoiceMasterId == invoiceId);
            item.Emails = string.Join(";", _currentDbContext.AccountContacts.Where(u => u.AccountID == m.AccountId && u.IsDeleted != true && u.ConTypeInvoices == true).Select(u => u.ContactEmail).ToList());
            return item;
        }

        public List<InvoiceDetailViewModel> GetAllInvoiceDetailByInvoiceId(int invoiceId)
        {
            var invoiceDetails = _currentDbContext.InvoiceDetails.Where(x => x.InvoiceMasterId == invoiceId && x.IsDeleted != true).ToList();
            var results = new List<InvoiceDetailViewModel>();
            invoiceDetails.ForEach(m =>
            {
                var item = _mapper.Map<InvoiceDetail, InvoiceDetailViewModel>(m);
                item.Description = m.Product.NameWithCode;
                item.SkuCode = m.Product.SKUCode;
                item.Percentage = _currentDbContext.GlobalTax.FirstOrDefault(u => u.TaxID == item.TaxId)?.PercentageOfAmount;
                results.Add(item);
            });
            return results;
        }

        public List<InvoiceDetailReportViewModel> GetAllInvoiceDetailReportData(int invoiceId)
        {
            var invoiceDetails = _currentDbContext.InvoiceDetails.Where(x => x.InvoiceMasterId == invoiceId && x.IsDeleted != true).ToList();
            var results = new List<InvoiceDetailReportViewModel>();

            var OrderIds = invoiceDetails.Select(i => i.OrderDetail.OrderID).Distinct();

            var directSalesPurchaseOrders = _currentDbContext.Order.Where(u => u.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder &&
                                                                                u.OrderStatusID == OrderStatusEnum.Complete &&
                                                                                u.IsDeleted != true &&
                                                                                OrderIds.Contains(u.OrderID));

            var relatedInventoryTransactions = _currentDbContext.InventoryTransactions.Where(p => p.OrderID != null && OrderIds.Contains(p.OrderID ?? 0));

            var palletPurchaseOrders = _currentDbContext.InventoryTransactions.Where(p => p.PalletTrackingId != null &&
                                                                                          relatedInventoryTransactions.Select(i => i.PalletTrackingId).Distinct().Contains(p.PalletTrackingId) &&
                                                                                          p.Order.InventoryTransactionTypeId == InventoryTransactionTypeEnum.PurchaseOrder);

            invoiceDetails.ForEach(m =>
            {
                var item = new InvoiceDetailReportViewModel();
                item.SkuCode = m.Product.SKUCode;
                item.ProductName = m.Product.Name;
                item.Quantity = m.Quantity;
                item.SaleOrderNumber = m.OrderDetail.Order.OrderNumber;
                item.DeliveryNote = _currentDbContext.OrderProcessDetail.FirstOrDefault(o => o.OrderDetailID == m.OrderDetailId && o.ProductId == m.ProductId)?.OrderProcess.DeliveryNO;

                var palletTracking = relatedInventoryTransactions.FirstOrDefault(i => i.OrderID == m.OrderDetail.OrderID && i.ProductId == m.ProductId)?.PalletTracking;
                item.PalletNumber = palletTracking?.PalletSerial;

                var purchaseOrderService = directSalesPurchaseOrders.FirstOrDefault(p => p.BaseOrderID == m.OrderDetail.OrderID);

                if (purchaseOrderService == null && palletTracking != null)
                {
                    purchaseOrderService = palletPurchaseOrders.FirstOrDefault(p => p.PalletTrackingId == palletTracking.PalletTrackingId &&
                                                                                    p.ProductId == m.ProductId)?.Order;
                }

                if (purchaseOrderService != null)
                {
                    item.PurchaseOrderNumber = purchaseOrderService?.OrderNumber;
                    item.SupplierName = purchaseOrderService?.Account.CompanyName;
                    item.SupplierInvoiceNumber = purchaseOrderService?.ExternalOrderNumber;
                }

                results.Add(item);
            });
            return results.OrderBy(p => p.ProductName).ToList();
        }

        public InvoiceMaster CreateInvoiceForSalesOrder(InvoiceViewModel invoiceData, int tenantId, int userId, bool useOrderProcessDate = false)
        {
            var account = _currentDbContext.Account.AsNoTracking().Where(x => x.AccountID == invoiceData.AccountId).FirstOrDefault();
            if (account == null)
            {
                account = _currentDbContext.Account.AsNoTracking().Where(x => x.AccountCode == "DCA").FirstOrDefault();
            }
            var tenant = _currentDbContext.Tenants.AsNoTracking().Where(x => x.TenantId == tenantId).FirstOrDefault();
            var process = _currentDbContext.OrderProcess.Find(invoiceData.OrderProcessId);
            InventoryTransactionTypeEnum inventoryTransactionType = InventoryTransactionTypeEnum.SalesOrder;

            var invoice = new InvoiceMaster
            {
                OrderProcessId = invoiceData.OrderProcessId < 1 ? (int?)null : invoiceData.OrderProcessId,
                TenantId = tenantId,
                DateCreated = (process != null && useOrderProcessDate) ? process.DateCreated : DateTime.UtcNow,
                CreatedBy = userId,
                InvoiceDate = (process != null && useOrderProcessDate) ? process.DateCreated : invoiceData.InvoiceDate,
                AccountId = account.AccountID,
                CardCharges = invoiceData.CardCharges,
                InvoiceAddress = account?.FullAddress,
                InvoiceCurrency = account?.GlobalCurrency?.CurrencyName ?? tenant.Currency.CurrencyName,
                DateUpdated = DateTime.UtcNow,
                InvoiceTotal = invoiceData.InvoiceTotal,
                NetAmount = invoiceData.NetAmount,
                PostageCharges = invoiceData.PostageCharges,
                TaxAmount = invoiceData.TaxAmount,
                WarrantyAmount = invoiceData.WarrantyAmount,
                OrderDiscount = invoiceData.OrderDiscount,
                UpdatedBy = userId,
                CurrencyId = account?.CurrencyID ?? tenant.CurrencyID,
            };

            invoice.InvoiceNumber = invoice.InvoiceNumber = process?.Order?.InvoiceNo != null ? process.Order.InvoiceNo : GenerateNextInvoiceNumber(tenantId);

            invoice.InvoiceDetails = invoiceData.AllInvoiceProducts.Select(m => new InvoiceDetail()
            {
                ProductId = m.ProductId,
                Quantity = m.QtyProcessed,
                Price = m.Price,
                Total = m.TotalAmount + (m.TaxAmountsInvoice ?? 0) + m.WarrantyAmount,
                Tax = (m.TaxAmountsInvoice ?? 0),
                NetAmount = m.TotalAmount,
                WarrantyAmount = m.WarrantyAmount,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                DateUpdated = DateTime.UtcNow,
                UpdatedBy = userId,
                TenantId = tenantId,
                TaxId = m.TaxId,
                OrderDetailId = m.OrderDetailId,
                WarrantyId = m.WarrantyId
            }).ToList();

            _currentDbContext.Entry(invoice).State = EntityState.Added;


            if (process != null)
            {
                inventoryTransactionType = process.Order.InventoryTransactionTypeId;
                process.InvoiceNo = invoice.InvoiceNumber;
                process.OrderProcessStatusId = OrderProcessStatusEnum.Invoiced;
                process.DateUpdated = DateTime.UtcNow;
                process.UpdatedBy = userId;

                process.Order.InvoiceNo = invoice.InvoiceNumber + string.Join(",", _currentDbContext.OrderProcess.Where(u => u.OrderID == process.OrderID).Select(u => u.InvoiceNo).ToList());
                _currentDbContext.Entry(process).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();

            if (process != null)
            {
                var orderPorcessCount = _currentDbContext.OrderProcess.Count(u => u.OrderID == process.OrderID && (u.OrderProcessStatusId != OrderProcessStatusEnum.Invoiced && u.OrderProcessStatusId != OrderProcessStatusEnum.PostedToAccounts));
                if (orderPorcessCount <= 0)
                {
                    process.Order.OrderStatusID = OrderStatusEnum.Invoiced;
                    _currentDbContext.SaveChanges();
                }
            }

            if (inventoryTransactionType != InventoryTransactionTypeEnum.DirectSales)
            {
                var accountTransaction = new AccountTransaction()
                {
                    AccountId = account?.AccountID,
                    AccountTransactionTypeId = AccountTransactionTypeEnum.InvoicedToAccount,
                    CreatedBy = userId,
                    Notes = ("Invoiced : " + (process != null ? process.Order.OrderNumber : "")).Trim(),
                    DateCreated = DateTime.UtcNow,
                    Amount = invoice.InvoiceTotal,
                    TenantId = tenantId
                };

                SaveAccountTransaction(accountTransaction, tenantId, userId);
            }

            return invoice;
        }

        public InvoiceMaster SaveInvoiceForSalesOrder(InvoiceViewModel invoiceData, int tenantId, int userId)
        {
            var account = _currentDbContext.Account.Find(invoiceData.AccountId);
            var invoiceMaster = _currentDbContext.InvoiceMasters.FirstOrDefault(u => u.InvoiceMasterId == invoiceData.InvoiceMasterId);
            decimal amount = invoiceMaster.InvoiceTotal;
            AccountTransactionTypeEnum accountTransactionTypeId = AccountTransactionTypeEnum.InvoicedToAccount;
            if (invoiceMaster != null)
            {
                invoiceMaster.OrderProcessId = invoiceData.OrderProcessId < 1 ? (int?)null : invoiceData.OrderProcessId;

                invoiceMaster.InvoiceDate = invoiceData.InvoiceDate;
                invoiceMaster.AccountId = invoiceData.AccountId;
                invoiceMaster.CardCharges = invoiceData.CardCharges;
                invoiceMaster.InvoiceAddress = account.FullAddress;
                invoiceMaster.InvoiceCurrency = account.GlobalCurrency.CurrencyName;
                invoiceMaster.DateUpdated = DateTime.UtcNow;
                invoiceMaster.InvoiceTotal = invoiceData.InvoiceTotal;
                invoiceMaster.NetAmount = (invoiceData.NetAmount - invoiceData.TaxAmount) - (invoiceData.WarrantyAmount);
                invoiceMaster.PostageCharges = invoiceData.PostageCharges;
                invoiceMaster.TaxAmount = invoiceData.TaxAmount;
                invoiceMaster.WarrantyAmount = invoiceData.WarrantyAmount;
                invoiceMaster.OrderDiscount = invoiceData.OrderDiscount;
                invoiceMaster.UpdatedBy = userId;
                invoiceMaster.CurrencyId = account.CurrencyID;
                invoiceMaster.InvoiceNumber = invoiceMaster.InvoiceNumber;
                var invoiceProductID = invoiceData.AllInvoiceProducts.Select(u => u.ProductId).ToList();
                var invoiceDelete = _currentDbContext.InvoiceDetails.Where(u => u.InvoiceMasterId == invoiceMaster.InvoiceMasterId && !invoiceProductID.Contains(u.ProductId) && u.IsDeleted != true).ToList();
                invoiceDelete.ForEach(u => { u.IsDeleted = true; u.DateUpdated = DateTime.UtcNow; });
                foreach (var item in invoiceData.AllInvoiceProducts)
                {
                    var invoiceDetail = _currentDbContext.InvoiceDetails.FirstOrDefault(u => u.ProductId == item.ProductId && u.InvoiceMasterId == invoiceMaster.InvoiceMasterId && u.IsDeleted != true);
                    if (invoiceDetail == null)
                    {
                        invoiceDetail = new InvoiceDetail()
                        {
                            ProductId = item.ProductId,
                            Quantity = item.QtyProcessed,
                            Price = item.Price,
                            Total = item.TotalAmount + (item.TaxAmountsInvoice ?? 0) + item.WarrantyAmount,
                            Tax = item.TaxAmountsInvoice ?? 0,
                            NetAmount = item.TotalAmount,
                            WarrantyAmount = item.WarrantyAmount,
                            WarrantyId = item.WarrantyId,
                            CreatedBy = userId,
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            UpdatedBy = userId,
                            InvoiceMasterId = invoiceMaster.InvoiceMasterId,
                            TenantId = tenantId,
                            TaxId = item.TaxId,
                            OrderDetailId = item.OrderDetailId
                        };
                        _currentDbContext.InvoiceDetails.Add(invoiceDetail);
                        _currentDbContext.Entry(invoiceDetail).State = EntityState.Added;
                    }
                    else
                    {
                        invoiceDetail.ProductId = item.ProductId;
                        invoiceDetail.Quantity = item.QtyProcessed;
                        invoiceDetail.Price = item.Price;
                        invoiceDetail.Total = item.TotalAmount + item.TaxAmountsInvoice ?? 0 + item.WarrantyAmount;
                        invoiceDetail.Tax = item.TaxAmountsInvoice ?? 0;
                        invoiceDetail.NetAmount = item.TotalAmount;
                        invoiceDetail.WarrantyAmount = item.WarrantyAmount;
                        invoiceDetail.WarrantyId = item.WarrantyId;
                        invoiceDetail.CreatedBy = userId;
                        invoiceDetail.DateCreated = DateTime.UtcNow;
                        invoiceDetail.DateUpdated = DateTime.UtcNow;
                        invoiceDetail.UpdatedBy = userId;
                        invoiceDetail.TenantId = tenantId;
                        invoiceDetail.InvoiceMasterId = invoiceMaster.InvoiceMasterId;
                        _currentDbContext.Entry(invoiceDetail).State = EntityState.Modified;
                    }
                }
                var process = _currentDbContext.OrderProcess.Find(invoiceData.OrderProcessId);
                if (process != null)
                {
                    process.DateUpdated = DateTime.UtcNow;
                    process.UpdatedBy = userId;
                    _currentDbContext.Entry(process).State = EntityState.Modified;
                }
                if (amount > invoiceData.InvoiceTotal)
                {
                    amount = (amount - invoiceData.InvoiceTotal);
                    accountTransactionTypeId = AccountTransactionTypeEnum.Refund;
                }
                else
                {
                    amount = (invoiceData.InvoiceTotal - amount);
                }
                if (amount > 0)
                {
                    var accountTransaction = new AccountTransaction()
                    {
                        AccountId = account.AccountID,
                        AccountTransactionTypeId = accountTransactionTypeId,
                        CreatedBy = userId,
                        Notes = ("Invoiced : " + (process != null ? process.Order.OrderNumber : "")).Trim(),
                        DateCreated = DateTime.UtcNow,
                        Amount = amount,
                        TenantId = tenantId
                    };
                    _currentDbContext.Entry(invoiceMaster).State = EntityState.Modified;

                    if (process != null)
                    {
                        var orderPorcessCount = _currentDbContext.OrderProcess.Count(u => u.OrderID == process.OrderID && u.OrderProcessStatusId != OrderProcessStatusEnum.Invoiced);
                        if (orderPorcessCount <= 0)
                        {
                            process.Order.OrderStatusID = OrderStatusEnum.Invoiced;
                            _currentDbContext.SaveChanges();
                        }
                    }

                    SaveAccountTransaction(accountTransaction, tenantId, userId);
                }
                _currentDbContext.SaveChanges();
                return invoiceMaster;
            }

            return null;
        }

        public AccountTransaction AddAccountTransaction(AccountTransactionTypeEnum type, decimal amount, string notes, int accountId, int tenantId, int userId, AccountPaymentModeEnum? accountPaymentModeId = null)
        {
            //TODO: add account opening nad closing balance in here if required
            return SaveAccountTransaction(new AccountTransaction
            {
                Notes = notes,
                AccountPaymentModeId = accountPaymentModeId,
                Amount = amount,
                AccountTransactionTypeId = type,
                AccountId = accountId
            }, tenantId, userId);
        }

        public AccountTransaction SaveAccountTransaction(AccountTransaction accountTransaction, int tenantId, int userId)
        {
            var trans = _currentDbContext.AccountTransactions.Find(accountTransaction.AccountTransactionId);

            if (trans == null)
            {
                var accountBalance = Financials.CalcAccountBalance(accountTransaction.AccountId ?? 0);

                if (accountTransaction.AccountTransactionTypeId == AccountTransactionTypeEnum.PaidByAccount || accountTransaction.AccountTransactionTypeId == AccountTransactionTypeEnum.Refund
                    || accountTransaction.AccountTransactionTypeId == AccountTransactionTypeEnum.CreditNote || accountTransaction.AccountTransactionTypeId == AccountTransactionTypeEnum.Discount)
                {
                    accountBalance = accountBalance - accountTransaction.Amount;
                }
                else
                {
                    accountBalance = accountBalance + accountTransaction.Amount;
                }

                trans = new AccountTransaction();
                _currentDbContext.Entry(trans).State = EntityState.Added;
                trans.Amount = accountTransaction.Amount;
                trans.AccountTransactionTypeId = accountTransaction.AccountTransactionTypeId;
                trans.FinalBalance = accountBalance;
                trans.AccountId = accountTransaction.AccountId;
                trans.DateCreated = DateTime.UtcNow;
                trans.CreatedBy = userId;
                trans.TenantId = tenantId;

                var account = _currentDbContext.Account.Find(trans.AccountId);
                account.FinalBalance = accountBalance;
                account.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(account).State = EntityState.Modified;
            }
            else
            {
                trans.DateUpdated = DateTime.UtcNow;
                trans.UpdatedBy = userId;
            }

            trans.Notes = accountTransaction.Notes;

            _currentDbContext.SaveChanges();
            accountTransaction = trans;
            return accountTransaction;
        }

        public List<AccountTransactionFile> GetaccountTransactionFiles(int accountTransactionId, int tenantId)
        {
            return _currentDbContext.AccountTransactionFiles.Where(u => u.AccountTransactionID == accountTransactionId && u.TenantId == tenantId).ToList();
        }

        public string GenerateNextInvoiceNumber(int tenantId)
        {
            var lastOrder = _currentDbContext.InvoiceMasters.Where(p => p.TenantId == tenantId)
               .OrderByDescending(m => m.InvoiceNumber)
               .FirstOrDefault();
            var prefix = "IN-";
            if (lastOrder != null)
            {
                var lastNumber = lastOrder.InvoiceNumber.Replace("IN-", string.Empty);
                int n;
                bool isNumeric = int.TryParse(lastNumber, out n);

                if (isNumeric == true)
                {
                    var lastInvoiceNumber = (int.Parse(lastNumber) + 1).ToString("00000000");
                    return prefix + lastInvoiceNumber;
                }
                else
                {
                    return prefix + "00000001";
                }
            }
            else
            {
                return prefix + "00000001";
            }
        }

        public InvoiceViewModel LoadInvoiceProductValuesByOrderProcessId(int orderProcessId, int? inventoryTransctionType = null)
        {
            InvoiceViewModel model = new InvoiceViewModel();
            var orderProcess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == orderProcessId && u.IsDeleted != true);
            if (orderProcess != null)
            {
                model.OrderNumber = orderProcess.Order.OrderNumber;
                model.OrderProcessId = orderProcess.OrderProcessID;
                model.InvoiceCurrency = orderProcess.Order?.Account?.GlobalCurrency?.CurrencyName;
                model.InvoiceAddress = orderProcess.Order?.Account?.FullAddressWithNameHtml;
                model.AccountId = orderProcess.Order?.AccountID ?? 0;
                model.InvoiceDate = orderProcess.InvoiceDate ?? DateTime.UtcNow;
                model.OrderDiscount = orderProcess.Order.OrderDiscount;
            }

            var orderProcesses = _currentDbContext.OrderProcessDetail
                .Where(o => o.OrderProcessId == orderProcessId && o.IsDeleted != true)
                .Include(o => o.ProductMaster)
                .ToList();

            foreach (var x in orderProcesses)
            {
                var pd = model.AllInvoiceProducts.FirstOrDefault(m => m.ProductId == x.ProductId);
                if (pd == null)
                {
                    pd = new OrderProcessDetailsViewModel()
                    {
                        QtyProcessed = x.QtyProcessed,
                        ProductName = x.ProductMaster.Name,
                        ProductCode = x.ProductMaster.SKUCode,
                        ProductId = x.ProductId,
                        OrderProcessId = x.OrderProcessId,
                        //Remove methods from here
                        Price = x?.OrderDetail?.Price ?? 0,
                        OrderProcessDetailId = x.OrderProcessDetailID,
                        OrderDetailId = x?.OrderDetailID,
                        TaxId = x?.OrderDetail?.TaxID,
                        WarrantyId = x?.OrderDetail?.WarrantyID,
                        TaxAmountsInvoice = Math.Round((((x?.OrderDetail?.Price ?? 0) * x.QtyProcessed) / 100) * (x?.OrderDetail?.TaxName?.PercentageOfAmount ?? 0), 2)
                    };
                    if (x?.OrderDetail?.Warranty != null)
                    {
                        pd.WarrantyAmount = x.OrderDetail.Warranty.IsPercent ? Math.Round(x.OrderDetail.Warranty.PercentageOfPrice * ((pd.Price * x.QtyProcessed) / 100), 2) : Math.Round(x.OrderDetail.Warranty.FixedPrice * x.QtyProcessed, 2);
                    }
                    if (x?.OrderDetail?.TaxName != null)
                    {
                        pd.TaxPercent = x.OrderDetail.TaxName.PercentageOfAmount;
                    }

                    model.AllInvoiceProducts.Add(pd);
                }
                else
                {
                    var index = model.AllInvoiceProducts.IndexOf(pd);
                    model.AllInvoiceProducts[index].QtyProcessed += x.QtyProcessed;
                    model.AllInvoiceProducts[index].TaxAmountsInvoice += Math.Round((((x?.OrderDetail?.Price ?? 0) * x.QtyProcessed) / 100) * (x.OrderDetail?.TaxName?.PercentageOfAmount ?? 0), 2);
                }
            }

            model.TaxAmount = model.AllInvoiceProducts.Select(I => I.TaxAmount).DefaultIfEmpty(0).Sum();
            var amount = model.AllInvoiceProducts.Select(u => u.NetAmount).DefaultIfEmpty(0).Sum();
            model.WarrantyAmount += model.AllInvoiceProducts.Select(u => u.WarrantyAmount).DefaultIfEmpty(0).Sum();
            var taxAmount = model.TaxAmount;
            var warrantyAmount = model.WarrantyAmount;
            model.NetAmount = ((amount - taxAmount) - warrantyAmount);
            model.InvoiceTotal = Math.Round(model.NetAmount + model.TaxAmount + model.WarrantyAmount - model.OrderDiscount, 2);

            return model;
        }

        public List<InvoiceProductPriceModel> GetInvoiceProductsPrices(int InvoiceMasterId, int[] productIds, int tenantId)
        {
            productIds = productIds ?? new int[] { };

            var InvocieDetaildata = _currentDbContext.InvoiceDetails.Include(i => i.OrderDetail)
                                                                    .Where(u => u.InvoiceMasterId == InvoiceMasterId &&
                                                                                u.IsDeleted != true &&
                                                                                (productIds.Contains(u.ProductId) || productIds.Count() == 0))
                                                                    .ToList();

            return _productPriceService.GetInvoiceDetailsProductPrices(InvocieDetaildata, tenantId);
        }

        public InvoiceViewModel GetInvoicePreviewModelByOrderProcessId(int orderProcessId, caTenant tenant)
        {
            var model = LoadInvoiceProductValuesByOrderProcessId(orderProcessId);

            var invoice = GetInvoiceMasterByOrderProcessId(orderProcessId);
            if (invoice != null)
            {
                model.InvoiceNumber = invoice.InvoiceNumber;
                model.InvoiceDate = invoice.InvoiceDate;
            }

            model.TenantName = tenant.TenantName;
            return model;
        }
    }
}
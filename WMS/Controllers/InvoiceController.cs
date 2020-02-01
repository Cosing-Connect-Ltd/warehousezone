using AutoMapper;
using ClosedXML.Excel;
using DevExpress.Web.ASPxHtmlEditor;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WMS.CustomBindings;
using WMS.Helpers;

namespace WMS.Controllers
{
    public class InvoiceController : BaseReportsController
    {
        private readonly IProductPriceService _priceService;
        private readonly IGaneConfigurationsHelper _helper;
        private readonly IProductServices _productServices;
        private readonly IInvoiceService _invoiceService;
        private readonly IAccountServices _accountService;
        private readonly IMapper _mapper;

        public InvoiceController(IProductServices productServices, ISalesOrderService salesOrderService, ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices,
             IAppointmentsService appointmentsService, IGaneConfigurationsHelper configurationsHelper, IEmailServices emailServices, ITenantLocationServices tenantLocationservices, ITenantsServices tenantsServices,
             IProductPriceService priceService, IGaneConfigurationsHelper helper, IInvoiceService invoiceService, IMapper mapper)
             : base(orderService, propertyService, accountServices, lookupServices, appointmentsService, configurationsHelper, emailServices, tenantLocationservices, tenantsServices)
        {

            _priceService = priceService;
            _helper = helper;
            _productServices = productServices;
            _invoiceService = invoiceService;
            _mapper = mapper;
            _accountService = accountServices;
        }



        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult ProcessedOrdersPartial(string type)
        {

            if (type == "VI")
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("PurchaseViewInvoice");

                if (viewModel == null)
                    viewModel = InvoiceCustomBinding.CreateInvoiceGridViewModel();

                return InvoiceGridActionCore(viewModel, type);
            }
            else
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("gridMasterAwaitingInvoicing");

                if (viewModel == null)
                    viewModel = InvoiceCustomBinding.CreateInvoiceGridViewModel();

                return InvoiceGridActionCore(viewModel, type);
            }
        }

        public ActionResult InvoiceGridActionCore(GridViewModel gridViewModel, string type)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    InvoiceCustomBinding.InvoiceGetDataRowCount(args, type);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        InvoiceCustomBinding.InvoiceGetData(args, type);
                    })
            );
            return PartialView("_OrderProcessesGridPartial", gridViewModel);
        }

        public ActionResult _InvoiceGridViewsPaging(GridViewPagerState pager, string type)
        {
            if (type == "VI")
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("PurchaseViewInvoice");
                viewModel.Pager.Assign(pager);
                return InvoiceGridActionCore(viewModel, type);
            }
            else
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("gridMasterAwaitingInvoicing");
                viewModel.Pager.Assign(pager);
                return InvoiceGridActionCore(viewModel, type);
            }
        }

        public ActionResult _InvoiceGridViewFiltering(GridViewFilteringState filteringState, string type)
        {
            if (type == "VI")
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("PurchaseViewInvoice");
                viewModel.ApplyFilteringState(filteringState);
                return InvoiceGridActionCore(viewModel, type);
            }
            else
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("gridMasterAwaitingInvoicing");
                viewModel.ApplyFilteringState(filteringState);
                return InvoiceGridActionCore(viewModel, type);
            }
        }

        public ActionResult _InvoiceGridViewDataSorting(GridViewColumnState column, string type, bool reset)
        {
            if (type == "VI")
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("PurchaseViewInvoice");
                viewModel.ApplySortingState(column, reset);
                return InvoiceGridActionCore(viewModel, type);
            }
            else
            {
                ViewBag.GridName = type;
                var viewModel = GridViewExtension.GetViewModel("gridMasterAwaitingInvoicing");
                viewModel.ApplySortingState(column, reset);
                return InvoiceGridActionCore(viewModel, type);
            }
        }

        public ActionResult InvoicesListPartial(string type)
        {
            ViewBag.GridName = type;
            var viewModel = GridViewExtension.GetViewModel("gridMasterInvoices");

            if (viewModel == null)
                viewModel = InvoiceCustomBinding.CreateInvoiceCompletedGridViewModel();

            return InvoiceCompleteGridActionCore(viewModel);
        }

        public ActionResult InvoiceCompleteGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    InvoiceCustomBinding.InvoiceCompletedGetDataRowCount(args);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        InvoiceCustomBinding.InvoiceCompletedGetData(args);
                    })
            );
            return PartialView("_InvoiceGridviewPartial", gridViewModel);
        }

        public ActionResult _InvoiceCompleteViewsPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("gridMasterInvoices");
            viewModel.Pager.Assign(pager);
            return InvoiceCompleteGridActionCore(viewModel);
        }

        public ActionResult _InvoiceCompleteGridViewFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("gridMasterInvoices");
            viewModel.ApplyFilteringState(filteringState);
            return InvoiceCompleteGridActionCore(viewModel);
        }

        public ActionResult _InvoiceCompleteGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("gridMasterInvoices");
            viewModel.ApplySortingState(column, reset);
            return InvoiceCompleteGridActionCore(viewModel);
        }

        #region invoiceview
        public ActionResult InvoicesViewListPartial()
        {
            var viewModel = GridViewExtension.GetViewModel("InvoiceViewGridview");

            if (viewModel == null)
                viewModel = InvoiceCustomBinding.CreateInvoiceViewGridViewModel();

            return InvoiceViewGridActionCore(viewModel);
        }
        public ActionResult InvoiceViewGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    InvoiceCustomBinding.InvoiceViewGetDataRowCount(args);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        InvoiceCustomBinding.InvoiceViewGetData(args);
                    })
            );
            return PartialView("_InvoiceViewGridviewPartial", gridViewModel);
        }

        public ActionResult _InvoiceViewCompleteViewsPaging(GridViewPagerState pager)
        {

            var viewModel = GridViewExtension.GetViewModel("InvoiceViewGridview");
            viewModel.Pager.Assign(pager);
            return InvoiceViewGridActionCore(viewModel);
        }

        public ActionResult _InvoiceViewCompleteGridViewFiltering(GridViewFilteringState filteringState)
        {

            var viewModel = GridViewExtension.GetViewModel("InvoiceViewGridview");
            viewModel.ApplyFilteringState(filteringState);
            return InvoiceViewGridActionCore(viewModel);
        }

        public ActionResult _InvoiceViewCompleteGridViewDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("InvoiceViewGridview");
            viewModel.ApplySortingState(column, reset);
            return InvoiceViewGridActionCore(viewModel);
        }
        #endregion

        public ActionResult InvoiceDetailsPartial(int id)
        {
            ViewBag.InvoiceID = id;

            var invoiceDetails = _invoiceService.GetAllInvoiceDetailByInvoiceId(id);

            return PartialView("_InvoiceDetailsPartial", invoiceDetails);
        }

        public ActionResult OrderProcessedDetails(string id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var orderProcesses = OrderService.GetOrderProcessDetailsByProcessId(int.Parse(id ?? "0"));

            var model = orderProcesses.Select(x => new OrderProcessDetailsViewModel()
            {
                QtyProcessed = x.QtyProcessed,
                ProductName = x.ProductMaster.Name,
                ProductCode = x.ProductMaster.SKUCode,
                TaxPercent = x.OrderDetail.TaxName != null ? x.OrderDetail.TaxName.PercentageOfAmount : 0,
                ProductId = x.ProductId,
                OrderProcessId = x.OrderProcessId,
                Price = x.OrderDetail.Price,
                OrderProcessDetailId = x.OrderProcessDetailID,
                WarrantyAmount = x.OrderDetail?.Warranty != null ? x.OrderDetail.WarrantyAmount : 0
            }).ToList();
            ViewBag.OrderProcessID = id;
            return PartialView("_OrderProcessDetailsGridPartial", model);
        }

        public ActionResult CreateInvoice(int? id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var allTaxes = LookupServices.GetAllValidGlobalTaxes().ToList();
            var allWarranties = LookupServices.GetAllTenantWarrenties(CurrentTenantId).ToList();
            var InvoiceMaster = _invoiceService.GetInvoiceMasterById(id ?? 0);
            var accountId = InvoiceMaster?.AccountId;
            var productId = InvoiceMaster?.ProductId;
            var orderProcessId = InvoiceMaster?.OrderProcessId;

            var model = new InvoiceViewModel()
            {
                AccountId = accountId ?? 0,
                InvoiceMasterId = id ?? 0,
                OrderProcessId = orderProcessId ?? 0,
                AllAccounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).Select(m => new SelectListItem { Text = m.CompanyName, Value = m.AccountID.ToString(), Selected = m.AccountID == accountId ? true : false }).ToList(),
                //AllProducts = _productServices.GetAllValidProductMasters(CurrentTenantId).Select(m => new SelectListItem { Text = m.NameWithCode, Value = m.ProductId.ToString(), Selected = m.ProductId == productId ? true : false }).ToList(),
                AllTaxes = allTaxes.Select(m => new SelectListItem { Text = m.TaxName, Value = m.TaxID.ToString() }).ToList(),
                TaxDataHelper = Newtonsoft.Json.JsonConvert.SerializeObject(allTaxes.Select(m => new { m.TaxID, m.PercentageOfAmount })),
                AllWarranties = allWarranties.Select(m => new SelectListItem { Text = m.WarrantyName, Value = m.WarrantyID.ToString() }).ToList(),
                WarrantyDataHelper = Newtonsoft.Json.JsonConvert.SerializeObject(allWarranties.Select(m => new { m.WarrantyID, m.IsPercent, m.PercentageOfPrice, m.FixedPrice })),
            };

            LoadInvoiceProductValuesByInvoiceId(model, id);
            return View(model);
        }

        public JsonResult GetInvoiceDetail(int masterId)
        {
            InvoiceViewModel model = new InvoiceViewModel();
            LoadInvoiceProductValuesByInvoiceId(model, masterId);
            return Json(new { InvoiceProducts = model.AllInvoiceProducts, CurrentWarehouse.ShowPriceAlertInSalesOrder }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProductPrice(ProductPriceRequestModel model)
        {
            var productPriceData = _priceService.GetProductPriceThresholdByAccountId(model.ProductId, model.AccountId);
            var product = _productServices.GetProductMasterById(model.ProductId);
            var account = _accountService.GetAccountsById(model.AccountId);
            var PercentMargin = (_priceService.GetPercentageMarginPrice(product.BuyPrice, product.PercentMargin)) + (product.LandedCost ?? 0);
            int? taxId = (account?.GlobalTax?.PercentageOfAmount <= 0 ? account.TaxID : 0);
            return Json(new { productPriceData?.MinimumThresholdPrice, productPriceData?.SellPrice, ProductTaxID = taxId > 0 ? (taxId):(product?.GlobalTax?.TaxID), product?.AllowModifyPrice, product?.EnableTax, product?.BuyPrice, PercentMargin, ShowPriceAlert = CurrentWarehouse.ShowPriceAlertInSalesOrder, product.EnableWarranty, percentageValue = product.PercentMargin, minimumSalePrice = Math.Round((productPriceData.MinimumSellPrice ?? 0), 2) }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SaveInvoice(InvoiceViewModel model)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (model.InvoiceMasterId <= 0)
            {
                _invoiceService.CreateInvoiceForSalesOrder(model, CurrentTenantId, CurrentUserId);
            }
            else
            {
                _invoiceService.SaveInvoiceForSalesOrder(model, CurrentTenantId, CurrentUserId);

            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult GenerateBulkInvoice(InvoiceExportModel model)
        {
            var processIds = model.OrderProcessIds.Split(',');
            foreach (var item in processIds)
            {
                var invoice = GetInvoicePreviewModelByOrderProcessId(item.AsInt());
                var invoiceMaster = _invoiceService.CreateInvoiceForSalesOrder(invoice, CurrentTenantId, CurrentUserId);
                invoice.InvoiceMasterId = invoiceMaster.InvoiceMasterId;
                //ExportInvoiceByModel(invoice);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public ActionResult GenerateBulkInvoicePrint(InvoiceExportModel model)
        {
            var processIds = model.OrderProcessIds.Split(',');
            foreach (var item in processIds)
            {
                var invoice = GetInvoicePreviewModelByOrderProcessId(item.AsInt());
                var invoiceMaster = _invoiceService.CreateInvoiceForSalesOrder(invoice, CurrentTenantId, CurrentUserId);
                invoice.InvoiceMasterId = invoiceMaster.InvoiceMasterId;
                //ExportInvoiceByModel(invoice);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public async Task<ActionResult> GenerateInvoiceEmail(string OrderProcessIds)
        {
            var processIds = OrderProcessIds.Split(',');
            int orderId = 0;
            string messgae = "";
            foreach (var item in processIds)
            {
                var masterInvoice = _invoiceService.GetInvoiceMasterById(item.AsInt());
                var orderProcessId = masterInvoice.OrderProcessId;
                var OrderProcess = OrderService.GetOrderProcessByOrderProcessId(orderProcessId);
                orderId = OrderProcess.OrderID ?? 0;
                var accountContact = AccountServices.GetAllValidAccountContactsByAccountId(OrderProcess?.Order?.AccountID ?? 0, CurrentTenantId).Where(u => u.ConTypeInvoices == true).ToList();
                if (accountContact.Count > 0)
                {

                    var orderViewModel = _mapper.Map(OrderProcess.Order, new OrderViewModel());
                    orderViewModel.InvoiceId = item.AsInt();
                    var report = CreateInvoicePrint(masterInvoice.InvoiceMasterId);
                    PrepareDirectory("~/UploadedFiles/reports/Invoices/");
                    var reportPath = "~/UploadedFiles/reports/Invoices/" + masterInvoice.InvoiceNumber + ".pdf";
                    report.ExportToPdf(Server.MapPath(reportPath));

                    var result = await GaneConfigurationsHelper.CreateTenantEmailNotificationQueue($"#{masterInvoice.InvoiceNumber} - Invoice", orderViewModel, attachmentVirtualPath: reportPath,
                    worksOrderNotificationType: WorksOrderNotificationTypeEnum.InvoiceTemplate);
                    if (result == "Success")
                    {
                        result = "Email Sent";
                    }
                    messgae += $"{result} against Invoice Number: " + masterInvoice.InvoiceNumber + "\n";
                }
                else
                {
                    messgae += $"{"No invoicing email against Number: " + masterInvoice.InvoiceNumber} \n";
                }
            }

            return Json(string.IsNullOrEmpty(messgae) ? $"No Data Found Against this Invoice" : messgae, JsonRequestBehavior.AllowGet);

        }

        public ActionResult DownloadInvoice(string InvoiceMasterIds)
        {
            if (string.IsNullOrEmpty(InvoiceMasterIds)) return RedirectToAction("Index");
            int[] masterIds = Array.ConvertAll(InvoiceMasterIds.Split(','), Int32.Parse);
            var report = CreateInvoicePrint(0, masterIds);
            string filename = "" + masterIds[0] + "_Bulk";
            PrepareDirectory("~/UploadedFiles/reports/Invoices/");
            var reportPath = "~/UploadedFiles/reports/Invoices/" + filename + ".pdf";
            report.ExportToPdf(Server.MapPath(reportPath));
            var invoiceFile = Directory.GetFiles(Server.MapPath("~/UploadedFiles/reports/Invoices")).ToList().FirstOrDefault(m => Path.GetFileName(m).StartsWith(filename.ToString()));
            if (invoiceFile != null)
            {
                var mimeType = MimeMapping.GetMimeMapping(invoiceFile);

                var cd = new System.Net.Mime.ContentDisposition
                {
                    FileName = invoiceFile,
                    Inline = true,
                };

                Response.AppendHeader("Content-Disposition", cd.ToString());
                return File(System.IO.File.ReadAllBytes(invoiceFile), mimeType);
            }

            return RedirectToAction("Index");
        }

        public ActionResult GetInvoicePreview(int? id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = GetInvoicePreviewModelByInvoiceId(id ?? 0);
            return PartialView("_InvoicePreviewPartial", model);
        }

        public JsonResult GetAccountsList()
        {
            var accounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).Select(m => new { AccountName = m.CompanyName, AccountID = m.AccountID, AccountCode = m.AccountCode, Currency = m.GlobalCurrency.Symbol, AccountAddress = m.FullAddress });
            return Json(accounts, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAccountDetails(int id)
        {
            var m = AccountServices.GetAccountsById(id);
            return Json(new { AccountName = m.CompanyName, AccountID = m.AccountID, AccountCode = m.AccountCode, Currency = m.GlobalCurrency.Symbol, AccountAddress = m.FullAddressWithNameHtml, InvoiceDate = DateTime.UtcNow.ToString("dd/MM/yyyy") }, JsonRequestBehavior.AllowGet);
        }

        private void LoadInvoiceProductValuesByInvoiceId(InvoiceViewModel model, int? id)
        {
            if (!id.HasValue) return;

            var invoice = _invoiceService.GetInvoiceMasterById(id.Value);
            if (invoice != null)
            {
                model.InvoiceCurrency = invoice.InvoiceCurrency;
                model.InvoiceAddress = invoice.InvoiceAddress;
                model.AccountId = invoice.AccountId;
                model.InvoiceDate = invoice.InvoiceDate;
                model.TenantName = invoice.TenantName;
            }

            var invoiceDetails = _invoiceService.GetAllInvoiceDetailByInvoiceId(id.Value);

            foreach (var x in invoiceDetails)
            {
                var pd = model.AllInvoiceProducts.FirstOrDefault(m => m.ProductId == x.ProductId);
                var taxPercentage = _productServices.GetProductMasterById(x.ProductId)?.GlobalTax?.PercentageOfAmount ?? 0;
                var taxEnable = _productServices.GetProductMasterById(x.ProductId)?.EnableTax;

                if (pd == null)
                {
                    pd = new OrderProcessDetailsViewModel()
                    {
                        QtyProcessed = x.Quantity,
                        ProductName = x.Description,
                        //TODO: adding percentage here by adding navigation properties in invoices detail table
                        TaxPercent = x.Percentage.HasValue ? decimal.Parse(x.Percentage.ToString()) : 0,
                        ProductId = x.ProductId,
                        Price = x.Price,
                        TaxId = x.TaxId,
                        WarrantyId = x.WarrantyId

                    };
                    pd.WarrantyAmount = x.WarrantyAmount;
                }
                else
                {
                    pd.QtyProcessed += x.Quantity;
                }
                pd.MinSellPrice = Math.Round((_priceService.GetProductPriceThresholdByAccountId(x.ProductId, model.AccountId).MinimumSellPrice ?? 0), 2);
                model.AllInvoiceProducts.Add(pd);


            }

            model.TaxAmount = model.AllInvoiceProducts.Select(I => I.TaxAmount).DefaultIfEmpty(0).Sum();
            var amount = model.AllInvoiceProducts.Select(u => u.NetAmount).DefaultIfEmpty(0).Sum();
            model.NetAmount = amount - model.TaxAmount;
            model.WarrantyAmount += model.AllInvoiceProducts.Select(u => u.WarrantyAmount).DefaultIfEmpty(0).Sum();
            model.InvoiceTotal = Math.Round(model.NetAmount + model.TaxAmount + model.WarrantyAmount, 2);
        }


        public void ExportInvoiceByModel(InvoiceViewModel model)
        {
            var content = "<html><body><form id='frmInvoiceCreate'>" + _helper.GetActionResultHtml(this, "_InvoicePreviewPartial", model) + "</form></body></html>";
            DevexHtmlToPdfExport(content, model.InvoiceMasterId);
        }

        public InvoiceViewModel GetInvoicePreviewModelByOrderProcessId(int orderProcessId)
        {
            //var model = new InvoiceViewModel() { OrderProcessId = orderProcessId, AllAccounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).Select(m => new SelectListItem { Text = m.CompanyName, Value = m.AccountID.ToString() }).ToList() };

            var model = _invoiceService.LoadInvoiceProductValuesByOrderProcessId(orderProcessId);


            var invoice = _invoiceService.GetInvoiceMasterByOrderProcessId(orderProcessId);
            if (invoice != null)
            {
                model.InvoiceNumber = invoice.InvoiceNumber;
                model.InvoiceDate = invoice.InvoiceDate;
            }
            model.TenantName = CurrentTenant.TenantName;
            return model;
        }

        public InvoiceViewModel GetInvoicePreviewModelByInvoiceId(int invoiceId)
        {
            var model = new InvoiceViewModel() { InvoiceMasterId = invoiceId, AllAccounts = AccountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Customer).Select(m => new SelectListItem { Text = m.CompanyName, Value = m.AccountID.ToString() }).ToList() };

            LoadInvoiceProductValuesByInvoiceId(model, invoiceId);
            model.InvoiceDate = DateTime.UtcNow;
            var invoice = _invoiceService.GetInvoiceMasterById(invoiceId);
            if (invoice != null)
            {
                model.InvoiceNumber = invoice.InvoiceNumber;
                model.InvoiceDate = invoice.InvoiceDate;
            }
            model.TenantName = CurrentTenant.TenantName;
            return model;
        }

        [ValidateInput(false)]
        public byte[] DevexHtmlToPdfExport(string htmlContentString, int invoiceId)
        {
            HtmlEditorSettings settings = new HtmlEditorSettings();
            settings.SettingsHtmlEditing.AllowedDocumentType = AllowedDocumentType.Both;
            settings.SettingsHtmlEditing.AllowFormElements = true;
            settings.SettingsHtmlEditing.AllowIdAttributes = true;
            settings.SettingsHtmlEditing.AllowIFrames = true;
            settings.SettingsHtmlEditing.AllowStyleAttributes = true;
            settings.SettingsHtmlEditing.UpdateDeprecatedElements = true;
            settings.SettingsHtmlEditing.UpdateBoldItalic = true;

            settings.Name = "Invoice";
            settings.ActiveView = HtmlEditorView.Design;
            settings.Html = htmlContentString;

            settings.CssFiles.Add(HttpContext.Request.ApplicationPath.TrimEnd("/"[0]) + "/Content/fontawesome/css/font-awesome.min.css");
            settings.CssFiles.Add(HttpContext.Request.ApplicationPath.TrimEnd("/"[0]) + "/Content/Themes/UI-WZ/jquery-ui.css");
            settings.CssFiles.Add(HttpContext.Request.ApplicationPath.TrimEnd("/"[0]) + "/Content/site.css");
            settings.CssFiles.Add(HttpContext.Request.ApplicationPath.TrimEnd("/"[0]) + "/Content/bootstrap.css");

            var filePath = Server.MapPath(" ~/UploadedFiles/Invoices/" + invoiceId + "_") + DateTime.UtcNow.ToString("ddMMyyyyHHmmssfff") + ".pdf";
            var outputStream = System.IO.File.Open(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            HtmlEditorExtension.Export(settings, outputStream, HtmlEditorExportFormat.Pdf);
            var result = outputStream.ToByteArray();
            outputStream.Close();
            return result;
        }

        public JsonResult Export(string orderProcessId, string InvoiceIds)
        {
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            string fileName = "";
            DataTable dt = ReportDataTableDesign();
            if (!string.IsNullOrEmpty(orderProcessId))
            {
                var processIds = orderProcessId.Split(',');
                foreach (var item in processIds)
                {
                    var Id = item.AsInt();
                    var orderprocesses = context.OrderProcess.FirstOrDefault(u => u.OrderProcessID == Id);
                    if (orderprocesses != null && orderprocesses?.Order?.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.PurchaseOrder)
                    {
                        orderprocesses.OrderProcessStatusId = (int)OrderProcessStatusEnum.PostedToAccounts;
                        orderprocesses.UpdatedBy = CurrentUserId;
                        orderprocesses.DateUpdated = DateTime.UtcNow;

                        context.OrderProcess.Attach(orderprocesses);
                        context.Entry(orderprocesses).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        var orderPorcessCount = context.OrderProcess.Where(u => u.OrderID == (orderprocesses.OrderID ?? 0) && u.OrderProcessStatusId != (int)OrderProcessStatusEnum.PostedToAccounts).Count();
                        if (orderPorcessCount <= 0)
                        {
                            orderprocesses.Order.OrderStatusID = (int)OrderStatusEnum.PostedToAccounts;
                        }

                        InvoiceViewModel InvoiceReport = _invoiceService.LoadInvoiceProductValuesByOrderProcessId(Id, (int)InventoryTransactionTypeEnum.PurchaseOrder);
                        if (InvoiceReport != null)
                        {
                            if (orderprocesses?.Order?.OrderNumber != fileName)
                            {
                                if (string.IsNullOrEmpty(fileName))
                                {
                                    // purcahsinvoices-datestamp/ hour mint second
                                    fileName += "PI";
                                }
                            }
                            int? ProductNominalCode = null;
                            var NominalCode = orderprocesses.OrderProcessDetail.Select(u => u.ProductMaster.NominalCode).ToList();
                            var duplicates = NominalCode.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                            if (duplicates != null)
                            {
                                ProductNominalCode = duplicates;
                            }
                            else
                            {
                                ProductNominalCode = NominalCode.FirstOrDefault(u => u != null);
                            }

                            dt.Rows.Add("PI", orderprocesses?.Order?.Account.AccountCode, orderprocesses?.Order?.InvoiceNo,
                                (ProductNominalCode.HasValue ? ProductNominalCode : 4000), "", orderprocesses.InvoiceDate ?? DateTime.UtcNow, "", InvoiceReport.NetAmount.ToString("#.##"), "T1",
                                InvoiceReport.TaxAmount.ToString("#.##"), "1.0000", orderprocesses?.Order?.OrderNumber, caCurrent.CurrentUser().UserName, "", "");

                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(InvoiceIds))
            {
                var processIds = InvoiceIds.Split(',');
                foreach (var item in processIds)
                {
                    var Id = item.AsInt();

                    var oderProcessId = context.InvoiceMasters.FirstOrDefault(u => u.InvoiceMasterId == Id)?.OrderProcessId;
                    var orderprocess = context.OrderProcess.FirstOrDefault(u => u.OrderProcessID == oderProcessId);
                    var InvoiceReport = context.InvoiceMasters.FirstOrDefault(u => u.InvoiceMasterId == Id);
                    if (orderprocess != null)
                    {
                        orderprocess.OrderProcessStatusId = (int)OrderProcessStatusEnum.PostedToAccounts;
                        orderprocess.UpdatedBy = CurrentUserId;
                        orderprocess.InvoiceNo = InvoiceReport.InvoiceNumber;
                        orderprocess.DateUpdated = DateTime.UtcNow;

                        //orderprocess.Order.OrderStatusID = (int)OrderStatusEnum.PostedToAccounts;
                        context.OrderProcess.Attach(orderprocess);
                        context.Entry(orderprocess).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        var orderPorcessCount = context.OrderProcess.Count(u => u.OrderID == (orderprocess.OrderID ?? 0) && u.OrderProcessStatusId != (int)OrderProcessStatusEnum.PostedToAccounts);
                        if (orderPorcessCount <= 0)
                        {
                            orderprocess.Order.OrderStatusID = (int)OrderStatusEnum.PostedToAccounts;


                        }
                    }

                    InvoiceReport.InvoiceStatus = InvoiceStatusEnum.PostedToAccounts;
                    context.InvoiceMasters.Attach(InvoiceReport);
                    context.Entry(InvoiceReport).State = System.Data.Entity.EntityState.Modified;
                    if (InvoiceReport != null)
                    {
                        if (string.IsNullOrEmpty(fileName))
                        {
                            fileName += "SI";
                        }

                        int? ProductNominalCode = null;
                        var NominalCode = InvoiceReport.InvoiceDetails.Select(u => u.Product.NominalCode).ToList();
                        var duplicates = NominalCode.GroupBy(i => i).OrderByDescending(grp => grp.Count()).Select(grp => grp.Key).First();
                        if (duplicates != null)
                        {
                            ProductNominalCode = duplicates;
                        }
                        else
                        {
                            ProductNominalCode = NominalCode.FirstOrDefault(u => u != null);
                        }
                        dt.Rows.Add("SI", InvoiceReport.Account.AccountCode, InvoiceReport.InvoiceNumber, (ProductNominalCode.HasValue ? ProductNominalCode : 4000), "", InvoiceReport.InvoiceDate, "", InvoiceReport.NetAmount, "T1", InvoiceReport.TaxAmount,
                            "1.0000", orderprocess?.Order?.OrderNumber, caCurrent.CurrentUser().UserName, "", "");
                    }
                }
            }
            else
            {
                return Json(new { fileName = "", errorMessage = "" }, JsonRequestBehavior.AllowGet);

            }

            context.SaveChanges();

            using (XLWorkbook wb = new XLWorkbook())
            {
                fileName = fileName + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHmmssfff") + ".xlsx";
                var filePath = Server.MapPath(" ~/UploadedFiles/Invoices/" + fileName);
                wb.Worksheets.Add(dt);
                wb.SaveAs(filePath);
            }

            return Json(new { fileName = fileName, errorMessage = "" }, JsonRequestBehavior.AllowGet);

        }
        public virtual ActionResult Download(string file)
        {
            string fullPath = Server.MapPath(" ~/UploadedFiles/Invoices/" + file);
            var fileByteArray = System.IO.File.ReadAllBytes(fullPath);
            System.IO.File.Delete(fullPath);
            return File(fileByteArray, "application/vnd.ms-excel", file);
        }

        #region PurchaseOrderInvoice
        public ActionResult PurchaseInvoiceIndex()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        #endregion

        public DataTable ReportDataTableDesign()
        {
            DataTable dt = new DataTable("ExportOrderProcess");
            dt.Columns.AddRange(new DataColumn[]
            { new DataColumn("Type"),new DataColumn("Account Reference"),new DataColumn("Reference"),new DataColumn("Nominal A/C Ref"),new DataColumn("Department Code"),
                new DataColumn("Date"),new DataColumn("Details"),new DataColumn("Net Amount"),new DataColumn("Tax Code"),
                new DataColumn("Tax Amount"),new DataColumn("Exchange Rate"),new DataColumn("Extra Reference"),new DataColumn("User Name"),
                new DataColumn("Project Refn"),new DataColumn("Cost Code Refn")
            });

            return dt;

        }

        public ActionResult SaveInvoiceNumber(int OrderProcessID, string InvoiceNumber, string InvoiceDate)
        {
            DateTime invoiceDateTime = DateTime.MinValue;
            bool date = DateTime.TryParse(InvoiceDate, out invoiceDateTime);
            bool status = false;
            if (date)
            {
                status = OrderService.UpdateOrderInvoiceNumber(OrderProcessID, InvoiceNumber, invoiceDateTime);
            }
            else
            {

                status = OrderService.UpdateOrderInvoiceNumber(OrderProcessID, InvoiceNumber, null);
            }
            if (!status)
            {
                return ProcessedOrdersPartial("PO");
            }
            else
            {
                return ProcessedOrdersPartial("Active");
            }
        }

        public ActionResult CustomGridViewCallback(bool select)
        {
            ViewData["select"] = select;
            return ProcessedOrdersPartial("PO");
        }

    }
}
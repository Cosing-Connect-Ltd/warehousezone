using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class TenantConfigsController : BaseController
    {
        
        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IAccountServices _accountServices;
        private readonly ITenantsServices _tenantsServices;


        public TenantConfigsController(ITenantsServices tenantsServices,ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _userService = userService;
            _invoiceService = invoiceService;
            _lookupServices = lookupServices;
            _accountServices = accountServices;
            _tenantsServices = tenantsServices;
        }
        // GET: TenantConfigs
        public ActionResult Index()
        {
            var tenantConfigs = _tenantsServices.GetAllTenantConfig(CurrentTenantId);
            return View(tenantConfigs.ToList());
        }

        // GET: TenantConfigs/Details/5
        public ActionResult Details(int? id)
        {
            
            TenantConfig tenantConfig = _tenantsServices.GetTenantConfigById(CurrentTenantId);
            if (tenantConfig == null)
            {
                return HttpNotFound();
            }
            return View(tenantConfig);
        }

        // GET: TenantConfigs/Create
        public ActionResult Create()
        {
            var tenantConfig = _tenantsServices.GetTenantConfigById(CurrentTenantId);
            
            if (tenantConfig != null)
            {
                ViewBag.DefaultCashAccountID = new SelectList(_accountServices.GetAllValidAccounts(CurrentTenantId).ToList(), "AccountID", "AccountCode", tenantConfig.DefaultCashAccountID);
         
                return View(tenantConfig);

            }


            ViewBag.DefaultCashAccountID = new SelectList(_accountServices.GetAllValidAccounts(CurrentTenantId).ToList(), "AccountID", "AccountCode");
          


            return View();
        }

        // POST: TenantConfigs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenantConfigId,AlertMinimumProductPrice,EnforceMinimumProductPrice,AlertMinimumPriceMessage,EnforceMinimumPriceMessage,PoReportFooterMsg1,PoReportFooterMsg2,SoReportFooterMsg1,SoReportFooterMsg2,DnReportFooterMsg1,DnReportFooterMsg2,WorksOrderScheduleByMarginHours,WorksOrderScheduleByAmPm,EnableLiveEmails,MiniProfilerEnabled,EnabledRelayEmailServer,EnablePalletingOnPick,WarehouseLogEmailsToDefault,WarehouseScheduleStartTime,WarehouseScheduleEndTime,ErrorLogsForwardEmails,DefaultReplyToAddress,DefaultMailFromText,AutoTransferStockEnabled,EnableStockVarianceAlerts,AuthorisationAdminEmail,DefaultCashAccountID,TenantReceiptPrintHeaderLine1,TenantReceiptPrintHeaderLine2,TenantReceiptPrintHeaderLine3,TenantReceiptPrintHeaderLine4,TenantLogo,PrintLogoForReceipts,SessionTimeoutHours,TenantId,DateCreated,DateUpdated,CreatedBy,UpdatedBy,IsDeleted")] TenantConfig tenantConfig)
        {
            if (ModelState.IsValid)
            {
                if (tenantConfig.TenantConfigId > 0)
                {
                    tenantConfig.TenantId = CurrentTenantId;
                    tenantConfig.DateUpdated = DateTime.UtcNow;
                    tenantConfig.UpdatedBy = CurrentUserId;
                    _tenantsServices.UpdateTenantConfig(tenantConfig);

                }
                else
                {
                    tenantConfig.TenantId = CurrentTenantId;
                    tenantConfig.DateCreated = DateTime.UtcNow;
                    tenantConfig.CreatedBy = CurrentUserId;
                    _tenantsServices.AddTenantConfig(tenantConfig);
                }
                return RedirectToAction("Index");
            }

            ViewBag.DefaultCashAccountID = new SelectList(_accountServices.GetAllValidAccounts(CurrentTenantId).ToList(), "AccountID", "AccountCode", tenantConfig.DefaultCashAccountID);
            ViewBag.TenantId = new SelectList(_tenantsServices.GetAllTenants(), "TenantId", "TenantName");
            return View(tenantConfig);
        }

        public ActionResult TenantConfigGridViewPartial()
        {
            var tennatConfig = _tenantsServices.GetAllTenantConfig(CurrentTenantId);
            return PartialView("_TenantConfigGridViewPartial", tennatConfig.ToList());
        }




      

    }
}

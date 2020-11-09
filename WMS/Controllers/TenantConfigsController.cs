using DevExpress.Web;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace WMS.Controllers
{
    public class TenantConfigsController : BaseController
    {

        private readonly IUserService _userService;
        private readonly IInvoiceService _invoiceService;
        private readonly ILookupServices _lookupServices;
        private readonly IAccountServices _accountServices;
        private readonly ITenantsServices _tenantsServices;

        string UploadDirectory = "~/UploadedFiles/TenantConfigs/";
        string UploadTempDirectory = "~/UploadedFiles/TenantConfigs/TempFiles/";
        string UploadedFileSessionName = "UploadTenantConfigsLoyaltyAppSplashScreenImage";


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
            Session[UploadedFileSessionName] = null;
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

            Session[UploadedFileSessionName] = !string.IsNullOrEmpty(tenantConfig?.LoyaltyAppSplashScreenImage) ? new DirectoryInfo(tenantConfig.LoyaltyAppSplashScreenImage).Name : null;

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
        public ActionResult Create(TenantConfig tenantConfig, IEnumerable<UploadedFile> LoyaltyAppSplashScreenImageUploadControl)
        {
            if (ModelState.IsValid)
            {
                var fileName = Session[UploadedFileSessionName] as string;
                if (string.IsNullOrEmpty(fileName)) { tenantConfig.LoyaltyAppSplashScreenImage = ""; }
                else
                {
                    if (LoyaltyAppSplashScreenImageUploadControl != null && LoyaltyAppSplashScreenImageUploadControl.Count() > 0)
                    {
                        tenantConfig.LoyaltyAppSplashScreenImage = MoveFile(fileName, CurrentTenantId);
                    }
                }

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

        public ActionResult _FileUploader(string name, string filePath)
        {
            return PartialView(new FileUploaderViewModel
            {
                BindingName = name,
                DisplayName = name,
                UploadedFiles = !string.IsNullOrEmpty(filePath) ? new List<string> { new DirectoryInfo(filePath).Name } : null,
                AllowedFileExtensions = new string[] { ".jpg", ".jpeg", ".gif", ".png", ".ico" }
            });
        }

        private string MoveFile(string FileName, int RelatedObjectId)
        {
            if (!Directory.Exists(Server.MapPath(UploadDirectory + RelatedObjectId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + RelatedObjectId.ToString()));

            string sourceFile = Server.MapPath(UploadTempDirectory + @"/" + FileName);
            string destFile = Server.MapPath(UploadDirectory + RelatedObjectId.ToString() + @"/" + FileName);
            if (System.IO.File.Exists(sourceFile) && !System.IO.File.Exists(destFile))
            {
                System.IO.File.Move(sourceFile, destFile);
            }
            return (UploadDirectory.Replace("~", "") + RelatedObjectId.ToString() + @"/" + FileName);
        }

        public ActionResult UploadLoyaltyAppSplashScreenImageFile(IEnumerable<UploadedFile> LoyaltyAppSplashScreenImageUploadControl)
        {
            foreach (var file in LoyaltyAppSplashScreenImageUploadControl)
            {
                SaveFile(file);
                Session[UploadedFileSessionName] = file.FileName;
            }

            return Content("true");
        }

        private void SaveFile(UploadedFile file)
        {
            if (!Directory.Exists(Server.MapPath(UploadTempDirectory)))
                Directory.CreateDirectory(Server.MapPath(UploadTempDirectory));
            string resFileName = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            file.SaveAs(resFileName);
        }

        public JsonResult RemoveFile()
        {
            Session[UploadedFileSessionName] = null;
            return null;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            var binder = (DevExpressEditorsBinder)ModelBinders.Binders.DefaultBinder;

            binder.UploadControlBinderSettings.FileUploadCompleteHandler = (s, e) =>
            {
                e.CallbackData = e.UploadedFile.FileName;
            };

            base.Initialize(requestContext);
        }
    }
}

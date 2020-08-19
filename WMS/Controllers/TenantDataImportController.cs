using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Services;
using System.Linq;

namespace WMS.Controllers
{
    public class TenantDataImportController : BaseController
    {
        private DataImportFactory _importFactory;
        private IProductLookupService _productLookupService;

        public TenantDataImportController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, IProductLookupService productLookupService, ILookupServices lookupServices)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _importFactory = new DataImportFactory();
            _productLookupService = productLookupService;
        }
        // GET: TenantDataImport
        public ActionResult Index()
        {
            /// Authorization Check
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl, string ImportType)
        {

            string importResponse = "";
            try
            {
                //UploadControlExtension.GetUploadedFiles("UploadControl", FileUploadComplete)
                foreach (var file in UploadControl)
                {

                    var importsDirectory = Server.MapPath("~/UploadedFiles/imports/");
                    if (!Directory.Exists(importsDirectory))
                        Directory.CreateDirectory(importsDirectory);
                    var importFileName = Server.MapPath("~/UploadedFiles/imports/" + "/Import_" + ImportType + "_" +
                                                        DateTime.UtcNow.ToString("ddMMyyyyHHMMss") + "_" + file.FileName);
                    file.SaveAs(importFileName);

                    switch (ImportType)
                    {
                        case "ProductsCategoriesAssociations":
                            _importFactory.ImportProductsCategoriesAssociations(importFileName, Path.GetFileNameWithoutExtension(file.FileName), CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "ProductsAttributes":
                            _importFactory.ImportProductsAttributes(importFileName, CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "ProductsParentChildAssociations":
                            _importFactory.ImportParentChildProductsAssociations(importFileName, CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "Products":
                            _importFactory.ImportProducts(importFileName, Path.GetFileNameWithoutExtension(file.FileName), CurrentTenantId, CurrentWarehouseId).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "Accounts":
                            importResponse += "<li>" + _importFactory.ImportSupplierAccounts(importFileName, CurrentTenantId, null, CurrentUserId) + "</li>";
                            break;
                        case "AccountsWithMarketInfo":
                            importResponse += "<li>" + _importFactory.ImportSupplierAccounts(importFileName, CurrentTenantId, null, CurrentUserId, true) + "</li>";
                            break;
                        case "ProductPrice":
                            importResponse += "<li>" + _importFactory.ImportProductsPrice(importFileName, CurrentTenantId, CurrentWarehouseId, null, CurrentUserId) + "</li>";
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                importResponse = "<li>Import failed : " + ex.Message + "</li>";

                return Json(importResponse, JsonRequestBehavior.AllowGet);
            }
            return Json(importResponse, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ImportProductPriceCsv(int PriceGroupId)
        {
            var obj = LookupServices.GetAllPriceGroups(CurrentTenantId).Where(u => u.PriceGroupID == PriceGroupId)?.Select(u => u.Name).ToList();
            ViewBag.ProductGroup = obj.FirstOrDefault();
            ViewBag.ProductGroupId = PriceGroupId;
            return View();
        }

        public ActionResult UploadProductPriceFile(int? ProductGroupId, int? ActionDetail, string ImportType)
        {

            ViewBag.ProductGroupId = ProductGroupId;
            ViewBag.actionDetail = ActionDetail;
            ViewBag.ImportType = ImportType;
            UploadControlExtension.GetUploadedFiles("UploadControl", ValidationSettings, uc_FileUploadComplete);

            return null;
        }

        public readonly UploadControlValidationSettings ValidationSettings = new UploadControlValidationSettings
        {
            AllowedFileExtensions = new string[] { ".csv", },
            MaxFileSize = 100000000,
        };
        public void uc_FileUploadComplete(object sender, FileUploadCompleteEventArgs e)
        {
            if (e.UploadedFile.IsValid)
            {

                try
                {
                    int? ProductGroupId = ViewBag.ProductGroupId;
                    int? actiondetail = ViewBag.actionDetail;
                    string ImportType = ViewBag.ImportType;
                    _importFactory = new DataImportFactory();
                    var importsDirectory = Server.MapPath("~/UploadedFiles/imports/");
                    if (!Directory.Exists(importsDirectory))
                    {
                        Directory.CreateDirectory(importsDirectory);
                    }
                    var importFileName = Server.MapPath("~/UploadedFiles/imports/" + "/Import_" + ImportType + "_" + DateTime.UtcNow.ToString("ddMMyyyyHHMMss") + "_" + e.UploadedFile.FileName);
                    e.UploadedFile.SaveAs(importFileName, true);
                    string importResponse = "";
                    switch (ImportType)
                    {
                        case "ProductsCategoriesAssociations":
                            _importFactory.ImportProductsCategoriesAssociations(importFileName, Path.GetFileNameWithoutExtension(e.UploadedFile.FileName), CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "ProductsAttributes":
                            _importFactory.ImportProductsAttributes(importFileName, CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "ProductsParentChildAssociations":
                            _importFactory.ImportParentChildProductsAssociations(importFileName, CurrentTenantId, CurrentUserId, null).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "Products":
                            _importFactory.ImportProducts(importFileName, Path.GetFileNameWithoutExtension(e.UploadedFile.FileName), CurrentTenantId, CurrentWarehouseId).ForEach(m => importResponse += "<li>" + m + "</li>");
                            break;
                        case "Accounts":
                            importResponse += "<li>" + _importFactory.ImportSupplierAccounts(importFileName, CurrentTenantId, null, CurrentUserId) + "</li>";
                            break;
                        case "AccountsWithMarketInfo":
                            importResponse += "<li>" + _importFactory.ImportSupplierAccounts(importFileName, CurrentTenantId, null, CurrentUserId, true) + "</li>";
                            break;
                        case "ProductPrice":
                            importResponse += "<li>" + _importFactory.ImportProductsPrice(importFileName, CurrentTenantId, CurrentWarehouseId, null, CurrentUserId, ProductGroupId ?? 0, actiondetail ?? 0) + "</li>";
                            break;

                    }
                    e.CallbackData = importResponse;
                    e.ErrorText = importResponse;

                }
                catch (Exception ex)
                {

                    e.CallbackData = "<li>File not imported: " + ex.Message + "<li>";
                }
            }

        }



    }

}
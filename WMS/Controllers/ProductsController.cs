﻿using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Ganedata.Core.Models.AdyenPayments;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductsController : BaseController
    {
        private readonly IAccountServices _accountServices;
        private readonly IProductServices _productServices;
        private readonly ILookupServices _lookupServices;
        private readonly IProductLookupService _productLookupService;
        private readonly ITenantWebsiteService _tenantWebsiteService;
        private readonly ITenantsServices _tenantsServices;
        private readonly IProductPriceService _priceService;

        public ProductsController(ICoreOrderService orderService, 
                                  IPropertyService propertyService, 
                                  IAccountServices accountServices, 
                                  ILookupServices lookupServices, 
                                  IProductServices productServices, 
                                  ITenantWebsiteService tenantWebsiteService, 
                                  IProductLookupService productLookupService,
                                  ITenantsServices tenantsServices, IProductPriceService priceService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _accountServices = accountServices;
            _productServices = productServices;
            _lookupServices = lookupServices;
            _productLookupService = productLookupService;
            _tenantWebsiteService = tenantWebsiteService;
            _tenantsServices = tenantsServices;
            _priceService = priceService;
        }
        string UploadDirectory = "~/UploadedFiles/Products/";
        string UploadTempDirectory = "~/UploadedFiles/Products/TempFiles/";
        // GET: Products

        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productMaster = _productServices.GetProductMasterById(id.Value);
            if (productMaster == null)
            {
                return HttpNotFound();
            }
            return View(productMaster);
        }

        private void SetProductCreateViewBags(string id)
        {
            List<KeyValuePair<string, UploadedFileViewModel>> files = new List<KeyValuePair<string, UploadedFileViewModel>>();

            Session["files"] = files;
            Session["ProductKitModelItems"] = null;
            var weightUoms = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Weight).ToList();
            var dimensionUoms = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Dimensions).ToList();
            var taxes = _lookupServices.GetAllValidGlobalTaxes(taxType: TaxTypeEnum.Products).ToList();
            var weightGroups = _lookupServices.GetAllValidGlobalWeightGroups().ToList();
            var lotOptionCodes = _productLookupService.GetAllValidProductLotOptionsCodes().ToList();
            var lotProcessTypeCodes = _productLookupService.GetAllValidProductLotProcessTypeCodes().ToList();
            var WebSites = _lookupServices.GetAllValidWebsites(CurrentTenantId).ToList();

            ViewBag.DimensionUOMId = new SelectList(dimensionUoms, "UOMId", "UOM", dimensionUoms.Select(m => m.UOMId).FirstOrDefault());
            ViewBag.AttributeGroups = _productLookupService.GetAllValidProductAttributes();
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", taxes.Select(x => x.TaxID).FirstOrDefault());
            ViewBag.UOMId = new SelectList(weightUoms, "UOMId", "UOM", weightUoms.Select(m => m.UOMId).FirstOrDefault());
            ViewBag.WeightGroupId = new SelectList(weightGroups, "WeightGroupId", "Description", weightGroups.Select(x => x.WeightGroupId).FirstOrDefault());
            ViewBag.LotOptionCodeId = new SelectList(lotOptionCodes, "LotOptionCodeId", "Description", lotOptionCodes.Select(x => x.LotOptionCodeId).FirstOrDefault());
            ViewBag.LotProcessTypeCodeId = new SelectList(lotProcessTypeCodes, "LotProcessTypeCodeId", "Description", lotProcessTypeCodes.Select(x => x.LotProcessTypeCodeId).FirstOrDefault());
            ViewBag.Groups = new SelectList(_lookupServices.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            ViewBag.Category = new SelectList(_lookupServices.GetAllValidProductCategories(CurrentTenantId), "ProductCategoryId", "ProductCategoryName");
            ViewBag.Departments = new SelectList(_lookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            ViewBag.PalletType = new SelectList(_lookupServices.GetAllValidPalletTypes(CurrentTenantId), "PalletTypeId", "Description");
            ViewBag.ProductLocations = _productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId);
            ViewBag.ProductTag = _productLookupService.GetAllValidProductTag(CurrentTenantId);
            ViewBag.WebsiteIds = WebSites;
            ViewBag.SKUCode = id;
            ViewBag.Manufacturer = new SelectList(_lookupServices.GetAllValidProductManufacturer(CurrentTenantId), "Id", "Name");
            ViewBag.Accounts = new List<ProductAccountCodes>();
            ViewBag.ProductKitItems = new List<ProductKitMap>();
            ViewBag.ProductgroupIds = new List<ProductKitMap>();
            ViewBag.ProductPrices = new List<string>();
            ViewBag.Attributes = _productLookupService.GetAllValidProductAttributeValues().Select(patr => new
            {
                Id = patr.AttributeValueId,
                Attribute = patr.ProductAttributes.AttributeName,
                Value = patr.Value
            });

            var prefAccounts = _accountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier).Select(acnts => new { PreferredSupplier = acnts.AccountID, Supplier = acnts.AccountCode }).ToList();

            ViewBag.PreferredSuppliers = new SelectList(prefAccounts, "PreferredSupplier", "Supplier");

            ViewBag.IsDeliverectIntegrated = _tenantsServices.GetTenantConfigById(CurrentTenantId)?.LoyaltyAppOrderProcessType == LoyaltyAppOrderProcessTypeEnum.Deliverect;

            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.AbsolutePath.ToLower().Contains("edit"))
                    ViewBag.back = 1;

            }
        }

        private ProductMaster SetProductEditViewBags(int? id)
        {
            if (!id.HasValue) throw new Exception("Product cannot be found.");

            Session["pId"] = id;
            Session["ProductKitModelItems"] = null;

            ProductMaster productMaster = _productServices.GetProductMasterById(id.Value);


            if (productMaster == null) throw new Exception("Product cannot be found.");

            var weightUoms = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Weight);
            var dimensionUoms = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Dimensions);
            var taxes = _lookupServices.GetAllValidGlobalTaxes(taxType: TaxTypeEnum.Products).ToList();
            var weightGroups = _lookupServices.GetAllValidGlobalWeightGroups();
            var lotOptionCodes = _productLookupService.GetAllValidProductLotOptionsCodes();
            var lotProcessTypeCodes = _productLookupService.GetAllValidProductLotProcessTypeCodes();
            ViewBag.linksiteIds = productMaster.ProductTagMaps.Where(u => u.IsDeleted != true).Select(u => u.TagId).ToList();
            ViewBag.DimensionUOMId = new SelectList(dimensionUoms, "UOMId", "UOM", productMaster.DimensionUOMId);
            ViewBag.TaxID = new SelectList(taxes, "TaxID", "TaxName", productMaster.TaxID);
            ViewBag.UOMId = new SelectList(weightUoms, "UOMId", "UOM", productMaster.UOMId);
            ViewBag.WeightGroupId = new SelectList(weightGroups, "WeightGroupId", "Description", productMaster.WeightGroupId);
            ViewBag.LotOptionCodeId = new SelectList(lotOptionCodes, "LotOptionCodeId", "Description", productMaster.LotOptionCodeId);
            ViewBag.LotProcessTypeCodeId = new SelectList(lotProcessTypeCodes, "LotProcessTypeCodeId", "Description", productMaster.LotProcessTypeCodeId);
            ViewBag.productAttributeids = _productLookupService.GetAllValidProductAttributeMaps(CurrentTenantId).Where(x => x.ProductId == id.Value).Select(u => u.AttributeId).ToList();
            ViewBag.ProductLocations = _productLookupService.GetAllValidProductLocations(CurrentTenantId, CurrentWarehouseId);
            ViewBag.WebsiteIds = _lookupServices.GetAllValidWebsites(CurrentTenantId).ToList();
            ViewBag.ProductTag = _productLookupService.GetAllValidProductTag(CurrentTenantId).ToList();
            ViewBag.ProductLocationIds = _productLookupService.GetAllValidProductLocations(CurrentWarehouseId, CurrentTenantId, id.Value).Select(m => m.LocationId);
            ViewBag.Groups = new SelectList(_lookupServices.GetAllValidProductGroups(CurrentTenantId), "ProductGroupId", "ProductGroup");
            ViewBag.Category = new SelectList(_lookupServices.GetAllValidProductCategories(CurrentTenantId), "ProductCategoryId", "ProductCategoryName");
            ViewBag.PalletType = new SelectList(_lookupServices.GetAllValidPalletTypes(CurrentTenantId), "PalletTypeId", "Description", productMaster.PalletTypeId);
            ViewBag.AttributeGroups = _productLookupService.GetAllValidProductAttributes();
            ViewBag.Departments = new SelectList(_lookupServices.GetAllValidTenantDepartments(CurrentTenantId), "DepartmentId", "DepartmentName");
            ViewBag.Manufacturer = new SelectList(_lookupServices.GetAllValidProductManufacturer(CurrentTenantId), "Id", "Name", productMaster.ManufacturerId);
            ViewBag.Accounts = _accountServices.GetAllValidProductAccountCodes(id.Value).Select(o =>
            {

                return new { accountid = o.ProdAccCodeID, account = o.ProdAccCode };

            }).ToList();
            ViewBag.ProductAccountCodeIds = _accountServices.GetAllValidProductAccountCodes(id.Value).Select(o =>
                {
                    return o.ProdAccCodeID;
                }
            ).ToList();

            ViewBag.Attributes = _productLookupService.GetAllValidProductAttributeValues().
                Select(patr => new
                {
                    Id = patr.AttributeValueId,
                    Attribute = patr.ProductAttributes.AttributeName,
                    Value = patr.Value
                }).OrderBy(a => a.Attribute).ToList();

            ViewBag.ProductAttributesIds = _productLookupService.GetAllValidProductAttributeValuesMap().Where(x => (x.ProductId == id.Value)).Select(a => a.AttributeValueId).ToList();


            if (!productMaster.Discontinued)
                productMaster.DiscontDate = DateTime.Today.AddDays(15);

            ViewBag.ProductKitItems = new MultiSelectList(productMaster.ProductKitMap.Where(u => u.ProductKitType == ProductKitTypeEnum.Kit && u.IsDeleted != true).Select(pm => new { ProductId = pm.KitProductId, ProductName = pm.KitProductMaster.Name + "" + "(" + (pm.Quantity) + ")" }).ToList(), "ProductId", "ProductName", productMaster.ProductKitMap.Where(u => u.ProductKitType == ProductKitTypeEnum.Kit && u.IsDeleted != true).Select(u => u.KitProductId).ToList());
            ViewBag.ProductgroupItems = new MultiSelectList(productMaster.ProductKitMap.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsDeleted != true).Select(pm => new { ProductId = pm.KitProductId, ProductName = pm.KitProductMaster.Name }).ToList(), "ProductId", "ProductName", productMaster.ProductKitMap.Where(u => u.ProductKitType == ProductKitTypeEnum.Grouped && u.IsDeleted != true).Select(u => u.KitProductId).ToList());

            var prefAccounts = _accountServices.GetAllValidAccounts(CurrentTenantId, EnumAccountType.Supplier).Select(acnts => new
            {
                PreferredSupplier = acnts.AccountID,
                Supplier = acnts.AccountCode
            }).ToList();

            ViewBag.PreferredSuppliers = new SelectList(prefAccounts, "PreferredSupplier", "Supplier");

            var directory = Server.MapPath(UploadDirectory + id.ToString() + "/");
            Session["files"] = null;
            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.AbsolutePath.ToLower().Contains("edit"))
                    ViewBag.back = 1;
                else
                    Session["pId"] = id;
            }
            else
            {
                Session["pId"] = id;
            }
            var filePaths = _productServices.GetProductFiles(id ?? 0, CurrentTenantId);
            if (filePaths.Count() > 0)
            {
                List<KeyValuePair<string, UploadedFileViewModel>> files = new List<KeyValuePair<string, UploadedFileViewModel>>();
                Session["files"] = files;
                ViewBag.Files = files;
                foreach (var file in filePaths)
                {
                    DirectoryInfo dInfo = new DirectoryInfo(file.FilePath);
                    files.Add(new KeyValuePair<string, UploadedFileViewModel>(dInfo.Name, null));
                }
            }

            ViewBag.IsDeliverectIntegrated = _tenantsServices.GetTenantConfigById(CurrentTenantId)?.LoyaltyAppOrderProcessType == LoyaltyAppOrderProcessTypeEnum.Deliverect;
            LoadProductAttributesToView(productMaster, 1);
            return productMaster;
        }

   
        private void LoadProductAttributesToView(ProductMaster product, int priceGroupId)
        {
            ViewBag.ProductAttributeLinks =
                product.ProductSpecialAttributePrices.Where(m=> m.IsDeleted!=true && m.PriceGroupID == priceGroupId).Select(m => new ProductAttributeValuesMapModel()
                {
                    AttributeName = m.ProductAttribute.AttributeName,
                    AttributeValueName = m.ProductAttributeValue.Value,
                    AttributeSpecificPrice = m.AttributeSpecificPrice,
                    AttributeId = m.ProductAttributeId,
                    ProductId = m.ProductId,
                    AttributeValueId = m.ProductAttributeValueId,
                    ProductSpecialAttributePriceId = m.ProductSpecialAttributePriceId
                });

        }

        private void FillViewBagForProductTags()
        {
            ViewBag.Tags = _productLookupService.GetAllValidProductTag(CurrentTenantId).Select(u => new
            {
                u.Id,
                u.TagName
            }).ToList();

        }

        // GET: Products/Create
        public ActionResult Create(string id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            SetProductCreateViewBags(id);
            return View(new ProductMaster { SKUCode = string.IsNullOrEmpty(id) ? _productServices.GenerateNextProductCode(CurrentTenantId) : id, ProdStartDate = DateTime.Today, EnableTax = true, IsActive = true, DiscontDate = DateTime.Today.AddDays(15), ProductType = ProductKitTypeEnum.Simple });
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductMaster productMaster, List<int> AttributeIds, IEnumerable<DevExpress.Web.UploadedFile> UploadControl, List<string> ProductAccountCodeIds, List<int> ProductAttributesIds, List<int> ProductLocationIds, string back, List<int> SiteIds)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }


            if (ModelState.IsValid)
            {

                try
                {
                    if (!_productServices.IsCodeAvailableForUse(productMaster.SKUCode, CurrentTenantId, EnumProductCodeType.SkuCode))
                    {
                        throw new Exception("Product with same code already exist");
                    }

                    productMaster = _productServices.SaveProduct(productMaster, ProductAccountCodeIds, ProductAttributesIds, ProductLocationIds, AttributeIds, CurrentUserId, CurrentTenantId, SiteIds, (List<RecipeProductItemRequest>)Session["ProductKitModelItems"]);

                    var files = Session["files"] as List<KeyValuePair<string, UploadedFileViewModel>>;
                    foreach (var file in files)
                    {
                        string filePath = MoveFile(file.Value, productMaster.ProductId);
                        if (!string.IsNullOrEmpty(filePath))
                        {
                            _productServices.SaveProductFile(filePath, productMaster.ProductId, CurrentTenantId, CurrentUserId);

                        }

                    }

                    if (back == "1")
                    {
                        //_productServices.UpdateProductKitMap(productMaster.ProductId, (int)Session["pId"], CurrentUserId, CurrentTenantId);
                        //return RedirectToAction("Edit", new { id = Session["pId"] });
                    }

                }
                catch (Exception ex)
                {
                    throw ex;
                    SetProductCreateViewBags(productMaster.SKUCode);
                    ViewBag.Error = ex.Message;
                    return View(productMaster);
                }

                return RedirectToAction("Index");
            }

            SetProductCreateViewBags(productMaster.SKUCode);
            return View(productMaster);

        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productMaster = SetProductEditViewBags(id);

            return View(productMaster);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductMaster productMaster, List<int> AttributeIds, List<string> ProductAccountCodeIds, List<int> ProductAttributesIds, List<int> ProductLocationIds, string back, List<int> SiteIds)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            try
            {
                _productServices.SaveProduct(productMaster, ProductAccountCodeIds, ProductAttributesIds, ProductLocationIds, AttributeIds, CurrentUserId, CurrentTenantId, SiteIds, (List<RecipeProductItemRequest>)Session["ProductKitModelItems"]);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                SetProductEditViewBags(productMaster.ProductId);
                return View(productMaster);
            }

            return RedirectToAction("Index");
        }
        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productMaster = _productServices.GetProductMasterById(id.Value);

            if (productMaster == null)
            {
                return HttpNotFound();
            }

            if (Request.UrlReferrer != null)
            {
                if (Request.UrlReferrer.AbsolutePath.ToLower().Contains("edit"))
                {
                    ViewBag.back = 1;
                }
            }

            var kits = _productServices.GetAllProductInKitsByProductId(id.Value);

            ViewBag.Kits = kits.Any() ? kits : null /*(bool?)true :*/;

            return View(productMaster);
        }
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, string back)
        {
            if (back == "1")
            {
                var kitProductId = 0;
                if (Session["pId"] != null)
                {
                    kitProductId = (int)Session["pId"];
                }
                _productServices.DeleteProductsAndKits(id, kitProductId, CurrentTenantId, CurrentUserId);
                return RedirectToAction("Edit", new { id });
            }
            else
            {
                _productServices.DeleteProductsAndKits(id, 0, CurrentTenantId, CurrentUserId);
            }
            return RedirectToAction("Index");
        }

        // This is used for MVC remote method to validate dynamic values
        // validate dynamic values from database through Ajax, JQuery Validate libraries
        public JsonResult IsSKUAvailable(string skucode, int ProductId = 0)
        {
            if (!String.IsNullOrEmpty(skucode)) skucode = skucode.Trim();

            return Json(_productServices.IsCodeAvailableForUse(skucode, CurrentTenantId, EnumProductCodeType.SkuCode, ProductId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsNameAvailable(string Name, int ProductId = 0)
        {
            if (!String.IsNullOrEmpty(Name)) Name = Name.Trim();

            return Json(_productServices.IsNameAvailableForUse(Name, CurrentTenantId, EnumProductCodeType.All, ProductId), JsonRequestBehavior.AllowGet);
        }

        // This is used for MVC remote method to validate dynamic values
        // validate dynamic values from database through Ajax, JQuery Validate libraries
        public JsonResult IsBarCodeAvailable(string barcode, int ProductId = 0)
        {

            if (String.IsNullOrEmpty(barcode))
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {

            }
            barcode = barcode.Trim();

            return Json(_productServices.IsCodeAvailableForUse(barcode, CurrentTenantId, EnumProductCodeType.Barcode, ProductId), JsonRequestBehavior.AllowGet);
        }

        public JsonResult _CreateProductGroup(string productGroup)
        {
            if (!String.IsNullOrEmpty(productGroup)) throw new Exception("Product group name cannot be empty");

            var pGroup = _productLookupService.CreateProductGroup(new ProductGroups() { ProductGroup = productGroup }, CurrentUserId, CurrentTenantId);

            return Json(new { id = pGroup.ProductGroupId, group = productGroup }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult _CreateProductLocation(string productLocation)
        {
            if (!String.IsNullOrEmpty(productLocation)) throw new Exception("Location name cannot be empty");

            var plocation = _productServices.SaveLocationGroup(productLocation, CurrentUserId, CurrentTenantId);

            return Json(new { id = plocation.LocationGroupId, location = productLocation }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult _ProductGroupSubmit(ProductGroups model)
        {
            try
            {
                ProductGroups pGroup = _productLookupService.CreateProductGroup(model, CurrentUserId, CurrentTenantId);
                return Json(new { error = false, id = pGroup.ProductGroupId, productgroup = model.ProductGroup });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });

            }
        }

        [HttpPost]
        public JsonResult _LocationSubmit(Locations model)
        {
            try
            {
                _productLookupService.SaveProductLocation(model, CurrentWarehouseId, CurrentTenantId, CurrentUserId);

                return Json(new { error = false, id = model.LocationId, location = model.LocationCode });

            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });
            }

        }

        public PartialViewResult _ProductGroup(int? Id)
        {
            if (Id == null)
                return PartialView("_PGroup");
            else
            {
                var model = _productLookupService.GetProductGroupById(Id.Value);
                return PartialView("_PGroup", model);
            }
        }

        public ActionResult _LocationCreate(int? Id)
        {
            ViewBag.LocationGroups = _lookupServices.GetAllValidLocationGroups(CurrentTenantId).Select(locg => new { Id = locg.LocationGroupId, Group = locg.Locdescription });
            ViewBag.DimensionUOMs = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Dimensions).Select(duom => new { Id = duom.UOMId, DUOM = duom.UOM });
            ViewBag.UOMs = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Weight).Select(duom => new { Id = duom.UOMId, UOM = duom.UOM }).ToList();
            ViewBag.LocationTypes = _lookupServices.GetAllValidLocationTypes(CurrentTenantId);
            //ViewBag.Products = _productServices.GetAllValidProductMasters(CurrentTenantId).ToList();

            if (Id == null) return PartialView("_LocationCreate", new Locations());
            else
            {
                ViewBag.ProductIds = _productServices.GetAllProductsInALocationFromMaps(Id.Value);
                return PartialView("_LocationCreate", _productLookupService.GetLocationById(Id.Value));
            }
        }

        public ActionResult _AttributeValueCreate(int? Id)
        {
            if (Id == null)
            {
                ViewBag.Attributes = _productLookupService.GetAllValidProductAttributes();
                ViewBag.AttributeValues = _productLookupService.GetAllValidProductAttributeValues();
                return PartialView();
            }
            else
            {
                var productAttributeValuesMap = _productLookupService.GetProductAttributeValuesMapById(Id.Value);
                ViewBag.AttributeValues = _productLookupService.GetAllValidProductAttributeValues().Where(a => a.AttributeId == productAttributeValuesMap.ProductAttributeValues.AttributeId);
                ViewBag.ProductAttributeValuesMapId = Id;
                return PartialView(productAttributeValuesMap.ProductAttributeValues);
            }
        }

        public ActionResult _ProductAttributePriceCreateEdit()
        {
            int productId = int.Parse(Request.Params["productId"]);
            int priceGroupId = int.Parse(Request.Params["priceGroupId"]);
            ViewBag.Attributes = _productLookupService.GetAllValidProductAttributes();
            ViewBag.AttributeValues = _productLookupService.GetAllValidProductAttributeValues();
            ViewBag.PriceGroupId = priceGroupId;
            ViewBag.SelectedProductId = productId;
            var priceGroup = _priceService.GetTenantPriceGroupById(priceGroupId);
            ViewBag.PriceGroupName = priceGroup.Name;

            var product = _productServices.GetProductMasterById(productId);
            ViewBag.ProductName = product.Name;
            ViewBag.ProductId = productId;

            //if (id.HasValue)
            //{
            //    var productAttributeValuesMap = _productLookupService.GetProductAttributeValuesMapById(id.Value);
            //    ViewBag.AttributeValues = _productLookupService.GetAllValidProductAttributeValues().Where(a => a.AttributeId == productAttributeValuesMap.ProductAttributeValues.AttributeId);
            //    return PartialView(productAttributeValuesMap);
            //}

            return PartialView();
        }

        [HttpPost]
        public ActionResult SaveProductAttributePrice(ProductSpecialAttributePrice model)
        {
            var map = _productLookupService.SaveProductAttributeValuesMap(model, CurrentUserId, CurrentTenantId);
            return Json(map.ProductSpecialAttributePriceId);
        }
        [HttpPost]
        public ActionResult DeleteProductAttributePrice(ProductSpecialAttributePrice model)
        {
            var result = _productLookupService.DeleteProductAttributeValuesMap(model.ProductSpecialAttributePriceId, CurrentUserId);
            return Json(result);
        }

        public ActionResult _AttributeCreate()
        {
            return PartialView();
        }
        public ActionResult _AttrValueCreate(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound("Product Attribute Id cannot be blank.");
            var productAttribute = _productLookupService.GetProductAttributeById(id.Value);
            var model = new ProductAttributeValues()
            {
                ProductAttributes = new ProductAttributes()
                {
                    AttributeId = id??0,
                    IsPriced = productAttribute.IsPriced,
                    IsColorTyped = productAttribute.IsColorTyped
                }
            };

            return PartialView(model);
        }
        [HttpPost]
        public JsonResult _AttributesValueSubmit(ProductAttributeValues model)
        {
            try
            {
                if (model.AttributeId <= 0)
                {
                    return Json(new { error = true, errormessage = "Attribute must be selected first" });
                }
                var chkAttribute = _productLookupService.SaveProductAttributeValue(model.AttributeId, model.Value, model.SortOrder, model.Color, CurrentUserId);
                if (chkAttribute == null)
                    throw new Exception("Attribute Already exists");

                return Json(new { error = false, id = chkAttribute.AttributeValueId, name = chkAttribute.Value });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });
            }
        }

        [HttpPost]
        public JsonResult _AttributeSubmit(ProductAttributes model)
        {
            try
            {
                var chkAttribute = _productLookupService.SaveProductAttribute(model.AttributeName, model.SortOrder, model.IsColorTyped, null, model.IsPriced);
                if (chkAttribute == null)
                    throw new Exception("Attribute Already exists");

                return Json(new { error = false, id = chkAttribute.AttributeId, name = chkAttribute.AttributeName });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });
            }
        }
        [HttpPost]
        public JsonResult _AttributeValueSubmit(ProductAttributeValues model)
        {
            try
            {
                var value = _productLookupService.GetAllValidProductAttributeValues().FirstOrDefault(u => u.AttributeValueId == model.AttributeValueId);
                return Json(new { error = false, Id = value.AttributeValueId, value = model.Value });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });
            }
        }
        public JsonResult GetAttributeValuesById(int id)
        {
            try
            {
                var value = _productLookupService.GetAllValidProductAttributeValues().Where(u => u.AttributeId == id).Select(u => new { u.AttributeValueId, u.Value });
                return Json(new { error = false, data = value });
            }
            catch (Exception exp)
            {
                return Json(new { error = true, errormessage = exp.Message });
            }


        }


        public JsonResult _CreateProductAccount(ProductAccountCodes productAccountCodes)
        {
            var productAccountCode = _productServices.CreateProductAccountCodes(productAccountCodes, CurrentTenantId, CurrentUserId);
            return Json(productAccountCode, JsonRequestBehavior.AllowGet);

        }
        public JsonResult _GetOrderingNotes(int? ProdAccId)
        {
            if (ProdAccId.HasValue)
            {
                var orderingNotes = AccountServices.GetProductAccountCodesById(ProdAccId ?? 0)?.ProdOrderingNotes;
                return Json(orderingNotes, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public PartialViewResult _ProductCategory(int? Id)
        {
            if (Id == null)
                return PartialView();
            else
                return PartialView(_lookupServices.GetTenantDepartmentById(Id.Value));

        }

        [HttpPost]
        public JsonResult _ProductCategorySubmit(TenantDepartments model)
        {
            var dept = _lookupServices.SaveTenantDepartment(model.DepartmentName, model.AccountID, CurrentUserId, CurrentTenantId);
            if (dept == null)
            {
                return Json(new { error = true, errormessage = "Department already exists." });
            }
            return Json(new { error = false, id = dept.DepartmentId, category = dept.DepartmentName });
        }


        public PartialViewResult _ProductAccount(int? Id)
        {
            ViewBag.Accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                accountid = acnts.AccountID,
                account = acnts.AccountCode
            }).ToList();
            if (Id == null)
                return PartialView();
            else
                return PartialView(_accountServices.GetProductAccountCodesById(Id.Value));

        }
        public PartialViewResult _ProductKits(int? kitTypeId, int? ProductKitId, bool? Grouped = false)
        {
            if (kitTypeId.HasValue && kitTypeId == 2)
            { Grouped = true; }
            ViewBag.kitType = kitTypeId;
            ViewBag.grouped = Grouped;

            if (Grouped == true && !ProductKitId.HasValue)
            {
                ViewBag.groupedtype = new SelectList(_productLookupService.GetProductKitTypes(CurrentTenantId).ToList(), "Id", "Name");
            }
            if (ProductKitId.HasValue)
            {
                var productKit = _productServices.GetProductInKitsByKitId(ProductKitId ?? 0);
                ViewBag.productId = productKit.KitProductId;
                if (Grouped == true && ProductKitId.HasValue)
                {
                    ViewBag.groupedtype = new SelectList(_productLookupService.GetProductKitTypes(CurrentTenantId).ToList(), "Id", "Name", productKit.ProductKitTypeId);
                }
                return PartialView(productKit);

            }

            return PartialView();
        }

        public JsonResult AddProductKitItemsWithQuantity(RecipeProductItemRequest model)
        {

            if (Session["ProductKitModelItems"] == null)
            {
                var recipeItems = new List<RecipeProductItemRequest> { model };
                Session["ProductKitModelItems"] = recipeItems;
                return Json(model, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var productList = (List<RecipeProductItemRequest>)Session["ProductKitModelItems"];
                var checkDataForAlternativeTypes = productList.Where(u => u.KitType != model.KitType).ToList();
                if (checkDataForAlternativeTypes.Count > 0)
                {
                    Session["ProductKitModelItems"] = null;
                }
                var existingProduct = productList.FirstOrDefault(m => m.ProductId == model.ProductId);
                if (existingProduct == null)
                {
                    productList.Add(model);
                }
                else
                {
                    var index = productList.IndexOf(existingProduct);
                    productList[index].Quantity += model.Quantity;
                }
                Session["ProductKitModelItems"] = productList;
                return Json(model, JsonRequestBehavior.AllowGet);
            }


        }

        public JsonResult SaveEditKitProduct(int KitId, decimal? Quantity, int ProductId, int? ProductKitTypeId)
        {
            _productServices.SaveProductKit(KitId, (Quantity ?? 1), ProductId, CurrentUserId, ProductKitTypeId);
            return Json(true, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetNextProductCode()
        {
            return Json(_productServices.GenerateNextProductCode(CurrentTenantId), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ProductList()
        {
            var viewModel = GridViewExtension.GetViewModel("ProductListGridView");

            if (viewModel == null)
                viewModel = ProductListCustomBinding.CreateProductGridViewModel();

            if (string.IsNullOrEmpty(viewModel.FilterExpression) && ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("ProductsList"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["ProductsList"];
                var decodedValue = HttpUtility.UrlDecode(cookie.Value);
                var filterParams = decodedValue
                    .Split('|')
                    .ToList();
                var lengthParam = filterParams.Where(x => x.StartsWith("filter")).SingleOrDefault();

                if (!string.IsNullOrEmpty(lengthParam))
                {
                    var index = filterParams.IndexOf(lengthParam);
                    var savedFilterExpression = filterParams[index + 1];
                    viewModel.FilterExpression = savedFilterExpression;
                }

                var pageNo = filterParams.Where(x => x.StartsWith("page")).SingleOrDefault();
                if (!string.IsNullOrEmpty(pageNo))
                {
                    var index = filterParams.IndexOf(pageNo);
                    var savedPageNo = filterParams[index];
                    var savedPageSize = filterParams[index + 1];
                    GridViewPagerState state = new GridViewPagerState();
                    state.PageIndex = Convert.ToInt32(string.Join("", savedPageNo.ToCharArray().Where(Char.IsDigit))) - 1;
                    state.PageSize = Convert.ToInt32(string.Join("", savedPageSize.ToCharArray().Where(Char.IsDigit)));
                    viewModel.Pager.Assign(state);
                }
            }

            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductListPaging(GridViewPagerState pager)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductListGridView");
            viewModel.Pager.Assign(pager);
            return ProductGridActionCore(viewModel);
        }

        public ActionResult _ProductsFiltering(GridViewFilteringState filteringState)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductListGridView");
            viewModel.ApplyFilteringState(filteringState);
            return ProductGridActionCore(viewModel);
        }


        public ActionResult _ProductsGetDataSorting(GridViewColumnState column, bool reset)
        {
            var viewModel = GridViewExtension.GetViewModel("ProductListGridView");
            viewModel.ApplySortingState(column, reset);
            return ProductGridActionCore(viewModel);
        }



        public ActionResult ProductGridActionCore(GridViewModel gridViewModel)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProductListCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProductListCustomBinding.ProductGetData(args, CurrentTenantId, CurrentWarehouseId);
                    })
            );
            return PartialView("_ProductList", gridViewModel);
        }

        #region Tabs


        #region  EditableGrid

        public ActionResult EditProducts()
        {
            return View();
        }
        public ActionResult _EditableProductGrid(bool? AssociatedItem, string gridName, ProductKitTypeEnum? KitType = null, int? productId = null)
        {
            if (!AssociatedItem.HasValue) { FillViewBagForProductTags(); }
            ViewBag.AssociatedItem = AssociatedItem;
            ViewBag.KitTypes = KitType;
            ViewBag.ProductId = productId;
            if (KitType == ProductKitTypeEnum.Grouped)
            {
                ViewBag.GetKitTypes = _productLookupService.GetProductKitTypes(CurrentTenantId).ToList();
            }

            gridName = string.IsNullOrEmpty(gridName)
                ? (KitType.HasValue ? (KitType == ProductKitTypeEnum.RelatedProduct ? "RelatedProductListGridView" :
                "ProductEditListGridView") : "ProductEditListGridView") : gridName;
            var viewModel = GridViewExtension.GetViewModel(gridName);

            if (viewModel == null)
                viewModel = ProductListCustomBinding.CreateProductGridViewModel();

            return ProductEditGridActionCore(viewModel, KitType, productId);
        }
        public ActionResult _ProductEditListPaging(GridViewPagerState pager, string gridName, bool? AssociatedItem, ProductKitTypeEnum? KitType = null, int? productId = null)
        {
            if (!AssociatedItem.HasValue) { FillViewBagForProductTags(); }
            ViewBag.AssociatedItem = AssociatedItem;
            ViewBag.KitTypes = KitType;
            if (KitType == ProductKitTypeEnum.Grouped)
            {
                ViewBag.GetKitTypes = _productLookupService.GetProductKitTypes(CurrentTenantId).ToList();
            }
            var viewModel = GridViewExtension.GetViewModel(gridName);
            viewModel.Pager.Assign(pager);
            return ProductEditGridActionCore(viewModel, KitType, productId);
        }
        public ActionResult _ProductsEditFiltering(GridViewFilteringState filteringState, string gridName, bool? AssociatedItem, ProductKitTypeEnum? KitType = null, int? productId = null)
        {
            if (!AssociatedItem.HasValue) { FillViewBagForProductTags(); }
            ViewBag.AssociatedItem = AssociatedItem;
            ViewBag.KitTypes = KitType;
            if (KitType == ProductKitTypeEnum.Grouped)
            {
                ViewBag.GetKitTypes = _productLookupService.GetProductKitTypes(CurrentTenantId).ToList();
            }
            var viewModel = GridViewExtension.GetViewModel(gridName);
            viewModel.ApplyFilteringState(filteringState);
            return ProductEditGridActionCore(viewModel, KitType, productId);
        }
        public ActionResult _ProductsEditGetDataSorting(GridViewColumnState column, bool reset, string gridName, bool? AssociatedItem, ProductKitTypeEnum? KitType = null, int? productId = null)
        {
            if (!AssociatedItem.HasValue) { FillViewBagForProductTags(); }
            ViewBag.AssociatedItem = AssociatedItem;
            ViewBag.KitTypes = KitType;
            if (KitType == ProductKitTypeEnum.Grouped)
            {
                ViewBag.GetKitTypes = _productLookupService.GetProductKitTypes(CurrentTenantId).ToList();
            }
            var viewModel = GridViewExtension.GetViewModel(gridName);
            viewModel.ApplySortingState(column, reset);
            return ProductEditGridActionCore(viewModel, KitType, productId);
        }
        public ActionResult ProductEditGridActionCore(GridViewModel gridViewModel, ProductKitTypeEnum? KitType, int? productId = null)
        {
            gridViewModel.ProcessCustomBinding(
                new GridViewCustomBindingGetDataRowCountHandler(args =>
                {
                    ProductListCustomBinding.ProductGetDataRowCount(args, CurrentTenantId, CurrentWarehouseId, KitType, productId);
                }),

                    new GridViewCustomBindingGetDataHandler(args =>
                    {
                        ProductListCustomBinding.ProductGetData(args, CurrentTenantId, CurrentWarehouseId, KitType, productId);
                    })
            );
            return PartialView("_EditableProductGrid", gridViewModel);
        }
        public ActionResult SaveProductEdit(int productId, MVCxGridViewBatchUpdateValues<ProductMasterViewModel, int> updateValues)
        {
            if (productId > 0)
            {
                foreach (var value in updateValues.Update)
                {
                    value.ProductId = productId;
                    var res = _productServices.SaveEditProduct(value, CurrentUserId, CurrentTenantId);
                    //results.Add(res);
                }
            }


            return _EditableProductGrid(null, null);
        }
        public PartialViewResult _GetAssociatedItems(ProductKitTypeEnum productKitType, int productId)
        {
            ViewBag.AssociatedItem = true;
            ViewBag.KitTypes = productKitType;
            ViewBag.ProductId = productId;
            return PartialView();
        }
        public ActionResult SaveAssociated(string gridName, bool? AssociatedItem, ProductKitTypeEnum KitType, int ProductID, MVCxGridViewBatchUpdateValues<ProductMasterViewModel, int> updateValues)
        {
            List<bool> results = new List<bool>();
            foreach (var value in updateValues.Update)
            {

                var res = _productLookupService.CreateOrUpdateKitMap(value, ProductID, KitType, CurrentUserId, CurrentTenantId);
                //results.Add(res);
            }

            return _EditableProductGrid(AssociatedItem, gridName, KitType, ProductID);
        }

        #endregion



        public ActionResult UploadFile(IEnumerable<DevExpress.Web.UploadedFile> UploadControl)
        {
            if (Session["files"] == null)
                Session["files"] = new List<KeyValuePair<string, UploadedFileViewModel>>();
            var files = Session["files"] as List<KeyValuePair<string, UploadedFileViewModel>>;
            foreach (var file in UploadControl)
            {
                if (Request.UrlReferrer.AbsolutePath.Contains("Edit"))
                {
                    string filePath = SaveFileWithPId(file, Convert.ToInt32(Session["pId"]));
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        _productServices.SaveProductFile(filePath, Convert.ToInt32(Session["pId"]), CurrentTenantId, CurrentUserId);

                    }

                    files.Add(new KeyValuePair<string, UploadedFileViewModel>(file.FileName, new UploadedFileViewModel { FileName = file.FileName }));
                }
                else
                {
                    SaveFile(file);
                    files.Add(new KeyValuePair<string, UploadedFileViewModel>(file.FileName, new UploadedFileViewModel { FileName = file.FileName }));
                }
            }

            return null;
        }

        private void SaveFile(DevExpress.Web.UploadedFile file)
        {
            if (!Directory.Exists(Server.MapPath(UploadTempDirectory)))
                Directory.CreateDirectory(Server.MapPath(UploadTempDirectory));
            string resFileName = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            file.SaveAs(resFileName);
        }

        private string SaveFileWithPId(DevExpress.Web.UploadedFile file, int ProductId)
        {
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductId.ToString())))
            {
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductId.ToString()));
            }
            string resFileName = Server.MapPath(UploadDirectory + ProductId.ToString() + @"/" + file.FileName);
            file.SaveAs(resFileName);
            return (UploadDirectory.Replace("~", "") + ProductId.ToString() + @"/" + file.FileName);

        }

        private string MoveFile(UploadedFileViewModel file, int ProductId)
        {
            if (!Directory.Exists(Server.MapPath(UploadDirectory + ProductId.ToString())))
                Directory.CreateDirectory(Server.MapPath(UploadDirectory + ProductId.ToString()));

            string sourceFile = Server.MapPath(UploadTempDirectory + @"/" + file.FileName);
            string destFile = Server.MapPath(UploadDirectory + ProductId.ToString() + @"/" + file.FileName);
            System.IO.File.Move(sourceFile, destFile);
            return (UploadDirectory.Replace("~", "") + ProductId.ToString() + @"/" + file.FileName);

        }

        [HttpPost]
        public JsonResult _RemoveFile(string filename)
        {



            var files = Session["files"] as List<KeyValuePair<string, UploadedFileViewModel>>;
            var filetoremove = files.FirstOrDefault(a => a.Key == filename);
            files.Remove(filetoremove);
            if (Request.UrlReferrer.AbsolutePath.Contains("Edit"))
            {
                string FilePath = (UploadDirectory.Replace("~", "") + Session["pId"].ToString() + "/" + filename);
                _productServices.RemoveFile(Convert.ToInt32((Session["pId"].ToString() ?? "0")), CurrentTenantId, CurrentUserId, FilePath);
                //System.IO.File.Delete(Server.MapPath(UploadDirectory + Session["pId"].ToString() + "/" + filename));
            }
            else
            {
                System.IO.File.Delete(Server.MapPath(UploadTempDirectory + "/" + filename));
            }

            var cfiles = files.Select(a => a.Key).ToList();
            return Json(new { files = cfiles.Count == 0 ? null : cfiles });
        }
        public ActionResult Download(string filename)
        {
            var path = Server.MapPath(Path.Combine(UploadDirectory, Session["pId"].ToString(), filename));

            if (System.IO.File.Exists(path))
            {
                byte[] fileBytes = System.IO.File.ReadAllBytes(path);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
            }
            else
            {
                return null;
            }
        }
        byte[] GetFile(string s)
        {
            System.IO.FileStream fs = System.IO.File.OpenRead(s);
            byte[] data = new byte[fs.Length];
            int br = fs.Read(data, 0, data.Length);
            if (br != fs.Length)
                throw new System.IO.IOException(s);
            return data;
        }
        public ActionResult _ProductImages(int? ProductId)
        {
            ViewBag.productid = ProductId;
            string[] imageFormats = ConfigurationManager.AppSettings["ImageFormats"].Split(new char[] { ',' });
            var path = _productServices.GetProductFiles(ProductId ?? 0, CurrentTenantId).ToList();
            if (path.Count > 0)
            {

                var data = from files in path.Where(a => imageFormats.Contains(new DirectoryInfo(a.FilePath).Extension, StringComparer.CurrentCultureIgnoreCase))
                           select new
                           {
                               FileUrl = files.FilePath

                           };
                return PartialView(data);
            }
            return PartialView();
        }
        public ActionResult _ProductVideos(int? ProductId)
        {
            ViewBag.productid = ProductId;
            string[] videoFormats = ConfigurationManager.AppSettings["VideoFormats"].Split(new char[] { ',' });
            var path = _productServices.GetProductFiles(ProductId ?? 0, CurrentTenantId).ToList();
            if (path.Count > 0)
            {

                var data = from files in path.Where(a => videoFormats.Contains(new DirectoryInfo(a.FilePath).Extension, StringComparer.CurrentCultureIgnoreCase))
                           select new
                           {
                               FileUrl = files.FilePath

                           };
                return PartialView(data);
            }
            return PartialView();
        }
        public ActionResult _ProductDocuments(int? ProductId)
        {
            ViewBag.productid = ProductId;
            string[] docFormats = ConfigurationManager.AppSettings["DocumentFormats"].Split(new char[] { ',' });
            var path = _productServices.GetProductFiles(ProductId ?? 0, CurrentTenantId).ToList();
            if (path.Count > 0)
            {

                var data = from files in path.Where(a => docFormats.Contains(new DirectoryInfo(a.FilePath).Extension, StringComparer.CurrentCultureIgnoreCase))
                           select new
                           {
                               FileUrl = files.FilePath.Remove(0, 1)

                           };
                return PartialView(data);
            }
            return PartialView();
        }
        public ActionResult _ProductDocumentsDetails(int? ProductId)
        {
            ViewBag.productid = ProductId;
            var productFiles = _productServices.GetProductFiles(ProductId ?? 0, CurrentTenantId).ToList();
            return PartialView("_ProductDocumentsDetails", productFiles);

        }

        public ActionResult ProductFilesDetail(int? ProductId)
        {
            ViewBag.productid = ProductId;
            return View();
        }

        public ActionResult SaveProductFiles(MVCxGridViewBatchUpdateValues<ProductFiles, int> updateValues)
        {
            var file = updateValues.Update.FirstOrDefault();
            var productFiles = _productServices.GetProductFilesById(file.Id);
            if (productFiles != null)
            {
                productFiles.SortOrder = file.SortOrder;
                productFiles.DefaultImage = file.DefaultImage;
                productFiles.HoverImage = file.HoverImage;
                productFiles.IsDeleted = file.IsDeleted;
            }
            int productId = _productServices.EditProductFile(productFiles, CurrentTenantId, CurrentUserId);

            return _ProductDocumentsDetails(productId);
        }

        public ActionResult _ProductFilesContainer(int? ProductId)
        {

            ViewBag.productid = ProductId;
            return PartialView();
        }
        public ActionResult _SCCCodes(int? productId)
        {
            ViewBag.productId = productId;

            var data = _productLookupService.GetAllProductSccCodesByProductId(productId.Value, CurrentTenantId).
                        Select(codes => new
                        {
                            codes.ProductSCCCodeId,
                            codes.SCC,
                            codes.Quantity
                        }).ToList();
            return PartialView(data);


        }
        public ActionResult _AccountCodes(int? productId)
        {
            ViewBag.productId = productId;
            var data = _productServices.GetAllProductAccountCodesByProductId(productId.Value).Select(
                accode => new
                {
                    accode.ProdAccCodeID,
                    accode.Account.AccountCode,
                    accode.ProdAccCode,
                    accode.Account.CompanyName
                }).ToList();
            return PartialView(data);
        }

        public ActionResult _Categories(int productId)
        {
            ViewBag.productId = productId;
            return PartialView(null);
        }
        public ActionResult _Groups(int productId)
        {
            return PartialView(null);
        }
        public ActionResult _ProductRecipeItems(int productId, bool? selectionMode)
        {
            var recipeGridname = productId + "productRecipeItems" + (selectionMode == true ? "-select" : "");
            ViewBag.settingsName = recipeGridname;

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductRecipeItems", productId, selectionMode };

            if (selectionMode == true)
            {
                ViewBag.SelectionMode = true;
            }

            var product = GetKitProductModelById(productId, ProductKitTypeEnum.Recipe);

            return PartialView(product.AllSelectedSubItems);
        }
        public ActionResult _ProductKitItems(int productId, ProductKitTypeEnum productKitType)
        {
            var recipeGridname = productId + "productKitItems" + (productKitType);
            ViewBag.settingsName = recipeGridname;
            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductKitItems", productId, productKitType };
            var product = GetKitProductModelById(productId, productKitType);
            return PartialView(product.AllSelectedSubItems);
        }
        public ActionResult _ProductGroupedItems(int productId, bool? selectionMode)
        {
            var recipeGridname = productId + "productgroupedItems" + (selectionMode == true ? "-select" : "");
            ViewBag.settingsName = recipeGridname;

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductGroupedItems", productId, selectionMode };

            if (selectionMode == true)
            {
                ViewBag.SelectionMode = true;
            }

            var product = GetKitProductModelById(productId, ProductKitTypeEnum.Grouped);

            return PartialView(product.AllSelectedSubItems);
        }

        public ActionResult _ProductRecipeSelectedItems(int productId)
        {
            ViewBag.settingsName = productId.ToString() + "productRecipeSelectedItems";

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductRecipeSelectedItems", productId };

            var productList = (List<RecipeProductItemRequest>)Session["ProductRecipeModelItems"];

            return PartialView(productList);
        }

        public ActionResult _ProductKitSelectedItems(int productId)
        {
            ViewBag.settingsName = productId.ToString() + "productKitSelectedItems";

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductKitSelectedItems", productId };

            var productList = (List<RecipeProductItemRequest>)Session["ProductKitModelItems"];

            return PartialView(productList);
        }

        public ActionResult _ProductRecipeItemsEditor(int productId)
        {

            ViewBag.settingsName = productId.ToString() + "productRecipeEditorItems";

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductRecipeItemsEditor", productId };

            ViewBag.SelectionMode = true;

            var productModel = GetRecipeProductModelById(productId);

            var recipeItems = productModel.AllSelectedSubItems.Select(m => new RecipeProductItemRequest() { ProductId = m.ProductId, ProductName = m.Name, ParentProductId = productId, Quantity = m.Quantity }).ToList();
            Session["ProductRecipeModelItems"] = recipeItems;

            return PartialView("_ProductRecipeItemsEditor", productModel);
        }
        public ActionResult _ProductKitItemsEditor(int productId)
        {

            ViewBag.settingsName = productId.ToString() + "productKitEditorItems";

            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductKitItemsEditor", productId };

            ViewBag.SelectionMode = true;

            var productModel = GetKitProductModelById(productId, ProductKitTypeEnum.Recipe);

            var items = productModel.AllSelectedSubItems.Select(m => new RecipeProductItemRequest() { ProductId = m.ProductId, ProductName = m.Name, ParentProductId = productId, Quantity = m.Quantity }).ToList();
            Session["ProductKitModelItems"] = items;

            return PartialView(productModel);
        }

        private ProductMasterViewModel GetRecipeProductModelById(int productId)
        {
            var product = _productServices.GetProductMasterById(productId);
            var productModel = new ProductMasterViewModel();
            productModel.AllSelectedSubItems = product.ProductKitMap.Where(m => m.IsDeleted != true && m.ProductKitType == ProductKitTypeEnum.Recipe).
                Select(r => new ProductRecipeItemViewModel
                {
                    Name = r.KitProductMaster.Name,
                    ProductId = r.KitProductId,
                    SKUCode = r.KitProductMaster.SKUCode,
                    BarCode = r.KitProductMaster.BarCode,
                    Quantity = r.Quantity,
                    ParentProductId = productId
                }).ToList();
            return productModel;
        }

        private ProductMasterViewModel GetKitProductModelById(int productId, ProductKitTypeEnum productKitType)
        {
            var product = _productServices.GetAllProductKitMapsByProductId(productId, productKitType).ToList();
            var productModel = new ProductMasterViewModel();
            productModel.AllSelectedSubItems = product.
                Select(r => new ProductRecipeItemViewModel
                {
                    Name = r.KitProductMaster.Name,
                    ProductId = r.KitProductId,
                    SKUCode = r.KitProductMaster.SKUCode,
                    BarCode = r.KitProductMaster.BarCode,
                    Quantity = r.Quantity,
                    ParentProductId = productId,
                    ProductKitId = r.Id,
                    ProductKitType = r.ProductKitTypes == null ? "" : r.ProductKitTypes.Name,
                    ProductKitTypeEnum = r.ProductKitType
                }).ToList();




            return productModel;
        }
        public JsonResult AddProductRecipeItem(RecipeProductItemRequest model)
        {
            if (Session["ProductRecipeModelItems"] == null)
            {
                var recipeItems = new List<RecipeProductItemRequest> { model };
                Session["ProductRecipeModelItems"] = recipeItems;
            }
            else
            {
                var productList = (List<RecipeProductItemRequest>)Session["ProductRecipeModelItems"];
                var existingProduct = productList.FirstOrDefault(m => m.ProductId == model.ProductId);
                if (existingProduct == null)
                {
                    productList.Add(model);
                }
                else
                {
                    var index = productList.IndexOf(existingProduct);
                    productList[index].Quantity += model.Quantity;
                }
                Session["ProductRecipeModelItems"] = productList;
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditSelectedRecipetem(RecipeProductItemRequest request)
        {
            var recipeItems = (List<RecipeProductItemRequest>)Session["ProductRecipeModelItems"];
            if (recipeItems != null)
            {
                var kitItem = recipeItems.FirstOrDefault(m => m.ProductId == request.ProductId);
                if (kitItem != null)
                {
                    var index = recipeItems.IndexOf(kitItem);
                    recipeItems[index].Quantity = request.Quantity;
                }
            }
            Session["ProductRecipeModelItems"] = recipeItems;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSelectedRecipeItem(int Id)
        {
            var recipeItems = (List<RecipeProductItemRequest>)Session["ProductRecipeModelItems"];
            if (recipeItems != null)
            {
                recipeItems.RemoveAll(m => m.ProductId == Id);
            }
            Session["ProductRecipeModelItems"] = recipeItems;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AddProductKitItem(RecipeProductItemRequest model)
        {
            if (Session["ProductKitModelItems"] == null)
            {
                var recipeItems = new List<RecipeProductItemRequest> { model };
                Session["ProductKitModelItems"] = recipeItems;
            }
            else
            {
                var productList = (List<RecipeProductItemRequest>)Session["ProductKitModelItems"];
                var existingProduct = productList.FirstOrDefault(m => m.ProductId == model.ProductId);
                if (existingProduct == null)
                {
                    productList.Add(model);
                }
                else
                {
                    var index = productList.IndexOf(existingProduct);
                    productList[index].Quantity += model.Quantity;
                }
                Session["ProductKitModelItems"] = productList;
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSelectedKitItem(int Id)
        {
            var kitItems = (List<RecipeProductItemRequest>)Session["ProductKitModelItems"];
            if (kitItems != null)
            {
                kitItems.RemoveAll(m => m.ProductId == Id);
            }
            Session["ProductKitModelItems"] = kitItems;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult EditSelectedKitItem(RecipeProductItemRequest request)
        {
            var kitItems = (List<RecipeProductItemRequest>)Session["ProductKitModelItems"];
            if (kitItems != null)
            {
                var kitItem = kitItems.FirstOrDefault(m => m.ProductId == request.ProductId);
                if (kitItem != null)
                {
                    var index = kitItems.IndexOf(kitItem);
                    kitItems[index].Quantity = request.Quantity;
                }
            }
            Session["ProductKitModelItems"] = kitItems;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult RemoveRecipeItemProduct(RemoveRecipeItemRequest request)
        {
            _productServices.RemoveRecipeItemProduct(request.Id, CurrentUserId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateRecipeItemProduct(RecipeProductItemRequest request)
        {
            _productServices.UpdateRecipeItemProduct(request.ParentProductId, request.ProductId, request.Quantity, CurrentUserId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult UpdateKitItemProduct(RecipeProductItemRequest request)
        {
            _productServices.UpdateKitItemProduct(request.ParentProductId, request.ProductId, request.Quantity, CurrentUserId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RemoveKitItemProduct(RemoveRecipeItemRequest request)
        {
            _productServices.RemoveKitItemProduct(request.Id, request.RecipeProductId, CurrentUserId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmAddedRecipeProducts(int Id)
        {
            var recipeItems = (List<RecipeProductItemRequest>)Session["ProductRecipeModelItems"];
            if (recipeItems != null)
            {
                _productServices.SaveSelectedProductRecipeItems(Id, recipeItems, CurrentUserId);
            }

            Session["ProductRecipeModelItems"] = null;
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConfirmAddedKitProducts(RecipeProductItemRequest recipeProductItem)
        {
            _productServices.SaveSelectedProductKitItems(recipeProductItem.ParentProductId, recipeProductItem, CurrentUserId, CurrentTenantId);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _ProductLocations(int productId)
        {
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;

            ViewBag.productId = productId;
            ViewBag.settingsName = productId.ToString() + "productLocations";
            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductLocations", productId = ViewBag.productId };
            var data = _productLookupService.GetAllProductLocationsByProductId(productId, warehouseId).
                Select(plocation => new
                {
                    ProductName = plocation.ProductMaster.Name,
                    plocation.LocationId,
                    plocation.Locations.LocationType?.LocTypeName,
                    plocation.Locations.LocationGroup?.Locdescription,
                    plocation.Locations.LocationCode,
                    plocation.Locations.TenantWarehouses.WarehouseName
                }).ToList();
            return PartialView(data);
        }


        public ActionResult _InventoryStock(int productId)
        {
            ViewBag.productId = productId;
            var model = _productServices.GetAllInventoryStocksByProductId(productId)
            .Select(istk => new
            {
                istk.InventoryStockId,
                istk.InStock,
                istk.Allocated,
                istk.Available,
                istk.TenantWarehous.WarehouseName
            });
            return PartialView(model);
        }
        public ActionResult _InventoryTransactions(int productId)
        {
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;

            ViewBag.productId = productId;

            var data = _productServices.GetAllInventoryTransactionsByProductId(productId, warehouseId).
                Select(intrans => new
                {
                    intrans.InventoryTransactionId,
                    intrans.TenantWarehouse.WarehouseName,
                    LocationName = intrans.Location != null ? intrans.Location.LocationName : "",
                    InventoryTransactionTypeName = intrans.InventoryTransactionTypeId,
                    intrans.Quantity,
                    intrans.DateCreated,
                    intrans.InventoryTransactionRef
                }).ToList();
            return PartialView(data);
        }
        public ActionResult _StockTakeSnapshot(int productId)
        {
            ViewBag.productId = productId;
            var data = _productServices.GetAllStockTakeSnapshotsByProductId(productId);
            return PartialView(data);
        }
        public ActionResult _Serializations(int productId)
        {
            int warehouseId = caCurrent.CurrentWarehouse().WarehouseId;

            ViewBag.productId = productId;

            var data = _productServices.GetAllProductSerialsByProductId(productId, warehouseId).Select(ser => new
            {
                ser.SerialID,
                ser.SerialNo,
                ser.CurrentStatus,
                ser.Batch,
                ser.BuyPrice,
                WarrantyStartDate = ser.SoldWarrantyStartDate,
                ser.ExpiryDate,
                LocationName = ser.Location != null ? ser.Location.LocationName : "",
            }).ToList();

            return PartialView(data);
        }


        public ActionResult SaveSCCCode(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(Id);
        }
        [HttpPost]
        public ActionResult SaveSCCCode(ProductSCCCodes model)
        {
            var productId = (int)Session["pId"];
            _productLookupService.SaveSccCode(model, productId, CurrentUserId, CurrentTenantId);
            return Redirect(Url.Action("Edit", new { id = productId }) + "#product-codes");
        }

        public ActionResult _SCCCodeSave(int? Id)
        {
            if (Id == null)
                return PartialView("_SCCCode");
            else
            {
                var current = _productLookupService.GetProductSccCodesById(Id.Value);
                return PartialView("_SCCCode", current);
            }
        }
        public ActionResult DeleteSCCCode(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current = _productLookupService.GetProductSccCodesById(Id.Value);
            if (current == null)
                return HttpNotFound();

            return View(current);
        }
        [HttpPost, ActionName("DeleteSCCCode")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSCCCodeConfirmed(int id)
        {
            _productLookupService.DeleteSccCode(id, CurrentUserId);
            return Redirect(Url.Action("Edit", new { id = Session["pId"] }) + "#product-codes");
        }
        public ActionResult _Attributes(int ProductId)
        {
            ViewBag.productId = ProductId;
            var data = _productLookupService.GetAllValidProductAttributeValuesMapsByProductId(ProductId).Select(attr => new
            {
                attr.Id,
                attr.ProductAttributeValues.AttributeValueId,
                attr.ProductAttributeValues.ProductAttributes.AttributeName,
                attr.ProductAttributeValues.Value
            }).ToList();
            return PartialView(data);
        }
        [HttpPost]
        public ActionResult SaveProductAttributeValueMap(ProductAttributeValues model, int? productAttributeValueMapId)
        {
            var productId = (int)Session["pId"];

            _productLookupService.SaveProductAttributeValueMap(model.AttributeValueId, CurrentUserId, CurrentTenantId, productId, productAttributeValueMapId);

            return Redirect(Url.Action("Edit", new { id = productId }) + "#product-attributes");

        }
        public ActionResult SaveProductAttributeValueMap(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(Id);
        }

        public ActionResult _ProductAttributesSelection(int id, int priceGroupId)
        {
            ViewBag.routeValues = new { Controller = "Products", Action = "_ProductAttributesSelection", id, priceGroupId };
            var product = _productServices.GetProductMasterById(id);
            LoadProductAttributesToView(product, priceGroupId);
            return PartialView("_ProductAttributesSelection", product);
        }

        public ActionResult DeleteAttributeValue(int? Id)
        {
            return View(_productLookupService.GetProductAttributeValuesMapById(Id.Value).ProductAttributeValues);
        }
        [HttpPost, ActionName("DeleteAttributeValue")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAttributeValueConfirmed(int attributeValueId)
        {
            var cpId = (int)Session["pId"];
            _productLookupService.DeleteProductAttributeValuesMap(cpId, attributeValueId, CurrentUserId, CurrentTenantId);

            return Redirect(Url.Action("Edit", new { id = Session["pId"] }) + "#product-attributes");
        }
        public ActionResult SaveCategory(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(Id);
        }
        [HttpPost]
        public ActionResult SaveCategory(TenantDepartments model)
        {
            try
            {
                return RedirectToAction("Edit", new { id = Session["pId"] });
            }
            catch (Exception exp)
            {
                ModelState.AddModelError("", exp.Message);
                return View(0);
            }
        }
        public ActionResult DeleteProductCategory(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current = _lookupServices.GetTenantDepartmentById(Id.Value);
            if (current == null)
                return HttpNotFound();
            return View(current);
        }
        [HttpPost, ActionName("DeleteProductCategory")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductCategoryConfirmed(int id)
        {
            return RedirectToAction("Edit", new { id = Session["pId"] });
        }
        public ActionResult SaveProductGroup(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View(Id);
        }
        [HttpPost]
        public ActionResult SaveProductGroup(ProductGroups model)
        {
            return RedirectToAction("Edit", new { id = Session["pId"] });

        }
        public ActionResult DeleteProductGroup(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current = _productLookupService.GetProductGroupById(Id.Value);
            if (current == null)
                return HttpNotFound();
            return View(current);
        }
        [HttpPost, ActionName("DeleteProductGroup")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductGroupConfirmed(int? Id)
        {
            return RedirectToAction("Edit", new { id = Session["pId"] });
        }
        public ActionResult SaveLocation(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (Id != null && Id > 0)
            {
                ViewBag.Title = "Edit Location";
            }
            else
            {
                ViewBag.Title = "Create Location";
            }

            return View(Id);
        }
        [HttpPost]
        public ActionResult SaveLocation(Locations model)
        {
            try
            {
                _productLookupService.SaveProductLocation(model, CurrentWarehouseId, CurrentTenantId, CurrentUserId, (int)Session["pId"]);
                return Redirect(Url.Action("Edit", new { id = Session["pId"] }) + "#product-locations");
            }
            catch (Exception exp)
            {
                ModelState.AddModelError("", exp.Message);
                return View();
            }
        }
        public ActionResult DeleteLocation(int? Id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current = _productLookupService.GetLocationById(Id.Value);
            if (current == null)
                return HttpNotFound();
            return View(current);

        }
        [HttpPost, ActionName("DeleteLocation")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLocationConfirmed(int Id)
        {
            int pId = (int)Session["pId"];
            _productLookupService.DeleteProductLocation(pId, Id, CurrentWarehouseId, CurrentTenantId, CurrentUserId);
            return Redirect(Url.Action("Edit", new { id = Session["pId"] }) + "#product-locations");
        }

        public ActionResult SaveProductAccount(int? Id)
        {

            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.Accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
            {
                accountid = acnts.AccountID,
                account = acnts.AccountCode
            }).ToList();

            if (Id == null) return View(new ProductAccountCodes { });
            else
                return View(_accountServices.GetProductAccountCodesById(Id.Value));
        }

        [HttpPost]
        public ActionResult SaveProductAccount(ProductAccountCodes model)
        {

            var productId = (int)Session["pId"];
            var accountCode = _accountServices.SaveProductAccount(model, productId, CurrentTenantId, CurrentUserId);
            if (accountCode == null)
            {
                ModelState.AddModelError("", "Account code for this Account already exists");
                ViewBag.Accounts = _accountServices.GetAllValidAccounts(CurrentTenantId).Select(acnts => new
                {
                    accountid = acnts.AccountID,
                    account = acnts.AccountCode + " - " + acnts.CompanyName
                }).ToList();
                return View(model);
            }

            return Redirect(Url.Action("Edit", new { id = productId }) + "#product-codes");
        }

        public ActionResult DeleteProductAccount(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var current = _accountServices.GetProductAccountCodesById(Id.Value);
            if (current == null)
                return HttpNotFound();

            return View(current);
        }

        [HttpPost, ActionName("DeleteProductAccount")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteProductAccountConfirmed(int? Id)
        {
            _accountServices.DeleteProductAccount(Id.Value, CurrentUserId);
            return Redirect(Url.Action("Edit", new { id = Session["pId"] }) + "#product-codes");
        }

        #endregion
        public ActionResult _ProductKitCombobox(int? ProductId)
        {
            ViewBag.ProductgroupIds = string.Join(",", _productServices.GetAllProductKitMapsByProductId(ProductId ?? 0, ProductKitTypeEnum.Grouped).Select(a => a.KitProductId).Distinct().ToList());

            return PartialView();
        }

        public ActionResult _ProductLabelPrint(int productId, int orderDetailId)
        {

            var orderDetail = OrderService.GetOrderDetailsById(orderDetailId);

            var product = orderDetail?.ProductMaster ?? _productServices.GetProductMasterById(productId);

            var quantity = orderDetail?.Qty ?? 1;
            
            var model = new LabelPrintViewModel
            {
                ProductId = productId,
                LabelDate = DateTime.Today,
                OrderDetailId = orderDetailId,
                ProductName = product?.NameWithCode,
                ProductSkuCode = product?.SKUCode,
                OrderNumber = orderDetail?.Order?.OrderNumber,
                ProductBarcode = !string.IsNullOrEmpty(product?.BarCode?.Trim()) ? product?.BarCode?.Trim() : product?.BarCode2?.Trim(),
                Quantity = (int)quantity,
                Cases = product.ProductsPerCase != null ? (int)Math.Ceiling(quantity / product.ProductsPerCase.Value) : 1,
                RequiresBatchNumber = product.RequiresBatchNumberOnReceipt,
                RequiresExpiryDate = product.RequiresExpiryDateOnReceipt
            };

            if (product.ProcessByPallet && CurrentWarehouse.EnableGlobalProcessByPallet)
            {
                model.PalletsCount = product.CasesPerPallet != null ? (int)Math.Ceiling(Math.Ceiling(quantity / (product.ProductsPerCase ?? 1)) / product.CasesPerPallet.Value) : 1;
                model.Cases = product.CasesPerPallet ?? 1;

                return PartialView("_PalletLabelPrint", model);
            }
            else
            {
                return PartialView(model);
            }
        }

        public JsonResult PrintProductLabel(LabelPrintViewModel requestData)
        {
            var labelPrint = new ProductLabelPrint();
            var labels = CreatePalletLabels(requestData);
            labelPrint.DataSource = labels;
            labelPrint.CreateDocument();
            var tool = new PrintToolBase(labelPrint.PrintingSystem);
            tool.PrinterSettings.Copies = requestData.LabelsCount;
            tool.Print();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintProductLabelPreview(LabelPrintViewModel requestData)
        {
            var labelPrint = new ProductLabelPrint();
            var labels = CreateProductLabels(requestData);
            labelPrint.DataSource = labels;

            labelPrint.CreateDocument();

            return View("PalletLabelPrintViewer", labelPrint);
        }

        private IEnumerable<LabelPrintViewModel> CreateProductLabels(LabelPrintViewModel requestData)
        {
            var labels = new List<LabelPrintViewModel>();

            for (int i = 1; i <= requestData.Cases; i++)
            {
                var reportData = requestData.DeepClone();
                reportData.Cases = i;
                labels.Add(reportData);
            }

            return labels;
        }


        public JsonResult PrintPalletLabel(LabelPrintViewModel requestData)
        {
            var labels = CreatePalletLabels(requestData);
            var labelPrint = new PalletLabelPrint();
            labelPrint.DataSource = labels;
            labelPrint.CreateDocument();

            var tool = new PrintToolBase(labelPrint.PrintingSystem);
            tool.PrinterSettings.Copies = requestData.LabelsCount;
            tool.Print();

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PrintPalletLabelPreview(LabelPrintViewModel requestData)
        {
            var labels = CreatePalletLabels(requestData);
            var labelPrint = new PalletLabelPrint();
            labelPrint.DataSource = labels;

            labelPrint.CreateDocument();

            return View("PalletLabelPrintViewer", labelPrint);
        }

        private IEnumerable<LabelPrintViewModel> CreatePalletLabels(LabelPrintViewModel requestData)
        {
            var palletSerials = _productServices.CreatePalletTracking(requestData, CurrentTenantId, CurrentWarehouseId);

            var labels = palletSerials.Select(palletSerial =>
            {
                var reportData = requestData.DeepClone();
                reportData.Cases = requestData.Cases;
                reportData.PalletSerial = palletSerial;

                return reportData;
            });

            return labels;
        }

        protected override void Initialize(RequestContext requestContext)
        {
            var binder = (DevExpressEditorsBinder)ModelBinders.Binders.DefaultBinder;
            var actionName = (string)requestContext.RouteData.Values["Action"];
            switch (actionName)
            {
                case "UploadFile":
                    binder.UploadControlBinderSettings.FileUploadCompleteHandler = (s, e) =>
                    {
                        e.CallbackData = e.UploadedFile.FileName;
                    };
                    break;
            }
            base.Initialize(requestContext);
        }

    }
}

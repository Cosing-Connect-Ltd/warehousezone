using System;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using WMS.CustomBindings;

namespace WMS.Controllers
{
    public class ProductSpecialPriceController : BaseController
    {
        private readonly IProductPriceService _productPriceService;
        private readonly IProductServices _productService;
        private readonly ITenantsServices _tenantsServices;
        private readonly IMapper _mapper;

        public ProductSpecialPriceController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductPriceService productPriceService,
            IProductServices productService, ITenantsServices tenantsServices, IMapper mapper) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productPriceService = productPriceService;
            _productService = productService;
            _tenantsServices = tenantsServices;
            _mapper = mapper;
        }

        public ActionResult Index(int? id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            if (id.HasValue)
            {
                ViewBag.SelectedAccountID = id ?? 0;
            }
            return View();
        }

        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var priceGroup = LookupServices.GetAllPriceGroups(CurrentTenantId, id).FirstOrDefault();

            if (priceGroup == null) return HttpNotFound();

            ViewBag.PriceGroupName = priceGroup.Name;
            ViewBag.PriceGroupId = priceGroup.PriceGroupID;

            return View();
        }

        public ActionResult _ProductSpecialPriceGroups()
        {
            var priceGroups = LookupServices.GetAllPriceGroups(CurrentTenantId).ToList();

            var model = priceGroups.Select(x => _mapper.Map<TenantPriceGroups, TenantPriceGroupViewModel>(x)).ToList();

            return PartialView("_ProductGroupGridPartial", model);
        }

        public ActionResult _GroupSpecialPrices(int id)
        {
            var model = _productPriceService.GetAllSpecialProductPrices(CurrentTenantId, id);
            ViewBag.PriceGroupId = id;
            return PartialView("_ProductGroupPricesPartial", model);
        }

        public ActionResult SaveProductSpecialPrice(int PriceGroupId, MVCxGridViewBatchUpdateValues<ProductSpecialPriceViewModel, int> updateValues)
        {
            var value = updateValues.Update.FirstOrDefault();
            _productPriceService.SaveSpecialProductPrice(value.ProductID, value.SpecialPrice ?? 0, PriceGroupId, value.StartDate, value.EndDate, CurrentTenantId, CurrentUserId);
            return _GroupSpecialPrices(PriceGroupId);
        }

        public ActionResult DeleteProductSpecialPrice(int? id = null)
        {
            var result = _productPriceService.DeleteSpecialProductPriceById(id ?? 0, CurrentUserId);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult _PriceGroupForm(int? id = null)
        {
            ViewData["PriceGroupID"] = id;
            var model = _mapper.Map<TenantPriceGroups, PriceGroupViewModel>(_productPriceService.GetTenantPriceGroupById(id ?? 0));
            if (model == null)
            {
                model = new PriceGroupViewModel() { PriceGroupId = id ?? 0 };
            }
            return PartialView("_CreateEditPriceGroup", model);
        }

        public ActionResult SavePriceGroup(PriceGroupViewModel model)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var result = _productPriceService.SavePriceGroup(model.PriceGroupId, model.Name, model.Percent ?? 0, CurrentTenantId, CurrentUserId, model.ApplyDiscountOnTotal, model.ApplyDiscountOnSpecialPrice);

            return Json(new { Success = result != null, Message = result != null ? "Success" : "Please make sure the price group name is unique." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletePriceGroup(int? id = null)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }

            var result = _productPriceService.DeleteProductGroupById(id ?? 0, CurrentUserId);

            return Json(new { Success = result, Message = result ? "Success" : "Please make sure the price group is not used by any accounts." }, JsonRequestBehavior.AllowGet);
        }


    }
}
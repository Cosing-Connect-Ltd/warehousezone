using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductAttributeValuesController : BaseController
    {

        private readonly IProductLookupService _productLookupService;

        public ProductAttributeValuesController(ICoreOrderService orderService, IMarketServices marketServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IUserService userService, IInvoiceService invoiceService, IProductLookupService productLookupService)
            : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productLookupService = productLookupService;

        }
        // GET: ProductAttributeValues
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        public PartialViewResult _productAttributeValues()
        {
            var model = _productLookupService.GetAllValidProductAttributeValues().ToList();
            return PartialView(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            ViewBag.PoductAttributes = new SelectList(_productLookupService.GetAllValidProductAttributes(),
                "AttributeId", "AttributeName");
            return View();
        }
        [HttpPost]
        public ActionResult Create(ProductAttributeValues productAttributeValues)
        {
            _productLookupService.SaveProductAttributeValue(productAttributeValues.AttributeId,
                productAttributeValues.Value, productAttributeValues.SortOrder, productAttributeValues.Color,
                CurrentUserId);
            return View("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var attributeValueMap = _productLookupService.GetProductAttributeValueById(id);

            ViewBag.PoductAttributes = new SelectList(_productLookupService.GetAllValidProductAttributes(),
                "AttributeId", "AttributeName", attributeValueMap.AttributeValueId);


            return View(attributeValueMap);
        }
        [HttpPost]
        public ActionResult Edit(ProductAttributeValues productAttributeValues)
        {
            _productLookupService.SaveProductAttributeValue(productAttributeValues.AttributeId,
                productAttributeValues.Value, productAttributeValues.SortOrder, productAttributeValues.Color,
                CurrentUserId, productAttributeValues.AttributeValueId);
            return View("Index");
        }

        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = _productLookupService.RemoveProductAttriubteValueMap((id ?? 0), CurrentUserId);
            return RedirectToAction("Index");
        }
    }
}
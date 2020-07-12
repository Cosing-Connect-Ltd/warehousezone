using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.Controllers
{
    public class ProductAttributesController : BaseController
    {
        private readonly IProductServices _productServices;
        private readonly IProductLookupService _productLookupService;
        public ProductAttributesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices, IProductLookupService productLookupService) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
            _productLookupService = productLookupService;
        }
        public ActionResult Index()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }

        public PartialViewResult _ProductAttributes()
        {
            var model= _productServices.GetAllProductAttributes();
            return  PartialView(model);
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            return View();
        }
        [HttpPost]
        public ActionResult Create(ProductAttributes productAttributes)
        {
            _productLookupService.SaveProductAttribute(productAttributes.AttributeName, productAttributes.SortOrder,
                productAttributes.IsColorTyped);
            return View("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var model = _productLookupService.GetAllValidProductAttributes().FirstOrDefault(u => u.AttributeId == id);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(ProductAttributes productAttributes)
        {
            _productLookupService.SaveProductAttribute(productAttributes.AttributeName, productAttributes.SortOrder,
                productAttributes.IsColorTyped,productAttributes.AttributeId);
            return View("Index");

        }
        public ActionResult Delete(int? id)
        {
            if (!caSession.AuthoriseSession()) { return Redirect((string)Session["ErrorUrl"]); }
            var result = _productLookupService.RemoveProductAttriubte((id ?? 0));
            return RedirectToAction("Index");
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttribute(int ProductId = 0, int Attributes = 0, int Values = 0)
        {
            if (Values == 0 || ProductId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var map = _productServices.UpdateProductAttributeMap(ProductId, Values);

            if (map == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index", new { id = ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddAttributesVallist(int ProductId, int?[] Attr)
        {
            if (ProductId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var result = _productServices.UpdateProductAttributeMapCollection(ProductId, Attr);
            if (result == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index", new { id = ProductId });

        }

        public ActionResult DeleteAttribute(int AttributeValueId = 0, int ProductId = 0)
        {
            if (AttributeValueId == 0 || ProductId == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var map = _productServices.RemoveProductAttributeMap(ProductId, AttributeValueId);

            if (map == null)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index", new { id = ProductId });

        }

        public JsonResult JsongetValues(int id)
        {
            try
            {
                var pg = _productServices.GetAllProductAttributeValuesForAttribute(id)
                     .Select(r => new { AttributeId = r.AttributeValueId, Value = r.Value });

                return Json(pg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, isValid = false, isException = true }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult JsongetValuelist(int ProductId, int id, string q)
        {
            try
            {
                var result = _productServices.GetProductAttributesValues(ProductId, id, q);

                return Json(result.Select(m => new { AttributeValueId = m.Key, Value = m.Value }).ToList(), JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, isValid = false, isException = true }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
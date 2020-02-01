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

        public ProductAttributesController(ICoreOrderService orderService, IPropertyService propertyService, IAccountServices accountServices, ILookupServices lookupServices, IProductServices productServices) : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
        }
        public ActionResult Index(int id = 0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductMaster productmaster = _productServices.GetProductMasterById(id);
            if (productmaster == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = id;

            var attr = _productServices.GetAllProductAttributes();
            ViewBag.Attributes = new SelectList(attr, "AttributeId", "AttributeName");

            try
            {
                ViewBag.Values = new SelectList(attr.First().ProductAttributeValues, "AttributeValueId", "Value");
            }

            catch (Exception ex)
            {
                throw new Exception("Exception while getting Product attribute values" + ex.Message.ToString(), ex.InnerException);
            }
            return View(productmaster);
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
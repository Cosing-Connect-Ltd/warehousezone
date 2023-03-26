using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WMS.Controllers;

namespace WMS.Views
{
    public class ReportExcelController : BaseController
    {
        private readonly IProductServices _productServices;

        public ReportExcelController(ICoreOrderService orderService, ProductServices productServices, IPropertyService propertyService, IAccountServices accountServices, ILookupServices
            lookupServices)
           : base(orderService, propertyService, accountServices, lookupServices)
        {
            _productServices = productServices;
        }
        // GET: ReportExcel
        public ActionResult Index()
        {

            ViewBag.products = _productServices.GetAllValidProductMasters(CurrentTenantId).OrderBy(c => c.Name).Select(c => new { c.ProductId, c.Name }).ToList().Distinct();
            return View();
        }
        public PartialViewResult _stockValuePartial(int[] productId)
        {
            if (productId == null)
            {
                return PartialView(new List<ProductOrdersDetailViewModel>());
            }
            ViewBag.productIds = productId;
            var orders = OrderService.GetPurhaseOrderAgainstProductId(productId.ToArray(), CurrentTenantId, CurrentWarehouseId).OrderBy(c => c.SkuCode);
            return PartialView(orders);
        }
    }
}
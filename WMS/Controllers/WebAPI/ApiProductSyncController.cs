using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;

namespace WMS.Controllers.WebAPI
{
    public class ApiProductSyncController : BaseApiController
    {
        private readonly IMapper _mapper;

        public ApiProductSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _mapper = mapper;
        }
        // GET: api/HandheldUserSync
        // call example through URI http://localhost:8005/api/GetProducts?ReqDate=2014-11-23&SerialNo=920013c000814
        public IHttpActionResult GetProducts(DateTime reqDate, string serialNo)
        {
            serialNo = serialNo.Trim().ToLower();

            var terminal = TerminalServices.GetTerminalBySerial(serialNo);

            if (terminal == null)
            {
                return Unauthorized();
            }

            var result = new ProductMasterSyncCollection();

            var allProducts = ProductServices.GetAllValidProductMasters(terminal.TenantId, reqDate, true).ToList();
            var products = new List<ProductMasterSync>();

            foreach (var p in allProducts)
            {
                var product = new ProductMasterSync();
                var mappedProduct = _mapper.Map(p, product);
                mappedProduct.ProductGroupName = p.Name;
                mappedProduct.DepartmentName = p.TenantDepartment.DepartmentName;
                mappedProduct.TaxPercent = p.GlobalTax.PercentageOfAmount;
                products.Add(mappedProduct);
            }

            result.Count = products.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, products.Count(), terminal.TerminalId, TerminalLogTypeEnum.ProductsSync).TerminalLogId;
            result.Products = products;
            return Ok(result);
        }

    }
}
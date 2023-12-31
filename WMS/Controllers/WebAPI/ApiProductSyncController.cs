using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Http;
using AutoMapper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Web.Http.Cors;

namespace WMS.Controllers.WebAPI
{
    [EnableCorsAttribute("*", "*", "*")]
    public class ApiProductSyncController : BaseApiController
    {
        private readonly IMapper _mapper;

        public ApiProductSyncController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, IMapper mapper)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _mapper = mapper;
        }
        // GET: api/HandheldUserSync
        // call example through URI http://localhost:8005/api/sync/products/2014-11-23/WesWiCR4dostoBr7
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
                mappedProduct.ProductGroupName = p?.ProductGroup?.ProductGroup;
                mappedProduct.DepartmentName = p?.TenantDepartment?.DepartmentName;
                mappedProduct.TaxPercent = p.GlobalTax.PercentageOfAmount;
                mappedProduct.ProductKitMapViewModelList = _mapper.Map(p.ProductKitItems.Where(x => x.IsActive == true && x.IsDeleted != true).ToList(), mappedProduct.ProductKitMapViewModelList);
                mappedProduct.ProductTags = p.ProductTagMaps.Where(x => x.IsDeleted != true && x.ProductTag.IsDeleted != true).Select(x => x.ProductTag.TagName).ToList();

                var baseUrl = new Uri(ConfigurationManager.AppSettings["WarehouseStoreBaseUri"]);
                string[] imageFormats = ConfigurationManager.AppSettings["ImageFormats"].Split(new char[] { ',' });
                var filePath = p.ProductFiles.Where(a => imageFormats.Contains(new DirectoryInfo(a.FilePath).Extension, StringComparer.CurrentCultureIgnoreCase) && a.IsDeleted != true)
                    .OrderBy(a => a.SortOrder).FirstOrDefault()?.FilePath;
                mappedProduct.MainImage = filePath == null ? "" : baseUrl + filePath;
               
                products.Add(mappedProduct);
            }

            result.Count = products.Count;
            result.TerminalLogId = TerminalServices.CreateTerminalLog(reqDate, terminal.TenantId, products.Count(), terminal.TerminalId, TerminalLogTypeEnum.ProductsSync).TerminalLogId;
            result.Products = products;
            return Ok(result);
        }
        public IHttpActionResult GetProducts(int shopId)
        {
            ProductMasterSyncCollection masterSyncCollection = new ProductMasterSyncCollection();
            var  list = this.ProductServices.GetAllValidProductMasters(shopId).Where(u=>u.IsDeleted != true).ToList();
            List<ProductMasterSync> productMasterSyncList = new List<ProductMasterSync>();
            foreach (var source in list)
            {
                ProductMasterSync destination = new ProductMasterSync();
                ProductMasterSync productMasterSync = this._mapper.Map<ProductMaster, ProductMasterSync>(source, destination);
                productMasterSync.ProductGroupName = source?.ProductGroup?.ProductGroup;
                productMasterSync.DepartmentName = source?.TenantDepartment?.DepartmentName;
                productMasterSync.TaxPercent = source.GlobalTax.PercentageOfAmount;
                productMasterSync.SortOrder = source != null ? source.TenantDepartment.SortOrder : 0;
                Uri uri = new Uri(ConfigurationManager.AppSettings["WarehouseStoreBaseUri"]);
                ConfigurationManager.AppSettings["ImageFormats"].Split(',');
                productMasterSyncList.Add(productMasterSync);
            }
            masterSyncCollection.Count = productMasterSyncList.Count;
            masterSyncCollection.Products = productMasterSyncList;
            return (IHttpActionResult)this.Ok<ProductMasterSyncCollection>(masterSyncCollection);
        }


    }
}
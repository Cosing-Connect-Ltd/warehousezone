using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace WMS.Controllers
{
    public class TerminalStockEnquiryController : BaseApiController
    {
        public TerminalStockEnquiryController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {

        }

        // GET: http://localhost:16257/api/HandheldStockEnquiry?SkuCode=abc1234xyz&SerialNo=920013C000520
        [ResponseType(typeof(InventoryStock))]
        public IHttpActionResult GetInventoryStock(string SkuCode, string serialNo)
        {
            // trim the serial no and convert all into lower case
            serialNo = serialNo.Trim().ToLower();

            // check if this terminal exists in the system and get warehouse id
            Terminals terminal = TerminalServices.GetTerminalBySerial(serialNo);


            if (terminal == null)
            {
                return Unauthorized();
            }

            // get tenant for that warehouse id
            int tenantId = TenantLocationServices.GetTenantIdByTenantLocationId(terminal.WarehouseId);

            //get the product detail against SKU code
            ProductMaster Product = ProductServices.GetProductMasterByProductCode(SkuCode, tenantId);
            if (Product == null)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.NoContent);
                return ResponseMessage(response);
            }

            // get the stock level in strip down object to return to Web API
            StockEnquiry NewEnquiry = new StockEnquiry();

            // get the inventory stock level for this 

            InventoryStock InventoryStock = ProductServices.GetInventoryStocksByProductAndTenantLocation(Product.ProductId, terminal.WarehouseId);
            if (InventoryStock == null)
            {
                NewEnquiry.SkuCode = Product.SKUCode;
                NewEnquiry.ShortDesc = Product.Name;
                NewEnquiry.InStock = 0;
                NewEnquiry.Allocated = 0;
                NewEnquiry.Available = 0;
            }

            else
            {
                NewEnquiry.SkuCode = Product.SKUCode;
                NewEnquiry.ShortDesc = Product.Name;
                NewEnquiry.InStock = InventoryStock.InStock;
                NewEnquiry.Allocated = InventoryStock.Allocated;
                NewEnquiry.Available = InventoryStock.Available;
            }

            return Ok(NewEnquiry);

        }
    }
}
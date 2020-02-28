using Ganedata.Core.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace WMS.Controllers.WebAPI
{
    public class ApiDataImportController : ApiController
    {
        private DataImportFactory dataImportFactory = new DataImportFactory();

        public IHttpActionResult GetCipherLabProductDataImport(int tenantId)
        {
            try
            {
                dataImportFactory.ImportCipherLabProductData(tenantId);
               
                return Ok("All Cipher Lab product data has been imported");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not import data:  " + ex.Message);
            }
        }

        public IHttpActionResult GetScanSourceDataImport(int tenantId)
        {
            try
            {
                dataImportFactory.ImportScanSourceProductData(tenantId);

                return Ok("All Scan Source product data has been imported");
            }
            catch (Exception ex)
            {
                return BadRequest("Could not import data:  " + ex.Message);
            }
        }

        public IHttpActionResult GetDPDServices()
        {
            var value=dataImportFactory.GetDPDServices();
            return Ok(value);


        }
        
    }
}

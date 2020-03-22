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
            var res = dataImportFactory.ImportCipherLabProductData(tenantId);
            return Ok(res);
        }

        public IHttpActionResult GetScanSourceDataImport(int tenantId)
        {
            var res = dataImportFactory.ImportScanSourceProductData(tenantId);
            return Ok(res);
        }

        public IHttpActionResult GetDPDServices()
        {
            var value = dataImportFactory.GetDPDServices();
            return Ok(value);


        }

    }
}

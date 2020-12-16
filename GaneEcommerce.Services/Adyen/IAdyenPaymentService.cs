using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models.AdyenPayments;
using Newtonsoft.Json;

namespace Ganedata.Core.Services
{
    public interface IAdyenPaymentService
    {
        Task<AdyenCreatePayLinkResponseModel> GenerateOrderPaymentLink(AdyenCreatePayLinkRequestModel model);
    }

    public class AdyenPaymentService : IAdyenPaymentService
    {
        private readonly IApplicationContext _context;


        public static string AdyenUsername => ConfigurationManager.AppSettings["AdyenUsername"] != null ? ConfigurationManager.AppSettings["AdyenUsername"] : "ws@Company.GaneDatascanLtd";
        public static string AdyenClientKey => ConfigurationManager.AppSettings["AdyenClientKey"] != null ? ConfigurationManager.AppSettings["AdyenClientKey"] : "test_6DC3WWEE2VDVJOBWTBCJCAGP3UDX57BX";
        public static string AdyenApiKey => ConfigurationManager.AppSettings["AdyenApiKey"] != null ? ConfigurationManager.AppSettings["AdyenApiKey"] : "AQEqhmfuXNWTK0Qc+iSXk2o9g+WPSZhODJ1mTGE6yd/OgR82Wd+/SMYBi9rlEMFdWw2+5HzctViMSCJMYAc=-3oD+dvkhz0MpGi97nyZ0YCrHCX9aQUCT9RhbqVN6FQo=-gvU2Fa33)VCb5G(a";
        public static string AdyenPaylinkCreateEndpoint => ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] != null ? ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] : "https://checkout-test.adyen.com/v65/paymentLinks";
        public static string AdyenMerchantAccountName => ConfigurationManager.AppSettings["AdyenMerchantAccountName"] != null ? ConfigurationManager.AppSettings["AdyenMerchantAccountName"] : "GaneDatascanLtdECOM";

        public AdyenPaymentService(IApplicationContext context)
        {
            _context = context;
        }

        public async Task<AdyenCreatePayLinkResponseModel> GenerateOrderPaymentLink(AdyenCreatePayLinkRequestModel model)
        {
            using (var httpClient = new HttpClient())
            {
                var tokenRequestUri = new Uri(AdyenPaylinkCreateEndpoint);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Add("x-api-key", AdyenApiKey);
                var body = JsonConvert.SerializeObject(model);
                var response = await httpClient.PostAsync(tokenRequestUri, new StringContent(body, Encoding.UTF8, "application/json"));
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<AdyenCreatePayLinkResponseModel>(await response.Content.ReadAsStringAsync());
                }
            }

            return new AdyenCreatePayLinkResponseModel();
        }

    }
}
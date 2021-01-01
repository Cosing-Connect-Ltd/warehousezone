using System;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using Ganedata.Core.Models.AdyenPayments;
using Newtonsoft.Json;

namespace Ganedata.Core.Services
{
    public interface IAdyenPaymentService
    {
        Task<AdyenCreatePayLinkResponseModel> GenerateOrderPaymentLink(AdyenCreatePayLinkRequestModel model);
        Task<AdyenOrderPaylink> CreateOrderPaymentLink(AdyenCreatePayLinkResponseModel model, int orderId);
        Task<AdyenOrderPaylink> UpdateOrderPaymentAuthorisationHook(AdyenPaylinkHookNotificationRequest model);
        Task<AdyenApiPaymentStatusResponseModel> GetPaymentStatus(string linkId);
    }

    public class AdyenPaymentService : IAdyenPaymentService
    {
        private readonly IApplicationContext _context;


        public static string AdyenUsername => ConfigurationManager.AppSettings["AdyenUsername"] != null ? ConfigurationManager.AppSettings["AdyenUsername"] : "ws@Company.GaneDatascanLtd";
        public static string AdyenClientKey => ConfigurationManager.AppSettings["AdyenClientKey"] != null ? ConfigurationManager.AppSettings["AdyenClientKey"] : "test_6DC3WWEE2VDVJOBWTBCJCAGP3UDX57BX";
        public static string AdyenApiKey => ConfigurationManager.AppSettings["AdyenApiKey"] != null ? ConfigurationManager.AppSettings["AdyenApiKey"] : "AQEqhmfuXNWTK0Qc+iSXk2o9g+WPSZhODJ1mTGE6yd/OgR82Wd+/SMYBi9rlEMFdWw2+5HzctViMSCJMYAc=-3oD+dvkhz0MpGi97nyZ0YCrHCX9aQUCT9RhbqVN6FQo=-gvU2Fa33)VCb5G(a";
        public static string AdyenPaylinkCreateEndpoint => ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] != null ? ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] : "https://checkout-test.adyen.com/v66/paymentLinks";
        public static string AdyenMerchantAccountName => ConfigurationManager.AppSettings["AdyenMerchantAccountName"] != null ? ConfigurationManager.AppSettings["AdyenMerchantAccountName"] : "GaneDatascanLtdECOM";
        public static string AdyenHmacKey => ConfigurationManager.AppSettings["AdyenHmacKey"] != null ? ConfigurationManager.AppSettings["AdyenHmacKey"] : "BF43360B5EBA10D283279FE83257B8012798A690FFBFDCE78BDCAEEBA6BC6A8B";

        public AdyenPaymentService(IApplicationContext context)
        {
            _context = context;
        }

        public async Task<AdyenCreatePayLinkResponseModel> GenerateOrderPaymentLink(AdyenCreatePayLinkRequestModel model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenRequestUri = new Uri(AdyenPaylinkCreateEndpoint);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("x-api-key", AdyenApiKey);
                    var body = JsonConvert.SerializeObject(model);
                    var response = await httpClient.PostAsync(tokenRequestUri,
                        new StringContent(body, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<AdyenCreatePayLinkResponseModel>(
                            await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return new AdyenCreatePayLinkResponseModel()
                            {IsError = true, ErrorMessage = response.ReasonPhrase};
                    }
                }
            }
            catch (Exception ex)
            {
                return new AdyenCreatePayLinkResponseModel()
                    {IsError = true, ErrorMessage = ex.Message, ErrorMessageDetails = ex.StackTrace};
            }
        }
        public async Task<AdyenApiPaymentStatusResponseModel> GetPaymentStatus(string linkId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var tokenRequestUri = new Uri(AdyenPaylinkCreateEndpoint+"/"+ linkId);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("x-api-key", AdyenApiKey);
                    var response = await httpClient.GetAsync(tokenRequestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<AdyenApiPaymentStatusResponseModel>(
                            await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return new AdyenApiPaymentStatusResponseModel()
                            { IsError = true, ErrorMessage = response.ReasonPhrase };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AdyenApiPaymentStatusResponseModel()
                    { IsError = true, ErrorMessage = ex.Message, ErrorMessageDetails = ex.StackTrace };
            }
        }

        public async Task<AdyenOrderPaylink> CreateOrderPaymentLink(AdyenCreatePayLinkResponseModel model, int orderID)
        {
            var link = new AdyenOrderPaylink
            {
               LinkID = model.ID,
               LinkAmount = model.Amount.Value,
               LinkAmountCurrency = model.Amount.CurrencyCode,
               LinkExpiryDate = string.IsNullOrWhiteSpace(model.ExpiresAt)? (DateTime?)null: DateTime.Parse(model.ExpiresAt),
               LinkMerchantAccount = model.MerchantAccount,
               LinkOrderDescription = model.ShopperUniqueReference,
               LinkPaymentReference = model.PaymentReference,
               LinkRecurringProcessingModel = model.RecurringProcessingModel,
               LinkShopperReference = model.ShopperUniqueReference,
               LinkUrl = model.Url,
               LinkStorePaymentMethod = model.StorePaymentMethod,
               OrderID = orderID
            };

            _context.AdyenOrderPaylinks.Add(link);
            await _context.SaveChangesAsync();
            return link;
        }

        public async Task<AdyenOrderPaylink> UpdateOrderPaymentAuthorisationHook(AdyenPaylinkHookNotificationRequest model)
        {
            var link = await _context.AdyenOrderPaylinks.FirstAsync(m => m.LinkPaymentReference.Equals(model.MerchantReference));
            link.HookEventCode = model.EventCode;
            link.HookPspReference = model.PspReference;
            link.HookSuccess = model.Success;
            link.HookAmountCurrency = model.Amount.CurrencyCode;
            link.HookAmountPaid = model.Amount.Value;
            link.HookMerchantOrderReference = model.MerchantReference;
            link.HookCreatedDate = DateTime.Now;
            link.RawJson = model.RawJson;
            _context.Entry(link).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return link;
        }

    }
}
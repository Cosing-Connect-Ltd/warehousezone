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
        
        Order GetOrderByAdyenPaylinkID(string linkId);
        AdyenOrderPaylink GetAdyenPaylinkByOrderId(int orderId);

        Task<AdyenPaylinkRefundResponse> CreateRefundRequest(AdyenPaylinkRefundRequest model);
        Task<AdyenOrderPaylink> RequestRefundForPaymentLink(AdyenPaylinkRefundRequest model);
    }

    public class AdyenPaymentService : IAdyenPaymentService
    {
        private readonly IApplicationContext _context;


        public static string AdyenUsername => ConfigurationManager.AppSettings["AdyenUsername"] != null ? ConfigurationManager.AppSettings["AdyenUsername"] : "ws@Company.GaneDatascanLtd";
        public static string AdyenClientKey => ConfigurationManager.AppSettings["AdyenClientKey"] != null ? ConfigurationManager.AppSettings["AdyenClientKey"] : "test_6DC3WWEE2VDVJOBWTBCJCAGP3UDX57BX";
        public static string AdyenApiKey => ConfigurationManager.AppSettings["AdyenApiKey"] != null ? ConfigurationManager.AppSettings["AdyenApiKey"] : "AQEqhmfuXNWTK0Qc+iSXk2o9g+WPSZhODJ1mTGE6yd/OgR82Wd+/SMYBi9rlEMFdWw2+5HzctViMSCJMYAc=-3oD+dvkhz0MpGi97nyZ0YCrHCX9aQUCT9RhbqVN6FQo=-gvU2Fa33)VCb5G(a";
        public static string AdyenPaylinkCreateEndpoint => ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] != null ? ConfigurationManager.AppSettings["AdyenPaylinkCreateEndpoint"] : "https://checkout-test.adyen.com/v66/paymentLinks";
        public static string AdyenPaylinkRefundEndpoint => ConfigurationManager.AppSettings["AdyenPaylinkRefundEndpoint"] != null ? ConfigurationManager.AppSettings["AdyenPaylinkRefundEndpoint"] : "https://pal-test.adyen.com/pal/servlet/Payment/v64/refund";
        public static string AdyenMerchantAccountName => ConfigurationManager.AppSettings["AdyenMerchantAccountName"] != null ? ConfigurationManager.AppSettings["AdyenMerchantAccountName"] : "GaneDatascanLtdECOM";
        public static string AdyenHmacKey => ConfigurationManager.AppSettings["AdyenHmacKey"] != null ? ConfigurationManager.AppSettings["AdyenHmacKey"] : "95D0C2C9B2BA6EF5F3E50DAE742E35F4D98521706883364E520BA9E9F979C927";
        public static string AdyenRefundHmacKey => ConfigurationManager.AppSettings["AdyenRefundHmacKey"] != null ? ConfigurationManager.AppSettings["AdyenRefundHmacKey"] : "058672FDBCDC65BBC2695D8C2C5B1123CBCDD8013D6E2AAD2AFFEDA19591952C";

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
                    var amountInMinorUnits = model.Amount.Value * 100;
                    model.Amount = new AdyenAmount(){ CurrencyCode = model.Amount.CurrencyCode, Value = amountInMinorUnits };
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
            var link = _context.AdyenOrderPaylinks.FirstOrDefault(m => m.LinkPaymentReference.Equals(model.MerchantReference));

            if (model.EventCode.ToLower().Contains("refund"))
            {
                link = _context.AdyenOrderPaylinks.FirstOrDefault(m => m.HookPspReference.Equals(model.PspReference, StringComparison.InvariantCultureIgnoreCase));
                if (link != null)
                {
                    link.RefundHookCreatedDate = DateTime.Now;
                    link.RefundHookPspReference = model.PspReference;
                    link.RefundHookEventCode = model.EventCode;

                    if (model.Success)
                    {
                        link.RefundHookAmountCurrency = model.Amount.CurrencyCode;
                        link.RefundHookAmountPaid = model.Amount.Value;
                        link.RefundHookPspReference = model.PspReference;
                        link.RefundHookCreatedDate = DateTime.Now;
                        link.RefundProcessedDateTime = DateTime.Now;
                        link.RefundHookEventCode = model.EventCode;
                    }
                    _context.Entry(link).State = EntityState.Modified;
                }
            }
            else
            {
                if (link != null)
                {
                    link.HookEventCode = model.EventCode;
                    link.HookPspReference = model.PspReference;
                    link.HookSuccess = model.Success;
                    link.HookAmountCurrency = model.Amount.CurrencyCode;
                    link.HookAmountPaid = model.Amount.Value;
                    link.HookMerchantOrderReference = model.MerchantReference;
                    link.HookCreatedDate = DateTime.Now;
                    link.HookRawJson = model.RawJson;
                    _context.Entry(link).State = EntityState.Modified;
                }
            }

            await _context.SaveChangesAsync();
            return link;
        }

        public Order GetOrderByAdyenPaylinkID(string linkId)
        {
            var link = _context.AdyenOrderPaylinks.FirstOrDefault(m => m.LinkID.Equals(linkId));
            if (link != null)
            {
                return _context.Order.Include(m=> m.BillingAccountAddress).Include(m=> m.ShipmentAccountAddress).Include(m=>m.OrderDetails).FirstOrDefault(m => m.OrderID == link.OrderID);
            }
            
            return null;
        }

        public AdyenOrderPaylink GetAdyenPaylinkByOrderId(int orderId)
        {
            return _context.AdyenOrderPaylinks.OrderByDescending(m=> m.HookCreatedDate).FirstOrDefault(m => m.OrderID.Equals(orderId));
        }

        public async Task<AdyenPaylinkRefundResponse> CreateRefundRequest(AdyenPaylinkRefundRequest model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var amountInMinorUnits = model.Amount.Value * 100;
                    model.Amount = new AdyenAmount() { CurrencyCode = model.Amount.CurrencyCode, Value = amountInMinorUnits };
                    var refundUri = new Uri(AdyenPaylinkRefundEndpoint);
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Add("x-api-key", AdyenApiKey);
                    var body = JsonConvert.SerializeObject(model);
                    var response = await httpClient.PostAsync(refundUri, new StringContent(body, Encoding.UTF8, "application/json"));
                    if (response.IsSuccessStatusCode)
                    {
                        return JsonConvert.DeserializeObject<AdyenPaylinkRefundResponse>(await response.Content.ReadAsStringAsync());
                    }
                    else
                    {
                        return new AdyenPaylinkRefundResponse()
                            { IsError = true, ErrorMessage = response.ReasonPhrase };
                    }
                }
            }
            catch (Exception ex)
            {
                return new AdyenPaylinkRefundResponse()
                    { IsError = true, ErrorMessage = ex.Message, ErrorMessageDetails = ex.StackTrace };
            }
        }
        public async Task<AdyenOrderPaylink> RequestRefundForPaymentLink(AdyenPaylinkRefundRequest model)
        {
            var link = _context.AdyenOrderPaylinks.FirstOrDefault(m => m.OrderID == model.OrderID);
            model.PspReference = link.HookPspReference;

            link.RefundMerchantReference = model.RefundReference;
            link.RefundOriginalMerchantReference = model.PspReference;
            link.RefundRequestedUserID = model.RequestedUserID;
            link.RefundRequestedDateTime = DateTime.Now;
            link.RefundRequestedAmount = model.Amount.Value;
            link.RefundRequestedAmountCurrency = model.Amount.CurrencyCode;
            link.RefundNotes = model.RefundNotes;

            _context.Entry(link).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            var response = await CreateRefundRequest(model);
            
            link = _context.AdyenOrderPaylinks.FirstOrDefault(m => m.OrderID == model.OrderID);
            link.RefundResponseStatus = response.ResponseStatus;
            link.RefundResponsePspReference = response.PspReference;
            _context.Entry(link).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return link;
        }

        //TODO: Code to post the json to adyen
    }
}
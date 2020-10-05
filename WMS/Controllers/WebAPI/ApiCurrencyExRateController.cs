using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.Models;
using Ganedata.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace WMS.Controllers.WebAPI
{
    public class ApiCurrencyExRateController : BaseApiController
    {
        private readonly ITenantsCurrencyRateServices _tenantCurrencyRateServices;
        private readonly ITenantsServices _tenantServices;

        public ApiCurrencyExRateController(ITerminalServices terminalServices, ITenantLocationServices tenantLocationServices, IOrderService orderService, IProductServices productServices, IUserService userService, ITenantsCurrencyRateServices TenantCurrencyRateServices, ITenantsServices TenantServices)
            : base(terminalServices, tenantLocationServices, orderService, productServices, userService)
        {
            _tenantCurrencyRateServices = TenantCurrencyRateServices;
            _tenantServices = TenantServices;
        }

        // i.e. http://localhost:8005/api/sync/currency-ex-rates/1
        public async Task<IHttpActionResult> GetTenantCurrencyExRate(int tenantId)
        {
            try
            {
                if (tenantId == 0)
                {
                    return BadRequest("Tenant id cannot be 0");
                }

                var tenantBaseCurrency = _tenantCurrencyRateServices.GetTenantCurrencyById(tenantId);
                var model = _tenantCurrencyRateServices.GetTenantCurrencies(tenantId).ToList();
                CurrenciesRates currenciesRates = new CurrenciesRates();
                currenciesRates = await GetCurrencyExchangeRate(tenantBaseCurrency.CurrencyName, model);
                if (!currenciesRates.success)
                {
                    return NotFound();
                }
                else
                {
                    List<TenantCurrenciesExRates> tenantCurrenciesExRatesList = new List<TenantCurrenciesExRates>();
                    var tenant = _tenantServices.GetByClientId((int)tenantId);
                    foreach (var item in currenciesRates.quotes)
                    {
                        foreach (var itemModel in model)
                        {
                            string Key = item.Key;
                            Key = Key.Substring(3);
                            string currency1 = (itemModel.GlobalCurrency.CurrencyName).Substring(0, 3).ToUpper();

                            if (Key == currency1)
                            {
                                TenantCurrenciesExRates tenantCurrenciesExRates = new TenantCurrenciesExRates();
                                tenantCurrenciesExRates.TenantCurrencyID = itemModel.TenantCurrencyID;
                                tenantCurrenciesExRates.ActualRate = Convert.ToDecimal(item.Value);
                                tenantCurrenciesExRates.DiffFactor = Convert.ToDecimal(itemModel.DiffFactor);
                                tenantCurrenciesExRates.Rate = Convert.ToDecimal(tenantCurrenciesExRates.DiffFactor + tenantCurrenciesExRates.ActualRate);
                                if (Key == tenantBaseCurrency.CurrencyName)
                                {
                                    tenantCurrenciesExRates.Rate = Convert.ToDecimal(tenantCurrenciesExRates.ActualRate);
                                }

                                tenantCurrenciesExRates.DateUpdated = DateTime.UtcNow;
                                SaveTenantCurrencyRate(tenantCurrenciesExRates);
                            }
                        }
                    }

                    return Ok("Currency rate saved");
                }
            }
            catch (Exception Exp)
            {
                if (Exp.InnerException != null)
                {
                    _tenantCurrencyRateServices.LogAPI(Exp.InnerException.ToString(), HttpStatusCode.Ambiguous, typeof(ApiCurrencyExRateController));
                    Debug.WriteLine(Exp.InnerException);
                }

                return BadRequest();
            }
        }

        public async Task<CurrenciesRates> GetCurrencyExchangeRate(string BaseCurrency, List<TenantCurrencies> modellist)
        {
            HttpResponseMessage response = null;
            try
            {
                var apiToken = ConfigurationManager.AppSettings["CurrencyLayerApiKey"];
                var apiLink = ConfigurationManager.AppSettings["CurrencyLayerApiUrl"];

                int tenantid = 0;
                CurrenciesRates model = new CurrenciesRates();
                string apiUrl;
                if (modellist != null && modellist.Count > 0)
                {
                    List<string> namesList = new List<string>();
                    foreach (var ml in modellist)
                    {
                        tenantid = ml.Tenant.TenantId;
                        namesList.Add((ml.GlobalCurrency.CurrencyName).ToUpper().Substring(0, 3));
                    }
                    //base currency currently is GBP 
                    BaseCurrency = _tenantServices.GetAllTenants().Where(u => u.TenantId == tenantid).FirstOrDefault().Currency.CurrencyName;

                    //Api Key Provided by apilayer
                    apiUrl = apiLink + (BaseCurrency).ToUpper()
                                            + "&access_key=" + apiToken + "&currencies=" +
                                            string.Join(",", namesList);
                }
                else
                {
                    apiUrl = apiLink + (BaseCurrency).ToUpper() + "&access_key=" + apiToken;
                }
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    response = await client.GetAsync(new Uri(apiUrl));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        _tenantCurrencyRateServices.LogAPI(response.Content.ReadAsStringAsync().Result, HttpStatusCode.OK, typeof(ApiCurrencyExRateController));
                        model = JsonConvert.DeserializeObject<CurrenciesRates>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        _tenantCurrencyRateServices.LogAPI(response.Content.ReadAsStringAsync().Result, (HttpStatusCode)response.StatusCode, typeof(ApiCurrencyExRateController));
                    }
                }
                return model;
            }
            catch (Exception Exp)
            {
                if (Exp.InnerException != null)
                {
                    _tenantCurrencyRateServices.LogAPI(Exp.InnerException.ToString(), HttpStatusCode.Ambiguous, typeof(ApiCurrencyExRateController));
                    Debug.WriteLine(Exp.InnerException);
                }
                return null;
            }
        }
        public void SaveTenantCurrencyRate([Bind(Include = "ExchnageRateID,TenantCurrencyID,DiffFactor,ActualRate,Rate,DateUpdated,Tenant_TenantId")] TenantCurrenciesExRates tenantCurrenciesExRates)
        {
            if (ModelState.IsValid)
            {
                _tenantCurrencyRateServices.Insert(tenantCurrenciesExRates);
            }
        }
    }
}
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Ganedata.Core.Entities.Enums;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Ganedata.Core.Entities.Domain.Models;
using System.IO;

namespace Ganedata.Core.Services
{
    public class TenantsCurrencyRateServices : ITenantsCurrencyRateServices
    {

        private readonly IApplicationContext _currentDbContext;

        //constructor
        public TenantsCurrencyRateServices(IApplicationContext applicationContext)
        {
            _currentDbContext = applicationContext;
        }

        public GlobalCurrency GetTenantCurrencyById(int tenantId)
        {
            var model = _currentDbContext.Tenants.Where(t => t.TenantId == tenantId).FirstOrDefault();

            return _currentDbContext.GlobalCurrencies.Where(x => x.CurrencyID == model.CurrencyID)
                .FirstOrDefault();
        }
        public GlobalCurrency GetCurrencyNameById(int CurrencyID)
        {
           

            return _currentDbContext.GlobalCurrencies.Where(x => x.CurrencyID == CurrencyID)
                .FirstOrDefault();
        }
        public IEnumerable<TenantCurrencies> GetTenantCurrencies(int tenantId)
        {
            return _currentDbContext.TenantCurrencies.Where(x => x.TenantId == tenantId).ToList();
        }
        public IEnumerable<TenantCurrenciesExRates> GetTenantCurrenciesExRates(int tenantId)
        {
            return _currentDbContext.TenantCurrenciesExRates.Where(x => x.TenantCurrencies.TenantId == tenantId).ToList();
        }
        public int Insert(TenantCurrenciesExRates tenantCurrenciesExRates)
        {
            _currentDbContext.Entry(tenantCurrenciesExRates).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return tenantCurrenciesExRates.ExchnageRateID;
        }
        public decimal GetCurrencyRateByTenantid(int currencyTenantId)
        {
            return _currentDbContext.TenantCurrenciesExRates.OrderByDescending(u=>u.ExchnageRateID).FirstOrDefault(u => u.TenantCurrencyID == currencyTenantId).Rate;
        }


        public void LogAPI(string apiLog, HttpStatusCode httpStatusCode, Type type)
        {
            if(httpStatusCode == HttpStatusCode.OK)
            {
                var cr = JsonConvert.DeserializeObject<CurrenciesRates>(apiLog);
                // write in error log file using string builder and stream writer
                StringBuilder builder = new StringBuilder();
                if (cr.error == null)
                {
                    builder
                        .AppendLine("Date/Time: " + DateTime.UtcNow.ToString())
                        .AppendLine("StatusCode: " + HttpStatusCode.OK.ToString())
                        .AppendLine("Controller: " + type.ToString())
                        .AppendLine("Success: " + cr.success)
                        .AppendLine("Terms: " + cr.terms)
                        .AppendLine("Privacy: " + cr.privacy)
                        .AppendLine("TimeStamp: " + cr.timestamp)
                        .AppendLine("Source: " + cr.source)
                        .AppendLine("Quotes: " + string.Join(",", cr.quotes.Select(m => $"{m.Key}:{m.Value}")))
                        .AppendLine("-----------------------------------------------")
                        .Append(Environment.NewLine);
                }
                else
                {

                    builder
                        .AppendLine("Date/Time: " + DateTime.UtcNow.ToString())
                        .AppendLine("StatusCode: " + HttpStatusCode.OK.ToString())
                        .AppendLine("Controller: " + type.ToString())
                        .AppendLine("Success: " + cr.success)
                        .AppendLine("Terms: " + cr.terms)
                        .AppendLine("Privacy: " + cr.privacy)
                        .AppendLine("TimeStamp: " + cr.timestamp)
                        .AppendLine("Source: " + cr.source)
                        .AppendLine("ErrorCode: " + cr.error.code)
                        .AppendLine("ErrorInfo: " + cr.error.info)
                        .AppendLine("-----------------------------------------------")
                        .Append(Environment.NewLine);
                }
                string filePath = HttpContext.Current.Server.MapPath("~/Logs/CurrencyRate.log");

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(builder.ToString());
                    writer.Flush();
                }

            }
            else if(httpStatusCode == HttpStatusCode.Ambiguous)
            {

                // write in error log file using string builder and stream writer
                StringBuilder builder = new StringBuilder();
                builder
                    .AppendLine("Date/Time: " + DateTime.UtcNow.ToString())
                    .AppendLine("StatusCode: " + HttpStatusCode.Ambiguous.ToString())
                    .AppendLine("Controller: " + type.ToString())
                    .AppendLine("Error InnerException: " + apiLog)
                    .AppendLine("-----------------------------------------------")
                    .Append(Environment.NewLine);
                string filePath = HttpContext.Current.Server.MapPath("~/Logs/CurrencyRate.log");

                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.Write(builder.ToString());
                    writer.Flush();
                }
            }

        }
    }
}
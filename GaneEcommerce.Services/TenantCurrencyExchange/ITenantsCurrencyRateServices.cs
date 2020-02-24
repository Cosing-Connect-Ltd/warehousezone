using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface ITenantsCurrencyRateServices
    {
        GlobalCurrency GetTenantCurrencyById(int tenantId);
        GlobalCurrency GetCurrencyNameById(int CurrencyID);
        IEnumerable<TenantCurrencies> GetTenantCurrencies(int tenantId);
        IEnumerable<TenantCurrenciesExRates> GetTenantCurrenciesExRates(int tenantId);
        int Insert(TenantCurrenciesExRates tenantCurrenciesExRates);
        void LogAPI(string apiLog, HttpStatusCode httpStatusCode, Type type);

        decimal GetCurrencyRateByTenantid(int currencyTenantId);

    };
}

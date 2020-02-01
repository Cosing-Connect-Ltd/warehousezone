using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface IAccountServices
    {
        IEnumerable<Account> GetAllValidAccounts(int tenantId, EnumAccountType customerType = EnumAccountType.All, string searchString = null, DateTime? lastUpdated = null, bool includeIsDeleted = false);
        IEnumerable<ProductAccountCodes> GetAllValidProductAccountCodes(int productId, int? accountId = null);
        IEnumerable<ProductAccountCodes> GetAllProductAccountCodesByAccount(int accountId);

        Account GetAccountsById(int accountId);
        Account GetAccountsByCode(string accountCode, int tenantId);
        ProductAccountCodes GetProductAccountCodesById(int productAccountCodeId);

        ProductAccountCodes SaveProductAccount(ProductAccountCodes model, int productId, int tenantId, int userId);
        void DeleteProductAccount(int productAccountId, int userId);

        IEnumerable<AccountContacts> GetAllTopAccountContactsByTenantId(int tenantId);
        IEnumerable<AccountContacts> GetAllValidAccountContactsByAccountId(int accountId, int tenantId);
        IEnumerable<AccountAddresses> GetAllValidAccountAddressesByAccountId(int accountId);

        AccountAddresses SaveAccountAddress(AccountAddresses customeraddresses, int currentUserId);
        AccountAddresses GetAccountAddressById(int id);
        IEnumerable<AccountAddresses> GetAccountAddress();
        AccountAddresses DeleteAccountAddress(int addressId, int currentUserId);

        Account SaveAccount(Account model, List<int> accountAddressIds, List<int> accountContactIds,
            int globalCountryIds, int globalCurrencyIds, int accountStatusIds, int priceGroupId, int ownerUserId,
            List<AccountAddresses> addresses, List<AccountContacts> contacts, int userId, int tenantId, string stopReason = null);

        void DeleteAccount(int accountId, int userId);
        AccountContacts GetAccountContactById(int id);

        AccountContacts SaveAccountContact(AccountContacts model, int userId);

        AccountContacts DeleteAccountContact(int contactId, int currentUserId);

        AccountTransactionViewModel GetAccountTransactionById(int transactionId);

        List<SelectListItem> GetAllAccountPaymentModesSelectList();
        List<SelectListItem> GetAllAccountsSelectList(int tenantId);
        List<AccountStatusAuditViewModel> GetAccountAudits(int accountId);

        string GetLatestAuditComment(int accountId,int TenantId);


        IQueryable<Account> GetAllValidAccountbyList(List<int> accountId);
        AccountTransactionFile AddAccountTransactionFile(AccountTransactionFileSync file, int tenantId);

        IQueryable<AccountTransactionViewModel> GetTenantAccountTransactions(int tenantId, int accountId = 0);

        IEnumerable<AccountContacts> GetAllValidAccountContactsByAccountContactId(int accountId);
        IEnumerable<AccountContacts> GetAllValidAccountContactsByAccountContactIds(int?[] accountContactIds);
        IQueryable<Account> GetAllValidAccountsCustom(int tenantId, EnumAccountType customerType = EnumAccountType.All, string searchString = null, DateTime? lastUpdated = null);

        bool UpdateOrderPTenantEmailRecipients(int?[]accountContactId,int OrderId,int UserId);
    }
}

using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

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
        IEnumerable<AccountAddresses> GetAllValidAccountAddressesByAccountId(int accountId, DateTime? lastUpdated = null, bool includeDeleted = false);
        IEnumerable<AccountAddresses> GetAllValidAccountAddressesByAccountIdOrSessionKey(int accountId, string sessionId = null, DateTime? lastUpdated = null, bool includeDeleted = false);

        AccountAddresses SaveAccountAddress(AccountAddresses customeraddresses, int currentUserId);
        void SetAddressType(int addressId, bool isShippingType, bool isBillingType, int currentUserId);
        AccountAddresses GetAccountAddressById(int id);
        IEnumerable<AccountAddresses> GetAccountAddress();
        AccountAddresses DeleteAccountAddress(int addressId, int currentUserId);
        int CreateNewAccountForEcommerceUser(string accountCode, int currentUserId, int tenantId);
        Account SaveAccount(Account model, List<int> accountAddressIds, List<int> accountContactIds,
                            int globalCountryIds, int globalCurrencyIds, int priceGroupId, int? accountSectorId,
                            int ownerUserId, List<AccountAddresses> addresses, List<AccountContacts> contacts,
                            int userId, int tenantId, string stopReason = null, int[] MarketId = null);

        void DeleteAccount(int accountId, int userId);
        AccountContacts GetAccountContactById(int id);

        AccountContacts SaveAccountContact(AccountContacts model, int userId);

        AccountContacts DeleteAccountContact(int contactId, int currentUserId);

        AccountTransactionViewModel GetAccountTransactionById(int transactionId);

        string GetTransactionNumberByOrderId(int orderId);

        List<SelectListItem> GetAllAccountsSelectList(int tenantId);
        List<AccountStatusAuditViewModel> GetAccountAudits(int accountId);

        string GetLatestAuditComment(int accountId, int TenantId);


        IQueryable<Account> GetAllValidAccountbyList(List<int> accountId);
        AccountTransactionFile AddAccountTransactionFile(AccountTransactionFileSync file, int tenantId);

        IQueryable<AccountTransactionViewModel> GetTenantAccountTransactions(int tenantId, int accountId = 0);

        IEnumerable<AccountContacts> GetAllValidAccountContactsByAccountContactId(int accountId);
        IEnumerable<AccountContacts> GetAllValidAccountContactsByAccountContactIds(int?[] accountContactIds);
        IEnumerable<Account> GetAllValidAccountsByAccountIds(int[] accountIds);
        IQueryable<Account> GetAllValidAccountsCustom(int tenantId, EnumAccountType customerType = EnumAccountType.All, string searchString = null, DateTime? lastUpdated = null);

        bool UpdateOrderPTenantEmailRecipients(int?[] accountContactId, int OrderId, int UserId);
    }
}

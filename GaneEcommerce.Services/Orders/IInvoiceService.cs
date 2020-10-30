using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;
using Ganedata.Core.Entities.Enums;
using System.Linq;

namespace Ganedata.Core.Services
{
    public interface IInvoiceService
    {
        InvoiceMaster CreateInvoiceForSalesOrder(InvoiceViewModel invoiceData, int tenantId, int userId);
        IQueryable<InvoiceMaster> GetAllInvoiceMasters(int TenantId);
        IQueryable<InvoiceMaster> GetAllInvoiceViews(int TenantId);
        IQueryable<InvoiceMaster> GetAllInvoiceMastersWithAllStatus(int tenantId,int[] accountIds);
        List<InvoiceDetailViewModel> GetAllInvoiceDetailByInvoiceId(int invoiceId);
        List<InvoiceDetailReportViewModel> GetAllInvoiceDetailReportData(int invoiceId);
        AccountTransaction AddAccountTransaction(AccountTransactionTypeEnum type, decimal amount, string notes, int accountId, int tenantId, int userId, AccountPaymentModeEnum? accountPaymentModeId = null);
        AccountTransaction SaveAccountTransaction(AccountTransaction accountTransaction, int tenantId, int userId);
        InvoiceViewModel GetInvoiceMasterByOrderProcessId(int orderProcessId);
        InvoiceViewModel GetInvoiceMasterById(int invoiceId);
        List<AccountTransactionFile> GetaccountTransactionFiles(int accountTransactionId, int tenantId);
        string GenerateNextInvoiceNumber(int tenantId);
        InvoiceViewModel LoadInvoiceProductValuesByOrderProcessId(int orderProcessId, int? inventoryTransctionType = null);
        InvoiceMaster SaveInvoiceForSalesOrder(InvoiceViewModel invoiceData, int tenantId, int userId);
        List<InvoiceProductPriceModel> GetInvoiceProductsPrices(int InvoiceMasterId, int[] productIds);
        InvoiceViewModel GetInvoicePreviewModelByOrderProcessId(int orderProcessId, caTenant tenant);
    }
}
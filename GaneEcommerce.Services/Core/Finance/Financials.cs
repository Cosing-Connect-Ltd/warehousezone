using Ganedata.Core.Data;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Ganedata.Core.Services
{
    public class Financials
    {
        public static decimal CalcAccountBalance(int accountId)
        {
            decimal balance = 0;
            if (accountId <= 0)
            {
                return balance;
            }

            var context = DependencyResolver.Current.GetService<IApplicationContext>();

            var totalInvoiced = context.AccountTransactions.Where(x => x.AccountId == accountId && (x.AccountTransactionTypeId == AccountTransactionTypeEnum.InvoicedToAccount) && x.IsDeleted != true).ToList().Sum(m => m.Amount);

            var totalPaid = context.AccountTransactions.Where(x => x.AccountId == accountId && (x.AccountTransactionTypeId == AccountTransactionTypeEnum.PaidByAccount
            || x.AccountTransactionTypeId == AccountTransactionTypeEnum.Refund || x.AccountTransactionTypeId == AccountTransactionTypeEnum.Discount || x.AccountTransactionTypeId == AccountTransactionTypeEnum.CreditNote) && x.IsDeleted != true).ToList().Sum(m => m.Amount);

            balance = totalInvoiced - totalPaid;

            return balance;

        }
    }
}
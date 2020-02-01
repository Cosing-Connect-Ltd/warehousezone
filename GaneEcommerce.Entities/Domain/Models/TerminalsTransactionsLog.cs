using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class TerminalsTransactionsLog
    {
        public int Id { get; set; }
        public int TerminalId { get; set; }
        public Guid TransactionLogReference { get; set; }
        public DateTime DateCreated { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
    }
}
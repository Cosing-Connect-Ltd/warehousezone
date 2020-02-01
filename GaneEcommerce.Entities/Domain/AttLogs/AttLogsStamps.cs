using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class AttLogsStamps
    {
        [Key]
        public int Id { get; set; }
        public int SStamp { get; set; }
        public TnALogsStampType TnALogsStampType { get; set; }
        public int TerminalId { get; set; }
        [ForeignKey("TerminalId")]
        public virtual Terminals Terminals { get; set; }
    }
}
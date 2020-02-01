using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class TerminalCommandsQueue : PersistableEntity<int>
    {
        public int Id { get; set; }

        public int CommandId { get; set; }
        public DateTime ExecutionDate { get; set; }
        public bool sent { get; set; }
        public DateTime? SentDate { get; set; }
        public bool result { get; set; }
        public string resultString { get; set; }
        public DateTime? ResultDate { get; set; }
        public int TerminalId { get; set; }
        public virtual Terminals Terminal { get; set; }
        [ForeignKey("CommandId")]
        public virtual TerminalCommands TerminalCommands { get; set; }
    }
}
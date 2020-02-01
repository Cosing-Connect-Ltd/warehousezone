using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class TerminalCommands : PersistableEntity<int>
    {
        public TerminalCommands()
        {
            TerminalCommandsQueue = new HashSet<TerminalCommandsQueue>();
        }

        public int Id { get; set; }
        public string CommandIdentifier { get; set; }
        public string CommandString { get; set; }
        public string CommandDescription { get; set; }

        public virtual ICollection<TerminalCommandsQueue> TerminalCommandsQueue { get; set; }

    }
}
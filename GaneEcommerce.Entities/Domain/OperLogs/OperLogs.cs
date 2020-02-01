using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    public class OperLogs
    {
        [Key]
        public int Id { get; set; }
        public string OperationType { get; set; }
        public string AdminID { get; set; }
        public DateTime OperationTime { get; set; }
        public int OperationObject1 { get; set; }
        public int OperationObject2 { get; set; }
        public int OperationObject3 { get; set; }
        public int OperationObject4 { get; set; }

    }
}
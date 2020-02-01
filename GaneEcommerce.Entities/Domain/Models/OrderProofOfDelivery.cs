using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class OrderProofOfDelivery : PersistableEntity<int>
    {
        public int Id { get; set; }
        public string SignatoryName { get; set; }
        public string FileName { get; set; }
        public string FileExtension { get; set; }
        public byte[] FileContent { get; set; }
        public int? OrderProcessID { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual AuthUser User { get; set; }
        [ForeignKey("OrderProcessID")]
        public virtual OrderProcess OrderProcess { get; set; }
        public decimal NoOfCases { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class ContactNumbers
    {
        [Key]
        public int Id { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string WorkNumber { get; set; }
        public string EmailAddress { get; set; }
        public string Fax { get; set; }

    }
}
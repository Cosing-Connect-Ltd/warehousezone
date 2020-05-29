using Ganedata.Core.Entities.Enums;
using System;
using System.ComponentModel;

namespace Ganedata.Core.Models
{
    public class ApiCredentialsViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        [DisplayName("Api URL")]
        public string ApiUrl { get; set; }
        [DisplayName("Api Key")]
        public string ApiKey { get; set; }
        [DisplayName("Api Type")]
        public ApiTypes ApiTypes { get; set; }
        [DisplayName("Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [DisplayName("Account Number")]
        public string AccountNumber { get; set; }
        public int DefaultWarehouseId { get; set; }
        public int? SiteID { get; set; }
        [DisplayName("Website")]
        public string SiteName { get; set; }
        public string Location { get; set; }


    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Domain
{
    [Serializable]
    public class AccountAddressesMapcs : PersistableEntity<int>
    {
        
        [Key]
        [Display(Name = "Id")]
        public int Id { get; set; }
        [Display(Name = "Account")]
        public int AccountId { get; set; }
        [Display(Name = "Address")]
        public int AddressId { get; set; }
    
        public virtual Account Account { get; set; }
        public virtual AccountAddresses AccountAddress { get; set; }
    }

}

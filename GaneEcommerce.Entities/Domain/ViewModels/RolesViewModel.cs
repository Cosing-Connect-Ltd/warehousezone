using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class RolesViewModel : PersistableEntity<int>
    {
        public int Id { get; set; }
        [DisplayName("Role Name"), Required(ErrorMessage = "Role Name is required.")]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Use letters only")]
        public string RoleName { get; set; }
    }
}
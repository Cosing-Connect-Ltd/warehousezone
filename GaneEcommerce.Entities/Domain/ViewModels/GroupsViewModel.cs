using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ganedata.Core.Entities.Domain;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Models
{
    public class GroupsViewModel : PersistableEntity<int>
    {
        public int Id { get; set; }
        [DisplayName("Group Name"), Required(ErrorMessage = "Group Name is required.")]
        public string GroupName { get; set; }
    }
}
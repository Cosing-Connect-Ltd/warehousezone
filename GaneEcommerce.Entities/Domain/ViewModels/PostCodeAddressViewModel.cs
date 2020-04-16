using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class PostCodeAddressViewModel
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<string> addresses { get; set; }
    }
}
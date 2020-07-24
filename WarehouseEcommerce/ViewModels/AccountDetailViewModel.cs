using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;

namespace WarehouseEcommerce.ViewModels
{
    public class AccountDetailViewModel
    {
        public AuthUser AuthUser { get; set; }

        public List<Order> OrderHistory { get; set; }
    }
}
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class AddressServices : IAddressServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public AddressServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public void Update(Address address)
        {
            _currentDbContext.Address.Attach(address);
            _currentDbContext.Entry(address).State = System.Data.Entity.EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public int Insert(Address address)
        {
            _currentDbContext.Entry(address).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return address.Id;
        }

        public void Delete(Address address)
        {
            _currentDbContext.Address.Attach(address);
            _currentDbContext.Address.Remove(address);
            _currentDbContext.SaveChanges();
        }
    }
}
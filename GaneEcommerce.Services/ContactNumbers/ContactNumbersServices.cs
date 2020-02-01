using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class ContactNumbersServices : IContactNumbersServices
    {
        private readonly IApplicationContext _currentDbContext;

        //constructor
        public ContactNumbersServices(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public void Update(ContactNumbers contactNumbers)
        {
            _currentDbContext.ContactNumbers.Attach(contactNumbers);
            _currentDbContext.Entry(contactNumbers).State = System.Data.Entity.EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public int Insert(ContactNumbers contactNumberes)
        {
            _currentDbContext.Entry(contactNumberes).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return contactNumberes.Id;
        }

        public void Delete(ContactNumbers contactNumbers)
        {
            _currentDbContext.ContactNumbers.Attach(contactNumbers);
            _currentDbContext.ContactNumbers.Remove(contactNumbers);
            _currentDbContext.SaveChanges();
        }
    }
}
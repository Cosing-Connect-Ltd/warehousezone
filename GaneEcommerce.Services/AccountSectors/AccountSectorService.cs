using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class AccountSectorService : IAccountSectorService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public AccountSectorService(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public IEnumerable<AccountSector> GetAll()
        {
            return _currentDbContext.AccountSectors.Where(o => o.IsDeleted != true).OrderBy(o => o.Name).ToList();
        }

        public AccountSector Save(string name, int? id = null)
        {
            var accountSector = _currentDbContext.AccountSectors.FirstOrDefault(a => id.HasValue && a.Id == id);
            if (accountSector != null)
            {
                accountSector.Name = name;
                _currentDbContext.Entry(accountSector).State = EntityState.Modified;
            }
            else
            {
                accountSector = new AccountSector
                {
                    Name = name
                };

                _currentDbContext.Entry(accountSector).State = EntityState.Added;
            }

            _currentDbContext.SaveChanges();
            return accountSector;
        }

        public bool Delete(int? id = null)
        {
            var accountSectors = _currentDbContext.AccountSectors.FirstOrDefault(u => u.Id == id);
            if (accountSectors != null)
            {
                accountSectors.IsDeleted = true;
                _currentDbContext.Entry(accountSectors).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return true;
        }
    }
}
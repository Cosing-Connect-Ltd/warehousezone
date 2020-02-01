using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IAddressServices
    {
        void Update(Address address);
        int Insert(Address address);
        void Delete(Address address);
    }
}

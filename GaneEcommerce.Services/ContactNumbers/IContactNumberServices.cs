using Ganedata.Core.Entities.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public interface IContactNumbersServices
    {
        void Update(ContactNumbers contactNumbers);
        int Insert(ContactNumbers contactNumberes);
        void Delete(ContactNumbers contactNumbers);

    }
}

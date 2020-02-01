using System.Collections.Generic;
using System.Linq;
using Ganedata.Warehouse.PropertiesSync.Data;
using Ganedata.Warehouse.PropertiesSync.Services.Interfaces;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.Implementations
{
    public class PLandlordService : IPLandlordService
    {
        public List<PLandlord> GetLandlordsInfoForSite(int siteId)
        {
            var context = UnityServicesToBeReplaced.Repository.GetContext(siteId);
            return context.Host_Land_inf.Select(m => new PLandlord()
            {
                LandlordFullname = m.LNAME,
                LandlordCode = m.LCODE,
                LandlordAdded = m.LADDED,
                LandlordNotes1 = m.LNOTES1,
                LandlordNotes2 = m.LNOTES2,
                MobileNumber = m.LTELMOBL,
                Email = m.LE_MAIL,
                AddressPostcode = m.LPSTCD,
                AddressLine1 = m.LADD1,
                AddressLine2 = m.LADD2,
                AddressLine3 = m.LADD3,
                AddressLine4 = m.LADD4,
                WorkTelephone1 = m.LTELWK1,
                HomeTelephone = m.LTELHOME,
                WorkTelephoneFax = m.LTELFAX,
                LandlordSalutation = m.LSALU,
                LandlordStatus = m.LSTATUS,
                UserNotes1 = m.LUSERNT1,
                UserNotes2 = m.LUSERNT2,
                WorkTelephone2 = m.LTELWK2
            }).ToList();
        }

    }
}
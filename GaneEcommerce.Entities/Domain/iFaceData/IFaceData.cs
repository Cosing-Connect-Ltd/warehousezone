using System;

namespace GaneEcommerce.Entities.Domain
{
    public class IFaceData : BaseEntity
    {
        public string iFaceId { get; set; } 
        public string Status { get; set; }
        DateTime iFaceTimeIN { get; set; }
        DateTime iFaceTimeOUT { get; set; }
        DateTime Date { get; set; }
    }
}

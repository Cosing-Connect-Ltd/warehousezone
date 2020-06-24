using System;

namespace Ganedata.Core.Entities.Domain
{
    public class AbandonedCartNotification
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SiteId { get; set; }
        public DateTime SendDate { get; set; }
    }
}
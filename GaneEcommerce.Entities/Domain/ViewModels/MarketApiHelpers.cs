namespace Ganedata.Core.Entities.Domain
{
    public class MarketJobRequestViewModel
    {
        public int Id { get; set; }

        public int MarketJobStatusId { get; set; }

        public string Reason { get; set; }
        public string UserInfo { get; set; }
        public string DeviceInfo { get; set; }
    }
    
}
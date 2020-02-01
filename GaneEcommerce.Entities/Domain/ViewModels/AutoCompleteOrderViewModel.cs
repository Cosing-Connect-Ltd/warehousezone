namespace Ganedata.Core.Entities.Domain
{
    public class AutoCompleteOrderViewModel
    {
        public int OrderID { get; set; }
        public bool ForceComplete { get; set; }
        public bool IncludeProcessing { get; set; }

        public string DeliveryNumber { get; set; }
    }
}
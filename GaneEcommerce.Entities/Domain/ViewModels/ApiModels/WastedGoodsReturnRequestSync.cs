namespace Ganedata.Core.Models
{
    public class WastedGoodsReturnRequestSync
    {
        public string SerialNo { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string[] Serials { get; set; }
        public string ReturnReason { get; set; }
    }

    public class WastedGoodsReturnResponse : WastedGoodsReturnRequestSync
    {
        public bool IsSuccess { get; set; }
        public bool CanProceed { get; set; }
        public string FailureMessage { get; set; }
    }
}
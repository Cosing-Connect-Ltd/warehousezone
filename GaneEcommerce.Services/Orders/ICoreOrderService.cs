namespace Ganedata.Core.Services
{
    public interface ICoreOrderService: IOrderService, IPurchaseOrderService, IWorksOrderService, ITransferOrderService, ISalesOrderService
    {
      
    }
}
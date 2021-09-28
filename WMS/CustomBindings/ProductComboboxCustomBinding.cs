using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DevExpress.Web;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;

namespace WMS.CustomBindings
{
    public class ProductComboboxCustomBinding
    {
        
        public static IEnumerable<object> GetProducts(ListEditItemsRequestedByFilterConditionEventArgs args)
       {
            var warehouseid= caCurrent.CurrentWarehouse().WarehouseId;
            var OrderId= int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["OrderId"]) ? HttpContext.Current.Request.Params["OrderId"] : "0");
            var ProductDepartment = 0; 
            var ProductGroup = 0; 
            var ProductId = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["ProductId"]) ? HttpContext.Current.Request.Params["ProductId"] : "0");
            var _productServices = DependencyResolver.Current.GetService<IProductServices>();
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            var query = _productServices.GetAllValidProducts(caCurrent.CurrentTenant().TenantId, args.Filter,OrderId,ProductDepartment,ProductGroup, ProductId).OrderBy(u => u.ProductId).Skip(skip).Take(take)
                .Select(u => new {
                    ProductId=u.ProductId,
                    SKUCode=u.SKUCode,
                    BarCode=u.BarCode,
                    Name=u.Name,
                    InventoryStocks= u.InventoryStocks.FirstOrDefault(c=>c.WarehouseId== warehouseid) == null ? 0 : u.InventoryStocks.FirstOrDefault(c => c.WarehouseId == warehouseid).Available,

                }).ToList();
           
            return query.Count==0?null:query.ToList();
        }
        public static object GetProductByID(ListEditItemRequestedByValueEventArgs args)
        {
            var _productServices = DependencyResolver.Current.GetService<IProductServices>();
            int id;
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out id))
                return null;
            return _productServices.GetProductMasterDropdown(id).Select(u => new {
                ProductId = u.ProductId,
                SKUCode = u.SKUCode,
                BarCode = u.BarCode,
                Name = u.Name,
                InventoryStocks = u.InventoryStocks.FirstOrDefault() == null ? 0 : u.InventoryStocks.FirstOrDefault().Available,

            }).FirstOrDefault();
        }

        public static IEnumerable<Order> GetOrders(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var _orderServices = DependencyResolver.Current.GetService<IOrderService>();
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            var query = _orderServices.GetValidSalesOrderByOrderNumber(args.Filter, caCurrent.CurrentTenant().TenantId,null);
            return query.ToList();
        }
        public static Order GetOrderByOrderId(ListEditItemRequestedByValueEventArgs args)
        {
            var _orderServices = DependencyResolver.Current.GetService<IOrderService>();
            int id;
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out id))
                return null;
            return _orderServices.GetOrderById(id);
        }

         public static IQueryable<ProductMaster> ProductMasters { get {
                var DB = DependencyResolver.Current.GetService<IApplicationContext>();
                return DB.ProductMaster;
            } }

        public static IEnumerable<object> GetOrderByFilter(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            var _productServices = DependencyResolver.Current.GetService<IOrderService>();
            var skip = args.BeginIndex;
            var take = args.EndIndex - args.BeginIndex + 1;
            var query = _productServices.GetAllOrders(caCurrent.CurrentTenant().TenantId,caCurrent.CurrentWarehouse().WarehouseId).Where(u=>(u.Account.AccountCode.Contains(args.Filter) || u.Account.CompanyName.Contains(args.Filter)|| u.OrderNumber.Contains(args.Filter)) && u.InventoryTransactionTypeId==Ganedata.Core.Entities.Enums.InventoryTransactionTypeEnum.PurchaseOrder && u.OrderStatusID==Ganedata.Core.Entities.Enums.OrderStatusEnum.Active).OrderBy(u => u.OrderID).Skip(skip).Take(take)
                .Select(u => new {
                    OrderId = u.OrderID,
                    OrderNumber = u.OrderNumber,
                    Account = u.Account.CompanyName,
                    AccountCode = u.Account.AccountCode
                }).ToList();

            return query.Count == 0 ? null : query.ToList();
        }
        public static object GetOrderByID(ListEditItemRequestedByValueEventArgs args)
        {
            var _productServices = DependencyResolver.Current.GetService<IOrderService>();
            int id;
            if (args.Value == null || !int.TryParse(args.Value.ToString(), out id))
                return null;
             return _productServices.GetAllOrders(caCurrent.CurrentTenant().TenantId, caCurrent.CurrentWarehouse().WarehouseId).Where(u=>u.OrderID==id)
                .Select(u => new {
                    OrderId = u.OrderID,
                    OrderNumber = u.OrderNumber,
                    Account = u.Account.CompanyName,
                    AccountCode = u.Account.AccountCode
                }).FirstOrDefault();
        }



    }
}
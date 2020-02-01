using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ConsignmentCustomBinding
    {

        private static IQueryable<DelieveryViewModel> ConsignmentDataset(int tenantId, int warehouseId, int? orderStatusId)
        {
            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();
            if (!orderStatusId.HasValue)
            {
                 orderStatusId = int.Parse(!string.IsNullOrEmpty(HttpContext.Current.Request.Params["OrderStatusId"]) ? HttpContext.Current.Request.Params["OrderStatusId"] : "0");
            }
           
            var transactions = orderServices.GetAllSalesConsignments(tenantId, warehouseId,orderstatusId:(orderStatusId==0?null:orderStatusId)).OrderByDescending(x => x.DateCreated)
                        .Select(ops => new DelieveryViewModel
                         {
                            DeliveryNO = ops.DeliveryNO,
                             OrderID = ops.OrderID,
                             DateCreated = ops.DateCreated,
                             OrderProcessID = ops.OrderProcessID,
                             OrderNumber = ops.Order.OrderNumber,
                             AccountCode=ops.Order.Account.AccountCode,
                             CompanyName=ops.Order.Account.CompanyName,
                            Status = ops.OrderProcessStatusId,
                            orderstatus=ops.Order.OrderStatusID,

                            
                        });

            return transactions;
        }

        public static void ConsignmentDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId,int? consignmentId)
        {

            var transactions = ConsignmentDataset(tenantId, warehouseId,consignmentId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                transactions = transactions.OrderBy(sortString);
            }
            else
            {
                transactions = transactions.OrderBy("DateCreated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void ConsignmentData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId, int? consignmentId)
        {
            var transactions = ConsignmentDataset(tenantId, warehouseId,consignmentId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                transactions = transactions.OrderBy(sortString);
            }
            else
            {
                transactions = transactions.OrderBy("DateCreated Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                transactions = transactions.Where(filterString);

            }

            transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            var context = DependencyResolver.Current.GetService<IApplicationContext>();
            var result = transactions.ToList();
            result.ForEach(u => u.EmailCount = context.TenantEmailNotificationQueues.Count(c => c.OrderId == u.OrderID && c.TenantEmailTemplatesId == 4));
            e.Data = result.ToList();
        }


        public static GridViewModel CreateConsignmentGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderProcessID";

            viewModel.Columns.Add("DeliveryNO");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("EmailCount");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }



    }

}
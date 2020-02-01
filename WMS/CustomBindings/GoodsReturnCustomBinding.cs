using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class GoodsReturnCustomBinding
    {

        private static IQueryable<object> GoodsReturnDataset(int tenantId, int warehouseId)
        {
            var orderServices = DependencyResolver.Current.GetService<ICoreOrderService>();

            var transactions = orderServices.GetAllSalesConsignments(tenantId, warehouseId, (int)InventoryTransactionTypeEnum.Returns).OrderByDescending(x => x.DateCreated)
                        .Select(ops => new
                         {
                            DeliveryNO = ops.DeliveryNO,
                             OrderID = ops.OrderID,
                             DateCreated = ops.DateCreated,
                             OrderProcessID = ops.OrderProcessID,
                             OrderNumber = ops.Order.OrderNumber,
                             AccountCode=ops.Order.Account.AccountCode,
                             CompanyName=ops.Order.Account.CompanyName
                         });

            return transactions;
        }

        public static void GoodsReturnDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GoodsReturnDataset(tenantId, warehouseId);

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

        public static void GoodsReturnData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GoodsReturnDataset(tenantId, warehouseId);

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



            e.Data = transactions.ToList();
        }


        public static GridViewModel CreateGoodsReturnGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "OrderProcessID";

            viewModel.Columns.Add("DeliveryNO");
            viewModel.Columns.Add("OrderNumber");
            viewModel.Columns.Add("CompanyName");
            viewModel.Columns.Add("AccountCode");
            viewModel.Columns.Add("DateCreated");
            
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }


        //private static IQueryable<OrderProcessDetail> GoodsReturnDetailsDataset(int ProcessId)
        //{
        //    var orderServices = DependencyResolver.Current.GetService<IOrderService>();

        //    var transactions = orderServices.GetOrderProcessDetailsByProcessId(ProcessId);
        //    //.Select(ops => new
        //    //{
        //    //    DeliveryNO=ops.DeliveryNO,
        //    //    OrderID=ops.OrderID,
        //    //    DateCreated=ops.DateCreated,
        //    //    OrderProcessID=ops.OrderProcessID,
        //    //    OrderNumber=ops.Order.OrderNumber,
        //    //    ops.Order.Account.AccountCode,
        //    //    ops.Order.Account.CompanyName
        //    //});

        //    return transactions;
        //}

        //public static void GoodsReturnDetailsDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int ProcessId)
        //{

        //    var transactions = GoodsReturnDetailsDataset(ProcessId);

        //    if (e.State.SortedColumns.Count() > 0)
        //    {

        //        string sortString = "";

        //        foreach (var column in e.State.SortedColumns)
        //        {
        //            sortString += column.FieldName + " " + column.SortOrder;
        //        }

        //        transactions = transactions.OrderBy(sortString);
        //    }
        //    else
        //    {
        //        transactions = transactions.OrderBy("DateCreated Desc");
        //    }
        //    if (e.FilterExpression != string.Empty)
        //    {
        //        CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
        //        string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
        //        transactions = transactions.Where(filterString);
        //    }

        //    e.DataRowCount = transactions.Count();

        //}

        //public static void GoodsReturnDetailsData(GridViewCustomBindingGetDataArgs e, int ProcessId)
        //{
        //    var transactions = GoodsReturnDetailsDataset(ProcessId);

        //    if (e.State.SortedColumns.Count() > 0)
        //    {

        //        string sortString = "";

        //        foreach (var column in e.State.SortedColumns)
        //        {
        //            sortString += column.FieldName + " " + column.SortOrder;
        //        }
        //        transactions = transactions.OrderBy(sortString);
        //    }
        //    else
        //    {
        //        transactions = transactions.OrderBy("DateCreated Desc");
        //    }


        //    if (e.FilterExpression != string.Empty)
        //    {
        //        CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

        //        string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

        //        transactions = transactions.Where(filterString);

        //    }

        //    transactions = transactions.Skip(e.StartDataRowIndex).Take(e.DataRowCount);



        //    e.Data = transactions.ToList();
        //}


        //public static GridViewModel CreateGoodsReturnDetailsGridViewModel()
        //{
        //    var viewModel = new GridViewModel();
        //    viewModel.KeyFieldName = "OrderProcessID";

        //    viewModel.Columns.Add("DeliveryNO");
        //    viewModel.Columns.Add("OrderNumber");
        //    viewModel.Columns.Add("CompanyName");
        //    viewModel.Columns.Add("AccountCode");
        //    viewModel.Columns.Add("DateCreated");

        //    viewModel.Pager.PageSize = 10;
        //    return viewModel;
        //}


    }

}
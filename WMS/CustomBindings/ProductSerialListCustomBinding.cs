using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ProductSerialListCustomBinding
    {

        private static IQueryable<object> GetProductSerialDataset(int tenantId, int warehouseId)
        {
            var productServices = DependencyResolver.Current.GetService<IProductServices>();

            var serial = from p in productServices.GetAllProductSerial(tenantId, warehouseId)
                          select new
                          {
                              p.SerialID,
                              p.ProductId,
                              p.ProductMaster.Name,
                              p.ProductMaster.SKUCode,
                              p.SerialNo,
                              p.ExpiryDate,
                              p.Batch,
                              Status = p.CurrentStatus.ToString(),
                              p.DateCreated,
                              p.DateUpdated
                              
                          };

            return serial;
        }

        public static void GetPoductSerialDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var serial = GetProductSerialDataset(tenantId, warehouseId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                serial = serial.OrderBy(sortString);
            }
            else
            {
                serial = serial.OrderBy("DateCreated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                serial = serial.Where(filterString);
            }

            e.DataRowCount = serial.Count();
        }

        public static void GetProductSerialData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var serial = GetProductSerialDataset(tenantId, warehouseId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                serial = serial.OrderBy(sortString);
            }
            else
            {
                serial = serial.OrderBy("DateCreated Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                serial = serial.Where(filterString);

            }

            serial = serial.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            e.Data = serial.ToList();
        }

        public static GridViewModel CreateProductSerialGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "SerialID";

            viewModel.Columns.Add("SerialNo");
            viewModel.Columns.Add("ExpiryDate");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("ProductId");
            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("Batch");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("DateUpdated");
            
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }

}
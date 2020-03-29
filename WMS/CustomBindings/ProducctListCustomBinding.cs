using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class ProducctListCustomBinding
    {

        private static IQueryable<object> GetProductDataset(int tenantId, int warehouseId)
        {
            var productServices = DependencyResolver.Current.GetService<IProductServices>();
            var transactions = productServices.GetAllProductMasterDetail(tenantId, warehouseId);
            return transactions;
        }

        public static void ProductGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetProductDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("SKUCode");
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

        public static void ProductGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetProductDataset(tenantId, warehouseId);

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
                transactions = transactions.OrderBy("SKUCode");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static GridViewModel CreateProductGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "ProductId";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("SKUCode");
            viewModel.Columns.Add("BarCode");
            viewModel.Columns.Add("BarCode2");
            viewModel.Columns.Add("Location");
            viewModel.Columns.Add("UOM");
            viewModel.Columns.Add("DepartmentName");
            viewModel.Columns.Add("Serialisable");
            viewModel.Columns.Add("IsStockItem");
            viewModel.Columns.Add("IsRawMaterial");
            viewModel.Columns.Add("InStock");
            viewModel.Columns.Add("Property");
            viewModel.Columns.Add("TopProduct");
            viewModel.Columns.Add("BestSellerProduct");
            viewModel.Columns.Add("OnSaleProduct");
            viewModel.Columns.Add("SpecialProduct");

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        #region EditableLookupGrid
        static IQueryable<ProductMaster> ProductDetail {
            get 
            {
                var db = DependencyResolver.Current.GetService<IApplicationContext>();
                return db.ProductMaster; 
            } 
        }
        public static GridLookupViewModel CreateProductLookupGridViewModel()
        {
            var viewModel = new GridLookupViewModel();
            viewModel.KeyFieldName = "ProductId";
            viewModel.Columns.Add("Name");
            viewModel.Columns.Add("Qty");
            viewModel.TextFormatString = "{0} {1}";

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void GetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {
            
            e.DataRowCount = ProductDetail.Where(u=>u.TenantId== tenantId).Count();
        }
        public static void GetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            ProductDetail.Where(u => u.TenantId == tenantId).Count();
            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                ProductDetail.Where(u => u.TenantId == tenantId).OrderBy(sortString);
            }
            else
            {
                ProductDetail.Where(u => u.TenantId == tenantId).OrderBy("ProductId");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                ProductDetail.Where(u => u.TenantId == tenantId).Where(filterString);

            }
            ProductDetail.Where(u => u.TenantId == tenantId).Skip(e.StartDataRowIndex).Take(e.DataRowCount);
            e.Data = ProductDetail.Where(u => u.TenantId == tenantId);

        }
        public static void GetRowValues(GridViewCustomBindingGetRowValuesArgs e, int tenantId, int warehouseId)
        {
            if (e.KeyValues.Count() == 0)
                e.RowValues = ProductDetail.Where(u => u.TenantId == tenantId && object.Equals(u.ProductId, -1)).ToList();
            else
            {
                var list = new List<ProductMaster>();
                foreach (var item in e.KeyValues)
                {
                    list.Add(ProductDetail.Where(c => object.Equals(c.ProductId, item)).FirstOrDefault());
                }
                e.RowValues = list;
            }



        }

        #endregion

    }
}
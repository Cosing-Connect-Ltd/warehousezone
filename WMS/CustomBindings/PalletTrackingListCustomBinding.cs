using DevExpress.Data.Filtering;
using DevExpress.Data.Linq.Helpers;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Services;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class PalletTrackingListCustomBinding
    {

        public  static IQueryable<PalletTracking> GetPalletTrackingDataset(int tenantId, int warehouseId)
        {
            var productServices = DependencyResolver.Current.GetService<IProductServices>();
            var pallets = productServices.GetAllPalletTrackings(tenantId, warehouseId,includeArchived:true);
            //var pallets = from p in productServices.GetAllPalletTrackings(tenantId, warehouseId)
            //              select new
            //              {
            //                  p.PalletTrackingId,
            //                  p.ProductId,
            //                  p.OrderId,
            //                  p.ProductMaster.Name,
            //                  p.ProductMaster.SKUCode,
            //                  p.PalletSerial,
            //                  p.ExpiryDate,
            //                  p.RemainingCases,
            //                  p.TotalCases,
            //                  p.BatchNo,
            //                  p.Comments,
            //                  Status = p.Status.ToString(),
            //                  p.DateCreated,
            //                  p.DateUpdated,
            //                  p.ProductMaster.ProductGroup.ProductGroup,
            //                  p.ProductMaster.TenantDepartment.DepartmentName
            //              };

            return pallets;
        }

        public static void GetPalletTrackingDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var pallets = GetPalletTrackingDataset(tenantId, warehouseId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }

                pallets = pallets.OrderBy(sortString);
            }
            else
            {
                pallets = pallets.OrderBy("DateCreated Desc");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                pallets = pallets.Where(filterString);
            }

            e.DataRowCount = pallets.Count();
        }

        public static void GetGetPalletTrackingData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var pallets = GetPalletTrackingDataset(tenantId, warehouseId);

            if (e.State.SortedColumns.Count() > 0)
            {

                string sortString = "";

                foreach (var column in e.State.SortedColumns)
                {
                    sortString += column.FieldName + " " + column.SortOrder;
                }
                pallets = pallets.OrderBy(sortString);
            }
            else
            {
                pallets = pallets.OrderBy("DateCreated Desc");
            }


            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);

                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);

                pallets = pallets.Where(filterString);

            }

            pallets = pallets.Skip(e.StartDataRowIndex).Take(e.DataRowCount);

            e.Data = pallets.ToList();
        }

        public static GridViewModel CreateGetPalletTrackingGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "PalletTrackingId";
            
            viewModel.Columns.Add("PalletSerial");
            viewModel.Columns.Add("ExpiryDate");
            viewModel.Columns.Add("RemainingCases");
            viewModel.Columns.Add("OrderId");
            viewModel.Columns.Add("TotalCases");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("ProductId");
            viewModel.Columns.Add("ProductName");
            viewModel.Columns.Add("ProductGroup");
            viewModel.Columns.Add("DepartmentName");
            viewModel.Columns.Add("BatchNo");
            viewModel.Columns.Add("Status");
            viewModel.Columns.Add("DateUpdated");
            
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }

    }

}
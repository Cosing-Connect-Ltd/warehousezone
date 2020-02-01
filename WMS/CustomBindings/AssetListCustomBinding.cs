using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Models;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class AssetListCustomBinding
    {
        public static GridViewModel CreateAssetListGridViewModel()
        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "AssetId";
            viewModel.Columns.Add("AssetName");
            viewModel.Columns.Add("AssetDescription");
            viewModel.Columns.Add("AssetCode");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("IsActive");


            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void AssetListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int tenantId, int warehouseId)
        {

            var transactions = GetAssetListDataset(tenantId, warehouseId);

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

        private static IQueryable<Assets> GetAssetListDataset(int tenantId, int warehouseId)
        {
            var assetServices = DependencyResolver.Current.GetService<IAssetServices>();
            var transactions = assetServices.GetAllValidAssets(tenantId);
            return transactions;
        }
        public static void AssetListGetData(GridViewCustomBindingGetDataArgs e, int tenantId, int warehouseId)
        {
            var transactions = GetAssetListDataset(tenantId, warehouseId);

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


        public static GridViewModel CreateAssetLogListGridViewModel()
        {

            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "AssetLogId";
            viewModel.Columns.Add("TerminalId");
            viewModel.Columns.Add("AssetId");
            viewModel.Columns.Add("piAddress");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DateUpdated");
            viewModel.Columns.Add("uuid");
            viewModel.Columns.Add("major");
            viewModel.Columns.Add("minor");
            viewModel.Columns.Add("measuredPower");
            viewModel.Columns.Add("rssi");
            viewModel.Columns.Add("accuracy");
            viewModel.Columns.Add("proximity");
            viewModel.Columns.Add("address");
            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
        public static void AssetLogListGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int AssetId)
        {


            var transactions = GetAssetLogListDataset(AssetId);

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

        private static IQueryable<AssetLog> GetAssetLogListDataset(int AssetId)
        {
            var assetservices = DependencyResolver.Current.GetService<IAssetServices>();
            var transactions = assetservices.GetAllAssetLog(AssetId);
            return transactions;
        }
        public static void AssetLogListGetData(GridViewCustomBindingGetDataArgs e, int AssetId)
        {
            var transactions = GetAssetLogListDataset(AssetId);

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




    }
}
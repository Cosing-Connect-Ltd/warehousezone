using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using System.Web.Mvc;

namespace WMS.CustomBindings
{
    public class PalletsCustomBinding
    {

        #region pallets
        private static IQueryable<PalletViewModel> GetPalletDataset(int? type, bool status = false, int? PalletsDispatchID = null)
        {
            var _palletingService = DependencyResolver.Current.GetService<IPalletingService>();

            IQueryable<PalletViewModel> result;
            if (!status)
            {
                switch (type)
                {
                    case 2:
                        result = _palletingService.GetAllPallets(null, PalletStatusEnum.Completed);
                        break;

                    default:
                        result = _palletingService.GetAllPallets(null, PalletStatusEnum.Active,dispatchId: PalletsDispatchID);
                        break;
                }

                return result;
            }
            else
            {
                result = _palletingService.GetAllPallets(null, PalletStatusEnum.Completed, null, null, type);
                return result;
            }

        }

        public static void GetPalletDataRowCount(GridViewCustomBindingGetDataRowCountArgs e, int? type, bool status = false, int? PalletsDispatchID = null)
        {

            var pallets = GetPalletDataset(type, status, PalletsDispatchID);

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

        public static void GetPalletData(GridViewCustomBindingGetDataArgs e, int? type, bool status = false, int? PalletsDispatchID = null)
        {
            var pallets = GetPalletDataset(type, status,PalletsDispatchID);


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

        public static GridViewModel CreatePalletGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "PalletID";

            viewModel.Columns.Add("PalletNumber");
            viewModel.Columns.Add("AccountName");
            viewModel.Columns.Add("DateCreated");
            viewModel.Columns.Add("DispatchTime");
            viewModel.Columns.Add("Dispatch.DispatchStatus");
            viewModel.Columns.Add("ScannedOnLoading");
            viewModel.Columns.Add("ScannedOnDelivered");
            

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }


        #endregion
    }
}
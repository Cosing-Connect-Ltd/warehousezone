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
    public class PLandlordsCustomBinding
    {
        private static IQueryable<PLandlord> GetPlandlordDataset()
        {
            
            var PropertyService = DependencyResolver.Current.GetService<IPropertyService>();
            var transactions = PropertyService.GetAllValidPropertyLandlords();

            return transactions;
        }


        public static void PlandlordGetDataRowCount(GridViewCustomBindingGetDataRowCountArgs e)
        {

            var transactions = GetPlandlordDataset();

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
                transactions = transactions.OrderBy("PLandlordId");
            }
            if (e.FilterExpression != string.Empty)
            {
                CriteriaOperator op = CriteriaOperator.Parse(e.FilterExpression);
                string filterString = CriteriaToWhereClauseHelper.GetDynamicLinqWhere(op);
                transactions = transactions.Where(filterString);
            }

            e.DataRowCount = transactions.Count();

        }

        public static void PlandlordGetData(GridViewCustomBindingGetDataArgs e)
        {

            var transactions = GetPlandlordDataset();

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
                transactions = transactions.OrderBy("PLandlordId");
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

        public static GridViewModel CreatePlandlordGridViewModel()
        {
            var viewModel = new GridViewModel();
            viewModel.KeyFieldName = "PLandlordId";

            viewModel.Columns.Add("LandlordCode");
            viewModel.Columns.Add("LandlordStatus");
            viewModel.Columns.Add("LandlordFullname");
            viewModel.Columns.Add("LandlordAdded");
            viewModel.Columns.Add("Email");
            viewModel.Columns.Add("AddressLine1");
            viewModel.Columns.Add("AddressLine2");
            viewModel.Columns.Add("AddressPostcode");
            viewModel.Columns.Add("HomeTelephone");
            viewModel.Columns.Add("LandlordNotes1");
          

            viewModel.Pager.PageSize = 10;
            return viewModel;
        }
    }
}
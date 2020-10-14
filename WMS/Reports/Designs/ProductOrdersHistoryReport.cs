using Ganedata.Core.Services;
using System;
using System.Linq;
using System.Web.Mvc;

namespace WMS.Reports.Designs
{
    public partial class ProductOrdersHistoryReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ProductOrdersHistoryReport()
        {
            InitializeComponent();
        }

        private void ProductSoldBySkuPrint_DataSourceDemanded(object sender, EventArgs e)
        {
            ProductOrdersHistoryReport report = (ProductOrdersHistoryReport)sender;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["EndDate"].Value = endDate;
            var accountIds = (int[])report.Parameters["AccountIds"].Value;
            var ownerIds = (int[])report.Parameters["OwnerIds"].Value;
            username.Visible = false;
            ComanyName.Visible = false;
            lblAcount.Visible = false;
            lblOwner.Visible = false;
            if ((accountIds?.Length ?? 0) >= 1)
            {
                var account = DependencyResolver.Current.GetService<IAccountServices>();
                ComanyName.Visible = true;
                ComanyName.Text = string.Join(", ",  account.GetAllValidAccountsByAccountIds(accountIds)?.Select(a => a.CompanyName));
                lblAcount.Visible = true;
            }
            if (ownerIds?.Length >= 1)
            {
                var users = DependencyResolver.Current.GetService<IUserService>();
                username.Visible = true;
                lblOwner.Visible = true;
                username.Text = string.Join(", ", users.GetAuthUserByIds(ownerIds)?.Select(a => a.DisplayName)) ;
            }
        }
    }
}

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Web.Mvc;
using Ganedata.Core.Services;

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
            int? accountId = (int?)report.Parameters["AccountId"].Value;
            int? UserId = (int?)report.Parameters["paramOwnerID"].Value;
            username.Visible = false;
            ComanyName.Visible = false;
            lblAcount.Visible = false;
            lblOwner.Visible = false;
            if (accountId.HasValue)
            {
                var account = DependencyResolver.Current.GetService<IAccountServices>();
                ComanyName.Visible = true;
                ComanyName.Text = account.GetAccountsById(accountId??0)?.CompanyName;
                lblAcount.Visible = true;
            }
            if (UserId.HasValue)
            {
                var users = DependencyResolver.Current.GetService<IUserService>();
                username.Visible = true;
                lblOwner.Visible = true;
                username.Text = users.GetAuthUserById(UserId ?? 0)?.DisplayName;
            }
        }
    }
}

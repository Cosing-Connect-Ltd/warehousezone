using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class ProductSoldByPalletType : DevExpress.XtraReports.UI.XtraReport
    {
        public ProductSoldByPalletType()
        {
            InitializeComponent();
        }

        private void ProductSoldByPalletType_DataSourceDemanded(object sender, EventArgs e)
        {
            ProductSoldByPalletType report = (ProductSoldByPalletType)sender;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["EndDate"].Value = endDate;

        }
    }
}

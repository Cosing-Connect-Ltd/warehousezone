using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class ProductSoldBySkuPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public ProductSoldBySkuPrint()
        {
            InitializeComponent();
        }

        private void ProductSoldBySkuPrint_DataSourceDemanded(object sender, EventArgs e)
        {
            ProductSoldBySkuPrint report = (ProductSoldBySkuPrint)sender;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["EndDate"].Value = endDate;
        }
    }
}

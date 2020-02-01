using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class ProductMovementPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public ProductMovementPrint()
        {
            InitializeComponent();
        }

        private void ProductMovementPrint_DataSourceDemanded(object sender, EventArgs e)
        {
            ProductMovementPrint report = (ProductMovementPrint)sender;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["EndDate"].Value = endDate;
        }
    }
}

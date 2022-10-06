using System;

namespace WMS.Reports.Designs
{
    public partial class StockValueDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public StockValueDetailReport()
        {
            InitializeComponent();
        }

        private void StockValueDetailReportlPrint_DataSourceDemanded(object sender, EventArgs e)
        {

        }
    }
}

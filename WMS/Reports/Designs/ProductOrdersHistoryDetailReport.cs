using System;

namespace WMS.Reports.Designs
{
    public partial class ProductOrdersHistoryDetailReport : DevExpress.XtraReports.UI.XtraReport
    {
        public ProductOrdersHistoryDetailReport()
        {
            InitializeComponent();
        }

        private void ProductSoldBySkuDetailPrint_DataSourceDemanded(object sender, EventArgs e)
        {

        }
    }
}

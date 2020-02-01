using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports
{
    public partial class MarketTotal : DevExpress.XtraReports.UI.XtraReport
    {
        public MarketTotal()
        {
            InitializeComponent();
        }

        private void MarketTotal_DataSourceDemanded(object sender, EventArgs e)
        {
            MarketTotal report = (MarketTotal)sender;
            DateTime endDate = (DateTime)report.Parameters["paramEndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["paramEndDate"].Value = endDate;
        }

        private void MarketTotal_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}

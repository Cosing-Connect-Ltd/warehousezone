using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class DailySalesZReport : DevExpress.XtraReports.UI.XtraReport
    {
        public DailySalesZReport()
        {
            InitializeComponent();
        }

        private void xrLabel32_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}

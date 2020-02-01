using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class HolidayReportPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public HolidayReportPrint()
        {
            InitializeComponent();
        }

        private void Detail1_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
    }
}

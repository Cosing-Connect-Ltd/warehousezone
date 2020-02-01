using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Web.Mvc;
using Ganedata.Core.Services;

namespace WMS.Reports
{
    public partial class PalleteDispatchesReport : DevExpress.XtraReports.UI.XtraReport
    {
        public PalleteDispatchesReport()
        {
            InitializeComponent();
           
        }
        

        private void xrTableCell9_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XtraReportBase report = (sender as XRControl).Report;
            bool timberproperties = report.GetCurrentColumnValue<bool>("EnableTimberProperties");
           
            if (timberproperties == false)
            {
                e.Cancel = true;
            }
        }

        private void xrTable2_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
           
           

            
        }

        private void PalleteDispatchesReport_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            

        }

        private void xrTableCell10_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XtraReportBase report = (sender as XRControl).Report;
            bool timberproperties = report.GetCurrentColumnValue<bool>("EnableTimberProperties");

            if (timberproperties == false)
            {
                e.Cancel = true;
            }
        }
    }
}

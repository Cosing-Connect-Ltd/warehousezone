using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace WMS.Reports.Designs
{
    public partial class FinancialTransactionReport : DevExpress.XtraReports.UI.XtraReport
    {
        public FinancialTransactionReport()
        {
            InitializeComponent();
        }

        private void FinancialTransactionReport_DataSourceDemanded(object sender, EventArgs e)
        {
            FinancialTransactionReport report = (FinancialTransactionReport)sender;
            DateTime endDate = (DateTime)report.Parameters["EndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["EndDate"].Value = endDate;
        }

        private void FinancialTransactionReport_AfterPrint(object sender, EventArgs e)
        {
        }
    }
}

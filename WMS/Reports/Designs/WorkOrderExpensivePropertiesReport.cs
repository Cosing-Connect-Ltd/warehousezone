using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using Ganedata.Core.Models;

namespace WMS.Reports
{
    public partial class WorkOrderExpensivePropertiesReport : DevExpress.XtraReports.UI.XtraReport
    {
        public WorkOrderExpensivePropertiesReport()
        {
            InitializeComponent();
        }

        private void WorkOrderExpensivePropertiesReport_DataSourceDemanded(object sender, EventArgs e)
        {
            WorkOrderExpensivePropertiesReport report = (WorkOrderExpensivePropertiesReport)sender;
            DateTime endDate = (DateTime)report.Parameters["paramEndDate"].Value;
            endDate = endDate.AddHours(24);
            report.Parameters["paramEndDate"].Value = endDate;
        }
    }
}

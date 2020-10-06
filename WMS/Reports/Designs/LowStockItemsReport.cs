using DevExpress.XtraReports.Parameters;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WMS.Reports
{
    public partial class LowStockItemsReport : DevExpress.XtraReports.UI.XtraReport
    {
        public LowStockItemsReport()
        {
            InitializeComponent();
        }

        private void xrLabel12_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            (sender as XRLabel).Text = "";
            Parameter parameter = Parameters["paramProductGroupId"];
            if (parameter.Value != null)
            {
                List<LookUpValue> staticValueCollection = (parameter.LookUpSettings as StaticListLookUpSettings).LookUpValues.ToList();
                (sender as XRLabel).Text = staticValueCollection.FirstOrDefault(x => Object.Equals(x.Value, this.Parameters["paramProductGroupId"].Value)).Description;
            }
        }

        private void xrLabel14_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            (sender as XRLabel).Text = "";
            Parameter parameter = Parameters["paramdepartmentId"];
            if (parameter.Value != null)
            {
                List<LookUpValue> staticValueCollection = (parameter.LookUpSettings as StaticListLookUpSettings).LookUpValues.ToList();
                (sender as XRLabel).Text = staticValueCollection.FirstOrDefault(x => Object.Equals(x.Value, this.Parameters["paramdepartmentId"].Value)).Description;
            }
        }
    }
}

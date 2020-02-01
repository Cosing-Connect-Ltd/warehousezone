using Ganedata.Core.Entities.Enums;

namespace WMS.Reports.Designs
{
    public partial class PurchaseOrderAuditReport : DevExpress.XtraReports.UI.XtraReport
    {
        public PurchaseOrderAuditReport()
        {
            InitializeComponent();
        }

        private void InventoryTransType_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            

        }
    }
}

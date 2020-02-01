using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;
using Ganedata.Core.Entities.Enums;

namespace WMS.Reports
{
    public partial class PurchaseOrderPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public PurchaseOrderPrint()
        {
            InitializeComponent();
        }

        private void PurchaseOrderPrint_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }


        private void prodDelType_GetValue(object sender, DevExpress.XtraReports.UI.GetValueEventArgs e)
        {
            //ProductDeliveryTypeEnum = e.GetColumnValue("OrderDate");
            //e.Value = (int)((DateTime)columnValue).DayOfWeek;

        }

        private void xrLabel65_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            var DeliveryTypeEnum = DetailReport.GetCurrentColumnValue("ProdDeliveryType");
            if (!string.IsNullOrEmpty(DeliveryTypeEnum.ToString()))
            {
                var value = (ProductDeliveryTypeEnum?)((int?)DeliveryTypeEnum);
                if (value.HasValue)
                {
                    xrLabel65.Text = value.ToString();
                }
            }
        }
        //private void xrRichText4_BeforePrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        //{
        //    var DeliveryTypeEnum = DetailReport.GetCurrentColumnValue("ProdDeliveryType");
        //    var value = (ProductDeliveryTypeEnum?)((int?)DeliveryTypeEnum);
        //    if (value.HasValue)
        //    {
        //        xrRichText4.Rtf = value.ToString();
        //    }
        //}
    }
}

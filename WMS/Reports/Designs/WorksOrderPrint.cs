using DevExpress.XtraReports.UI;
using DevExpress.XtraRichEdit;

namespace WMS.Reports
{
    public partial class WorksOrderPrint : DevExpress.XtraReports.UI.XtraReport
    {
        public WorksOrderPrint()
        {
            InitializeComponent();
        }

        private void xrRichText2_BeforePrint_1(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            XRRichText richText = (XRRichText)sender;

            using (RichEditDocumentServer docServer = new RichEditDocumentServer())
            {
                docServer.RtfText = richText.Rtf;
                docServer.Document.DefaultCharacterProperties.FontName = "Tahoma";
                docServer.Document.DefaultCharacterProperties.FontSize = 9f;
                richText.Rtf = docServer.RtfText;
            }
        }
    }
}

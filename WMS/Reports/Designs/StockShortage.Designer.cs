namespace WMS.Reports.Designs
{
    partial class StockShortage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ReOrder = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductName = new DevExpress.XtraReports.UI.XRLabel();
            this.Qty = new DevExpress.XtraReports.UI.XRLabel();
            this.SkuCode = new DevExpress.XtraReports.UI.XRLabel();
            this.paramStartDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramEndDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.OrderNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.order = new DevExpress.XtraReports.UI.XRLabel();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 5F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.order,
            this.OrderNumber});
            this.Detail.HeightF = 22.79167F;
            this.Detail.Name = "Detail";
            // 
            // ReOrder
            // 
            this.ReOrder.BorderWidth = 0F;
            this.ReOrder.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReOrder.LocationFloat = new DevExpress.Utils.PointFloat(643.7084F, 0F);
            this.ReOrder.Multiline = true;
            this.ReOrder.Name = "ReOrder";
            this.ReOrder.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ReOrder.SizeF = new System.Drawing.SizeF(162.2914F, 19.19999F);
            this.ReOrder.StylePriority.UseBorderWidth = false;
            this.ReOrder.StylePriority.UseFont = false;
            this.ReOrder.Text = "Qty";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel20,
            this.xrLabel21,
            this.xrLine1,
            this.xrLabel22,
            this.lblDate,
            this.xrLabel1});
            this.ReportHeader.HeightF = 83.74998F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // xrLabel20
            // 
            this.xrLabel20.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?paramStartDate")});
            this.xrLabel20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel20.ForeColor = System.Drawing.Color.Black;
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(262.0419F, 49.45835F);
            this.xrLabel20.Name = "xrLabel20";
            this.xrLabel20.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel20.SizeF = new System.Drawing.SizeF(156.7502F, 18.83335F);
            this.xrLabel20.StylePriority.UseBackColor = false;
            this.xrLabel20.StylePriority.UseFont = false;
            this.xrLabel20.StylePriority.UseForeColor = false;
            this.xrLabel20.StylePriority.UseTextAlignment = false;
            this.xrLabel20.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel20.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLabel21
            // 
            this.xrLabel21.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "AddSeconds(?paramEndDate,-1)")});
            this.xrLabel21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel21.ForeColor = System.Drawing.Color.Black;
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(442.7504F, 49.45835F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(158.3333F, 18.83334F);
            this.xrLabel21.StylePriority.UseBackColor = false;
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.StylePriority.UseForeColor = false;
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel21.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.000122F, 68.29173F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(800.9998F, 12.99999F);
            // 
            // xrLabel22
            // 
            this.xrLabel22.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel22.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel22.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(1.000126F, 18.83335F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(800.9998F, 30.62501F);
            this.xrLabel22.StylePriority.UseBackColor = false;
            this.xrLabel22.StylePriority.UseBorders = false;
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.StylePriority.UseTextAlignment = false;
            this.xrLabel22.Text = "Stock Shortage Report";
            this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // lblDate
            // 
            this.lblDate.BackColor = System.Drawing.Color.Transparent;
            this.lblDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.lblDate.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDate.ForeColor = System.Drawing.Color.Black;
            this.lblDate.LocationFloat = new DevExpress.Utils.PointFloat(56.24987F, 0F);
            this.lblDate.Name = "lblDate";
            this.lblDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblDate.SizeF = new System.Drawing.SizeF(191.1664F, 18.83335F);
            this.lblDate.StylePriority.UseBackColor = false;
            this.lblDate.StylePriority.UseFont = false;
            this.lblDate.StylePriority.UseForeColor = false;
            this.lblDate.StylePriority.UseTextAlignment = false;
            this.lblDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.lblDate.TextFormatString = "{0:dd/MM/yy HH:mm}";
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.ForeColor = System.Drawing.Color.Black;
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(56.24987F, 18.83335F);
            this.xrLabel1.StylePriority.UseBackColor = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseForeColor = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Date:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // ProductName
            // 
            this.ProductName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductName]")});
            this.ProductName.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProductName.LocationFloat = new DevExpress.Utils.PointFloat(154.1665F, 0F);
            this.ProductName.Name = "ProductName";
            this.ProductName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductName.SizeF = new System.Drawing.SizeF(489.5419F, 19.77499F);
            this.ProductName.StylePriority.UseFont = false;
            this.ProductName.Text = "Product";
            // 
            // Qty
            // 
            this.Qty.BorderWidth = 0F;
            this.Qty.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Qty]")});
            this.Qty.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Qty.LocationFloat = new DevExpress.Utils.PointFloat(643.7087F, 0F);
            this.Qty.Multiline = true;
            this.Qty.Name = "Qty";
            this.Qty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Qty.SizeF = new System.Drawing.SizeF(162.2914F, 19.19999F);
            this.Qty.StylePriority.UseBorderWidth = false;
            this.Qty.StylePriority.UseFont = false;
            this.Qty.Text = "Qty";
            // 
            // SkuCode
            // 
            this.SkuCode.BorderWidth = 0F;
            this.SkuCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SkuCode]")});
            this.SkuCode.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkuCode.LocationFloat = new DevExpress.Utils.PointFloat(7.947286E-06F, 0F);
            this.SkuCode.Name = "SkuCode";
            this.SkuCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SkuCode.SizeF = new System.Drawing.SizeF(154.1665F, 20.11668F);
            this.SkuCode.StylePriority.UseBorderWidth = false;
            this.SkuCode.StylePriority.UseFont = false;
            this.SkuCode.Text = "SKU";
            // 
            // paramStartDate
            // 
            this.paramStartDate.Description = "Date From";
            this.paramStartDate.Name = "paramStartDate";
            this.paramStartDate.Type = typeof(System.DateTime);
            this.paramStartDate.ValueInfo = "2022-06-27";
            // 
            // paramEndDate
            // 
            this.paramEndDate.Description = "To Date";
            this.paramEndDate.Name = "paramEndDate";
            this.paramEndDate.Type = typeof(System.DateTime);
            this.paramEndDate.ValueInfo = "2022-06-27";
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.ReportHeader1});
            this.DetailReport.DataMember = "StockReportDetails";
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.Qty,
            this.SkuCode,
            this.ProductName});
            this.Detail1.HeightF = 20.11668F;
            this.Detail1.Name = "Detail1";
            // 
            // OrderNumber
            // 
            this.OrderNumber.BorderWidth = 0F;
            this.OrderNumber.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderNumber]")});
            this.OrderNumber.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderNumber.LocationFloat = new DevExpress.Utils.PointFloat(168.2497F, 3.000005F);
            this.OrderNumber.Name = "OrderNumber";
            this.OrderNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.OrderNumber.SizeF = new System.Drawing.SizeF(167.2496F, 18.87499F);
            this.OrderNumber.StylePriority.UseBorderWidth = false;
            this.OrderNumber.StylePriority.UseFont = false;
            this.OrderNumber.Text = "Order Number";
            // 
            // xrLabel5
            // 
            this.xrLabel5.BorderWidth = 0F;
            this.xrLabel5.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(154.1665F, 19.19999F);
            this.xrLabel5.StylePriority.UseBorderWidth = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.Text = "SKU";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(154.1665F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(489.5419F, 19.77499F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Product";
            // 
            // ReportHeader1
            // 
            this.ReportHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7,
            this.ReOrder,
            this.xrLabel5});
            this.ReportHeader1.HeightF = 19.77499F;
            this.ReportHeader1.Name = "ReportHeader1";
            // 
            // order
            // 
            this.order.BorderWidth = 0F;
            this.order.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.order.LocationFloat = new DevExpress.Utils.PointFloat(1.000122F, 2.675011F);
            this.order.Name = "order";
            this.order.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.order.SizeF = new System.Drawing.SizeF(167.2496F, 19.19999F);
            this.order.StylePriority.UseBorderWidth = false;
            this.order.StylePriority.UseFont = false;
            this.order.Text = "Order Number";
            // 
            // StockShortage
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.ReportHeader,
            this.DetailReport});
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(35, 7, 5, 0);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramStartDate,
            this.paramEndDate});
            this.Version = "21.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        public DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.Parameters.Parameter paramStartDate;
        public DevExpress.XtraReports.Parameters.Parameter paramEndDate;
        private DevExpress.XtraReports.UI.XRLabel ReOrder;
        private DevExpress.XtraReports.UI.XRLabel ProductName;
        private DevExpress.XtraReports.UI.XRLabel Qty;
        private DevExpress.XtraReports.UI.XRLabel SkuCode;
        private DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel OrderNumber;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader1;
        private DevExpress.XtraReports.UI.XRLabel order;
    }
}

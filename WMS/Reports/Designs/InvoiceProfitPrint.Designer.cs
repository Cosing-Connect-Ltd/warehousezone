namespace WMS.Reports.Designs
{
    partial class InvoiceProfitPrint
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
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings2 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings3 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.InvoiceNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.CompanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.Date = new DevExpress.XtraReports.UI.XRLabel();
            this.NetAmtB = new DevExpress.XtraReports.UI.XRLabel();
            this.NetAmtS = new DevExpress.XtraReports.UI.XRLabel();
            this.Profit = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.paramStartDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramEndDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.SKU = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReOrder = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.TotalProfit = new DevExpress.XtraReports.UI.XRLabel();
            this.TotalNetAmtS = new DevExpress.XtraReports.UI.XRLabel();
            this.TotalNetAmtB = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.paramAccountIds = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramProductIds = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramMarketId = new DevExpress.XtraReports.Parameters.Parameter();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.detailsToggle = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader1 = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.ReportFooter1 = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 1F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 0.04167557F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.detailsToggle,
            this.InvoiceNumber,
            this.CompanyName,
            this.Date,
            this.NetAmtB,
            this.NetAmtS,
            this.Profit});
            this.Detail.HeightF = 25.14165F;
            this.Detail.Name = "Detail";
            // 
            // InvoiceNumber
            // 
            this.InvoiceNumber.BorderWidth = 0F;
            this.InvoiceNumber.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[InvoiceNumber]")});
            this.InvoiceNumber.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvoiceNumber.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.InvoiceNumber.Name = "InvoiceNumber";
            this.InvoiceNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.InvoiceNumber.SizeF = new System.Drawing.SizeF(90.62487F, 23F);
            this.InvoiceNumber.StylePriority.UseBorderWidth = false;
            this.InvoiceNumber.StylePriority.UseFont = false;
            // 
            // CompanyName
            // 
            this.CompanyName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CompanyName]")});
            this.CompanyName.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CompanyName.LocationFloat = new DevExpress.Utils.PointFloat(90.62492F, 0F);
            this.CompanyName.Name = "CompanyName";
            this.CompanyName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.CompanyName.SizeF = new System.Drawing.SizeF(283.9861F, 23F);
            this.CompanyName.StylePriority.UseFont = false;
            // 
            // Date
            // 
            this.Date.BorderWidth = 0F;
            this.Date.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Date]")});
            this.Date.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Date.LocationFloat = new DevExpress.Utils.PointFloat(374.611F, 0F);
            this.Date.Multiline = true;
            this.Date.Name = "Date";
            this.Date.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Date.SizeF = new System.Drawing.SizeF(94.79166F, 23F);
            this.Date.StylePriority.UseBorderWidth = false;
            this.Date.StylePriority.UseFont = false;
            this.Date.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // NetAmtB
            // 
            this.NetAmtB.BorderWidth = 0F;
            this.NetAmtB.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NetAmtB]")});
            this.NetAmtB.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NetAmtB.LocationFloat = new DevExpress.Utils.PointFloat(469.4026F, 0F);
            this.NetAmtB.Multiline = true;
            this.NetAmtB.Name = "NetAmtB";
            this.NetAmtB.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.NetAmtB.SizeF = new System.Drawing.SizeF(113.3744F, 23.0111F);
            this.NetAmtB.StylePriority.UseBorderWidth = false;
            this.NetAmtB.StylePriority.UseFont = false;
            this.NetAmtB.TextFormatString = "{0:#.00}";
            // 
            // NetAmtS
            // 
            this.NetAmtS.BorderWidth = 0F;
            this.NetAmtS.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[NetAmtS]")});
            this.NetAmtS.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NetAmtS.LocationFloat = new DevExpress.Utils.PointFloat(582.7772F, 0F);
            this.NetAmtS.Multiline = true;
            this.NetAmtS.Name = "NetAmtS";
            this.NetAmtS.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.NetAmtS.SizeF = new System.Drawing.SizeF(107.3755F, 23F);
            this.NetAmtS.StylePriority.UseBorderWidth = false;
            this.NetAmtS.StylePriority.UseFont = false;
            this.NetAmtS.TextFormatString = "{0:#.00}";
            // 
            // Profit
            // 
            this.Profit.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Profit]")});
            this.Profit.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Profit.LocationFloat = new DevExpress.Utils.PointFloat(690.1526F, 0F);
            this.Profit.Name = "Profit";
            this.Profit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Profit.SizeF = new System.Drawing.SizeF(92.09766F, 23F);
            this.Profit.StylePriority.UseFont = false;
            this.Profit.TextFormatString = "{0:#.00}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblDate,
            this.xrLabel1,
            this.xrLabel19,
            this.xrLabel20,
            this.xrLabel21,
            this.xrLine1,
            this.xrLabel22});
            this.ReportHeader.HeightF = 86.66681F;
            this.ReportHeader.Name = "ReportHeader";
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
            // xrLabel19
            // 
            this.xrLabel19.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel19.ForeColor = System.Drawing.Color.Black;
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(137.5838F, 49.45835F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(123.458F, 18.83335F);
            this.xrLabel19.StylePriority.UseBackColor = false;
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UseForeColor = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.Text = "Report Period :";
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel20
            // 
            this.xrLabel20.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel20.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.paramStartDate]")});
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
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.paramEndDate]")});
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
            this.xrLabel22.Text = "Invoice Profit Report";
            this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // paramStartDate
            // 
            this.paramStartDate.Description = "Start Date";
            this.paramStartDate.Name = "paramStartDate";
            this.paramStartDate.Type = typeof(System.DateTime);
            // 
            // paramEndDate
            // 
            this.paramEndDate.Description = "End Date";
            this.paramEndDate.Name = "paramEndDate";
            this.paramEndDate.Type = typeof(System.DateTime);
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.SKU,
            this.xrLabel7,
            this.xrLabel3,
            this.ReOrder,
            this.xrLabel8});
            this.PageHeader.HeightF = 26.17486F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.BorderWidth = 0F;
            this.xrLabel4.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(469.4026F, 0F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(113.3744F, 23.38333F);
            this.xrLabel4.StylePriority.UseBorderWidth = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Net Amt (Buy)";
            // 
            // SKU
            // 
            this.SKU.BorderWidth = 0F;
            this.SKU.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SKU.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.SKU.Name = "SKU";
            this.SKU.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SKU.SizeF = new System.Drawing.SizeF(90.62487F, 23F);
            this.SKU.StylePriority.UseBorderWidth = false;
            this.SKU.StylePriority.UseFont = false;
            this.SKU.Text = "Invoice No";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(90.62492F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(283.9861F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Account";
            // 
            // xrLabel3
            // 
            this.xrLabel3.BorderWidth = 0F;
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(374.611F, 0F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(94.79166F, 23F);
            this.xrLabel3.StylePriority.UseBorderWidth = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Date";
            // 
            // ReOrder
            // 
            this.ReOrder.BorderWidth = 0F;
            this.ReOrder.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReOrder.LocationFloat = new DevExpress.Utils.PointFloat(582.7772F, 0F);
            this.ReOrder.Multiline = true;
            this.ReOrder.Name = "ReOrder";
            this.ReOrder.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ReOrder.SizeF = new System.Drawing.SizeF(107.3755F, 23F);
            this.ReOrder.StylePriority.UseBorderWidth = false;
            this.ReOrder.StylePriority.UseFont = false;
            this.ReOrder.Text = "Net Amt (Sell)";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(690.1526F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(111.8472F, 23F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "Profit";
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.TotalProfit,
            this.TotalNetAmtS,
            this.TotalNetAmtB,
            this.xrLabel5});
            this.ReportFooter.HeightF = 31.29168F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // TotalProfit
            // 
            this.TotalProfit.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalProfit.LocationFloat = new DevExpress.Utils.PointFloat(690.1526F, 0F);
            this.TotalProfit.Name = "TotalProfit";
            this.TotalProfit.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TotalProfit.SizeF = new System.Drawing.SizeF(111.8472F, 23F);
            this.TotalProfit.StylePriority.UseFont = false;
            this.TotalProfit.StylePriority.UseTextAlignment = false;
            this.TotalProfit.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.TotalProfit.TextFormatString = "{0:#.00}";
            // 
            // TotalNetAmtS
            // 
            this.TotalNetAmtS.BorderWidth = 0F;
            this.TotalNetAmtS.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalNetAmtS.LocationFloat = new DevExpress.Utils.PointFloat(582.7772F, 0F);
            this.TotalNetAmtS.Multiline = true;
            this.TotalNetAmtS.Name = "TotalNetAmtS";
            this.TotalNetAmtS.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TotalNetAmtS.SizeF = new System.Drawing.SizeF(107.3755F, 23F);
            this.TotalNetAmtS.StylePriority.UseBorderWidth = false;
            this.TotalNetAmtS.StylePriority.UseFont = false;
            this.TotalNetAmtS.StylePriority.UseTextAlignment = false;
            this.TotalNetAmtS.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.TotalNetAmtS.TextFormatString = "{0:#.00}";
            // 
            // TotalNetAmtB
            // 
            this.TotalNetAmtB.BorderWidth = 0F;
            this.TotalNetAmtB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalNetAmtB.LocationFloat = new DevExpress.Utils.PointFloat(469.4026F, 0F);
            this.TotalNetAmtB.Multiline = true;
            this.TotalNetAmtB.Name = "TotalNetAmtB";
            this.TotalNetAmtB.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TotalNetAmtB.SizeF = new System.Drawing.SizeF(113.3744F, 21.84998F);
            this.TotalNetAmtB.StylePriority.UseBorderWidth = false;
            this.TotalNetAmtB.StylePriority.UseFont = false;
            this.TotalNetAmtB.StylePriority.UseTextAlignment = false;
            this.TotalNetAmtB.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.TotalNetAmtB.TextFormatString = "{0:#.00}";
            // 
            // xrLabel5
            // 
            this.xrLabel5.BorderWidth = 0F;
            this.xrLabel5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(361.0695F, 0F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(108.3332F, 21.85001F);
            this.xrLabel5.StylePriority.UseBorderWidth = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Total";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // paramAccountIds
            // 
            this.paramAccountIds.AllowNull = true;
            this.paramAccountIds.Description = "Select Accounts";
            this.paramAccountIds.MultiValue = true;
            this.paramAccountIds.Name = "paramAccountIds";
            this.paramAccountIds.Type = typeof(int);
            this.paramAccountIds.ValueSourceSettings = staticListLookUpSettings1;
            // 
            // paramProductIds
            // 
            this.paramProductIds.AllowNull = true;
            this.paramProductIds.Description = "Select Products";
            this.paramProductIds.MultiValue = true;
            this.paramProductIds.Name = "paramProductIds";
            this.paramProductIds.Type = typeof(int);
            this.paramProductIds.ValueSourceSettings = staticListLookUpSettings2;
            // 
            // paramMarketId
            // 
            this.paramMarketId.AllowNull = true;
            this.paramMarketId.Description = "Market";
            this.paramMarketId.Name = "paramMarketId";
            this.paramMarketId.Type = typeof(int);
            this.paramMarketId.ValueSourceSettings = staticListLookUpSettings3;
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.ReportHeader1,
            this.ReportFooter1});
            this.DetailReport.DataMember = "ProductsDetail";
            this.DetailReport.DrillDownControl = this.detailsToggle;
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            this.DetailReport.ReportPrintOptions.DetailCountOnEmptyDataSource = 0;
            this.DetailReport.ReportPrintOptions.PrintOnEmptyDataSource = false;
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel10,
            this.xrLabel9,
            this.xrLabel6,
            this.xrLabel2});
            this.Detail1.HeightF = 23.0111F;
            this.Detail1.Name = "Detail1";
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductName]")});
            this.xrLabel2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(26.04158F, 0F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(443.3611F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            // 
            // xrLabel6
            // 
            this.xrLabel6.BorderWidth = 0F;
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BuyingPrice]")});
            this.xrLabel6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(469.4026F, 0F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(113.3744F, 23.0111F);
            this.xrLabel6.StylePriority.UseBorderWidth = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.TextFormatString = "{0:#.00}";
            // 
            // xrLabel9
            // 
            this.xrLabel9.BorderWidth = 0F;
            this.xrLabel9.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SellingPrice]")});
            this.xrLabel9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(582.7772F, 0F);
            this.xrLabel9.Multiline = true;
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(107.3755F, 23F);
            this.xrLabel9.StylePriority.UseBorderWidth = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.TextFormatString = "{0:#.00}";
            // 
            // xrLabel10
            // 
            this.xrLabel10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Profit]")});
            this.xrLabel10.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(690.1526F, 0F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(111.8472F, 23F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.TextFormatString = "{0:#.00}";
            // 
            // detailsToggle
            // 
            this.detailsToggle.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.detailsToggle.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([ReportItems.DetailReport].[DrillDownExpanded], \'-\', \'+\')")});
            this.detailsToggle.Font = new System.Drawing.Font("Arial", 13F);
            this.detailsToggle.ForeColor = System.Drawing.Color.RoyalBlue;
            this.detailsToggle.LocationFloat = new DevExpress.Utils.PointFloat(782.2501F, 2.119276E-05F);
            this.detailsToggle.Multiline = true;
            this.detailsToggle.Name = "detailsToggle";
            this.detailsToggle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.detailsToggle.SizeF = new System.Drawing.SizeF(20.74988F, 23F);
            this.detailsToggle.StylePriority.UseBorders = false;
            this.detailsToggle.StylePriority.UseFont = false;
            this.detailsToggle.StylePriority.UseForeColor = false;
            this.detailsToggle.StylePriority.UseTextAlignment = false;
            this.detailsToggle.Text = "+";
            this.detailsToggle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // ReportHeader1
            // 
            this.ReportHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine3});
            this.ReportHeader1.HeightF = 6.666692F;
            this.ReportHeader1.Name = "ReportHeader1";
            // 
            // ReportFooter1
            // 
            this.ReportFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine2});
            this.ReportFooter1.HeightF = 11.99443F;
            this.ReportFooter1.Name = "ReportFooter1";
            // 
            // xrLine2
            // 
            this.xrLine2.ForeColor = System.Drawing.Color.Silver;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(26.04158F, 0F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(756.2087F, 3.888914F);
            this.xrLine2.StylePriority.UseForeColor = false;
            // 
            // xrLine3
            // 
            this.xrLine3.ForeColor = System.Drawing.Color.Gray;
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(26.04158F, 0F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(756.2087F, 6.666692F);
            this.xrLine3.StylePriority.UseForeColor = false;
            // 
            // InvoiceProfitPrint
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.ReportHeader,
            this.PageHeader,
            this.ReportFooter,
            this.DetailReport});
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(28, 19, 1, 0);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramStartDate,
            this.paramEndDate,
            this.paramAccountIds,
            this.paramProductIds,
            this.paramMarketId});
            this.Version = "20.1";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.Parameters.Parameter paramStartDate;
        public DevExpress.XtraReports.Parameters.Parameter paramEndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.UI.XRLabel lblDate;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRLabel SKU;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel ReOrder;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        public DevExpress.XtraReports.UI.XRLabel InvoiceNumber;
        public DevExpress.XtraReports.UI.XRLabel CompanyName;
        public DevExpress.XtraReports.UI.XRLabel Date;
        public DevExpress.XtraReports.UI.XRLabel NetAmtB;
        public DevExpress.XtraReports.UI.XRLabel NetAmtS;
        public DevExpress.XtraReports.UI.XRLabel Profit;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        public DevExpress.XtraReports.UI.XRLabel TotalProfit;
        public DevExpress.XtraReports.UI.XRLabel TotalNetAmtS;
        public DevExpress.XtraReports.UI.XRLabel TotalNetAmtB;
        public DevExpress.XtraReports.Parameters.Parameter paramAccountIds;
        public DevExpress.XtraReports.Parameters.Parameter paramProductIds;
        public DevExpress.XtraReports.Parameters.Parameter paramMarketId;
        private DevExpress.XtraReports.UI.DetailBand Detail1;
        public DevExpress.XtraReports.UI.XRLabel xrLabel2;
        public DevExpress.XtraReports.UI.DetailReportBand DetailReport;
        public DevExpress.XtraReports.UI.XRLabel xrLabel10;
        public DevExpress.XtraReports.UI.XRLabel xrLabel9;
        public DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel detailsToggle;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader1;
        private DevExpress.XtraReports.UI.XRLine xrLine3;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter1;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
    }
}

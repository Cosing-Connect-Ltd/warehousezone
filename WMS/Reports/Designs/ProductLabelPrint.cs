using DevExpress.XtraReports.UI;
using System;

/// <summary>
/// Summary description for ProductLabelPrintTestTest
/// </summary>
public class ProductLabelPrint : DevExpress.XtraReports.UI.XtraReport
{
    private TopMarginBand TopMargin;
    private BottomMarginBand BottomMargin;
    private DetailBand Detail;
    public XRLabel ProductName;
    private XRLabel xrLabel1;
    public XRLabel ProductSkuCode;
    private XRLine xrLine5;
    public XRBarCode BarCode;
    private XRLine xrLine2;
    private XRLabel xrLabel3;
    private XRLabel xrLabel2;
    public XRLabel Cases;
    private XRLine xrLine3;
    private XRLabel xrLabel4;
    private XRLabel xrLabel5;
    public XRLabel Comments;
    public XRLabel ExpiryDate;
    private XRLine xrLine4;
    private XRLine xrLine1;
    public XRBarCode Quantity;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public ProductLabelPrint()
    {
        InitializeComponent();
    }

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
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator1 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            DevExpress.XtraPrinting.BarCode.Code128Generator code128Generator2 = new DevExpress.XtraPrinting.BarCode.Code128Generator();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.ProductName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductSkuCode = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine5 = new DevExpress.XtraReports.UI.XRLine();
            this.BarCode = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.Cases = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.Comments = new DevExpress.XtraReports.UI.XRLabel();
            this.ExpiryDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.Quantity = new DevExpress.XtraReports.UI.XRBarCode();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 0F;
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
            this.ProductName,
            this.xrLabel1,
            this.ProductSkuCode,
            this.xrLine5,
            this.BarCode,
            this.xrLine2,
            this.xrLabel3,
            this.xrLabel2,
            this.Cases,
            this.xrLine3,
            this.xrLabel4,
            this.xrLabel5,
            this.Comments,
            this.ExpiryDate,
            this.xrLine4,
            this.xrLine1,
            this.Quantity});
            this.Detail.HeightF = 596.5277F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            // 
            // ProductName
            // 
            this.ProductName.CanGrow = false;
            this.ProductName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductName]")});
            this.ProductName.Font = new System.Drawing.Font("Tahoma", 16F);
            this.ProductName.LocationFloat = new DevExpress.Utils.PointFloat(12.5974F, 37.08333F);
            this.ProductName.Multiline = true;
            this.ProductName.Name = "ProductName";
            this.ProductName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductName.SizeF = new System.Drawing.SizeF(224.9026F, 116.2155F);
            this.ProductName.StylePriority.UseFont = false;
            this.ProductName.StylePriority.UseTextAlignment = false;
            this.ProductName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 9.999996F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(77.59739F, 23F);
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Product:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // ProductSkuCode
            // 
            this.ProductSkuCode.CanGrow = false;
            this.ProductSkuCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductSkuCode]")});
            this.ProductSkuCode.Font = new System.Drawing.Font("Tahoma", 16F);
            this.ProductSkuCode.LocationFloat = new DevExpress.Utils.PointFloat(252.9861F, 37.08333F);
            this.ProductSkuCode.Multiline = true;
            this.ProductSkuCode.Name = "ProductSkuCode";
            this.ProductSkuCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductSkuCode.SizeF = new System.Drawing.SizeF(138.6112F, 116.2155F);
            this.ProductSkuCode.StylePriority.UseFont = false;
            this.ProductSkuCode.StylePriority.UseTextAlignment = false;
            this.ProductSkuCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLine5
            // 
            this.xrLine5.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
            this.xrLine5.LineWidth = 3F;
            this.xrLine5.LocationFloat = new DevExpress.Utils.PointFloat(237.5F, 0F);
            this.xrLine5.Name = "xrLine5";
            this.xrLine5.SizeF = new System.Drawing.SizeF(12.88867F, 171.6113F);
            // 
            // BarCode
            // 
            this.BarCode.Alignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.BarCode.AutoModule = true;
            this.BarCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductBarCode]")});
            this.BarCode.LocationFloat = new DevExpress.Utils.PointFloat(12.59732F, 494.5279F);
            this.BarCode.Module = 1.354167F;
            this.BarCode.Name = "BarCode";
            this.BarCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.BarCode.SizeF = new System.Drawing.SizeF(376.4026F, 75.6944F);
            this.BarCode.StylePriority.UseTextAlignment = false;
            this.BarCode.Symbology = code128Generator1;
            this.BarCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrLine2
            // 
            this.xrLine2.LineWidth = 3F;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 171.6113F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(400F, 3.208313F);
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(210.9723F, 185.9306F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(91.90961F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "CTN:";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 185.9306F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(64.82633F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "QTY:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // Cases
            // 
            this.Cases.CanGrow = false;
            this.Cases.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Cases]")});
            this.Cases.Font = new System.Drawing.Font("Tahoma", 16F);
            this.Cases.LocationFloat = new DevExpress.Utils.PointFloat(213.5696F, 225.3476F);
            this.Cases.Multiline = true;
            this.Cases.Name = "Cases";
            this.Cases.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Cases.SizeF = new System.Drawing.SizeF(178.0277F, 84.12131F);
            this.Cases.StylePriority.UseFont = false;
            this.Cases.StylePriority.UseTextAlignment = false;
            this.Cases.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLine3
            // 
            this.xrLine3.LineWidth = 3F;
            this.xrLine3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 338.2953F);
            this.xrLine3.Name = "xrLine3";
            this.xrLine3.SizeF = new System.Drawing.SizeF(400F, 6.333313F);
            // 
            // xrLabel4
            // 
            this.xrLabel4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 356.4342F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(112.0486F, 23F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.Text = "Date:";
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel5
            // 
            this.xrLabel5.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(210.9723F, 356.4342F);
            this.xrLabel5.Multiline = true;
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(109.9653F, 23F);
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "PO No:";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // Comments
            // 
            this.Comments.CanGrow = false;
            this.Comments.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderNumber]")});
            this.Comments.Font = new System.Drawing.Font("Tahoma", 16F);
            this.Comments.LocationFloat = new DevExpress.Utils.PointFloat(210.9723F, 394.1911F);
            this.Comments.Multiline = true;
            this.Comments.Name = "Comments";
            this.Comments.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Comments.SizeF = new System.Drawing.SizeF(180.625F, 73.36804F);
            this.Comments.StylePriority.UseFont = false;
            this.Comments.StylePriority.UseTextAlignment = false;
            this.Comments.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // ExpiryDate
            // 
            this.ExpiryDate.CanGrow = false;
            this.ExpiryDate.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[LabelDate]")});
            this.ExpiryDate.Font = new System.Drawing.Font("Tahoma", 16F);
            this.ExpiryDate.LocationFloat = new DevExpress.Utils.PointFloat(10.00003F, 394.1911F);
            this.ExpiryDate.Multiline = true;
            this.ExpiryDate.Name = "ExpiryDate";
            this.ExpiryDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ExpiryDate.SizeF = new System.Drawing.SizeF(180.625F, 73.36804F);
            this.ExpiryDate.StylePriority.UseFont = false;
            this.ExpiryDate.StylePriority.UseTextAlignment = false;
            this.ExpiryDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            this.ExpiryDate.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLine4
            // 
            this.xrLine4.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
            this.xrLine4.LineWidth = 3F;
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(196.6146F, 174.8196F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(3.131912F, 305.7604F);
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 3F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 480.58F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(399F, 3.295044F);
            // 
            // Quantity
            // 
            this.Quantity.Alignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.Quantity.AutoModule = true;
            this.Quantity.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Quantity]")});
            this.Quantity.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 225.3476F);
            this.Quantity.Module = 1.354167F;
            this.Quantity.Name = "Quantity";
            this.Quantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.Quantity.SizeF = new System.Drawing.SizeF(180.625F, 84.12128F);
            this.Quantity.StylePriority.UseTextAlignment = false;
            this.Quantity.Symbology = code128Generator2;
            this.Quantity.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // ProductLabelPrint
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail});
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(0, 0, 0, 0);
            this.PageHeight = 600;
            this.PageWidth = 400;
            this.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            this.Version = "20.1";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.ProductLabelPrintTestTest_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion

    private void ProductLabelPrintTestTest_DataSourceDemanded(object sender, EventArgs e)
    {
    }
}

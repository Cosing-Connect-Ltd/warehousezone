using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for PalletLabelPrintTest
/// </summary>
public class PalletLabelPrint : DevExpress.XtraReports.UI.XtraReport
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
    public XRLabel BatchNumber;
    private XRLine xrLine3;
    private XRLabel xrLabel4;
    private XRLabel xrLabel5;
    public XRLabel Comments;
    public XRLabel ExpiryDate;
    private XRLine xrLine4;
    private XRLine xrLine1;
    public XRBarCode PalletSerial;
    private XRLabel xrLabel6;
    private XRLabel xrLabel8;
    private XRLabel xrLabel7;
    private XRLine xrLine6;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public PalletLabelPrint()
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
            this.xrLine6 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductSkuCode = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine5 = new DevExpress.XtraReports.UI.XRLine();
            this.BarCode = new DevExpress.XtraReports.UI.XRBarCode();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.Cases = new DevExpress.XtraReports.UI.XRLabel();
            this.BatchNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine3 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.Comments = new DevExpress.XtraReports.UI.XRLabel();
            this.ExpiryDate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine4 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.PalletSerial = new DevExpress.XtraReports.UI.XRBarCode();
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
            this.xrLine6,
            this.xrLabel8,
            this.xrLabel7,
            this.xrLabel6,
            this.ProductName,
            this.xrLabel1,
            this.ProductSkuCode,
            this.xrLine5,
            this.BarCode,
            this.xrLine2,
            this.xrLabel3,
            this.xrLabel2,
            this.Cases,
            this.BatchNumber,
            this.xrLine3,
            this.xrLabel4,
            this.xrLabel5,
            this.Comments,
            this.ExpiryDate,
            this.xrLine4,
            this.xrLine1,
            this.PalletSerial});
            this.Detail.HeightF = 596.5277F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            // 
            // xrLine6
            // 
            this.xrLine6.LineWidth = 3F;
            this.xrLine6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 36.45836F);
            this.xrLine6.Name = "xrLine6";
            this.xrLine6.SizeF = new System.Drawing.SizeF(400F, 3.208313F);
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderNumber]")});
            this.xrLabel8.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(167.5694F, 3.000005F);
            this.xrLabel8.Multiline = true;
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(221.4305F, 33.45836F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.Text = "PO-00123456";
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(17.45123F, 3.000005F);
            this.xrLabel7.Multiline = true;
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(119.6527F, 33.45836F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.Text = "PO Number:";
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel6
            // 
            this.xrLabel6.Font = new System.Drawing.Font("Tahoma", 12F);
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(12.59732F, 567.8645F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(139.1319F, 23F);
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "Pallet Serial";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // ProductName
            // 
            this.ProductName.CanGrow = false;
            this.ProductName.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductName]")});
            this.ProductName.Font = new System.Drawing.Font("Tahoma", 16F);
            this.ProductName.LocationFloat = new DevExpress.Utils.PointFloat(12.59739F, 62.16667F);
            this.ProductName.Multiline = true;
            this.ProductName.Name = "ProductName";
            this.ProductName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductName.SizeF = new System.Drawing.SizeF(224.9026F, 70.6599F);
            this.ProductName.StylePriority.UseFont = false;
            this.ProductName.StylePriority.UseTextAlignment = false;
            this.ProductName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(15.45123F, 40.29166F);
            this.xrLabel1.Multiline = true;
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(77.59739F, 19.875F);
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
            this.ProductSkuCode.LocationFloat = new DevExpress.Utils.PointFloat(252.9861F, 62.16667F);
            this.ProductSkuCode.Multiline = true;
            this.ProductSkuCode.Name = "ProductSkuCode";
            this.ProductSkuCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductSkuCode.SizeF = new System.Drawing.SizeF(138.6112F, 133.4167F);
            this.ProductSkuCode.StylePriority.UseFont = false;
            this.ProductSkuCode.StylePriority.UseTextAlignment = false;
            this.ProductSkuCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLine5
            // 
            this.xrLine5.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
            this.xrLine5.LineWidth = 3F;
            this.xrLine5.LocationFloat = new DevExpress.Utils.PointFloat(237.5F, 39.66667F);
            this.xrLine5.Name = "xrLine5";
            this.xrLine5.SizeF = new System.Drawing.SizeF(12.88867F, 159.0279F);
            // 
            // BarCode
            // 
            this.BarCode.Alignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.BarCode.AutoModule = true;
            this.BarCode.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[ProductBarCode]")});
            this.BarCode.LocationFloat = new DevExpress.Utils.PointFloat(12.5974F, 146.2779F);
            this.BarCode.Module = 1.354167F;
            this.BarCode.Name = "BarCode";
            this.BarCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.BarCode.SizeF = new System.Drawing.SizeF(224.9026F, 49.30556F);
            this.BarCode.StylePriority.UseTextAlignment = false;
            this.BarCode.Symbology = code128Generator1;
            this.BarCode.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // xrLine2
            // 
            this.xrLine2.LineWidth = 3F;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 198.6946F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(400F, 3.208313F);
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([RequiresBatchNumber], \'Batch No:\', \'\')\n")});
            this.xrLabel3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(210.9723F, 201.9029F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(91.90961F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(12.59735F, 201.9029F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(64.82633F, 23F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.Text = "Cases:";
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // Cases
            // 
            this.Cases.CanGrow = false;
            this.Cases.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Cases]")});
            this.Cases.Font = new System.Drawing.Font("Tahoma", 16F);
            this.Cases.LocationFloat = new DevExpress.Utils.PointFloat(12.59732F, 237.1532F);
            this.Cases.Multiline = true;
            this.Cases.Name = "Cases";
            this.Cases.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Cases.SizeF = new System.Drawing.SizeF(178.0277F, 84.12131F);
            this.Cases.StylePriority.UseFont = false;
            this.Cases.StylePriority.UseTextAlignment = false;
            this.Cases.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // BatchNumber
            // 
            this.BatchNumber.CanGrow = false;
            this.BatchNumber.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[BatchNumber]")});
            this.BatchNumber.Font = new System.Drawing.Font("Tahoma", 16F);
            this.BatchNumber.LocationFloat = new DevExpress.Utils.PointFloat(210.9723F, 237.1532F);
            this.BatchNumber.Multiline = true;
            this.BatchNumber.Name = "BatchNumber";
            this.BatchNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.BatchNumber.SizeF = new System.Drawing.SizeF(180.625F, 84.12131F);
            this.BatchNumber.StylePriority.UseFont = false;
            this.BatchNumber.StylePriority.UseTextAlignment = false;
            this.BatchNumber.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
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
            this.xrLabel4.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([RequiresExpiryDate], \'Expiry Date:\', \'Date:\')")});
            this.xrLabel4.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 356.4342F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(112.0486F, 23F);
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
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
            this.xrLabel5.Text = "Comments:";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // Comments
            // 
            this.Comments.CanGrow = false;
            this.Comments.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Comments]")});
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
            this.xrLine4.LocationFloat = new DevExpress.Utils.PointFloat(196.6146F, 201.9029F);
            this.xrLine4.Name = "xrLine4";
            this.xrLine4.SizeF = new System.Drawing.SizeF(3.131897F, 278.6771F);
            // 
            // xrLine1
            // 
            this.xrLine1.LineWidth = 3F;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 480.58F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(399F, 3.295044F);
            // 
            // PalletSerial
            // 
            this.PalletSerial.Alignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
            this.PalletSerial.AutoModule = true;
            this.PalletSerial.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[PalletSerial]")});
            this.PalletSerial.LocationFloat = new DevExpress.Utils.PointFloat(10.00003F, 492.8578F);
            this.PalletSerial.Module = 1.354167F;
            this.PalletSerial.Name = "PalletSerial";
            this.PalletSerial.Padding = new DevExpress.XtraPrinting.PaddingInfo(10, 10, 0, 0, 100F);
            this.PalletSerial.SizeF = new System.Drawing.SizeF(378.9999F, 75.00674F);
            this.PalletSerial.StylePriority.UseTextAlignment = false;
            this.PalletSerial.Symbology = code128Generator2;
            this.PalletSerial.TextAlignment = DevExpress.XtraPrinting.TextAlignment.BottomCenter;
            // 
            // PalletLabelPrint
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
            this.Version = "21.1";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.PalletLabelPrintTest_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion

    private void PalletLabelPrintTest_DataSourceDemanded(object sender, EventArgs e)
    {
    }
}

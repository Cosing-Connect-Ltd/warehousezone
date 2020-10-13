using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for InvoiceDetailReport
/// </summary>
public class InvoiceDetailReport : DevExpress.XtraReports.UI.XtraReport
{
    private TopMarginBand TopMargin;
    private BottomMarginBand BottomMargin;
    private DetailBand Detail;
    public DevExpress.XtraReports.Parameters.Parameter InvoiceId;
    private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
    private ReportHeaderBand ReportHeader;
    private XRLabel xrLabel22;
    private XRLine xrLine1;
    private XRLabel xrLabel19;
    private XRLabel xrLabel1;
    public XRLabel lblDate;
    private PageHeaderBand PageHeader;
    private XRLabel xrLabel2;
    private XRLabel SKU;
    private XRLabel xrLabel7;
    private XRLabel xrLabel3;
    private XRLabel xrLabel4;
    public XRLabel DeliveryNote;
    public XRLabel SaleOrderNumber;
    public XRLabel PalletNumber;
    public XRLabel Quantity;
    public XRLabel ProductName;
    public XRLabel SkuCode;
    public DevExpress.XtraReports.Parameters.Parameter TenantID;
    public DevExpress.XtraReports.Parameters.Parameter WarehouseId;
    private XRLabel xrLabel14;
    private XRLabel xrLabel17;
    private XRLabel xrLabel16;
    private XRLabel xrLabel15;
    private XRLabel ReOrder;
    public XRLabel SupplierInvoiceNumber;
    public XRLabel SupplierName;
    public XRLabel PurchaseOrderNumber;
    public XRLabel InvoiceDate;
    public XRLabel InvoiceNumber;
    private XRPageInfo xrPageInfo1;
    private XRLine xrLine2;
    private PageFooterBand PageFooter;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public InvoiceDetailReport()
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
            this.components = new System.ComponentModel.Container();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.SupplierInvoiceNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.SupplierName = new DevExpress.XtraReports.UI.XRLabel();
            this.PurchaseOrderNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.DeliveryNote = new DevExpress.XtraReports.UI.XRLabel();
            this.SaleOrderNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.PalletNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.Quantity = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductName = new DevExpress.XtraReports.UI.XRLabel();
            this.SkuCode = new DevExpress.XtraReports.UI.XRLabel();
            this.InvoiceId = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.InvoiceDate = new DevExpress.XtraReports.UI.XRLabel();
            this.InvoiceNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel14 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblDate = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel17 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel15 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.SKU = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReOrder = new DevExpress.XtraReports.UI.XRLabel();
            this.TenantID = new DevExpress.XtraReports.Parameters.Parameter();
            this.WarehouseId = new DevExpress.XtraReports.Parameters.Parameter();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 5F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 10F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.SupplierInvoiceNumber,
            this.SupplierName,
            this.PurchaseOrderNumber,
            this.DeliveryNote,
            this.SaleOrderNumber,
            this.PalletNumber,
            this.Quantity,
            this.ProductName,
            this.SkuCode});
            this.Detail.HeightF = 23.3333F;
            this.Detail.KeepTogether = true;
            this.Detail.Name = "Detail";
            // 
            // SupplierInvoiceNumber
            // 
            this.SupplierInvoiceNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SupplierInvoiceNumber.LocationFloat = new DevExpress.Utils.PointFloat(1018.482F, 0F);
            this.SupplierInvoiceNumber.Multiline = true;
            this.SupplierInvoiceNumber.Name = "SupplierInvoiceNumber";
            this.SupplierInvoiceNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SupplierInvoiceNumber.SizeF = new System.Drawing.SizeF(106.5182F, 19.99998F);
            this.SupplierInvoiceNumber.StylePriority.UseFont = false;
            // 
            // SupplierName
            // 
            this.SupplierName.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SupplierName.LocationFloat = new DevExpress.Utils.PointFloat(842.7963F, 0F);
            this.SupplierName.Multiline = true;
            this.SupplierName.Name = "SupplierName";
            this.SupplierName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SupplierName.SizeF = new System.Drawing.SizeF(175.6853F, 19.99998F);
            this.SupplierName.StylePriority.UseFont = false;
            // 
            // PurchaseOrderNumber
            // 
            this.PurchaseOrderNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PurchaseOrderNumber.LocationFloat = new DevExpress.Utils.PointFloat(748.1295F, 0F);
            this.PurchaseOrderNumber.Multiline = true;
            this.PurchaseOrderNumber.Name = "PurchaseOrderNumber";
            this.PurchaseOrderNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.PurchaseOrderNumber.SizeF = new System.Drawing.SizeF(94.66675F, 19.99998F);
            this.PurchaseOrderNumber.StylePriority.UseFont = false;
            // 
            // DeliveryNote
            // 
            this.DeliveryNote.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeliveryNote.LocationFloat = new DevExpress.Utils.PointFloat(562.259F, 0F);
            this.DeliveryNote.Multiline = true;
            this.DeliveryNote.Name = "DeliveryNote";
            this.DeliveryNote.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.DeliveryNote.SizeF = new System.Drawing.SizeF(185.8705F, 19.99998F);
            this.DeliveryNote.StylePriority.UseFont = false;
            // 
            // SaleOrderNumber
            // 
            this.SaleOrderNumber.CanGrow = false;
            this.SaleOrderNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaleOrderNumber.LocationFloat = new DevExpress.Utils.PointFloat(470.2131F, 0F);
            this.SaleOrderNumber.Name = "SaleOrderNumber";
            this.SaleOrderNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SaleOrderNumber.SizeF = new System.Drawing.SizeF(92.04599F, 19.99998F);
            this.SaleOrderNumber.StylePriority.UseFont = false;
            // 
            // PalletNumber
            // 
            this.PalletNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PalletNumber.LocationFloat = new DevExpress.Utils.PointFloat(365.6992F, 0F);
            this.PalletNumber.Multiline = true;
            this.PalletNumber.Name = "PalletNumber";
            this.PalletNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.PalletNumber.SizeF = new System.Drawing.SizeF(104.5137F, 19.99998F);
            this.PalletNumber.StylePriority.UseFont = false;
            // 
            // Quantity
            // 
            this.Quantity.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Quantity.LocationFloat = new DevExpress.Utils.PointFloat(299.0325F, 0F);
            this.Quantity.Multiline = true;
            this.Quantity.Name = "Quantity";
            this.Quantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Quantity.SizeF = new System.Drawing.SizeF(66.66666F, 19.99998F);
            this.Quantity.StylePriority.UseFont = false;
            this.Quantity.TextFormatString = "{0:#,#}";
            // 
            // ProductName
            // 
            this.ProductName.CanGrow = false;
            this.ProductName.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ProductName.LocationFloat = new DevExpress.Utils.PointFloat(77.08321F, 0F);
            this.ProductName.Name = "ProductName";
            this.ProductName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ProductName.SizeF = new System.Drawing.SizeF(221.9493F, 19.99998F);
            this.ProductName.StylePriority.UseFont = false;
            // 
            // SkuCode
            // 
            this.SkuCode.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SkuCode.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.SkuCode.Multiline = true;
            this.SkuCode.Name = "SkuCode";
            this.SkuCode.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SkuCode.SizeF = new System.Drawing.SizeF(77.08327F, 19.99998F);
            this.SkuCode.StylePriority.UseFont = false;
            // 
            // InvoiceId
            // 
            this.InvoiceId.Description = "Select Invoice";
            this.InvoiceId.Name = "InvoiceId";
            this.InvoiceId.Type = typeof(int);
            this.InvoiceId.ValueSourceSettings = staticListLookUpSettings1;
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ApplicationContext";
            this.sqlDataSource1.Name = "sqlDataSource1";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.InvoiceDate,
            this.InvoiceNumber,
            this.xrLabel14,
            this.xrLabel22,
            this.xrLine1,
            this.xrLabel19,
            this.xrLabel1,
            this.lblDate});
            this.ReportHeader.HeightF = 55.4584F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // InvoiceDate
            // 
            this.InvoiceDate.CanGrow = false;
            this.InvoiceDate.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvoiceDate.LocationFloat = new DevExpress.Utils.PointFloat(802.1243F, 18.83336F);
            this.InvoiceDate.Name = "InvoiceDate";
            this.InvoiceDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.InvoiceDate.SizeF = new System.Drawing.SizeF(221.9493F, 18.83334F);
            this.InvoiceDate.StylePriority.UseFont = false;
            this.InvoiceDate.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // InvoiceNumber
            // 
            this.InvoiceNumber.CanGrow = false;
            this.InvoiceNumber.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InvoiceNumber.LocationFloat = new DevExpress.Utils.PointFloat(802.1243F, 1.059638E-05F);
            this.InvoiceNumber.Name = "InvoiceNumber";
            this.InvoiceNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.InvoiceNumber.SizeF = new System.Drawing.SizeF(221.9493F, 18.83334F);
            this.InvoiceNumber.StylePriority.UseFont = false;
            // 
            // xrLabel14
            // 
            this.xrLabel14.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel14.ForeColor = System.Drawing.Color.Black;
            this.xrLabel14.LocationFloat = new DevExpress.Utils.PointFloat(703.6661F, 0F);
            this.xrLabel14.Name = "xrLabel14";
            this.xrLabel14.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel14.SizeF = new System.Drawing.SizeF(98.45819F, 18.83335F);
            this.xrLabel14.StylePriority.UseBackColor = false;
            this.xrLabel14.StylePriority.UseFont = false;
            this.xrLabel14.StylePriority.UseForeColor = false;
            this.xrLabel14.StylePriority.UseTextAlignment = false;
            this.xrLabel14.Text = "Invoice No:";
            this.xrLabel14.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel22
            // 
            this.xrLabel22.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel22.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel22.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(470.2131F, 0F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(169.1752F, 30.62501F);
            this.xrLabel22.StylePriority.UseBackColor = false;
            this.xrLabel22.StylePriority.UseBorders = false;
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.StylePriority.UseTextAlignment = false;
            this.xrLabel22.Text = "Invoice Detail Report";
            this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 37.66671F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(1125F, 12.99998F);
            // 
            // xrLabel19
            // 
            this.xrLabel19.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel19.ForeColor = System.Drawing.Color.Black;
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(697.4161F, 18.83334F);
            this.xrLabel19.Name = "xrLabel19";
            this.xrLabel19.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel19.SizeF = new System.Drawing.SizeF(104.708F, 18.83334F);
            this.xrLabel19.StylePriority.UseBackColor = false;
            this.xrLabel19.StylePriority.UseFont = false;
            this.xrLabel19.StylePriority.UseForeColor = false;
            this.xrLabel19.StylePriority.UseTextAlignment = false;
            this.xrLabel19.Text = "Invoice Date:";
            this.xrLabel19.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
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
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel17,
            this.xrLabel16,
            this.xrLabel15,
            this.xrLabel2,
            this.SKU,
            this.xrLabel7,
            this.xrLabel3,
            this.xrLabel4,
            this.ReOrder});
            this.PageHeader.HeightF = 18.09347F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel17
            // 
            this.xrLabel17.BorderWidth = 0F;
            this.xrLabel17.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel17.LocationFloat = new DevExpress.Utils.PointFloat(1018.482F, 0F);
            this.xrLabel17.Multiline = true;
            this.xrLabel17.Name = "xrLabel17";
            this.xrLabel17.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel17.SizeF = new System.Drawing.SizeF(106.5182F, 17.66666F);
            this.xrLabel17.StylePriority.UseBorderWidth = false;
            this.xrLabel17.StylePriority.UseFont = false;
            this.xrLabel17.Text = "Supplier Invoice No";
            // 
            // xrLabel16
            // 
            this.xrLabel16.BorderWidth = 0F;
            this.xrLabel16.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(842.7963F, 0F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(175.6853F, 17.66666F);
            this.xrLabel16.StylePriority.UseBorderWidth = false;
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.Text = "Supplier";
            // 
            // xrLabel15
            // 
            this.xrLabel15.BorderWidth = 0F;
            this.xrLabel15.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel15.LocationFloat = new DevExpress.Utils.PointFloat(748.1295F, 0F);
            this.xrLabel15.Multiline = true;
            this.xrLabel15.Name = "xrLabel15";
            this.xrLabel15.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel15.SizeF = new System.Drawing.SizeF(94.66675F, 17.66666F);
            this.xrLabel15.StylePriority.UseBorderWidth = false;
            this.xrLabel15.StylePriority.UseFont = false;
            this.xrLabel15.Text = "Purchase Order";
            // 
            // xrLabel2
            // 
            this.xrLabel2.BorderWidth = 0F;
            this.xrLabel2.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(365.6992F, 0F);
            this.xrLabel2.Multiline = true;
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(104.5139F, 17.66666F);
            this.xrLabel2.StylePriority.UseBorderWidth = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = "Pallet Number";
            // 
            // SKU
            // 
            this.SKU.BorderWidth = 0F;
            this.SKU.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SKU.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.SKU.Name = "SKU";
            this.SKU.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SKU.SizeF = new System.Drawing.SizeF(77.08321F, 17.66665F);
            this.SKU.StylePriority.UseBorderWidth = false;
            this.SKU.StylePriority.UseFont = false;
            this.SKU.Text = "SKU";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(77.08327F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(221.9492F, 17.66665F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Product";
            // 
            // xrLabel3
            // 
            this.xrLabel3.BorderWidth = 0F;
            this.xrLabel3.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(299.0325F, 0F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(66.66666F, 17.66666F);
            this.xrLabel3.StylePriority.UseBorderWidth = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Quantity";
            // 
            // xrLabel4
            // 
            this.xrLabel4.BorderWidth = 0F;
            this.xrLabel4.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(470.2131F, 0F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(92.04599F, 17.66665F);
            this.xrLabel4.StylePriority.UseBorderWidth = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Sale Order";
            // 
            // ReOrder
            // 
            this.ReOrder.BorderWidth = 0F;
            this.ReOrder.Font = new System.Drawing.Font("Arial Narrow", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReOrder.LocationFloat = new DevExpress.Utils.PointFloat(562.259F, 0F);
            this.ReOrder.Multiline = true;
            this.ReOrder.Name = "ReOrder";
            this.ReOrder.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ReOrder.SizeF = new System.Drawing.SizeF(185.8705F, 17.66666F);
            this.ReOrder.StylePriority.UseBorderWidth = false;
            this.ReOrder.StylePriority.UseFont = false;
            this.ReOrder.Text = "Delivery Note";
            // 
            // TenantID
            // 
            this.TenantID.Description = "tenant";
            this.TenantID.Name = "TenantID";
            this.TenantID.Type = typeof(int);
            this.TenantID.ValueInfo = "0";
            this.TenantID.Visible = false;
            // 
            // WarehouseId
            // 
            this.WarehouseId.Description = "Warehouse";
            this.WarehouseId.Name = "WarehouseId";
            this.WarehouseId.Type = typeof(int);
            this.WarehouseId.ValueInfo = "0";
            this.WarehouseId.Visible = false;
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 3.972202F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(350.4723F, 16.75F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.xrPageInfo1.TextFormatString = "Page {0} of {1}";
            // 
            // xrLine2
            // 
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(1125F, 3.972202F);
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine2,
            this.xrPageInfo1});
            this.PageFooter.HeightF = 21.32956F;
            this.PageFooter.Name = "PageFooter";
            // 
            // InvoiceDetailReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.ReportHeader,
            this.PageHeader,
            this.PageFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataSource = this.sqlDataSource1;
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(35, 9, 5, 10);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.InvoiceId,
            this.TenantID,
            this.WarehouseId});
            this.Version = "20.1";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.InvoiceDetailReport_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion

    private void InvoiceDetailReport_DataSourceDemanded(object sender, EventArgs e)
    {
    }
}

﻿using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

/// <summary>
/// Summary description for PickersOrdersReport
/// </summary>
public class PickersOrdersReport : DevExpress.XtraReports.UI.XtraReport
{
    private DevExpress.XtraReports.UI.DetailBand Detail;
    private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
    private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
    private DetailReportBand DetailReport;
    private DetailBand Detail1;
    private PageHeaderBand PageHeader;
    private XRLabel xrLabel5;
    private XRPanel xrPanel2;
    private XRLabel xrLabel10;
    private XRLabel xrLabel11;
    private XRLabel xrLabel21;
    private XRLabel xrLabel33;
    private XRLabel xrLabel29;
    private XRLabel xrLabel43;
    private GroupHeaderBand GroupHeader1;
    private XRLabel xrLabel30;
    private XRLabel xrLabel34;
    private XRLabel xrLabel35;
    private XRLabel xrLabel38;
    private XRLabel xrLabel39;
    private XRLabel xrLabel40;
    public XRLabel lblQuantity;
    private XRLabel xrLabel24;
    private XRLabel xrLabel23;
    private XRLabel xrLabel36;
    private XRLabel xrLabel37;
    private XRLabel xrLabel41;
    private PageFooterBand PageFooter;
    private XRLabel xrLabel4;
    private XRLabel xrLabel1;
    private XRLabel xrLabel2;
    private XRLabel xrLabel3;
    private XRLabel xrLabel7;
    private GroupFooterBand GroupFooter1;
    private XRLine xrLine2;
    public DevExpress.XtraReports.Parameters.Parameter paramDate;
    public DevExpress.XtraReports.Parameters.Parameter paramTenantId;
    public DevExpress.XtraReports.Parameters.Parameter paramAccountIds;
    public DevExpress.XtraReports.Parameters.Parameter paramProperty;
    private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
    public DevExpress.XtraReports.Parameters.Parameter paramPickerIds;
    private GroupHeaderBand GroupHeader2;
    private XRControlStyle xrControlStyle1;
    private XRLabel xrLabel8;
    private XRLabel xrLabel9;
    private XRPageInfo xrPageInfo1;

    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    public PickersOrdersReport()
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
            DevExpress.DataAccess.Sql.SelectQuery selectQuery1 = new DevExpress.DataAccess.Sql.SelectQuery();
            DevExpress.DataAccess.Sql.AllColumns allColumns1 = new DevExpress.DataAccess.Sql.AllColumns();
            DevExpress.DataAccess.Sql.Table table1 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.AllColumns allColumns2 = new DevExpress.DataAccess.Sql.AllColumns();
            DevExpress.DataAccess.Sql.Table table2 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.AllColumns allColumns3 = new DevExpress.DataAccess.Sql.AllColumns();
            DevExpress.DataAccess.Sql.Table table3 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.Column column1 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression1 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Table table4 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.Join join1 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.Join join2 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo2 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.Join join3 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo3 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PickersOrdersReport));
            DevExpress.DataAccess.Sql.MasterDetailInfo masterDetailInfo1 = new DevExpress.DataAccess.Sql.MasterDetailInfo();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo4 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings2 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings3 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel33 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel29 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel43 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.DetailReport = new DevExpress.XtraReports.UI.DetailReportBand();
            this.Detail1 = new DevExpress.XtraReports.UI.DetailBand();
            this.lblQuantity = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel24 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel23 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel36 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel37 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel41 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel30 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel34 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel35 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel38 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel39 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel40 = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupFooter1 = new DevExpress.XtraReports.UI.GroupFooterBand();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrPanel2 = new DevExpress.XtraReports.UI.XRPanel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.paramDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.paramTenantId = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramAccountIds = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramProperty = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramPickerIds = new DevExpress.XtraReports.Parameters.Parameter();
            this.GroupHeader2 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrControlStyle1 = new DevExpress.XtraReports.UI.XRControlStyle();
            this.xrPageInfo1 = new DevExpress.XtraReports.UI.XRPageInfo();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel2,
            this.xrLabel7,
            this.xrLabel21,
            this.xrLabel33});
            this.Detail.HeightF = 20.03206F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel2
            // 
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[AddressLine1]")});
            this.xrLabel2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(101.0132F, 0F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(349.7578F, 20.03206F);
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.StylePriority.UseTextAlignment = false;
            this.xrLabel2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel7
            // 
            this.xrLabel7.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[CompanyName]")});
            this.xrLabel7.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(450.7711F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(179.7104F, 20.03206F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.StylePriority.UseTextAlignment = false;
            this.xrLabel7.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel21
            // 
            this.xrLabel21.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderNumber]")});
            this.xrLabel21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel21.Name = "xrLabel21";
            this.xrLabel21.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel21.SizeF = new System.Drawing.SizeF(101.0132F, 20.03206F);
            this.xrLabel21.StylePriority.UseFont = false;
            this.xrLabel21.StylePriority.UseTextAlignment = false;
            this.xrLabel21.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel33
            // 
            this.xrLabel33.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[OrderTotal]")});
            this.xrLabel33.Font = new System.Drawing.Font("Verdana", 8.25F);
            this.xrLabel33.LocationFloat = new DevExpress.Utils.PointFloat(630.4814F, 0F);
            this.xrLabel33.Name = "xrLabel33";
            this.xrLabel33.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel33.SizeF = new System.Drawing.SizeF(116.5187F, 20.03206F);
            this.xrLabel33.StylePriority.UseFont = false;
            this.xrLabel33.StylePriority.UseTextAlignment = false;
            this.xrLabel33.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.Gainsboro;
            this.xrLabel1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(101.0132F, 58.65065F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(349.7577F, 21.47757F);
            this.xrLabel1.StylePriority.UseBackColor = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "PROPERTY:";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel3
            // 
            this.xrLabel3.BackColor = System.Drawing.Color.Gainsboro;
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(450.7708F, 58.65065F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(179.7106F, 21.47756F);
            this.xrLabel3.StylePriority.UseBackColor = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.StylePriority.UseTextAlignment = false;
            this.xrLabel3.Text = "ACCOUNT:";
            this.xrLabel3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel29
            // 
            this.xrLabel29.BackColor = System.Drawing.Color.Gainsboro;
            this.xrLabel29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel29.LocationFloat = new DevExpress.Utils.PointFloat(0F, 58.65065F);
            this.xrLabel29.Name = "xrLabel29";
            this.xrLabel29.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel29.SizeF = new System.Drawing.SizeF(101.0132F, 21.47757F);
            this.xrLabel29.StylePriority.UseBackColor = false;
            this.xrLabel29.StylePriority.UseFont = false;
            this.xrLabel29.StylePriority.UseTextAlignment = false;
            this.xrLabel29.Text = "ORDER NO:";
            this.xrLabel29.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel43
            // 
            this.xrLabel43.BackColor = System.Drawing.Color.Gainsboro;
            this.xrLabel43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel43.LocationFloat = new DevExpress.Utils.PointFloat(630.4814F, 58.65065F);
            this.xrLabel43.Name = "xrLabel43";
            this.xrLabel43.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel43.SizeF = new System.Drawing.SizeF(116.5184F, 21.47756F);
            this.xrLabel43.StylePriority.UseBackColor = false;
            this.xrLabel43.StylePriority.UseFont = false;
            this.xrLabel43.StylePriority.UseTextAlignment = false;
            this.xrLabel43.Text = "ORDER TOTAL:";
            this.xrLabel43.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 8.999999F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 7.480303F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // DetailReport
            // 
            this.DetailReport.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail1,
            this.GroupHeader1,
            this.GroupFooter1});
            this.DetailReport.DataMember = "OrderHeader.OrderHeaderOrderDetail";
            this.DetailReport.DataSource = this.sqlDataSource1;
            this.DetailReport.Level = 0;
            this.DetailReport.Name = "DetailReport";
            // 
            // Detail1
            // 
            this.Detail1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblQuantity,
            this.xrLabel24,
            this.xrLabel23,
            this.xrLabel36,
            this.xrLabel37,
            this.xrLabel41});
            this.Detail1.HeightF = 18.11919F;
            this.Detail1.Name = "Detail1";
            // 
            // lblQuantity
            // 
            this.lblQuantity.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.lblQuantity.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Qty]")});
            this.lblQuantity.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuantity.LocationFloat = new DevExpress.Utils.PointFloat(4.000508F, 0F);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblQuantity.SizeF = new System.Drawing.SizeF(100F, 17.08334F);
            this.lblQuantity.StylePriority.UseBorders = false;
            this.lblQuantity.StylePriority.UseFont = false;
            // 
            // xrLabel24
            // 
            this.xrLabel24.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel24.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SKUCode]")});
            this.xrLabel24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel24.LocationFloat = new DevExpress.Utils.PointFloat(104.0006F, 0F);
            this.xrLabel24.Name = "xrLabel24";
            this.xrLabel24.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel24.SizeF = new System.Drawing.SizeF(115.4661F, 17.08334F);
            this.xrLabel24.StylePriority.UseBorders = false;
            this.xrLabel24.StylePriority.UseFont = false;
            // 
            // xrLabel23
            // 
            this.xrLabel23.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel23.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Name]")});
            this.xrLabel23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel23.LocationFloat = new DevExpress.Utils.PointFloat(219.4667F, 0F);
            this.xrLabel23.Name = "xrLabel23";
            this.xrLabel23.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel23.SizeF = new System.Drawing.SizeF(213.5961F, 17.08334F);
            this.xrLabel23.StylePriority.UseBorders = false;
            this.xrLabel23.StylePriority.UseFont = false;
            // 
            // xrLabel36
            // 
            this.xrLabel36.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel36.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Price]")});
            this.xrLabel36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel36.LocationFloat = new DevExpress.Utils.PointFloat(433.0627F, 0F);
            this.xrLabel36.Name = "xrLabel36";
            this.xrLabel36.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel36.SizeF = new System.Drawing.SizeF(116.2974F, 17.08334F);
            this.xrLabel36.StylePriority.UseBorders = false;
            this.xrLabel36.StylePriority.UseFont = false;
            // 
            // xrLabel37
            // 
            this.xrLabel37.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel37.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TotalAmount]")});
            this.xrLabel37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel37.LocationFloat = new DevExpress.Utils.PointFloat(630.4814F, 0F);
            this.xrLabel37.Name = "xrLabel37";
            this.xrLabel37.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel37.SizeF = new System.Drawing.SizeF(116.5187F, 17.08334F);
            this.xrLabel37.StylePriority.UseBorders = false;
            this.xrLabel37.StylePriority.UseFont = false;
            // 
            // xrLabel41
            // 
            this.xrLabel41.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel41.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[TaxAmount]")});
            this.xrLabel41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel41.LocationFloat = new DevExpress.Utils.PointFloat(549.3602F, 0F);
            this.xrLabel41.Name = "xrLabel41";
            this.xrLabel41.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel41.SizeF = new System.Drawing.SizeF(81.12134F, 17.08334F);
            this.xrLabel41.StylePriority.UseBorders = false;
            this.xrLabel41.StylePriority.UseFont = false;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel30,
            this.xrLabel34,
            this.xrLabel35,
            this.xrLabel38,
            this.xrLabel39,
            this.xrLabel40});
            this.GroupHeader1.HeightF = 17.08333F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // xrLabel30
            // 
            this.xrLabel30.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel30.LocationFloat = new DevExpress.Utils.PointFloat(4.000473F, 0F);
            this.xrLabel30.Name = "xrLabel30";
            this.xrLabel30.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel30.SizeF = new System.Drawing.SizeF(100.0001F, 17.08333F);
            this.xrLabel30.StylePriority.UseFont = false;
            this.xrLabel30.Text = "QTY";
            // 
            // xrLabel34
            // 
            this.xrLabel34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel34.LocationFloat = new DevExpress.Utils.PointFloat(104.0006F, 0F);
            this.xrLabel34.Name = "xrLabel34";
            this.xrLabel34.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel34.SizeF = new System.Drawing.SizeF(115.4661F, 17.08333F);
            this.xrLabel34.StylePriority.UseFont = false;
            this.xrLabel34.Text = "SKU CODE";
            // 
            // xrLabel35
            // 
            this.xrLabel35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel35.LocationFloat = new DevExpress.Utils.PointFloat(219.4667F, 0F);
            this.xrLabel35.Name = "xrLabel35";
            this.xrLabel35.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel35.SizeF = new System.Drawing.SizeF(213.5961F, 17.08333F);
            this.xrLabel35.StylePriority.UseFont = false;
            this.xrLabel35.Text = "Name";
            // 
            // xrLabel38
            // 
            this.xrLabel38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel38.LocationFloat = new DevExpress.Utils.PointFloat(433.0627F, 0F);
            this.xrLabel38.Name = "xrLabel38";
            this.xrLabel38.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel38.SizeF = new System.Drawing.SizeF(116.2974F, 17.08333F);
            this.xrLabel38.StylePriority.UseFont = false;
            this.xrLabel38.Text = "Unit Price";
            // 
            // xrLabel39
            // 
            this.xrLabel39.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel39.LocationFloat = new DevExpress.Utils.PointFloat(630.4814F, 0F);
            this.xrLabel39.Name = "xrLabel39";
            this.xrLabel39.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel39.SizeF = new System.Drawing.SizeF(116.5184F, 17.08333F);
            this.xrLabel39.StylePriority.UseFont = false;
            this.xrLabel39.Text = "Total";
            // 
            // xrLabel40
            // 
            this.xrLabel40.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel40.LocationFloat = new DevExpress.Utils.PointFloat(549.3602F, 0F);
            this.xrLabel40.Name = "xrLabel40";
            this.xrLabel40.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel40.SizeF = new System.Drawing.SizeF(81.12128F, 17.08333F);
            this.xrLabel40.StylePriority.UseFont = false;
            this.xrLabel40.Text = "Tax";
            // 
            // GroupFooter1
            // 
            this.GroupFooter1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine2});
            this.GroupFooter1.HeightF = 23F;
            this.GroupFooter1.KeepTogether = true;
            this.GroupFooter1.Name = "GroupFooter1";
            // 
            // xrLine2
            // 
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(2.335642F, 0F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(744.6643F, 23F);
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ApplicationContext";
            this.sqlDataSource1.Name = "sqlDataSource1";
            table1.MetaSerializable = "<Meta X=\"30\" Y=\"30\" Width=\"125\" Height=\"1487\" />";
            table1.Name = "Orders";
            allColumns1.Table = table1;
            table2.MetaSerializable = "<Meta X=\"185\" Y=\"30\" Width=\"125\" Height=\"607\" />";
            table2.Name = "PProperties";
            allColumns2.Table = table2;
            table3.MetaSerializable = "<Meta X=\"340\" Y=\"30\" Width=\"125\" Height=\"747\" />";
            table3.Name = "Account";
            allColumns3.Table = table3;
            columnExpression1.ColumnName = "UserFirstName";
            table4.MetaSerializable = "<Meta X=\"495\" Y=\"30\" Width=\"125\" Height=\"567\" />";
            table4.Name = "AuthUsers";
            columnExpression1.Table = table4;
            column1.Expression = columnExpression1;
            selectQuery1.Columns.Add(allColumns1);
            selectQuery1.Columns.Add(allColumns2);
            selectQuery1.Columns.Add(allColumns3);
            selectQuery1.Columns.Add(column1);
            selectQuery1.FilterString = "[Orders.ExpectedDate] = ?SqlParamDate And [Orders.TenentId] = ?SqlParamTenantId A" +
    "nd [Orders.PickerId] In (?SqlParamPickerIds) And ([Orders.IsDeleted] = False OR " +
    "[Orders.IsDeleted] IS NULL)";
            selectQuery1.GroupFilterString = "";
            selectQuery1.MetaSerializable = "<Meta X=\"20\" Y=\"20\" Width=\"290\" Height=\"1414\" />";
            selectQuery1.Name = "OrderHeader";
            queryParameter1.Name = "SqlParamDate";
            queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("[Parameters.paramDate]", typeof(System.DateTime));
            queryParameter2.Name = "SqlParamTenantId";
            queryParameter2.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("[Parameters.paramTenantId]", typeof(int));
            queryParameter3.Name = "SqlParamTransactionTypes";
            queryParameter3.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter3.Value = new DevExpress.DataAccess.Expression("[Parameters.paramTransactionTypes]", typeof(int));
            queryParameter4.Name = "SqlParamPickerIds";
            queryParameter4.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("[Parameters.paramPickerIds]", typeof(int));
            selectQuery1.Parameters.Add(queryParameter1);
            selectQuery1.Parameters.Add(queryParameter2);
            selectQuery1.Parameters.Add(queryParameter3);
            selectQuery1.Parameters.Add(queryParameter4);
            relationColumnInfo1.NestedKeyColumn = "PPropertyId";
            relationColumnInfo1.ParentKeyColumn = "ShipmentPropertyId";
            join1.KeyColumns.Add(relationColumnInfo1);
            join1.Nested = table2;
            join1.Parent = table1;
            join1.Type = DevExpress.Xpo.DB.JoinType.LeftOuter;
            relationColumnInfo2.NestedKeyColumn = "AccountID";
            relationColumnInfo2.ParentKeyColumn = "AccountID";
            join2.KeyColumns.Add(relationColumnInfo2);
            join2.Nested = table3;
            join2.Parent = table1;
            join2.Type = DevExpress.Xpo.DB.JoinType.LeftOuter;
            relationColumnInfo3.NestedKeyColumn = "UserId";
            relationColumnInfo3.ParentKeyColumn = "PickerId";
            join3.KeyColumns.Add(relationColumnInfo3);
            join3.Nested = table4;
            join3.Parent = table1;
            join3.Type = DevExpress.Xpo.DB.JoinType.LeftOuter;
            selectQuery1.Relations.Add(join1);
            selectQuery1.Relations.Add(join2);
            selectQuery1.Relations.Add(join3);
            selectQuery1.Tables.Add(table1);
            selectQuery1.Tables.Add(table2);
            selectQuery1.Tables.Add(table3);
            selectQuery1.Tables.Add(table4);
            customSqlQuery1.Name = "OrderDetail";
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            selectQuery1,
            customSqlQuery1});
            masterDetailInfo1.DetailQueryName = "OrderDetail";
            relationColumnInfo4.NestedKeyColumn = "OrderID";
            relationColumnInfo4.ParentKeyColumn = "OrderID";
            masterDetailInfo1.KeyColumns.Add(relationColumnInfo4);
            masterDetailInfo1.MasterQueryName = "OrderHeader";
            this.sqlDataSource1.Relations.AddRange(new DevExpress.DataAccess.Sql.MasterDetailInfo[] {
            masterDetailInfo1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel43,
            this.xrLabel3,
            this.xrLabel29,
            this.xrLabel1,
            this.xrLabel5,
            this.xrPanel2});
            this.PageHeader.HeightF = 86.75323F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel5
            // 
            this.xrLabel5.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(3.999964F, 0F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(743.0002F, 30.62501F);
            this.xrLabel5.StylePriority.UseBackColor = false;
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Pickers Assigned Orders Report";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrPanel2
            // 
            this.xrPanel2.BackColor = System.Drawing.Color.Transparent;
            this.xrPanel2.BorderColor = System.Drawing.Color.Black;
            this.xrPanel2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel10,
            this.xrLabel11});
            this.xrPanel2.LocationFloat = new DevExpress.Utils.PointFloat(3.999964F, 30.62502F);
            this.xrPanel2.Name = "xrPanel2";
            this.xrPanel2.SizeF = new System.Drawing.SizeF(743.0002F, 22.85416F);
            this.xrPanel2.StylePriority.UseBackColor = false;
            this.xrPanel2.StylePriority.UseBorderColor = false;
            // 
            // xrLabel10
            // 
            this.xrLabel10.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel10.ForeColor = System.Drawing.Color.Black;
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(232.6575F, 5.722046E-06F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(107.973F, 18.83335F);
            this.xrLabel10.StylePriority.UseBackColor = false;
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.StylePriority.UseForeColor = false;
            this.xrLabel10.StylePriority.UseTextAlignment = false;
            this.xrLabel10.Text = "Report Date :";
            this.xrLabel10.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel11
            // 
            this.xrLabel11.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Parameters.paramDate]")});
            this.xrLabel11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel11.ForeColor = System.Drawing.Color.Black;
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(340.6305F, 0F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(156.7502F, 18.83335F);
            this.xrLabel11.StylePriority.UseBackColor = false;
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.StylePriority.UseForeColor = false;
            this.xrLabel11.StylePriority.UseTextAlignment = false;
            this.xrLabel11.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrLabel11.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // paramDate
            // 
            this.paramDate.Description = "Date";
            this.paramDate.Name = "paramDate";
            this.paramDate.Type = typeof(System.DateTime);
            this.paramDate.ValueInfo = "2020-09-29";
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrPageInfo1,
            this.xrLabel4});
            this.PageFooter.HeightF = 35.41667F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLabel4
            // 
            this.xrLabel4.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel4.Borders = DevExpress.XtraPrinting.BorderSide.Top;
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(2.706041F, 1.958361F);
            this.xrLabel4.Multiline = true;
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(744.2939F, 8.041669F);
            this.xrLabel4.StylePriority.UseBackColor = false;
            this.xrLabel4.StylePriority.UseBorders = false;
            this.xrLabel4.StylePriority.UseTextAlignment = false;
            this.xrLabel4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // paramTenantId
            // 
            this.paramTenantId.Name = "paramTenantId";
            this.paramTenantId.Type = typeof(int);
            this.paramTenantId.ValueInfo = "0";
            this.paramTenantId.Visible = false;
            // 
            // paramAccountIds
            // 
            this.paramAccountIds.Description = "Select Account";
            this.paramAccountIds.MultiValue = true;
            this.paramAccountIds.Name = "paramAccountIds";
            this.paramAccountIds.Type = typeof(int);
            this.paramAccountIds.ValueSourceSettings = staticListLookUpSettings1;
            this.paramAccountIds.Visible = false;
            // 
            // paramProperty
            // 
            this.paramProperty.Description = "Property";
            this.paramProperty.MultiValue = true;
            this.paramProperty.Name = "paramProperty";
            this.paramProperty.Type = typeof(int);
            this.paramProperty.ValueSourceSettings = staticListLookUpSettings2;
            this.paramProperty.Visible = false;
            // 
            // paramPickerIds
            // 
            this.paramPickerIds.Description = "Picker";
            this.paramPickerIds.MultiValue = true;
            this.paramPickerIds.Name = "paramPickerIds";
            this.paramPickerIds.Type = typeof(int);
            this.paramPickerIds.ValueSourceSettings = staticListLookUpSettings3;
            // 
            // GroupHeader2
            // 
            this.GroupHeader2.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel9,
            this.xrLabel8});
            this.GroupHeader2.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupHeader2.GroupFields.AddRange(new DevExpress.XtraReports.UI.GroupField[] {
            new DevExpress.XtraReports.UI.GroupField("PickerId", DevExpress.XtraReports.UI.XRColumnSortOrder.Ascending)});
            this.GroupHeader2.HeightF = 33.65695F;
            this.GroupHeader2.Name = "GroupHeader2";
            this.GroupHeader2.Padding = new DevExpress.XtraPrinting.PaddingInfo(5, 5, 5, 5, 100F);
            this.GroupHeader2.PageBreak = DevExpress.XtraReports.UI.PageBreak.BeforeBandExceptFirstEntry;
            this.GroupHeader2.StyleName = "xrControlStyle1";
            this.GroupHeader2.StylePriority.UseFont = false;
            this.GroupHeader2.StylePriority.UsePadding = false;
            // 
            // xrLabel9
            // 
            this.xrLabel9.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel9.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel9.ForeColor = System.Drawing.Color.Black;
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(9.999974F, 0F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(91.01321F, 23.65694F);
            this.xrLabel9.StylePriority.UseBackColor = false;
            this.xrLabel9.StylePriority.UseBorders = false;
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.StylePriority.UseForeColor = false;
            this.xrLabel9.StylePriority.UseTextAlignment = false;
            this.xrLabel9.Text = "Picker Name:";
            this.xrLabel9.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[UserFirstName]")});
            this.xrLabel8.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(100, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(746.9997F, 23.65694F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.StylePriority.UsePadding = false;
            this.xrLabel8.StylePriority.UseTextAlignment = false;
            this.xrLabel8.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // xrControlStyle1
            // 
            this.xrControlStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.xrControlStyle1.BorderColor = System.Drawing.Color.Silver;
            this.xrControlStyle1.Borders = ((DevExpress.XtraPrinting.BorderSide)((((DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Top) 
            | DevExpress.XtraPrinting.BorderSide.Right) 
            | DevExpress.XtraPrinting.BorderSide.Bottom)));
            this.xrControlStyle1.BorderWidth = 1F;
            this.xrControlStyle1.Name = "xrControlStyle1";
            // 
            // xrPageInfo1
            // 
            this.xrPageInfo1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrPageInfo1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 10.00004F);
            this.xrPageInfo1.Name = "xrPageInfo1";
            this.xrPageInfo1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrPageInfo1.SizeF = new System.Drawing.SizeF(746.9996F, 23F);
            this.xrPageInfo1.StylePriority.UseFont = false;
            this.xrPageInfo1.StylePriority.UseTextAlignment = false;
            this.xrPageInfo1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.xrPageInfo1.TextFormatString = "Page {0} of {1}";
            // 
            // PickersOrdersReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.DetailReport,
            this.PageHeader,
            this.PageFooter,
            this.GroupHeader2});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "OrderHeader";
            this.DataSource = this.sqlDataSource1;
            this.Margins = new System.Drawing.Printing.Margins(51, 52, 9, 7);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramDate,
            this.paramTenantId,
            this.paramAccountIds,
            this.paramProperty,
            this.paramPickerIds});
            this.StyleSheet.AddRange(new DevExpress.XtraReports.UI.XRControlStyle[] {
            this.xrControlStyle1});
            this.Version = "20.1";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.PickersOrdersReport_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

    }

    #endregion

    private void PickersOrdersReport_DataSourceDemanded(object sender, EventArgs e)
    {
        var report = (PickersOrdersReport)sender;
        var date = (DateTime)report.Parameters["paramDate"].Value;
        report.Parameters["paramDate"].Value = date.Date;

    }
}

namespace WMS.Reports.Designs
{
    partial class ProductSoldBySkuPrint
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
            this.components = new System.ComponentModel.Container();
            DevExpress.DataAccess.Sql.CustomSqlQuery customSqlQuery1 = new DevExpress.DataAccess.Sql.CustomSqlQuery();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter3 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter4 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter5 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter6 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter7 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter8 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter9 = new DevExpress.DataAccess.Sql.QueryParameter();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProductSoldBySkuPrint));
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings2 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings3 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings4 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings5 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel11 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel10 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.lblOwner = new DevExpress.XtraReports.UI.XRLabel();
            this.lblAcount = new DevExpress.XtraReports.UI.XRLabel();
            this.username = new DevExpress.XtraReports.UI.XRLabel();
            this.ComanyName = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.lbldate = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel22 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel19 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel20 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel21 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel9 = new DevExpress.XtraReports.UI.XRLabel();
            this.SKU = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.ReOrder = new DevExpress.XtraReports.UI.XRLabel();
            this.StartDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.EndDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLabel12 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductsIds = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramProductGroups = new DevExpress.XtraReports.Parameters.Parameter();
            this.TenantId = new DevExpress.XtraReports.Parameters.Parameter();
            this.WarehouseId = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramProductDepartment = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramOwnerID = new DevExpress.XtraReports.Parameters.Parameter();
            this.AccountId = new DevExpress.XtraReports.Parameters.Parameter();
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
            this.xrLabel11,
            this.xrLabel10,
            this.xrLabel2,
            this.xrLabel3,
            this.xrLabel6});
            this.Detail.HeightF = 25.20851F;
            this.Detail.Name = "Detail";
            // 
            // xrLabel11
            // 
            this.xrLabel11.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Query].[SELLINGPRICE]")});
            this.xrLabel11.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel11.LocationFloat = new DevExpress.Utils.PointFloat(712.8333F, 0F);
            this.xrLabel11.Name = "xrLabel11";
            this.xrLabel11.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel11.SizeF = new System.Drawing.SizeF(88.16656F, 23F);
            this.xrLabel11.StylePriority.UseFont = false;
            this.xrLabel11.Text = "Sales Price";
            // 
            // xrLabel10
            // 
            this.xrLabel10.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Query].[BUYINGPRICE]")});
            this.xrLabel10.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel10.LocationFloat = new DevExpress.Utils.PointFloat(625.2913F, 0F);
            this.xrLabel10.Name = "xrLabel10";
            this.xrLabel10.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel10.SizeF = new System.Drawing.SizeF(87.54193F, 23F);
            this.xrLabel10.StylePriority.UseFont = false;
            this.xrLabel10.Text = "Cost Price";
            // 
            // xrLabel2
            // 
            this.xrLabel2.BorderWidth = 0F;
            this.xrLabel2.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Query].[SKUCODE]")});
            this.xrLabel2.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(162.4999F, 23F);
            this.xrLabel2.StylePriority.UseBorderWidth = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = "SKU";
            // 
            // xrLabel3
            // 
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Query].[NAME]")});
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(162.4999F, 0F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(364.5416F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Product";
            // 
            // xrLabel6
            // 
            this.xrLabel6.BorderWidth = 0F;
            this.xrLabel6.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([ShowCaseQtyInReports]==1,([Query].[QTYOUT]/(Iif(IsNullOrEmpty([ProductsPerCa" +
                    "se]),1,[ProductsPerCase]))), [Query].[QTYOUT])")});
            this.xrLabel6.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(527.0415F, 0F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(98.24982F, 23F);
            this.xrLabel6.StylePriority.UseBorderWidth = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "OUT";
            this.xrLabel6.TextFormatString = "{0:#.00}";
            // 
            // ReportHeader
            // 
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblOwner,
            this.lblAcount,
            this.username,
            this.ComanyName,
            this.xrLabel5,
            this.lbldate,
            this.xrLabel22,
            this.xrLabel19,
            this.xrLabel20,
            this.xrLabel21,
            this.xrLine1});
            this.ReportHeader.HeightF = 115.0834F;
            this.ReportHeader.Name = "ReportHeader";
            // 
            // lblOwner
            // 
            this.lblOwner.BackColor = System.Drawing.Color.Transparent;
            this.lblOwner.CanShrink = true;
            this.lblOwner.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOwner.ForeColor = System.Drawing.Color.Black;
            this.lblOwner.LocationFloat = new DevExpress.Utils.PointFloat(477.9168F, 82.08334F);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblOwner.SizeF = new System.Drawing.SizeF(76.58322F, 18.83335F);
            this.lblOwner.StylePriority.UseBackColor = false;
            this.lblOwner.StylePriority.UseFont = false;
            this.lblOwner.StylePriority.UseForeColor = false;
            this.lblOwner.StylePriority.UseTextAlignment = false;
            this.lblOwner.Text = "Owner:";
            this.lblOwner.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblOwner.Visible = false;
            // 
            // lblAcount
            // 
            this.lblAcount.BackColor = System.Drawing.Color.Transparent;
            this.lblAcount.CanShrink = true;
            this.lblAcount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcount.ForeColor = System.Drawing.Color.Black;
            this.lblAcount.LocationFloat = new DevExpress.Utils.PointFloat(45.79198F, 82.08329F);
            this.lblAcount.Name = "lblAcount";
            this.lblAcount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblAcount.SizeF = new System.Drawing.SizeF(123.458F, 18.83335F);
            this.lblAcount.StylePriority.UseBackColor = false;
            this.lblAcount.StylePriority.UseFont = false;
            this.lblAcount.StylePriority.UseForeColor = false;
            this.lblAcount.StylePriority.UseTextAlignment = false;
            this.lblAcount.Text = "Account:";
            this.lblAcount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            this.lblAcount.Visible = false;
            // 
            // username
            // 
            this.username.CanShrink = true;
            this.username.LocationFloat = new DevExpress.Utils.PointFloat(554.5F, 82.08329F);
            this.username.Multiline = true;
            this.username.Name = "username";
            this.username.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.username.SizeF = new System.Drawing.SizeF(158.3333F, 18.83335F);
            this.username.StylePriority.UseTextAlignment = false;
            this.username.Text = "UserName";
            this.username.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.username.Visible = false;
            // 
            // ComanyName
            // 
            this.ComanyName.CanShrink = true;
            this.ComanyName.LocationFloat = new DevExpress.Utils.PointFloat(169.25F, 82.08329F);
            this.ComanyName.Multiline = true;
            this.ComanyName.Name = "ComanyName";
            this.ComanyName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ComanyName.SizeF = new System.Drawing.SizeF(281.2083F, 18.83335F);
            this.ComanyName.StylePriority.UseTextAlignment = false;
            this.ComanyName.Text = "CompanyName";
            this.ComanyName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.ComanyName.Visible = false;
            // 
            // xrLabel5
            // 
            this.xrLabel5.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel5.ForeColor = System.Drawing.Color.Black;
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(54.16679F, 18.83335F);
            this.xrLabel5.StylePriority.UseBackColor = false;
            this.xrLabel5.StylePriority.UseFont = false;
            this.xrLabel5.StylePriority.UseForeColor = false;
            this.xrLabel5.StylePriority.UseTextAlignment = false;
            this.xrLabel5.Text = "Date:";
            this.xrLabel5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            // 
            // lbldate
            // 
            this.lbldate.BackColor = System.Drawing.Color.Transparent;
            this.lbldate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbldate.ForeColor = System.Drawing.Color.Black;
            this.lbldate.LocationFloat = new DevExpress.Utils.PointFloat(54.16679F, 0F);
            this.lbldate.Name = "lbldate";
            this.lbldate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lbldate.SizeF = new System.Drawing.SizeF(177.0833F, 18.83335F);
            this.lbldate.StylePriority.UseBackColor = false;
            this.lbldate.StylePriority.UseFont = false;
            this.lbldate.StylePriority.UseForeColor = false;
            this.lbldate.StylePriority.UseTextAlignment = false;
            this.lbldate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleLeft;
            this.lbldate.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // xrLabel22
            // 
            this.xrLabel22.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel22.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel22.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel22.LocationFloat = new DevExpress.Utils.PointFloat(0.5000916F, 18.83335F);
            this.xrLabel22.Name = "xrLabel22";
            this.xrLabel22.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel22.SizeF = new System.Drawing.SizeF(800.9998F, 30.62501F);
            this.xrLabel22.StylePriority.UseBackColor = false;
            this.xrLabel22.StylePriority.UseBorders = false;
            this.xrLabel22.StylePriority.UseFont = false;
            this.xrLabel22.StylePriority.UseTextAlignment = false;
            this.xrLabel22.Text = "Product Sale By Sku";
            this.xrLabel22.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel19
            // 
            this.xrLabel19.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel19.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel19.ForeColor = System.Drawing.Color.Black;
            this.xrLabel19.LocationFloat = new DevExpress.Utils.PointFloat(169.25F, 54.79179F);
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
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "?StartDate")});
            this.xrLabel20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel20.ForeColor = System.Drawing.Color.Black;
            this.xrLabel20.LocationFloat = new DevExpress.Utils.PointFloat(293.7081F, 54.79179F);
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
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "AddSeconds(?EndDate,-1)")});
            this.xrLabel21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel21.ForeColor = System.Drawing.Color.Black;
            this.xrLabel21.LocationFloat = new DevExpress.Utils.PointFloat(474.4166F, 54.79179F);
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
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(1.000122F, 102.0834F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(800.9998F, 12.99999F);
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8,
            this.xrLabel9,
            this.SKU,
            this.xrLabel7,
            this.ReOrder});
            this.PageHeader.HeightF = 27.20827F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel8
            // 
            this.xrLabel8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(625.2913F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(87.54193F, 23F);
            this.xrLabel8.StylePriority.UseFont = false;
            this.xrLabel8.Text = "Buy Price";
            // 
            // xrLabel9
            // 
            this.xrLabel9.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel9.LocationFloat = new DevExpress.Utils.PointFloat(712.8333F, 0F);
            this.xrLabel9.Name = "xrLabel9";
            this.xrLabel9.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel9.SizeF = new System.Drawing.SizeF(88.16656F, 23F);
            this.xrLabel9.StylePriority.UseFont = false;
            this.xrLabel9.Text = "Sale Price";
            // 
            // SKU
            // 
            this.SKU.BorderWidth = 0F;
            this.SKU.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SKU.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.SKU.Name = "SKU";
            this.SKU.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SKU.SizeF = new System.Drawing.SizeF(162.4999F, 23F);
            this.SKU.StylePriority.UseBorderWidth = false;
            this.SKU.StylePriority.UseFont = false;
            this.SKU.Text = "SKU";
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(162.4999F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(364.5416F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Product";
            // 
            // ReOrder
            // 
            this.ReOrder.BorderWidth = 0F;
            this.ReOrder.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Iif([ShowCaseQtyInReports]==1,\'Cases\',\'Qty\')")});
            this.ReOrder.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReOrder.LocationFloat = new DevExpress.Utils.PointFloat(527.0415F, 0F);
            this.ReOrder.Multiline = true;
            this.ReOrder.Name = "ReOrder";
            this.ReOrder.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ReOrder.SizeF = new System.Drawing.SizeF(98.24982F, 23F);
            this.ReOrder.StylePriority.UseBorderWidth = false;
            this.ReOrder.StylePriority.UseFont = false;
            this.ReOrder.Text = "Qty";
            // 
            // StartDate
            // 
            this.StartDate.Description = "Start Date";
            this.StartDate.Name = "StartDate";
            this.StartDate.Type = typeof(System.DateTime);
            // 
            // EndDate
            // 
            this.EndDate.Description = "End Date";
            this.EndDate.Name = "EndDate";
            this.EndDate.Type = typeof(System.DateTime);
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ApplicationContext";
            this.sqlDataSource1.Name = "sqlDataSource1";
            customSqlQuery1.Name = "Query";
            queryParameter1.Name = "sqlStartDate";
            queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?StartDate", typeof(System.DateTime));
            queryParameter2.Name = "sqlEndDate";
            queryParameter2.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("?EndDate", typeof(int));
            queryParameter3.Name = "sqlProductIds";
            queryParameter3.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter3.Value = new DevExpress.DataAccess.Expression("JOIN(?ProductsIds)", typeof(string));
            queryParameter4.Name = "sqlProductGroups";
            queryParameter4.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter4.Value = new DevExpress.DataAccess.Expression("JOIN(?paramProductGroups)", typeof(string));
            queryParameter5.Name = "sqlTenantId";
            queryParameter5.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter5.Value = new DevExpress.DataAccess.Expression("?TenantId", typeof(int));
            queryParameter6.Name = "sqlWarehouseId";
            queryParameter6.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter6.Value = new DevExpress.DataAccess.Expression("?WarehouseId", typeof(int));
            queryParameter7.Name = "sqlProductDept";
            queryParameter7.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter7.Value = new DevExpress.DataAccess.Expression("JOIN(?paramProductDepartment)", typeof(string));
            queryParameter8.Name = "sqlAccountId";
            queryParameter8.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter8.Value = new DevExpress.DataAccess.Expression("?AccountId", typeof(int));
            queryParameter9.Name = "sqlOwnerId";
            queryParameter9.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter9.Value = new DevExpress.DataAccess.Expression("?paramOwnerID", typeof(int));
            customSqlQuery1.Parameters.Add(queryParameter1);
            customSqlQuery1.Parameters.Add(queryParameter2);
            customSqlQuery1.Parameters.Add(queryParameter3);
            customSqlQuery1.Parameters.Add(queryParameter4);
            customSqlQuery1.Parameters.Add(queryParameter5);
            customSqlQuery1.Parameters.Add(queryParameter6);
            customSqlQuery1.Parameters.Add(queryParameter7);
            customSqlQuery1.Parameters.Add(queryParameter8);
            customSqlQuery1.Parameters.Add(queryParameter9);
            customSqlQuery1.Sql = resources.GetString("customSqlQuery1.Sql");
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            customSqlQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel12});
            this.ReportFooter.HeightF = 26.04167F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLabel12
            // 
            this.xrLabel12.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel12.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel12.BorderWidth = 0F;
            this.xrLabel12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel12.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel12.Multiline = true;
            this.xrLabel12.Name = "xrLabel12";
            this.xrLabel12.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel12.SizeF = new System.Drawing.SizeF(800.9999F, 23.33333F);
            this.xrLabel12.StylePriority.UseBackColor = false;
            this.xrLabel12.StylePriority.UseBorders = false;
            this.xrLabel12.StylePriority.UseBorderWidth = false;
            this.xrLabel12.StylePriority.UseFont = false;
            this.xrLabel12.StylePriority.UseTextAlignment = false;
            this.xrLabel12.Text = "©WarehouseZone\r\n\r\n";
            this.xrLabel12.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // ProductsIds
            // 
            this.ProductsIds.AllowNull = true;
            this.ProductsIds.Description = "Select Products";
            this.ProductsIds.MultiValue = true;
            this.ProductsIds.Name = "ProductsIds";
            this.ProductsIds.Type = typeof(int);
            this.ProductsIds.ValueSourceSettings = staticListLookUpSettings1;
            // 
            // paramProductGroups
            // 
            this.paramProductGroups.AllowNull = true;
            this.paramProductGroups.Description = "Select Groups";
            this.paramProductGroups.MultiValue = true;
            this.paramProductGroups.Name = "paramProductGroups";
            this.paramProductGroups.Type = typeof(int);
            this.paramProductGroups.ValueSourceSettings = staticListLookUpSettings2;
            // 
            // TenantId
            // 
            this.TenantId.Description = "Tenant";
            this.TenantId.Name = "TenantId";
            this.TenantId.Type = typeof(int);
            this.TenantId.ValueInfo = "0";
            this.TenantId.Visible = false;
            // 
            // WarehouseId
            // 
            this.WarehouseId.Description = "warehouse";
            this.WarehouseId.Name = "WarehouseId";
            this.WarehouseId.Type = typeof(int);
            this.WarehouseId.ValueInfo = "0";
            this.WarehouseId.Visible = false;
            // 
            // paramProductDepartment
            // 
            this.paramProductDepartment.AllowNull = true;
            this.paramProductDepartment.Description = "Select Department";
            this.paramProductDepartment.MultiValue = true;
            this.paramProductDepartment.Name = "paramProductDepartment";
            this.paramProductDepartment.Type = typeof(int);
            this.paramProductDepartment.ValueSourceSettings = staticListLookUpSettings3;
            // 
            // paramOwnerID
            // 
            this.paramOwnerID.AllowNull = true;
            this.paramOwnerID.Description = "Owner";
            this.paramOwnerID.Name = "paramOwnerID";
            this.paramOwnerID.Type = typeof(int);
            this.paramOwnerID.ValueSourceSettings = staticListLookUpSettings4;
            // 
            // AccountId
            // 
            this.AccountId.AllowNull = true;
            this.AccountId.Description = "Account";
            this.AccountId.Name = "AccountId";
            this.AccountId.Type = typeof(int);
            this.AccountId.ValueSourceSettings = staticListLookUpSettings5;
            // 
            // ProductSoldBySkuPrint
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.ReportHeader,
            this.PageHeader,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "Query";
            this.DataSource = this.sqlDataSource1;
            this.FilterString = "[PRODUCTID] In (?ProductsIds) Or [ProductGroupId] In (?paramProductGroups) Or [De" +
    "partmentId] In (?paramProductDepartment)";
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(22, 25, 0, 0);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.StartDate,
            this.EndDate,
            this.ProductsIds,
            this.paramProductGroups,
            this.TenantId,
            this.WarehouseId,
            this.paramProductDepartment,
            this.AccountId,
            this.paramOwnerID});
            this.Version = "19.2";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.ProductSoldBySkuPrint_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        private DevExpress.XtraReports.UI.XRLabel SKU;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.XtraReports.UI.XRLabel ReOrder;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel9;
        public DevExpress.XtraReports.Parameters.Parameter StartDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel10;
        private DevExpress.XtraReports.UI.XRLabel xrLabel11;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel12;
        public DevExpress.XtraReports.Parameters.Parameter EndDate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        public DevExpress.XtraReports.UI.XRLabel lbldate;
        private DevExpress.XtraReports.UI.XRLabel xrLabel22;
        private DevExpress.XtraReports.UI.XRLabel xrLabel19;
        private DevExpress.XtraReports.UI.XRLabel xrLabel20;
        private DevExpress.XtraReports.UI.XRLabel xrLabel21;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.Parameters.Parameter ProductsIds;
        public DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        public DevExpress.XtraReports.Parameters.Parameter paramProductGroups;
        public DevExpress.XtraReports.Parameters.Parameter TenantId;
        public DevExpress.XtraReports.Parameters.Parameter WarehouseId;
        public DevExpress.XtraReports.Parameters.Parameter paramProductDepartment;
        public DevExpress.XtraReports.Parameters.Parameter paramOwnerID;
        public DevExpress.XtraReports.Parameters.Parameter AccountId;
        public DevExpress.XtraReports.UI.XRLabel username;
        public DevExpress.XtraReports.UI.XRLabel ComanyName;
        public DevExpress.XtraReports.UI.XRLabel lblOwner;
        public DevExpress.XtraReports.UI.XRLabel lblAcount;
    }
}

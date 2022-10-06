namespace WMS.Reports.Designs
{
    partial class StockValueDetailReport
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
            DevExpress.DataAccess.Sql.MasterDetailInfo masterDetailInfo1 = new DevExpress.DataAccess.Sql.MasterDetailInfo();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.Cases = new DevExpress.XtraReports.UI.XRLabel();
            this.BuyPrice = new DevExpress.XtraReports.UI.XRLabel();
            this.OrderNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.Pallet = new DevExpress.XtraReports.UI.XRLabel();
            this.ExpectedDate = new DevExpress.XtraReports.UI.XRLabel();
            this.TotalAmount = new DevExpress.XtraReports.UI.XRLabel();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.paramsTenantId = new DevExpress.XtraReports.Parameters.Parameter();
            this.paramWarehouseId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.paramProductId = new DevExpress.XtraReports.Parameters.Parameter();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ApplicationContext";
            this.sqlDataSource1.Name = "sqlDataSource1";
            masterDetailInfo1.DetailQueryName = "OrderDetails";
            relationColumnInfo1.NestedKeyColumn = "ProductId";
            relationColumnInfo1.ParentKeyColumn = "PRODUCTID";
            masterDetailInfo1.KeyColumns.Add(relationColumnInfo1);
            masterDetailInfo1.MasterQueryName = "Products";
            this.sqlDataSource1.Relations.AddRange(new DevExpress.DataAccess.Sql.MasterDetailInfo[] {
            masterDetailInfo1});
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
            this.Detail.BorderColor = System.Drawing.Color.Silver;
            this.Detail.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.Cases,
            this.BuyPrice,
            this.OrderNumber,
            this.Pallet,
            this.ExpectedDate,
            this.TotalAmount});
            this.Detail.HeightF = 19.70872F;
            this.Detail.Name = "Detail";
            this.Detail.StylePriority.UseBorderColor = false;
            this.Detail.StylePriority.UseBorders = false;
            // 
            // Cases
            // 
            this.Cases.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cases.LocationFloat = new DevExpress.Utils.PointFloat(369.375F, 0F);
            this.Cases.Multiline = true;
            this.Cases.Name = "Cases";
            this.Cases.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Cases.SizeF = new System.Drawing.SizeF(74.38846F, 18.83333F);
            this.Cases.StylePriority.UseFont = false;
            this.Cases.TextFormatString = "{0:d}";
            // 
            // BuyPrice
            // 
            this.BuyPrice.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuyPrice.LocationFloat = new DevExpress.Utils.PointFloat(570.4999F, 0F);
            this.BuyPrice.Multiline = true;
            this.BuyPrice.Name = "BuyPrice";
            this.BuyPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.BuyPrice.SizeF = new System.Drawing.SizeF(75.52777F, 18.83333F);
            this.BuyPrice.StylePriority.UseFont = false;
            this.BuyPrice.StylePriority.UseTextAlignment = false;
            this.BuyPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.BuyPrice.TextFormatString = "{0:#.00}";
            // 
            // OrderNumber
            // 
            this.OrderNumber.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OrderNumber.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 0F);
            this.OrderNumber.Multiline = true;
            this.OrderNumber.Name = "OrderNumber";
            this.OrderNumber.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.OrderNumber.SizeF = new System.Drawing.SizeF(112.5F, 18.83333F);
            this.OrderNumber.StylePriority.UseFont = false;
            // 
            // Pallet
            // 
            this.Pallet.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Pallet.LocationFloat = new DevExpress.Utils.PointFloat(122.5F, 0.0418663F);
            this.Pallet.Multiline = true;
            this.Pallet.Name = "Pallet";
            this.Pallet.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Pallet.SizeF = new System.Drawing.SizeF(246.875F, 18.83333F);
            this.Pallet.StylePriority.UseFont = false;
            this.Pallet.TextFormatString = "{0:}";
            // 
            // ExpectedDate
            // 
            this.ExpectedDate.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpectedDate.LocationFloat = new DevExpress.Utils.PointFloat(443.7634F, 0F);
            this.ExpectedDate.Multiline = true;
            this.ExpectedDate.Name = "ExpectedDate";
            this.ExpectedDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ExpectedDate.SizeF = new System.Drawing.SizeF(126.7363F, 18.83333F);
            this.ExpectedDate.StylePriority.UseFont = false;
            this.ExpectedDate.StylePriority.UseTextAlignment = false;
            this.ExpectedDate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.ExpectedDate.TextFormatString = "{0:d}";
            // 
            // TotalAmount
            // 
            this.TotalAmount.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalAmount.LocationFloat = new DevExpress.Utils.PointFloat(646.0276F, 0F);
            this.TotalAmount.Multiline = true;
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TotalAmount.SizeF = new System.Drawing.SizeF(138.1944F, 18.83333F);
            this.TotalAmount.StylePriority.UseFont = false;
            this.TotalAmount.StylePriority.UseTextAlignment = false;
            this.TotalAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.TotalAmount.TextFormatString = "{0:#.00}";
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel1,
            this.xrLabel2,
            this.xrLabel3,
            this.xrLabel6,
            this.xrLabel28});
            this.PageHeader.HeightF = 26.24982F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel4.BorderWidth = 0F;
            this.xrLabel4.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(369.375F, 3.24982F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(74.38846F, 23F);
            this.xrLabel4.StylePriority.UseBackColor = false;
            this.xrLabel4.StylePriority.UseBorderWidth = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Cases";
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel1.BorderWidth = 0F;
            this.xrLabel1.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(570.4997F, 3.249825F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(75.52795F, 23F);
            this.xrLabel1.StylePriority.UseBackColor = false;
            this.xrLabel1.StylePriority.UseBorderWidth = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "Buy Price";
            // 
            // xrLabel2
            // 
            this.xrLabel2.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel2.BorderWidth = 0F;
            this.xrLabel2.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel2.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 3.249825F);
            this.xrLabel2.Name = "xrLabel2";
            this.xrLabel2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel2.SizeF = new System.Drawing.SizeF(112.5F, 23F);
            this.xrLabel2.StylePriority.UseBackColor = false;
            this.xrLabel2.StylePriority.UseBorderWidth = false;
            this.xrLabel2.StylePriority.UseFont = false;
            this.xrLabel2.Text = "Order Number";
            // 
            // xrLabel3
            // 
            this.xrLabel3.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel3.BorderWidth = 0F;
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(122.5F, 3.24982F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(246.875F, 23F);
            this.xrLabel3.StylePriority.UseBackColor = false;
            this.xrLabel3.StylePriority.UseBorderWidth = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Pallet";
            // 
            // xrLabel6
            // 
            this.xrLabel6.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel6.BorderWidth = 0F;
            this.xrLabel6.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(443.7634F, 3.24982F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(126.7363F, 23F);
            this.xrLabel6.StylePriority.UseBackColor = false;
            this.xrLabel6.StylePriority.UseBorderWidth = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "Expiry Date";
            // 
            // xrLabel28
            // 
            this.xrLabel28.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel28.BorderWidth = 0F;
            this.xrLabel28.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(646.0276F, 3.249825F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(138.1944F, 23F);
            this.xrLabel28.StylePriority.UseBackColor = false;
            this.xrLabel28.StylePriority.UseBorderWidth = false;
            this.xrLabel28.StylePriority.UseFont = false;
            this.xrLabel28.Text = "Total Amt";
            // 
            // paramsTenantId
            // 
            this.paramsTenantId.Description = "Tenant";
            this.paramsTenantId.Name = "paramsTenantId";
            this.paramsTenantId.Type = typeof(int);
            this.paramsTenantId.ValueInfo = "0";
            this.paramsTenantId.Visible = false;
            // 
            // paramWarehouseId
            // 
            this.paramWarehouseId.Description = "warehouse";
            this.paramWarehouseId.Name = "paramWarehouseId";
            this.paramWarehouseId.Type = typeof(int);
            this.paramWarehouseId.ValueInfo = "0";
            this.paramWarehouseId.Visible = false;
            // 
            // ReportFooter
            // 
            this.ReportFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLine1});
            this.ReportFooter.HeightF = 19.86126F;
            this.ReportFooter.Name = "ReportFooter";
            // 
            // xrLine1
            // 
            this.xrLine1.ForeColor = System.Drawing.Color.LightGray;
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(10.00001F, 0.3334681F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(787.9999F, 9.527779F);
            this.xrLine1.StylePriority.UseForeColor = false;
            // 
            // paramProductId
            // 
            this.paramProductId.Description = "ProductId";
            this.paramProductId.MultiValue = true;
            this.paramProductId.Name = "paramProductId";
            this.paramProductId.Type = typeof(int);
            // 
            // StockValueDetailReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.PageHeader,
            this.ReportFooter});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataSource = this.sqlDataSource1;
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Landscape = true;
            this.Margins = new System.Drawing.Printing.Margins(0, 297, 0, 0);
            this.PageHeight = 850;
            this.PageWidth = 1100;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.paramsTenantId,
            this.paramWarehouseId,
            this.paramProductId});
            this.Version = "21.2";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.StockValueDetailReportlPrint_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        public DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        public DevExpress.XtraReports.Parameters.Parameter paramsTenantId;
        public DevExpress.XtraReports.Parameters.Parameter paramWarehouseId;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel28;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.UI.XRLabel OrderNumber;
        public DevExpress.XtraReports.UI.XRLabel Pallet;
        public DevExpress.XtraReports.UI.XRLabel ExpectedDate;
        public DevExpress.XtraReports.UI.XRLabel TotalAmount;
        public DevExpress.XtraReports.UI.XRLabel BuyPrice;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.UI.XRLabel Cases;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
        private DevExpress.XtraReports.Parameters.Parameter paramProductId;
    }
}

namespace WMS.Reports.Designs
{
    partial class ProductOrdersHistoryDetailReport
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
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings2 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.AccountName = new DevExpress.XtraReports.UI.XRLabel();
            this.BuyPrice = new DevExpress.XtraReports.UI.XRLabel();
            this.OrderNumber = new DevExpress.XtraReports.UI.XRLabel();
            this.ExpectedDate = new DevExpress.XtraReports.UI.XRLabel();
            this.Qty = new DevExpress.XtraReports.UI.XRLabel();
            this.SellPrice = new DevExpress.XtraReports.UI.XRLabel();
            this.TaxAmount = new DevExpress.XtraReports.UI.XRLabel();
            this.TotalAmount = new DevExpress.XtraReports.UI.XRLabel();
            this.AccountId = new DevExpress.XtraReports.Parameters.Parameter();
            this.EndDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.StartDate = new DevExpress.XtraReports.Parameters.Parameter();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel4 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel2 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.SellPriceTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.TaxAmountTitle = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel28 = new DevExpress.XtraReports.UI.XRLabel();
            this.ProductId = new DevExpress.XtraReports.Parameters.Parameter();
            this.TenantId = new DevExpress.XtraReports.Parameters.Parameter();
            this.WarehouseId = new DevExpress.XtraReports.Parameters.Parameter();
            this.OwnerId = new DevExpress.XtraReports.Parameters.Parameter();
            this.MarketId = new DevExpress.XtraReports.Parameters.Parameter();
            this.InventoryTransactionTypeId = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportFooter = new DevExpress.XtraReports.UI.ReportFooterBand();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
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
            this.AccountName,
            this.BuyPrice,
            this.OrderNumber,
            this.ExpectedDate,
            this.Qty,
            this.SellPrice,
            this.TaxAmount,
            this.TotalAmount});
            this.Detail.HeightF = 19.70872F;
            this.Detail.Name = "Detail";
            this.Detail.StylePriority.UseBorderColor = false;
            this.Detail.StylePriority.UseBorders = false;
            // 
            // AccountName
            // 
            this.AccountName.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AccountName.LocationFloat = new DevExpress.Utils.PointFloat(212.0833F, 0F);
            this.AccountName.Multiline = true;
            this.AccountName.Name = "AccountName";
            this.AccountName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.AccountName.SizeF = new System.Drawing.SizeF(403.5551F, 18.83333F);
            this.AccountName.StylePriority.UseFont = false;
            this.AccountName.TextFormatString = "{0:d}";
            // 
            // BuyPrice
            // 
            this.BuyPrice.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BuyPrice.LocationFloat = new DevExpress.Utils.PointFloat(687.1666F, 0F);
            this.BuyPrice.Multiline = true;
            this.BuyPrice.Name = "BuyPrice";
            this.BuyPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.BuyPrice.SizeF = new System.Drawing.SizeF(75.52777F, 18.83333F);
            this.BuyPrice.StylePriority.UseFont = false;
            this.BuyPrice.StylePriority.UseTextAlignment = false;
            this.BuyPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
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
            // ExpectedDate
            // 
            this.ExpectedDate.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ExpectedDate.LocationFloat = new DevExpress.Utils.PointFloat(122.5F, 0.0418769F);
            this.ExpectedDate.Multiline = true;
            this.ExpectedDate.Name = "ExpectedDate";
            this.ExpectedDate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ExpectedDate.SizeF = new System.Drawing.SizeF(89.58333F, 18.83333F);
            this.ExpectedDate.StylePriority.UseFont = false;
            this.ExpectedDate.TextFormatString = "{0:d}";
            // 
            // Qty
            // 
            this.Qty.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Qty.LocationFloat = new DevExpress.Utils.PointFloat(615.6385F, 0F);
            this.Qty.Multiline = true;
            this.Qty.Name = "Qty";
            this.Qty.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Qty.SizeF = new System.Drawing.SizeF(71.52795F, 18.83333F);
            this.Qty.StylePriority.UseFont = false;
            this.Qty.StylePriority.UseTextAlignment = false;
            this.Qty.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Qty.TextFormatString = "{0:#,#}";
            // 
            // SellPrice
            // 
            this.SellPrice.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SellPrice.LocationFloat = new DevExpress.Utils.PointFloat(762.6944F, 0F);
            this.SellPrice.Multiline = true;
            this.SellPrice.Name = "SellPrice";
            this.SellPrice.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SellPrice.SizeF = new System.Drawing.SizeF(88.19446F, 18.83333F);
            this.SellPrice.StylePriority.UseFont = false;
            this.SellPrice.StylePriority.UseTextAlignment = false;
            this.SellPrice.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TaxAmount
            // 
            this.TaxAmount.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TaxAmount.LocationFloat = new DevExpress.Utils.PointFloat(850.8888F, 0F);
            this.TaxAmount.Multiline = true;
            this.TaxAmount.Name = "TaxAmount";
            this.TaxAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TaxAmount.SizeF = new System.Drawing.SizeF(122.9167F, 18.83333F);
            this.TaxAmount.StylePriority.UseFont = false;
            this.TaxAmount.StylePriority.UseTextAlignment = false;
            this.TaxAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // TotalAmount
            // 
            this.TotalAmount.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalAmount.LocationFloat = new DevExpress.Utils.PointFloat(973.8055F, 0F);
            this.TotalAmount.Multiline = true;
            this.TotalAmount.Name = "TotalAmount";
            this.TotalAmount.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TotalAmount.SizeF = new System.Drawing.SizeF(138.1944F, 18.83333F);
            this.TotalAmount.StylePriority.UseFont = false;
            this.TotalAmount.StylePriority.UseTextAlignment = false;
            this.TotalAmount.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // AccountId
            // 
            this.AccountId.AllowNull = true;
            this.AccountId.Name = "AccountId";
            this.AccountId.Type = typeof(int);
            this.AccountId.ValueSourceSettings = staticListLookUpSettings1;
            this.AccountId.Visible = false;
            // 
            // EndDate
            // 
            this.EndDate.Name = "EndDate";
            this.EndDate.Type = typeof(System.DateTime);
            this.EndDate.Visible = false;
            // 
            // StartDate
            // 
            this.StartDate.Description = "Start Date";
            this.StartDate.Name = "StartDate";
            this.StartDate.Type = typeof(System.DateTime);
            this.StartDate.Visible = false;
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel4,
            this.xrLabel1,
            this.xrLabel2,
            this.xrLabel3,
            this.xrLabel6,
            this.SellPriceTitle,
            this.TaxAmountTitle,
            this.xrLabel28});
            this.PageHeader.HeightF = 26.24982F;
            this.PageHeader.Name = "PageHeader";
            // 
            // xrLabel4
            // 
            this.xrLabel4.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel4.BorderWidth = 0F;
            this.xrLabel4.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel4.LocationFloat = new DevExpress.Utils.PointFloat(212.0833F, 3.249825F);
            this.xrLabel4.Name = "xrLabel4";
            this.xrLabel4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel4.SizeF = new System.Drawing.SizeF(403.5551F, 23F);
            this.xrLabel4.StylePriority.UseBackColor = false;
            this.xrLabel4.StylePriority.UseBorderWidth = false;
            this.xrLabel4.StylePriority.UseFont = false;
            this.xrLabel4.Text = "Account";
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel1.BorderWidth = 0F;
            this.xrLabel1.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(687.1664F, 3.249825F);
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
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(122.5F, 3.249825F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(89.58333F, 23F);
            this.xrLabel3.StylePriority.UseBackColor = false;
            this.xrLabel3.StylePriority.UseBorderWidth = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Date";
            // 
            // xrLabel6
            // 
            this.xrLabel6.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel6.BorderWidth = 0F;
            this.xrLabel6.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(615.6385F, 3.249825F);
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(71.52795F, 23F);
            this.xrLabel6.StylePriority.UseBackColor = false;
            this.xrLabel6.StylePriority.UseBorderWidth = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.Text = "Qty";
            // 
            // SellPriceTitle
            // 
            this.SellPriceTitle.BackColor = System.Drawing.Color.Transparent;
            this.SellPriceTitle.BorderWidth = 0F;
            this.SellPriceTitle.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SellPriceTitle.LocationFloat = new DevExpress.Utils.PointFloat(762.6944F, 3.249825F);
            this.SellPriceTitle.Name = "SellPriceTitle";
            this.SellPriceTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SellPriceTitle.SizeF = new System.Drawing.SizeF(88.19446F, 23F);
            this.SellPriceTitle.StylePriority.UseBackColor = false;
            this.SellPriceTitle.StylePriority.UseBorderWidth = false;
            this.SellPriceTitle.StylePriority.UseFont = false;
            this.SellPriceTitle.Text = "Sell Price";
            // 
            // TaxAmountTitle
            // 
            this.TaxAmountTitle.BackColor = System.Drawing.Color.Transparent;
            this.TaxAmountTitle.BorderWidth = 0F;
            this.TaxAmountTitle.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TaxAmountTitle.LocationFloat = new DevExpress.Utils.PointFloat(850.8888F, 3.249825F);
            this.TaxAmountTitle.Name = "TaxAmountTitle";
            this.TaxAmountTitle.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.TaxAmountTitle.SizeF = new System.Drawing.SizeF(122.9167F, 23F);
            this.TaxAmountTitle.StylePriority.UseBackColor = false;
            this.TaxAmountTitle.StylePriority.UseBorderWidth = false;
            this.TaxAmountTitle.StylePriority.UseFont = false;
            this.TaxAmountTitle.Text = "Tax Amt";
            // 
            // xrLabel28
            // 
            this.xrLabel28.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel28.BorderWidth = 0F;
            this.xrLabel28.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel28.LocationFloat = new DevExpress.Utils.PointFloat(973.8055F, 3.249825F);
            this.xrLabel28.Name = "xrLabel28";
            this.xrLabel28.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel28.SizeF = new System.Drawing.SizeF(138.1944F, 23F);
            this.xrLabel28.StylePriority.UseBackColor = false;
            this.xrLabel28.StylePriority.UseBorderWidth = false;
            this.xrLabel28.StylePriority.UseFont = false;
            this.xrLabel28.Text = "Total Amt";
            // 
            // ProductId
            // 
            this.ProductId.AllowNull = true;
            this.ProductId.Name = "ProductId";
            this.ProductId.Type = typeof(int);
            this.ProductId.ValueSourceSettings = staticListLookUpSettings2;
            this.ProductId.Visible = false;
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
            // OwnerId
            // 
            this.OwnerId.AllowNull = true;
            this.OwnerId.Description = "Owner";
            this.OwnerId.Name = "OwnerId";
            this.OwnerId.Type = typeof(int);
            this.OwnerId.Visible = false;
            // 
            // MarketId
            // 
            this.MarketId.AllowNull = true;
            this.MarketId.Name = "MarketId";
            this.MarketId.Type = typeof(int);
            this.MarketId.ValueInfo = "0";
            this.MarketId.Visible = false;
            // 
            // InventoryTransactionTypeId
            // 
            this.InventoryTransactionTypeId.Name = "InventoryTransactionTypeId";
            this.InventoryTransactionTypeId.Type = typeof(int);
            this.InventoryTransactionTypeId.ValueInfo = "2";
            this.InventoryTransactionTypeId.Visible = false;
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
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(10.00002F, 0.3334893F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(1102F, 9.527779F);
            this.xrLine1.StylePriority.UseForeColor = false;
            // 
            // ProductOrdersHistoryDetailReport
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
            this.Margins = new System.Drawing.Printing.Margins(22, 25, 0, 0);
            this.PageHeight = 827;
            this.PageWidth = 1169;
            this.PaperKind = System.Drawing.Printing.PaperKind.A4;
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.StartDate,
            this.EndDate,
            this.ProductId,
            this.TenantId,
            this.WarehouseId,
            this.AccountId,
            this.OwnerId,
            this.MarketId,
            this.InventoryTransactionTypeId});
            this.Version = "20.1";
            this.DataSourceDemanded += new System.EventHandler<System.EventArgs>(this.ProductSoldBySkuDetailPrint_DataSourceDemanded);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        public DevExpress.XtraReports.Parameters.Parameter StartDate;
        public DevExpress.XtraReports.Parameters.Parameter EndDate;
        public DevExpress.XtraReports.Parameters.Parameter ProductId;
        public DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        public DevExpress.XtraReports.Parameters.Parameter TenantId;
        public DevExpress.XtraReports.Parameters.Parameter WarehouseId;
        public DevExpress.XtraReports.Parameters.Parameter OwnerId;
        public DevExpress.XtraReports.Parameters.Parameter AccountId;
        public DevExpress.XtraReports.Parameters.Parameter MarketId;
        public DevExpress.XtraReports.Parameters.Parameter InventoryTransactionTypeId;
        private DevExpress.XtraReports.UI.XRLabel xrLabel2;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.XRLabel xrLabel28;
        private DevExpress.XtraReports.UI.ReportFooterBand ReportFooter;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.UI.XRLabel OrderNumber;
        public DevExpress.XtraReports.UI.XRLabel ExpectedDate;
        public DevExpress.XtraReports.UI.XRLabel Qty;
        public DevExpress.XtraReports.UI.XRLabel SellPrice;
        public DevExpress.XtraReports.UI.XRLabel TaxAmount;
        public DevExpress.XtraReports.UI.XRLabel TotalAmount;
        public DevExpress.XtraReports.UI.XRLabel BuyPrice;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.UI.XRLabel SellPriceTitle;
        public DevExpress.XtraReports.UI.XRLabel TaxAmountTitle;
        public DevExpress.XtraReports.UI.XRLabel AccountName;
        private DevExpress.XtraReports.UI.XRLabel xrLabel4;
    }
}

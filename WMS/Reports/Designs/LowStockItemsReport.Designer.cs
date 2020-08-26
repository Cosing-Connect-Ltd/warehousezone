namespace WMS.Reports
{
    partial class LowStockItemsReport
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
            DevExpress.XtraReports.Parameters.StaticListLookUpSettings staticListLookUpSettings1 = new DevExpress.XtraReports.Parameters.StaticListLookUpSettings();
            DevExpress.DataAccess.Sql.SelectQuery selectQuery1 = new DevExpress.DataAccess.Sql.SelectQuery();
            DevExpress.DataAccess.Sql.Column column1 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression1 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Table table1 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.Column column2 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression2 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Column column3 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression3 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Table table2 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.Column column4 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression4 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Column column5 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression5 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Table table3 = new DevExpress.DataAccess.Sql.Table();
            DevExpress.DataAccess.Sql.Column column6 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression6 = new DevExpress.DataAccess.Sql.ColumnExpression();
            DevExpress.DataAccess.Sql.Column column7 = new DevExpress.DataAccess.Sql.Column();
            DevExpress.DataAccess.Sql.ColumnExpression columnExpression7 = new DevExpress.DataAccess.Sql.ColumnExpression();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LowStockItemsReport));
            DevExpress.DataAccess.Sql.QueryParameter queryParameter1 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.QueryParameter queryParameter2 = new DevExpress.DataAccess.Sql.QueryParameter();
            DevExpress.DataAccess.Sql.Join join1 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo1 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.Join join2 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo2 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            DevExpress.DataAccess.Sql.Join join3 = new DevExpress.DataAccess.Sql.Join();
            DevExpress.DataAccess.Sql.RelationColumnInfo relationColumnInfo3 = new DevExpress.DataAccess.Sql.RelationColumnInfo();
            this.Detail = new DevExpress.XtraReports.UI.DetailBand();
            this.xrLabel8 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblQuantity = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.TopMargin = new DevExpress.XtraReports.UI.TopMarginBand();
            this.BottomMargin = new DevExpress.XtraReports.UI.BottomMarginBand();
            this.WarehouseParam = new DevExpress.XtraReports.Parameters.Parameter();
            this.ReportHeader = new DevExpress.XtraReports.UI.ReportHeaderBand();
            this.xrLabel5 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine2 = new DevExpress.XtraReports.UI.XRLine();
            this.SKU = new DevExpress.XtraReports.UI.XRLabel();
            this.InStock = new DevExpress.XtraReports.UI.XRLabel();
            this.ReOrder = new DevExpress.XtraReports.UI.XRLabel();
            this.PageFooter = new DevExpress.XtraReports.UI.PageFooterBand();
            this.xrLabel6 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLine1 = new DevExpress.XtraReports.UI.XRLine();
            this.PageHeader = new DevExpress.XtraReports.UI.PageHeaderBand();
            this.xrLabel7 = new DevExpress.XtraReports.UI.XRLabel();
            this.TenantIdParam = new DevExpress.XtraReports.Parameters.Parameter();
            this.sqlDataSource1 = new DevExpress.DataAccess.Sql.SqlDataSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel8,
            this.xrLabel3,
            this.lblQuantity,
            this.xrLabel1});
            this.Detail.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Detail.HeightF = 30.20833F;
            this.Detail.Name = "Detail";
            this.Detail.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.Detail.StylePriority.UseFont = false;
            this.Detail.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // xrLabel8
            // 
            this.xrLabel8.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Name]")});
            this.xrLabel8.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel8.LocationFloat = new DevExpress.Utils.PointFloat(151.0417F, 0F);
            this.xrLabel8.Name = "xrLabel8";
            this.xrLabel8.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel8.SizeF = new System.Drawing.SizeF(402.1252F, 23F);
            this.xrLabel8.StylePriority.UseFont = false;
            // 
            // xrLabel3
            // 
            this.xrLabel3.BorderWidth = 0F;
            this.xrLabel3.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[Available]")});
            this.xrLabel3.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(682.6667F, 0F);
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(118.3332F, 23F);
            this.xrLabel3.StylePriority.UseBorderWidth = false;
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "xrLabel3";
            // 
            // lblQuantity
            // 
            this.lblQuantity.BorderWidth = 0F;
            this.lblQuantity.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[MinStockQuantity]")});
            this.lblQuantity.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblQuantity.LocationFloat = new DevExpress.Utils.PointFloat(553.167F, 0F);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblQuantity.SizeF = new System.Drawing.SizeF(129.4998F, 23F);
            this.lblQuantity.StylePriority.UseBorderWidth = false;
            this.lblQuantity.StylePriority.UseFont = false;
            // 
            // xrLabel1
            // 
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel1.BorderWidth = 0F;
            this.xrLabel1.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "[SKUCode]")});
            this.xrLabel1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(151.0417F, 23F);
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseBorderWidth = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.Text = "xrLabel1";
            // 
            // TopMargin
            // 
            this.TopMargin.HeightF = 20.66666F;
            this.TopMargin.Name = "TopMargin";
            this.TopMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.TopMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 4F;
            this.BottomMargin.Name = "BottomMargin";
            this.BottomMargin.Padding = new DevExpress.XtraPrinting.PaddingInfo(0, 0, 0, 0, 100F);
            this.BottomMargin.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // WarehouseParam
            // 
            this.WarehouseParam.Description = "Location";
            this.WarehouseParam.Name = "WarehouseParam";
            this.WarehouseParam.Type = typeof(int);
            this.WarehouseParam.ValueInfo = "0";
            this.WarehouseParam.ValueSourceSettings = staticListLookUpSettings1;
            // 
            // ReportHeader
            // 
            this.ReportHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.ReportHeader.BorderColor = System.Drawing.Color.Black;
            this.ReportHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel5,
            this.xrLine2});
            this.ReportHeader.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReportHeader.HeightF = 47.29169F;
            this.ReportHeader.Name = "ReportHeader";
            this.ReportHeader.StylePriority.UseBackColor = false;
            this.ReportHeader.StylePriority.UseBorderColor = false;
            this.ReportHeader.StylePriority.UseFont = false;
            this.ReportHeader.StylePriority.UseTextAlignment = false;
            this.ReportHeader.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLabel5
            // 
            this.xrLabel5.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel5.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel5.BorderWidth = 0F;
            this.xrLabel5.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel5.Name = "xrLabel5";
            this.xrLabel5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel5.SizeF = new System.Drawing.SizeF(800.9999F, 30.62501F);
            this.xrLabel5.StylePriority.UseBackColor = false;
            this.xrLabel5.StylePriority.UseBorders = false;
            this.xrLabel5.StylePriority.UseBorderWidth = false;
            this.xrLabel5.Text = "Low Stock Items";
            // 
            // xrLine2
            // 
            this.xrLine2.BackColor = System.Drawing.Color.White;
            this.xrLine2.LocationFloat = new DevExpress.Utils.PointFloat(0F, 30.62501F);
            this.xrLine2.Name = "xrLine2";
            this.xrLine2.SizeF = new System.Drawing.SizeF(801.0001F, 13.62502F);
            this.xrLine2.StylePriority.UseBackColor = false;
            // 
            // SKU
            // 
            this.SKU.BorderWidth = 0F;
            this.SKU.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SKU.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.SKU.Name = "SKU";
            this.SKU.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SKU.SizeF = new System.Drawing.SizeF(151.0417F, 23F);
            this.SKU.StylePriority.UseBorderWidth = false;
            this.SKU.StylePriority.UseFont = false;
            this.SKU.Text = "SKU";
            // 
            // InStock
            // 
            this.InStock.BorderWidth = 0F;
            this.InStock.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InStock.LocationFloat = new DevExpress.Utils.PointFloat(682.6667F, 0F);
            this.InStock.Name = "InStock";
            this.InStock.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.InStock.SizeF = new System.Drawing.SizeF(118.3332F, 23F);
            this.InStock.StylePriority.UseBorderWidth = false;
            this.InStock.StylePriority.UseFont = false;
            this.InStock.Text = "Available";
            // 
            // ReOrder
            // 
            this.ReOrder.BorderWidth = 0F;
            this.ReOrder.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReOrder.LocationFloat = new DevExpress.Utils.PointFloat(553.167F, 0F);
            this.ReOrder.Multiline = true;
            this.ReOrder.Name = "ReOrder";
            this.ReOrder.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ReOrder.SizeF = new System.Drawing.SizeF(129.4998F, 23F);
            this.ReOrder.StylePriority.UseBorderWidth = false;
            this.ReOrder.StylePriority.UseFont = false;
            this.ReOrder.Text = "ReOrder Qty";
            // 
            // PageFooter
            // 
            this.PageFooter.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel6,
            this.xrLine1});
            this.PageFooter.HeightF = 39.76981F;
            this.PageFooter.Name = "PageFooter";
            // 
            // xrLabel6
            // 
            this.xrLabel6.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel6.Borders = DevExpress.XtraPrinting.BorderSide.None;
            this.xrLabel6.BorderWidth = 0F;
            this.xrLabel6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel6.LocationFloat = new DevExpress.Utils.PointFloat(0F, 13.62502F);
            this.xrLabel6.Multiline = true;
            this.xrLabel6.Name = "xrLabel6";
            this.xrLabel6.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel6.SizeF = new System.Drawing.SizeF(800.9999F, 23.33333F);
            this.xrLabel6.StylePriority.UseBackColor = false;
            this.xrLabel6.StylePriority.UseBorders = false;
            this.xrLabel6.StylePriority.UseBorderWidth = false;
            this.xrLabel6.StylePriority.UseFont = false;
            this.xrLabel6.StylePriority.UseTextAlignment = false;
            this.xrLabel6.Text = "©WarehouseZone\r\n\r\n";
            this.xrLabel6.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
            // 
            // xrLine1
            // 
            this.xrLine1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLine1.Name = "xrLine1";
            this.xrLine1.SizeF = new System.Drawing.SizeF(801.0001F, 13.62502F);
            // 
            // PageHeader
            // 
            this.PageHeader.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel7,
            this.ReOrder,
            this.SKU,
            this.InStock});
            this.PageHeader.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PageHeader.HeightF = 32.29167F;
            this.PageHeader.Name = "PageHeader";
            this.PageHeader.StylePriority.UseFont = false;
            // 
            // xrLabel7
            // 
            this.xrLabel7.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel7.LocationFloat = new DevExpress.Utils.PointFloat(151.0417F, 0F);
            this.xrLabel7.Name = "xrLabel7";
            this.xrLabel7.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel7.SizeF = new System.Drawing.SizeF(402.1252F, 23F);
            this.xrLabel7.StylePriority.UseFont = false;
            this.xrLabel7.Text = "Product";
            // 
            // TenantIdParam
            // 
            this.TenantIdParam.Description = "Tenant";
            this.TenantIdParam.Name = "TenantIdParam";
            this.TenantIdParam.Type = typeof(int);
            this.TenantIdParam.ValueInfo = "0";
            this.TenantIdParam.Visible = false;
            // 
            // sqlDataSource1
            // 
            this.sqlDataSource1.ConnectionName = "ApplicationContext";
            this.sqlDataSource1.Name = "sqlDataSource1";
            columnExpression1.ColumnName = "MinStockQuantity";
            table1.MetaSerializable = "<Meta X=\"30\" Y=\"30\" Width=\"210\" Height=\"248\" />";
            table1.Name = "ProductLocationStockLevel";
            columnExpression1.Table = table1;
            column1.Expression = columnExpression1;
            columnExpression2.ColumnName = "TenantLocationID";
            columnExpression2.Table = table1;
            column2.Expression = columnExpression2;
            columnExpression3.ColumnName = "SKUCode";
            table2.MetaSerializable = "<Meta X=\"270\" Y=\"30\" Width=\"125\" Height=\"1312\" />";
            table2.Name = "ProductMaster";
            columnExpression3.Table = table2;
            column3.Expression = columnExpression3;
            columnExpression4.ColumnName = "Name";
            columnExpression4.Table = table2;
            column4.Expression = columnExpression4;
            columnExpression5.ColumnName = "Available";
            table3.MetaSerializable = "<Meta X=\"425\" Y=\"30\" Width=\"125\" Height=\"324\" />";
            table3.Name = "InventoryStocks";
            columnExpression5.Table = table3;
            column5.Expression = columnExpression5;
            columnExpression6.ColumnName = "TenantId";
            columnExpression6.Table = table1;
            column6.Expression = columnExpression6;
            columnExpression7.ColumnName = "ProductMasterID";
            columnExpression7.Table = table1;
            column7.Expression = columnExpression7;
            selectQuery1.Columns.Add(column1);
            selectQuery1.Columns.Add(column2);
            selectQuery1.Columns.Add(column3);
            selectQuery1.Columns.Add(column4);
            selectQuery1.Columns.Add(column5);
            selectQuery1.Columns.Add(column6);
            selectQuery1.Columns.Add(column7);
            selectQuery1.FilterString = resources.GetString("selectQuery1.FilterString");
            selectQuery1.GroupFilterString = "";
            selectQuery1.Name = "ProductLocationStockLevel_1";
            queryParameter1.Name = "sqlParamTenantId";
            queryParameter1.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter1.Value = new DevExpress.DataAccess.Expression("?TenantIdParam", typeof(int));
            queryParameter2.Name = "sqlParamWarehouseId";
            queryParameter2.Type = typeof(DevExpress.DataAccess.Expression);
            queryParameter2.Value = new DevExpress.DataAccess.Expression("?WarehouseParam", typeof(int));
            selectQuery1.Parameters.Add(queryParameter1);
            selectQuery1.Parameters.Add(queryParameter2);
            relationColumnInfo1.NestedKeyColumn = "ProductId";
            relationColumnInfo1.ParentKeyColumn = "ProductMasterID";
            join1.KeyColumns.Add(relationColumnInfo1);
            join1.Nested = table2;
            join1.Parent = table1;
            relationColumnInfo2.NestedKeyColumn = "ProductId";
            relationColumnInfo2.ParentKeyColumn = "ProductId";
            join2.KeyColumns.Add(relationColumnInfo2);
            join2.Nested = table3;
            join2.Parent = table2;
            relationColumnInfo3.NestedKeyColumn = "WarehouseId";
            relationColumnInfo3.ParentKeyColumn = "TenantLocationID";
            join3.KeyColumns.Add(relationColumnInfo3);
            join3.Nested = table3;
            join3.Parent = table1;
            selectQuery1.Relations.Add(join1);
            selectQuery1.Relations.Add(join2);
            selectQuery1.Relations.Add(join3);
            selectQuery1.Tables.Add(table1);
            selectQuery1.Tables.Add(table2);
            selectQuery1.Tables.Add(table3);
            this.sqlDataSource1.Queries.AddRange(new DevExpress.DataAccess.Sql.SqlQuery[] {
            selectQuery1});
            this.sqlDataSource1.ResultSchemaSerializable = resources.GetString("sqlDataSource1.ResultSchemaSerializable");
            // 
            // LowStockItemsReport
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.Detail,
            this.TopMargin,
            this.BottomMargin,
            this.ReportHeader,
            this.PageFooter,
            this.PageHeader});
            this.ComponentStorage.AddRange(new System.ComponentModel.IComponent[] {
            this.sqlDataSource1});
            this.DataMember = "ProductLocationStockLevel_1";
            this.DataSource = this.sqlDataSource1;
            this.FilterString = "[TenantId] = ?TenantIdParam";
            this.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margins = new System.Drawing.Printing.Margins(25, 24, 21, 4);
            this.Parameters.AddRange(new DevExpress.XtraReports.Parameters.Parameter[] {
            this.WarehouseParam,
            this.TenantIdParam});
            this.Version = "19.2";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        public DevExpress.XtraReports.Parameters.Parameter WarehouseParam;
        private DevExpress.XtraReports.UI.ReportHeaderBand ReportHeader;
        private DevExpress.XtraReports.UI.XRLabel xrLabel5;
        private DevExpress.XtraReports.UI.XRLabel SKU;
        private DevExpress.XtraReports.UI.XRLabel InStock;
        private DevExpress.XtraReports.UI.XRLabel ReOrder;
        private DevExpress.XtraReports.UI.PageFooterBand PageFooter;
        private DevExpress.XtraReports.UI.XRLabel xrLabel6;
        private DevExpress.XtraReports.UI.PageHeaderBand PageHeader;
        public DevExpress.XtraReports.Parameters.Parameter TenantIdParam;
        private DevExpress.XtraReports.UI.XRLabel xrLabel8;
        private DevExpress.XtraReports.UI.XRLabel xrLabel7;
        private DevExpress.DataAccess.Sql.SqlDataSource sqlDataSource1;
        private DevExpress.XtraReports.UI.XRLine xrLine2;
        private DevExpress.XtraReports.UI.XRLine xrLine1;
        public DevExpress.XtraReports.UI.XRLabel lblQuantity;
    }
}

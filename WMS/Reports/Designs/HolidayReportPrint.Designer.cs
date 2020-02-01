namespace WMS.Reports.Designs
{
    partial class HolidayReportPrint
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
            this.xrLabel1 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel3 = new DevExpress.XtraReports.UI.XRLabel();
            this.xrLabel16 = new DevExpress.XtraReports.UI.XRLabel();
            this.calculatedField1 = new DevExpress.XtraReports.UI.CalculatedField();
            this.EndingYear = new DevExpress.XtraReports.UI.CalculatedField();
            this.calculatedField2 = new DevExpress.XtraReports.UI.CalculatedField();
            this.lblUserName = new DevExpress.XtraReports.UI.XRLabel();
            this.lblYear1 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblYear2 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblstartdate = new DevExpress.XtraReports.UI.XRLabel();
            this.lblYear3 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblYear4 = new DevExpress.XtraReports.UI.XRLabel();
            this.lblYear5 = new DevExpress.XtraReports.UI.XRLabel();
            this.UserName = new DevExpress.XtraReports.UI.XRLabel();
            this.Date = new DevExpress.XtraReports.UI.XRLabel();
            this.FirstYear = new DevExpress.XtraReports.UI.XRLabel();
            this.SecondYear = new DevExpress.XtraReports.UI.XRLabel();
            this.ThirdYear = new DevExpress.XtraReports.UI.XRLabel();
            this.FourthYear = new DevExpress.XtraReports.UI.XRLabel();
            this.FifthYear = new DevExpress.XtraReports.UI.XRLabel();
            this.GroupHeader1 = new DevExpress.XtraReports.UI.GroupHeaderBand();
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            // 
            // TopMargin
            // 
            this.TopMargin.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.xrLabel1,
            this.xrLabel3,
            this.xrLabel16});
            this.TopMargin.HeightF = 63.41667F;
            this.TopMargin.Name = "TopMargin";
            // 
            // BottomMargin
            // 
            this.BottomMargin.HeightF = 1.041667F;
            this.BottomMargin.Name = "BottomMargin";
            // 
            // Detail
            // 
            this.Detail.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.FifthYear,
            this.FourthYear,
            this.UserName,
            this.SecondYear,
            this.FirstYear,
            this.Date,
            this.ThirdYear});
            this.Detail.HeightF = 28.125F;
            this.Detail.Name = "Detail";
            // 
            // xrLabel1
            // 
            this.xrLabel1.BackColor = System.Drawing.Color.Transparent;
            this.xrLabel1.Borders = DevExpress.XtraPrinting.BorderSide.Bottom;
            this.xrLabel1.BorderWidth = 0F;
            this.xrLabel1.Font = new System.Drawing.Font("Trebuchet MS", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel1.LocationFloat = new DevExpress.Utils.PointFloat(0F, 23F);
            this.xrLabel1.Name = "xrLabel1";
            this.xrLabel1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel1.SizeF = new System.Drawing.SizeF(848F, 28.86219F);
            this.xrLabel1.StylePriority.UseBackColor = false;
            this.xrLabel1.StylePriority.UseBorders = false;
            this.xrLabel1.StylePriority.UseBorderWidth = false;
            this.xrLabel1.StylePriority.UseFont = false;
            this.xrLabel1.StylePriority.UseTextAlignment = false;
            this.xrLabel1.Text = "Holiday Report";
            this.xrLabel1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopCenter;
            // 
            // xrLabel3
            // 
            this.xrLabel3.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel3.LocationFloat = new DevExpress.Utils.PointFloat(0F, 0F);
            this.xrLabel3.Multiline = true;
            this.xrLabel3.Name = "xrLabel3";
            this.xrLabel3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel3.SizeF = new System.Drawing.SizeF(53.12513F, 23F);
            this.xrLabel3.StylePriority.UseFont = false;
            this.xrLabel3.Text = "Date:";
            // 
            // xrLabel16
            // 
            this.xrLabel16.ExpressionBindings.AddRange(new DevExpress.XtraReports.UI.ExpressionBinding[] {
            new DevExpress.XtraReports.UI.ExpressionBinding("BeforePrint", "Text", "Now()")});
            this.xrLabel16.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.xrLabel16.LocationFloat = new DevExpress.Utils.PointFloat(53.12513F, 0F);
            this.xrLabel16.Multiline = true;
            this.xrLabel16.Name = "xrLabel16";
            this.xrLabel16.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.xrLabel16.SizeF = new System.Drawing.SizeF(178.125F, 23F);
            this.xrLabel16.StylePriority.UseFont = false;
            this.xrLabel16.Text = "xrLabel16";
            // 
            // calculatedField1
            // 
            this.calculatedField1.DataMember = "Resources";
            this.calculatedField1.DisplayName = "StartingYear";
            this.calculatedField1.Expression = "GetYear([JobStartDate])";
            this.calculatedField1.Name = "calculatedField1";
            // 
            // EndingYear
            // 
            this.EndingYear.DataMember = "Resources";
            this.EndingYear.Expression = "GetYear(UtcNow())";
            this.EndingYear.Name = "EndingYear";
            // 
            // calculatedField2
            // 
            this.calculatedField2.DataMember = "Resources";
            this.calculatedField2.Name = "calculatedField2";
            // 
            // lblUserName
            // 
            this.lblUserName.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserName.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 0F);
            this.lblUserName.Multiline = true;
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblUserName.SizeF = new System.Drawing.SizeF(216.0416F, 25.62498F);
            this.lblUserName.StylePriority.UseFont = false;
            this.lblUserName.StylePriority.UseTextAlignment = false;
            this.lblUserName.Text = "Name";
            this.lblUserName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblYear1
            // 
            this.lblYear1.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear1.LocationFloat = new DevExpress.Utils.PointFloat(354.1666F, 0F);
            this.lblYear1.Multiline = true;
            this.lblYear1.Name = "lblYear1";
            this.lblYear1.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblYear1.SizeF = new System.Drawing.SizeF(105.2085F, 25.62498F);
            this.lblYear1.StylePriority.UseFont = false;
            this.lblYear1.StylePriority.UseTextAlignment = false;
            this.lblYear1.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblYear2
            // 
            this.lblYear2.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear2.LocationFloat = new DevExpress.Utils.PointFloat(459.3751F, 0F);
            this.lblYear2.Multiline = true;
            this.lblYear2.Name = "lblYear2";
            this.lblYear2.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblYear2.SizeF = new System.Drawing.SizeF(101.7502F, 25.62498F);
            this.lblYear2.StylePriority.UseFont = false;
            this.lblYear2.StylePriority.UseTextAlignment = false;
            this.lblYear2.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblstartdate
            // 
            this.lblstartdate.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstartdate.LocationFloat = new DevExpress.Utils.PointFloat(226.0416F, 0F);
            this.lblstartdate.Multiline = true;
            this.lblstartdate.Name = "lblstartdate";
            this.lblstartdate.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblstartdate.SizeF = new System.Drawing.SizeF(128.125F, 25.62498F);
            this.lblstartdate.StylePriority.UseFont = false;
            this.lblstartdate.StylePriority.UseTextAlignment = false;
            this.lblstartdate.Text = "Job Start Date";
            this.lblstartdate.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblYear3
            // 
            this.lblYear3.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear3.LocationFloat = new DevExpress.Utils.PointFloat(561.1252F, 0F);
            this.lblYear3.Multiline = true;
            this.lblYear3.Name = "lblYear3";
            this.lblYear3.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblYear3.SizeF = new System.Drawing.SizeF(95.4166F, 25.62498F);
            this.lblYear3.StylePriority.UseFont = false;
            this.lblYear3.StylePriority.UseTextAlignment = false;
            this.lblYear3.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblYear4
            // 
            this.lblYear4.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear4.LocationFloat = new DevExpress.Utils.PointFloat(656.5418F, 0F);
            this.lblYear4.Multiline = true;
            this.lblYear4.Name = "lblYear4";
            this.lblYear4.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblYear4.SizeF = new System.Drawing.SizeF(88.12494F, 25.62498F);
            this.lblYear4.StylePriority.UseFont = false;
            this.lblYear4.StylePriority.UseTextAlignment = false;
            this.lblYear4.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // lblYear5
            // 
            this.lblYear5.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYear5.LocationFloat = new DevExpress.Utils.PointFloat(744.6667F, 0F);
            this.lblYear5.Multiline = true;
            this.lblYear5.Name = "lblYear5";
            this.lblYear5.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.lblYear5.SizeF = new System.Drawing.SizeF(103.3332F, 25.62498F);
            this.lblYear5.StylePriority.UseFont = false;
            this.lblYear5.StylePriority.UseTextAlignment = false;
            this.lblYear5.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // UserName
            // 
            this.UserName.LocationFloat = new DevExpress.Utils.PointFloat(9.999998F, 0F);
            this.UserName.Multiline = true;
            this.UserName.Name = "UserName";
            this.UserName.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.UserName.SizeF = new System.Drawing.SizeF(216.0416F, 25.62498F);
            this.UserName.StylePriority.UseTextAlignment = false;
            this.UserName.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // Date
            // 
            this.Date.LocationFloat = new DevExpress.Utils.PointFloat(226.0416F, 0F);
            this.Date.Multiline = true;
            this.Date.Name = "Date";
            this.Date.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.Date.SizeF = new System.Drawing.SizeF(128.125F, 25.62498F);
            this.Date.StylePriority.UseTextAlignment = false;
            this.Date.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            this.Date.TextFormatString = "{0:dd/MM/yyyy}";
            // 
            // FirstYear
            // 
            this.FirstYear.LocationFloat = new DevExpress.Utils.PointFloat(354.1667F, 0F);
            this.FirstYear.Multiline = true;
            this.FirstYear.Name = "FirstYear";
            this.FirstYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.FirstYear.SizeF = new System.Drawing.SizeF(105.2084F, 25.62498F);
            this.FirstYear.StylePriority.UseTextAlignment = false;
            this.FirstYear.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // SecondYear
            // 
            this.SecondYear.LocationFloat = new DevExpress.Utils.PointFloat(459.3751F, 0F);
            this.SecondYear.Multiline = true;
            this.SecondYear.Name = "SecondYear";
            this.SecondYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.SecondYear.SizeF = new System.Drawing.SizeF(101.7502F, 25.62498F);
            this.SecondYear.StylePriority.UseTextAlignment = false;
            this.SecondYear.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // ThirdYear
            // 
            this.ThirdYear.LocationFloat = new DevExpress.Utils.PointFloat(561.1252F, 0F);
            this.ThirdYear.Multiline = true;
            this.ThirdYear.Name = "ThirdYear";
            this.ThirdYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.ThirdYear.SizeF = new System.Drawing.SizeF(95.4166F, 25.62498F);
            this.ThirdYear.StylePriority.UseTextAlignment = false;
            this.ThirdYear.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // FourthYear
            // 
            this.FourthYear.LocationFloat = new DevExpress.Utils.PointFloat(656.5418F, 0F);
            this.FourthYear.Multiline = true;
            this.FourthYear.Name = "FourthYear";
            this.FourthYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.FourthYear.SizeF = new System.Drawing.SizeF(88.12494F, 25.62498F);
            this.FourthYear.StylePriority.UseTextAlignment = false;
            this.FourthYear.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // FifthYear
            // 
            this.FifthYear.LocationFloat = new DevExpress.Utils.PointFloat(744.6667F, 0F);
            this.FifthYear.Multiline = true;
            this.FifthYear.Name = "FifthYear";
            this.FifthYear.Padding = new DevExpress.XtraPrinting.PaddingInfo(2, 2, 0, 0, 100F);
            this.FifthYear.SizeF = new System.Drawing.SizeF(103.3332F, 25.62498F);
            this.FifthYear.StylePriority.UseTextAlignment = false;
            this.FifthYear.TextAlignment = DevExpress.XtraPrinting.TextAlignment.TopLeft;
            // 
            // GroupHeader1
            // 
            this.GroupHeader1.Controls.AddRange(new DevExpress.XtraReports.UI.XRControl[] {
            this.lblYear5,
            this.lblYear4,
            this.lblUserName,
            this.lblYear2,
            this.lblYear1,
            this.lblstartdate,
            this.lblYear3});
            this.GroupHeader1.HeightF = 26.16666F;
            this.GroupHeader1.Name = "GroupHeader1";
            // 
            // HolidayReportPrint
            // 
            this.Bands.AddRange(new DevExpress.XtraReports.UI.Band[] {
            this.TopMargin,
            this.BottomMargin,
            this.Detail,
            this.GroupHeader1});
            this.CalculatedFields.AddRange(new DevExpress.XtraReports.UI.CalculatedField[] {
            this.calculatedField1,
            this.EndingYear,
            this.calculatedField2});
            this.Font = new System.Drawing.Font("Arial", 9.75F);
            this.Margins = new System.Drawing.Printing.Margins(2, 0, 63, 1);
            this.Version = "19.1";
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();

        }

        #endregion

        private DevExpress.XtraReports.UI.TopMarginBand TopMargin;
        private DevExpress.XtraReports.UI.BottomMarginBand BottomMargin;
        private DevExpress.XtraReports.UI.DetailBand Detail;
        private DevExpress.XtraReports.UI.XRLabel xrLabel1;
        private DevExpress.XtraReports.UI.XRLabel xrLabel3;
        private DevExpress.XtraReports.UI.XRLabel xrLabel16;
        private DevExpress.XtraReports.UI.CalculatedField calculatedField1;
        private DevExpress.XtraReports.UI.CalculatedField EndingYear;
        private DevExpress.XtraReports.UI.CalculatedField calculatedField2;
        private DevExpress.XtraReports.UI.XRLabel lblstartdate;
        private DevExpress.XtraReports.UI.XRLabel lblUserName;
        public DevExpress.XtraReports.UI.XRLabel lblYear5;
        public DevExpress.XtraReports.UI.XRLabel lblYear4;
        public DevExpress.XtraReports.UI.XRLabel lblYear3;
        public DevExpress.XtraReports.UI.XRLabel lblYear2;
        public DevExpress.XtraReports.UI.XRLabel lblYear1;
        public DevExpress.XtraReports.UI.XRLabel FifthYear;
        public DevExpress.XtraReports.UI.XRLabel UserName;
        public DevExpress.XtraReports.UI.XRLabel ThirdYear;
        public DevExpress.XtraReports.UI.XRLabel SecondYear;
        public DevExpress.XtraReports.UI.XRLabel Date;
        public DevExpress.XtraReports.UI.XRLabel FourthYear;
        public DevExpress.XtraReports.UI.XRLabel FirstYear;
        private DevExpress.XtraReports.UI.GroupHeaderBand GroupHeader1;
    }
}

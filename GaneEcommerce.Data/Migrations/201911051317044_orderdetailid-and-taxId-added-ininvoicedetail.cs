namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderdetailidandtaxIdaddedininvoicedetail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceDetail", "OrderDetailId", c => c.Int());
            AddColumn("dbo.InvoiceDetail", "TaxId", c => c.Int());
            CreateIndex("dbo.InvoiceDetail", "OrderDetailId");
            CreateIndex("dbo.InvoiceDetail", "TaxId");
            AddForeignKey("dbo.InvoiceDetail", "TaxId", "dbo.GlobalTax", "TaxID");
            AddForeignKey("dbo.InvoiceDetail", "OrderDetailId", "dbo.OrderDetails", "OrderDetailID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.InvoiceDetail", "OrderDetailId", "dbo.OrderDetails");
            DropForeignKey("dbo.InvoiceDetail", "TaxId", "dbo.GlobalTax");
            DropIndex("dbo.InvoiceDetail", new[] { "TaxId" });
            DropIndex("dbo.InvoiceDetail", new[] { "OrderDetailId" });
            DropColumn("dbo.InvoiceDetail", "TaxId");
            DropColumn("dbo.InvoiceDetail", "OrderDetailId");
        }
    }
}

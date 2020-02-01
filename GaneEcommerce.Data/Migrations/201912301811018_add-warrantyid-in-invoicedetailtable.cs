namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addwarrantyidininvoicedetailtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InvoiceDetail", "WarrantyId", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.InvoiceDetail", "WarrantyId");
        }
    }
}

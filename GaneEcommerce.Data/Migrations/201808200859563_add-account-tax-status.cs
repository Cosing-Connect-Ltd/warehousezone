namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addaccounttaxstatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Account", "TaxID", c => c.Int(nullable: false, defaultValue: 1));
            CreateIndex("dbo.Account", "TaxID");
            AddForeignKey("dbo.Account", "TaxID", "dbo.GlobalTax", "TaxID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Account", "TaxID", "dbo.GlobalTax");
            DropIndex("dbo.Account", new[] { "TaxID" });
            DropColumn("dbo.Account", "TaxID");
        }
    }
}

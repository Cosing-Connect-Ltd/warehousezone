namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountReceipientIdnullableinPallets : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Pallets", new[] { "RecipientAccountID" });
            AlterColumn("dbo.Pallets", "RecipientAccountID", c => c.Int());
            CreateIndex("dbo.Pallets", "RecipientAccountID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Pallets", new[] { "RecipientAccountID" });
            AlterColumn("dbo.Pallets", "RecipientAccountID", c => c.Int(nullable: false));
            CreateIndex("dbo.Pallets", "RecipientAccountID");
        }
    }
}

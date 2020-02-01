namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AccountPaymentModeIdaddedinorders : DbMigration
    {
        public override void Up()
        {
            CreateIndex("dbo.Orders", "AccountPaymentModeId");
            AddForeignKey("dbo.Orders", "AccountPaymentModeId", "dbo.AccountPaymentModes", "AccountPaymentModeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "AccountPaymentModeId", "dbo.AccountPaymentModes");
            DropIndex("dbo.Orders", new[] { "AccountPaymentModeId" });
        }
    }
}

namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productGroupIdaddedinorderdetailtable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "ProductGroupId", c => c.Int());
            CreateIndex("dbo.OrderDetails", "ProductGroupId");
            AddForeignKey("dbo.OrderDetails", "ProductGroupId", "dbo.ProductGroups", "ProductGroupId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderDetails", "ProductGroupId", "dbo.ProductGroups");
            DropIndex("dbo.OrderDetails", new[] { "ProductGroupId" });
            DropColumn("dbo.OrderDetails", "ProductGroupId");
        }
    }
}

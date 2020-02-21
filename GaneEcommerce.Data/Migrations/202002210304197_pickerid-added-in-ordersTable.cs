namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class pickeridaddedinordersTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "PickerId", c => c.Int());
            CreateIndex("dbo.Orders", "PickerId");
            AddForeignKey("dbo.Orders", "PickerId", "dbo.AuthUsers", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "PickerId", "dbo.AuthUsers");
            DropIndex("dbo.Orders", new[] { "PickerId" });
            DropColumn("dbo.Orders", "PickerId");
        }
    }
}

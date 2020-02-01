namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sentmethodsremoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PalletsDispatches", "SentMethodID", "dbo.SentMethods");
            DropIndex("dbo.PalletsDispatches", new[] { "SentMethodID" });
            AddColumn("dbo.PalletsDispatches", "DeliveryMethod", c => c.Int());
            DropColumn("dbo.PalletsDispatches", "SentMethodID");
            DropTable("dbo.SentMethods");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.SentMethods",
                c => new
                    {
                        SentMethodID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TrackUrl = c.String(),
                    })
                .PrimaryKey(t => t.SentMethodID);
            
            AddColumn("dbo.PalletsDispatches", "SentMethodID", c => c.Int());
            DropColumn("dbo.PalletsDispatches", "DeliveryMethod");
            CreateIndex("dbo.PalletsDispatches", "SentMethodID");
            AddForeignKey("dbo.PalletsDispatches", "SentMethodID", "dbo.SentMethods", "SentMethodID");
        }
    }
}

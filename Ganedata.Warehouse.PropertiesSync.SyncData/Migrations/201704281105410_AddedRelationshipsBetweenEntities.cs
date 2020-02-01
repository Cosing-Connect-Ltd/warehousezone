namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRelationshipsBetweenEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PProperty", "CurrentLandlordCode", c => c.String());
            AddColumn("dbo.PProperty", "PropertyLandlord_PLandlordId", c => c.Int());
            AddColumn("dbo.PTenant", "CurrentPropertyCode", c => c.String());
            AddColumn("dbo.PTenant", "CurrentProperty_PPropertyId", c => c.Int());
            CreateIndex("dbo.PProperty", "PropertyLandlord_PLandlordId");
            CreateIndex("dbo.PTenant", "CurrentProperty_PPropertyId");
            AddForeignKey("dbo.PProperty", "PropertyLandlord_PLandlordId", "dbo.PLandlord", "PLandlordId");
            AddForeignKey("dbo.PTenant", "CurrentProperty_PPropertyId", "dbo.PProperty", "PPropertyId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PTenant", "CurrentProperty_PPropertyId", "dbo.PProperty");
            DropForeignKey("dbo.PProperty", "PropertyLandlord_PLandlordId", "dbo.PLandlord");
            DropIndex("dbo.PTenant", new[] { "CurrentProperty_PPropertyId" });
            DropIndex("dbo.PProperty", new[] { "PropertyLandlord_PLandlordId" });
            DropColumn("dbo.PTenant", "CurrentProperty_PPropertyId");
            DropColumn("dbo.PTenant", "CurrentPropertyCode");
            DropColumn("dbo.PProperty", "PropertyLandlord_PLandlordId");
            DropColumn("dbo.PProperty", "CurrentLandlordCode");
        }
    }
}

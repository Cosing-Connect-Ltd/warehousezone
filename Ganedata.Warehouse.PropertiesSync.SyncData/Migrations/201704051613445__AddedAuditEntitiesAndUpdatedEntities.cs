namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _AddedAuditEntitiesAndUpdatedEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PLandlord", "LandlordSalutation", c => c.String());
            AddColumn("dbo.PLandlord", "LandlordAdded", c => c.DateTime());
            AddColumn("dbo.PLandlord", "SiteId", c => c.Int(nullable: false));
            AddColumn("dbo.PLandlord", "SyncRequiredFlag", c => c.Boolean(nullable: false));
            AddColumn("dbo.PProperty", "PropertyCode", c => c.String());
            AddColumn("dbo.PProperty", "AddressLine1", c => c.String());
            AddColumn("dbo.PProperty", "AddressLine2", c => c.String());
            AddColumn("dbo.PProperty", "AddressLine3", c => c.String());
            AddColumn("dbo.PProperty", "AddressLine4", c => c.String());
            AddColumn("dbo.PProperty", "AddressLine5", c => c.String());
            AddColumn("dbo.PProperty", "AddressPostcode", c => c.String());
            AddColumn("dbo.PProperty", "SiteId", c => c.Int(nullable: false));
            AddColumn("dbo.PProperty", "SyncRequiredFlag", c => c.Boolean(nullable: false));
            AlterColumn("dbo.PProperty", "DateAvailable", c => c.DateTime());
            AlterColumn("dbo.PProperty", "DateAdded", c => c.DateTime());
            AlterColumn("dbo.PProperty", "LetDate", c => c.DateTime());
            AlterColumn("dbo.PProperty", "TenancyMonths", c => c.Double());
            DropColumn("dbo.PProperty", "PropertyAddressLine1");
            DropColumn("dbo.PProperty", "PropertyAddressLine2");
            DropColumn("dbo.PProperty", "PropertyAddressLine3");
            DropColumn("dbo.PProperty", "PropertyAddressLine4");
            DropColumn("dbo.PProperty", "PropertyAddressLine5");
            DropColumn("dbo.PProperty", "PropertyAddressPostcode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PProperty", "PropertyAddressPostcode", c => c.String());
            AddColumn("dbo.PProperty", "PropertyAddressLine5", c => c.String());
            AddColumn("dbo.PProperty", "PropertyAddressLine4", c => c.String());
            AddColumn("dbo.PProperty", "PropertyAddressLine3", c => c.String());
            AddColumn("dbo.PProperty", "PropertyAddressLine2", c => c.String());
            AddColumn("dbo.PProperty", "PropertyAddressLine1", c => c.String());
            AlterColumn("dbo.PProperty", "TenancyMonths", c => c.Int(nullable: false));
            AlterColumn("dbo.PProperty", "LetDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PProperty", "DateAdded", c => c.DateTime(nullable: false));
            AlterColumn("dbo.PProperty", "DateAvailable", c => c.DateTime(nullable: false));
            DropColumn("dbo.PProperty", "SyncRequiredFlag");
            DropColumn("dbo.PProperty", "SiteId");
            DropColumn("dbo.PProperty", "AddressPostcode");
            DropColumn("dbo.PProperty", "AddressLine5");
            DropColumn("dbo.PProperty", "AddressLine4");
            DropColumn("dbo.PProperty", "AddressLine3");
            DropColumn("dbo.PProperty", "AddressLine2");
            DropColumn("dbo.PProperty", "AddressLine1");
            DropColumn("dbo.PProperty", "PropertyCode");
            DropColumn("dbo.PLandlord", "SyncRequiredFlag");
            DropColumn("dbo.PLandlord", "SiteId");
            DropColumn("dbo.PLandlord", "LandlordAdded");
            DropColumn("dbo.PLandlord", "LandlordSalutation");
        }
    }
}

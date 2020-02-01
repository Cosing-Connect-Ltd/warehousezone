namespace Ganedata.Warehouse.PropertiesSync.SyncData.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialRun : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PLandlord",
                c => new
                    {
                        PLandlordId = c.Int(nullable: false, identity: true),
                        LandlordCode = c.String(),
                        LandlordFullname = c.String(),
                        LandlordStatus = c.String(),
                        LandlordNotes1 = c.String(),
                        LandlordNotes2 = c.String(),
                        UserNotes1 = c.String(),
                        UserNotes2 = c.String(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        AddressPostcode = c.String(),
                        HomeTelephone = c.String(),
                        WorkTelephone1 = c.String(),
                        WorkTelephone2 = c.String(),
                        WorkTelephoneFax = c.String(),
                        MobileNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.PLandlordId);
            
            CreateTable(
                "dbo.PProperty",
                c => new
                    {
                        PPropertyId = c.Int(nullable: false, identity: true),
                        PropertyAddressLine1 = c.String(),
                        PropertyAddressLine2 = c.String(),
                        PropertyAddressLine3 = c.String(),
                        PropertyAddressLine4 = c.String(),
                        PropertyAddressLine5 = c.String(),
                        PropertyAddressPostcode = c.String(),
                        PropertyStatus = c.String(),
                        IsVacant = c.Boolean(nullable: false),
                        DateAvailable = c.DateTime(nullable: false),
                        DateAdded = c.DateTime(nullable: false),
                        LetDate = c.DateTime(nullable: false),
                        PropertyBranch = c.String(),
                        TenancyMonths = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PPropertyId);
            
            CreateTable(
                "dbo.PTenant",
                c => new
                    {
                        PTenantId = c.Int(nullable: false, identity: true),
                        TenantCode = c.String(),
                        TenantYCode = c.String(),
                        TenantFullName = c.String(),
                        TenantSalutation = c.String(),
                        TenancyStatus = c.String(),
                        TenancyCategory = c.Int(),
                        TenancyAdded = c.DateTime(),
                        TenancyStarted = c.DateTime(),
                        TenancyRenewDate = c.DateTime(),
                        TenancyPeriodMonths = c.Double(),
                        SiteId = c.Int(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        AddressPostcode = c.String(),
                        HomeTelephone = c.String(),
                        WorkTelephone1 = c.String(),
                        WorkTelephone2 = c.String(),
                        WorkTelephoneFax = c.String(),
                        MobileNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.PTenantId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PTenant");
            DropTable("dbo.PProperty");
            DropTable("dbo.PLandlord");
        }
    }
}

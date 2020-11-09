namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedsomeLoyaltyAppconfigstoTenantConfigstable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TenantConfigs", "LoyaltyAppSplashScreenImage", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppPrimaryColour", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppSecondaryColour", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppPrimaryTextColour", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppSecondaryTextColour", c => c.String());
            AddColumn("dbo.TenantConfigs", "LoyaltyAppAboutUsPageText", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TenantConfigs", "LoyaltyAppAboutUsPageText");
            DropColumn("dbo.TenantConfigs", "LoyaltyAppSecondaryTextColour");
            DropColumn("dbo.TenantConfigs", "LoyaltyAppPrimaryTextColour");
            DropColumn("dbo.TenantConfigs", "LoyaltyAppSecondaryColour");
            DropColumn("dbo.TenantConfigs", "LoyaltyAppPrimaryColour");
            DropColumn("dbo.TenantConfigs", "LoyaltyAppSplashScreenImage");
        }
    }
}

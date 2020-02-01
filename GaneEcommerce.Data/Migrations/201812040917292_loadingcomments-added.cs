namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class loadingcommentsadded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.VehicleInspection", "LoadingComments", c => c.String());
            AlterColumn("dbo.TenantEmailTemplates", "Body", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TenantEmailTemplates", "Body", c => c.String());
            DropColumn("dbo.VehicleInspection", "LoadingComments");
        }
    }
}

namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ResourceRequestChange : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ResourceRequests", "RequestStatus", c => c.Int(nullable: false));
            AddColumn("dbo.ResourceRequests", "ActionedBy", c => c.Int());
            AddColumn("dbo.ResourceRequests", "ActionReason", c => c.String());
            DropColumn("dbo.ResourceRequests", "IsAccepted");
            DropColumn("dbo.ResourceRequests", "IsAnnualHoliday");
            DropColumn("dbo.ResourceRequests", "AcceptedBy");
            DropColumn("dbo.ResourceRequests", "IsCanceled");
            DropColumn("dbo.ResourceRequests", "CancelReason");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ResourceRequests", "CancelReason", c => c.String());
            AddColumn("dbo.ResourceRequests", "IsCanceled", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResourceRequests", "AcceptedBy", c => c.Int());
            AddColumn("dbo.ResourceRequests", "IsAnnualHoliday", c => c.Boolean(nullable: false));
            AddColumn("dbo.ResourceRequests", "IsAccepted", c => c.Boolean(nullable: false));
            DropColumn("dbo.ResourceRequests", "ActionReason");
            DropColumn("dbo.ResourceRequests", "ActionedBy");
            DropColumn("dbo.ResourceRequests", "RequestStatus");
        }
    }
}

namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_preorder_feature_to_products : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "IsPreOrderAccepted", c => c.Boolean());
            AddColumn("dbo.ProductMaster", "MinDispatchDays", c => c.Int());
            AddColumn("dbo.ProductMaster", "MaxDispatchDays", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ProductMaster", "MaxDispatchDays");
            DropColumn("dbo.ProductMaster", "MinDispatchDays");
            DropColumn("dbo.ProductMaster", "IsPreOrderAccepted");
        }
    }
}

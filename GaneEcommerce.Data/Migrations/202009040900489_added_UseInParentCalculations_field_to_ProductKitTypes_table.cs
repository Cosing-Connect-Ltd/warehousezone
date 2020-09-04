namespace Ganedata.Core.Data.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class added_UseInParentCalculations_field_to_ProductKitTypes_table : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductKitTypes", "UseInParentCalculations", c => c.Boolean());
        }

        public override void Down()
        {
            DropColumn("dbo.ProductKitTypes", "UseInParentCalculations");
        }
    }
}

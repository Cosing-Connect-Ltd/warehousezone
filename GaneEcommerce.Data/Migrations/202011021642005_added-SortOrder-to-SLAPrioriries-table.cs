namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class addedSortOrdertoSLAPrioririestable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SLAPriorits", "SortOrder", c => c.Int(nullable: true));
        }

        public override void Down()
        {
            DropColumn("dbo.SLAPriorits", "SortOrder");
        }
    }
}

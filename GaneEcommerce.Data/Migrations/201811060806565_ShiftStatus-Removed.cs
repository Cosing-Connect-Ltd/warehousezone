namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ShiftStatusRemoved : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ResourceShifts", "ShiftStatusId", "dbo.ShiftStatus");
            DropIndex("dbo.ResourceShifts", new[] { "ShiftStatusId" });
            DropTable("dbo.ShiftStatus");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.ShiftStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateIndex("dbo.ResourceShifts", "ShiftStatusId");
            AddForeignKey("dbo.ResourceShifts", "ShiftStatusId", "dbo.ShiftStatus", "Id");
        }
    }
}

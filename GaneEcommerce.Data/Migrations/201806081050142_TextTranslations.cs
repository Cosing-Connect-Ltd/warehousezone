namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TextTranslations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TextTranslations",
                c => new
                    {
                        Culture = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 128),
                        Value = c.String(),
                    })
                .PrimaryKey(t => new { t.Culture, t.Name });
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TextTranslations");
        }
    }
}

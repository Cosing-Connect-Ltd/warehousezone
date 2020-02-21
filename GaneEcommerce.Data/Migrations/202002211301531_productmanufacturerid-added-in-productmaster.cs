namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class productmanufactureridaddedinproductmaster : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductMaster", "ManufacturerId", c => c.Int());
            CreateIndex("dbo.ProductMaster", "ManufacturerId");
            AddForeignKey("dbo.ProductMaster", "ManufacturerId", "dbo.ProductManufacturers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProductMaster", "ManufacturerId", "dbo.ProductManufacturers");
            DropIndex("dbo.ProductMaster", new[] { "ManufacturerId" });
            DropColumn("dbo.ProductMaster", "ManufacturerId");
        }
    }
}

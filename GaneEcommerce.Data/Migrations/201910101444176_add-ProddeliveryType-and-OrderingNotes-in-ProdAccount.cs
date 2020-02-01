namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addProddeliveryTypeandOrderingNotesinProdAccount : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ProductAccountCodes", "ProdOrderingNotes", c => c.String());
            AddColumn("dbo.ProductAccountCodes", "ProdDeliveryType", c => c.Int());
            
        }
        
        public override void Down()
        {
           
            DropColumn("dbo.ProductAccountCodes", "ProdDeliveryType");
            DropColumn("dbo.ProductAccountCodes", "ProdOrderingNotes");
        }
    }
}

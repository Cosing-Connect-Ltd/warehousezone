﻿namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRefundNotesToPaymentLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AdyenOrderPaylinks", "RefundNotes", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AdyenOrderPaylinks", "RefundNotes");
        }
    }
}

﻿// <auto-generated />
namespace Ganedata.Core.Data.Migrations
{
    using System.CodeDom.Compiler;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.Infrastructure;
    using System.Resources;
    
    [GeneratedCode("EntityFramework.Migrations", "6.4.4")]
    public sealed partial class add_TenantDeliveryServiceId_to_orders_table : IMigrationMetadata
    {
        private readonly ResourceManager Resources = new ResourceManager(typeof(add_TenantDeliveryServiceId_to_orders_table));
        
        string IMigrationMetadata.Id
        {
            get { return "202009241539597_add_TenantDeliveryServiceId_to_orders_table"; }
        }
        
        string IMigrationMetadata.Source
        {
            get { return null; }
        }
        
        string IMigrationMetadata.Target
        {
            get { return Resources.GetString("Target"); }
        }
    }
}
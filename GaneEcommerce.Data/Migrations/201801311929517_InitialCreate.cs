namespace Ganedata.Core.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        AccountID = c.Int(nullable: false, identity: true),
                        AccountCode = c.String(),
                        CompanyName = c.String(nullable: false),
                        CountryID = c.Int(nullable: false),
                        CurrencyID = c.Int(nullable: false),
                        AccountStatusID = c.Int(nullable: false),
                        PriceGroupID = c.Int(nullable: false),
                        VATNo = c.String(maxLength: 50),
                        RegNo = c.String(maxLength: 50),
                        Comments = c.String(),
                        AccountEmail = c.String(),
                        Telephone = c.String(),
                        Fax = c.String(),
                        Mobile = c.String(),
                        website = c.String(),
                        CreditLimit = c.Double(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                        AccountTypeCustomer = c.Boolean(nullable: false),
                        AccountTypeSupplier = c.Boolean(nullable: false),
                        AccountTypeEndUser = c.Boolean(nullable: false),
                        OwnerUserId = c.Int(nullable: false),
                        FinalBalance = c.Decimal(precision: 18, scale: 2),
                        DateBalanceUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.AccountID)
                .ForeignKey("dbo.GlobalAccountStatus", t => t.AccountStatusID)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .ForeignKey("dbo.GlobalCurrency", t => t.CurrencyID)
                .ForeignKey("dbo.TenantPriceGroups", t => t.PriceGroupID)
                .Index(t => t.CountryID)
                .Index(t => t.CurrencyID)
                .Index(t => t.AccountStatusID)
                .Index(t => t.PriceGroupID);
            
            CreateTable(
                "dbo.AccountAddresses",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        AddressLine1 = c.String(nullable: false),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        Telephone = c.String(),
                        Town = c.String(),
                        County = c.String(),
                        PostCode = c.String(),
                        CountryID = c.Int(nullable: false),
                        AccountID = c.Int(nullable: false),
                        AddTypeDefault = c.Boolean(),
                        AddTypeShipping = c.Boolean(),
                        AddTypeBilling = c.Boolean(),
                        AddTypeMarketing = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .Index(t => t.CountryID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.GlobalCountry",
                c => new
                    {
                        CountryID = c.Int(nullable: false, identity: true),
                        CountryCode = c.String(nullable: false, maxLength: 4),
                        CountryName = c.String(nullable: false, maxLength: 200),
                    })
                .PrimaryKey(t => t.CountryID);
            
            CreateTable(
                "dbo.GlobalCurrency",
                c => new
                    {
                        CurrencyID = c.Int(nullable: false, identity: true),
                        CurrencyName = c.String(nullable: false, maxLength: 100),
                        Symbol = c.String(),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CurrencyID)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.AccountContacts",
                c => new
                    {
                        AccountContactId = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
                        ContactName = c.String(nullable: false, maxLength: 200),
                        ContactJobTitle = c.String(maxLength: 200),
                        ContactEmail = c.String(maxLength: 200),
                        TenantContactPhone = c.String(maxLength: 50),
                        TenantContactPin = c.Short(nullable: false),
                        ConTypeExecutive = c.Boolean(nullable: false),
                        ConTypeAdmin = c.Boolean(nullable: false),
                        ConTypeBilling = c.Boolean(nullable: false),
                        ConTypeMarketing = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AccountContactId)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .Index(t => t.AccountID);
            
            CreateTable(
                "dbo.AccountTransaction",
                c => new
                    {
                        AccountTransactionId = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        Notes = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FinalBalance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderId = c.Int(),
                        OrderProcessId = c.Int(),
                        AccountPaymentModeId = c.Int(),
                        AccountTransactionTypeId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AccountTransactionId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.AccountPaymentModes", t => t.AccountPaymentModeId)
                .ForeignKey("dbo.AccountTransactionTypes", t => t.AccountTransactionTypeId)
                .Index(t => t.AccountId)
                .Index(t => t.AccountPaymentModeId)
                .Index(t => t.AccountTransactionTypeId);
            
            CreateTable(
                "dbo.AccountPaymentModes",
                c => new
                    {
                        AccountPaymentModeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AccountPaymentModeId);
            
            CreateTable(
                "dbo.AccountTransactionTypes",
                c => new
                    {
                        AccountTransactionTypeId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.AccountTransactionTypeId);
            
            CreateTable(
                "dbo.GlobalAccountStatus",
                c => new
                    {
                        AccountStatusID = c.Int(nullable: false, identity: true),
                        AccountStatus = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.AccountStatusID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        OrderID = c.Int(nullable: false, identity: true),
                        OrderNumber = c.String(nullable: false),
                        IssueDate = c.DateTime(storeType: "date"),
                        ExpectedDate = c.DateTime(storeType: "date"),
                        Note = c.String(),
                        InventoryTransactionTypeId = c.Int(nullable: false),
                        AccountID = c.Int(),
                        JobTypeId = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CancelDate = c.DateTime(),
                        ConfirmDate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        ConfirmBy = c.Int(),
                        CancelBy = c.Int(),
                        TenentId = c.Int(nullable: false),
                        IsCancel = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        OrderStatusID = c.Int(nullable: false),
                        LoanID = c.Int(),
                        AccountContactId = c.Int(),
                        OrderTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Posted = c.Boolean(nullable: false),
                        InvoiceNo = c.String(),
                        InvoiceDetails = c.String(),
                        OrderCost = c.Decimal(precision: 18, scale: 2),
                        ReportTypeId = c.Int(),
                        ReportTypeChargeId = c.Int(),
                        TransferFrom_WarehouseId = c.Int(),
                        TransferTo_WarehouseId = c.Int(),
                        DepartmentId = c.Int(),
                        SLAPriorityId = c.Int(),
                        ExpectedHours = c.Short(),
                        AuthorisedDate = c.DateTime(),
                        AuthorisedUserID = c.Int(),
                        AuthorisedNotes = c.String(),
                        WarehouseId = c.Int(),
                        ShipmentAddressLine1 = c.String(),
                        ShipmentAddressLine2 = c.String(),
                        ShipmentAddressLine3 = c.String(),
                        ShipmentAddressLine4 = c.String(),
                        ShipmentAddressPostcode = c.String(),
                        PPropertyId = c.Int(),
                        ShipmentPropertyId = c.Int(),
                        OrderGroupToken = c.Guid(),
                        ShipmentWarehouseId = c.Int(),
                        IsShippedToTenantMainLocation = c.Boolean(nullable: false),
                        CustomEmailRecipient = c.String(),
                        CustomCCEmailRecipient = c.String(),
                        CustomBCCEmailRecipient = c.String(),
                        AccountCurrencyID = c.Int(),
                        JobSubTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.OrderID)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.AccountContacts", t => t.AccountContactId)
                .ForeignKey("dbo.GlobalCurrency", t => t.AccountCurrencyID)
                .ForeignKey("dbo.OrderStatus", t => t.OrderStatusID)
                .ForeignKey("dbo.JobTypes", t => t.JobTypeId)
                .ForeignKey("dbo.TenantDepartments", t => t.DepartmentId)
                .ForeignKey("dbo.JobSubTypes", t => t.JobSubTypeId)
                .ForeignKey("dbo.ReportTypes", t => t.ReportTypeId)
                .ForeignKey("dbo.PProperties", t => t.ShipmentPropertyId)
                .ForeignKey("dbo.TenantLocations", t => t.ShipmentWarehouseId)
                .ForeignKey("dbo.SLAPriorits", t => t.SLAPriorityId)
                .ForeignKey("dbo.Tenants", t => t.TenentId)
                .ForeignKey("dbo.InventoryTransactionTypes", t => t.InventoryTransactionTypeId)
                .ForeignKey("dbo.TenantLocations", t => t.TransferFrom_WarehouseId)
                .ForeignKey("dbo.TenantLocations", t => t.TransferTo_WarehouseId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.InventoryTransactionTypeId)
                .Index(t => t.AccountID)
                .Index(t => t.JobTypeId)
                .Index(t => t.TenentId)
                .Index(t => t.OrderStatusID)
                .Index(t => t.AccountContactId)
                .Index(t => t.ReportTypeId)
                .Index(t => t.TransferFrom_WarehouseId)
                .Index(t => t.TransferTo_WarehouseId)
                .Index(t => t.DepartmentId)
                .Index(t => t.SLAPriorityId)
                .Index(t => t.WarehouseId)
                .Index(t => t.ShipmentPropertyId)
                .Index(t => t.ShipmentWarehouseId)
                .Index(t => t.AccountCurrencyID)
                .Index(t => t.JobSubTypeId);
            
            CreateTable(
                "dbo.Appointments",
                c => new
                    {
                        AppointmentId = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Subject = c.String(),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        Label = c.Int(nullable: false),
                        Location = c.String(),
                        AllDay = c.Boolean(nullable: false),
                        EventType = c.Int(nullable: false),
                        RecurrenceInfo = c.String(),
                        ReminderInfo = c.String(),
                        ResourceId = c.Int(),
                        ResourceIDs = c.String(),
                        OrderId = c.Int(),
                        TenentId = c.Int(),
                        IsCanceled = c.Boolean(nullable: false),
                        CancelReason = c.String(),
                    })
                .PrimaryKey(t => t.AppointmentId)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.ResourceId)
                .Index(t => t.OrderId);
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ResourceId = c.Int(nullable: false, identity: true),
                        Title = c.Int(),
                        FirstName = c.String(),
                        MiddleName = c.String(),
                        SurName = c.String(),
                        LikeToBeKnownAs = c.String(),
                        Gender = c.Int(),
                        Married = c.Boolean(nullable: false),
                        Nationality = c.Int(),
                        HourlyRate = c.Decimal(precision: 18, scale: 2),
                        Color = c.String(),
                        JobDescription = c.String(),
                        InternalStaff = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        AddressId = c.Int(),
                        ContactNumbersId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ResourceId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.ContactNumbers", t => t.ContactNumbersId)
                .ForeignKey("dbo.GlobalCountry", t => t.Nationality)
                .Index(t => t.Nationality)
                .Index(t => t.AddressId)
                .Index(t => t.ContactNumbersId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        HouseNumber = c.String(),
                        PostCode = c.String(),
                        Town = c.String(),
                        County = c.String(),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.ResourceShifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeShiftID = c.String(),
                        Date = c.DateTime(nullable: false),
                        WeekNumber = c.Int(nullable: false),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        ShiftStatusId = c.Int(),
                        StatusType = c.String(),
                        ResourceId = c.Int(),
                        TerminalId = c.Int(nullable: false),
                        TimeStamp = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.ShiftStatus", t => t.ShiftStatusId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.ShiftStatusId)
                .Index(t => t.ResourceId)
                .Index(t => t.TerminalId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ShiftStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Tenants",
                c => new
                    {
                        TenantId = c.Int(nullable: false, identity: true),
                        TenantName = c.String(maxLength: 100),
                        TenantNo = c.String(),
                        TenantVatNo = c.String(maxLength: 50),
                        TenantAccountReference = c.String(maxLength: 50),
                        TenantWebsite = c.String(maxLength: 250),
                        TenantDayPhone = c.String(maxLength: 50),
                        TenantEveningPhone = c.String(maxLength: 50),
                        TenantMobilePhone = c.String(maxLength: 50),
                        TenantFax = c.String(maxLength: 50),
                        TenantEmail = c.String(maxLength: 200),
                        TenantAddress1 = c.String(maxLength: 200),
                        TenantAddress2 = c.String(maxLength: 200),
                        TenantAddress3 = c.String(maxLength: 200),
                        TenantAddress4 = c.String(maxLength: 200),
                        TenantCity = c.String(maxLength: 200),
                        TenantStateCounty = c.String(maxLength: 200),
                        TenantPostalCode = c.String(maxLength: 50),
                        TenantCountry = c.String(),
                        CurrencyID = c.Int(nullable: false),
                        TenantSubDmoain = c.String(maxLength: 50),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        CountryID = c.Int(),
                        AccountNumber = c.String(),
                        ProductCodePrefix = c.String(),
                    })
                .PrimaryKey(t => t.TenantId);
            
            CreateTable(
                "dbo.AuthUsers",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        UserPassword = c.String(),
                        UserFirstName = c.String(maxLength: 50),
                        UserLastName = c.String(maxLength: 50),
                        UserEmail = c.String(maxLength: 200),
                        IsActive = c.Boolean(nullable: false),
                        SuperUser = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.AuthPermissions",
                c => new
                    {
                        PermissionId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        ActivityId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.PermissionId)
                .ForeignKey("dbo.AuthActivities", t => t.ActivityId)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.UserId)
                .Index(t => t.WarehouseId)
                .Index(t => t.ActivityId);
            
            CreateTable(
                "dbo.AuthActivities",
                c => new
                    {
                        ActivityId = c.Int(nullable: false, identity: true),
                        ActivityName = c.String(maxLength: 200),
                        ActivityController = c.String(),
                        ActivityAction = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        RightNav = c.Boolean(),
                        ExcludePermission = c.Boolean(),
                        SuperAdmin = c.Boolean(),
                        SortOrder = c.Int(nullable: false),
                        ModuleId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ActivityId)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.AuthActivityGroupMaps",
                c => new
                    {
                        ActivityGroupMapId = c.Int(nullable: false, identity: true),
                        ActivityId = c.Int(nullable: false),
                        ActivityGroupId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ActivityGroupMapId)
                .ForeignKey("dbo.AuthActivities", t => t.ActivityId)
                .ForeignKey("dbo.AuthActivityGroups", t => t.ActivityGroupId)
                .Index(t => t.ActivityId)
                .Index(t => t.ActivityGroupId);
            
            CreateTable(
                "dbo.AuthActivityGroups",
                c => new
                    {
                        ActivityGroupId = c.Int(nullable: false, identity: true),
                        ActivityGroupName = c.String(maxLength: 200),
                        ActivityGroupDetail = c.String(maxLength: 1000),
                        ActivityGroupParentId = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        GroupIcon = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ActivityGroupId);
            
            CreateTable(
                "dbo.Modules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleName = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TenantLocations",
                c => new
                    {
                        WarehouseId = c.Int(nullable: false, identity: true),
                        WarehouseName = c.String(maxLength: 200),
                        AddressLine1 = c.String(maxLength: 200),
                        AddressLine2 = c.String(maxLength: 200),
                        AddressLine3 = c.String(maxLength: 200),
                        CountyState = c.String(maxLength: 200),
                        PostalCode = c.String(maxLength: 50),
                        CountryID = c.Int(nullable: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        IsActive = c.Boolean(nullable: false),
                        SortOrder = c.Int(nullable: false),
                        Name = c.String(),
                        AddressId = c.Int(),
                        ContactNumbersId = c.Int(),
                        MinimumDrivers = c.Int(nullable: false),
                        MinimumKitchenStaff = c.Int(nullable: false),
                        MinimumGeneralStaff = c.Int(nullable: false),
                        IsMobile = c.Boolean(),
                        AutoTransferOrders = c.Boolean(),
                        MonitorStockVariance = c.Boolean(),
                        MarketVehicleID = c.Int(),
                        ParentWarehouseId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.WarehouseId)
                .ForeignKey("dbo.Addresses", t => t.AddressId)
                .ForeignKey("dbo.ContactNumbers", t => t.ContactNumbersId)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .ForeignKey("dbo.MarketVehicles", t => t.MarketVehicleID)
                .ForeignKey("dbo.TenantLocations", t => t.ParentWarehouseId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.CountryID)
                .Index(t => t.AddressId)
                .Index(t => t.ContactNumbersId)
                .Index(t => t.MarketVehicleID)
                .Index(t => t.ParentWarehouseId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ContactNumbers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        MobileNumber = c.String(),
                        HomeNumber = c.String(),
                        WorkNumber = c.String(),
                        EmailAddress = c.String(),
                        Fax = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Terminals",
                c => new
                    {
                        TerminalId = c.Int(nullable: false, identity: true),
                        TerminalName = c.String(maxLength: 50),
                        TermainlSerial = c.String(),
                        WarehouseId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TerminalId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.WarehouseId);
            
            CreateTable(
                "dbo.TerminalsLogs",
                c => new
                    {
                        TerminalLogId = c.Guid(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        TerminalLogType = c.String(maxLength: 50),
                        DateRequest = c.DateTime(nullable: false),
                        SentCount = c.Int(),
                        Ack = c.Boolean(),
                        RecievedCount = c.Int(),
                        Response = c.String(),
                        ResponseText = c.String(),
                        AdditonalInfo = c.String(),
                        clientIp = c.String(),
                        ServerIp = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                    })
                .PrimaryKey(t => t.TerminalLogId)
                .ForeignKey("dbo.Terminals", t => t.TerminalId)
                .Index(t => t.TerminalId);
            
            CreateTable(
                "dbo.EmployeeShifts_Stores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        WarehouseId = c.Int(nullable: false),
                        ResourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.WarehouseId)
                .Index(t => t.ResourceId);
            
            CreateTable(
                "dbo.InventoryStocks",
                c => new
                    {
                        InventoryStockId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        InStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OnOrder = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Allocated = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Available = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.InventoryStockId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.WarehouseId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ProductMaster",
                c => new
                    {
                        ProductId = c.Int(nullable: false, identity: true),
                        SKUCode = c.String(),
                        SecondCode = c.String(),
                        Name = c.String(nullable: false),
                        Description = c.String(unicode: false, storeType: "text"),
                        BarCode = c.String(),
                        BarCode2 = c.String(),
                        UOMId = c.Int(nullable: false),
                        Serialisable = c.Boolean(nullable: false),
                        AllowZeroSale = c.Boolean(),
                        LotOption = c.Boolean(nullable: false),
                        LotOptionCodeId = c.Int(nullable: false),
                        LotProcessTypeCodeId = c.Int(nullable: false),
                        ShelfLifeDays = c.Int(),
                        ReorderQty = c.Decimal(precision: 18, scale: 2),
                        ShipConditionCode = c.String(maxLength: 50),
                        CommodityCode = c.String(maxLength: 50),
                        CommodityClass = c.String(maxLength: 50),
                        DimensionUOMId = c.Int(nullable: false),
                        WeightGroupId = c.Int(nullable: false),
                        Height = c.Double(nullable: false),
                        Width = c.Double(nullable: false),
                        Depth = c.Double(nullable: false),
                        Weight = c.Double(nullable: false),
                        TaxID = c.Int(nullable: false),
                        BuyPrice = c.Decimal(precision: 18, scale: 2),
                        LandedCost = c.Decimal(precision: 18, scale: 2),
                        SellPrice = c.Decimal(precision: 18, scale: 2),
                        MinThresholdPrice = c.Decimal(precision: 18, scale: 2),
                        PercentMargin = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Kit = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ProdStartDate = c.DateTime(nullable: false),
                        Discontinued = c.Boolean(nullable: false),
                        DiscontDate = c.DateTime(),
                        DepartmentId = c.Int(nullable: false),
                        ProductGroupId = c.Int(),
                        ProductsPerCase = c.Int(),
                        CasesPerPallet = c.Int(),
                        ProcessByCase = c.Boolean(nullable: false),
                        ProcessByPallet = c.Boolean(nullable: false),
                        RequiresBatchNumberOnReceipt = c.Boolean(),
                        RequiresExpiryDateOnReceipt = c.Boolean(),
                        PreferredSupplier = c.Int(),
                        IsRawMaterial = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductId)
                .ForeignKey("dbo.GlobalTax", t => t.TaxID)
                .ForeignKey("dbo.GlobalUOM", t => t.UOMId)
                .ForeignKey("dbo.GlobalWeightGroups", t => t.WeightGroupId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId)
                .ForeignKey("dbo.ProductLotOptionsCodes", t => t.LotOptionCodeId)
                .ForeignKey("dbo.ProductLotProcessTypeCodes", t => t.LotProcessTypeCodeId)
                .ForeignKey("dbo.TenantDepartments", t => t.DepartmentId)
                .Index(t => t.UOMId)
                .Index(t => t.LotOptionCodeId)
                .Index(t => t.LotProcessTypeCodeId)
                .Index(t => t.WeightGroupId)
                .Index(t => t.TaxID)
                .Index(t => t.DepartmentId)
                .Index(t => t.ProductGroupId);
            
            CreateTable(
                "dbo.GlobalTax",
                c => new
                    {
                        TaxID = c.Int(nullable: false, identity: true),
                        TaxName = c.String(),
                        TaxDescription = c.String(),
                        PercentageOfAmount = c.Int(nullable: false),
                        CountryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TaxID)
                .ForeignKey("dbo.GlobalCountry", t => t.CountryID)
                .Index(t => t.CountryID);
            
            CreateTable(
                "dbo.OrderDetails",
                c => new
                    {
                        OrderDetailID = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        ExpectedDate = c.DateTime(storeType: "date"),
                        Notes = c.String(),
                        ProductId = c.Int(nullable: false),
                        ProdAccCodeID = c.Int(),
                        Qty = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyID = c.Int(),
                        WarrantyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxID = c.Int(),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        TenentId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(),
                        OrderDetailStatusId = c.Int(),
                    })
                .PrimaryKey(t => t.OrderDetailID)
                .ForeignKey("dbo.ProductAccountCodes", t => t.ProdAccCodeID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.OrderStatus", t => t.OrderDetailStatusId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.GlobalTax", t => t.TaxID)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .ForeignKey("dbo.TenantWarranties", t => t.WarrantyID)
                .Index(t => t.OrderID)
                .Index(t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.ProdAccCodeID)
                .Index(t => t.WarrantyID)
                .Index(t => t.TaxID)
                .Index(t => t.OrderDetailStatusId);
            
            CreateTable(
                "dbo.ProductAccountCodes",
                c => new
                    {
                        ProdAccCodeID = c.Int(nullable: false, identity: true),
                        AccountID = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProdAccCode = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProdAccCodeID)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.AccountID)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.OrderStatus",
                c => new
                    {
                        OrderStatusID = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.OrderStatusID);
            
            CreateTable(
                "dbo.PalletProducts",
                c => new
                    {
                        PalletProductID = c.Int(nullable: false, identity: true),
                        OrderDetailID = c.Int(nullable: false),
                        ProductID = c.Int(nullable: false),
                        PalletID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PalletProductID)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.OrderDetails", t => t.OrderDetailID)
                .ForeignKey("dbo.Pallets", t => t.PalletID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductID)
                .Index(t => t.OrderDetailID)
                .Index(t => t.ProductID)
                .Index(t => t.PalletID)
                .Index(t => t.OrderID);
            
            CreateTable(
                "dbo.Pallets",
                c => new
                    {
                        PalletID = c.Int(nullable: false, identity: true),
                        PalletNumber = c.String(),
                        ProofOfLoadingImage = c.String(),
                        RecipientAccountID = c.Int(nullable: false),
                        DateCompleted = c.DateTime(),
                        CompletedBy = c.Int(),
                        PalletsDispatchID = c.Int(),
                        MobileToken = c.Guid(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.PalletID)
                .ForeignKey("dbo.PalletsDispatches", t => t.PalletsDispatchID)
                .ForeignKey("dbo.Account", t => t.RecipientAccountID)
                .Index(t => t.RecipientAccountID)
                .Index(t => t.PalletsDispatchID);
            
            CreateTable(
                "dbo.PalletsDispatches",
                c => new
                    {
                        PalletsDispatchID = c.Int(nullable: false, identity: true),
                        DispatchReference = c.String(),
                        VehicleIdentifier = c.String(),
                        VehicleDescription = c.String(),
                        DateCompleted = c.DateTime(),
                        CompletedBy = c.Int(),
                        TrackingReference = c.String(),
                        CustomVehicleNumber = c.String(),
                        CustomVehicleModel = c.String(),
                        CustomDriverDetails = c.String(),
                        ProofOfDeliveryImageFilenames = c.String(),
                        DispatchNotes = c.String(),
                        MarketVehicleID = c.Int(),
                        SentMethodID = c.Int(),
                        VehicleDriverResourceID = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedBy = c.Int(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.PalletsDispatchID)
                .ForeignKey("dbo.SentMethods", t => t.SentMethodID)
                .ForeignKey("dbo.MarketVehicles", t => t.MarketVehicleID)
                .ForeignKey("dbo.Resources", t => t.VehicleDriverResourceID)
                .Index(t => t.MarketVehicleID)
                .Index(t => t.SentMethodID)
                .Index(t => t.VehicleDriverResourceID);
            
            CreateTable(
                "dbo.SentMethods",
                c => new
                    {
                        SentMethodID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        TrackUrl = c.String(),
                    })
                .PrimaryKey(t => t.SentMethodID);
            
            CreateTable(
                "dbo.MarketVehicles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        VehicleIdentifier = c.String(),
                        MarketId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        MarketRoute_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MarketRoutes", t => t.MarketRoute_Id)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .Index(t => t.MarketId)
                .Index(t => t.MarketRoute_Id);
            
            CreateTable(
                "dbo.Markets",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        Town = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MarketRoutes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        IsDefaultRoute = c.Boolean(nullable: false),
                        LastVehicleId = c.Int(),
                        RouteDurationMins = c.Int(),
                        MarketId = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MarketVehicles", t => t.LastVehicleId)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .Index(t => t.LastVehicleId)
                .Index(t => t.MarketId);
            
            CreateTable(
                "dbo.MarketRouteCustomers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountId = c.Int(nullable: false),
                        RouteOrder = c.Int(nullable: false),
                        IsSkippable = c.Boolean(nullable: false),
                        SkipFromDate = c.DateTime(),
                        SkipToDate = c.DateTime(),
                        MarketRouteId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.MarketRoutes", t => t.MarketRouteId)
                .Index(t => t.AccountId)
                .Index(t => t.MarketRouteId);
            
            CreateTable(
                "dbo.TenantWarranties",
                c => new
                    {
                        WarrantyID = c.Int(nullable: false, identity: true),
                        WarrantyName = c.String(),
                        WarrantyDescription = c.String(),
                        IsPercent = c.Boolean(nullable: false),
                        PercentageOfPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FixedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyDays = c.Int(nullable: false),
                        PostageTypeId = c.Int(nullable: false),
                        HotSwap = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        ProductSerialis_SerialID = c.Int(),
                    })
                .PrimaryKey(t => t.WarrantyID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantPostageTypes", t => t.PostageTypeId)
                .ForeignKey("dbo.ProductSerialis", t => t.ProductSerialis_SerialID)
                .Index(t => t.PostageTypeId)
                .Index(t => t.TenantId)
                .Index(t => t.ProductSerialis_SerialID);
            
            CreateTable(
                "dbo.TenantPostageTypes",
                c => new
                    {
                        PostageTypeId = c.Int(nullable: false, identity: true),
                        PostTypeName = c.String(maxLength: 30),
                        Description = c.String(maxLength: 50),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.PostageTypeId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.GlobalUOM",
                c => new
                    {
                        UOMId = c.Int(nullable: false, identity: true),
                        UOM = c.String(nullable: false, maxLength: 50),
                        UOMTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UOMId)
                .ForeignKey("dbo.GlobalUOMTypes", t => t.UOMTypeId)
                .Index(t => t.UOMTypeId);
            
            CreateTable(
                "dbo.GlobalUOMTypes",
                c => new
                    {
                        UOMTypeId = c.Int(nullable: false, identity: true),
                        UOMType = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.UOMTypeId);
            
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        LocationId = c.Int(nullable: false, identity: true),
                        WarehouseId = c.Int(nullable: false),
                        LocationGroupId = c.Int(nullable: false),
                        LocationTypeId = c.Int(nullable: false),
                        LocationName = c.String(nullable: false),
                        LocationCode = c.String(nullable: false, maxLength: 50),
                        Description = c.String(),
                        LocationWeight = c.Double(),
                        LocationHeight = c.Double(),
                        LocationWidth = c.Double(),
                        LocationDepth = c.Double(),
                        UOMId = c.Int(nullable: false),
                        LevelOfDetail = c.Int(),
                        DimensionUOMId = c.Int(nullable: false),
                        StagingLocation = c.Boolean(nullable: false),
                        MixContainer = c.Boolean(nullable: false),
                        AllowPutAway = c.Boolean(nullable: false),
                        AllowPick = c.Boolean(nullable: false),
                        AllowReplenish = c.Boolean(nullable: false),
                        PutAwaySeq = c.Int(),
                        PickSeq = c.Int(),
                        ReplenishSeq = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocationId)
                .ForeignKey("dbo.GlobalUOM", t => t.UOMId)
                .ForeignKey("dbo.LocationGroups", t => t.LocationGroupId)
                .ForeignKey("dbo.LocationTypes", t => t.LocationTypeId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.WarehouseId)
                .Index(t => t.LocationGroupId)
                .Index(t => t.LocationTypeId)
                .Index(t => t.UOMId);
            
            CreateTable(
                "dbo.LocationGroups",
                c => new
                    {
                        LocationGroupId = c.Int(nullable: false, identity: true),
                        Locdescription = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocationGroupId);
            
            CreateTable(
                "dbo.LocationTypes",
                c => new
                    {
                        LocationTypeId = c.Int(nullable: false, identity: true),
                        LocTypeName = c.String(),
                        LocTypeDescription = c.String(maxLength: 50),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LocationTypeId);
            
            CreateTable(
                "dbo.ProductLocations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        LocationId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.GlobalWeightGroups",
                c => new
                    {
                        WeightGroupId = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 50),
                        Weight = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.WeightGroupId);
            
            CreateTable(
                "dbo.InventoryTransactions",
                c => new
                    {
                        InventoryTransactionId = c.Int(nullable: false, identity: true),
                        InventoryTransactionTypeId = c.Int(nullable: false),
                        OrderID = c.Int(),
                        InventoryTransactionRef = c.String(),
                        ProductId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        TenentId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WastageReasonId = c.Int(),
                        LocationId = c.Int(),
                        SerialID = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        BatchNumber = c.String(),
                        ExpiryDate = c.DateTime(),
                        IsCurrentLocation = c.Boolean(nullable: false),
                        OrderProcessId = c.Int(),
                        Tenant_TenantId = c.Int(),
                    })
                .PrimaryKey(t => t.InventoryTransactionId)
                .ForeignKey("dbo.InventoryTransactionTypes", t => t.InventoryTransactionTypeId)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.OrderProcesses", t => t.OrderProcessId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductSerialis", t => t.SerialID)
                .ForeignKey("dbo.Tenants", t => t.Tenant_TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .ForeignKey("dbo.WastageReason", t => t.WastageReasonId)
                .Index(t => t.InventoryTransactionTypeId)
                .Index(t => t.OrderID)
                .Index(t => t.ProductId)
                .Index(t => t.WarehouseId)
                .Index(t => t.WastageReasonId)
                .Index(t => t.LocationId)
                .Index(t => t.SerialID)
                .Index(t => t.OrderProcessId)
                .Index(t => t.Tenant_TenantId);
            
            CreateTable(
                "dbo.InventoryTransactionTypes",
                c => new
                    {
                        InventoryTransactionTypeId = c.Int(nullable: false, identity: true),
                        OrderType = c.String(nullable: false),
                        InventoryTransactionTypeName = c.String(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.InventoryTransactionTypeId);
            
            CreateTable(
                "dbo.OrderProcesses",
                c => new
                    {
                        OrderProcessID = c.Int(nullable: false, identity: true),
                        DeliveryNO = c.String(),
                        ConsignmentNumber = c.String(),
                        ConsignmentTypeId = c.Int(),
                        OrderID = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        TenentId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        InventoryTransactionTypeId = c.Int(),
                        OrderProcessStatusId = c.Int(),
                    })
                .PrimaryKey(t => t.OrderProcessID)
                .ForeignKey("dbo.OrderConsignmentTypes", t => t.ConsignmentTypeId)
                .ForeignKey("dbo.InventoryTransactionTypes", t => t.InventoryTransactionTypeId)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.OrderProcessStatus", t => t.OrderProcessStatusId)
                .Index(t => t.ConsignmentTypeId)
                .Index(t => t.OrderID)
                .Index(t => t.InventoryTransactionTypeId)
                .Index(t => t.OrderProcessStatusId);
            
            CreateTable(
                "dbo.OrderConsignmentTypes",
                c => new
                    {
                        ConsignmentTypeId = c.Int(nullable: false, identity: true),
                        ConsignmentType = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ConsignmentTypeId);
            
            CreateTable(
                "dbo.OrderProcessDetails",
                c => new
                    {
                        OrderProcessDetailID = c.Int(nullable: false, identity: true),
                        OrderProcessId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        QtyProcessed = c.Decimal(nullable: false, precision: 18, scale: 2),
                        OrderDetailID = c.Int(),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        TenentId = c.Int(nullable: false),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.OrderProcessDetailID)
                .ForeignKey("dbo.OrderDetails", t => t.OrderDetailID)
                .ForeignKey("dbo.OrderProcesses", t => t.OrderProcessId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.OrderProcessId)
                .Index(t => t.ProductId)
                .Index(t => t.OrderDetailID);
            
            CreateTable(
                "dbo.OrderProcessStatus",
                c => new
                    {
                        OrderProcessStatusId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.OrderProcessStatusId);
            
            CreateTable(
                "dbo.ProductSerialis",
                c => new
                    {
                        SerialID = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        SerialNo = c.String(nullable: false),
                        ElectronicTag = c.String(),
                        CurrentStatus = c.Int(nullable: false),
                        Batch = c.String(),
                        Notes = c.String(),
                        BuyPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyID = c.Int(nullable: false),
                        SoldWarrantyDays = c.Int(nullable: false),
                        SoldWarrantyIsPercent = c.Boolean(nullable: false),
                        SoldWarrantyPercentage = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SoldWarrantyFixedPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        SoldWarrantyStartDate = c.DateTime(),
                        SoldWarrentyEndDate = c.DateTime(),
                        SoldWarrantyName = c.String(),
                        PostageTypeId = c.Int(),
                        ExpiryDate = c.DateTime(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(nullable: false),
                        TenentId = c.Int(nullable: false),
                        WarehouseId = c.Int(),
                        LocationId = c.Int(),
                    })
                .PrimaryKey(t => t.SerialID)
                .ForeignKey("dbo.Locations", t => t.LocationId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.TenantPostageTypes", t => t.PostageTypeId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.ProductId)
                .Index(t => t.PostageTypeId)
                .Index(t => t.WarehouseId)
                .Index(t => t.LocationId);
            
            CreateTable(
                "dbo.WastageReason",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductAttributeValuesMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        AttributeValueId = c.Int(nullable: false),
                        DateCreated = c.DateTime(),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductAttributeValues", t => t.AttributeValueId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.AttributeValueId);
            
            CreateTable(
                "dbo.ProductAttributeValues",
                c => new
                    {
                        AttributeValueId = c.Int(nullable: false, identity: true),
                        AttributeId = c.Int(nullable: false),
                        Value = c.String(nullable: false, maxLength: 20),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.AttributeValueId)
                .ForeignKey("dbo.ProductAttributes", t => t.AttributeId)
                .Index(t => t.AttributeId);
            
            CreateTable(
                "dbo.ProductAttributes",
                c => new
                    {
                        AttributeId = c.Int(nullable: false, identity: true),
                        AttributeName = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.AttributeId);
            
            CreateTable(
                "dbo.ProductGroups",
                c => new
                    {
                        ProductGroupId = c.Int(nullable: false, identity: true),
                        ProductGroup = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsActive = c.Boolean(nullable: false),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductGroupId);
            
            CreateTable(
                "dbo.ProductKitMaps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        KitProductId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ProductMaster_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductMaster", t => t.KitProductId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMaster_ProductId)
                .Index(t => t.ProductId)
                .Index(t => t.KitProductId)
                .Index(t => t.ProductMaster_ProductId);
            
            CreateTable(
                "dbo.ProductLotOptionsCodes",
                c => new
                    {
                        LotOptionCodeId = c.Int(nullable: false, identity: true),
                        Description = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.LotOptionCodeId);
            
            CreateTable(
                "dbo.ProductLotProcessTypeCodes",
                c => new
                    {
                        LotProcessTypeCodeId = c.Int(nullable: false, identity: true),
                        Description = c.String(storeType: "ntext"),
                    })
                .PrimaryKey(t => t.LotProcessTypeCodeId);
            
            CreateTable(
                "dbo.ProductPriceLevels",
                c => new
                    {
                        ProductPriceLevelID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        DiscountPercent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExpiryDate = c.DateTime(),
                        ProductMasterID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductPriceLevelID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMasterID)
                .Index(t => t.ProductMasterID);
            
            CreateTable(
                "dbo.ProductSCCCodes",
                c => new
                    {
                        ProductSCCCodeId = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        SCC = c.String(maxLength: 50),
                        Quantity = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProductSCCCodeId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductReceipeMasters",
                c => new
                    {
                        ProductReceipeMasterID = c.Int(nullable: false, identity: true),
                        ProductMasterID = c.Int(nullable: false),
                        RecipeItemProductID = c.Int(nullable: false),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        ProductMaster_ProductId = c.Int(),
                    })
                .PrimaryKey(t => t.ProductReceipeMasterID)
                .ForeignKey("dbo.ProductMaster", t => t.RecipeItemProductID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMasterID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMaster_ProductId)
                .Index(t => t.ProductMasterID)
                .Index(t => t.RecipeItemProductID)
                .Index(t => t.ProductMaster_ProductId);
            
            CreateTable(
                "dbo.StockTakeSnapshots",
                c => new
                    {
                        StockTakeSnapshotId = c.Int(nullable: false, identity: true),
                        StockTakeId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ReceivedSku = c.String(maxLength: 50),
                        ActionTaken = c.Boolean(nullable: false),
                        PreviousQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NewQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockTakeSnapshotId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.StockTakes", t => t.StockTakeId)
                .Index(t => t.StockTakeId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StockTakes",
                c => new
                    {
                        StockTakeId = c.Int(nullable: false, identity: true),
                        StockTakeReference = c.String(),
                        StockTakeDescription = c.String(),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.StockTakeId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.WarehouseId)
                .Index(t => t.WarehouseId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.StockTakeDetails",
                c => new
                    {
                        StockTakeDetailId = c.Int(nullable: false, identity: true),
                        StockTakeId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ReceivedSku = c.String(maxLength: 50),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateScanned = c.DateTime(nullable: false),
                        CreatedBy = c.Int(),
                        IsApplied = c.Boolean(),
                        DateApplied = c.DateTime(),
                        IsDeleted = c.Boolean(),
                        TenentId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.StockTakeDetailId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.StockTakes", t => t.StockTakeId)
                .Index(t => t.StockTakeId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.StockTakeSerialSnapshots",
                c => new
                    {
                        StockTakeSerialSnapshotId = c.Int(nullable: false, identity: true),
                        StockTakeSnapshotId = c.Int(nullable: false),
                        StockTakeId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        ProductSerialId = c.Int(nullable: false),
                        CurrentStatus = c.Int(nullable: false),
                        IsInStock = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.StockTakeSerialSnapshotId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductSerialis", t => t.ProductSerialId)
                .ForeignKey("dbo.StockTakes", t => t.StockTakeId)
                .ForeignKey("dbo.StockTakeSnapshots", t => t.StockTakeSnapshotId)
                .Index(t => t.StockTakeSnapshotId)
                .Index(t => t.StockTakeId)
                .Index(t => t.ProductId)
                .Index(t => t.ProductSerialId);
            
            CreateTable(
                "dbo.TenantDepartments",
                c => new
                    {
                        DepartmentId = c.Int(nullable: false, identity: true),
                        DepartmentName = c.String(nullable: false, maxLength: 250),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.DepartmentId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ProductLocationStockLevel",
                c => new
                    {
                        ProductLocationStockLevelID = c.Int(nullable: false, identity: true),
                        ProductMasterID = c.Int(nullable: false),
                        TenantLocationID = c.Int(nullable: false),
                        MinStockQuantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ProductLocationStockLevelID)
                .ForeignKey("dbo.ProductMaster", t => t.ProductMasterID)
                .ForeignKey("dbo.TenantLocations", t => t.TenantLocationID)
                .Index(t => t.ProductMasterID)
                .Index(t => t.TenantLocationID);
            
            CreateTable(
                "dbo.AuthUserLogins",
                c => new
                    {
                        UserLoginId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        DateLoggedIn = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserLoginId)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AuthUserLoginActivities",
                c => new
                    {
                        LoginActivityId = c.Int(nullable: false, identity: true),
                        UserLoginId = c.Int(nullable: false),
                        WarehouseId = c.Int(nullable: false),
                        ActivityId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LoginActivityId)
                .ForeignKey("dbo.AuthUserLogins", t => t.UserLoginId)
                .Index(t => t.UserLoginId);
            
            CreateTable(
                "dbo.AuthUserprofiles",
                c => new
                    {
                        UserProfileId = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        UserProfileKey = c.String(maxLength: 200),
                        UserProfileKeyValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.UserProfileId)
                .ForeignKey("dbo.AuthUsers", t => t.UserId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.TenantEmailConfigs",
                c => new
                    {
                        TenantEmailConfigId = c.Int(nullable: false, identity: true),
                        SmtpHost = c.String(nullable: false),
                        SmtpPort = c.Int(nullable: false),
                        UserEmail = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        EnableSsl = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        DailyEmailDispatchTime = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TenantEmailConfigId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.EmployeeGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        GroupsId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupsId)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.ResourceId)
                .Index(t => t.GroupsId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupName = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.EmployeeRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        RolesId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.Roles", t => t.RolesId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.ResourceId)
                .Index(t => t.RolesId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.JobTypes",
                c => new
                    {
                        JobTypeId = c.Int(nullable: false, identity: true),
                        Job_Type = c.String(nullable: false),
                        Description = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdate = c.DateTime(),
                        CreatedBy = c.Int(nullable: false),
                        UpdatedBy = c.Int(),
                    })
                .PrimaryKey(t => t.JobTypeId);
            
            CreateTable(
                "dbo.JobSubTypes",
                c => new
                    {
                        JobSubTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Description = c.String(),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.JobSubTypeId);
            
            CreateTable(
                "dbo.OrderNotes",
                c => new
                    {
                        OrderNoteId = c.Int(nullable: false, identity: true),
                        OrderID = c.Int(nullable: false),
                        Notes = c.String(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.OrderNoteId)
                .ForeignKey("dbo.Orders", t => t.OrderID)
                .ForeignKey("dbo.AuthUsers", t => t.CreatedBy)
                .Index(t => t.OrderID)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.PProperties",
                c => new
                    {
                        PPropertyId = c.Int(nullable: false, identity: true),
                        PropertyCode = c.String(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        AddressLine5 = c.String(),
                        AddressPostcode = c.String(nullable: false),
                        PropertyStatus = c.String(),
                        IsLandlordManaged = c.Boolean(nullable: false),
                        IsVacant = c.Boolean(nullable: false),
                        DateAvailable = c.DateTime(),
                        DateAdded = c.DateTime(),
                        PropertyBranch = c.String(),
                        TenancyMonths = c.Double(),
                        SiteId = c.Int(nullable: false),
                        SyncRequiredFlag = c.Boolean(nullable: false),
                        LetDate = c.DateTime(),
                        CurrentLandlordCode = c.String(),
                        CurrentTenantCode = c.String(),
                        CurrentLandlordId = c.Int(),
                        CurrentPTenentId = c.Int(),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(),
                        DateUpdated = c.DateTime(nullable: false),
                        UpdatedUserId = c.Int(),
                        Order_OrderID = c.Int(),
                    })
                .PrimaryKey(t => t.PPropertyId)
                .ForeignKey("dbo.PLandlords", t => t.CurrentLandlordId)
                .ForeignKey("dbo.Orders", t => t.Order_OrderID)
                .Index(t => t.CurrentLandlordId)
                .Index(t => t.Order_OrderID);
            
            CreateTable(
                "dbo.PLandlords",
                c => new
                    {
                        PLandlordId = c.Int(nullable: false, identity: true),
                        LandlordCode = c.String(nullable: false),
                        LandlordFullname = c.String(nullable: false),
                        LandlordSalutation = c.String(),
                        LandlordStatus = c.String(),
                        LandlordAdded = c.DateTime(),
                        LandlordNotes1 = c.String(),
                        LandlordNotes2 = c.String(),
                        UserNotes1 = c.String(),
                        UserNotes2 = c.String(),
                        SiteId = c.Int(nullable: false),
                        SyncRequiredFlag = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(),
                        DateUpdated = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        AddressPostcode = c.String(),
                        HomeTelephone = c.String(),
                        WorkTelephone1 = c.String(),
                        WorkTelephone2 = c.String(),
                        WorkTelephoneFax = c.String(),
                        MobileNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.PLandlordId);
            
            CreateTable(
                "dbo.ReportTypes",
                c => new
                    {
                        ReportTypeId = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        AllowChargeTo = c.Boolean(),
                        AllowReportType = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.ReportTypeId);
            
            CreateTable(
                "dbo.SLAPriorits",
                c => new
                    {
                        SLAPriorityId = c.Int(nullable: false, identity: true),
                        Priority = c.String(nullable: false),
                        Description = c.String(),
                        Colour = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.SLAPriorityId);
            
            CreateTable(
                "dbo.TenantPriceGroups",
                c => new
                    {
                        PriceGroupID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Percent = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.PriceGroupID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.AttLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DeviceSerialNo = c.String(),
                        UserPIN = c.String(),
                        Time = c.DateTime(),
                        Status = c.Int(nullable: false),
                        Verify = c.Int(nullable: false),
                        EventCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AttLogs_EmployeeShifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AttLogsId = c.Int(nullable: false),
                        EmployeeShiftsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AttLogs", t => t.AttLogsId)
                .ForeignKey("dbo.ResourceShifts", t => t.EmployeeShiftsId)
                .Index(t => t.AttLogsId)
                .Index(t => t.EmployeeShiftsId);
            
            CreateTable(
                "dbo.AttLogsStamps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SStamp = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.InvoiceDetail",
                c => new
                    {
                        InvoiceDetailId = c.Int(nullable: false, identity: true),
                        InvoiceMasterId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Description = c.String(),
                        Quantity = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Tax = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.InvoiceDetailId)
                .ForeignKey("dbo.InvoiceMaster", t => t.InvoiceMasterId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .Index(t => t.InvoiceMasterId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.InvoiceMaster",
                c => new
                    {
                        InvoiceMasterId = c.Int(nullable: false, identity: true),
                        NetAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TaxAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CardCharges = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PostageCharges = c.Decimal(nullable: false, precision: 18, scale: 2),
                        WarrantyAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceNumber = c.String(),
                        InvoiceAddress = c.String(),
                        InvoiceCurrency = c.String(),
                        InvoiceDate = c.DateTime(nullable: false),
                        OrderProcessId = c.Int(),
                        AccountId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.InvoiceMasterId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.OrderProcesses", t => t.OrderProcessId)
                .Index(t => t.OrderProcessId)
                .Index(t => t.AccountId);
            
            CreateTable(
                "dbo.MarketJobs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        AccountID = c.Int(),
                        MarketRouteAssociationId = c.Int(),
                        MarketJobStatusId = c.Int(nullable: false),
                        DateCancelled = c.DateTime(),
                        CancelledReason = c.String(),
                        DateDeclined = c.DateTime(),
                        DeclinedReason = c.String(),
                        DeviceIdentifier = c.String(),
                        DeviceUsername = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Account", t => t.AccountID)
                .ForeignKey("dbo.MarketJobStatus", t => t.MarketJobStatusId)
                .ForeignKey("dbo.MarketRouteAssociations", t => t.MarketRouteAssociationId)
                .Index(t => t.AccountID)
                .Index(t => t.MarketRouteAssociationId)
                .Index(t => t.MarketJobStatusId);
            
            CreateTable(
                "dbo.MarketJobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MarketRouteAssociations",
                c => new
                    {
                        MarketRouteAssociationId = c.Int(nullable: false, identity: true),
                        MarketId = c.Int(nullable: false),
                        MarketRouteId = c.Int(nullable: false),
                        TenantLocationID = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.MarketRouteAssociationId)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.MarketRoutes", t => t.MarketRouteId)
                .ForeignKey("dbo.TenantLocations", t => t.TenantLocationID)
                .Index(t => t.MarketId)
                .Index(t => t.MarketRouteId)
                .Index(t => t.TenantLocationID);
            
            CreateTable(
                "dbo.MarketRouteSchedules",
                c => new
                    {
                        MarketRouteScheduleId = c.Int(nullable: false, identity: true),
                        StartTime = c.DateTime(nullable: false),
                        EndTime = c.DateTime(nullable: false),
                        Subject = c.String(),
                        Status = c.Int(nullable: false),
                        Description = c.String(),
                        Label = c.Int(nullable: false),
                        Location = c.String(),
                        AllDay = c.Boolean(nullable: false),
                        EventType = c.Int(nullable: false),
                        RecurrenceInfo = c.String(),
                        ReminderInfo = c.String(),
                        VehicleId = c.Int(),
                        VehicleIDs = c.String(),
                        TenentId = c.Int(),
                        IsCanceled = c.Boolean(nullable: false),
                        CancelReason = c.String(),
                        MarketId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MarketRouteScheduleId)
                .ForeignKey("dbo.Markets", t => t.MarketId)
                .ForeignKey("dbo.MarketVehicles", t => t.VehicleId)
                .Index(t => t.VehicleId)
                .Index(t => t.MarketId);
            
            CreateTable(
                "dbo.OperLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OperationType = c.String(),
                        AdminID = c.String(),
                        OperationTime = c.DateTime(nullable: false),
                        OperationObject1 = c.Int(nullable: false),
                        OperationObject2 = c.Int(nullable: false),
                        OperationObject3 = c.Int(nullable: false),
                        OperationObject4 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OperLogsStamps",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SStamp = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderPTenantEmailRecipients",
                c => new
                    {
                        OrderPTenantEmailRecipientId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        PPropertyId = c.Int(),
                        PTenantId = c.Int(),
                        LastEmailNotificationId = c.Int(),
                        EmailAddress = c.String(),
                        IsDeleted = c.Boolean(),
                        UpdatedBy = c.Int(),
                        DateUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.OrderPTenantEmailRecipientId)
                .ForeignKey("dbo.TenantEmailNotificationQueues", t => t.LastEmailNotificationId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.PProperties", t => t.PPropertyId)
                .ForeignKey("dbo.PTenants", t => t.PTenantId)
                .Index(t => t.OrderId)
                .Index(t => t.PPropertyId)
                .Index(t => t.PTenantId)
                .Index(t => t.LastEmailNotificationId);
            
            CreateTable(
                "dbo.TenantEmailNotificationQueues",
                c => new
                    {
                        TenantEmailNotificationQueueId = c.Int(nullable: false, identity: true),
                        OrderId = c.Int(nullable: false),
                        EmailSubject = c.String(),
                        AttachmentVirtualPath = c.String(),
                        TenantEmailTemplatesId = c.Int(nullable: false),
                        AppointmentId = c.Int(),
                        IsNotificationCancelled = c.Boolean(),
                        TenantEmailConfigId = c.Int(nullable: false),
                        ScheduledProcessing = c.Boolean(nullable: false),
                        ScheduledProcessingTime = c.DateTime(nullable: false),
                        ActualProcessingTime = c.DateTime(),
                        WorkOrderStartTime = c.DateTime(),
                        WorkOrderEndTime = c.DateTime(),
                        WorksOrderResourceName = c.String(),
                        ProcessedImmediately = c.Boolean(nullable: false),
                        CustomRecipients = c.String(),
                        CustomCcRecipients = c.String(),
                        CustomBccRecipients = c.String(),
                    })
                .PrimaryKey(t => t.TenantEmailNotificationQueueId)
                .ForeignKey("dbo.Appointments", t => t.AppointmentId)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .ForeignKey("dbo.TenantEmailConfigs", t => t.TenantEmailConfigId)
                .ForeignKey("dbo.TenantEmailTemplates", t => t.TenantEmailTemplatesId)
                .Index(t => t.OrderId)
                .Index(t => t.TenantEmailTemplatesId)
                .Index(t => t.AppointmentId)
                .Index(t => t.TenantEmailConfigId);
            
            CreateTable(
                "dbo.TenantEmailTemplates",
                c => new
                    {
                        TemplateId = c.Int(nullable: false, identity: true),
                        EventName = c.String(nullable: false),
                        DateUpdated = c.DateTime(),
                        UpdatedBy = c.Int(),
                        TenantId = c.Int(nullable: false),
                        TenantEmailConfigId = c.Int(nullable: false),
                        InventoryTransactionTypeId = c.Int(),
                        HtmlFooter = c.String(),
                        HtmlHeader = c.String(nullable: false),
                        Body = c.String(),
                    })
                .PrimaryKey(t => t.TemplateId)
                .ForeignKey("dbo.InventoryTransactionTypes", t => t.InventoryTransactionTypeId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId)
                .Index(t => t.InventoryTransactionTypeId);
            
            CreateTable(
                "dbo.PTenants",
                c => new
                    {
                        PTenantId = c.Int(nullable: false, identity: true),
                        TenantCode = c.String(nullable: false),
                        TenantYCode = c.String(),
                        TenantFullName = c.String(nullable: false),
                        TenantSalutation = c.String(),
                        TenancyStatus = c.String(),
                        TenancyCategory = c.Int(),
                        TenancyAdded = c.DateTime(),
                        TenancyStarted = c.DateTime(),
                        TenancyRenewDate = c.DateTime(),
                        TenancyVacateDate = c.DateTime(),
                        TenancyPeriodMonths = c.Double(),
                        SiteId = c.Int(nullable: false),
                        SyncRequiredFlag = c.Boolean(nullable: false),
                        CurrentPropertyCode = c.String(),
                        CurrentPropertyId = c.Int(),
                        IsCurrentTenant = c.Boolean(nullable: false),
                        IsFutureTenant = c.Boolean(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        CreatedUserId = c.Int(),
                        DateUpdated = c.DateTime(),
                        UpdatedUserId = c.Int(),
                        IsHeadTenant = c.Boolean(nullable: false),
                        AddressLine1 = c.String(),
                        AddressLine2 = c.String(),
                        AddressLine3 = c.String(),
                        AddressLine4 = c.String(),
                        AddressPostcode = c.String(),
                        HomeTelephone = c.String(),
                        WorkTelephone1 = c.String(),
                        WorkTelephone2 = c.String(),
                        WorkTelephoneFax = c.String(),
                        MobileNumber = c.String(),
                        Email = c.String(),
                    })
                .PrimaryKey(t => t.PTenantId)
                .ForeignKey("dbo.PProperties", t => t.CurrentPropertyId)
                .Index(t => t.CurrentPropertyId);
            
            CreateTable(
                "dbo.PSyncHistories",
                c => new
                    {
                        PSyncHistoryId = c.Int(nullable: false, identity: true),
                        SyncStartTime = c.DateTime(nullable: false),
                        ImportCompletedTime = c.DateTime(nullable: false),
                        SyncCompletedTime = c.DateTime(nullable: false),
                        TenantsSynced = c.Int(nullable: false),
                        LandlordsSynced = c.Int(nullable: false),
                        PropertiesSynced = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PSyncHistoryId);
            
            CreateTable(
                "dbo.ResourceRequests",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        HolidayReason = c.String(),
                        RequestedDate = c.DateTime(nullable: false),
                        Label = c.Int(nullable: false),
                        Location = c.String(),
                        AllDay = c.Boolean(nullable: false),
                        EventType = c.Int(nullable: false),
                        RecurrenceInfo = c.String(),
                        ReminderInfo = c.String(),
                        ResourceIDs = c.String(),
                        Status = c.Int(nullable: false),
                        IsAccepted = c.Boolean(nullable: false),
                        IsAnnualHoliday = c.Boolean(nullable: false),
                        AcceptedBy = c.Int(),
                        IsCanceled = c.Boolean(nullable: false),
                        CancelReason = c.String(),
                        RequestType = c.Int(nullable: false),
                        Notes = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.ResourceId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.ResourceJobAllocation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ResourceId = c.Int(nullable: false),
                        ScheduledDate = c.DateTime(),
                        DeclinedComment = c.String(),
                        DeclinedDate = c.DateTime(),
                        CompletedComment = c.String(),
                        CompletedDate = c.DateTime(),
                        ResourceJobStatusId = c.Int(nullable: false),
                        AcceptedGeoLocation = c.Geography(),
                        CancelledGeoLocation = c.Geography(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.ResourceId)
                .ForeignKey("dbo.ResourceJobStatus", t => t.ResourceJobStatusId)
                .Index(t => t.ResourceId)
                .Index(t => t.ResourceJobStatusId);
            
            CreateTable(
                "dbo.ResourceJobStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ResourceJob",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(),
                        Description = c.String(),
                        RequiredDate = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Shifts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        EmployeeId = c.Int(nullable: false),
                        WeekNumber = c.Int(),
                        WeekDay = c.Int(),
                        ExpectedHours = c.Time(precision: 7),
                        TimeBreaks = c.Time(precision: 7),
                        LocationsId = c.Int(),
                        Date = c.DateTime(),
                        StartTime = c.DateTime(),
                        EndTime = c.DateTime(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                        Resources_ResourceId = c.Int(),
                        TenantLocations_WarehouseId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Resources", t => t.Resources_ResourceId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .ForeignKey("dbo.TenantLocations", t => t.TenantLocations_WarehouseId)
                .Index(t => t.TenantId)
                .Index(t => t.Resources_ResourceId)
                .Index(t => t.TenantLocations_WarehouseId);
            
            CreateTable(
                "dbo.StockTakeDetailsSerials",
                c => new
                    {
                        StockTakeDetailsSerialId = c.Int(nullable: false, identity: true),
                        ProductSerialId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        SerialNumber = c.String(),
                        StockTakeDetailId = c.Int(nullable: false),
                        DateScanned = c.DateTime(nullable: false),
                        CreatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.StockTakeDetailsSerialId)
                .ForeignKey("dbo.ProductMaster", t => t.ProductId)
                .ForeignKey("dbo.ProductSerialis", t => t.ProductSerialId)
                .ForeignKey("dbo.StockTakeDetails", t => t.StockTakeDetailId)
                .Index(t => t.ProductSerialId)
                .Index(t => t.ProductId)
                .Index(t => t.StockTakeDetailId);
            
            CreateTable(
                "dbo.StockTakeScanLogs",
                c => new
                    {
                        StockTakeScanLogId = c.Int(nullable: false, identity: true),
                        RequestCurrentTenantId = c.Int(nullable: false),
                        RequestWarehouseId = c.Int(nullable: false),
                        RequestAuthUserId = c.Int(nullable: false),
                        RequestStockTakeId = c.Int(nullable: false),
                        RequestProductCode = c.String(),
                        RequestProductSerial = c.String(),
                        ResponseProductName = c.String(),
                        ResponseProductDescription = c.String(),
                        ResponseInStock = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResponseAllocated = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResponseAvailable = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ResponseSerialRequired = c.Boolean(nullable: false),
                        ResponseSuccess = c.Boolean(nullable: false),
                        ResponseFailureMessage = c.String(),
                        ResponseHasWarning = c.Boolean(nullable: false),
                        ResponseWarningMessage = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.StockTakeScanLogId);
            
            CreateTable(
                "dbo.TenantConfigs",
                c => new
                    {
                        TenantConfigId = c.Int(nullable: false, identity: true),
                        AlertMinimumProductPrice = c.Boolean(nullable: false),
                        EnforceMinimumProductPrice = c.Boolean(nullable: false),
                        AlertMinimumPriceMessage = c.String(),
                        EnforceMinimumPriceMessage = c.String(),
                        PurchaseOrderReportFooter = c.String(),
                        WorksOrderScheduleByMarginHours = c.Int(),
                        WorksOrderScheduleByAmPm = c.Boolean(),
                        EnableLiveEmails = c.Boolean(),
                        MiniProfilerEnabled = c.Boolean(),
                        EnabledRelayEmailServer = c.Boolean(),
                        EnablePalletingOnPick = c.Boolean(),
                        WarehouseLogEmailsToDefault = c.String(),
                        WarehouseScheduleStartTime = c.String(),
                        WarehouseScheduleEndTime = c.String(),
                        ErrorLogsForwardEmails = c.String(),
                        DefaultReplyToAddress = c.String(),
                        DefaultMailFromText = c.String(),
                        AutoTransferStockEnabled = c.Boolean(),
                        EnableStockVarianceAlerts = c.Boolean(nullable: false),
                        AuthorisationAdminEmail = c.String(),
                        DefaultCashAccountID = c.Int(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TenantConfigId)
                .ForeignKey("dbo.Account", t => t.DefaultCashAccountID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.DefaultCashAccountID)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantContacts",
                c => new
                    {
                        TenantContactId = c.Int(nullable: false, identity: true),
                        TenantContactName = c.String(maxLength: 200),
                        TenantContactEmail = c.String(maxLength: 200),
                        TenantContactPhone = c.String(maxLength: 50),
                        TenantContactPin = c.Short(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TenantContactId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantCurrencies",
                c => new
                    {
                        TenantCurrencyID = c.Int(nullable: false, identity: true),
                        CurrencyID = c.Int(nullable: false),
                        DiffFactor = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TenantCurrencyID)
                .ForeignKey("dbo.GlobalCurrency", t => t.CurrencyID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.CurrencyID)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantCurrenciesExRates",
                c => new
                    {
                        ExchnageRateID = c.Int(nullable: false, identity: true),
                        TenantCurrencyID = c.Int(nullable: false),
                        DiffFactor = c.Decimal(precision: 18, scale: 2),
                        ActualRate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateUpdated = c.DateTime(),
                        Tenant_TenantId = c.Int(),
                    })
                .PrimaryKey(t => t.ExchnageRateID)
                .ForeignKey("dbo.Tenants", t => t.Tenant_TenantId)
                .ForeignKey("dbo.TenantCurrencies", t => t.TenantCurrencyID)
                .Index(t => t.TenantCurrencyID)
                .Index(t => t.Tenant_TenantId);
            
            CreateTable(
                "dbo.TenantEmailTemplateVariables",
                c => new
                    {
                        TenantEmailVariableId = c.Int(nullable: false, identity: true),
                        VariableName = c.String(nullable: false),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TenantEmailVariableId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantLoanTypes",
                c => new
                    {
                        LoanID = c.Int(nullable: false, identity: true),
                        LoanName = c.String(),
                        LoanDescription = c.String(),
                        LoanDays = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.LoanID)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantModules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ModuleId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Modules", t => t.ModuleId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.ModuleId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.TenantProfiles",
                c => new
                    {
                        TenantProfileId = c.Int(nullable: false, identity: true),
                        TenantProfileKey = c.String(maxLength: 200),
                        TenantProfileKeyValue = c.String(),
                        IsActive = c.Boolean(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.TenantProfileId)
                .ForeignKey("dbo.Tenants", t => t.TenantId)
                .Index(t => t.TenantId);
            
            CreateTable(
                "dbo.VehicleInspectionCheckList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                        VehicleInspectionTypeId = c.Int(nullable: false),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleInspectionType", t => t.VehicleInspectionTypeId)
                .Index(t => t.VehicleInspectionTypeId);
            
            CreateTable(
                "dbo.VehicleInspectionType",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VehicleInspectionConfirmedList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VehicleInspectionId = c.Int(nullable: false),
                        VehicleInspectionCheckListId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.VehicleInspection", t => t.VehicleInspectionId)
                .ForeignKey("dbo.VehicleInspectionCheckList", t => t.VehicleInspectionCheckListId)
                .Index(t => t.VehicleInspectionId)
                .Index(t => t.VehicleInspectionCheckListId);
            
            CreateTable(
                "dbo.VehicleInspection",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InspectionDateTime = c.DateTime(nullable: false),
                        VehicleDriverId = c.Int(nullable: false),
                        DriverSignature = c.Binary(),
                        MarketVehicleId = c.Int(),
                        FleetNumber = c.String(),
                        MileageReading = c.Int(nullable: false),
                        Notes = c.String(),
                        ReportedToUserId = c.Int(),
                        ReportedToName = c.String(),
                        RectifiedUserId = c.Int(),
                        RectifiedUserName = c.String(),
                        RectifiedSignature = c.Binary(),
                        RectifiedDateTime = c.DateTime(),
                        NilDefect = c.Boolean(),
                        TenantId = c.Int(nullable: false),
                        DateCreated = c.DateTime(nullable: false),
                        DateUpdated = c.DateTime(),
                        CreatedBy = c.Int(),
                        UpdatedBy = c.Int(),
                        IsDeleted = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.MarketVehicles", t => t.MarketVehicleId)
                .ForeignKey("dbo.Resources", t => t.RectifiedUserId)
                .ForeignKey("dbo.Resources", t => t.ReportedToUserId)
                .ForeignKey("dbo.Resources", t => t.VehicleDriverId)
                .Index(t => t.VehicleDriverId)
                .Index(t => t.MarketVehicleId)
                .Index(t => t.ReportedToUserId)
                .Index(t => t.RectifiedUserId);
            
            CreateTable(
                "dbo.JobTypeResources",
                c => new
                    {
                        JobType_JobTypeId = c.Int(nullable: false),
                        Resources_ResourceId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.JobType_JobTypeId, t.Resources_ResourceId })
                .ForeignKey("dbo.JobTypes", t => t.JobType_JobTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Resources", t => t.Resources_ResourceId, cascadeDelete: true)
                .Index(t => t.JobType_JobTypeId)
                .Index(t => t.Resources_ResourceId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.VehicleInspectionConfirmedList", "VehicleInspectionCheckListId", "dbo.VehicleInspectionCheckList");
            DropForeignKey("dbo.VehicleInspection", "VehicleDriverId", "dbo.Resources");
            DropForeignKey("dbo.VehicleInspection", "ReportedToUserId", "dbo.Resources");
            DropForeignKey("dbo.VehicleInspection", "RectifiedUserId", "dbo.Resources");
            DropForeignKey("dbo.VehicleInspection", "MarketVehicleId", "dbo.MarketVehicles");
            DropForeignKey("dbo.VehicleInspectionConfirmedList", "VehicleInspectionId", "dbo.VehicleInspection");
            DropForeignKey("dbo.VehicleInspectionCheckList", "VehicleInspectionTypeId", "dbo.VehicleInspectionType");
            DropForeignKey("dbo.TenantProfiles", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantModules", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantModules", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.TenantLoanTypes", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantEmailTemplateVariables", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantCurrenciesExRates", "TenantCurrencyID", "dbo.TenantCurrencies");
            DropForeignKey("dbo.TenantCurrenciesExRates", "Tenant_TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantCurrencies", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantCurrencies", "CurrencyID", "dbo.GlobalCurrency");
            DropForeignKey("dbo.TenantContacts", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantConfigs", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantConfigs", "DefaultCashAccountID", "dbo.Account");
            DropForeignKey("dbo.StockTakeDetailsSerials", "StockTakeDetailId", "dbo.StockTakeDetails");
            DropForeignKey("dbo.StockTakeDetailsSerials", "ProductSerialId", "dbo.ProductSerialis");
            DropForeignKey("dbo.StockTakeDetailsSerials", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.Shifts", "TenantLocations_WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.Shifts", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Shifts", "Resources_ResourceId", "dbo.Resources");
            DropForeignKey("dbo.ResourceJobAllocation", "ResourceJobStatusId", "dbo.ResourceJobStatus");
            DropForeignKey("dbo.ResourceJobAllocation", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.ResourceRequests", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ResourceRequests", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.OrderPTenantEmailRecipients", "PTenantId", "dbo.PTenants");
            DropForeignKey("dbo.PTenants", "CurrentPropertyId", "dbo.PProperties");
            DropForeignKey("dbo.OrderPTenantEmailRecipients", "PPropertyId", "dbo.PProperties");
            DropForeignKey("dbo.OrderPTenantEmailRecipients", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.OrderPTenantEmailRecipients", "LastEmailNotificationId", "dbo.TenantEmailNotificationQueues");
            DropForeignKey("dbo.TenantEmailNotificationQueues", "TenantEmailTemplatesId", "dbo.TenantEmailTemplates");
            DropForeignKey("dbo.TenantEmailTemplates", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantEmailTemplates", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.TenantEmailNotificationQueues", "TenantEmailConfigId", "dbo.TenantEmailConfigs");
            DropForeignKey("dbo.TenantEmailNotificationQueues", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.TenantEmailNotificationQueues", "AppointmentId", "dbo.Appointments");
            DropForeignKey("dbo.MarketRouteSchedules", "VehicleId", "dbo.MarketVehicles");
            DropForeignKey("dbo.MarketRouteSchedules", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketJobs", "MarketRouteAssociationId", "dbo.MarketRouteAssociations");
            DropForeignKey("dbo.MarketRouteAssociations", "TenantLocationID", "dbo.TenantLocations");
            DropForeignKey("dbo.MarketRouteAssociations", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteAssociations", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketJobs", "MarketJobStatusId", "dbo.MarketJobStatus");
            DropForeignKey("dbo.MarketJobs", "AccountID", "dbo.Account");
            DropForeignKey("dbo.InvoiceDetail", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.InvoiceMaster", "OrderProcessId", "dbo.OrderProcesses");
            DropForeignKey("dbo.InvoiceDetail", "InvoiceMasterId", "dbo.InvoiceMaster");
            DropForeignKey("dbo.InvoiceMaster", "AccountId", "dbo.Account");
            DropForeignKey("dbo.AttLogs_EmployeeShifts", "EmployeeShiftsId", "dbo.ResourceShifts");
            DropForeignKey("dbo.AttLogs_EmployeeShifts", "AttLogsId", "dbo.AttLogs");
            DropForeignKey("dbo.Account", "PriceGroupID", "dbo.TenantPriceGroups");
            DropForeignKey("dbo.TenantPriceGroups", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Orders", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.Orders", "TransferTo_WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.Orders", "TransferFrom_WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.Orders", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.Orders", "TenentId", "dbo.Tenants");
            DropForeignKey("dbo.Orders", "SLAPriorityId", "dbo.SLAPriorits");
            DropForeignKey("dbo.Orders", "ShipmentWarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.Orders", "ShipmentPropertyId", "dbo.PProperties");
            DropForeignKey("dbo.Orders", "ReportTypeId", "dbo.ReportTypes");
            DropForeignKey("dbo.PProperties", "Order_OrderID", "dbo.Orders");
            DropForeignKey("dbo.PProperties", "CurrentLandlordId", "dbo.PLandlords");
            DropForeignKey("dbo.OrderNotes", "CreatedBy", "dbo.AuthUsers");
            DropForeignKey("dbo.OrderNotes", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "JobSubTypeId", "dbo.JobSubTypes");
            DropForeignKey("dbo.Orders", "DepartmentId", "dbo.TenantDepartments");
            DropForeignKey("dbo.Appointments", "OrderId", "dbo.Orders");
            DropForeignKey("dbo.Orders", "JobTypeId", "dbo.JobTypes");
            DropForeignKey("dbo.JobTypeResources", "Resources_ResourceId", "dbo.Resources");
            DropForeignKey("dbo.JobTypeResources", "JobType_JobTypeId", "dbo.JobTypes");
            DropForeignKey("dbo.Resources", "Nationality", "dbo.GlobalCountry");
            DropForeignKey("dbo.EmployeeRoles", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.EmployeeRoles", "RolesId", "dbo.Roles");
            DropForeignKey("dbo.Roles", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.EmployeeRoles", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.EmployeeGroups", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.EmployeeGroups", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.EmployeeGroups", "GroupsId", "dbo.Groups");
            DropForeignKey("dbo.Groups", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.Resources", "ContactNumbersId", "dbo.ContactNumbers");
            DropForeignKey("dbo.Appointments", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.ResourceShifts", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.ResourceShifts", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantEmailConfigs", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.AuthUsers", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.AuthUserprofiles", "UserId", "dbo.AuthUsers");
            DropForeignKey("dbo.AuthUserLoginActivities", "UserLoginId", "dbo.AuthUserLogins");
            DropForeignKey("dbo.AuthUserLogins", "UserId", "dbo.AuthUsers");
            DropForeignKey("dbo.AuthPermissions", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.TenantLocations", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductLocationStockLevel", "TenantLocationID", "dbo.TenantLocations");
            DropForeignKey("dbo.ProductLocationStockLevel", "ProductMasterID", "dbo.ProductMaster");
            DropForeignKey("dbo.TenantLocations", "ParentWarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.TenantLocations", "MarketVehicleID", "dbo.MarketVehicles");
            DropForeignKey("dbo.InventoryStocks", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.InventoryStocks", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantDepartments", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductMaster", "DepartmentId", "dbo.TenantDepartments");
            DropForeignKey("dbo.StockTakeSerialSnapshots", "StockTakeSnapshotId", "dbo.StockTakeSnapshots");
            DropForeignKey("dbo.StockTakeSerialSnapshots", "StockTakeId", "dbo.StockTakes");
            DropForeignKey("dbo.StockTakeSerialSnapshots", "ProductSerialId", "dbo.ProductSerialis");
            DropForeignKey("dbo.StockTakeSerialSnapshots", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.StockTakes", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.StockTakes", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.StockTakeSnapshots", "StockTakeId", "dbo.StockTakes");
            DropForeignKey("dbo.StockTakeDetails", "StockTakeId", "dbo.StockTakes");
            DropForeignKey("dbo.StockTakeDetails", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.StockTakeSnapshots", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductReceipeMasters", "ProductMaster_ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductReceipeMasters", "ProductMasterID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductReceipeMasters", "RecipeItemProductID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductSCCCodes", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductPriceLevels", "ProductMasterID", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductMaster", "LotProcessTypeCodeId", "dbo.ProductLotProcessTypeCodes");
            DropForeignKey("dbo.ProductMaster", "LotOptionCodeId", "dbo.ProductLotOptionsCodes");
            DropForeignKey("dbo.ProductKitMaps", "ProductMaster_ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductKitMaps", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductKitMaps", "KitProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductMaster", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductAttributeValuesMaps", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAttributeValuesMaps", "AttributeValueId", "dbo.ProductAttributeValues");
            DropForeignKey("dbo.ProductAttributeValues", "AttributeId", "dbo.ProductAttributes");
            DropForeignKey("dbo.InventoryTransactions", "WastageReasonId", "dbo.WastageReason");
            DropForeignKey("dbo.InventoryTransactions", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.InventoryTransactions", "Tenant_TenantId", "dbo.Tenants");
            DropForeignKey("dbo.ProductSerialis", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.TenantWarranties", "ProductSerialis_SerialID", "dbo.ProductSerialis");
            DropForeignKey("dbo.ProductSerialis", "PostageTypeId", "dbo.TenantPostageTypes");
            DropForeignKey("dbo.ProductSerialis", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductSerialis", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.InventoryTransactions", "SerialID", "dbo.ProductSerialis");
            DropForeignKey("dbo.InventoryTransactions", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.InventoryTransactions", "OrderProcessId", "dbo.OrderProcesses");
            DropForeignKey("dbo.OrderProcesses", "OrderProcessStatusId", "dbo.OrderProcessStatus");
            DropForeignKey("dbo.OrderProcessDetails", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.OrderProcessDetails", "OrderProcessId", "dbo.OrderProcesses");
            DropForeignKey("dbo.OrderProcessDetails", "OrderDetailID", "dbo.OrderDetails");
            DropForeignKey("dbo.OrderProcesses", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderProcesses", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.OrderProcesses", "ConsignmentTypeId", "dbo.OrderConsignmentTypes");
            DropForeignKey("dbo.InventoryTransactions", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.InventoryTransactions", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.InventoryTransactions", "InventoryTransactionTypeId", "dbo.InventoryTransactionTypes");
            DropForeignKey("dbo.InventoryStocks", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductMaster", "WeightGroupId", "dbo.GlobalWeightGroups");
            DropForeignKey("dbo.ProductMaster", "UOMId", "dbo.GlobalUOM");
            DropForeignKey("dbo.Locations", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.ProductLocations", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductLocations", "LocationId", "dbo.Locations");
            DropForeignKey("dbo.Locations", "LocationTypeId", "dbo.LocationTypes");
            DropForeignKey("dbo.Locations", "LocationGroupId", "dbo.LocationGroups");
            DropForeignKey("dbo.Locations", "UOMId", "dbo.GlobalUOM");
            DropForeignKey("dbo.GlobalUOM", "UOMTypeId", "dbo.GlobalUOMTypes");
            DropForeignKey("dbo.ProductMaster", "TaxID", "dbo.GlobalTax");
            DropForeignKey("dbo.OrderDetails", "WarrantyID", "dbo.TenantWarranties");
            DropForeignKey("dbo.TenantWarranties", "PostageTypeId", "dbo.TenantPostageTypes");
            DropForeignKey("dbo.TenantPostageTypes", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.TenantWarranties", "TenantId", "dbo.Tenants");
            DropForeignKey("dbo.OrderDetails", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.OrderDetails", "TaxID", "dbo.GlobalTax");
            DropForeignKey("dbo.OrderDetails", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.PalletProducts", "ProductID", "dbo.ProductMaster");
            DropForeignKey("dbo.Pallets", "RecipientAccountID", "dbo.Account");
            DropForeignKey("dbo.PalletsDispatches", "VehicleDriverResourceID", "dbo.Resources");
            DropForeignKey("dbo.PalletsDispatches", "MarketVehicleID", "dbo.MarketVehicles");
            DropForeignKey("dbo.MarketVehicles", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketVehicles", "MarketRoute_Id", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteCustomers", "MarketRouteId", "dbo.MarketRoutes");
            DropForeignKey("dbo.MarketRouteCustomers", "AccountId", "dbo.Account");
            DropForeignKey("dbo.MarketRoutes", "MarketId", "dbo.Markets");
            DropForeignKey("dbo.MarketRoutes", "LastVehicleId", "dbo.MarketVehicles");
            DropForeignKey("dbo.PalletsDispatches", "SentMethodID", "dbo.SentMethods");
            DropForeignKey("dbo.Pallets", "PalletsDispatchID", "dbo.PalletsDispatches");
            DropForeignKey("dbo.PalletProducts", "PalletID", "dbo.Pallets");
            DropForeignKey("dbo.PalletProducts", "OrderDetailID", "dbo.OrderDetails");
            DropForeignKey("dbo.PalletProducts", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "OrderDetailStatusId", "dbo.OrderStatus");
            DropForeignKey("dbo.Orders", "OrderStatusID", "dbo.OrderStatus");
            DropForeignKey("dbo.OrderDetails", "OrderID", "dbo.Orders");
            DropForeignKey("dbo.OrderDetails", "ProdAccCodeID", "dbo.ProductAccountCodes");
            DropForeignKey("dbo.ProductAccountCodes", "ProductId", "dbo.ProductMaster");
            DropForeignKey("dbo.ProductAccountCodes", "AccountID", "dbo.Account");
            DropForeignKey("dbo.GlobalTax", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.TenantLocations", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.EmployeeShifts_Stores", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.EmployeeShifts_Stores", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.TerminalsLogs", "TerminalId", "dbo.Terminals");
            DropForeignKey("dbo.Terminals", "WarehouseId", "dbo.TenantLocations");
            DropForeignKey("dbo.TenantLocations", "ContactNumbersId", "dbo.ContactNumbers");
            DropForeignKey("dbo.TenantLocations", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.AuthPermissions", "UserId", "dbo.AuthUsers");
            DropForeignKey("dbo.AuthActivities", "ModuleId", "dbo.Modules");
            DropForeignKey("dbo.AuthPermissions", "ActivityId", "dbo.AuthActivities");
            DropForeignKey("dbo.AuthActivityGroupMaps", "ActivityGroupId", "dbo.AuthActivityGroups");
            DropForeignKey("dbo.AuthActivityGroupMaps", "ActivityId", "dbo.AuthActivities");
            DropForeignKey("dbo.ResourceShifts", "ShiftStatusId", "dbo.ShiftStatus");
            DropForeignKey("dbo.ResourceShifts", "ResourceId", "dbo.Resources");
            DropForeignKey("dbo.Resources", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.Addresses", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.Orders", "AccountCurrencyID", "dbo.GlobalCurrency");
            DropForeignKey("dbo.Orders", "AccountContactId", "dbo.AccountContacts");
            DropForeignKey("dbo.Orders", "AccountID", "dbo.Account");
            DropForeignKey("dbo.Account", "CurrencyID", "dbo.GlobalCurrency");
            DropForeignKey("dbo.Account", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.Account", "AccountStatusID", "dbo.GlobalAccountStatus");
            DropForeignKey("dbo.AccountTransaction", "AccountTransactionTypeId", "dbo.AccountTransactionTypes");
            DropForeignKey("dbo.AccountTransaction", "AccountPaymentModeId", "dbo.AccountPaymentModes");
            DropForeignKey("dbo.AccountTransaction", "AccountId", "dbo.Account");
            DropForeignKey("dbo.AccountContacts", "AccountID", "dbo.Account");
            DropForeignKey("dbo.AccountAddresses", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.GlobalCurrency", "CountryID", "dbo.GlobalCountry");
            DropForeignKey("dbo.AccountAddresses", "AccountID", "dbo.Account");
            DropIndex("dbo.JobTypeResources", new[] { "Resources_ResourceId" });
            DropIndex("dbo.JobTypeResources", new[] { "JobType_JobTypeId" });
            DropIndex("dbo.VehicleInspection", new[] { "RectifiedUserId" });
            DropIndex("dbo.VehicleInspection", new[] { "ReportedToUserId" });
            DropIndex("dbo.VehicleInspection", new[] { "MarketVehicleId" });
            DropIndex("dbo.VehicleInspection", new[] { "VehicleDriverId" });
            DropIndex("dbo.VehicleInspectionConfirmedList", new[] { "VehicleInspectionCheckListId" });
            DropIndex("dbo.VehicleInspectionConfirmedList", new[] { "VehicleInspectionId" });
            DropIndex("dbo.VehicleInspectionCheckList", new[] { "VehicleInspectionTypeId" });
            DropIndex("dbo.TenantProfiles", new[] { "TenantId" });
            DropIndex("dbo.TenantModules", new[] { "TenantId" });
            DropIndex("dbo.TenantModules", new[] { "ModuleId" });
            DropIndex("dbo.TenantLoanTypes", new[] { "TenantId" });
            DropIndex("dbo.TenantEmailTemplateVariables", new[] { "TenantId" });
            DropIndex("dbo.TenantCurrenciesExRates", new[] { "Tenant_TenantId" });
            DropIndex("dbo.TenantCurrenciesExRates", new[] { "TenantCurrencyID" });
            DropIndex("dbo.TenantCurrencies", new[] { "TenantId" });
            DropIndex("dbo.TenantCurrencies", new[] { "CurrencyID" });
            DropIndex("dbo.TenantContacts", new[] { "TenantId" });
            DropIndex("dbo.TenantConfigs", new[] { "TenantId" });
            DropIndex("dbo.TenantConfigs", new[] { "DefaultCashAccountID" });
            DropIndex("dbo.StockTakeDetailsSerials", new[] { "StockTakeDetailId" });
            DropIndex("dbo.StockTakeDetailsSerials", new[] { "ProductId" });
            DropIndex("dbo.StockTakeDetailsSerials", new[] { "ProductSerialId" });
            DropIndex("dbo.Shifts", new[] { "TenantLocations_WarehouseId" });
            DropIndex("dbo.Shifts", new[] { "Resources_ResourceId" });
            DropIndex("dbo.Shifts", new[] { "TenantId" });
            DropIndex("dbo.ResourceJobAllocation", new[] { "ResourceJobStatusId" });
            DropIndex("dbo.ResourceJobAllocation", new[] { "ResourceId" });
            DropIndex("dbo.ResourceRequests", new[] { "TenantId" });
            DropIndex("dbo.ResourceRequests", new[] { "ResourceId" });
            DropIndex("dbo.PTenants", new[] { "CurrentPropertyId" });
            DropIndex("dbo.TenantEmailTemplates", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.TenantEmailTemplates", new[] { "TenantId" });
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "TenantEmailConfigId" });
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "AppointmentId" });
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "TenantEmailTemplatesId" });
            DropIndex("dbo.TenantEmailNotificationQueues", new[] { "OrderId" });
            DropIndex("dbo.OrderPTenantEmailRecipients", new[] { "LastEmailNotificationId" });
            DropIndex("dbo.OrderPTenantEmailRecipients", new[] { "PTenantId" });
            DropIndex("dbo.OrderPTenantEmailRecipients", new[] { "PPropertyId" });
            DropIndex("dbo.OrderPTenantEmailRecipients", new[] { "OrderId" });
            DropIndex("dbo.MarketRouteSchedules", new[] { "MarketId" });
            DropIndex("dbo.MarketRouteSchedules", new[] { "VehicleId" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "TenantLocationID" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "MarketRouteId" });
            DropIndex("dbo.MarketRouteAssociations", new[] { "MarketId" });
            DropIndex("dbo.MarketJobs", new[] { "MarketJobStatusId" });
            DropIndex("dbo.MarketJobs", new[] { "MarketRouteAssociationId" });
            DropIndex("dbo.MarketJobs", new[] { "AccountID" });
            DropIndex("dbo.InvoiceMaster", new[] { "AccountId" });
            DropIndex("dbo.InvoiceMaster", new[] { "OrderProcessId" });
            DropIndex("dbo.InvoiceDetail", new[] { "ProductId" });
            DropIndex("dbo.InvoiceDetail", new[] { "InvoiceMasterId" });
            DropIndex("dbo.AttLogs_EmployeeShifts", new[] { "EmployeeShiftsId" });
            DropIndex("dbo.AttLogs_EmployeeShifts", new[] { "AttLogsId" });
            DropIndex("dbo.TenantPriceGroups", new[] { "TenantId" });
            DropIndex("dbo.PProperties", new[] { "Order_OrderID" });
            DropIndex("dbo.PProperties", new[] { "CurrentLandlordId" });
            DropIndex("dbo.OrderNotes", new[] { "CreatedBy" });
            DropIndex("dbo.OrderNotes", new[] { "OrderID" });
            DropIndex("dbo.Roles", new[] { "TenantId" });
            DropIndex("dbo.EmployeeRoles", new[] { "TenantId" });
            DropIndex("dbo.EmployeeRoles", new[] { "RolesId" });
            DropIndex("dbo.EmployeeRoles", new[] { "ResourceId" });
            DropIndex("dbo.Groups", new[] { "TenantId" });
            DropIndex("dbo.EmployeeGroups", new[] { "TenantId" });
            DropIndex("dbo.EmployeeGroups", new[] { "GroupsId" });
            DropIndex("dbo.EmployeeGroups", new[] { "ResourceId" });
            DropIndex("dbo.TenantEmailConfigs", new[] { "TenantId" });
            DropIndex("dbo.AuthUserprofiles", new[] { "UserId" });
            DropIndex("dbo.AuthUserLoginActivities", new[] { "UserLoginId" });
            DropIndex("dbo.AuthUserLogins", new[] { "UserId" });
            DropIndex("dbo.ProductLocationStockLevel", new[] { "TenantLocationID" });
            DropIndex("dbo.ProductLocationStockLevel", new[] { "ProductMasterID" });
            DropIndex("dbo.TenantDepartments", new[] { "TenantId" });
            DropIndex("dbo.StockTakeSerialSnapshots", new[] { "ProductSerialId" });
            DropIndex("dbo.StockTakeSerialSnapshots", new[] { "ProductId" });
            DropIndex("dbo.StockTakeSerialSnapshots", new[] { "StockTakeId" });
            DropIndex("dbo.StockTakeSerialSnapshots", new[] { "StockTakeSnapshotId" });
            DropIndex("dbo.StockTakeDetails", new[] { "ProductId" });
            DropIndex("dbo.StockTakeDetails", new[] { "StockTakeId" });
            DropIndex("dbo.StockTakes", new[] { "TenantId" });
            DropIndex("dbo.StockTakes", new[] { "WarehouseId" });
            DropIndex("dbo.StockTakeSnapshots", new[] { "ProductId" });
            DropIndex("dbo.StockTakeSnapshots", new[] { "StockTakeId" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "ProductMaster_ProductId" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "RecipeItemProductID" });
            DropIndex("dbo.ProductReceipeMasters", new[] { "ProductMasterID" });
            DropIndex("dbo.ProductSCCCodes", new[] { "ProductId" });
            DropIndex("dbo.ProductPriceLevels", new[] { "ProductMasterID" });
            DropIndex("dbo.ProductKitMaps", new[] { "ProductMaster_ProductId" });
            DropIndex("dbo.ProductKitMaps", new[] { "KitProductId" });
            DropIndex("dbo.ProductKitMaps", new[] { "ProductId" });
            DropIndex("dbo.ProductAttributeValues", new[] { "AttributeId" });
            DropIndex("dbo.ProductAttributeValuesMaps", new[] { "AttributeValueId" });
            DropIndex("dbo.ProductAttributeValuesMaps", new[] { "ProductId" });
            DropIndex("dbo.ProductSerialis", new[] { "LocationId" });
            DropIndex("dbo.ProductSerialis", new[] { "WarehouseId" });
            DropIndex("dbo.ProductSerialis", new[] { "PostageTypeId" });
            DropIndex("dbo.ProductSerialis", new[] { "ProductId" });
            DropIndex("dbo.OrderProcessDetails", new[] { "OrderDetailID" });
            DropIndex("dbo.OrderProcessDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderProcessDetails", new[] { "OrderProcessId" });
            DropIndex("dbo.OrderProcesses", new[] { "OrderProcessStatusId" });
            DropIndex("dbo.OrderProcesses", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.OrderProcesses", new[] { "OrderID" });
            DropIndex("dbo.OrderProcesses", new[] { "ConsignmentTypeId" });
            DropIndex("dbo.InventoryTransactions", new[] { "Tenant_TenantId" });
            DropIndex("dbo.InventoryTransactions", new[] { "OrderProcessId" });
            DropIndex("dbo.InventoryTransactions", new[] { "SerialID" });
            DropIndex("dbo.InventoryTransactions", new[] { "LocationId" });
            DropIndex("dbo.InventoryTransactions", new[] { "WastageReasonId" });
            DropIndex("dbo.InventoryTransactions", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryTransactions", new[] { "ProductId" });
            DropIndex("dbo.InventoryTransactions", new[] { "OrderID" });
            DropIndex("dbo.InventoryTransactions", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.ProductLocations", new[] { "LocationId" });
            DropIndex("dbo.ProductLocations", new[] { "ProductId" });
            DropIndex("dbo.Locations", new[] { "UOMId" });
            DropIndex("dbo.Locations", new[] { "LocationTypeId" });
            DropIndex("dbo.Locations", new[] { "LocationGroupId" });
            DropIndex("dbo.Locations", new[] { "WarehouseId" });
            DropIndex("dbo.GlobalUOM", new[] { "UOMTypeId" });
            DropIndex("dbo.TenantPostageTypes", new[] { "TenantId" });
            DropIndex("dbo.TenantWarranties", new[] { "ProductSerialis_SerialID" });
            DropIndex("dbo.TenantWarranties", new[] { "TenantId" });
            DropIndex("dbo.TenantWarranties", new[] { "PostageTypeId" });
            DropIndex("dbo.MarketRouteCustomers", new[] { "MarketRouteId" });
            DropIndex("dbo.MarketRouteCustomers", new[] { "AccountId" });
            DropIndex("dbo.MarketRoutes", new[] { "MarketId" });
            DropIndex("dbo.MarketRoutes", new[] { "LastVehicleId" });
            DropIndex("dbo.MarketVehicles", new[] { "MarketRoute_Id" });
            DropIndex("dbo.MarketVehicles", new[] { "MarketId" });
            DropIndex("dbo.PalletsDispatches", new[] { "VehicleDriverResourceID" });
            DropIndex("dbo.PalletsDispatches", new[] { "SentMethodID" });
            DropIndex("dbo.PalletsDispatches", new[] { "MarketVehicleID" });
            DropIndex("dbo.Pallets", new[] { "PalletsDispatchID" });
            DropIndex("dbo.Pallets", new[] { "RecipientAccountID" });
            DropIndex("dbo.PalletProducts", new[] { "OrderID" });
            DropIndex("dbo.PalletProducts", new[] { "PalletID" });
            DropIndex("dbo.PalletProducts", new[] { "ProductID" });
            DropIndex("dbo.PalletProducts", new[] { "OrderDetailID" });
            DropIndex("dbo.ProductAccountCodes", new[] { "ProductId" });
            DropIndex("dbo.ProductAccountCodes", new[] { "AccountID" });
            DropIndex("dbo.OrderDetails", new[] { "OrderDetailStatusId" });
            DropIndex("dbo.OrderDetails", new[] { "TaxID" });
            DropIndex("dbo.OrderDetails", new[] { "WarrantyID" });
            DropIndex("dbo.OrderDetails", new[] { "ProdAccCodeID" });
            DropIndex("dbo.OrderDetails", new[] { "ProductId" });
            DropIndex("dbo.OrderDetails", new[] { "WarehouseId" });
            DropIndex("dbo.OrderDetails", new[] { "OrderID" });
            DropIndex("dbo.GlobalTax", new[] { "CountryID" });
            DropIndex("dbo.ProductMaster", new[] { "ProductGroupId" });
            DropIndex("dbo.ProductMaster", new[] { "DepartmentId" });
            DropIndex("dbo.ProductMaster", new[] { "TaxID" });
            DropIndex("dbo.ProductMaster", new[] { "WeightGroupId" });
            DropIndex("dbo.ProductMaster", new[] { "LotProcessTypeCodeId" });
            DropIndex("dbo.ProductMaster", new[] { "LotOptionCodeId" });
            DropIndex("dbo.ProductMaster", new[] { "UOMId" });
            DropIndex("dbo.InventoryStocks", new[] { "TenantId" });
            DropIndex("dbo.InventoryStocks", new[] { "WarehouseId" });
            DropIndex("dbo.InventoryStocks", new[] { "ProductId" });
            DropIndex("dbo.EmployeeShifts_Stores", new[] { "ResourceId" });
            DropIndex("dbo.EmployeeShifts_Stores", new[] { "WarehouseId" });
            DropIndex("dbo.TerminalsLogs", new[] { "TerminalId" });
            DropIndex("dbo.Terminals", new[] { "WarehouseId" });
            DropIndex("dbo.TenantLocations", new[] { "TenantId" });
            DropIndex("dbo.TenantLocations", new[] { "ParentWarehouseId" });
            DropIndex("dbo.TenantLocations", new[] { "MarketVehicleID" });
            DropIndex("dbo.TenantLocations", new[] { "ContactNumbersId" });
            DropIndex("dbo.TenantLocations", new[] { "AddressId" });
            DropIndex("dbo.TenantLocations", new[] { "CountryID" });
            DropIndex("dbo.AuthActivityGroupMaps", new[] { "ActivityGroupId" });
            DropIndex("dbo.AuthActivityGroupMaps", new[] { "ActivityId" });
            DropIndex("dbo.AuthActivities", new[] { "ModuleId" });
            DropIndex("dbo.AuthPermissions", new[] { "ActivityId" });
            DropIndex("dbo.AuthPermissions", new[] { "WarehouseId" });
            DropIndex("dbo.AuthPermissions", new[] { "UserId" });
            DropIndex("dbo.AuthUsers", new[] { "TenantId" });
            DropIndex("dbo.ResourceShifts", new[] { "TenantId" });
            DropIndex("dbo.ResourceShifts", new[] { "TerminalId" });
            DropIndex("dbo.ResourceShifts", new[] { "ResourceId" });
            DropIndex("dbo.ResourceShifts", new[] { "ShiftStatusId" });
            DropIndex("dbo.Addresses", new[] { "CountryID" });
            DropIndex("dbo.Resources", new[] { "ContactNumbersId" });
            DropIndex("dbo.Resources", new[] { "AddressId" });
            DropIndex("dbo.Resources", new[] { "Nationality" });
            DropIndex("dbo.Appointments", new[] { "OrderId" });
            DropIndex("dbo.Appointments", new[] { "ResourceId" });
            DropIndex("dbo.Orders", new[] { "JobSubTypeId" });
            DropIndex("dbo.Orders", new[] { "AccountCurrencyID" });
            DropIndex("dbo.Orders", new[] { "ShipmentWarehouseId" });
            DropIndex("dbo.Orders", new[] { "ShipmentPropertyId" });
            DropIndex("dbo.Orders", new[] { "WarehouseId" });
            DropIndex("dbo.Orders", new[] { "SLAPriorityId" });
            DropIndex("dbo.Orders", new[] { "DepartmentId" });
            DropIndex("dbo.Orders", new[] { "TransferTo_WarehouseId" });
            DropIndex("dbo.Orders", new[] { "TransferFrom_WarehouseId" });
            DropIndex("dbo.Orders", new[] { "ReportTypeId" });
            DropIndex("dbo.Orders", new[] { "AccountContactId" });
            DropIndex("dbo.Orders", new[] { "OrderStatusID" });
            DropIndex("dbo.Orders", new[] { "TenentId" });
            DropIndex("dbo.Orders", new[] { "JobTypeId" });
            DropIndex("dbo.Orders", new[] { "AccountID" });
            DropIndex("dbo.Orders", new[] { "InventoryTransactionTypeId" });
            DropIndex("dbo.AccountTransaction", new[] { "AccountTransactionTypeId" });
            DropIndex("dbo.AccountTransaction", new[] { "AccountPaymentModeId" });
            DropIndex("dbo.AccountTransaction", new[] { "AccountId" });
            DropIndex("dbo.AccountContacts", new[] { "AccountID" });
            DropIndex("dbo.GlobalCurrency", new[] { "CountryID" });
            DropIndex("dbo.AccountAddresses", new[] { "AccountID" });
            DropIndex("dbo.AccountAddresses", new[] { "CountryID" });
            DropIndex("dbo.Account", new[] { "PriceGroupID" });
            DropIndex("dbo.Account", new[] { "AccountStatusID" });
            DropIndex("dbo.Account", new[] { "CurrencyID" });
            DropIndex("dbo.Account", new[] { "CountryID" });
            DropTable("dbo.JobTypeResources");
            DropTable("dbo.VehicleInspection");
            DropTable("dbo.VehicleInspectionConfirmedList");
            DropTable("dbo.VehicleInspectionType");
            DropTable("dbo.VehicleInspectionCheckList");
            DropTable("dbo.TenantProfiles");
            DropTable("dbo.TenantModules");
            DropTable("dbo.TenantLoanTypes");
            DropTable("dbo.TenantEmailTemplateVariables");
            DropTable("dbo.TenantCurrenciesExRates");
            DropTable("dbo.TenantCurrencies");
            DropTable("dbo.TenantContacts");
            DropTable("dbo.TenantConfigs");
            DropTable("dbo.StockTakeScanLogs");
            DropTable("dbo.StockTakeDetailsSerials");
            DropTable("dbo.Shifts");
            DropTable("dbo.ResourceJob");
            DropTable("dbo.ResourceJobStatus");
            DropTable("dbo.ResourceJobAllocation");
            DropTable("dbo.ResourceRequests");
            DropTable("dbo.PSyncHistories");
            DropTable("dbo.PTenants");
            DropTable("dbo.TenantEmailTemplates");
            DropTable("dbo.TenantEmailNotificationQueues");
            DropTable("dbo.OrderPTenantEmailRecipients");
            DropTable("dbo.OperLogsStamps");
            DropTable("dbo.OperLogs");
            DropTable("dbo.MarketRouteSchedules");
            DropTable("dbo.MarketRouteAssociations");
            DropTable("dbo.MarketJobStatus");
            DropTable("dbo.MarketJobs");
            DropTable("dbo.InvoiceMaster");
            DropTable("dbo.InvoiceDetail");
            DropTable("dbo.AttLogsStamps");
            DropTable("dbo.AttLogs_EmployeeShifts");
            DropTable("dbo.AttLogs");
            DropTable("dbo.TenantPriceGroups");
            DropTable("dbo.SLAPriorits");
            DropTable("dbo.ReportTypes");
            DropTable("dbo.PLandlords");
            DropTable("dbo.PProperties");
            DropTable("dbo.OrderNotes");
            DropTable("dbo.JobSubTypes");
            DropTable("dbo.JobTypes");
            DropTable("dbo.Roles");
            DropTable("dbo.EmployeeRoles");
            DropTable("dbo.Groups");
            DropTable("dbo.EmployeeGroups");
            DropTable("dbo.TenantEmailConfigs");
            DropTable("dbo.AuthUserprofiles");
            DropTable("dbo.AuthUserLoginActivities");
            DropTable("dbo.AuthUserLogins");
            DropTable("dbo.ProductLocationStockLevel");
            DropTable("dbo.TenantDepartments");
            DropTable("dbo.StockTakeSerialSnapshots");
            DropTable("dbo.StockTakeDetails");
            DropTable("dbo.StockTakes");
            DropTable("dbo.StockTakeSnapshots");
            DropTable("dbo.ProductReceipeMasters");
            DropTable("dbo.ProductSCCCodes");
            DropTable("dbo.ProductPriceLevels");
            DropTable("dbo.ProductLotProcessTypeCodes");
            DropTable("dbo.ProductLotOptionsCodes");
            DropTable("dbo.ProductKitMaps");
            DropTable("dbo.ProductGroups");
            DropTable("dbo.ProductAttributes");
            DropTable("dbo.ProductAttributeValues");
            DropTable("dbo.ProductAttributeValuesMaps");
            DropTable("dbo.WastageReason");
            DropTable("dbo.ProductSerialis");
            DropTable("dbo.OrderProcessStatus");
            DropTable("dbo.OrderProcessDetails");
            DropTable("dbo.OrderConsignmentTypes");
            DropTable("dbo.OrderProcesses");
            DropTable("dbo.InventoryTransactionTypes");
            DropTable("dbo.InventoryTransactions");
            DropTable("dbo.GlobalWeightGroups");
            DropTable("dbo.ProductLocations");
            DropTable("dbo.LocationTypes");
            DropTable("dbo.LocationGroups");
            DropTable("dbo.Locations");
            DropTable("dbo.GlobalUOMTypes");
            DropTable("dbo.GlobalUOM");
            DropTable("dbo.TenantPostageTypes");
            DropTable("dbo.TenantWarranties");
            DropTable("dbo.MarketRouteCustomers");
            DropTable("dbo.MarketRoutes");
            DropTable("dbo.Markets");
            DropTable("dbo.MarketVehicles");
            DropTable("dbo.SentMethods");
            DropTable("dbo.PalletsDispatches");
            DropTable("dbo.Pallets");
            DropTable("dbo.PalletProducts");
            DropTable("dbo.OrderStatus");
            DropTable("dbo.ProductAccountCodes");
            DropTable("dbo.OrderDetails");
            DropTable("dbo.GlobalTax");
            DropTable("dbo.ProductMaster");
            DropTable("dbo.InventoryStocks");
            DropTable("dbo.EmployeeShifts_Stores");
            DropTable("dbo.TerminalsLogs");
            DropTable("dbo.Terminals");
            DropTable("dbo.ContactNumbers");
            DropTable("dbo.TenantLocations");
            DropTable("dbo.Modules");
            DropTable("dbo.AuthActivityGroups");
            DropTable("dbo.AuthActivityGroupMaps");
            DropTable("dbo.AuthActivities");
            DropTable("dbo.AuthPermissions");
            DropTable("dbo.AuthUsers");
            DropTable("dbo.Tenants");
            DropTable("dbo.ShiftStatus");
            DropTable("dbo.ResourceShifts");
            DropTable("dbo.Addresses");
            DropTable("dbo.Resources");
            DropTable("dbo.Appointments");
            DropTable("dbo.Orders");
            DropTable("dbo.GlobalAccountStatus");
            DropTable("dbo.AccountTransactionTypes");
            DropTable("dbo.AccountPaymentModes");
            DropTable("dbo.AccountTransaction");
            DropTable("dbo.AccountContacts");
            DropTable("dbo.GlobalCurrency");
            DropTable("dbo.GlobalCountry");
            DropTable("dbo.AccountAddresses");
            DropTable("dbo.Account");
        }
    }
}

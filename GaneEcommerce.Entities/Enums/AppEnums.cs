﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Ganedata.Core.Entities.Enums
{
    public enum EnumAttStatusType
    {

        Shift = 1,
        Break = 2,
        Lunch = 3,
        Unknown = 4,
        In = 5,
        Out = 6
    }
    public enum EnumProcessOrderType
    {
        Normal = 1,
        Serialize = 2,
        PalleteProcess = 3,


    }

    public enum EnumAccountType
    {
        All = 0,
        Customer = 1,
        Supplier = 2,
        EndUser = 3
    }

    public enum EnumProductCodeType
    {
        All = 0,
        SkuCode = 1,
        Barcode = 2
    }

    public enum EnumUomType
    {
        All = 0,
        Weight = 1,
        Dimensions = 2
    }

    public enum MailMergeVariableEnum
    {

        CompanyName = 1,
        AccountCode = 2,
        AccountRemittancesContactName = 3,
        AccountStatementsContactName = 4,
        AccountInvoicesContactName = 5,
        AccountMarketingContactName = 6,
        OrderId = 7,
        OrderNumber = 8,
        OrderStatus = 9,
        BillingAccountToEmail = 10,
        WorksOrderResourceName = 11,
        WorksOrderTimeslot = 12,
        WorksTenantName = 13,
        WorkPropertyAddress = 14,
        WorksJobTypeDescription = 15,
        WorksJobSubTypeDescription = 16,
        WorksSlaJobPriorityName = 17,
        WorksPropertyContactNumber = 18,
        ScheduledDate = 19,
        CustomMessage = 20,
        AccountPurchasingContactName = 21,
        UserName = 22,
        ConfirmationLink = 23
    }

    public enum WorksOrderNotificationTypeEnum
    {
        [Display(Name = "Works Order Log Template")]
        WorksOrderLogTemplate = 1,
        [Display(Name = "Works Order Scheduled Template")]
        WorksOrderScheduledTemplate = 2,
        [Display(Name = "Works Order Completed Template")]
        WorksOrderCompletedTemplate = 3,
        [Display(Name = "Purchase Order Template")]
        PurchaseOrderTemplate = 4,
        [Display(Name = "Sales Order Template")]
        SalesOrderTemplate = 5,
        [Display(Name = "Works Order Blank List")]
        WorksOrderBlankList = 6,
        Standard = 7,
        [Display(Name = "Sales Order Update Template")]
        SalesOrderUpdateTemplate = 8,
        [Display(Name = "Product Group Template")]
        ProductGroupTemplate = 9,
        [Display(Name = "Awaiting Order Template")]
        AwaitingOrderTemplate = 10,
        [Display(Name = "Invoice Template")]
        InvoiceTemplate = 11,
        [Display(Name = "Account Statement Template")]
        AccountStatementTemplate = 12,
        [Display(Name = "Direct Sales Order Invoice Template")]
        DirectSalesOrderReportTemplate = 13,
        [Display(Name = "Email Confirmation")]
        EmailConfirmation = 14,
    }

    public enum InventoryTransactionTypeEnum
    {
        [Display(Name = "Goods In / Purchase Order")]
        PurchaseOrder = 1,
        [Display(Name = "Goods Out / Sales Order")]
        SalesOrder = 2,
        TransferIn = 3,
        TransferOut = 4,
        Allocated = 5,
        AdjustmentIn = 6,
        AdjustmentOut = 7,
        WorksOrder = 8,
        Proforma = 9,
        Quotation = 10,
        Loan = 11,
        Returns = 12,
        Samples = 13,
        Wastage = 14,
        DirectSales = 15,
        Exchange = 16,
        WastedReturn = 17,
        StockMovement = 18
    }

    public enum OrderStatusEnum
    {
        Active = 1,
        Complete = 2,
        Hold = 3,
        Pending = 4,
        NotScheduled = 5,
        Scheduled = 6,
        ReAllocationRequired = 7,
        AwaitingAuthorisation = 8,
        Cancelled = 9,
        BeingPicked = 10,
        AwaitingArrival = 11,
        Approved = 12,
        [Display(Name = "Posted To Accounts")]
        PostedToAccounts = 13,
        Invoiced = 14
    }

    public enum PalletStatusEnum
    {
        Active = 1,
        Completed = 2
    }

    public enum PalletTrackingStatusEnum
    {
        Created = 1,
        Active = 2,
        Completed = 3,
        Hold = 4,
        Archived = 5
    }

    public enum GenderTypeEnum
    {
        Male = 1,
        Female = 2,
        Other = 3
    }

    public enum SyncEntityTypeEnum
    {
        Tenants = 1,
        Landlords = 2,
        Properties = 3
    }

    public enum NameTitlesEnum
    {
        Mr,
        Miss,
        Mrs,
        Ms,
        Doc,
        Sir
    }

    public enum ResourceRequestTypesEnum
    {
        [Display(Name = "Annual Holiday")]
        AnnualHoliday = 1,
        Meeting = 2,
        [Display(Name = "Sick Leave")]
        SickLeave = 3,
        Casual = 4
    }

    public enum ResourceRequestStatusEnum
    {
        Created = 1,
        Accepted = 2,
        Declined = 3,
        Cancelled = 4
    }

    public enum VerifySerilaStockStatusEnum
    {
        NotExist,
        InStock,
        OutofStock
    }

    public enum MarketJobStatusEnum
    {
        UnAllocated = 1,
        Allocated = 2,
        Accepted = 3,
        Declined = 4,
        Completed = 5,
        FailedToComplete = 6,
        Cancelled = 7
    }

    public enum AccountStatusEnum
    {
        Active = 1,
        InActive = 2,
        OnStop = 3
    }

    public enum TerminalLogTypeEnum
    {
        UsersSync = 1,
        ProductsSync = 2,
        AccountsSync = 3,
        ProductSerialSync = 4,
        InventoryStockSync = 5,
        StockTakeSync = 6,
        OrdersSync = 7,
        OrderProcessSync = 8,
        OrderProcessesPost = 9,
        PalletingSync = 10,
        MarketJobSync = 11,
        MarketVehiclesSync = 12,
        MarketRouteScheduleSync = 13,
        RandomJobsSync = 14,
        HolidaySync = 15,
        VanSalesDailyReport = 16,
        PalletTrackingSync = 17,
        TenantPriceGroupsSync = 18,
        TenantPriceGroupDetailsSync = 19,
        OrderReceiveCountSync = 20,
        PostOrderReceiveCount = 21,
        PalletDispatchMethodsSync = 22,
        PostPalletTracking = 23,
        MarketRoutesSync = 24,
        DayMarketRouteSync = 25,
        VehicleCheckLists = 26,
        TnAGetCommand = 27,
        TnAGetConfig = 28,
        TnAPostStampsAndLogs = 29,
        TnAPostCmdResponse = 30,
        PalletProductsSync = 31,
        PrestaShopOrderSync = 32,
        LocationsSync = 33,
        PostStockMovement = 34

    }

    public enum OrderProcessStatusEnum
    {
        Active = 1,
        Complete = 2,
        Dispatched = 3,
        Loaded = 4,
        Delivered = 5,
        Invoiced = 6,
        PostedToAccounts = 7
    }
    public enum AccountTransactionTypeEnum
    {
        InvoicedToAccount = 1,
        PaidByAccount = 2,
        Refund = 3,
        CreditNote = 4,
        Discount = 5
    }

    public enum InvoiceStatusEnum
    {
        Created = 1,
        PostedToAccounts = 2,
        Paid = 3
    }

    public enum TenantModuleEnum
    {
        Core = 1,
        SalesOrder = 2,
        PurchaseOrder = 3,
        WorksOrder = 4,
        Warehouse = 5,
        PointOfSale = 6,
        VanSales = 7,
        Palleting = 8,
        HumanResources = 9,
        TimeAndAttendance = 10,
        AssetTracking = 11,
        Accounting = 12,
        POD = 13,
        Accounts = 14,
        Products = 15,
        OrdersCore = 16,
        Ecommerce = 17
    }

    public enum AccountPaymentModeEnum
    {
        Cash = 1,
        Card = 2,
        OnlineTransfer = 3,
        BankDeposit = 4,
        Cheque = 5
    }

    public enum MarketCustomerVisitFrequency
    {
        Daily = 1,
        Weekly = 2,
        Fortnightly = 3,
        Monthly = 4,
        ThreeWeekly = 5
    }
    public enum ConsignmentTypeEnum
    {
        Standard = 1,
        Priority = 2,
        PreTen = 3,
        Collection = 4
    }

    public enum PalletTrackingSchemeEnum
    {
        FirstInFirstOut = 1,
        FirstInLastOut = 2,
        ByExpiryDate = 3,
        ByExpiryMonth = 4,
        DontEnforce = 5
    }

    public enum ScannedCodeTypeEnum
    {
        ProductCode = 1,
        SerialCode = 2,
        PalletCode = 3
    }
    public enum SortProductTypeEnum
    {
        [Display(Name = "Name(A-Z)")]
        NameByDesc = 1,
        [Display(Name = "Name(Z-A)")]
        NameByAsc = 2,
        [Display(Name = "Price (High > Low)")]
        PriceByDesc = 3,
        [Display(Name = "Price (Low > High)")]

        PriceByAsc = 4
    }

    public enum PalletDispatchStatusEnum
    {
        Created = 1,
        Loaded = 2,
        Delivered = 3,
        Scheduled = 4,
    }

    public enum TnALogsStampType
    {
        AttendanceLogStamp = 1,
        OperatorLogStamp = 2
    }

    public enum ProductDeliveryTypeEnum
    {
        HandBall = 1,
        Palleting = 2
    }

    public enum DeliveryMethods
    {
        InternalFleet = 1,
        Collection = 2,
        DPD = 3,
        DHL = 4,
        ParcelForce = 5
    }

    public enum TenantWebsiteTypes
    {
        Internal = 1,
        PrestaShop = 2,
    }
    public enum ApiTypes
    {
        MerakiSecretKey = 1,
        DPD = 2
    }
    public enum ProductKitTypeEnum
    {
        Kit = 1,
        Grouped = 2,
        Recipe = 3,
    }

    public enum WarehouseThemeEnum
    {
        Default = 1,
        Modern = 2
    }

    public enum WebsiteThemeEnum
    {
        ElecTech = 1,
        Smart = 2,
        University = 3
    }

    public enum WebsiteNavigationType
    {
        Blank = 1,
        Content = 2,
        Category = 3
    }

    public enum WebsiteDiscountTypeEnum
    {
        [Display(Name = "All In Basket")]
        AllInBasket = 1,
        [Display(Name = "Any In Basket")]
        AnyInBasket = 2,
        [Display(Name = "Apply On Cheapest")]
        ApplyOnCheapest = 3
    }

    public enum ProductAllowanceAdjustmentReasons
    {
        [Display(Name = "Size Issue")]
        SizeIssue = 1,
        [Display(Name = "Incorrectly Ordered")]
        IncorrectlyOrdered = 2,
        Other = 3
    }

    public enum ContentType
    {
        page = 1,
        post = 2
    }
}

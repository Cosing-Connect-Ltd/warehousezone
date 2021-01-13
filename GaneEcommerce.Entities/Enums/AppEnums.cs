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
        ConfirmationLink = 23,
        TransactionReferenceNumber = 24
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

        [Display(Name = "Website Order Confirmation")]
        WebsiteOrderConfirmation = 15,
        [Display(Name = "Order Status Notification")]
        OrderStatusNotification = 16,
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
        MovementIn = 18,
        MovementOut = 19
    }

    public enum OrderStatusEnum
    {
        Active = 1,
        Complete = 2,
        Hold = 3,
        Pending = 4,
        [Display(Name = "Not Scheduled")]
        NotScheduled = 5,
        Scheduled = 6,
        [Display(Name = "Reallocation Required")]
        ReAllocationRequired = 7,
        [Display(Name = "Awaiting Authorisation")]
        AwaitingAuthorisation = 8,
        Cancelled = 9,
        [Display(Name = "Being Picked")]
        BeingPicked = 10,
        [Display(Name = "Awaiting Arrival")]
        AwaitingArrival = 11,
        Approved = 12,
        [Display(Name = "Posted To Accounts")]
        PostedToAccounts = 13,
        Invoiced = 14,
        Preparing = 15,
        [Display(Name = "Quality Checks")]
        QualityChecks = 16,
        [Display(Name = "Out For Delivery")]
        OutForDelivery = 17,
        Delivered = 18,
        Rejected = 19,
        Failed = 20,
        PosFailed = 21,
        ParseFailed = 22,
        Prepared = 23,
        ReadyForPickup = 24,
        Finalised = 25,
        Accepted = 26
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
        OnStop = 3,
        OnHold = 4
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
        PostStockMovement = 34,
        TenantLocationsSyncLog = 35,
        UserAccountsSync = 36,
        AccountAddressesSync = 37,
        ProductLocationStocksSync = 38
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
        Ecommerce = 17,
        FoodDelivery = 18
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

    public enum FoodOrderTypeEnum
    {
        Delivery = 1,
        Collection = 2,
        EatIn = 3
    }

    public enum LoyaltyAppOrderProcessTypeEnum
    {
        Internal = 0,
        Deliverect = 1
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
        [Display(Name = "Recommended")]
        Recommended = 0,

        [Display(Name = "Name (A-Z)")]
        NameByAsc = 1,

        [Display(Name = "Name (Z-A)")]
        NameByDesc = 2,

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

    public enum ApiTypes
    {
        Meraki = 1,
        DPD = 2,
        PrestaShop = 3,
        PayPal = 4,
        SagePay = 5,
        GetAddress = 6,
        Adyen = 7,
        Deliverect = 8
    }

    public enum ProductKitTypeEnum
    {
        Kit = 1,
        Grouped = 2,
        Recipe = 3,
        Simple = 4,

        [Display(Name = "Product By Attribute")]
        ProductByAttribute = 5,

        RelatedProduct = 6,
        Virtual = 7
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
        Category = 3,
        Link = 4
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

    public enum UISettingItemInputType
    {
        Color = 1,
        Text = 2,
        Number = 3,
        Font = 4
    }

    public enum UISettingFonts
    {
        Arial,
        Calibri,
        Cambria,
        Candara,
        Courior,
        Cursive,
        Didot,
        Fantasy,
        Garamond,
        Geneva,
        Gotham,
        Helvetica,
        Monaco,
        Monospace,
        Montserrat,
        OpenSans,
        Optima,
        Perpetua,
        Roboto,
        SegoeUI,
        Serif,
        Tahoma,
        Times,
        Verdana,
    }

    public enum CheckoutStep
    {
        DeliveryMethod = 0,
        ShippingAddress = 1,
        CollectionPoint = 2,
        ShipmentRule = 3,
        PaymentDetails = 4,
        AddOrEditAddress = 5,
    }

    public enum DeliveryMethod
    {
        ToPickupPoint = 1,
        ToShipmentAddress = 2
    }

    public enum AddressType
    {
        Billing = 1,
        Shipping = 2
    }

    public enum PaymentMethodEnum
    {
        PayPal = 1,
        Cash = 2,
        SagePay = 3,
        PurchaseOrder = 4,
        Adyen = 5
    }

    public enum OrderProgressStep
    {
        Product = 1,
        Basket = 2,
        BillingAndShipping = 3,
        ReviewItems = 4,
        Payment = 5,
        OrderConfirmation = 6
    }

    public enum UserVerifyTypes
    {
        Mobile = 1,
        Email = 2
    }

    public enum TaxTypeEnum
    {
        All = 0,
        Products = 1,
        Accounts = 2
    }

    public enum ShopDeliveryTypeEnum
    {
        NextDay = 1,
        Standard = 2
    }

    public enum ShoppingVoucherDiscountTypeEnum
    {
        Percentage = 1,
        FixedPrice = 2
    }

    public enum ShoppingVoucherStatus
    {
        Active = 1,
        Applied = 2,
        Expired = 3,
        Invalid= 4
    }
     

}
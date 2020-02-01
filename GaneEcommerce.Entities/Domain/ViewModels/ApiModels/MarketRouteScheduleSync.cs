using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;

namespace Ganedata.Core.Models
{
    public class MarketRouteScheduleSync
    {
        public int MarketRouteScheduleId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Subject { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }
        public int Label { get; set; }
        public string Location { get; set; }
        public bool AllDay { get; set; }
        public int EventType { get; set; }
        public string RecurrenceInfo { get; set; }
        public string ReminderInfo { get; set; }

        public string VehicleIDs { get; set; }
        public int? TenentId { get; set; }
        public bool IsCanceled { get; set; }
        public string CancelReason { get; set; }

        public int? VehicleId { get; set; }
        public int MarketId { get; set; }

        public string MarketName { get; set; }
        public string RouteName { get; set; }
    }

    public class MarketRouteScheduleSyncCollection
    {
        public List<MarketRouteScheduleSync> MarketRouteSchedules { get; set; }

        public Guid TerminalLogId { get; set; }

        public int Count { get; set; }
    }

    public class MarketRouteAccounts
    {
        public int AccountId { get; set; }
        public int Order { get; set; }
        public int MarketRouteId { get; set; }
        public string MarketRouteName { get; set; }
        public bool IsSkippable { get; set; }
        public DateTime? SkipFromDate { get; set; }
        public DateTime? SkipToDate { get; set; }
    }

    public class MarketRouteSync
    {
        public int Id { get; set; }
        public int RouteScheduleId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? RouteDurationMins { get; set; }
        public int? MarketId { get; set; }
        public List<MarketSync> Market { get; set; }
        public int SortOrder { get; set; }
    }


    public class MarketSync
    {
        public int MarketId { get; set; }

        public string MarketName { get; set; }

        public string Description { get; set; }

        public int SortOrder { get; set; }

        public List<MarketCustomersSync> MarketCustomers { get; set; }
    }

    public class MarketCustomersSync
    {
        public int MarketCustomerId { get; set; }

        public string MarketCustomerName { get; set; }

        public string FullAddress { get; set; }

        public string ContactNumber { get; set; }

        public bool IsSkippable { get; set; }

        public int SortOrder { get; set; }
        public int MarketCustomerAccountId { get; set; }
    }

    public class MarketRouteSyncCollection
    {
        public List<MarketRouteSync> MarketRoutes { get; set; }

        public Guid TerminalLogId { get; set; }

        public int Count { get; set; }
    }


    public class HolidayRequestSync
    {
        public int HolidayRequestId { get; set; }
        public int UserId { get; set; }
        public string HolidayReason { get; set; }
        public string RequestNotes { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsAllDay { get; set; }
        public string SerialNumber { get; set; }
        public bool? IsDeleted { get; set; }
        public ResourceRequestTypesEnum RequestType { get; set; }
        public Guid TransactionLogId { get; set; }

    }

    public class HolidayResponseSync : HolidayRequestSync
    {
        public ResourceRequestStatusEnum RequestStatus { get; set; }
        public int? ActionedBy { get; set; }
        public string ActionReason { get; set; }

    }

    public class HolidaySyncCollection
    {
        public Guid TerminalLogId { get; set; }
        public int Count { get; set; }
        public List<HolidayResponseSync> HolidayResponseSync { get; set; }
    }

    public class AccountTransactionFileSync
    {
        public int AccountTransactionFileID { get; set; }

        public int AccountTransactionID { get; set; }

        public string Title { get; set; }

        public string FileName { get; set; }

        public string FileExtension { get; set; }

        public byte[] FileContent { get; set; }

        public int UserId { get; set; }

        public bool IsDeleted { get; set; }

        public string SerialNumber { get; set; }

        public decimal ChequeAmount { get; set; }

        public DateTime? CreatedDate { get; set; }
    }

}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{

    public class WorksOrdersGridViewModel
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public string JobNotes { get; set; }
        public string JobTypeName { get; set; }
        public string JobSubTypeName { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string POStatus { get; set; }
        public string Account { get; set; }
        public string Property { get; set; }
        public int OrderTypeId { get; set; }
        public IEnumerable<OrderNotesViewModel> OrderNotesList { get; set; }
        public string OrderType { get; set; }
    }

    public class WorksOrdersScheduledGridViewModel
    {
        public int OrderID { get; set; }
        public string OrderNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public string JobNotes { get; set; }
        public string JobTypeName { get; set; }
        public string JobSubTypeName { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string POStatus { get; set; }
        public string Account { get; set; }
        public string Property { get; set; }
        public int OrderTypeId { get; set; }
        public IEnumerable<OrderNotesViewModel> OrderNotesList { get; set; }
        public string OrderType { get; set; }
        public Appointments Appointment { get; set; }



        public DateTime? ScheduledStartTime { get; set; }
        public DateTime? ScheduledEndTime { get; set; }
        public int? ResourceId { get; set; }
        public string ResourceName { get; set; }
    }


    public class PriceRequestViewModel
    {
        public int AccountId { get; set; }
        public int ProductId { get; set; }
    }

    public class POHDetailViewModel
    {
        public string GTIN { get; set; }
        public string Warehouse { get; set; }
        public Decimal Qty { get; set; }
        public Decimal QtyReceived { get; set; }
    }
}
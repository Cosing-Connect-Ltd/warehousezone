using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Entities.Domain.ViewModels
{
    public class DPDViewModel
    {
       public List<DpdService> data { get; set; }

    }
    public class DpdService
    {
        public Network network { get; set; }
    }
    public class Network
    {
        public string networkCode { get; set; }
        public string networkDescription { get; set; }
    }
    public class DpdShipmentDataViewModel
    {
        public int? jobId { get; set; }
        public bool collectionOnDelivery { get; set; }
        public object invoice { get; set; }
        public DateTime collectionDate { get; set; }
        public bool consolidate { get; set; }
        public List<Consignment> consignment { get; set; }
    }

    public class ContactDetails
    {
        public string contactName { get; set; }
        public string telephone { get; set; }
    }
   
    public class Address1
    {
        public string organisation { get; set; }
        public string countryCode { get; set; }
        public string postcode { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string town { get; set; }
        public string county { get; set; }
    }

    public class CollectionDetails
    {
        public ContactDetails contactDetails { get; set; }
        public Address1 address { get; set; }
    }

    public class ContactDetails2
    {
        public string contactName { get; set; }
        public string telephone { get; set; }
    }

    public class Address2
    {
        public string organisation { get; set; }
        public string countryCode { get; set; }
        public string postcode { get; set; }
        public string street { get; set; }
        public string locality { get; set; }
        public string town { get; set; }
        public string county { get; set; }
    }

    public class NotificationDetails
    {
        public string email { get; set; }
        public string mobile { get; set; }
    }

    public class DeliveryDetails
    {
        public ContactDetails2 contactDetails { get; set; }
        public Address2 address { get; set; }
        public NotificationDetails notificationDetails { get; set; }
    }

    public class Consignment
    {
        public int? consignmentNumber { get; set; }
        public int? consignmentRef { get; set; }
        public List<object> parcel { get; set; }
        public CollectionDetails collectionDetails { get; set; }
        public DeliveryDetails deliveryDetails { get; set; }
        public string networkCode { get; set; }
        public int numberOfParcels { get; set; }
        public decimal? totalWeight { get; set; }
        public string shippingRef1 { get; set; }
        public string shippingRef2 { get; set; }
        public string shippingRef3 { get; set; }
        public object customsValue { get; set; }
        public string deliveryInstructions { get; set; }
        public string parcelDescription { get; set; }
        public object liabilityValue { get; set; }
        public bool liability { get; set; }
    }


    public class ConsignmentDetail
    {
        public string consignmentNumber { get; set; }
        public List<string> parcelNumbers { get; set; }
    }

    public class DpdResponseData
    {
        public int shipmentId { get; set; }
        public bool consolidated { get; set; }
        public List<ConsignmentDetail> consignmentDetail { get; set; }
    }

    public class DpdReponseViewModel
    {
        public object error { get; set; }
        public DpdResponseData data { get; set; }
    }
    public class Error
    {
        public string errorCode { get; set; }
        public string obj { get; set; }
        public string errorType { get; set; }
        public string errorMessage { get; set; }
        public object errorAction { get; set; }
    }

    public class DpdErrorViewModel
    {
        public List<Error> error { get; set; }
        public object data { get; set; }
    }




}
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WMS.Helpers
{
    public class PropertyContactInfo : PContactInfo
    {
        public string PropertyCode { get; set; }
    }
    public class PTenantResponse
    {
        public List<PTenant> Tenants { get; set; }

        public static PTenantResponse BindJson(string json)
        {
            var tenantsResponse = new PTenantResponse();
            tenantsResponse = JsonConvert.DeserializeObject<PTenantResponse>(json);
            return tenantsResponse;
        }
    }

    public class PTenantDetailViewModel
    {
        public int PTenantID { get; set; }
        public string TenantFullName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public bool Selected { get; set; }
    }

    public class PropertyTenantsViewModel
    {
        public string PropertyAddress { get; set; }
        public List<PTenantDetailViewModel> Tenants { get; set; }
    }

    public class PLandlordResponse
    {
        public List<PLandlord> Landlords { get; set; }

        public static PLandlordResponse BindJson(string json)
        {
            var landlordResponse = new PLandlordResponse();
            landlordResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<PLandlordResponse>(json);
            return landlordResponse;
        }
    }
    public class PPropertyResponse
    {
        public List<PProperty> Properties { get; set; }

        public static PPropertyResponse BindJson(string json)
        {
            var response = new PPropertyResponse();
            response = Newtonsoft.Json.JsonConvert.DeserializeObject<PPropertyResponse>(json);
            return response;
        }
    }


    public class PropertySyncItemsResponse
    {
        public int Id { get; set; }

        public int SiteId { get; set; }

        public string ItemCode { get; set; }

        public string Description { get; set; }

        public SyncEntityTypeEnum SyncEntityType { get; set; }

        public static PropertySyncItemsResponse Map(object request, SyncEntityTypeEnum type)
        {
            var response = new PropertySyncItemsResponse();
            switch (type)
            {
                case SyncEntityTypeEnum.Tenants:
                    var tenant = request as PTenant;
                    if (tenant != null)
                    {
                        response.SiteId = tenant.SiteId;
                        response.Description = tenant.TenantFullName;
                        response.Id = tenant.PTenantId;
                        response.ItemCode = tenant.TenantYCode;
                    }
                    break;

                case SyncEntityTypeEnum.Landlords:
                    var landlord = request as PLandlord;
                    if (landlord != null)
                    {
                        response.SiteId = landlord.SiteId;
                        response.Description = landlord.LandlordFullname;
                        response.Id = landlord.PLandlordId;
                        response.ItemCode = landlord.LandlordCode;
                    }
                    break;

                case SyncEntityTypeEnum.Properties:
                    var property = request as PProperty;
                    if (property != null)
                    {
                        response.SiteId = property.SiteId;
                        response.Description =
                            $"{property.AddressLine1}-{property.AddressLine2}-{property.AddressPostcode}";
                        response.Id = property.PPropertyId;
                        response.ItemCode = property.PropertyCode;
                    }
                    break;

            }
            return response;
        }

        public static List<PropertySyncItemsResponse> MapAll(IEnumerable<Object> requests, SyncEntityTypeEnum type)
        {
            var responses = new List<PropertySyncItemsResponse>();
            foreach (var request in requests)
            {
                var response = new PropertySyncItemsResponse();
                switch (type)
                {
                    case SyncEntityTypeEnum.Tenants:
                        var tenant = request as PTenant;
                        if (tenant != null)
                        {
                            response.SiteId = tenant.SiteId;
                            response.Description = tenant.TenantFullName;
                            response.Id = tenant.PTenantId;
                            response.ItemCode = tenant.TenantYCode;
                        }
                        break;

                    case SyncEntityTypeEnum.Landlords:
                        var landlord = request as PLandlord;
                        if (landlord != null)
                        {
                            response.SiteId = landlord.SiteId;
                            response.Description = landlord.LandlordFullname;
                            response.Id = landlord.PLandlordId;
                            response.ItemCode = landlord.LandlordCode;
                        }
                        break;

                    case SyncEntityTypeEnum.Properties:
                        var property = request as PProperty;
                        if (property != null)
                        {
                            response.SiteId = property.SiteId;
                            response.Description =
                                $"{property.AddressLine1}-{property.AddressLine2}-{property.AddressPostcode}";
                            response.Id = property.PPropertyId;
                            response.ItemCode = property.PropertyCode;
                        }
                        break;

                }
                responses.Add(response);
            }

            return responses;
        }
    }

}
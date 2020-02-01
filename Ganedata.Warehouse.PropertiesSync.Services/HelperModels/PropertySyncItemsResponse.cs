using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Warehouse.PropertiesSync.SyncData.Entities;

namespace Ganedata.Warehouse.PropertiesSync.Services.HelperModels
{
    public enum SyncEntityTypeEnum
    {
        Tenants=1,
        Landlords=2,
        Properties=3
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
    public class PropertySyncFinalResponse
    {
        public List<PropertySyncItemsResponse> Responses { get; set; }
        public List<PTenant> SyncedTenants { get; set; }
        public List<PLandlord> SyncedLandlords { get; set; }
        public List<PProperty> SyncedProperties { get; set; }
    }

}

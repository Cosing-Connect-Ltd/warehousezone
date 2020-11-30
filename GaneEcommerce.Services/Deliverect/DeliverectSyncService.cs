using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ImportModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public class DeliverectSyncService : IDeliverectSyncService
    {
        private readonly IApplicationContext _context;
        private readonly string _deliverectApiUrl;
        private string _channelName;

        public DeliverectSyncService(IApplicationContext context)
        {
            _context = context;
            _deliverectApiUrl = ConfigurationManager.AppSettings["DeliverectApiUrl"];
        }

        private async Task<HttpClient> GetHttpClient()
        {
            using (var client = new HttpClient())
            {
                var tokenRequestUri = new Uri(_deliverectApiUrl + "/oauth/token");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var parameters = new
                {
                    client_id = ConfigurationManager.AppSettings["DeliverectClientId"],
                    client_secret = ConfigurationManager.AppSettings["DeliverectClientSecretKey"],
                    audience = _deliverectApiUrl,
                    grant_type = ConfigurationManager.AppSettings["DeliverectScope"]
                };
                var response = await client.PostAsync(tokenRequestUri, new StringContent(JsonConvert.SerializeObject(parameters), Encoding.UTF8, "application/json"));
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var token = JsonConvert.DeserializeObject<DeliverectApiToken>(response.Content.ReadAsStringAsync().Result);
                    _channelName = token.Scope.Replace("genericChannel:", "");
                    var httpClient = new HttpClient();
                    httpClient.DefaultRequestHeaders.Accept.Clear();
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                    return httpClient;
                }
            }
            return null;
        }

        public async Task SyncChannelLinks()
        {
            using (var client = await GetHttpClient())
            {
                var requestUri = new Uri(_deliverectApiUrl + "/channelLinks");
                var allItemsSynced = false;
                var page = 1;
                while (!allItemsSynced)
                {
                    var response = await client.GetAsync(requestUri + "?page=" + page);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var result = JsonConvert.DeserializeObject<DeliverectChannelLinks>(response.Content.ReadAsStringAsync().Result);
                        var country = _context.GlobalCountries.FirstOrDefault(c => c.CountryCode.ToUpper().IndexOf("GB") != -1) ??
                                      _context.GlobalCountries.FirstOrDefault();

                        foreach (var deliverectChannelLink in result.ChannelLinks.Where(c => c.ChannelSettings?.ChannelLocationId != null))
                        {
                            var changedLocations = _context.TenantWarehouses.Where(w => w.IsDeleted != true && 
                                                                                        w.WarehouseId != deliverectChannelLink.ChannelSettings.ChannelLocationId.Value &&
                                                                                        w.DeliverectChannelLinkId == deliverectChannelLink.Id)
                                                                            .ToList();

                            changedLocations.ForEach(l => {
                                l.DeliverectChannelLinkId = null;
                                l.DeliverectChannelLinkName = null;
                                l.DeliverectChannel = null;
                            });

                            var location = _context.TenantWarehouses.FirstOrDefault(w => w.IsDeleted != true && w.WarehouseId == deliverectChannelLink.ChannelSettings.ChannelLocationId.Value);

                            if (location != null)
                            {
                                location.DeliverectChannelLinkId = deliverectChannelLink.Id;
                                location.DeliverectChannel = deliverectChannelLink.Channel;
                                location.DeliverectChannelLinkName = deliverectChannelLink.Name;
                            }
                        }

                        await _context.SaveChangesAsync();

                        allItemsSynced = (result.Meta.Total / result.Meta.MaxResults) < page;
                        page++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public async Task SyncProducts(int? tenantId, int currentUserId)
        {
            tenantId = tenantId ?? _context.Tenants.First(t => t.IsActive && t.IsDeleted != true).TenantId;
            using (var client = await GetHttpClient())
            {
                var requestUri = new Uri(_deliverectApiUrl + "/products");
                var allItemsSynced = false;
                var page = 1;
                while (!allItemsSynced)
                {
                    var response = await client.GetAsync(requestUri + "?page=" + page);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var deliverectProducts = JsonConvert.DeserializeObject<DeliverectProducts>(response.Content.ReadAsStringAsync().Result);

                        foreach (var deliverectProduct in deliverectProducts.Products.Where(p => p.Type == 1))
                        {
                            SaveProduct(tenantId.Value, currentUserId, deliverectProduct);
                        }

                        await _context.SaveChangesAsync();

                        allItemsSynced = (deliverectProducts.Meta.Total / deliverectProducts.Meta.MaxResults) < page;
                        page++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        private void SaveProduct(int tenantId, int currentUserId, DeliverectProduct deliverectProduct)
        {
            var product = _context.ProductMaster.FirstOrDefault(w => w.IsDeleted != true && w.DeliverectProductId == deliverectProduct.Id);
            if (product == null)
            {
                product = new ProductMaster
                {
                    Name = deliverectProduct.Name,
                    SKUCode = deliverectProduct.PLU,
                    DeliverectPLU = deliverectProduct.PLU,
                    DeliverectProductId = deliverectProduct.Id,
                    DeliverectProductType = deliverectProduct.Type,
                    Description = deliverectProduct.Description,
                    SellPrice = deliverectProduct.Price,
                    TenantId = tenantId,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                    UOMId = 1,
                    Serialisable = false,
                    LotOption = false,
                    LotOptionCodeId = 1,
                    LotProcessTypeCodeId = 1,
                    Height = 0,
                    Width = 0,
                    Depth = 0,
                    Weight = 0,
                    TaxID = 1,
                    WeightGroupId = 1,
                    PercentMargin = 0,
                    ProductType = ProductKitTypeEnum.Simple,
                    IsActive = true,
                    ProdStartDate = DateTime.UtcNow,
                    Discontinued = false,
                    DepartmentId = _context.TenantDepartments.FirstOrDefault().DepartmentId,
                    ProcessByCase = false,
                    ProcessByPallet = false,
                    IsStockItem = false,
                };

                _context.ProductMaster.Add(product);
            }
            else
            {
                product.Name = deliverectProduct.Name;
                product.SKUCode = deliverectProduct.PLU;
                product.DeliverectPLU = deliverectProduct.PLU;
                product.DeliverectProductId = deliverectProduct.Id;
                product.Description = deliverectProduct.Description;
                product.SellPrice = deliverectProduct.Price;
            }
        }

        public async Task<bool> SendOrderToDeliverect(OrdersSync order)
        {
            var productIds = order.OrderDetails.Select(o => o.ProductId).ToList();
            var products = _context.ProductMaster.Where(p => productIds.Contains(p.ProductId)).ToList();

            using (var client = await GetHttpClient())
            {
                var deliverectOrder = new DeliverectOrder { 
                    ChannelLinkId = order.DeliverectChannelLinkId,
                    ChannelOrderId = order.OrderID.ToString(),
                    ChannelOrderDisplayId = order.OrderNumber,
                    OrderType = order.FoodOrderType == FoodOrderTypeEnum.Collection ? 1 : (order.FoodOrderType == FoodOrderTypeEnum.Delivery ? 2 : (int)FoodOrderTypeEnum.EatIn),
                    Channel = "10000",
                    CreatedBy = order.CreatedBy.ToString(),
                    Customer = new DeliverectOrderCustomer { 
                        PhoneNumber = order.MobileNumber,
                        Email = order.CustomEmailRecipient,
                        Name = order.FullName
                    },
                    decimalDigits = 2,
                    DeliveryAddress = new DeliverectOrderDeliveryAddress
                    {
                        Street = order.ShipmentAddressLine1 + 
                                (!string.IsNullOrEmpty(order.ShipmentAddressLine2?.Trim()) ? ", " + order.ShipmentAddressLine2 : string.Empty) +
                                (!string.IsNullOrEmpty(order.ShipmentAddressLine3?.Trim()) ? ", " + order.ShipmentAddressLine3 : string.Empty),
                        City = order.ShipmentAddressTown,
                        PostalCode = order.ShipmentAddressPostcode
                    },
                    DeliveryIsAsap = true,
                    DeliveryCost = 0,
                    DiscountTotal = 0,
                    Courier = order.TransferWarehouseName,
                    NumberOfCustomers = 1,
                    ServiceCharge = 0,
                    Table = string.Empty,
                    OrderIsAlreadyPaid = order.OrderPaid,
                    PickupTime = order.ExpectedDate.Value.ToString("u"),
                    EstimatedPickupTime = order.ExpectedDate.Value.ToString("u"),
                    DeliveryTime = order.ExpectedDate.Value.ToString("u"),
                    Note = order.Note,
                    Payment = new DeliverectOrderPayment
                    {
                        Amount = order.OrderCost ?? order.AmountPaidByAccount ?? 0,
                        Type = GetDeliverectPaymentType(order.AccountPaymentModeId)
                    },
                    Items = order.OrderDetails.Select(o => {
                        var product = products.FirstOrDefault(p => p.ProductId == o.ProductId);
                        if (product == null) return null;

                        return new DeliverectOrderItem
                        {
                            Name = product.Name,
                            Price = o.Price,
                            Quantity = o.Qty,
                            Remark = string.Empty,
                            PLU = product.DeliverectPLU,
                            ProductType = product.DeliverectProductType,
                            SubItems = new List<DeliverectOrderItem>()
                        };
                    }).ToList()
                };

                var requestUri = new Uri($"{_deliverectApiUrl}/{_channelName}/order/{order.DeliverectChannelLinkId}");
                var json = JsonConvert.SerializeObject(deliverectOrder);
                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync(requestUri, stringContent);

                return response.IsSuccessStatusCode;
            }
        }

        private int GetDeliverectPaymentType(int? accountPaymentModeId)
        {
            switch (accountPaymentModeId)
            {
                case 1:
                    return 1;
                case 2:
                    return 3;
                default:
                    return 9;
            }
        }
    }
}
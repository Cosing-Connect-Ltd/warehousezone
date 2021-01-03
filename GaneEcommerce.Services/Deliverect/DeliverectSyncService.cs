using Elmah;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ImportModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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

                            changedLocations.ForEach(l =>
                            {
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

            //delete all non deliverect products
            DeleteProductsExceptDeliverect(currentUserId);

            // Sync with deliverect
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

        public void SaveProduct(int tenantId, int currentUserId, DeliverectProduct deliverectProduct, int? sortOrder = null)
        {
            var product = _context.ProductMaster.FirstOrDefault(w => w.DeliverectProductId == deliverectProduct.Id);
            var productTaxpercent = Convert.ToInt32((deliverectProduct.DeliveryTax / 1000));
            var tax = _context.GlobalTax.Where(x => x.PercentageOfAmount == productTaxpercent).FirstOrDefault();

            if (tax == null)
            {
                tax = new GlobalTax
                {
                    TaxName = "Tax " + productTaxpercent.ToString() + " Percent",
                    CountryID = 1,
                    PercentageOfAmount = productTaxpercent
                };

                _context.GlobalTax.Add(tax);
                _context.SaveChanges();
            }


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
                    SellPrice = deliverectProduct.Price / 100,
                    TenantId = tenantId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
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
                    TaxID = tax.TaxID,
                    EnableTax = true,
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
                    IsDeleted = false,
                    SortOrder = sortOrder ?? 0
                };

                _context.ProductMaster.Add(product);
                _context.SaveChanges();
            }
            else
            {
                product.Name = deliverectProduct.Name;
                product.SKUCode = deliverectProduct.PLU;
                product.DeliverectPLU = deliverectProduct.PLU;
                product.DeliverectProductId = deliverectProduct.Id;
                product.Description = deliverectProduct.Description;
                product.SellPrice = deliverectProduct.Price / 100;
                product.DateUpdated = DateTime.UtcNow;
                product.IsDeleted = false;
                product.TaxID = tax.TaxID;
                product.EnableTax = true;
                product.SortOrder = sortOrder ?? 0;
            }


            //remove previous images
            _context.ProductFiles.RemoveRange(_context.ProductFiles.Where(x => x.ProductId == product.ProductId));
            _context.SaveChanges();

            // save product image
            string path = DownloadImage(deliverectProduct.ImageUrl, product.ProductId);

            if (!string.IsNullOrEmpty(path))
            {
                var productFile = new ProductFiles
                {
                    ProductId = product.ProductId,
                    FilePath = path,
                    TenantId = tenantId,
                    CreatedBy = currentUserId,
                    DateCreated = DateTime.Now,
                    DateUpdated = DateTime.Now
                };

                _context.ProductFiles.Add(productFile);

            }

            // add product tags
            var productTags = _context.ProductTags.Where(u => u.TagName == "Delivery" || u.TagName == "Collection" || u.TagName == "Eat In" && u.IsDeleted != true).ToList();
            if (productTags != null)
            {
                foreach (var item in productTags)
                {
                    var tag = new ProductTagMap();
                    tag.ProductId = product.ProductId;
                    tag.TagId = item.Id;
                    tag.TenantId = tenantId;
                    tag.UpdateCreatedInfo(currentUserId);
                    _context.ProductTagMaps.Add(tag);
                }
            }

            _context.SaveChanges();

        }

        public void SaveCategory(int tenantId, int currentUserId, DeliverectCategory deliverectCategory, int? sortOrder)
        {
            var category = _context.TenantDepartments.FirstOrDefault(w => w.DeliverectCategoryId == deliverectCategory.Id);

            if (category == null)
            {
                category = new TenantDepartments
                {
                    DepartmentName = deliverectCategory.Name,
                    DeliverectCategoryId = deliverectCategory.Id,
                    ImagePath = DownloadImage(deliverectCategory.ImageUrl, category.DepartmentId, true),
                    TenantId = tenantId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    CreatedBy = currentUserId,
                    IsDeleted = false,
                    SortOrder = sortOrder ?? 0
                };

                _context.TenantDepartments.Add(category);
            }
            else
            {
                category.DepartmentName = deliverectCategory.Name;
                category.ImagePath = deliverectCategory.ImageUrl;
                category.DateUpdated = DateTime.UtcNow;
                category.IsDeleted = false;
                category.SortOrder = sortOrder ?? 0;
                category.ImagePath = DownloadImage(deliverectCategory.ImageUrl, category.DepartmentId, true);
            }

            _context.SaveChanges();

            foreach (var product in deliverectCategory.Products)
            {
                UpdateProductCategory(product, category.DepartmentId, 1);
            }

        }

        public void DeleteProductsExceptDeliverect(int currentUserId, List<string> menuProducts = null)
        {
            var products = _context.ProductMaster.Where(w => w.IsDeleted != true && String.IsNullOrEmpty(w.DeliverectProductId));

            if (menuProducts != null)
            {
                products = _context.ProductMaster.Where(w => w.IsDeleted != true && !menuProducts.Any(x => x.Contains(w.DeliverectProductId)));
            }

            foreach (var item in products)
            {
                item.DateUpdated = DateTime.UtcNow;
                item.IsDeleted = true;
                item.UpdatedBy = currentUserId;
            }

            _context.SaveChanges();
        }

        public void DeleteDepartmentsExceptDeliverect(int currentUserId, List<string> menuCategories = null)
        {
            var category = _context.TenantDepartments.Where(w => w.IsDeleted != true && String.IsNullOrEmpty(w.DeliverectCategoryId));

            if (menuCategories != null)
            {
                category = _context.TenantDepartments.Where(w => w.IsDeleted != true && !menuCategories.Any(x => x.Contains(w.DeliverectCategoryId)));
            }

            foreach (var item in category)
            {
                item.DateUpdated = DateTime.UtcNow;
                item.IsDeleted = true;
                item.UpdatedBy = currentUserId;
            }

            _context.SaveChanges();
        }

        public void UpdateProductCategory(string deliverectProductId, int categoryId, int currentUserId)
        {
            var product = _context.ProductMaster.Where(w => w.IsDeleted != true && w.DeliverectProductId == deliverectProductId).FirstOrDefault();

            if (product != null)
            {
                product.DateUpdated = DateTime.UtcNow;
                product.DepartmentId = categoryId;
                product.UpdatedBy = currentUserId;
            }

            _context.SaveChanges();

        }

        public async Task<bool> SendOrderToDeliverect(OrdersSync order)
        {
            var productIds = order.OrderDetails.Select(o => o.ProductId).ToList();
            var products = _context.ProductMaster.Where(p => productIds.Contains(p.ProductId)).ToList();
            var accountUser = _context.AuthUsers.Where(x => x.AccountId == order.AccountID).FirstOrDefault();

            using (var client = await GetHttpClient())
            {
                var deliverectOrder = new DeliverectOrder
                {
                    ChannelLinkId = order.DeliverectChannelLinkId,
                    ChannelOrderId = order.OrderID.ToString(),
                    ChannelOrderDisplayId = order.OrderNumber,
                    OrderType = order.FoodOrderType == FoodOrderTypeEnum.Collection ? 1 : (order.FoodOrderType == FoodOrderTypeEnum.Delivery ? 2 : (int)FoodOrderTypeEnum.EatIn),
                    Channel = "10000",
                    CreatedBy = order.CreatedBy.ToString(),
                    Customer = new DeliverectOrderCustomer
                    {
                        PhoneNumber = accountUser?.UserMobileNumber,
                        Email = accountUser?.UserEmail,
                        Name = accountUser?.DisplayName
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
                    DiscountTotal = order.OrderDiscount * 100,
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
                        Amount = (order.AmountPaidByAccount * 100) ?? 0,
                        Type = GetDeliverectPaymentType(order.AccountPaymentModeId)
                    },
                    Items = order.OrderDetails.Select(o =>
                    {
                        var product = products.FirstOrDefault(p => p.ProductId == o.ProductId);
                        if (product == null) return null;

                        return new DeliverectOrderItem
                        {
                            Name = product.Name,
                            Price = (o.Price * 100) + ((o.TaxAmount / o.Qty) * 100),
                            Quantity = o.Qty,
                            Remark = o.Notes,
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

        public string DownloadImage(string path, int ItemId, bool category = false)
        {
            var value = "";
            string UploadDirectory = @"~/UploadedFiles/Products/";
            string type = "Product";

            if (category == true)
            {
                UploadDirectory = @"~/UploadedFiles/Departments/";
                type = "Departments";
            }

            if (!string.IsNullOrEmpty(path))
            {
                if (RemoteFileExists(path))
                {
                    try
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(UploadDirectory + ItemId.ToString())))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(UploadDirectory + ItemId.ToString()));
                        }

                        string resFileName = HttpContext.Current.Server.MapPath(UploadDirectory + ItemId.ToString() + @"/" + type + ItemId + ".jpg");

                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(path, resFileName);
                        value = (UploadDirectory.Remove(0, 1) + ItemId.ToString() + @"/" + type + ItemId + ".jpg");
                    }
                    catch (Exception ex)
                    {
                        ErrorSignal.FromCurrentContext().Raise(ex);
                    }
                }
            }

            return value;

        }

        private bool RemoteFileExists(string url)
        {
            try
            {
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "HEAD";
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                response.Close();
                return (response.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
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
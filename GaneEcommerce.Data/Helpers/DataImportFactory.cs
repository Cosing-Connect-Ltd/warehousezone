using CsvHelper;
using Elmah;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ImportModels;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace Ganedata.Core.Data.Helpers
{
    public class DataImportFactory
    {
        private static string DefaultContactName { get; set; } = "Helpdesk";

        public List<string> ImportProductsCategoriesAssociations(string importPath, string websiteName, int tenantId, int? userId = null, ApplicationContext dbContext = null)
        {
            if (dbContext == null)
            {
                dbContext = new ApplicationContext();
            }

            var website = dbContext.TenantWebsites.FirstOrDefault(w => w.SiteName == websiteName && w.IsDeleted != true && w.TenantId == tenantId);

            if (website == null)
            {
                return new List<string> { $"Filename should be a valid website name!" };
            }

            var adminUserId = dbContext.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
            var headers = new List<string>();
            List<ProductsCategoriesAssociationsImportModel> associationsData = null;

            try
            {
                using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                    associationsData = csv.GetRecords<ProductsCategoriesAssociationsImportModel>().ToList();
                }
            }
            catch (Exception)
            {
                return new List<string> { $"Incorrect file content!" };
            }

            if (associationsData == null || associationsData.Count() <= 0)
            {
                return new List<string> { $"Empty file, no values to import!" };
            }

            var sortedAssociationsData = SortProductsCategoriesAssociationsData(associationsData);

            var exceptionsList = new List<string>();
            var newlyAddedCategories = new List<string>();
            var associatedCount = 0;
            var previouslyAvailableCount = 0;

            AssociateProductsByCategory(tenantId,
                                        userId,
                                        dbContext,
                                        website.SiteID,
                                        sortedAssociationsData,
                                        ref exceptionsList,
                                        ref newlyAddedCategories,
                                        ref associatedCount,
                                        ref previouslyAvailableCount);

            var websiteAssociationProducts = GetWebsiteAssociationChildProducts(associationsData);

            try
            {
                AssociatProductsAndWebsite(tenantId, userId, website.SiteID, dbContext, websiteAssociationProducts);
            }
            catch (Exception ex)
            {
                exceptionsList.Add($"Unable to associate child products with website. Error message: {ex.Message}");
            }

            var result = new List<string>();

            result.Add($"{associatedCount} Associations imported for \"{website.SiteName}\" website navigastion categories.");
            if (previouslyAvailableCount > 0)
            {
                result.Add($"{previouslyAvailableCount} Associations are already available.");
            }
            if (websiteAssociationProducts.Count() > 0)
            {
                result.Add($"{websiteAssociationProducts.Count()} Child products associatiated to \"{website.SiteName}\" website.");
            }
            if (newlyAddedCategories.Count() > 0)
            {
                result.Add($"{newlyAddedCategories.Count} New Categories Added, please change the categories visibility settings from webdile categories list.");
                result.Add($"New categories list :");
                result.AddRange(newlyAddedCategories);
                result.Add($"___________________________________________________________________________________________");
            }
            if (exceptionsList.Count > 0)
            {
                result.Add($"{exceptionsList.Count} Failures in import process :");
                result.AddRange(exceptionsList);
            }

            return result;
        }

        private List<string> GetWebsiteAssociationChildProducts(List<ProductsCategoriesAssociationsImportModel> associationsData)
        {
            return associationsData.Where(a => string.IsNullOrEmpty(a.ParentHeading?.Trim()) &&
                                               string.IsNullOrEmpty(a.ChildHeading?.Trim()) &&
                                               string.IsNullOrEmpty(a.SubChildHeading?.Trim()) &&
                                               !string.IsNullOrEmpty(a.SkuCode?.Trim()))
                    .Select(a => a.SkuCode)
                    .ToList();
        }

        private static List<SortedProductCategoriesAssociationsImportModel> SortProductsCategoriesAssociationsData(List<ProductsCategoriesAssociationsImportModel> associationsData)
        {
            return associationsData.Where(a => !string.IsNullOrEmpty(a.ParentHeading?.Trim()) && !string.IsNullOrEmpty(a.SkuCode?.Trim()))
                                                         .GroupBy(a => a.ParentHeading)
                                                         .Select(g =>
                                                         {
                                                             return new SortedProductCategoriesAssociationsImportModel
                                                             {
                                                                 CategoryName = g.Key,
                                                                 AssociatedSkuCodes = g.Where(gc => (string.IsNullOrEmpty(gc.ChildHeading?.Trim()) || gc.ChildHeading == g.Key) &&
                                                                                                    (string.IsNullOrEmpty(gc.SubChildHeading?.Trim()) || gc.SubChildHeading == g.Key))
                                                                                       .Select(gc => gc.SkuCode.Trim()).ToList(),
                                                                 Childs = g.Where(gc => !string.IsNullOrEmpty(gc.ChildHeading?.Trim()) && gc.ChildHeading?.ToLower()?.Trim() != g.Key?.ToLower()?.Trim())
                                                                           .GroupBy(a => a.ChildHeading)
                                                                           .Select(gc =>
                                                                           {
                                                                               return new SortedProductCategoriesAssociationsImportModel
                                                                               {
                                                                                   CategoryName = gc.Key,
                                                                                   AssociatedSkuCodes = gc.Where(gsc => (string.IsNullOrEmpty(gsc.SubChildHeading?.Trim()) || gsc.SubChildHeading == gc.Key))
                                                                                                         .Select(gsc => gsc.SkuCode.Trim()).ToList(),
                                                                                   Childs = gc.Where(gdc => !string.IsNullOrEmpty(gdc.SubChildHeading?.Trim()) && gdc.SubChildHeading?.ToLower()?.Trim() != gc.Key?.ToLower()?.Trim())
                                                                                              .GroupBy(a => a.SubChildHeading)
                                                                                              .Select(gdc =>
                                                                                              {
                                                                                                  return new SortedProductCategoriesAssociationsImportModel
                                                                                                  {
                                                                                                      CategoryName = gdc.Key,
                                                                                                      AssociatedSkuCodes = gdc.Select(gsc => gsc.SkuCode.Trim()).ToList()
                                                                                                  };
                                                                                              }).ToList()
                                                                               };
                                                                           }).ToList()
                                                             };
                                                         }).ToList();
        }

        private static void AssociateProductsByCategory(int tenantId,
                                                        int? userId,
                                                        ApplicationContext dbContext,
                                                        int siteId,
                                                        IEnumerable<SortedProductCategoriesAssociationsImportModel> categoriesData,
                                                        ref List<string> exceptionsList,
                                                        ref List<string> newlyAddedCategories,
                                                        ref int associatedCount,
                                                        ref int previouslyAvailableCount,
                                                        WebsiteNavigation parentCategory = null)
        {
            var websiteCategories = dbContext.WebsiteNavigations.Where(n => n.Type == WebsiteNavigationType.Category && n.IsDeleted != true && n.TenantId == tenantId && n.SiteID == siteId).ToList();
            var websiteProducts = dbContext.ProductsWebsitesMap.Where(n => n.IsDeleted != true && n.TenantId == tenantId && n.SiteID == siteId).Select(p => p.ProductMaster.SKUCode).ToList();

            foreach (var categoryData in categoriesData)
            {
                var parentCategoryId = parentCategory?.Id;
                var category = websiteCategories.FirstOrDefault(c => c.Name.ToLower().Trim() == categoryData.CategoryName.ToLower().Trim() && c.ParentId == parentCategoryId);

                if (category == null)
                {
                    newlyAddedCategories.Add($"<span style=\"color: green\">\"{categoryData.CategoryName}\" Category {(!string.IsNullOrEmpty(parentCategory?.Name) ? $"with \"{parentCategory.Name}\" parent category" : string.Empty)}</span>");

                    dbContext.WebsiteNavigations.Add(new WebsiteNavigation
                    {
                        CreatedBy = userId,
                        DateCreated = DateTime.Now,
                        IsActive = true,
                        Name = categoryData.CategoryName,
                        ParentId = parentCategoryId,
                        ShowInNavigation = parentCategory?.ShowInNavigation ?? false,
                        SiteID = siteId,
                        TenantId = tenantId,
                        Type = WebsiteNavigationType.Category,
                        SortOrder = 0
                    });

                    dbContext.SaveChanges();
                    category = dbContext.WebsiteNavigations.FirstOrDefault(c => c.Name == categoryData.CategoryName && c.ParentId == parentCategoryId && c.SiteID == siteId && c.IsActive && c.IsDeleted != true);
                }

                try
                {
                    AssociatProductsAndWebsite(tenantId, userId, siteId, dbContext, categoryData.AssociatedSkuCodes);
                }
                catch (Exception ex)
                {
                    exceptionsList.Add($"Unable to associate products related by \"{categoryData.CategoryName}\" category with website. Error message: {ex.Message}");
                    continue;
                }

                try
                {
                    var newlyAssociatedProductsCount = AssociateProductsAndNavigationCategories(tenantId, userId, dbContext, category, categoryData.AssociatedSkuCodes, siteId);
                    associatedCount += newlyAssociatedProductsCount;
                    previouslyAvailableCount = categoryData.AssociatedSkuCodes.Count() - newlyAssociatedProductsCount;
                }
                catch (Exception ex)
                {
                    exceptionsList.Add($"Unable to associate products by \"{categoryData.CategoryName}\" category. Error message: {ex.Message}");
                    continue;
                }

                if (categoryData.Childs != null && categoryData.Childs.Count() > 0)
                {
                    AssociateProductsByCategory(tenantId,
                                                userId,
                                                dbContext,
                                                siteId,
                                                categoryData.Childs,
                                                ref exceptionsList,
                                                ref newlyAddedCategories,
                                                ref associatedCount,
                                                ref previouslyAvailableCount,
                                                category);
                }
            }
        }

        private static int AssociateProductsAndNavigationCategories(int tenantId, int? userId, ApplicationContext dbContext, WebsiteNavigation category, List<string> associatedSkuCodes, int siteId)
        {
            if (associatedSkuCodes == null || associatedSkuCodes.Count() <= 0)
            {
                return 0;
            }

            var unassociatedProductsWebsitesMapIds = dbContext.ProductsWebsitesMap.Where(p => associatedSkuCodes.Contains(p.ProductMaster.SKUCode) &&
                                                                                           p.IsDeleted != true &&
                                                                                           p.SiteID == siteId &&
                                                                                           p.TenantId == tenantId)
                                                                                .Select(p => p.Id)
                                                                                .ToList()
                                                                                .Where(id => (category.ProductsNavigationMap?.Any(pa => pa.IsDeleted != true &&
                                                                                                                                 pa.ProductWebsiteMapId == id &&
                                                                                                                                 pa.NavigationId == category.Id &&
                                                                                                                                 pa.TenantId == tenantId) != true))
                                                                                .ToList();


            if (unassociatedProductsWebsitesMapIds != null && unassociatedProductsWebsitesMapIds.Count() > 0)
            {
                dbContext.ProductsNavigationMaps.AddRange(unassociatedProductsWebsitesMapIds.ToList()
                                                                                          .Select(id => new ProductsNavigationMap
                                                                                          {
                                                                                              CreatedBy = userId,
                                                                                              DateCreated = DateTime.Now,
                                                                                              IsActive = true,
                                                                                              NavigationId = category.Id,
                                                                                              ProductWebsiteMapId = id,
                                                                                              TenantId = tenantId
                                                                                          })
                                                                                            .ToList());

                dbContext.SaveChanges();
            }

            return unassociatedProductsWebsitesMapIds.Count();
        }

        private static void AssociatProductsAndWebsite(int tenantId, int? userId, int siteId, ApplicationContext dbContext, List<string> associatedSkuCodes)
        {
            if (associatedSkuCodes == null || associatedSkuCodes.Count() <= 0)
            {
                return;
            }

            var websiteProducts = dbContext.ProductsWebsitesMap.Where(n => n.IsDeleted != true && n.TenantId == tenantId && n.SiteID == siteId).Select(p => p.ProductMaster.SKUCode).ToList();
            var newAssociationSkuCodes = associatedSkuCodes.Where(s => !websiteProducts.Contains(s)).ToList();
            if (newAssociationSkuCodes != null && newAssociationSkuCodes.Count() > 0)
            {
                var newProductsWebsitesMaps = dbContext.ProductMaster.Where(p => newAssociationSkuCodes.Contains(p.SKUCode) && p.IsDeleted != true && p.TenantId == tenantId)
                                                                     .ToList()
                                                                     .Select(p => new ProductsWebsitesMap
                                                                     {
                                                                         IsActive = true,
                                                                         ProductId = p.ProductId,
                                                                         SiteID = siteId,
                                                                         DateCreated = DateTime.Now,
                                                                         CreatedBy = userId,
                                                                         SortOrder = 0,
                                                                         TenantId = tenantId
                                                                     })
                                                                     .ToList();
                dbContext.ProductsWebsitesMap.AddRange(newProductsWebsitesMaps);
                dbContext.SaveChanges();
            }
        }

        public List<string> ImportParentChildProductsAssociations(string importPath, int tenantId, int? userId = null, ApplicationContext dbContext = null)
        {
            if (dbContext == null)
            {
                dbContext = new ApplicationContext();
            }
            var adminUserId = dbContext.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
            var headers = new List<string>();
            List<ParentChildProductsAssociationsImportModel> parentChildProductsAssociationsData = null;


            using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
            {
                try
                {
                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                    parentChildProductsAssociationsData = csv.GetRecords<ParentChildProductsAssociationsImportModel>().ToList();
                }
                catch (Exception)
                {
                    return new List<string> { $"Incorrect file content!" };
                }
            }

            if (parentChildProductsAssociationsData == null || parentChildProductsAssociationsData.Count() <= 0)
            {
                return new List<string> { $"Empty file, no values to import!" };
            }

            var groupedParentChildProductsAssociationsData = parentChildProductsAssociationsData.Where(p => !string.IsNullOrEmpty(p.ParentSkuCode) && !string.IsNullOrEmpty(p.ChildSkuCode) && !string.IsNullOrEmpty(p.ParentSkuCode))
                                                                                                .GroupBy(p => p.AssociationType)
                                                                                                .Select(pg =>
                                                                                                {
                                                                                                    return new
                                                                                                    {
                                                                                                        Name = pg.Key,
                                                                                                        Products = pg.GroupBy(p => p.ParentSkuCode)
                                                                                                                     .Select(pc => new { SkuCode = pc.Key, ChildSkuCodes = pc.Select(pcd => pcd.ChildSkuCode).ToList() })
                                                                                                    };
                                                                                                });

            var resultList = new List<string>();
            var associatedCount = 0;
            var previouslyAvailableCount = 0;

            foreach (var productKitTypeData in groupedParentChildProductsAssociationsData)
            {
                try
                {
                    var productKitTypeId = GetOrAddProductKitType(productKitTypeData.Name, dbContext, userId, tenantId);

                    foreach (var parentProductData in productKitTypeData.Products)
                    {
                        try
                        {
                            var parentProduct = dbContext.ProductMaster.FirstOrDefault(p => p.SKUCode == parentProductData.SkuCode && p.IsDeleted != true);

                            if (parentProduct == null)
                            {
                                resultList.Add($"Parent product does not exist with SkuCode : \"{parentProductData.SkuCode}\"");
                                continue;
                            }

                            foreach (var childSkuCode in parentProductData.ChildSkuCodes)
                            {

                                var childProductId = dbContext.ProductMaster.FirstOrDefault(p => p.SKUCode == childSkuCode && p.IsDeleted != true)?.ProductId;

                                if (childProductId == null)
                                {
                                    resultList.Add($"Child product does not exist with SkuCode : \"{childSkuCode}\"");
                                    continue;
                                }

                                if (dbContext.ProductKitMaps.Any(a => a.ProductId == parentProduct.ProductId &&
                                                                      a.KitProductId == childProductId &&
                                                                      a.ProductKitTypeId == productKitTypeId &&
                                                                      a.IsDeleted != true &&
                                                                      a.IsActive &&
                                                                      a.ProductKitType == parentProduct.ProductType))
                                {
                                    previouslyAvailableCount++;
                                    continue;
                                }

                                try
                                {
                                    AddProductKitMap(parentProduct.ProductId, childProductId.Value, parentProduct.ProductType, productKitTypeId, dbContext, userId, tenantId);
                                    associatedCount++;
                                }
                                catch (Exception ex)
                                {
                                    resultList.Add($"Failed to associate parent product \"{parentProductData.SkuCode}\" and child product \"{childSkuCode}\". Exception message : {ex.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            resultList.Add($"Failed to get products details, SkuCode: \"{parentProductData.SkuCode}\". Exception message : {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultList.Add($"Failed to get/create ProductKitType : \"{productKitTypeData.Name}\". Exception message : {ex.Message}");
                }

                dbContext.SaveChanges();
            }

            if (resultList.Count > 0)
            {
                resultList.Insert(0, $"{resultList.Count} Product/Child associations failed to import. List of failures :");
            }
            if (previouslyAvailableCount > 0)
            {
                resultList.Insert(0, $"{previouslyAvailableCount} Product/Child associations are already available.");
            }

            resultList.Insert(0, $"{associatedCount} Product/Child Associations imported.");

            return resultList;
        }

        private bool AddProductKitMap(int productId, int childProductId, ProductKitTypeEnum productType, int productKitTypeId, ApplicationContext dbContext, int? userId, int tenantId)
        {
            var productKitMap = dbContext.ProductKitMaps.FirstOrDefault(a => a.ProductId == productId && a.KitProductId == childProductId && a.ProductKitTypeId == productKitTypeId && a.IsDeleted != true);
            if (productKitMap != null)
            {
                productKitMap.IsActive = true;
                productKitMap.ProductKitType = productType;
            }
            else
            {
                dbContext.ProductKitMaps.Add(new ProductKitMap
                {
                    IsActive = true,
                    ProductId = productId,
                    KitProductId = childProductId,
                    ProductKitType = productType,
                    ProductKitTypeId = productKitTypeId,
                    CreatedBy = userId,
                    TenantId = tenantId,
                    DateCreated = DateTime.Now
                });
            }

            return true;
        }

        private int GetOrAddProductKitType(string associationType, ApplicationContext dbContext, int? userId, int tenantId)
        {
            var productKitType = dbContext.ProductKitTypes.FirstOrDefault(a => a.Name.Equals(associationType, StringComparison.InvariantCultureIgnoreCase) && a.IsDeleted != true);

            if (productKitType == null)
            {
                productKitType = dbContext.ProductKitTypes.Add(new ProductKitType()
                {
                    CreatedBy = userId ?? 0,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    Name = associationType,
                    TenentId = tenantId,
                    UpdatedBy = userId,
                    IsActive = true,
                    SortOrder = dbContext.ProductKitTypes.Count() + 1
                });
                dbContext.SaveChanges();
            }

            return productKitType.Id;
        }

        public List<string> ImportProductsAttributes(string importPath, int tenantId, int? userId = null, ApplicationContext dbContext = null)
        {
            if (dbContext == null)
            {
                dbContext = new ApplicationContext();
            }
            var adminUserId = dbContext.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
            var headers = new List<string>();
            List<ProductAttributeImportModel> productAttributesData = null;


            using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
            {
                try
                {
                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                    productAttributesData = csv.GetRecords<ProductAttributeImportModel>().ToList();
                }
                catch (Exception)
                {
                    return new List<string> { $"Incorrect file content!" };
                }
            }

            if (productAttributesData == null || productAttributesData.Count() <= 0)
            {
                return new List<string> { $"Empty file, no values to import!" };
            }

            var sortedProductAttributesData = productAttributesData.GroupBy(p => p.Attribute)
                                                                   .Select(g =>
                                                                    {
                                                                        return new
                                                                        {
                                                                            Name = g.Key,
                                                                            AttriButeValues = g.GroupBy(ga => ga.AttributeValue)
                                                                                               .Where(f => !string.IsNullOrEmpty(f.Key?.Trim()) && g.Any())
                                                                                               .Select(gad =>
                                                                                               {
                                                                                                   return new
                                                                                                   {
                                                                                                       Value = gad.Key,
                                                                                                       ProductSkuCodes = gad.Select(a => a.SkuCode).Distinct().OrderBy(s => s)
                                                                                                   };
                                                                                               })
                                                                        };
                                                                    })
                                                                   .Where(at => !string.IsNullOrEmpty(at.Name?.Trim()) &&
                                                                                at.AttriButeValues.Any(a => a.ProductSkuCodes.Any()));

            var resultList = new List<string>();
            var associatedCount = 0;
            var previouslyAvailableCount = 0;

            foreach (var attributeData in sortedProductAttributesData)
            {
                ProductAttributes attribute = null;
                try
                {
                    attribute = GetOrAddAttribute(attributeData.Name, dbContext);

                    foreach (var attributeValueData in attributeData.AttriButeValues)
                    {
                        ProductAttributeValues attributeValue = null;
                        try
                        {
                            attributeValue = GetOrAddAttributeValue(attribute, attributeValueData.Value, dbContext, userId, tenantId);

                            foreach (var skuCode in attributeValueData.ProductSkuCodes)
                            {
                                if (dbContext.ProductAttributeValuesMap.Any(a => a.AttributeValueId == attributeValue.AttributeValueId && a.ProductMaster.SKUCode == skuCode && a.IsDeleted != true))
                                {
                                    previouslyAvailableCount++;
                                    continue;
                                }

                                var productQuery = dbContext.ProductMaster.Where(p => p.IsDeleted != true && p.SKUCode == skuCode && p.TenantId == tenantId);

                                if (!productQuery.Any())
                                {
                                    resultList.Add($"Related product not found for SkuCode : \"{skuCode}\"");
                                    continue;
                                }

                                try
                                {
                                    AssociateProductAttribute(attributeValue.AttributeValueId, productQuery.First().ProductId, dbContext, userId, tenantId);
                                    associatedCount++;
                                }
                                catch (Exception ex)
                                {
                                    resultList.Add($"Failed to associate attribute value \"{attributeValueData.Value}\" and product : \"{skuCode}\". Exception message : {ex.Message}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            resultList.Add($"Failed to get/create attribute value : \"{attributeValueData.Value}\". Exception message : {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    resultList.Add($"Failed to get/create attribute : \"{attributeData.Name}\". Exception message : {ex.Message}");
                }

                dbContext.SaveChanges();
            }

            if (resultList.Count > 0)
            {
                resultList.Insert(0, $"{resultList.Count} Products attribute associations failed to import. List of failures :");
            }
            if (previouslyAvailableCount > 0)
            {
                resultList.Insert(0, $"{previouslyAvailableCount} Products attribute associations are already available.");
            }

            resultList.Insert(0, $"{associatedCount} Products attributes imported.");

            return resultList;
        }

        private bool AssociateProductAttribute(int attributeValueId, int productId, ApplicationContext dbContext, int? userId, int tenantId)
        {
            dbContext.ProductAttributeValuesMap.Add(new ProductAttributeValuesMap
            {
                AttributeValueId = attributeValueId,
                ProductId = productId,
                CreatedBy = userId,
                TenantId = tenantId,
                DateCreated = DateTime.Now
            });

            return true;
        }

        private ProductAttributeValues GetOrAddAttributeValue(ProductAttributes attribute, string value, ApplicationContext dbContext, int? userId, int tenantId)
        {
            var attributeValue = dbContext.ProductAttributeValues.FirstOrDefault(a => a.AttributeId == attribute.AttributeId && a.Value == value && a.IsDeleted != true);

            if (attributeValue == null)
            {
                attributeValue = dbContext.ProductAttributeValues.Add(new ProductAttributeValues
                {
                    SortOrder = 0,
                    DateCreated = DateTime.Now,
                    CreatedBy = userId,
                    AttributeId = attribute.AttributeId,
                    Color = attribute.IsColorTyped ? value : string.Empty,
                    Value = value,
                    TenantId = tenantId
                });
                dbContext.SaveChanges();
            }
            return attributeValue;
        }

        private static ProductAttributes GetOrAddAttribute(string name, ApplicationContext dbContext)
        {
            var attribute = dbContext.ProductAttributes.FirstOrDefault(a => a.AttributeName == name && a.IsDeleted != true);

            if (attribute == null)
            {
                attribute = dbContext.ProductAttributes.Add(new ProductAttributes
                {
                    AttributeName = name,
                    SortOrder = 0,
                    IsColorTyped = name.ToLower().Contains("colour") || name.ToLower().Contains("color"),
                });
                dbContext.SaveChanges();
            }
            return attribute;
        }

        public string ImportSupplierAccounts(string importPath, int tenantId, ApplicationContext context = null, int? userId = null, bool withMarketInfo = false)
        {
            if (context == null)
            {
                context = new ApplicationContext();
            }
            var addedSuppliers = 0;
            var updatedSuppliers = 0;
            int counter = 0;
            var currentTenant = context.Tenants.First(m => m.TenantId == tenantId);
            try
            {
                var adminUserId = context.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
                var lineNumber = 0;
                string recorednotmatched = "";
                int count = 0;
                List<string> headers = new List<string>();
                List<object> TotalRecored = new List<object>();
                using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.Read();
                        csv.ReadHeader();
                        headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                        TotalRecored = csv.GetRecords<object>().ToList();

                    }
                    catch (Exception)
                    {
                        return $"File headers mismatch! Please add required headers";
                    }
                }

                if (headers.Count >= 19 && headers.Count <= 21)
                {

                }
                else
                {
                    return $"File headers mismatch! Please add required headers";
                }

                if (!headers.Contains("account code") || !headers.Contains("account name") || !headers.Contains("account type") ||
                    !headers.Contains("invoice email") || !headers.Contains("invoice email name") || !headers.Contains("purchase email") || !headers.Contains("purchase email name") || !headers.Contains("phone") || !headers.Contains("fax number") ||
                    !headers.Contains("billing address 1") || !headers.Contains("billing address 2") || !headers.Contains("billing address 3") || !headers.Contains("billing address 4") || !headers.Contains("billing postcode") ||
                    !headers.Contains("shipping address 1") || !headers.Contains("shipping address 2") || !headers.Contains("shipping address 3") || !headers.Contains("shipping address 4") || !headers.Contains("shipping postcode"))
                {
                    return $"File headers mismatch! Please add required headers";
                }
                if (withMarketInfo)
                {
                    if (!headers.Contains("market name") || !headers.Contains("visit frequency"))
                    {
                        return $"File headers mismatch! Please add required headers";
                    }
                }

                if (TotalRecored.Count <= 0)
                {
                    return $"The file is Empty";
                }

                using (var fs = File.OpenRead(importPath))
                using (var reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        counter++;
                        var line = reader.ReadLine();
                        if (counter == 1877)
                        {
                            line.ToString();
                        }
                        lineNumber++;
                        if (line == null || lineNumber == 1)
                        {
                            continue;
                        }
                        var values = GetCsvLineContents(line);
                        if (0 > values.Length || string.IsNullOrEmpty(values[0]) || values[0].Trim().ToUpper() == "NULL")
                        {

                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : account code not found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;
                        }

                        if (1 > values.Length || string.IsNullOrEmpty(values[1]) || values[1].Trim().ToUpper() == "NULL")
                        {
                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : account name not found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;
                        }
                        if (2 > values.Length || string.IsNullOrEmpty(values[2]) || values[2].Trim().ToUpper() == "NULL")
                        {
                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : account type not found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;

                        }
                        if (withMarketInfo)
                        {
                            if (10 > values.Length || string.IsNullOrEmpty(values[10]))
                            {
                                if (count >= 50) { return recorednotmatched; }
                                recorednotmatched += "Import Failed :market name not found on line :@ " + lineNumber + "<br/> ";
                                count++;
                                continue;

                            }

                        }

                        var countryId = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0;
                        if (countryId <= 0)
                        {
                            if (count >= 50) { return recorednotmatched; }
                            return "Import Failed : No country found against this user or tenant <br/>";
                        }

                        var CurrencyID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId)?.CurrencyID;
                        if (CurrencyID == null)
                        {
                            return "Import Failed : No Currency found against this user or tenant <br/>";

                        }
                        var PriceGroupID = context.TenantPriceGroups.FirstOrDefault(m => m.TenantId == tenantId)?.PriceGroupID;

                        if (PriceGroupID == null)
                        {
                            return "Import Failed : No Price group found against this user or tenant <br/>";

                        }
                    }
                }

                if (!string.IsNullOrEmpty(recorednotmatched))
                {
                    return recorednotmatched;
                }
                else
                {

                    using (var fs = File.OpenRead(importPath))
                    using (var reader = new StreamReader(fs))
                    {
                        lineNumber = 0;
                        counter = 0;

                        while (!reader.EndOfStream)
                        {
                            counter++;
                            var accountCode = "";
                            var accountName = "";
                            var billingAddress1 = "";
                            var billingAddress2 = "";
                            var billingAddress3 = "";
                            var billingAddress4 = "";
                            var billingPostcode = "";
                            var shippingAddress1 = "";
                            var shippingAddress2 = "";
                            var shippingAddress3 = "";
                            var shippingAddress4 = "";
                            var shippingPostcode = "";
                            var invoiceEmail = "";
                            var invoiceEmailName = "";
                            var purchaseEmail = "";
                            var purchaseEmailName = "";
                            var phone = "";
                            var marketName = "";

                            var line = reader.ReadLine();
                            lineNumber++;
                            var values = GetCsvLineContents(line);
                            if (lineNumber == 1)
                            {
                                continue;
                            }
                            accountCode = values[0];
                            var existingAccount = context.Account.FirstOrDefault(m => m.AccountCode == accountCode);
                            var addRecord = false;
                            if (existingAccount == null)
                            {
                                addedSuppliers++;
                                addRecord = true;
                                existingAccount = new Account();
                            }
                            else
                            {
                                updatedSuppliers++;
                            }

                            if (0 < values.Length)
                            {
                                existingAccount.AccountCode = values[0];
                            }
                            if (1 < values.Length)
                            {

                                accountName = existingAccount.CompanyName = values[1];
                            }

                            if (2 < values.Length)
                            {
                                if (values[2] == "0")
                                {
                                    //FOR ALL
                                    existingAccount.AccountTypeCustomer = true;
                                    existingAccount.AccountTypeEndUser = true;
                                    existingAccount.AccountTypeSupplier = true;
                                }
                                else if (values[2] == "1")
                                {
                                    //FOR CUSTOMER
                                    existingAccount.AccountTypeCustomer = true;

                                    existingAccount.AccountTypeEndUser = false;
                                    existingAccount.AccountTypeSupplier = false;
                                }
                                else if (values[2] == "2")
                                {
                                    //FOR SUPPLIER
                                    existingAccount.AccountTypeSupplier = true;

                                    existingAccount.AccountTypeEndUser = false;
                                    existingAccount.AccountTypeCustomer = false;
                                }
                                else if (values[2] == "3")
                                {
                                    //FOR ENDUSER
                                    existingAccount.AccountTypeEndUser = true;

                                    existingAccount.AccountTypeCustomer = false;
                                    existingAccount.AccountTypeSupplier = false;
                                }
                            }

                            if (3 < values.Length)
                            {
                                //INVOICE EMAIL
                                existingAccount.AccountEmail = (string.IsNullOrEmpty(values[3]) || values[3].IndexOf('@') < 1) ? null : values[3];
                                invoiceEmail = existingAccount.AccountEmail;
                            }

                            if (4 < values.Length)
                            {
                                //INVOICE EMAIL NAME
                                invoiceEmailName = values[4];
                                if (string.IsNullOrEmpty(invoiceEmailName))
                                {
                                    invoiceEmailName = "Invoice";
                                }
                            }

                            if (5 < values.Length)
                            {
                                //PURCHASE EMAIL
                                existingAccount.AccountEmail = (string.IsNullOrEmpty(values[5]) || values[5].IndexOf('@') < 1) ? null : values[5];
                                purchaseEmail = existingAccount.AccountEmail;
                            }

                            if (6 < values.Length)
                            {
                                //PURCHASE EMAIL NAME
                                purchaseEmailName = values[6];
                                if (string.IsNullOrEmpty(purchaseEmailName))
                                {
                                    purchaseEmailName = "Purchase";
                                }
                            }

                            if (7 < values.Length)
                            {
                                //PHONE
                                phone = existingAccount.Telephone = values[7];
                            }

                            if (8 <= values.Length)
                            {
                                //FAX NUMBER
                                existingAccount.Fax = values[8];
                            }

                            if (9 < values.Length)
                            {
                                //BILLING ADDRESS 1
                                billingAddress1 = values[9];
                            }

                            if (10 < values.Length)
                            {
                                //BILLING ADDRESS 2
                                billingAddress2 = values[10];
                            }

                            if (11 < values.Length)
                            {
                                //BILLING ADDRESS 3
                                billingAddress3 = values[11];
                            }

                            if (12 < values.Length)
                            {
                                //BILLING ADDRESS 4
                                billingAddress4 = values[12];
                            }

                            if (13 < values.Length)
                            {
                                //BILLING POSTCODE
                                billingPostcode = IsValidUkPostcode(values[13]) ? values[13] : "";
                            }

                            if (14 < values.Length)
                            {
                                //SHIPPING ADDRESS 1
                                shippingAddress1 = values[14];
                            }

                            if (15 < values.Length)
                            {
                                //SHIPPING ADDRESS 2
                                shippingAddress2 = values[15];
                            }

                            if (16 < values.Length)
                            {
                                //SHIPPING ADDRESS 3
                                shippingAddress3 = values[16];
                            }

                            if (17 < values.Length)
                            {
                                //SHIPPING ADDRESS 4
                                shippingAddress4 = values[17];
                            }

                            if (18 < values.Length)
                            {
                                //SHIPPING POSTCODE
                                shippingPostcode = IsValidUkPostcode(values[18]) ? values[18] : "";
                            }

                            existingAccount.AccountStatusID = AccountStatusEnum.Active;
                            existingAccount.DateCreated = DateTime.UtcNow;

                            existingAccount.CreatedBy = userId ?? adminUserId;
                            existingAccount.TaxID = existingAccount.TaxID > 0 ? existingAccount.TaxID : context.GlobalTax.First(m => m.TaxName.Contains("Standard")).TaxID;
                            if (existingAccount.IsDeleted == true)
                            {
                                existingAccount.IsDeleted = true;
                            }

                            existingAccount.TenantId = tenantId;
                            existingAccount.CountryID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0;
                            existingAccount.CurrencyID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CurrencyID;
                            existingAccount.PriceGroupID = context.TenantPriceGroups.FirstOrDefault(m => m.TenantId == tenantId).PriceGroupID;
                            existingAccount.OwnerUserId = userId ?? adminUserId;

                            if (string.IsNullOrEmpty(accountName))
                            {
                                accountName = DefaultContactName;
                            }

                            var currentShiipingAddress = new AccountAddresses()
                            {
                                Name = accountName,
                                PostCode = shippingPostcode,
                                AddressLine1 = shippingAddress1,
                                AddressLine2 = shippingAddress2,
                                AddressLine3 = shippingAddress3,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true,
                                AddTypeShipping = true,
                                CountryID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0

                            };

                            var currentBillingAddress = new AccountAddresses()
                            {
                                Name = accountName,
                                PostCode = billingPostcode,
                                AddressLine1 = billingAddress1,
                                AddressLine2 = billingAddress2,
                                AddressLine3 = billingAddress3,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true,
                                AddTypeBilling = true,
                                CountryID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0

                            };

                            var invoiceCurrentContact = new AccountContacts()
                            {
                                ContactName = invoiceEmailName,
                                ConTypeInvoices = true,
                                ContactEmail = invoiceEmail,
                                TenantContactPhone = phone,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true
                            };

                            var purchaseCurrentContact = new AccountContacts()
                            {
                                ContactName = purchaseEmailName,
                                ConTypePurchasing = true,
                                ContactEmail = purchaseEmail,
                                TenantContactPhone = phone,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true
                            };

                            if (!addRecord)
                            {
                                line.ToString();

                                if (currentShiipingAddress != null && !String.IsNullOrWhiteSpace(currentShiipingAddress.AddressLine1))
                                {
                                    var shippingAddress = existingAccount.AccountAddresses.FirstOrDefault(m => m.Name == currentShiipingAddress.Name && m.AddressLine1 == currentShiipingAddress.AddressLine1 && m.AddTypeShipping == true);

                                    if (shippingAddress == null)
                                    {
                                        var testAddress = existingAccount.AccountAddresses;
                                        line.ToString();
                                        existingAccount.AccountAddresses.Add(currentShiipingAddress);
                                    }
                                    else
                                    {
                                        shippingAddress.PostCode = currentShiipingAddress.PostCode;
                                        shippingAddress.AddressLine1 = currentShiipingAddress.AddressLine1;
                                        shippingAddress.AddressLine2 = currentShiipingAddress.AddressLine2;
                                        shippingAddress.AddressLine3 = currentShiipingAddress.AddressLine3;
                                        shippingAddress.DateUpdated = DateTime.UtcNow;
                                        shippingAddress.AddTypeShipping = true;
                                        context.Entry(shippingAddress).State = EntityState.Modified;
                                    }

                                }

                                if (currentBillingAddress != null && !String.IsNullOrWhiteSpace(currentBillingAddress.AddressLine1))
                                {
                                    var billingAddress = existingAccount.AccountAddresses.FirstOrDefault(m => m.Name == currentBillingAddress.Name && m.AddressLine1 == currentBillingAddress.AddressLine1 && m.AddTypeBilling == true);

                                    if (billingAddress == null)
                                    {
                                        var testAddress = existingAccount.AccountAddresses;
                                        line.ToString();
                                        existingAccount.AccountAddresses.Add(currentBillingAddress);
                                    }
                                    else
                                    {
                                        billingAddress.PostCode = currentBillingAddress.PostCode;
                                        billingAddress.AddressLine1 = currentBillingAddress.AddressLine1;
                                        billingAddress.AddressLine2 = currentBillingAddress.AddressLine2;
                                        billingAddress.AddressLine3 = currentBillingAddress.AddressLine3;
                                        billingAddress.DateUpdated = DateTime.UtcNow;
                                        billingAddress.AddTypeBilling = true;
                                        context.Entry(billingAddress).State = EntityState.Modified;
                                    }

                                }

                                if (invoiceCurrentContact != null)
                                {
                                    var existingContact = existingAccount.AccountContacts.FirstOrDefault(m => m.ContactEmail == invoiceEmail);
                                    if (existingContact == null)
                                    {
                                        existingAccount.AccountContacts.Add(invoiceCurrentContact);
                                    }
                                    else
                                    {
                                        existingContact.ContactEmail = invoiceCurrentContact.ContactEmail;
                                        existingContact.ContactName = invoiceCurrentContact.ContactName;
                                        existingContact.TenantContactPhone = invoiceCurrentContact.TenantContactPhone;
                                        existingContact.DateUpdated = DateTime.UtcNow;
                                        context.Entry(existingContact).State = EntityState.Modified;
                                    }
                                }

                                if (purchaseCurrentContact != null)
                                {
                                    var existingContact = existingAccount.AccountContacts.FirstOrDefault(m => m.ContactEmail == purchaseEmail);
                                    if (existingContact == null)
                                    {
                                        existingAccount.AccountContacts.Add(purchaseCurrentContact);
                                    }
                                    else
                                    {
                                        existingContact.ContactEmail = purchaseCurrentContact.ContactEmail;
                                        existingContact.ContactName = purchaseCurrentContact.ContactName;
                                        existingContact.TenantContactPhone = purchaseCurrentContact.TenantContactPhone;
                                        existingContact.DateUpdated = DateTime.UtcNow;
                                        context.Entry(existingContact).State = EntityState.Modified;
                                    }
                                }

                            }
                            else
                            {
                                if (invoiceCurrentContact != null)
                                {
                                    existingAccount.AccountContacts.Add(invoiceCurrentContact);
                                }

                                if (purchaseCurrentContact != null)
                                {
                                    existingAccount.AccountContacts.Add(purchaseCurrentContact);
                                }

                                if (currentShiipingAddress != null && !String.IsNullOrWhiteSpace(currentShiipingAddress.AddressLine1))
                                {
                                    existingAccount.AccountAddresses.Add(currentShiipingAddress);
                                }

                                if (currentBillingAddress != null && !String.IsNullOrWhiteSpace(currentBillingAddress.AddressLine1))
                                {
                                    existingAccount.AccountAddresses.Add(currentBillingAddress);
                                }

                            }

                            if (addRecord)
                            {
                                context.Account.Add(existingAccount);
                            }
                            else
                            {
                                context.Entry(existingAccount).State = EntityState.Modified;
                            }
                            if (withMarketInfo)
                            {

                                var market = new Market();
                                if (!context.Markets.Any(s => s.Name.Equals(marketName)))
                                {
                                    market = new Market() { Name = marketName, CreatedBy = userId, TenantId = tenantId, DateCreated = DateTime.UtcNow };
                                    context.Entry(market).State = EntityState.Added;

                                }
                                else
                                {
                                    market = context.Markets.FirstOrDefault(m => m.Name == marketName);
                                }

                                if (!context.MarketCustomers.Any(x => x.AccountId == existingAccount.AccountID))
                                {
                                    var mc = new MarketCustomer() { Customer = existingAccount, Market = market, CreatedBy = userId, TenantId = tenantId, DateCreated = DateTime.UtcNow, VisitFrequency = GetVisitFrequencyEnum(values[4]) };
                                    market.MarketCustomers.Add(mc);
                                }
                                else
                                {
                                    string frequency = "";
                                    if (11 < values.Length)
                                    {
                                        frequency = values[11];
                                    }
                                    var marketCustomer = context.MarketCustomers.FirstOrDefault(x => x.AccountId == existingAccount.AccountID && x.MarketId == market.Id);
                                    if (marketCustomer != null)
                                    {
                                        marketCustomer.VisitFrequency = GetVisitFrequencyEnum(frequency);
                                        marketCustomer.DateUpdated = DateTime.UtcNow;
                                        marketCustomer.UpdatedBy = userId;
                                        context.Entry(marketCustomer).State = EntityState.Modified;
                                    }
                                }
                            }

                            if (counter % 200 == 0)
                            {
                                context.SaveChanges();
                                context.Dispose();
                                context = new ApplicationContext();
                            }

                        }

                    }
                }
            }
            catch (Exception ex)
            {
                return "Import Failed : " + ex.Message + " " + counter.ToString();
            }

            return $"Supplier Account details imported successfully. Added { addedSuppliers }, Updated = { updatedSuppliers }";
        }

        private MarketCustomerVisitFrequency GetVisitFrequencyEnum(string frequency)
        {
            switch (frequency)
            {
                case "Weekly": return MarketCustomerVisitFrequency.Weekly;
                case "4 Weeks": return MarketCustomerVisitFrequency.Monthly;
                case "Daily": return MarketCustomerVisitFrequency.Daily;
                default: return MarketCustomerVisitFrequency.Fortnightly;
            }
        }

        public bool IsValidUkPostcode(string postcodeString)
        {
            if (string.IsNullOrEmpty(postcodeString)) return true;

            var pattern = "^(([gG][iI][rR] {0,}0[aA]{2})|((([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y]?[0-9][0-9]?)|(([a-pr-uwyzA-PR-UWYZ][0-9][a-hjkstuwA-HJKSTUW])|([a-pr-uwyzA-PR-UWYZ][a-hk-yA-HK-Y][0-9][abehmnprv-yABEHMNPRV-Y]))) {0,}[0-9][abd-hjlnp-uw-zABD-HJLNP-UW-Z]{2}))$";
            var regex = new Regex(pattern);
            return regex.IsMatch(postcodeString);
        }

        public string[] GetCsvLineContents(string csvLine)
        {
            var fields = new string[] { };
            try
            {

                var parser = new TextFieldParser(new StringReader(csvLine)) { HasFieldsEnclosedInQuotes = true };
                parser.SetDelimiters(",");


                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                }
                parser.Close();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw;
            }

            return fields;
        }

        public string ImportScanSourceProductData(int tenantId, int? userId = null)
        {
            var TaxID = 0;
            var UOMId = 0;
            var adminUserId = 0;
            int warehouseId = 1;
            var addedProducts = 0;
            var weightGroupId = 0;
            var updatedProducts = 0;
            var productLotOptionId = 0;
            var productLotProcessId = 0;

            int counter = 0;
            var ItemImage = "";
            var MFGCode = "";
            var Manufacturer = "";
            var productHierarchy1 = "";
            var productHierarchy2 = "";
            var productHierarchy3 = "";
            var ProductFamilyImage = "";
            var scanSourceItemNumber = "";

            try
            {
                string path = ConfigurationManager.AppSettings["ImportScanSourceProductFilePath"];
                //string path = @"C:\Users\SamiChaudary\source\Workspaces\WarehouseZone\WarehouseZone\WMS\Content";
                char[] delimiter = new char[] { '|' };
                StreamReader streamreader = new StreamReader(path);
                var resultsList = new List<string>();

                using (var context = new ApplicationContext())
                {
                    var adminUser = context.AuthUsers.FirstOrDefault();
                    if (adminUser == null)
                    {
                        adminUser = new AuthUser()
                        {
                            UserId = 1
                        };
                        context.AuthUsers.Add(adminUser);
                        context.SaveChanges();
                    }
                    adminUserId = adminUser.UserId;

                    var productLotOption = context.ProductLotOptionsCodes.FirstOrDefault();
                    if (productLotOption == null)
                    {
                        productLotOption = new ProductLotOptionsCodes()
                        {
                            LotOptionCodeId = 1,
                            Description = "Imported Lot option code"
                        };
                        context.ProductLotOptionsCodes.Add(productLotOption);
                        context.SaveChanges();
                    }

                    productLotOptionId = productLotOption.LotOptionCodeId;

                    var productLotProcess = context.ProductLotProcessTypeCodes.FirstOrDefault();
                    if (productLotProcess == null)
                    {
                        productLotProcess = new ProductLotProcessTypeCodes()
                        {
                            LotProcessTypeCodeId = 1,
                            Description = "Imported Lot Process Type code"
                        };
                        context.ProductLotProcessTypeCodes.Add(productLotProcess);
                        context.SaveChanges();
                    }

                    productLotProcessId = productLotProcess.LotProcessTypeCodeId;

                    var weightGroup = context.GlobalWeightGroups.FirstOrDefault();
                    if (weightGroup == null)
                    {
                        weightGroup = new GlobalWeightGroups()
                        {
                            WeightGroupId = 1,
                            Weight = 1,
                            Description = "Imported Weight Group"
                        };
                        context.GlobalWeightGroups.Add(weightGroup);
                        context.SaveChanges();
                    }

                    weightGroupId = weightGroup.WeightGroupId;

                    var UoMId = context.GlobalUOM.FirstOrDefault();
                    if (UoMId == null)
                    {
                        UoMId = new GlobalUOM()
                        {
                            UOMId = 1,
                            UOM = "Each"
                        };
                    }
                    UOMId = UoMId.UOMId;

                    var TAXId = context.GlobalTax.FirstOrDefault();
                    if (TAXId == null)
                    {
                        TAXId = new GlobalTax()
                        {
                            TaxID = 1,
                            TaxName = "Standard",
                            PercentageOfAmount = 20,
                            CountryID = 1
                        };
                    }
                    TaxID = TAXId.TaxID;
                }

                var firstLine = streamreader.ReadLine().Split(delimiter);
                firstLine = firstLine.Select(x => x.Replace("\"", "")).ToArray();

                if (!firstLine[0].Contains("CustomerNumber") || !firstLine[1].Contains("DateGenerated") || !firstLine[2].Contains("Region") || !firstLine[3].Contains("Manufacturer") || !firstLine[4].Contains("ManufacturerDivision") || !firstLine[5].Contains("ManufacturerItemNumber") ||
                    !firstLine[6].Contains("MFGCode") || !firstLine[7].Contains("ScanSourceItemNumber") || !firstLine[8].Contains("ContractPrice") || !firstLine[9].Contains("ContractPriceCurrency") || !firstLine[10].Contains("MSRP") || !firstLine[11].Contains("MSRPCurrency") ||
                    !firstLine[12].Contains("QuantityAvailable") || !firstLine[13].Contains("BasicDescription") || !firstLine[14].Contains("EAN-UPC") || !firstLine[15].Contains("UNSPSC") || !firstLine[16].Contains("Serialized") || !firstLine[17].Contains("ItemStatus") ||
                    !firstLine[18].Contains("ItemStatusValidFrom") || !firstLine[19].Contains("DateAdded") || !firstLine[20].Contains("CountryOfOrigin") || !firstLine[21].Contains("PackagedLength") || !firstLine[22].Contains("PackagedLengthUOM") || !firstLine[23].Contains("PackagedWidth") ||
                    !firstLine[24].Contains("PackagedWidthUOM") || !firstLine[25].Contains("PackagedHeight") || !firstLine[26].Contains("PackagedHeightUOM") || !firstLine[27].Contains("GrossWeight") || !firstLine[28].Contains("GrossWeightUOM") || !firstLine[29].Contains("Commodity-ImportCode") ||
                    !firstLine[30].Contains("ProductHierarchy") || !firstLine[31].Contains("ProductHierarchy1") || !firstLine[32].Contains("ProductHierarchy2") || !firstLine[33].Contains("ProductHierarchy3") || !firstLine[34].Contains("ProductHierarchy4") || !firstLine[35].Contains("SellviaEDI") ||
                    !firstLine[36].Contains("SellviaWeb") || !firstLine[37].Contains("PublishedPricingDisplay") || !firstLine[38].Contains("MaterialType") || !firstLine[39].Contains("ProductFamily") || !firstLine[40].Contains("ProductFamilyDescription") || !firstLine[41].Contains("ProductFamilyImage") ||
                    !firstLine[42].Contains("ItemImageURL") || !firstLine[43].Contains("SpecSheetURL") || !firstLine[44].Contains("SpecSheetLanguage") || !firstLine[45].Contains("WebDescription") || !firstLine[46].Contains("WebDescriptionLang"))
                {
                    return $"File headers mismatch! Please add required headers or check the order";
                }

                string[] readText = File.ReadAllLines(path);

                if (readText.Length == 0 || readText.Length < 0)
                {
                    return $"File is empty";
                }
                else
                {
                    StreamReader sr = new StreamReader(path);
                    var headerLine = sr.ReadLine().Split(delimiter);
                    var context = new ApplicationContext();

                    while ((readText = sr.ReadLine()?.Split(delimiter)) != null)
                    {
                        try
                        {

                            readText = readText.Select(x => x.Replace("\"", "")).ToArray();

                            if (readText.Length >= 8)
                            {
                                scanSourceItemNumber = (string.IsNullOrEmpty(readText[7]) ? "0" : readText[7]);
                            }

                            if (readText.Length >= 32)
                            {
                                productHierarchy1 = readText[31];
                            }
                            if (readText.Length >= 33)
                            {
                                productHierarchy2 = readText[32];
                            }
                            if (readText.Length >= 34)
                            {
                                productHierarchy3 = readText[33];
                            }
                            if (readText.Length >= 7)
                            {
                                MFGCode = readText[6];
                            }
                            if (readText.Length >= 4)
                            {
                                Manufacturer = readText[3];
                            }

                            var actionTime = DateTime.Now;
                            var product = context.ProductMaster.FirstOrDefault(m => m.SKUCode == scanSourceItemNumber);
                            var isNewProduct = product == null;

                            if (isNewProduct)
                            {
                                addedProducts++;
                                product = new ProductMaster();
                                product.ProdStartDate = actionTime;
                                product.DateCreated = actionTime;
                                product.CreatedBy = userId;
                            }
                            else
                            {
                                updatedProducts++;
                                product.DateUpdated = actionTime;
                                product.UpdatedBy = userId;
                            }

                            var manufacturer = context.ProductManufacturers.AsNoTracking().FirstOrDefault(m => m.Name.Equals(Manufacturer));
                            if (manufacturer == null)
                            {
                                manufacturer = new ProductManufacturer()
                                {
                                    Name = string.IsNullOrEmpty(Manufacturer) ? "Scan Source" : Manufacturer,
                                    MFGCode = string.IsNullOrEmpty(MFGCode) ? "Scan Source" : MFGCode,
                                    CreatedBy = 1,
                                    DateCreated = actionTime,
                                    TenantId = tenantId,
                                };
                                context.ProductManufacturers.Add(manufacturer);
                                context.SaveChanges();
                            }

                            if (readText.Length >= 6)
                            {
                                product.ManufacturerPartNo = readText[5];
                            }
                            if (readText.Length >= 9)
                            {
                                product.BuyPrice = Decimal.Parse(string.IsNullOrEmpty(readText[8]) ? "0" : readText[8]);
                            }
                            if (readText.Length >= 11)
                            {
                                product.SellPrice = Decimal.Parse(string.IsNullOrEmpty(readText[10]) ? "0" : readText[10]);
                            }
                            if (readText.Length >= 17)
                            {
                                product.Serialisable = bool.Parse(readText[16]);
                            }
                            if (readText.Length >= 21)
                            {
                                product.CountryOfOrigion = readText[20];
                            }
                            if (readText.Length >= 24)
                            {
                                product.Width = Double.Parse(string.IsNullOrEmpty(readText[23]) ? "0" : readText[23]);
                            }
                            if (readText.Length >= 26)
                            {
                                product.Height = Double.Parse(string.IsNullOrEmpty(readText[25]) ? "0" : readText[25]);
                            }
                            if (readText.Length >= 42)
                            {
                                ProductFamilyImage = (string.IsNullOrEmpty(readText[41]) ? "" : readText[41]);
                            }
                            if (readText.Length >= 13)
                            {
                                ItemImage = (string.IsNullOrEmpty(readText[12]) ? "" : readText[42]);
                            }
                            if (readText.Length >= 13)
                            {
                                if (readText[12] != null && decimal.Parse(readText[12]) > 0)
                                {
                                    if (isNewProduct)
                                    {
                                        product.InventoryTransactions.Add(new InventoryTransaction()
                                        {
                                            WarehouseId = warehouseId,
                                            TenentId = tenantId,
                                            DateCreated = actionTime,
                                            IsActive = true,
                                            CreatedBy = userId ?? adminUserId,
                                            Quantity = decimal.Parse(readText[12]),
                                            LastQty = context.InventoryStocks.FirstOrDefault(x => x.ProductId == product.ProductId && x.TenantId == tenantId && x.WarehouseId == warehouseId)?.InStock ?? 0,
                                            IsCurrentLocation = true,
                                            InventoryTransactionTypeId = InventoryTransactionTypeEnum.AdjustmentIn
                                        });
                                    }
                                }
                            }

                            product.SKUCode = scanSourceItemNumber;
                            product.Depth = 1;
                            product.UOMId = 1;
                            product.ProductType = ProductKitTypeEnum.Simple;
                            product.TaxID = TaxID;
                            product.UOMId = UOMId;
                            product.IsActive = true;
                            product.IsDeleted = false;
                            product.PercentMargin = 0;
                            product.TenantId = tenantId;
                            product.DimensionUOMId = UOMId;
                            product.WeightGroupId = weightGroupId;
                            product.CreatedBy = userId ?? adminUserId;
                            product.BarCode = product.SKUCode;
                            product.LotOptionCodeId = productLotOptionId;
                            product.ManufacturerId = manufacturer.Id;
                            product.LotProcessTypeCodeId = productLotProcessId;
                            product.BarCode = (string.IsNullOrEmpty(scanSourceItemNumber) ? "0" : scanSourceItemNumber);

                            if (readText.Length >= 45)
                            {
                                product.Description = readText[45];
                            }
                            if (readText.Length >= 14)
                            {
                                product.Name = readText[13];
                            }

                            var department = context.TenantDepartments.AsNoTracking().FirstOrDefault(u => u.DepartmentName == productHierarchy1);
                            if (department == null)
                            {
                                department = new TenantDepartments()
                                {
                                    DepartmentName = string.IsNullOrEmpty(productHierarchy1) ? "testDepartment" : productHierarchy1,
                                    DateCreated = actionTime,
                                    TenantId = tenantId

                                };
                                context.TenantDepartments.Add(department);
                                context.SaveChanges();
                            }

                            var group = context.ProductGroups.AsNoTracking().FirstOrDefault(m => m.ProductGroup.Equals(productHierarchy2));
                            if (group == null)
                            {
                                group = new ProductGroups()
                                {
                                    ProductGroup = string.IsNullOrEmpty(productHierarchy2) ? "testGroup" : productHierarchy2,
                                    DepartmentId = department.DepartmentId,
                                    CreatedBy = 1,
                                    DateCreated = actionTime,
                                    IsActive = true,
                                    TenentId = tenantId
                                };
                                context.ProductGroups.Add(group);
                                context.SaveChanges();
                            }

                            var category = context.ProductCategories.AsNoTracking().FirstOrDefault(m => m.ProductCategoryName.Equals(productHierarchy3));
                            if (category == null)
                            {
                                category = new ProductCategory()
                                {
                                    ProductCategoryName = string.IsNullOrEmpty(productHierarchy3) ? "testCategory" : productHierarchy3,
                                    ProductGroupId = group.ProductGroupId,
                                    CreatedBy = 1,
                                    DateCreated = actionTime,
                                    TenantId = tenantId,
                                };
                                context.ProductCategories.Add(category);
                                context.SaveChanges();
                            }

                            product.ProductCategoryId = category.ProductCategoryId;
                            product.ProductGroupId = group.ProductGroupId;
                            product.DepartmentId = department.DepartmentId;

                            if (isNewProduct)
                            {
                                List<string> path1 = DownloadImage(ProductFamilyImage, scanSourceItemNumber, tenantId, userId ?? adminUserId, ItemImage);
                                product.ProductFiles = new List<ProductFiles>();
                                foreach (var filepath in path1)
                                {
                                    product.ProductFiles.Add(new ProductFiles
                                    {
                                        ProductId = product.ProductId,
                                        FilePath = filepath,
                                        TenantId = tenantId,
                                        CreatedBy = userId ?? adminUserId,
                                        DateCreated = actionTime
                                    });
                                }

                                context.ProductMaster.Add(product);
                                context.Entry(product).State = EntityState.Added;
                            }
                            else
                            {
                                context.Entry(product).State = EntityState.Modified;
                            }

                            counter++;

                            if (counter % 200 == 0)
                            {
                                context.SaveChanges();
                                context.Dispose();
                                context = new ApplicationContext();
                            }
                        }
                        catch (Exception ex)
                        {
                            resultsList.Add($"Could not import row data, row number {counter}, exception message : {ex.Message}");
                        }

                    }
                    context.SaveChanges();

                    resultsList.Insert(0, $"Product details imported successfully. Added : { addedProducts }; Updated { updatedProducts };");
                    return string.Join("   |   ", resultsList);
                }
            }
            catch (Exception e)
            {
                return "Could not import file data, exception message : " + e.Message;
            }
        }

        public string ImportCipherLabProductData(int tenantId, int? userId = null)
        {
            var UOMId = 0;
            var TaxID = 0;
            var faultyLines = 0;
            var adminUserId = 0;
            var departmentId = 0;
            var currentLine = ""; //To debug issue
            var addedProducts = 0;
            var weightGroupId = 0;
            var productGroupId = 0;
            var manufacturerId = 0;
            var updatedProducts = 0;
            var productLotOptionId = 0;
            var productLotProcessId = 0;

            string path = ConfigurationManager.AppSettings["ImportCipherLabProductFilePath"];

            try
            {
                using (var context = new ApplicationContext())
                {
                    var adminUser = context.AuthUsers.FirstOrDefault();
                    if (adminUser == null)
                    {
                        return "Import Failed: No user exists in system. Please add at least one system user.";
                    }
                    adminUserId = adminUser.UserId;

                    var department = context.TenantDepartments.FirstOrDefault();
                    if (department == null)
                    {
                        department = new TenantDepartments()
                        {
                            DepartmentName = department.DepartmentName,
                            DateCreated = DateTime.UtcNow,
                            TenantId = tenantId

                        };
                        context.TenantDepartments.Add(department);
                        context.SaveChanges();
                    }
                    departmentId = department.DepartmentId;

                    var group = context.ProductGroups.FirstOrDefault();
                    if (group == null)
                    {
                        group = new ProductGroups()
                        {
                            ProductGroup = group.ProductGroup,
                            CreatedBy = 1,
                            DateCreated = DateTime.UtcNow,
                            IsActive = true,
                            TenentId = tenantId
                        };
                        context.ProductGroups.Add(group);
                        context.SaveChanges();
                    }
                    productGroupId = group.ProductGroupId;

                    var manufacturer = context.ProductManufacturers.AsNoTracking().FirstOrDefault(m => m.Name.Equals("CipherLab"));
                    if (manufacturer == null)
                    {
                        manufacturer = new ProductManufacturer()
                        {
                            Name = "CipherLab",
                            CreatedBy = 1,
                            DateCreated = DateTime.UtcNow,
                            TenantId = tenantId,
                        };
                        context.ProductManufacturers.Add(manufacturer);
                        context.SaveChanges();
                    }
                    manufacturerId = manufacturer.Id;

                    var productLotOption = context.ProductLotOptionsCodes.FirstOrDefault();
                    if (productLotOption == null)
                    {
                        productLotOption = new ProductLotOptionsCodes()
                        {
                            LotOptionCodeId = 1,
                            Description = "Imported Lot option code"
                        };
                        context.ProductLotOptionsCodes.Add(productLotOption);
                        context.SaveChanges();
                    }
                    productLotOptionId = productLotOption.LotOptionCodeId;

                    var productLotProcess = context.ProductLotProcessTypeCodes.FirstOrDefault();
                    if (productLotProcess == null)
                    {
                        productLotProcess = new ProductLotProcessTypeCodes()
                        {
                            LotProcessTypeCodeId = 1,
                            Description = "Imported Lot Process Type code"
                        };
                        context.ProductLotProcessTypeCodes.Add(productLotProcess);
                        context.SaveChanges();
                    }
                    productLotProcessId = productLotProcess.LotProcessTypeCodeId;

                    var weightGroup = context.GlobalWeightGroups.FirstOrDefault();
                    if (weightGroup == null)
                    {
                        weightGroup = new GlobalWeightGroups()
                        {
                            WeightGroupId = 1,
                            Weight = 1,
                            Description = "Imported Weight Group"
                        };
                        context.GlobalWeightGroups.Add(weightGroup);
                        context.SaveChanges();
                    }
                    weightGroupId = weightGroup.WeightGroupId;

                    var UoMId = context.GlobalUOM.FirstOrDefault();
                    if (UoMId == null)
                    {
                        UoMId = new GlobalUOM()
                        {
                            UOMId = 1,
                            UOM = "Each"
                        };
                    }
                    UOMId = UoMId.UOMId;

                    var TAXId = context.GlobalTax.FirstOrDefault();
                    if (TAXId == null)
                    {
                        TAXId = new GlobalTax()
                        {
                            TaxID = 1,
                            TaxName = "Standard",
                            PercentageOfAmount = 20
                        };
                    }
                    TaxID = TAXId.TaxID;
                }


                var lineNumber = 0;
                List<string> headers = new List<string>();
                List<object> TotalRecored = new List<object>();
                using (var csv = new CsvReader(File.OpenText(path), CultureInfo.InvariantCulture))
                {
                    try
                    {

                        csv.Read();
                        csv.ReadHeader();
                        headers = csv.Context.HeaderRecord.ToList();
                        TotalRecored = csv.GetRecords<object>().ToList();
                    }
                    catch (Exception)
                    {
                        return $"File headers mismatch! Please add required headers";
                    }
                }

                if (headers.Count > 4)
                {
                    return $"File headers mismatch! Please add required headers";
                }

                if (!headers.Contains("Model Code") || !headers.Contains("Product Code") || !headers.Contains("Description") || !headers.Contains("List Price (USD)"))
                {
                    return $"File headers mismatch! Please add required headers";
                }
                if (TotalRecored.Count <= 0)
                {
                    return $"The file is Empty";
                }

                var productCode = "";
                using (var fs = File.OpenRead(path))
                using (var reader = new StreamReader(fs))
                {
                    var context = new ApplicationContext();
                    var currencyConversionRate = context.TenantCurrenciesExRates.OrderByDescending(x => x.TenantCurrencies.TenantId == tenantId
                                && x.TenantCurrencies.GlobalCurrency.CurrencyName == "USD").FirstOrDefault();

                    while (!reader.EndOfStream)
                    {

                        var line = reader.ReadLine();
                        lineNumber++;
                        var check = line.Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                        if (check.Length == 4)
                        {
                            var values = GetCsvLineContents(line);
                            productCode = values[1];

                            if (lineNumber == 1 || line == null)
                            {
                                continue;
                            }

                            var existingProduct = context.ProductMaster.FirstOrDefault(m => m.SKUCode == productCode);
                            var addRecord = false;

                            if (existingProduct == null)
                            {
                                addRecord = true;
                                addedProducts++;
                                existingProduct = new ProductMaster();

                                if (1 < values.Length)
                                {
                                    existingProduct.Name = values[0];
                                }
                                if (2 < values.Length)
                                {
                                    existingProduct.Description = values[2];
                                }
                            }
                            else
                            {
                                updatedProducts++;
                            }

                            existingProduct.SKUCode = productCode;
                            existingProduct.ManufacturerPartNo = productCode;

                            if (3 < values.Length)
                            {
                                // prices in cipherlab import are in dollars, so get the conversion rate and then convert it into tenant base currency before insert
                                var listPrice = string.IsNullOrEmpty(values[3]) ? 0 : decimal.Parse(values[3]);
                                if (listPrice >= 0 && currencyConversionRate != null)
                                {
                                    //convert dollar to tenant base currecny
                                    existingProduct.SellPrice = Math.Round((listPrice * currencyConversionRate.Rate), 2);
                                    existingProduct.BuyPrice = Math.Round((listPrice * currencyConversionRate.Rate) * 0.50m, 2);
                                    existingProduct.LandedCost = Math.Round((listPrice * currencyConversionRate.Rate) * 0.05m, 2);
                                }

                            }


                            existingProduct.DateCreated = DateTime.UtcNow;
                            existingProduct.ProdStartDate = DateTime.UtcNow;
                            existingProduct.Depth = 1;
                            existingProduct.UOMId = 1;
                            existingProduct.Width = 1;
                            existingProduct.Height = 1;
                            existingProduct.ProductType = ProductKitTypeEnum.Simple;
                            existingProduct.UOMId = UOMId;
                            existingProduct.TaxID = TaxID;
                            existingProduct.IsActive = true;
                            existingProduct.IsDeleted = false;
                            existingProduct.PercentMargin = 0;
                            existingProduct.TenantId = tenantId;
                            existingProduct.Serialisable = true;
                            existingProduct.DimensionUOMId = UOMId;
                            existingProduct.DepartmentId = departmentId;
                            existingProduct.CountryOfOrigion = "Taiwan";
                            existingProduct.WeightGroupId = weightGroupId;
                            existingProduct.ManufacturerId = manufacturerId;
                            existingProduct.ProductGroupId = productGroupId;
                            existingProduct.CreatedBy = userId ?? adminUserId;
                            existingProduct.BarCode = existingProduct.SKUCode;
                            existingProduct.LotOptionCodeId = productLotOptionId;
                            existingProduct.LotProcessTypeCodeId = productLotProcessId;
                            existingProduct.BarCode = (string.IsNullOrEmpty(existingProduct.SKUCode) ? "0" : existingProduct.SKUCode);


                            if (addRecord)
                            {
                                context.ProductMaster.Add(existingProduct);
                                context.Entry(existingProduct).State = EntityState.Added;
                            }
                            else
                            {
                                context.Entry(existingProduct).State = EntityState.Modified;
                            }

                            if (lineNumber % 200 == 0)
                            {
                                context.SaveChanges();
                                context.Dispose();
                                context = new ApplicationContext();
                            }

                        }
                        else
                        {
                            faultyLines++;
                            continue;
                        }
                    }
                    context.SaveChanges();
                    return $"Product details imported successfully. Added : { addedProducts }; Updated { updatedProducts }; Faulty Lines { faultyLines }";
                }
            }
            catch (Exception e)
            {
                return "Import Failed : " + e.Message + "Occurred in line :@ " + currentLine;
            }
        }
        public List<string> ImportProducts(string importPath, int tenantId, int warehouseId, ApplicationContext context = null, int? userId = null)
        {
            if (context == null)
            {
                context = new ApplicationContext();
            }

            var adminUserId = context.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
            var resultsList = new List<string>();
            var addedProducts = 0;
            var updatedProducts = 0;
            var lineNumber = 0;

            int productLotOptionId = 0;
            int productLotProcessId = 0;

            try
            {
                var productLotOption = context.ProductLotOptionsCodes.FirstOrDefault();
                if (productLotOption == null)
                {
                    productLotOption = new ProductLotOptionsCodes()
                    {
                        LotOptionCodeId = 1,
                        Description = "Imported Lot option code"
                    };
                    context.ProductLotOptionsCodes.Add(productLotOption);
                    context.SaveChanges();
                }

                productLotOptionId = productLotOption.LotOptionCodeId;

                var productLotProcess = context.ProductLotProcessTypeCodes.FirstOrDefault();
                if (productLotProcess == null)
                {
                    productLotProcess = new ProductLotProcessTypeCodes()
                    {
                        LotProcessTypeCodeId = 1,
                        Description = "Imported Lot Process Type code"
                    };
                    context.ProductLotProcessTypeCodes.Add(productLotProcess);
                    context.SaveChanges();
                }

                productLotProcessId = productLotProcess.LotProcessTypeCodeId;

                var headers = new List<string>();
                var allDataRecords = new List<ProductImportModel>();
                using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
                {
                    try
                    {
                        csv.Read();
                        csv.ReadHeader();
                        csv.Configuration.MissingFieldFound = null;
                        headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                        allDataRecords = csv.GetRecords<ProductImportModel>().ToList();
                    }
                    catch (Exception ex)
                    {
                        return new List<string> { $"Incorrect file content!, Exception: {ex.Message}" };
                    }
                }

                if (allDataRecords == null || allDataRecords.Count() <= 0)
                {
                    return new List<string> { $"Empty file, no values to import!" };
                }

                var departments = GetProductsImportDepartments(tenantId, context, allDataRecords);
                var groups = GetProductsImportGroups(tenantId, context, allDataRecords, userId ?? 0);
                var weightGroups = GetProductsImportWeightGroups(context, allDataRecords);

                foreach (var productData in allDataRecords)
                {
                    try
                    {
                        lineNumber++;
                        var product = context.ProductMaster.FirstOrDefault(m => m.SKUCode == productData.SkuCode);
                        var isNewProduct = product == null;
                        int weightGroupId = context.GlobalWeightGroups.FirstOrDefault().WeightGroupId;

                        if (weightGroups.Count() > 0)
                        {
                            int.TryParse((weightGroups[productData.WeightGroup].ToString()), out weightGroupId);
                        }


                        if (isNewProduct)
                        {
                            product = new ProductMaster
                            {
                                SKUCode = productData.SkuCode
                            };

                            if (string.IsNullOrEmpty(productData.Name?.Trim()))
                            {
                                resultsList.Add($"Product import failed. SkuCode: { productData.SkuCode }, Line number : {lineNumber}, Exception: product Name is empty");
                                continue;
                            }

                            if (string.IsNullOrEmpty(productData.SkuCode?.Trim()))
                            {
                                resultsList.Add($"Product import failed. Product Name: { productData.Name }, Line number : {lineNumber}, Exception: product SkuCode is empty");
                                continue;
                            }

                            if (departments[productData.Department] == null)
                            {
                                resultsList.Add($"Product import failed. SkuCode: { productData.SkuCode }, Exception: product Department is empty");
                                continue;
                            }

                            if (groups[productData.Group] == null)
                            {
                                resultsList.Add($"Product import failed. SkuCode: { productData.SkuCode }, Exception: product Group is empty");
                                continue;
                            }
                        }

                        if (productData.InventoryLevel != null && productData.InventoryLevel > 0)
                        {
                            var inventoryTransaction = new InventoryTransaction()
                            {
                                WarehouseId = warehouseId,
                                TenentId = tenantId,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true,
                                CreatedBy = userId ?? adminUserId,
                                Quantity = productData.InventoryLevel.Value,
                                LastQty = context.InventoryStocks.FirstOrDefault(x => x.ProductId == product.ProductId && x.TenantId == tenantId && x.WarehouseId == warehouseId)?.InStock ?? 0,
                                IsCurrentLocation = true,
                                InventoryTransactionTypeId = InventoryTransactionTypeEnum.AdjustmentIn
                            };

                            if (isNewProduct)
                            {
                                product.InventoryTransactions.Add(inventoryTransaction);
                            }
                            else
                            {
                                inventoryTransaction.ProductId = product.ProductId;
                                context.InventoryTransactions.Add(inventoryTransaction);
                            }
                        }

                        var UnitOfMeasurementId = context.GlobalUOM.FirstOrDefault(u => (u.UOMId == productData.UnitOfMeasurementId || productData.UnitOfMeasurementId == null))?.UOMId ?? 1;
                        var taxId = context.GlobalTax.FirstOrDefault(t => (t.TaxID == productData.TaxId || productData.TaxId == null)).TaxID;

                        product.PreferredSupplier = !string.IsNullOrEmpty(productData.PreferredSupplier?.Trim()) ? GetPreferredSupplier(tenantId, context, productData, taxId) : product.PreferredSupplier;
                        product.ManufacturerPartNo = !string.IsNullOrEmpty(productData.ManufacturerPartNo?.Trim()) ? productData.ManufacturerPartNo?.Trim() : product.ManufacturerPartNo;
                        product.Name = !string.IsNullOrEmpty(productData.Name?.Trim()) ? productData.Name?.Trim() : product.Name;
                        product.Description = !string.IsNullOrEmpty(productData.Description?.Trim()) ? productData.Description?.Trim() : product.Description;
                        product.BarCode = !string.IsNullOrEmpty(productData.BarCode?.Trim()) ? productData.BarCode?.Trim() : product.BarCode;
                        product.BarCode2 = !string.IsNullOrEmpty(productData.OuterBarCode?.Trim()) ? productData.OuterBarCode?.Trim() : product.BarCode2;
                        product.BuyPrice = productData.BuyPrice > 0 ? productData.BuyPrice : product.BuyPrice;
                        product.SellPrice = productData.SellPrice > 0 ? productData.SellPrice : product.SellPrice;
                        product.Serialisable = productData.Serialisable ?? product.Serialisable;
                        product.ProductType = productData.ProductType ?? product.ProductType;
                        product.IsPreOrderAccepted = productData?.IsPreOrderAccepted ?? product?.IsPreOrderAccepted;
                        product.MinDispatchDays = productData.MinDispatchDays ?? product.MinDispatchDays;
                        product.MaxDispatchDays = productData.MaxDispatchDays ?? product.MaxDispatchDays;
                        product.DateCreated = DateTime.UtcNow;
                        product.IsActive = true;
                        product.TenantId = tenantId;
                        product.IsDeleted = false;
                        product.CreatedBy = userId ?? adminUserId;
                        product.UOMId = UnitOfMeasurementId;
                        product.DimensionUOMId = UnitOfMeasurementId;
                        product.ProductGroupId = !string.IsNullOrEmpty(productData.Group?.Trim()) || isNewProduct ? groups[productData.Group] : product.ProductGroupId;
                        product.DepartmentId = !string.IsNullOrEmpty(productData.Department?.Trim()) || isNewProduct ? departments[productData.Department] ?? product.DepartmentId : product.DepartmentId;
                        product.TaxID = taxId;
                        product.WeightGroupId = (!string.IsNullOrEmpty(productData.WeightGroup?.Trim()) || isNewProduct) ? weightGroupId : product.WeightGroupId;
                        product.LotOptionCodeId = productLotOptionId;
                        product.LotProcessTypeCodeId = productLotProcessId;

                        if (isNewProduct)
                        {
                            product.ProdStartDate = DateTime.UtcNow;
                            context.ProductMaster.Add(product);
                            addedProducts++;
                        }
                        else
                        {
                            context.Entry(product).State = EntityState.Modified;
                            updatedProducts++;
                        }

                        if (lineNumber % 200 == 0)
                        {
                            context.SaveChanges();
                            context.Dispose();
                            context = new ApplicationContext();
                        }
                    }
                    catch (Exception ex)
                    {
                        resultsList.Add($"Product import failed, Line number : {lineNumber}, Exception: {ex.Message}");
                        continue;
                    }
                }

                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                return new List<string> { $"Import Failed! Exception: {ex.Message}" };
            }
            catch (Exception ex)
            {
                return new List<string> { $"Import Failed! Exception: {ex.Message}" };
            }

            if (resultsList.Count > 0)
            {
                resultsList.Insert(0, $"{resultsList.Count} Products failed to import. List of failures :");
            }
            if (updatedProducts > 0)
            {
                resultsList.Insert(0, $"{updatedProducts} Products updated.");
            }

            resultsList.Insert(0, $"{addedProducts} Products imported.");

            return resultsList;
        }

        private static int? GetPreferredSupplier(int tenantId, ApplicationContext context, ProductImportModel productData, int taxId)
        {
            if (int.TryParse(productData.PreferredSupplier, out int supplierId))
            {
                return context.Account.Find(supplierId)?.AccountID;
            }
            else
            {
                var account = context.Account.Where(x => x.CompanyName == productData.PreferredSupplier).FirstOrDefault();

                if (account != null)
                {
                    return account.AccountID;
                }
                else
                {
                    var supplier = context.Account.Add(new Account
                    {
                        CompanyName = productData.PreferredSupplier,
                        AccountTypeSupplier = true,
                        CreatedBy = 1,
                        TenantId = tenantId,
                        DateCreated = DateTime.UtcNow,
                        CountryID = 1,
                        CurrencyID = 1,
                        OwnerUserId = 1,
                        PriceGroupID = 1,
                        TaxID = taxId
                    });

                    supplier.AccountCode = "acc-" + supplier.AccountID;

                    context.SaveChanges();

                    return supplier.AccountID;
                }
            }
        }

        private static Dictionary<string, int?> GetProductsImportDepartments(int tenantId, ApplicationContext context, List<ProductImportModel> allDataRecords)
        {
            return allDataRecords.Where(p => !string.IsNullOrEmpty(p.Department?.Trim()))
                                 .Select(p => p.Department)
                                 .Distinct()
                                 .ToDictionary(p => p, p =>
                                                        {
                                                            if (int.TryParse(p, out int departmentId))
                                                            {
                                                                return context.TenantDepartments.Any(d => d.DepartmentId == departmentId && d.IsDeleted != true) ? departmentId : (int?)null;
                                                            }

                                                            var department = context.TenantDepartments.FirstOrDefault(x => x.DepartmentName.Equals(p));

                                                            if (department == null)
                                                            {
                                                                department = new TenantDepartments()
                                                                {
                                                                    DepartmentName = p,
                                                                    DateCreated = DateTime.UtcNow,
                                                                    TenantId = tenantId

                                                                };
                                                                context.TenantDepartments.Add(department);
                                                                context.SaveChanges();
                                                            }

                                                            return department.DepartmentId;
                                                        });
        }

        private static Dictionary<string, int?> GetProductsImportGroups(int tenantId, ApplicationContext context, List<ProductImportModel> allDataRecords, int userId)
        {
            return allDataRecords.Where(p => !string.IsNullOrEmpty(p.Group?.Trim()))
                                 .Select(p => p.Group)
                                 .Distinct()
                                 .ToDictionary(p => p, p =>
                                                        {
                                                            if (int.TryParse(p, out int groupId))
                                                            {
                                                                return context.ProductGroups.Any(d => d.ProductGroupId == groupId && d.IsDeleted != true) ? groupId : (int?)null;
                                                            }

                                                            var group = context.ProductGroups.FirstOrDefault(x => x.ProductGroup.Equals(p));

                                                            if (group == null)
                                                            {
                                                                group = new ProductGroups()
                                                                {
                                                                    ProductGroup = p,
                                                                    CreatedBy = userId,
                                                                    DateCreated = DateTime.UtcNow,
                                                                    IsActive = true,
                                                                    TenentId = tenantId
                                                                };
                                                                context.ProductGroups.Add(group);
                                                                context.SaveChanges();
                                                            }

                                                            return group.ProductGroupId;
                                                        });
        }

        private static Dictionary<string, int> GetProductsImportWeightGroups(ApplicationContext context, List<ProductImportModel> allDataRecords)
        {
            return allDataRecords.Where(p => !string.IsNullOrEmpty(p.WeightGroup?.Trim()))
                                 .Select(p => p.WeightGroup)
                                 .Distinct()
                                 .ToDictionary(p => p, p =>
                                                        {
                                                            GlobalWeightGroups weightGroup = null;

                                                            if (int.TryParse(p, out int weightGroupId))
                                                            {
                                                                weightGroup = context.GlobalWeightGroups.Find(weightGroupId);

                                                                if (weightGroup == null)
                                                                {
                                                                    weightGroup = new GlobalWeightGroups()
                                                                    {
                                                                        WeightGroupId = 1,
                                                                        Weight = 0,
                                                                        Description = "Imported Weight Group"
                                                                    };
                                                                    context.GlobalWeightGroups.Add(weightGroup);
                                                                    context.SaveChanges();
                                                                }
                                                            }
                                                            else
                                                            {
                                                                weightGroup = context.GlobalWeightGroups.FirstOrDefault(x => x.Description.Equals(p));

                                                                if (weightGroup == null)
                                                                {
                                                                    weightGroup = new GlobalWeightGroups()
                                                                    {
                                                                        WeightGroupId = 1,
                                                                        Weight = 0,
                                                                        Description = p

                                                                    };
                                                                    context.GlobalWeightGroups.Add(weightGroup);
                                                                    context.SaveChanges();
                                                                }
                                                            }
                                                            return weightGroup.WeightGroupId;
                                                        });
        }

        public string ImportProductsPrice(string importPath, int tenantId, int warehouseId, ApplicationContext context = null, int? userId = null, int pricegroupId = 0, int actiondetail = 1)
        {
            if (context == null)
            {
                context = new ApplicationContext();
            }
            var adminUserId = context.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;
            var lineNumber = 0;
            string recorednotmatched = "";
            int count = 0;
            List<string> headers = new List<string>();
            List<object> TotalRecored = new List<object>();
            using (var csv = new CsvReader(File.OpenText(importPath), CultureInfo.InvariantCulture))
            {
                try
                {



                    csv.Read();
                    csv.ReadHeader();
                    headers = csv.Context.HeaderRecord.ToList(); headers = headers.ConvertAll(d => d.ToLower());
                    TotalRecored = csv.GetRecords<object>().ToList();
                }
                catch (Exception)
                {
                    return $"File headers mismatch! Please add required headers";
                }
            }
            var group = context.TenantPriceGroups.FirstOrDefault(u => u.PriceGroupID == pricegroupId);
            if (headers.Count > 4)
            {
                return $"File headers mismatch! Please add required headers";
            }
            if (!headers.Contains("sku") || !headers.Contains("special price") || !headers.Contains("start date") || !headers.Contains("end date"))
            {
                return $"File headers mismatch! Please add required headers";
            }
            if (TotalRecored.Count <= 0)
            {
                return $"Empty file, no values to import";
            }
            if (group == null)
            {

                return $"No matching price group found";
            }
            else
            {
                using (var fs = File.OpenRead(importPath))
                using (var reader = new StreamReader(fs))
                {
                    if (reader != null)
                    {
                        while (!reader.EndOfStream)
                        {
                            try
                            {

                                lineNumber++;
                                var line = reader.ReadLine();
                                if (lineNumber == 1)
                                {
                                    continue;
                                }

                                if (line == null)
                                {
                                    if (count >= 50) { return recorednotmatched; }
                                    recorednotmatched += "Import Failed: no record found in line :@ " + lineNumber + "<br/> ";
                                    count++;
                                    continue;
                                }
                                var values = GetCsvLineContents(line);
                                if (values != null || values.Length >= 2)
                                {
                                    var productCode = values[0];
                                    if (string.IsNullOrEmpty(productCode))
                                    {
                                        if (count >= 50) { return recorednotmatched; }
                                        recorednotmatched += "Import failed: product code not found on line:@ " + lineNumber + "<br/> ";
                                        count++;
                                        continue;
                                    }
                                    var existingProduct = context.ProductMaster.FirstOrDefault(m => m.SKUCode.Equals(productCode.Trim(), StringComparison.InvariantCultureIgnoreCase));
                                    if (existingProduct == null)
                                    {
                                        if (count >= 50) { return recorednotmatched; }
                                        recorednotmatched += "Import failed: product not found on line :@ " + lineNumber + "<br/> ";
                                        count++;
                                        continue;
                                    }

                                    if (!string.IsNullOrEmpty(values[1]))
                                    {
                                        decimal price;
                                        if (!Decimal.TryParse(values[1], out price))
                                        {
                                            recorednotmatched += "Import failed: price not found on line :@ " + lineNumber + "<br/> ";
                                            count++;
                                            continue;
                                        }

                                    }
                                    else
                                    {
                                        recorednotmatched += "Import failed: price not found on line :@ " + lineNumber + "<br/> ";
                                        count++;
                                        continue;
                                    }



                                }
                            }
                            catch (Exception ex)
                            {
                                if (count >= 50) { return recorednotmatched; }
                                recorednotmatched += "Import Failed: " + ex.Message + "occurred on line :@ " + lineNumber + "<br/> ";
                                count++;
                                continue;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recorednotmatched))
                {
                    return recorednotmatched;
                }
                else
                {
                    DateTimeFormatInfo ukDtfi = new CultureInfo("en-GB", false).DateTimeFormat;
                    var formatStrings = new string[] { @"dd/MM/yyyy", @"d/MM/yyyy", @"d/M/yyyy", @"dd/M/yyyy" };
                    if (actiondetail == 2)
                    {
                        var ProductPriceGroup = context.ProductSpecialPrices.Where(u => u.PriceGroupID == pricegroupId).ToList();
                        ProductPriceGroup.ForEach(u => u.SpecialPrice = 0);
                        context.SaveChanges();
                    }
                    using (var fs = File.OpenRead(importPath))
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            try
                            {

                                var line = reader.ReadLine();
                                var values = GetCsvLineContents(line);
                                if (values != null || values.Length >= 4)
                                {
                                    var productCode = values[0];
                                    var existingProduct = context.ProductMaster.FirstOrDefault(m => m.SKUCode.Equals(productCode.Trim(), StringComparison.InvariantCultureIgnoreCase));
                                    if (existingProduct != null)
                                    {
                                        decimal specialPrice = 0;
                                        DateTime? startdate = DateTime.MinValue;
                                        DateTime? enddate = DateTime.MinValue;
                                        DateTime sdate = DateTime.UtcNow;
                                        DateTime edate = DateTime.UtcNow;
                                        if (!string.IsNullOrEmpty(values[1]))
                                        {
                                            specialPrice = Convert.ToDecimal(values[1]);

                                        }
                                        if (!string.IsNullOrEmpty(values[2]))
                                        {
                                            if (DateTime.TryParseExact(values[2], formatStrings, ukDtfi, DateTimeStyles.None, out sdate))
                                            {
                                                startdate = sdate;

                                            }
                                            //else if (DateTime.TryParseExact(values[2], formatStrings, ukDtfi, DateTimeStyles.None, out sdate))
                                            //{
                                            //    startdate = sdate;

                                            //}

                                            else
                                            {
                                                startdate = null;
                                            }
                                        }
                                        else
                                        {
                                            startdate = null;
                                        }
                                        if (!string.IsNullOrEmpty(values[3]))
                                        {
                                            if (DateTime.TryParseExact(values[3], formatStrings, ukDtfi, DateTimeStyles.None, out edate))
                                            {
                                                enddate = edate;
                                            }
                                            //else if (DateTime.TryParseExact(values[3], @"d/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out edate))
                                            //{


                                            //    enddate = edate;

                                            //}
                                            else
                                            {
                                                enddate = null;
                                            }
                                        }
                                        else
                                        {
                                            enddate = null;
                                        }
                                        var ProductSpeialPriceGroup = context.ProductSpecialPrices.FirstOrDefault(u => u.ProductID == existingProduct.ProductId && u.PriceGroupID == pricegroupId);
                                        if (ProductSpeialPriceGroup != null)
                                        {
                                            ProductSpeialPriceGroup.SpecialPrice = specialPrice;
                                            ProductSpeialPriceGroup.StartDate = startdate;
                                            ProductSpeialPriceGroup.EndDate = enddate;
                                            ProductSpeialPriceGroup.DateUpdated = DateTime.UtcNow;
                                            ProductSpeialPriceGroup.TenantId = tenantId;
                                            ProductSpeialPriceGroup.IsDeleted = false;
                                            ProductSpeialPriceGroup.CreatedBy = userId ?? adminUserId;

                                            context.Entry(ProductSpeialPriceGroup).State = EntityState.Modified;


                                        }
                                        else
                                        {
                                            TenantPriceGroupDetail tenantPriceGroupDetail = new TenantPriceGroupDetail();
                                            tenantPriceGroupDetail.ProductID = existingProduct.ProductId;
                                            tenantPriceGroupDetail.PriceGroupID = pricegroupId;
                                            tenantPriceGroupDetail.SpecialPrice = specialPrice;
                                            tenantPriceGroupDetail.StartDate = startdate;
                                            tenantPriceGroupDetail.EndDate = enddate;
                                            tenantPriceGroupDetail.DateCreated = DateTime.UtcNow;
                                            tenantPriceGroupDetail.TenantId = tenantId;
                                            tenantPriceGroupDetail.IsDeleted = false;
                                            tenantPriceGroupDetail.CreatedBy = userId ?? adminUserId;
                                            context.ProductSpecialPrices.Add(tenantPriceGroupDetail);



                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                EventLog.WriteEntry(ex.Source, ex.Message);
                                throw;
                            }
                        }

                        context.SaveChanges();
                    }

                }
            }

            return $"Product details imported successfully. Added : Updated";

        }


        public string ImportJobSubTypes(string importPath, int tenantId, ApplicationContext context = null)
        {
            if (context == null)
            {
                context = new ApplicationContext();
            }
            try
            {
                using (var fs = File.OpenRead(importPath))
                using (var reader = new StreamReader(fs))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (line == null) throw new ArgumentNullException(nameof(line));

                        var values = GetCsvLineContents(line);

                        if (string.IsNullOrEmpty(values[0]) || string.IsNullOrEmpty(values[1]))
                        {
                            continue;
                        }

                        var subTypeName = values[0];


                        var existingJobType = context.JobSubTypes.FirstOrDefault(m => m.Name == subTypeName);

                        if (existingJobType == null)
                        {
                            existingJobType = new JobSubType() { Name = subTypeName, Description = subTypeName, TenantId = tenantId };
                            context.JobSubTypes.Add(existingJobType);
                        }
                        //Just with name, can't do any updates
                        //else
                        //{
                        //    context.Entry(existingJobType).State = EntityState.Modified;
                        //}
                        context.SaveChanges();

                    }
                }

                context.SaveChanges();


            }
            catch (Exception ex)
            {
                return "Import Failed : " + ex.Message;
            }

            return $"Job Account details imported successfully.";
        }



        public bool ImportScanSourceProduct(int tenantId, int userId, ApplicationContext context = null)
        {
            bool status = false;

            try
            {

                string[] manufacturer = ConfigurationManager.AppSettings["ProductImportManufacturerNames"].Split(new string[] { ",", "," }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var items in manufacturer)
                {
                    for (int i = 1; i <= 1000; i++)
                    {
                        if (context == null)
                        {
                            context = new ApplicationContext();
                        }
                        var productSearchResult = GetScanSourceSearchproduct(i, items);
                        if (productSearchResult.Count > 0)
                        {
                            List<string> itemCodes = new List<string>();
                            foreach (var item in productSearchResult)
                            {
                                //var productDetail = GetScanSourceProductDetial(item.ManufacturerItemNumber);
                                if (item != null && !string.IsNullOrEmpty(item.ScanSourceItemNumber) && !string.IsNullOrEmpty(item.ProductFamilyHeadline))
                                {
                                    var product = context.ProductMaster.AsNoTracking().FirstOrDefault(u => u.SKUCode.Equals(item.ScanSourceItemNumber.Trim(), StringComparison.InvariantCultureIgnoreCase));
                                    var desc = item.Description.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                    var productFamilyHeadlines = item.ProductFamilyHeadline.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                                    var productCategories = context.ProductGroups.FirstOrDefault(u => desc.Contains(u.ProductGroup) || productFamilyHeadlines.Contains(u.ProductGroup));

                                    if (product == null)
                                    {
                                        itemCodes.Add(item.ManufacturerItemNumber);
                                        ProductMaster productMaster = new ProductMaster();
                                        productMaster.ManufacturerPartNo = item.ManufacturerItemNumber;
                                        productMaster.SKUCode = item.ScanSourceItemNumber.Trim();
                                        productMaster.TaxID = 1;
                                        productMaster.Name = item.ProductFamily;
                                        //productMaster.CommodityCode = item.CommodityImportCodeNumber;
                                        productMaster.Description = item.Description;
                                        productMaster.UOMId = 1;
                                        productMaster.Serialisable = false;
                                        productMaster.LotOption = false;
                                        productMaster.LotOptionCodeId = 1;
                                        productMaster.LotProcessTypeCodeId = 1;
                                        productMaster.BarCode = item.ScanSourceItemNumber;
                                        //productMaster.PackSize = 0;
                                        productMaster.Height = 0;
                                        productMaster.Width = 0;
                                        productMaster.Depth = 0;
                                        productMaster.Weight = 0;
                                        productMaster.TaxID = 1;
                                        productMaster.WeightGroupId = 1;
                                        productMaster.PercentMargin = 0;
                                        productMaster.ProductType = ProductKitTypeEnum.Simple;
                                        productMaster.IsActive = true;
                                        productMaster.ProdStartDate = DateTime.UtcNow;
                                        productMaster.Discontinued = false;
                                        productMaster.DepartmentId = 1;
                                        productMaster.ProcessByCase = false;
                                        productMaster.ProcessByPallet = false;
                                        productMaster.IsStockItem = false;
                                        productMaster.ProductType = ProductKitTypeEnum.Simple;
                                        productMaster.TenantId = tenantId;
                                        productMaster.ProductGroupId = productCategories?.ProductGroupId;

                                        productMaster.DateCreated = DateTime.UtcNow;
                                        //productMaster.CountryOfOrigion = productDetail.CountryofOrigin;
                                        List<string> path = DownloadImage(item?.ProductFamilyImage, item?.ScanSourceItemNumber, tenantId, userId, item.ItemImage);
                                        productMaster.ProductFiles = new List<ProductFiles>();
                                        foreach (var filepath in path)
                                        {
                                            productMaster.ProductFiles.Add(new ProductFiles
                                            {
                                                ProductId = productMaster.ProductId,
                                                FilePath = filepath,
                                                TenantId = tenantId,
                                                CreatedBy = userId,
                                                DateCreated = DateTime.UtcNow

                                            });
                                        }

                                        context.ProductMaster.Add(productMaster);

                                    }
                                }

                            }

                            context.SaveChanges();

                            // get prices for all items.
                            //TODO: customer number to be added in database against tenant config, static value should be replaced by DB value asap.
                            var productPrices = GetScanSourceProductPrice("1000003502", itemCodes);
                            if (productPrices != null)
                            {
                                foreach (var price in productPrices)
                                {
                                    var product = context.ProductMaster.FirstOrDefault(x => x.ManufacturerPartNo == price.ItemNumber);
                                    if (product != null)
                                    {
                                        product.BuyPrice = Convert.ToDecimal(price.UnitPrice);
                                        product.SellPrice = Convert.ToDecimal(price.MSRP);
                                        context.Entry(product).State = EntityState.Modified;
                                    }
                                }
                            }

                            context.SaveChanges();
                            context = null;
                            status = true;
                        }
                        else
                        {
                            break;
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw;
            }

            return status;
        }



        public List<ScanSourceSearchproductModel> GetScanSourceSearchproduct(int i, string manufacturer)
        {
            try
            {
                string url = "https://services.scansource.com/apisandbox/product/search?customerNumber=1000003502&Manufacturer=" + manufacturer + "&page=" + i + "&pageSize=99";
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", "NzFfEHqpWwDOVL7NR53enDPoNxUtmPI6");
                httpWebRequest.Accept = "application/json";
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    List<ScanSourceSearchproductModel> productSearch = JsonConvert.DeserializeObject<List<ScanSourceSearchproductModel>>(result);
                    return productSearch;
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw;
            }

        }
        public ProductDetails GetScanSourceProductDetial(string itemNumber)
        {
            ProductDetails productDetail = new ProductDetails();
            try
            {
                string urlProductDetial = "https://services.scansource.com/apisandbox/product/detail?customerNumber=1000003502&itemNumber=" + itemNumber + "&partNumberType=0";
                var httpWebRequestPd = (HttpWebRequest)WebRequest.Create(urlProductDetial);
                httpWebRequestPd.Headers.Add("Ocp-Apim-Subscription-Key", "NzFfEHqpWwDOVL7NR53enDPoNxUtmPI6");
                httpWebRequestPd.Accept = "application/json";
                httpWebRequestPd.Method = "GET";

                var httpResponsepd = (HttpWebResponse)httpWebRequestPd.GetResponse();
                using (var streamReader = new StreamReader(httpResponsepd.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    productDetail = JsonConvert.DeserializeObject<ProductDetails>(result);
                    return productDetail;
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw;
            }
        }

        public List<ScanSourceProductPrice> GetScanSourceProductPrice(string CustomerNumber, List<string> ManufacturerItemNumbers)
        {
            if (ManufacturerItemNumbers == null || ManufacturerItemNumbers.Count < 1)
            {
                return null;
            }

            ScanSourceProductPricePost priceRequest = new ScanSourceProductPricePost();
            priceRequest.CustomerNumber = CustomerNumber;
            priceRequest.Lines = new List<PricingRequestLine>();
            foreach (var item in ManufacturerItemNumbers)
            {
                PricingRequestLine pricingRequestLine = new PricingRequestLine();
                pricingRequestLine.itemNumber = item;
                priceRequest.Lines.Add(pricingRequestLine);
            }

            string urls = "https://services.scansource.com/apisandbox/product/pricing";

            // Uses the System.Net.WebClient and not HttpClient, because .NET 2.0 must be supported.
            using (var client = new WebClient())
            {
                try
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    client.Headers.Add("Ocp-Apim-Subscription-Key", "NzFfEHqpWwDOVL7NR53enDPoNxUtmPI6");
                    string serialisedData = JsonConvert.SerializeObject(priceRequest);
                    var response = client.UploadString(urls, serialisedData);
                    var result = JsonConvert.DeserializeObject<List<ScanSourceProductPrice>>(response);

                    return result;
                }
                catch (Exception ex)
                {
                    EventLog.WriteEntry(ex.Source, ex.Message);
                    throw;
                }
            }
        }

        public List<string> DownloadImage(string path, string productId, int tenantId, int userId, string itemImage, bool category = false, List<ProductMedia> productMedia = null)
        {
            string UploadDirectory = @"~/UploadedFiles/Products/";
            int i = 0;
            List<string> values = new List<string>();
            if (!string.IsNullOrEmpty(path))
            {
                if (RemoteFileExists(path))
                {
                    i++;
                    try
                    {
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString())))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString()));
                        }

                        string resFileName = HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString() + @"/" + "Product" + i + ".jpg");

                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(path, resFileName);
                        values.Add((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"));
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Source, ex.Message);
                    }
                }

            }
            if (!string.IsNullOrEmpty(itemImage))
            {
                if (RemoteFileExists(itemImage))
                {
                    try
                    {
                        i++;
                        if (!Directory.Exists(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString())))
                        {
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString()));
                        }

                        string resFileName = HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString() + @"/" + "Product" + i + ".jpg");

                        WebClient webClient = new WebClient();
                        webClient.DownloadFile(itemImage, resFileName);
                        values.Add((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"));
                    }
                    catch (Exception ex)
                    {
                        EventLog.WriteEntry(ex.Source, ex.Message);
                    }
                }
            }

            return values;

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

        public void SaveProductFile(string path, int ProductId, int tenantId, int userId, ApplicationContext _currentDbContext = null)
        {
            try
            {
                if (_currentDbContext == null)
                {
                    _currentDbContext = new ApplicationContext();
                }
                ProductFiles productFiles = new ProductFiles();
                productFiles.FilePath = path;
                productFiles.ProductId = ProductId;
                productFiles.TenantId = tenantId;
                productFiles.CreatedBy = userId;
                productFiles.DateCreated = DateTime.UtcNow;
                _currentDbContext.ProductFiles.Add(productFiles);
                _currentDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw;
            }
        }

        public List<int> GetPrestaShopProducts(int? Id, DateTime? date, string skuCode, int TenantId, int UserId, string PrestashopUrl, string PrestashopKey)
        {
            var context = new ApplicationContext();
            try
            {

                List<int> ProductIds = new List<int>();
                #region ApiCall
                string url = PrestashopUrl + "products/?filter[date_upd]=>[" + (date.HasValue ? date.Value.ToString() : "") + "]&date=1&display=full";
                if (Id.HasValue)
                {
                    url = PrestashopUrl + "products/?filter[id]=[" + Id + "]&display=full";
                }

                PrestashopProduct productSearch = new PrestashopProduct();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopProduct));
                    productSearch = serializer.Deserialize(streamReader) as PrestashopProduct;

                    if (productSearch?.Products?.Product?.Count < 0)
                    {
                        return default;
                    }

                }
                #endregion
                if (productSearch.Products.Product.Count > 0)
                {
                    foreach (var item in productSearch.Products.Product.Where(x => !string.IsNullOrEmpty(x.Reference)))
                    {
                        var productMaster = context.ProductMaster.FirstOrDefault(u => u.SKUCode == item.Reference.Trim() && u.IsDeleted != true);
                        if (productMaster == null)
                        {
                            productMaster = new ProductMaster();
                            productMaster.DateCreated = DateTime.UtcNow;
                            productMaster.CreatedBy = UserId;
                            productMaster.SKUCode = item.Reference.Trim();
                            productMaster.TaxID = 1;
                            productMaster.Name = string.IsNullOrEmpty(item?.Name?.Language?.Text) ? "PrestaShopProduct" : item.Name.Language.Text;
                            productMaster.Description = GetPlainTextFromHtml(item.Description.Language.Text);
                            productMaster.UOMId = 1;
                            productMaster.Serialisable = false;
                            productMaster.LotOption = false;
                            productMaster.LotOptionCodeId = 1;
                            productMaster.LotProcessTypeCodeId = 1;
                            productMaster.BarCode = item.Reference.Trim();
                            productMaster.Height = item.Height ?? 0;
                            productMaster.Width = item.Width ?? 0;
                            productMaster.Depth = item.Depth ?? 0;
                            productMaster.Weight = item.Weight ?? 0;
                            productMaster.TaxID = 1;
                            productMaster.WeightGroupId = 1;
                            productMaster.PercentMargin = 0;
                            productMaster.ProductType = ProductKitTypeEnum.Simple;
                            productMaster.IsActive = true;
                            productMaster.ProdStartDate = DateTime.UtcNow;
                            productMaster.Discontinued = false;
                            productMaster.DepartmentId = 1;
                            productMaster.ProcessByCase = false;
                            productMaster.ProcessByPallet = false;
                            productMaster.IsStockItem = false;
                            productMaster.ProductType = ProductKitTypeEnum.Simple;
                            productMaster.TenantId = TenantId;
                            productMaster.ProductGroupId = 1;
                            productMaster.BuyPrice = item.Price;
                            context.Entry(productMaster).State = EntityState.Added;
                            context.SaveChanges();
                            ProductIds.Add(productMaster.ProductId);
                        }
                        else
                        {
                            ProductIds.Add(productMaster.ProductId);
                        }
                    }
                }

                return ProductIds;
            }

            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw ex;
            }
        }

        public async Task<bool> GetPrestaShopAddress(int? id_customer, DateTime? date, int tenantId, int UserId, int accountId, int DeliveryAddressId, int InvoiceAdressId, string PrestashopUrl, string PrestashopKey)
        {
            var context = new ApplicationContext();
            bool result = false;
            try
            {
                #region ApiCall

                string url = PrestashopUrl + "addresses/?filter[date_upd]=>[" + date + "]&date=1&display=full";
                if (id_customer.HasValue)
                {
                    url = PrestashopUrl + "addresses/?filter[id_customer]=[" + id_customer + "]&display=full";
                }
                PrestashopAddress accountAddressSearch = new PrestashopAddress();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopAddress));
                    accountAddressSearch = serializer.Deserialize(streamReader) as PrestashopAddress;
                }

                #endregion

                foreach (var item in accountAddressSearch.Addresses.Address)
                {
                    result = true;
                    int countryId = 0;

                    if (item.Id_country?.Text > 0)
                    {
                        var country = await GetPrestaShopCountry(item.Id_country.Text, PrestashopUrl, PrestashopKey);
                        if (country != null)
                        {
                            countryId = country.FirstOrDefault();
                        }
                    }

                    int? mainId = Convert.ToInt32(item.Id);
                    var currentAddress = context.AccountAddresses.FirstOrDefault(u => u.PrestaShopAddressId == mainId && u.AccountID == accountId && u.IsDeleted != true);
                    if (currentAddress == null)
                    {
                        currentAddress = new AccountAddresses()
                        {
                            Name = item.Firstname + " " + item.Lastname,
                            PostCode = item.Postcode,
                            Town = item.City,
                            AddressLine1 = item.Address1,
                            AddressLine2 = item.Address2,
                            DateCreated = DateTime.UtcNow,
                            IsActive = true,
                            CountryID = countryId,
                            AccountID = accountId,
                            CreatedBy = UserId,
                            PrestaShopAddressId = mainId,
                            Telephone = item.Phone
                        };
                    }
                    else
                    {
                        currentAddress.Name = item.Firstname + " " + item.Lastname;
                        currentAddress.PostCode = item.Postcode;
                        currentAddress.AddressLine1 = item.Address1;
                        currentAddress.AddressLine2 = item.Address2;
                        currentAddress.Town = item.City;
                        currentAddress.DateUpdated = DateTime.UtcNow;
                        currentAddress.UpdatedBy = UserId;
                        currentAddress.CountryID = countryId;
                        currentAddress.Telephone = item.Phone;
                    }

                    if ((mainId == DeliveryAddressId || mainId == InvoiceAdressId) && DeliveryAddressId == InvoiceAdressId)
                    {
                        currentAddress.AddTypeShipping = true;
                        currentAddress.AddTypeBilling = true;
                    }
                    else if (mainId == DeliveryAddressId)
                    {
                        currentAddress.AddTypeShipping = true;
                    }
                    else if (mainId == InvoiceAdressId)
                    {
                        currentAddress.AddTypeBilling = true;
                    }

                    context.Entry(currentAddress).State = currentAddress.AddressID > 0 ? EntityState.Modified : EntityState.Added;
                    context.SaveChanges();

                }
            }

            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
            }

            return result;

        }

        public async Task<List<int>> GetPrestaShopAccount(int? Id, DateTime? date, int tenantId, int UserId, int DeliveryAddressId, int InvoiceAdressId, string PrestashopUrl, string PrestashopKey)
        {
            List<int> AccountIds = new List<int>();
            var context = new ApplicationContext();

            try
            {
                #region apicall
                string url = PrestashopUrl + "customers/?filter[date_upd]=>[" + (date.HasValue ? date.Value.ToString() : "") + "]&date=1&display=full";
                if (Id.HasValue)
                {
                    url = PrestashopUrl + "customers/?filter[id]=[" + Id + "]&display=full";
                }
                PrestashopAccounts accountSearch = new PrestashopAccounts();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopAccounts));
                    accountSearch = serializer.Deserialize(streamReader) as PrestashopAccounts;

                }
                #endregion

                foreach (var item in accountSearch.Customers.Customer)
                {
                    var account = context.Account.FirstOrDefault(u => u.AccountCode.Equals(item.Secure_key, StringComparison.CurrentCultureIgnoreCase) && u.IsDeleted != true);
                    if (account == null)
                    {
                        account = new Account();
                        account.CompanyName = string.IsNullOrEmpty(item.Company) ? item.Firstname + " " + item.Lastname : item.Company;
                        account.AccountCode = item.Secure_key;
                        account.website = item.Website;
                        account.AccountEmail = item.Email;
                        account.AccountStatusID = AccountStatusEnum.Active;
                        account.DateCreated = DateTime.UtcNow;
                        account.AccountTypeCustomer = true;
                        account.CreatedBy = UserId;
                        account.TaxID = context.GlobalTax.First(m => m.TaxName.Contains("Standard")).TaxID;
                        account.TenantId = tenantId;
                        account.CountryID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0;
                        account.CurrencyID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CurrencyID;
                        account.PriceGroupID = context.TenantPriceGroups.FirstOrDefault(m => m.TenantId == tenantId).PriceGroupID;

                        account.OwnerUserId = UserId;
                        context.Account.Add(account);
                        context.SaveChanges();
                    }
                    else
                    {
                        account.CompanyName = string.IsNullOrEmpty(item.Company) ? item.Firstname + " " + item.Lastname : item.Company;
                        account.AccountCode = item.Secure_key;
                        account.website = item.Website;
                        account.DateUpdated = DateTime.UtcNow;
                        account.UpdatedBy = UserId;
                        context.Entry(account).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    var currentContact = context.AccountContacts.FirstOrDefault(u => u.AccountID == account.AccountID && u.IsDeleted != true);
                    if (currentContact == null)
                    {
                        currentContact = new AccountContacts()
                        {
                            ContactName = item.Firstname + " " + item.Lastname,
                            ConTypeInvoices = true,
                            ContactEmail = item.Email,
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = UserId,
                            IsActive = true,
                            AccountID = account.AccountID
                        };

                        context.AccountContacts.Add(currentContact);
                        context.SaveChanges();

                    }
                    else
                    {
                        currentContact.ContactName = item.Firstname + " " + item.Lastname;
                        currentContact.ContactEmail = item.Email;
                        currentContact.DateUpdated = DateTime.UtcNow;
                        currentContact.UpdatedBy = UserId;
                        context.Entry(currentContact).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    var result = await GetPrestaShopAddress(item.Id, null, tenantId, UserId, account.AccountID, DeliveryAddressId, InvoiceAdressId, PrestashopUrl, PrestashopKey);

                    account.DateUpdated = DateTime.Now;
                    context.Entry(account).State = EntityState.Modified;
                    context.SaveChanges();
                    AccountIds.Add(account.AccountID);

                }

                return AccountIds;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw ex;
            }
        }

        public async Task<string> GetPrestaShopOrdersSync(int tenantId, int warehouseId, string PrestashopUrl, string PrestashopKey, int ApiId)
        {
            DateTime requestTime = new DateTime(2000, 01, 01);
            var dates = requestTime.ToString("yyyy-MM-dd-HH:mm:ss");
            WebResponse httpResponse = null;
            DateTime requestSentTime = DateTime.UtcNow;
            string url = "";
            try
            {
                var _currentDbContext = new ApplicationContext();
                var GetSyncRecored = _currentDbContext.TerminalsLog.Where(u => u.Ack != false && u.ApiId == ApiId).OrderByDescending(u => u.DateCreated).FirstOrDefault();
                if (GetSyncRecored != null)
                {
                    requestTime = GetSyncRecored.DateCreated;
                    dates = requestTime.ToString("yyyy-MM-dd-HH:mm:ss");
                }
                url = PrestashopUrl + "orders?filter[date_upd]=>[" + dates + "]&date=1&display=full";//RH: Removed &filter[current_state]=2  as its not downloading OrderUpdated records
                PrestashopOrders orderSearch = new PrestashopOrders();
                requestSentTime = DateTime.UtcNow;
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopOrders));
                    orderSearch = serializer.Deserialize(streamReader) as PrestashopOrders;
                    if (orderSearch?.Orders?.Order.Count > 0)
                    {
                        CreateWebSiteSyncLog(requestTime, "OK", true, orderSearch.Orders.Order.Count, requestSentTime, ApiId, tenantId);
                    }
                    else
                    {
                        CreateWebSiteSyncLog(requestTime, "OK", true, orderSearch.Orders.Order.Count, requestSentTime, ApiId, tenantId);
                        return "No Orders found";
                    }
                }

                foreach (var item in orderSearch.Orders.Order)
                {
                    var order = _currentDbContext.Order.FirstOrDefault(u => u.PrestaShopOrderId == item.Id && u.ApiCredentialId == ApiId && u.IsDeleted != true);
                    if (order == null)
                    {
                        order = new Order();
                        
                        string orderNumber = $"{item.Id}-{item.Reference}";
                        order.OrderNumber = orderNumber;
                        var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(orderNumber, StringComparison.CurrentCultureIgnoreCase) && m.IsDeleted != true);
                        if (duplicateOrder != null)
                        {
                            throw new Exception($"Order Number {orderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
                        }

                        order.IssueDate = Convert.ToDateTime(item.Date_add);
                        order.DateCreated = Convert.ToDateTime(item.Date_add);
                        order.DateUpdated = DateTime.UtcNow;
                        order.TenentId = tenantId;
                        order.CreatedBy = 1;
                        order.UpdatedBy = 1;
                        order.WarehouseId = warehouseId;

                        var warehouse = _currentDbContext.TenantWarehouses.FirstOrDefault(w => w.WarehouseId == warehouseId && w.IsDeleted != true && w.TenantId == tenantId);

                        if (item.Current_state.Text == (int)PrestashopOrderStateEnum.Updating)
                        {
                            order.OrderStatusID = OrderStatusEnum.OrderUpdating;
                            order.OrderNotes.Add(new OrderNotes() { TenantId = tenantId, DateCreated = DateTime.Now, Notes = "Prestashop order is set to 'Updating' status" });
                        }
                        else
                        {
                            if (!warehouse.AutoAllowProcess)
                            {
                                order.OrderStatusID = OrderStatusEnum.Hold;

                            }
                            else order.OrderStatusID = OrderStatusEnum.Active;
                        }

                        var accounts = await GetPrestaShopAccount(item.Id_customer.Text, null, tenantId, 1, item.Id_address_delivery.Text, item.Id_address_invoice.Text, PrestashopUrl, PrestashopKey);
                        var accountID = accounts?.FirstOrDefault() ?? 1;
                        if (accountID > 0)
                        {
                            var account = _currentDbContext.Account.Find(accountID);
                            if (account != null)
                            {
                                order.AccountCurrencyID = account.CurrencyID;
                            }
                            order.AccountID = accountID;
                            var accountAddress = GetAccountAddressesByPrestaShopAddressId(item.Id_address_delivery.Text);
                            if (accountAddress != null)
                            {
                                order.ShipmentAddressName = accountAddress.Name;
                                order.ShipmentAccountAddressId = accountAddress.AddressID;
                                order.ShipmentAddressLine1 = accountAddress.AddressLine1;
                                order.ShipmentAddressLine2 = accountAddress.AddressLine2;
                                order.ShipmentAddressTown = accountAddress.Town;
                                order.ShipmentAddressPostcode = accountAddress.PostCode;
                                order.ShipmentCountryId = accountAddress.CountryID;
                            }
                        }
                    }
                    else
                    {
                        var accounts = await GetPrestaShopAccount(item.Id_customer.Text, null, tenantId, 1, item.Id_address_delivery.Text, item.Id_address_invoice.Text, PrestashopUrl, PrestashopKey);
                        var accountID = accounts?.FirstOrDefault() ?? 1;
                        if (accountID > 0)
                        {
                            var account = _currentDbContext.Account.Find(accountID);
                            if (account != null)
                            {
                                order.AccountCurrencyID = account.CurrencyID;

                            }
                            order.AccountID = accountID;
                            var accountAddress = GetAccountAddressesByPrestaShopAddressId(item.Id_address_delivery.Text);
                            if (accountAddress != null)
                            {
                                order.ShipmentAccountAddressId = accountAddress.AddressID;
                                order.ShipmentAddressLine1 = accountAddress.AddressLine1;
                                order.ShipmentAddressLine2 = accountAddress.AddressLine2;
                                order.ShipmentAddressTown = accountAddress.Town;
                                order.ShipmentAddressPostcode = accountAddress.PostCode;
                                order.ShipmentCountryId = accountAddress.CountryID;
                            }
                        }

                        var warehouse = _currentDbContext.TenantWarehouses.FirstOrDefault(w => w.WarehouseId == warehouseId && w.IsDeleted != true && w.TenantId == tenantId);

                        if (item.Current_state.Text == (int)PrestashopOrderStateEnum.Updating)
                        {
                            order.OrderStatusID = OrderStatusEnum.OrderUpdating;
                            order.OrderNotes.Add(new OrderNotes() { TenantId = tenantId, DateCreated = DateTime.Now, Notes = "Prestashop order is set to 'Updating' status" });
                        }
                        else
                        {
                            if (!warehouse.AutoAllowProcess)
                            {
                                order.OrderStatusID = OrderStatusEnum.Hold;

                            }
                            else order.OrderStatusID = OrderStatusEnum.Active;
                        }

                        order.DateUpdated = DateTime.UtcNow;
                        order.TenentId = tenantId;
                        order.UpdatedBy = 1;
                        order.WarehouseId = warehouseId;
                    }

                    order.InventoryTransactionTypeId = InventoryTransactionTypeEnum.SalesOrder;
                    if (item.Urgent > 0)
                    {
                        order.SLAPriorityId = 2;
                    }
                    else if (item.Next_day_delivery > 0)
                    {
                        order.SLAPriorityId = 1;
                    }
                    else
                    {
                        order.SLAPriorityId = 3;
                    }


                    // check country and priority and find suitable delivery service for the order
                    var deliveryService = GetShiipingServiceForOrder(order.ShipmentCountryId, order.SLAPriorityId);

                    if (deliveryService != null)
                    {
                        order.TenantDeliveryServiceId = deliveryService.Id;
                        order.DeliveryMethod = deliveryService.DeliveryMethod;
                    }

                    _currentDbContext.Entry(order).State = order.OrderID > 0 ? EntityState.Modified : EntityState.Added;
                    decimal? ordTotal = 0;
                    foreach (var order_row in item.Associations.Order_rows.Order_row)
                    {
                        var orderDetail = _currentDbContext.OrderDetail.FirstOrDefault(u => u.OrderID == order.OrderID);
                        if (orderDetail == null)
                        {
                            orderDetail = new OrderDetail();
                            orderDetail.DateCreated = DateTime.UtcNow;
                            orderDetail.CreatedBy = 1;
                            var Product = GetPrestaShopProducts(order_row.Product_id, null, order_row.Product_reference, tenantId, 1, PrestashopUrl, PrestashopKey);
                            orderDetail.ProductId = Product?.FirstOrDefault() ?? 1;
                        }
                        else
                        {
                            orderDetail.DateUpdated = DateTime.UtcNow;
                            orderDetail.UpdatedBy = 1;
                        }

                        orderDetail.OrderID = order.OrderID;
                        orderDetail.TenentId = tenantId;
                        orderDetail.SortOrder = 0;
                        orderDetail.ProductMaster = null;
                        orderDetail.TaxName = null;
                        orderDetail.Warranty = null;
                        orderDetail.Qty = order_row.Product_quantity;
                        orderDetail.Price = order_row.Unit_price_tax_incl;
                        orderDetail.WarehouseId = warehouseId;
                        orderDetail.TotalAmount = (order_row.Unit_price_tax_incl * order_row.Product_quantity);
                        if (order.OrderID <= 0)
                        {
                            order.OrderDetails.Add(orderDetail);
                        }
                        else
                        {
                            _currentDbContext.Entry(orderDetail).State = EntityState.Modified;
                        }

                        ordTotal = ordTotal + ((order_row.Unit_price_tax_incl * order_row.Product_quantity));

                    }

                    order.OrderTotal = (decimal)ordTotal;
                    order.OrderCost = (decimal)ordTotal;
                    if (item.Id > 0)
                    {
                        order.PrestaShopOrderId = item.Id;
                    }

                    order.ApiCredentialId = ApiId;
                    _currentDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                CreateWebSiteSyncLog(requestTime, "Error", false, 0, requestSentTime, ApiId, tenantId);

                throw ex;
                EventLog.WriteEntry(ex.Source, ex.Message);
                return ex.Message;
            }

            return "All data synced successfully";
        }

        public AccountAddresses GetAccountAddressesByPrestaShopAddressId(int id)
        {
            var _currentdbContext = new ApplicationContext();

            return _currentdbContext.AccountAddresses.FirstOrDefault(u => u.PrestaShopAddressId == id);
        }

        public async Task<List<int>> GetPrestaShopCountry(int? Id, string PrestashopUrl, string PrestashopKey)
        {
            var context = new ApplicationContext();
            try
            {
                List<int> countIds = new List<int>();
                #region ApiCall
                string url = PrestashopUrl + "countries?filter[active]=[1]&display=full";
                if (Id.HasValue)
                {
                    url = PrestashopUrl + "countries?filter[id]=[" + Id + "]&display=full";
                }
                PrestaShopCountry countries = new PrestaShopCountry();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)await httpWebRequest.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestaShopCountry));
                    countries = serializer.Deserialize(streamReader) as PrestaShopCountry;
                    if (countries.Countries.Country.Count < 0)
                    {
                        return default;
                    }
                }
                #endregion
                if (countries.Countries.Country.Count > 0)
                {
                    foreach (var item in countries.Countries.Country)
                    {
                        var country = context.GlobalCountries.FirstOrDefault(u => u.CountryCode == item.Iso_code.Trim() || u.AdditionalCountryCodes.Contains(item.Iso_code.Trim()) || u.CountryName == item.Name.Language.Text);
                        if (country == null)
                        {
                            country = new GlobalCountry();
                            country.CountryName = item.Name.Language.Text;
                            country.CountryCode = item.Iso_code;
                            context.Entry(country).State = EntityState.Added;
                            context.SaveChanges();
                        }

                        countIds.Add(country.CountryID);
                    }
                }
                return countIds;
            }

            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
                throw ex;
            }

        }

        public async Task<string> PrestaShopStockSync(int tenantId, int warehouseId, string PrestashopUrl, string PrestashopKey, int SiteId)
        {
            DateTime requestTime = new DateTime(2000, 01, 01);
            var dates = requestTime.ToString("yyyy-MM-dd-HH:mm:ss");
            WebResponse httpResponse = null;
            DateTime requestSentTime = DateTime.UtcNow;
            string url = "";
            try
            {

                var _currentDbContext = new ApplicationContext();
                url = PrestashopUrl + "products/?display=full";
                PrestashopProduct prestashopProducts = new PrestashopProduct();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                requestSentTime = DateTime.UtcNow;
                httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopProduct));
                    prestashopProducts = serializer.Deserialize(streamReader) as PrestashopProduct;
                    if (prestashopProducts?.Products?.Product?.Count > 0)
                    {
                        CreateWebSiteSyncLog(requestTime, "OK", true, prestashopProducts.Products.Product.Count, requestSentTime, SiteId, tenantId);
                    }
                    else
                    {
                        CreateWebSiteSyncLog(requestTime, "OK", true, prestashopProducts.Products.Product.Count, requestSentTime, SiteId, tenantId);
                        return "No Product Found";
                    }

                }
                var prestashopSkucode = prestashopProducts.Products.Product.Select(u => u.Reference).ToList();

                var productavailableIds = prestashopProducts.Products.Product.Select(u => new
                {
                    StockAvailable = u.Associations.Stock_availables.Stock_available,
                    SKU = u.Reference,
                    ShopId = Convert.ToInt32(u.Id_shop_default)
                }).ToList();

                var getProductDetails = _currentDbContext.ProductMaster.Where(u => prestashopSkucode.Contains(u.SKUCode) && u.IsDeleted != true && u.TenantId == tenantId)
                    .Select(u => new
                    {
                        InventoryStock = u.InventoryStocks,
                        SkuCode = u.SKUCode,

                    }).ToList();

                if (getProductDetails.Count > 0)
                {
                    List<stock_available> stock_Availables = new List<stock_available>();
                    foreach (var item in getProductDetails)
                    {
                        var sku = item.SkuCode;
                        var stockdetail = productavailableIds.FirstOrDefault(u => u.SKU == sku).StockAvailable;
                        var shopId = productavailableIds.FirstOrDefault(u => u.SKU == sku).ShopId;
                        if (stockdetail != null)
                        {
                            stock_available stock_Available = new stock_available();
                            stock_Available.id_product = prestashopProducts.Products.Product.FirstOrDefault(u => u.Reference == item.SkuCode).Id;
                            stock_Available.out_of_stock = item.InventoryStock == null ? 0 : Convert.ToInt32(Math.Round(item.InventoryStock.Sum(u => u.InStock)));
                            stock_Available.quantity = item.InventoryStock == null ? 0 : Convert.ToInt32(Math.Round(item.InventoryStock.Sum(u => u.Available)));
                            stock_Available.depends_on_stock = 0;
                            stock_Available.id_product_attribute = stockdetail == null ? 0 : stockdetail.Id_product_attribute;
                            stock_Available.StockAvailableId = stockdetail == null ? 0 : stockdetail.Id;
                            stock_Available.id_shop = shopId;
                            stock_Availables.Add(stock_Available);
                        }
                    }
                    if (stock_Availables.Count > 0)
                    {
                        url = PrestashopUrl + "stock_availables";
                        UpdatePrestaShopStock(url, PrestashopKey, stock_Availables);
                    }
                }
            }
            catch (Exception e)
            {
                CreateWebSiteSyncLog(requestTime, e.Message, true, 0, requestSentTime, SiteId, tenantId);
                EventLog.WriteEntry(e.Source, e.Message);
                return "Stock levels update failed";
            }

            return "Stock levels updated successfully";

        }


        public async Task<string> PrestaShopOrderStatusUpdate(int orderId, PrestashopOrderStateEnum status, string packedByName=null, PalletsDispatch dispatchInfo=null)
        {
            var _currentDbContext = new ApplicationContext();
            var orderToUpdate = _currentDbContext.Order.FirstOrDefault(x => x.OrderID == orderId);
            var apiCredentials = _currentDbContext.ApiCredentials.FirstOrDefault(x => x.Id == orderToUpdate.ApiCredentialId && x.ApiTypes == ApiTypes.PrestaShop);

            DateTime requestTime = new DateTime(2000, 01, 01);
            WebResponse httpResponse = null;
            DateTime requestSentTime = DateTime.UtcNow;
            string url = "";
            try
            {
                url = apiCredentials.ApiUrl + "orders/" + orderToUpdate.PrestaShopOrderId;
                PrestashopOrderSingle prestaShopOrder = new PrestashopOrderSingle();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(apiCredentials.ApiKey, "");

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                requestSentTime = DateTime.UtcNow;
                httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var serializer = new XmlSerializer(typeof(PrestashopOrderSingle));

                    Current_state currentState = new Current_state {Text = (int) status};

                    prestaShopOrder = serializer.Deserialize(streamReader) as PrestashopOrderSingle;
                    
                    if(prestaShopOrder == null) throw new Exception("Failed to Deserialize, unable to locate the order " + url);

                    prestaShopOrder.Order.Current_state = currentState;
                    if (dispatchInfo!=null)
                    {
                        prestaShopOrder.Order.Dpd_Parcel_numbers = dispatchInfo.ParcelNumbers;
                        prestaShopOrder.Order.Dpd_Consignment_numbers = dispatchInfo.ConsignmentNumber;
                        prestaShopOrder.Order.Dpd_Shipment_Id = dispatchInfo.ShipmentId;
                    }

                    if (status == PrestashopOrderStateEnum.PickAndPack)
                    {
                        prestaShopOrder.Order.PickedByName = packedByName;
                    }

                    url = apiCredentials.ApiUrl + "orders";
                    var response = UpdatePrestaShopOrder(url, apiCredentials.ApiKey, prestaShopOrder);
                }
            }
            catch (Exception e)
            {
                CreateWebSiteSyncLog(requestTime, e.Message, true, 0, requestSentTime, apiCredentials.Id, orderToUpdate.TenentId);
                EventLog.WriteEntry(e.Source, e.Message);
                return "Unable to update order status";
            }

            return "Order Status updated successfully";

        }

        //public async Task<string> PrestaShopOrderUpdateConsignmentNumber(int palletDispatchId)
        //{
        //    var palletDispatch = _context.PalletDispatches.FirstOrDefault(m=> m.PalletDispatchId==palletDispatchId);

        //    DateTime requestTime = new DateTime(2000, 01, 01);
        //    WebResponse httpResponse = null;
        //    DateTime requestSentTime = DateTime.UtcNow;
        //    string url = "";
        //    try
        //    {
        //        url = PrestashopUrl + "orders/" + prestashopOrderId;
        //        PrestashopOrderSingle prestaShopOrder = new PrestashopOrderSingle();
        //        var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
        //        httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");

        //        httpWebRequest.Method = "GET";
        //        httpWebRequest.ContentType = "application/json";
        //        requestSentTime = DateTime.UtcNow;
        //        httpResponse = await httpWebRequest.GetResponseAsync().ConfigureAwait(false);
        //        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        //        {
        //            var serializer = new XmlSerializer(typeof(PrestashopOrderSingle));

        //            prestaShopOrder = serializer.Deserialize(streamReader) as PrestashopOrderSingle;

        //            if(prestaShopOrder == null) throw new Exception("Failed to Deserialize, unable to locate the order " + url);

        //            prestaShopOrder.Order.Shipping_number = new Shipping_number(){ NotFilterable  = consignmentNumber };

        //            url = PrestashopUrl + "orders";
        //            UpdatePrestaShopOrder(url, PrestashopKey, prestaShopOrder);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        CreateWebSiteSyncLog(requestTime, e.Message, true, 0, requestSentTime, null, tenantId);
        //        EventLog.WriteEntry(e.Source, e.Message);
        //        return "Unable to update order status";
        //    }

        //    return "Order Status updated successfully";

        //}

        public string UpdatePrestaShopOrder(string prestashopUrl, string prestashopKey, PrestashopOrderSingle order)
        {
            WebRequest req = null;
            WebResponse rsp = null;
            string Response = "";

            try
            {
                req = WebRequest.Create(prestashopUrl);
                req.Method = "PUT";
                req.Credentials = new NetworkCredential(prestashopKey, "");
                req.ContentType = "text/xml";

                XmlSerializer xmlSerializer = new XmlSerializer(order.GetType());
                StringWriter textWriter = new StringWriter();
                xmlSerializer.Serialize(textWriter, order);

                var data = textWriter.ToString();
                data = data.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n", "");

                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                writer.WriteLine(data);
                writer.Close();
                rsp = req.GetResponse();
                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                Response = sr.ReadToEnd();
                sr.Close();
                textWriter.Close();
            }
            catch (WebException webEx)
            {
                EventLog.WriteEntry(webEx.Source, webEx.Message);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
            }
            finally
            {
                if (req != null) req.GetRequestStream().Close();
                if (rsp != null) rsp.GetResponseStream().Close();
            }

            return Response;
        }

        private string GetPlainTextFromHtml(string htmlString)
        {
            if (!string.IsNullOrEmpty(htmlString))
            {
                string htmlTagPattern = "<.*?>";
                htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
                htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            }

            return htmlString;

        }

        public void CreateWebSiteSyncLog(DateTime RequestTime, string ErrorCode, bool synced, int resultCount, DateTime RequestedTime, int? SiteId=null, int TenantId=1)
        {
            var dbcontext = new ApplicationContext();
            TerminalLogTypeEnum logType = (TerminalLogTypeEnum)Enum.Parse(typeof(TerminalLogTypeEnum), "PrestaShopOrderSync");

            TerminalsLog newDeviceLog = new TerminalsLog();
            newDeviceLog.TerminalLogId = Guid.NewGuid();
            newDeviceLog.TerminalLogType = logType.ToString();
            newDeviceLog.Response = ErrorCode.Trim().ToString();
            newDeviceLog.DateCreated = RequestedTime;
            newDeviceLog.TenantId = TenantId;
            newDeviceLog.DateRequest = RequestTime;
            newDeviceLog.ApiId = SiteId;
            newDeviceLog.RecievedCount = resultCount;
            newDeviceLog.Ack = synced;
            dbcontext.TerminalsLog.Add(newDeviceLog);
            dbcontext.SaveChanges();


        }


        public string UpdatePrestaShopStock(string prestashopUrl, string prestashopKey, List<stock_available> stock_Availables)
        {
            WebRequest req = null;
            WebResponse rsp = null;
            string Response = "";

            try
            {
                req = WebRequest.Create(prestashopUrl);
                req.Method = "PUT";
                req.Credentials = new NetworkCredential(prestashopKey, "");
                req.ContentType = "text/xml";
                StreamWriter writer = new StreamWriter(req.GetRequestStream());
                var data = GenerateXmlPrestaShopStockUpdate(stock_Availables);
                writer.WriteLine(data);
                writer.Close();
                rsp = req.GetResponse();
                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                Response = sr.ReadToEnd();
                sr.Close();

            }
            catch (WebException webEx)
            {
                EventLog.WriteEntry(webEx.Source, webEx.Message);
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
            }
            finally
            {
                if (req != null) req.GetRequestStream().Close();
                if (rsp != null) rsp.GetResponseStream().Close();
            }

            return Response;
        }

        public string GenerateXmlPrestaShopStockUpdate(List<stock_available> stock_Availables)
        {

            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("prestashop");
            xml.AppendChild(root);

            foreach (var stock in stock_Availables)
            {
                XmlElement child = xml.CreateElement("stock_available");

                XmlElement id = xml.CreateElement("id");
                id.InnerText = stock.StockAvailableId.ToString();
                child.AppendChild(id);

                XmlElement id_product = xml.CreateElement("id_product");
                id_product.InnerText = stock.id_product.ToString();
                child.AppendChild(id_product);

                XmlElement id_product_attribute = xml.CreateElement("id_product_attribute");
                id_product_attribute.InnerText = stock.id_product_attribute.ToString();
                child.AppendChild(id_product_attribute);

                XmlElement quantity = xml.CreateElement("quantity");
                quantity.InnerText = stock.quantity.ToString();
                child.AppendChild(quantity);


                XmlElement depends_on_stock = xml.CreateElement("depends_on_stock");
                depends_on_stock.InnerText = stock.depends_on_stock.ToString();
                child.AppendChild(depends_on_stock);

                XmlElement out_of_stock = xml.CreateElement("out_of_stock");
                out_of_stock.InnerText = stock.out_of_stock.ToString();
                child.AppendChild(out_of_stock);

                XmlElement id_shop = xml.CreateElement("id_shop");
                id_shop.InnerText = stock.id_shop.ToString();
                child.AppendChild(id_shop);

                root.AppendChild(child);
            }

            return xml.OuterXml;
        }

        public string GetDPDServices()
        {
            WebResponse httpResponse = null;
            string url = "";
            try
            {
                var _currentDbContext = new ApplicationContext();
                var apiCredential = _currentDbContext.ApiCredentials.FirstOrDefault(u => u.ApiTypes == ApiTypes.DPD);
                url = apiCredential?.ApiUrl;
                url = url + "network/?businessUnit=0&deliveryDirection=1&numberOfParcels=1&shipmentType=0&totalWeight=1.0&deliveryDetails.address.countryCode=GB&deliveryDetails.address.countryName=United Kingdom&deliveryDetails.address.locality=&deliveryDetails.address.organisation=Gane DataScan Ltd&deliveryDetails.address.postcode=LS16 6RF&deliveryDetails.address.property=Airedale House&deliveryDetails.address.street=Clayton Wood Rise&deliveryDetails.address.town=Leeds&deliveryDetails.address.county=West Yorkshire&collectionDetails.address.countryCode=GB&collectionDetails.address.countryName=United Kingdom&collectionDetails.address.locality=&collectionDetails.address.organisation=&collectionDetails.address.postcode=BD4 6BU&collectionDetails.address.property=1 School View&collectionDetails.address.street=Bierley House Avenue&collectionDetails.address.town=Bradford&collectionDetails.address.county=West Yorkshire";
                DPDViewModel dpdServices = new DPDViewModel();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpResponse = httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dpdServices = JsonConvert.DeserializeObject<DPDViewModel>(result);
                }
                foreach (var item in dpdServices.data.ToList())
                {
                    var service = _currentDbContext.TenantDeliveryServices.FirstOrDefault(u => u.NetworkCode == item.network.networkCode.Trim());
                    if (service != null)
                    {
                        service.NetworkDescription = item.network.networkDescription;
                        _currentDbContext.Entry(service).State = EntityState.Modified;
                    }
                    else
                    {

                        service = new TenantDeliveryService();
                        service.NetworkCode = item.network.networkCode;
                        service.NetworkDescription = item.network.networkDescription;
                        service.TenantId = 1;
                        service.DateCreated = DateTime.UtcNow;
                        service.DateUpdated = DateTime.UtcNow;
                        service.CreatedBy = 1;
                        service.UpdatedBy = 1;
                        service.IsDeleted = true;
                        _currentDbContext.Entry(service).State = EntityState.Added;

                    }

                }
                _currentDbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
            }

            return "";

        }
        public string PostShipmentData(int PalletDispatchId, DpdShipmentDataViewModel dpdShipmentDataViewModel)
        {
            WebResponse httpResponse = null;
            string url = "";
            try
            {
                var _currentDbContext = new ApplicationContext();
                var apiCredential = _currentDbContext.ApiCredentials.FirstOrDefault(u => u.ApiTypes == ApiTypes.DPD);
                if (apiCredential == null || string.IsNullOrEmpty(apiCredential.ApiUrl) || string.IsNullOrEmpty(apiCredential.ApiKey) || string.IsNullOrEmpty(apiCredential.AccountNumber))
                {
                    return "Api Configuration is invalid, Either Api url, Api key or Api account fields are empty";
                }
                if (apiCredential.ExpiryDate == null || (apiCredential.ExpiryDate.HasValue && apiCredential.ExpiryDate.Value.Day != DateTime.Today.Day))
                {
                    apiCredential.ApiKey = GetDPDGeoSession(apiCredential);

                }
                url = apiCredential.ApiUrl + "shipping/shipment";
                DpdReponseViewModel dpdResponse = new DpdReponseViewModel();
                DpdErrorViewModel errorViewModel = new DpdErrorViewModel();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("GeoSession", apiCredential.ApiKey);
                httpWebRequest.Headers.Add("GeoClient", "account/" + apiCredential.AccountNumber);
                httpWebRequest.ContentType = "application/json";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(dpdShipmentDataViewModel);
                    streamWriter.Write(json);
                }
                httpResponse = httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    dpdResponse = JsonConvert.DeserializeObject<DpdReponseViewModel>(result);
                    errorViewModel = JsonConvert.DeserializeObject<DpdErrorViewModel>(result);
                }
                if (dpdResponse.data != null)
                {
                    foreach (var item in dpdResponse.data.consignmentDetail.ToList())
                    {
                        var palletDispatch = _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == PalletDispatchId);
                        if (palletDispatch != null)
                        {
                            palletDispatch.ShipmentId = dpdResponse.data.shipmentId.ToString();
                            palletDispatch.ParcelNumbers = string.Join(",", item.parcelNumbers.ToList());
                            palletDispatch.ConsignmentNumber = item.consignmentNumber;
                            _currentDbContext.Entry(palletDispatch).State = EntityState.Modified;
                            _currentDbContext.SaveChanges();
                        }

                    }
                }
                else
                {
                    if (errorViewModel.error != null)
                    {
                        foreach (var item in errorViewModel.error.ToList())
                        {
                            return item.obj + " " + item.errorMessage;
                        }

                    }

                }
                _currentDbContext.SaveChanges();


            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(ex.Source, ex.Message);
            }

            return "Data Posted";

        }

        public string GetDPDGeoSession(ApiCredentials apiCredential)
        {
            var _currentDbContext = new ApplicationContext();
            string authorization = GetEncodeUserNameBas64(apiCredential.UserName, apiCredential.Password);
            WebResponse httpResponse = null;
            string url = apiCredential.ApiUrl;
            url += "user/?action=login";
            GeoSession GeoSession = new GeoSession();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentLength = 0;
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authorization);
            httpResponse = httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                GeoSession = JsonConvert.DeserializeObject<GeoSession>(result);
            }
            if (GeoSession != null)
            {
                var updateApi = _currentDbContext.ApiCredentials.FirstOrDefault(u => u.Id == apiCredential.Id);
                if (updateApi != null)
                {
                    updateApi.ExpiryDate = DateTime.Today.AddHours(23);
                    updateApi.ApiKey = GeoSession.data.geoSession;
                    updateApi.DateUpdated = DateTime.UtcNow;
                    updateApi.UpdatedBy = 1;
                    _currentDbContext.Entry(updateApi).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();

                }

            }


            return GeoSession.data.geoSession;
        }


        private string GetEncodeUserNameBas64(string userName, string Password)
        {
            string singleString = userName + ":" + Password;
            var plainTextBytes = Encoding.UTF8.GetBytes(singleString);
            return Convert.ToBase64String(plainTextBytes);
        }

        public async Task<List<string>> GetAddressByPostCodeAsync(string postCode)
        {
            List<string> errorString = new List<string>();

            if (!IsValidUkPostcode(postCode))
            {
                errorString.Add("Invalid PostCode!");
                return errorString;
            }

            try
            {
                var apiToken = ConfigurationManager.AppSettings["PostCodeKey"];
                var apiLink = ConfigurationManager.AppSettings["ApiUrl"];
                string apiUrl = string.Empty;
                apiLink = apiLink + postCode;
                apiUrl = apiLink + "?api-key=" + apiToken;

                PostCodeAddressViewModel model = new PostCodeAddressViewModel();


                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(apiUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    var response = await client.GetAsync(new Uri(apiUrl));
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        model = JsonConvert.DeserializeObject<PostCodeAddressViewModel>(response.Content.ReadAsStringAsync().Result);
                    }
                    else
                    {
                        errorString.Add(response.Content.ReadAsStringAsync().Result);
                        return errorString;
                    }
                }
                return model.addresses;
            }
            catch (Exception Exp)
            {
                errorString.Add(Exp.Message.ToString());
                return errorString;
            }
        }


        public async Task<GoogleDistanceMatrixResult> GetDistancesFromPostcode(string origin, List<string> destinationsList)
        {
            var apiUrl = ConfigurationManager.AppSettings["GoogleApisUrl"];
            var apiKey = ConfigurationManager.AppSettings["GoogleApiKey"];

            if (string.IsNullOrEmpty(origin.Trim()) || !destinationsList.Any(w => !string.IsNullOrEmpty(w.Trim())))
            {
                return null;
            }

            var destinations = string.Join("|", destinationsList.Where(w => !string.IsNullOrEmpty(w.Trim())).Select(a => a.Replace(" ", "+")));

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"distancematrix/json?origins={origin.Replace(" ", "+")}&destinations={destinations}&key={apiKey}&mode=driving&language=en-GB&sensor=false&units=imperial");

                return JsonConvert.DeserializeObject<GoogleDistanceMatrixResult>(response.Content.ReadAsStringAsync().Result);
            }
        }

        public TenantDeliveryService GetShiipingServiceForOrder(int? countryId, int? priorityId)
        {
            var _currentDbContext = new ApplicationContext();
            return _currentDbContext.TenantDeliveryServices.Where(p => p.SLAPriorityId == priorityId && p.TenantDeliveryServiceCountryMap.Any(x => x.CountryId == countryId)).FirstOrDefault();
        }
    }


    public class GoogleDistanceMatrixResult
    {
        public string Status { get; set; }

        [JsonProperty(PropertyName = "destination_addresses")]
        public string[] Destinations { get; set; }

        [JsonProperty(PropertyName = "origin_addresses")]
        public string[] Origins { get; set; }

        public Row[] Rows { get; set; }

        public class Data
        {
            public int Value { get; set; }
            public string Text { get; set; }
        }

        public class Element
        {
            public string Status { get; set; }
            public Data Duration { get; set; }
            public Data Distance { get; set; }
        }

        public class Row
        {
            public Element[] Elements { get; set; }
        }
    }

    public class ProductPriceSpecial
    {

        string skucode { get; set; }
        string price { get; set; }

        string startDate { get; set; }
        string endDate { get; set; }



    }

    public class GeoSessionData
    {
        public string geoSession { get; set; }
        public string flag { get; set; }
    }

    public class GeoSession
    {
        public object error { get; set; }
        public GeoSessionData data { get; set; }
    }
}

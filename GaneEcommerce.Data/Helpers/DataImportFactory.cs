using CsvHelper;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Elmah;
using System.Text;

namespace Ganedata.Core.Data.Helpers
{
    public class DataImportFactory
    {
        private static string DefaultContactName { get; set; } = "Helpdesk";

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
                        //if (9 > values.Length || string.IsNullOrEmpty(values[9]) || values[9].Trim().ToUpper() == "NULL")
                        //{
                        //    if (count >= 50) { return recorednotmatched; }
                        //    recorednotmatched += "Import Failed : billing address 1 not found on line :@ " + lineNumber + "<br/> ";
                        //    count++;
                        //    continue;

                        //}
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
                        var saveCounter = 0;
                        var counter1 = 0;

                        while (!reader.EndOfStream)
                        {
                            counter++;
                            counter1++;
                            saveCounter++;
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

                            existingAccount.AccountStatusID = 1;
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

                            if (counter == 133)
                            {
                                line.ToString();
                            }

                            //if (!string.IsNullOrEmpty(shippingAddress1) || shippingAddress1.Trim().ToUpper() == "NULL" && !string.IsNullOrEmpty(shippingPostcode) || shippingPostcode.Trim().ToUpper() == "NULL")
                            //{
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
                            //}

                            //if (!string.IsNullOrEmpty(billingAddress1) || billingAddress1.Trim().ToUpper() == "NULL" && !string.IsNullOrEmpty(billingPostcode) || billingPostcode.Trim().ToUpper() == "NULL")
                            //{
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
                            //}






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
                                if (counter1 == 11)
                                {
                                    line.ToString();
                                }
                                line.ToString();
                                if (currentShiipingAddress != null)
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

                                if (currentBillingAddress != null)
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

                                if (currentShiipingAddress != null)
                                {
                                    existingAccount.AccountAddresses.Add(currentShiipingAddress);
                                }

                                if (currentBillingAddress != null)
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


                            //if (saveCounter == 200)
                            //{

                            context.SaveChanges();
                            counter.ToString();
                            //saveCounter = 0;
                            line.ToString();
                            //}

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

        private bool IsValidUkPostcode(string postcodeString)
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
                ex.ToString();
                throw;
            }

            return fields;
        }

        public string ImportScanSourceProductData(int tenantId, int? userId = null)
        {
            var TaxID = 0;
            var UOMId = 0;
            var adminUserId = 0;
            var addedProducts = 0;
            var weightGroupId = 0;
            var updatedProducts = 0;
            var productLotOptionId = 0;
            var productLotProcessId = 0;

            try
            {

                int counter = 0;
                var ItemImage = "";
                var productHierarchy1 = "";
                var productHierarchy2 = "";
                var ProductFamilyImage = "";
                var scanSourceItemNumber = "";

                string path = ConfigurationManager.AppSettings["ImportScanSourceProductFilePath"];
                //string path = @"C:\Users\SamiChaudary\source\Workspaces\WarehouseZone\WarehouseZone\WMS\Content";
                char[] delimiter = new char[] { '|' };
                StreamReader streamreader = new StreamReader(path);

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
                            Weight = 0,
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
                            UOMId = 1
                        };
                    }
                    UOMId = UoMId.UOMId;

                    var TAXId = context.GlobalTax.FirstOrDefault();
                    if (TAXId == null)
                    {
                        TAXId = new GlobalTax()
                        {
                            TaxID = 1
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

                    while ((readText = sr.ReadLine().Split(delimiter)) != null)
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


                        var department = context.TenantDepartments.AsNoTracking().FirstOrDefault(u => u.DepartmentName == productHierarchy1);
                        if (department == null)
                        {
                            department = new TenantDepartments()
                            {
                                DepartmentName = string.IsNullOrEmpty(productHierarchy1) ? "testDepartment" : productHierarchy1,
                                DateCreated = DateTime.UtcNow,
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
                                CreatedBy = 1,
                                DateCreated = DateTime.UtcNow,
                                IsActive = true,
                                TenentId = tenantId
                            };
                            context.ProductGroups.Add(group);
                            context.SaveChanges();
                        }

                        var existingProduct = context.ProductMaster.FirstOrDefault(m => m.SKUCode == scanSourceItemNumber);
                        var addRecord = false;
                        if (existingProduct == null)
                        {
                            addRecord = true;
                            addedProducts++;
                            existingProduct = new ProductMaster();
                            existingProduct.DateCreated = DateTime.UtcNow;
                            existingProduct.CreatedBy = userId;
                            existingProduct.ProdStartDate = DateTime.UtcNow;
                        }
                        else
                        {
                            updatedProducts++;
                            existingProduct.DateUpdated = DateTime.UtcNow;
                            existingProduct.ProdStartDate = DateTime.UtcNow;
                            existingProduct.UpdatedBy = userId;
                        }


                        existingProduct.SKUCode = scanSourceItemNumber;


                        if (readText.Length >= 7)
                        {
                            existingProduct.ManufacturerPartNo = readText[6];
                        }
                        if (readText.Length >= 11)
                        {
                            existingProduct.SellPrice = Decimal.Parse(string.IsNullOrEmpty(readText[10]) ? "0" : readText[10]);
                        }
                        if (readText.Length >= 14)
                        {
                            existingProduct.Description = readText[13];
                        }
                        if (readText.Length >= 17)
                        {
                            existingProduct.Serialisable = bool.Parse(readText[16]);
                        }
                        if (readText.Length >= 24)
                        {
                            existingProduct.Width = Double.Parse(string.IsNullOrEmpty(readText[23]) ? "0" : readText[23]);
                        }
                        if (readText.Length >= 26)
                        {
                            existingProduct.Height = Double.Parse(string.IsNullOrEmpty(readText[25]) ? "0" : readText[25]);
                        }
                        if (readText.Length >= 14)
                        {
                            existingProduct.Name = readText[13];
                        }
                        if (readText.Length >= 42)
                        {
                            ProductFamilyImage = (string.IsNullOrEmpty(readText[41]) ? "" : readText[41]);
                        }
                        if (readText.Length >= 43)
                        {
                            ItemImage = (string.IsNullOrEmpty(readText[42]) ? "" : readText[42]);
                        }

                        existingProduct.Depth = 1;
                        existingProduct.UOMId = 1;
                        existingProduct.Kit = false;
                        existingProduct.TaxID = TaxID;
                        existingProduct.UOMId = UOMId;
                        existingProduct.IsActive = true;
                        existingProduct.IsDeleted = false;
                        existingProduct.PercentMargin = 0;
                        existingProduct.TenantId = tenantId;
                        existingProduct.DimensionUOMId = UOMId;
                        existingProduct.WeightGroupId = weightGroupId;
                        existingProduct.CreatedBy = userId ?? adminUserId;
                        existingProduct.BarCode = existingProduct.SKUCode;
                        existingProduct.LotOptionCodeId = productLotOptionId;
                        existingProduct.ProductGroupId = group.ProductGroupId;
                        existingProduct.DepartmentId = department.DepartmentId;
                        existingProduct.LotProcessTypeCodeId = productLotProcessId;
                        existingProduct.BarCode = (string.IsNullOrEmpty(scanSourceItemNumber) ? "0" : scanSourceItemNumber);

                        List<string> path1 = DownloadImage(ProductFamilyImage, scanSourceItemNumber, tenantId, userId ?? adminUserId, ItemImage);
                        existingProduct.ProductFiles = new List<ProductFiles>();
                        foreach (var filepath in path1)
                        {
                            existingProduct.ProductFiles.Add(new ProductFiles
                            {
                                ProductId = existingProduct.ProductId,
                                FilePath = filepath,
                                TenantId = tenantId,
                                CreatedBy = userId ?? adminUserId,
                                DateCreated = DateTime.UtcNow
                            });
                        }

                        if (addRecord)
                        {
                            context.ProductMaster.Add(existingProduct);
                            context.Entry(existingProduct).State = EntityState.Added;
                        }
                        else
                        {
                            context.Entry(existingProduct).State = EntityState.Modified;
                        }

                        counter++;
                        if (counter % 50 == 0)
                        {
                            context.SaveChanges();
                            context.Dispose();
                            context = new ApplicationContext();
                        }

                        //streamreader.Close();
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception e)
            {
                return "Could not import file data, exception message : " + e.Message;
            }

            return $"Product details imported successfully. Added : { addedProducts }; Updated { updatedProducts }";
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
                        adminUser = new AuthUser()
                        {
                            UserId = 1
                        };
                        context.AuthUsers.Add(adminUser);
                        context.SaveChanges();
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
                            Weight = 0,
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
                            UOMId = 1
                        };
                    }
                    UOMId = UoMId.UOMId;

                    var TAXId = context.GlobalTax.FirstOrDefault();
                    if (TAXId == null)
                    {
                        TAXId = new GlobalTax()
                        {
                            TaxID = 1
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
                            }
                            else
                            {
                                updatedProducts++;
                            }

                            existingProduct.SKUCode = productCode;

                            if (1 < values.Length)
                            {
                                existingProduct.Name = values[0];
                            }
                            if (2 < values.Length)
                            {
                                existingProduct.Description = values[2];
                            }
                            if (3 < values.Length)
                            {
                                existingProduct.SellPrice = string.IsNullOrEmpty(values[3]) ? (decimal?)null : decimal.Parse(values[3]);
                            }


                            existingProduct.DateCreated = DateTime.UtcNow;
                            existingProduct.ProdStartDate = DateTime.UtcNow;
                            existingProduct.Depth = 1;
                            existingProduct.UOMId = 1;
                            existingProduct.Width = 1;
                            existingProduct.Height = 1;
                            existingProduct.Kit = false;
                            existingProduct.UOMId = UOMId;
                            existingProduct.TaxID = TaxID;
                            existingProduct.IsActive = true;
                            existingProduct.IsDeleted = false;
                            existingProduct.PercentMargin = 0;
                            existingProduct.TenantId = tenantId;
                            existingProduct.DimensionUOMId = UOMId;
                            existingProduct.DepartmentId = departmentId;
                            existingProduct.WeightGroupId = weightGroupId;
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

                            if (lineNumber % 50 == 0)
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
                }
            }
            catch (Exception e)
            {
                return "Import Failed : " + e.Message + "Occurred in line :@ " + currentLine;
            }

            return $"Product details imported successfully. Added : { addedProducts }; Updated { updatedProducts }; Faulty Lines { faultyLines }";
        }
        public string ImportProducts(string importPath, string groupName, int tenantId, int warehouseId, ApplicationContext context = null, int? userId = null)
        {
            if (context == null)
            {
                context = new ApplicationContext();
            }

            var adminUserId = context.AuthUsers.First(m => m.UserName.Equals("Admin")).UserId;

            var currentLine = "";//To debug issue
            var addedProducts = 0;
            var updatedProducts = 0;

            try
            {
                // department on 
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

                var group = context.ProductGroups.FirstOrDefault(m => m.ProductGroup.Equals(groupName));
                if (group == null)
                {
                    group = new ProductGroups()
                    {
                        ProductGroup = groupName,
                        CreatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        IsActive = true,
                        TenentId = tenantId
                    };
                    context.ProductGroups.Add(group);
                    context.SaveChanges();
                }

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

                var weightGroup = context.GlobalWeightGroups.FirstOrDefault();
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
                if (headers.Count > 9)
                {
                    return $"File headers mismatch! Please add required headers";
                }

                if (!headers.Contains("product code") || !headers.Contains("manufacturer code") || !headers.Contains("description") || !headers.Contains("description 2") &&
                    (!headers.Contains("inventory") || !headers.Contains("base unit of measure") || !headers.Contains("buy price")) || !headers.Contains("sell price")
                     || !headers.Contains("preferred vendor no"))
                {
                    return $"File headers mismatch! Please add required headers";
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
                        var line = reader.ReadLine();
                        lineNumber++;
                        var values = GetCsvLineContents(line);
                        if (lineNumber == 1 || line == null)
                        {
                            continue;
                        }
                        if (values.Length < 2)
                        {
                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : No product code and product name found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(values[0]))
                        {
                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : No product code found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;
                        }
                        if (string.IsNullOrEmpty(values[1]))
                        {
                            if (count >= 50) { return recorednotmatched; }
                            recorednotmatched += "Import Failed : No product name found on line :@ " + lineNumber + "<br/> ";
                            count++;
                            continue;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(recorednotmatched))
                {
                    return recorednotmatched;
                }
                else
                {
                    lineNumber = 0;
                    var isStockLevelSheet = false;
                    using (var fs = File.OpenRead(importPath))
                    using (var reader = new StreamReader(fs))
                    {
                        while (!reader.EndOfStream)
                        {
                            lineNumber++;
                            var line = reader.ReadLine();
                            if ((line == null || lineNumber == 1))
                            {
                                continue;
                            }
                            if (line.Contains("MinStockLevel"))
                            {
                                isStockLevelSheet = true;
                                continue;
                            }

                            currentLine = line;
                            var values = GetCsvLineContents(line);
                            var productCode = values[0];
                            var sellPrice = values[7];
                            var inventory = "";
                            var description = "";
                            var existingProduct = context.ProductMaster.FirstOrDefault(m => m.SKUCode == productCode);
                            var addRecord = false;
                            if (existingProduct == null)
                            {
                                addRecord = true;
                                addedProducts++;
                                existingProduct = new ProductMaster();
                            }
                            else
                            {
                                updatedProducts++;
                            }

                            existingProduct.SKUCode = productCode;
                            if (1 < values.Length)
                            {
                                existingProduct.ManufacturerPartNo = isStockLevelSheet ? values[1] : values[1];
                            }
                            if (2 < values.Length)
                            {
                                description = existingProduct.Description = isStockLevelSheet ? null : values[2];
                            }
                            if (3 < values.Length)
                            {
                                existingProduct.Name = isStockLevelSheet ? values[3] : values[3];
                            }
                            if (4 < values.Length)
                            {
                                inventory = values[4];
                            }
                            if (6 < values.Length)
                            {
                                existingProduct.BuyPrice = isStockLevelSheet ? null : string.IsNullOrEmpty(values[6]) ? (decimal?)null : decimal.Parse(values[6]);
                            }
                            if (7 < values.Length)
                            {
                                existingProduct.SellPrice = isStockLevelSheet ? null : string.IsNullOrEmpty(values[7]) ? (decimal?)null : decimal.Parse(values[7]);
                            }

                            existingProduct.DateCreated = DateTime.UtcNow;
                            existingProduct.ProdStartDate = DateTime.UtcNow;
                            existingProduct.IsActive = true;
                            existingProduct.TenantId = tenantId;
                            existingProduct.IsDeleted = false;
                            existingProduct.CreatedBy = userId ?? adminUserId;
                            existingProduct.Description = description;
                            existingProduct.Name = description;
                            existingProduct.BarCode = existingProduct.SKUCode;
                            existingProduct.UOMId = context.GlobalUOM.FirstOrDefault()?.UOMId ?? 1;
                            existingProduct.DimensionUOMId = context.GlobalUOM.FirstOrDefault()?.UOMId ?? 1;
                            existingProduct.ProductGroup = group;
                            existingProduct.TenantDepartment = department;
                            existingProduct.TaxID = context.GlobalTax.First(m => m.TaxName.Contains("Standard")).TaxID;
                            existingProduct.ProductLotOptionsCodes = productLotOption;
                            existingProduct.ProductLotProcessTypeCodes = productLotProcess;
                            existingProduct.GlobalWeightGroups = weightGroup;

                            Locations currentLocation = null;

                            if (isStockLevelSheet)
                            {
                                var locationCode = "";
                                if (2 < values.Length)
                                {
                                    locationCode = values[2];
                                }
                                var productLocation = context.Locations.FirstOrDefault(m => m.LocationCode.Equals(locationCode));
                                if (productLocation == null)
                                {
                                    productLocation = new Locations()
                                    {
                                        WarehouseId = warehouseId,
                                        AllowPick = true,
                                        AllowPutAway = true,
                                        AllowReplenish = true,
                                        CreatedBy = userId ?? adminUserId,
                                        DateCreated = DateTime.UtcNow,
                                        LocationCode = locationCode,
                                        TenentId = tenantId,
                                        LocationGroupId = context.LocationGroups.First().LocationGroupId,
                                        UOMId = context.GlobalUOM.FirstOrDefault()?.UOMId ?? 1,
                                        DimensionUOMId = context.GlobalUOM.FirstOrDefault()?.UOMId ?? 1,
                                        LocationName = existingProduct.SKUCode,
                                        LocationTypeId = context.LocationTypes.FirstOrDefault()?.LocationTypeId ?? 1,
                                    };

                                }
                                currentLocation = productLocation;
                                existingProduct.ProductLocationsMap.Add(new ProductLocations()
                                {
                                    ProductMaster = existingProduct,
                                    CreatedBy = userId ?? adminUserId,
                                    DateCreated = DateTime.UtcNow,
                                    IsActive = true,
                                    Locations = productLocation,
                                    TenantId = tenantId
                                });
                            }
                            if (context.InventoryStocks.Any())
                            {
                                existingProduct.InventoryStocks.Add(new InventoryStock()
                                {
                                    WarehouseId = warehouseId,
                                    TenantId = tenantId,
                                    InStock = decimal.Parse(inventory),
                                    DateCreated = DateTime.UtcNow,
                                    IsActive = true,
                                    CreatedBy = userId ?? adminUserId
                                });
                            }
                            else
                            {
                                existingProduct.InventoryStocks.Add(new InventoryStock()
                                {
                                    WarehouseId = warehouseId,
                                    TenantId = tenantId,
                                    InStock = decimal.Parse(inventory),
                                    DateCreated = DateTime.UtcNow,
                                    IsActive = true,
                                    CreatedBy = userId ?? adminUserId
                                });
                            }

                            if (context.InventoryTransactions.Any())
                            {
                                existingProduct.InventoryTransactions.Add(new InventoryTransaction()
                                {
                                    WarehouseId = warehouseId,
                                    TenentId = tenantId,
                                    DateCreated = DateTime.UtcNow,
                                    IsActive = true,
                                    CreatedBy = userId ?? adminUserId,
                                    Location = currentLocation,
                                    Quantity = decimal.Parse(inventory),
                                    LastQty = context.InventoryStocks.FirstOrDefault(x => x.ProductId == existingProduct.ProductId && x.TenantId == tenantId && x.WarehouseId == warehouseId)?.InStock ?? 0,
                                    IsCurrentLocation = true,
                                    InventoryTransactionTypeId = context.InventoryTransactionTypes.First().InventoryTransactionTypeId
                                });
                            }
                            else
                            {
                                existingProduct.InventoryTransactions.Add(new InventoryTransaction()
                                {
                                    WarehouseId = warehouseId,
                                    TenentId = tenantId,
                                    DateCreated = DateTime.UtcNow,
                                    IsActive = true,
                                    CreatedBy = userId ?? adminUserId,
                                    Location = currentLocation,
                                    Quantity = decimal.Parse(inventory),
                                    LastQty = context.InventoryStocks.FirstOrDefault(x => x.ProductId == existingProduct.ProductId && x.TenantId == tenantId && x.WarehouseId == warehouseId)?.InStock ?? 0,
                                    IsCurrentLocation = true,
                                    InventoryTransactionTypeId = context.InventoryTransactionTypes.First().InventoryTransactionTypeId
                                });
                            }

                            if (addRecord)
                            {
                                department.Products.Add(existingProduct);
                                context.Entry(department).State = EntityState.Modified;
                            }
                            else
                            {
                                context.Entry(existingProduct).State = EntityState.Modified;
                            }
                        }
                    }
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {

                return "Import Failed : " + ex.Message + "Occurred in line :@ " + currentLine;
            }

            return $"Product details imported successfully. Added : { addedProducts }; Updated { updatedProducts }";
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
                            catch (Exception)
                            {

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
                                        productMaster.Kit = false;
                                        productMaster.IsActive = true;
                                        productMaster.ProdStartDate = DateTime.UtcNow;
                                        productMaster.Discontinued = false;
                                        productMaster.DepartmentId = 1;
                                        productMaster.ProcessByCase = false;
                                        productMaster.ProcessByPallet = false;
                                        productMaster.IsStockItem = false;
                                        productMaster.IsRawMaterial = false;
                                        productMaster.TenantId = tenantId;
                                        productMaster.BestSellerProduct = false;
                                        productMaster.TopProduct = false;
                                        productMaster.SpecialProduct = false;
                                        productMaster.OnSaleProduct = false;
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
                catch (Exception)
                {
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
                        //SaveProductFile((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"), productId, tenantId, userId);
                    }
                    catch (Exception)
                    {
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
                        //SaveProductFile((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"), productId, tenantId, userId);
                    }
                    catch (Exception)
                    {
                    }

                }

            }
            //if (productMedia.Count > 0)
            //{

            //    foreach (var item in productMedia)
            //    {
            //        i++;
            //        if (RemoteFileExists(item.URL))
            //        {
            //            if (!Directory.Exists(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString())))
            //                Directory.CreateDirectory(HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString()));
            //            string resFileName = HttpContext.Current.Server.MapPath(UploadDirectory + productId.ToString() + @"/" + "Product" + i + ".jpg");
            //            try
            //            {
            //                WebClient webClient = new WebClient();
            //                webClient.DownloadFile(item.URL, resFileName);
            //                values.Add((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"));
            //                //SaveProductFile((UploadDirectory.Remove(0, 1) + productId.ToString() + @"/" + "Product" + i + ".jpg"), productId, tenantId, userId);
            //            }
            //            catch (Exception ex)
            //            {


            //            }
            //        }

            //    }

            //}


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
            catch (Exception)
            {

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
                string url = PrestashopUrl + "products/?filter[date_upd]=>[" + (date.HasValue ? date.Value.ToString() : "") + "]&date=1&display=full&output_format=JSON";
                if (Id.HasValue)
                {
                    url = PrestashopUrl + "products/?filter[id]=[" + Id + "]&display=full&output_format=JSON";
                }
                List<PrestaShopProductDetailViewModel> productSearch = new List<PrestaShopProductDetailViewModel>();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) || result == "[]")
                    {
                        return ProductIds;
                    }
                    var JsonObject = JObject.Parse(result).SelectToken("products").ToString();
                    productSearch = JsonConvert.DeserializeObject<List<PrestaShopProductDetailViewModel>>(JsonObject);

                }
                #endregion
                if (productSearch.Count > 0)
                {
                    foreach (var item in productSearch)
                    {
                        var productMaster = context.ProductMaster.FirstOrDefault(u => u.SKUCode == item.reference.Trim() && u.IsDeleted != true);
                        if (productMaster == null)
                        {
                            productMaster = new ProductMaster();
                            productMaster.DateCreated = DateTime.UtcNow;
                            productMaster.CreatedBy = UserId;
                            productMaster.SKUCode = string.IsNullOrEmpty(item.reference) ? "SkuPresta" : item.reference.Trim();
                            productMaster.ManufacturerPartNo = item.manufacturer_name;

                            productMaster.TaxID = 1;
                            productMaster.Name = string.IsNullOrEmpty(item.name) ? "PrestaShopProduct" : item.name;
                            //productMaster.CommodityCode = item.CommodityImportCodeNumber;
                            productMaster.Description = GetPlainTextFromHtml(item.description);
                            productMaster.UOMId = 1;
                            productMaster.Serialisable = false;

                            productMaster.LotOption = false;
                            productMaster.LotOptionCodeId = 1;
                            productMaster.LotProcessTypeCodeId = 1;
                            productMaster.BarCode = string.IsNullOrEmpty(item.reference) ? "SkuPresta" : item.reference.Trim(); ;
                            //productMaster.PackSize = 0;
                            productMaster.Height = item.height ?? 0;
                            productMaster.Width = item.width ?? 0;
                            productMaster.Depth = item.depth ?? 0;
                            productMaster.Weight = item.weight ?? 0;
                            productMaster.TaxID = 1;
                            productMaster.WeightGroupId = 1;
                            productMaster.PercentMargin = 0;
                            productMaster.Kit = false;
                            productMaster.IsActive = true;
                            productMaster.ProdStartDate = DateTime.UtcNow;
                            productMaster.Discontinued = false;
                            productMaster.DepartmentId = 1;
                            productMaster.ProcessByCase = false;
                            productMaster.ProcessByPallet = false;
                            productMaster.IsStockItem = false;
                            productMaster.IsRawMaterial = false;
                            productMaster.TenantId = TenantId;
                            productMaster.BestSellerProduct = false;
                            productMaster.TopProduct = false;
                            productMaster.SpecialProduct = false;
                            productMaster.OnSaleProduct = false;
                            productMaster.ProductGroupId = 1;
                            productMaster.BuyPrice = item.price;
                            //productMaster.CountryOfOrigion = productDetail.CountryofOrigin;
                            context.Entry(productMaster).State = EntityState.Added;
                            context.SaveChanges();
                            ProductIds.Add(productMaster.ProductId);

                        }
                        else
                        {
                            ProductIds.Add(productMaster.ProductId);
                            //productMaster.DateUpdated = DateTime.UtcNow;
                            //productMaster.UpdatedBy = UserId;
                            //productMaster.SKUCode = item.manufacturer_name;
                            //productMaster.Name = item.name?.FirstOrDefault()?.value;
                            //productMaster.Description = GetPlainTextFromHtml(item.description?.FirstOrDefault()?.value);
                            //productMaster.Height = item.height ?? 0;
                            //productMaster.Width = item.width ?? 0;
                            //productMaster.Depth = item.depth ?? 0;
                            //productMaster.Weight = item.weight ?? 0;
                            //productMaster.PrestaShopProductId = item.id;
                        }




                    }
                }
                return ProductIds;
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        public List<string> GetPrestaShopAddress(int? id_customer, DateTime? date, int tenantId, int UserId, int accountId, int DeliveryAddressId, int InvoiceAdressId, string PrestashopUrl, string PrestashopKey)
        {
            var context = new ApplicationContext();
            List<string> accountaddress = new List<string>();
            try
            {
                #region ApiCall

                string url = PrestashopUrl + "addresses/?filter[date_upd]=>[" + date + "]&date=1&display=full&output_format=JSON";
                if (id_customer.HasValue)
                {
                    url = PrestashopUrl + "addresses/?filter[id_customer]=[" + id_customer + "]&display=full&output_format=JSON";
                }
                List<PrestaShopAccountAddressViewModel> accountAddressSearch = new List<PrestaShopAccountAddressViewModel>();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) || result == "[]")
                    {
                        return accountaddress;
                    }
                    var JsonObject = JObject.Parse(result).SelectToken("addresses").ToString();
                    accountAddressSearch = JsonConvert.DeserializeObject<List<PrestaShopAccountAddressViewModel>>(JsonObject);

                }
                #endregion
                foreach (var item in accountAddressSearch)
                {
                    accountaddress.Add(item.company);
                    var currentAddress = context.AccountAddresses.FirstOrDefault(u => u.PrestaShopAddressId == item.id && u.IsDeleted != true);
                    if (currentAddress == null)
                    {

                        currentAddress = new AccountAddresses()
                        {
                            Name = item.firstname + " " + item.lastname,
                            PostCode = item.postcode,
                            AddressLine1 = item.address1,
                            AddressLine2 = item.address2,
                            AddressLine3 = " ",
                            DateCreated = DateTime.UtcNow,
                            IsActive = true,
                            CountryID = context.Tenants.FirstOrDefault(m => m.TenantId == tenantId).CountryID ?? 0,
                            AccountID = accountId,
                            CreatedBy = UserId,
                            PrestaShopAddressId = item.id,

                        };
                    }
                    else
                    {
                        currentAddress.Name = item.firstname + " " + item.lastname;
                        currentAddress.PostCode = item.postcode;
                        currentAddress.AddressLine1 = item.address1;
                        currentAddress.AddressLine2 = item.address2;
                        currentAddress.AddressLine3 = " ";

                    }
                    if ((item.id == DeliveryAddressId || item.id == InvoiceAdressId) && DeliveryAddressId == InvoiceAdressId)
                    {
                        currentAddress.AddTypeShipping = true;
                        currentAddress.AddTypeBilling = true;
                    }
                    else if (item.id == DeliveryAddressId)
                    {
                        currentAddress.AddTypeShipping = true;
                    }
                    else if (item.id == InvoiceAdressId)
                    {
                        currentAddress.AddTypeBilling = true;
                    }

                    context.Entry(currentAddress).State = currentAddress.AddressID > 0 ? EntityState.Modified : EntityState.Added;
                    context.SaveChanges();
                }

            }


            catch (Exception ex)
            {
                throw ex;

            }
            return accountaddress;

        }
        public List<int> GetPrestaShopAccount(int? Id, DateTime? date, int tenantId, int UserId, int DeliveryAddressId, int InvoiceAdressId, string PrestashopUrl, string PrestashopKey)
        {
            List<int> AccountIds = new List<int>();
            var context = new ApplicationContext();

            try
            {
                #region apicall
                string url = PrestashopUrl + "customers/?filter[date_upd]=>[" + (date.HasValue ? date.Value.ToString() : "") + "]&date=1&display=full&output_format=JSON";
                if (Id.HasValue)
                {
                    url = PrestashopUrl + "customers/?filter[id]=[" + Id + "]&display=full&output_format=JSON";
                }
                List<PrestaShopAccountViewModel> accountSearch = new List<PrestaShopAccountViewModel>();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) || result == "[]")
                    {
                        return AccountIds;
                    }
                    var JsonObject = JObject.Parse(result).SelectToken("customers").ToString();
                    accountSearch = JsonConvert.DeserializeObject<List<PrestaShopAccountViewModel>>(JsonObject);

                }
                #endregion
                foreach (var item in accountSearch)
                {
                    var account = context.Account.FirstOrDefault(u => u.AccountCode.Equals(item.secure_key, StringComparison.CurrentCultureIgnoreCase) && u.IsDeleted != true);
                    if (account == null)
                    {
                        account = new Account();
                        account.CompanyName = string.IsNullOrEmpty(item.company) ? "S" : item.company;
                        account.AccountCode = item.secure_key;
                        account.website = item.website;
                        account.AccountStatusID = 1;
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
                        account.CompanyName = string.IsNullOrEmpty(item.company) ? "S" : item.company;
                        account.AccountCode = item.secure_key;
                        account.website = item.website;
                        account.DateUpdated = DateTime.UtcNow;
                        account.UpdatedBy = UserId;
                        context.Entry(account).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    var accountadress = GetPrestaShopAddress(item.Id, null, tenantId, UserId, account.AccountID, DeliveryAddressId, InvoiceAdressId, PrestashopUrl, PrestashopKey);
                    if (accountadress.Count > 0)
                    {
                        var updateaccount = context.Account.FirstOrDefault(u => u.AccountID == account.AccountID);
                        if (updateaccount != null)
                        {
                            updateaccount.CompanyName = string.IsNullOrEmpty(accountadress.FirstOrDefault()) ? "S" : accountadress.FirstOrDefault();
                            context.Entry(updateaccount).State = EntityState.Modified;
                        }

                    }
                    var currentContact = context.AccountContacts.FirstOrDefault(u => u.AccountID == account.AccountID && u.IsDeleted != true);
                    if (currentContact == null)
                    {
                        currentContact = new AccountContacts()
                        {
                            ContactName = item.firstname + " " + item.lastname,
                            ConTypeInvoices = true,
                            ContactEmail = item.email,
                            //TenantContactPhone = item.x,
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
                        currentContact.ContactName = item.firstname + " " + item.lastname;
                        currentContact.ContactEmail = item.email;
                        currentContact.DateUpdated = DateTime.UtcNow;
                        currentContact.UpdatedBy = UserId;
                        context.Entry(currentContact).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                    AccountIds.Add(account.AccountID);
                }

                return AccountIds;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<bool> GetPrestaShopOrdersSync(int tenantId, int warehouseId, string PrestashopUrl, string PrestashopKey, int SiteId)
        {
            DateTime requestTime = DateTime.UtcNow;
            WebResponse httpResponse = null;
            DateTime responseTime = DateTime.UtcNow;
            string url = "";
            try
            {

                var _currentDbContext = new ApplicationContext();
                var dates = DateTime.UtcNow.ToString("yyyy-MM-dd-HH:mm:ss");
                var GetSyncRecored = _currentDbContext.TenantWebsitesSyncLog.OrderByDescending(u => u.RequestTime).FirstOrDefault();
                if (GetSyncRecored != null)
                {
                    dates = GetSyncRecored.RequestTime.ToString("yyyy-MM-dd-HH:mm:ss");
                }

                url = PrestashopUrl + "orders?filter[date_upd]=>[" + dates + "]&date=1&display=full&output_format=JSON";
                List<PrestaShopOrderViewModel> orderSearch = new List<PrestaShopOrderViewModel>();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");
                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                requestTime = DateTime.UtcNow;
                httpResponse = httpWebRequest.GetResponse();
                responseTime = DateTime.UtcNow;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) || result == "[]")
                    {
                        CreateWebSiteSyncLog(requestTime, url, 1, true, 0, responseTime, SiteId);
                        return false;
                    }
                    var JsonObject = JObject.Parse(result).SelectToken("orders").ToString();
                    orderSearch = JsonConvert.DeserializeObject<List<PrestaShopOrderViewModel>>(JsonObject);
                    CreateWebSiteSyncLog(requestTime, url, 1, true, orderSearch.Count, responseTime, SiteId);
                }
                foreach (var item in orderSearch)
                {
                    var order = _currentDbContext.Order.FirstOrDefault(u => u.PrestaShopOrderId == item.id && u.SiteID == SiteId && u.IsDeleted != true);
                    if (order == null)
                    {
                        order = new Order();
                        order.OrderNumber = GenerateNextOrderNumber(InventoryTransactionTypeEnum.SalesOrder, tenantId);
                        var duplicateOrder = _currentDbContext.Order.FirstOrDefault(m => m.OrderNumber.Equals(order.OrderNumber, StringComparison.CurrentCultureIgnoreCase) && m.IsDeleted != true);
                        if (duplicateOrder != null)
                        {
                            throw new Exception($"Order Number {order.OrderNumber} already associated with another Order. Please regenerate order number.", new Exception("Duplicate Order Number"));
                        }

                        order.IssueDate = DateTime.UtcNow;
                        order.OrderStatusID = (int)OrderStatusEnum.Active;
                        order.DateCreated = DateTime.UtcNow;
                        order.DateUpdated = DateTime.UtcNow;
                        order.TenentId = tenantId;
                        order.CreatedBy = 1;
                        order.UpdatedBy = 1;
                        order.WarehouseId = warehouseId;
                        var accounts = GetPrestaShopAccount(item.id_customer, null, tenantId, 1, item.id_address_delivery, item.id_address_invoice, PrestashopUrl, PrestashopKey);
                        var accountID = accounts?.FirstOrDefault() ?? 1;
                        if (accountID > 0)
                        {
                            var account = _currentDbContext.Account.Find(accountID);
                            if (account != null)
                            {
                                order.AccountCurrencyID = account.CurrencyID;

                            }
                            order.AccountID = accountID;
                            var accountAddress = GetAccountAddressesByPrestaShopAddressId(item.id_address_delivery);
                            if (accountAddress != null)
                            {
                                order.ShipmentAccountAddressId = accountAddress.AddressID;
                                order.ShipmentAddressLine1 = accountAddress.AddressLine1;
                                order.ShipmentAddressLine2 = accountAddress.AddressLine2;
                                order.ShipmentAddressLine3 = accountAddress.AddressLine3;
                                order.ShipmentAddressLine4 = accountAddress.AddressLine4;
                                order.ShipmentAddressPostcode = accountAddress.PostCode;

                            }



                        }

                        order.OrderStatusID = _currentDbContext.OrderStatus.First(a => a.OrderStatusID == (int)OrderStatusEnum.Active).OrderStatusID;
                    }
                    else
                    {
                        order.DateUpdated = DateTime.UtcNow;
                        order.TenentId = tenantId;
                        order.UpdatedBy = 1;
                        order.WarehouseId = warehouseId;

                    }

                    order.InventoryTransactionTypeId = (int)InventoryTransactionTypeEnum.SalesOrder;
                    _currentDbContext.Entry(order).State = order.OrderID > 0 ? EntityState.Modified : EntityState.Added;
                    decimal? ordTotal = 0;
                    foreach (var order_row in item.associations.order_rows)
                    {
                        var orderDetail = _currentDbContext.OrderDetail.FirstOrDefault(u => u.OrderID == order.OrderID);
                        if (orderDetail == null)
                        {
                            orderDetail = new OrderDetail();
                            orderDetail.DateCreated = DateTime.UtcNow;
                            orderDetail.CreatedBy = 1;
                            var Product = GetPrestaShopProducts(order_row.product_id, null, order_row.product_reference, tenantId, 1, PrestashopUrl, PrestashopKey);
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
                        orderDetail.Qty = order_row.product_quantity;
                        orderDetail.Price = order_row.unit_price_tax_incl;
                        orderDetail.WarehouseId = warehouseId;
                        orderDetail.TotalAmount = (order_row.unit_price_tax_incl * order_row.product_quantity);
                        if (order.OrderID <= 0)
                        {
                            order.OrderDetails.Add(orderDetail);
                        }
                        else
                        {
                            _currentDbContext.Entry(orderDetail).State = EntityState.Modified;
                        }

                        ordTotal = ordTotal + ((order_row.unit_price_tax_incl * order_row.product_quantity));

                    }
                    order.OrderTotal = (decimal)ordTotal;
                    order.OrderCost = (decimal)ordTotal;
                    order.PrestaShopOrderId = item.id;
                    order.SiteID = SiteId;
                    _currentDbContext.SaveChanges();

                }


            }
            catch (Exception ex)
            {
                CreateWebSiteSyncLog(requestTime, url, 0, false, 0, responseTime, SiteId);
                throw ex;
            }
            return true;
        }
        public AccountAddresses GetAccountAddressesByPrestaShopAddressId(int id)
        {
            var _currentdbContext = new ApplicationContext();

            return _currentdbContext.AccountAddresses.FirstOrDefault(u => u.PrestaShopAddressId == id);
        }
        public async Task<bool> PrestaShopStockSync(int tenantId, int warehouseId, string PrestashopUrl, string PrestashopKey, int SiteId)
        {
            DateTime requestTime = DateTime.UtcNow;
            WebResponse httpResponse = null;
            DateTime responseTime = DateTime.UtcNow;
            string url = "";
            try
            {

                var _currentDbContext = new ApplicationContext();
                url = PrestashopUrl + "products/?display=[id,reference]&output_format=JSON";
                List<PrestaShopProductViewModel> prestashopProducts = new List<PrestaShopProductViewModel>();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Credentials = new NetworkCredential(PrestashopKey, "");

                httpWebRequest.Method = "GET";
                httpWebRequest.ContentType = "application/json";
                requestTime = DateTime.UtcNow;
                httpResponse = await httpWebRequest.GetResponseAsync();
                responseTime = DateTime.UtcNow;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    if (string.IsNullOrEmpty(result) || result == "[]")
                    {
                        CreateWebSiteSyncLog(requestTime, url, 1, true, 0, responseTime, SiteId);
                        return false;
                    }
                    var JsonObject = JObject.Parse(result).SelectToken("products").ToString();
                    prestashopProducts = JsonConvert.DeserializeObject<List<PrestaShopProductViewModel>>(JsonObject);
                    CreateWebSiteSyncLog(requestTime, url, 1, true, prestashopProducts.Count, responseTime, SiteId);
                }
                var prestashopSkucode = prestashopProducts.Select(u => u.reference).ToList();
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
                        stock_available stock_Available = new stock_available();
                        stock_Available.id_product = prestashopProducts.FirstOrDefault(u => u.reference == item.SkuCode).id;
                        stock_Available.out_of_stock = item.InventoryStock == null ? 0 : Convert.ToInt32(Math.Round(item.InventoryStock.Sum(u => u.InStock)));
                        stock_Available.quantity = item.InventoryStock == null ? 0 : Convert.ToInt32(Math.Round(item.InventoryStock.Sum(u => u.Available)));
                        stock_Available.depends_on_stock = 0;
                        stock_Available.id_product_attribute = 0;
                        stock_Available.StockAvailableId = 1;
                        stock_Availables.Add(stock_Available);

                    }
                    if (stock_Availables.Count > 0)
                    {

                        url = PrestashopUrl + "stock_availables";
                        UpdatePrestaShopStock(url, PrestashopKey, stock_Availables);


                    }
                }


            }
            catch (Exception)
            {

                throw;
            }

            return true;


        }

        private string GetPlainTextFromHtml(string htmlString)
        {
            if (!string.IsNullOrEmpty(htmlString))
            {
                string htmlTagPattern = "<.*?>";

                htmlString = Regex.Replace(htmlString, htmlTagPattern, string.Empty);
                htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            }
            // htmlString = htmlString.Replace(" ", string.Empty);

            return htmlString;
        }
        public string GenerateNextOrderNumber(InventoryTransactionTypeEnum type, int tenantId)
        {

            var _currentDbContext = new ApplicationContext();
            var lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == (int)type)
                .OrderByDescending(m => m.OrderNumber)
                .FirstOrDefault();

            var prefix = "ON-";
            switch (type)
            {
                case InventoryTransactionTypeEnum.PurchaseOrder:
                    prefix = "PO-";
                    break;
                case InventoryTransactionTypeEnum.SalesOrder:
                case InventoryTransactionTypeEnum.Proforma:
                case InventoryTransactionTypeEnum.Quotation:
                case InventoryTransactionTypeEnum.Samples:
                    prefix = "SO-";
                    lastOrder = _currentDbContext.Order.Where(p =>
                    p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.SalesOrder ||
                    p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Proforma ||
                    p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Quotation ||
                    p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Samples
                    ).OrderByDescending(m => m.OrderNumber)
                     .FirstOrDefault();
                    break;
                case InventoryTransactionTypeEnum.WorksOrder:
                    prefix = "MO-";
                    break;
                case InventoryTransactionTypeEnum.DirectSales:
                    prefix = "DO-";
                    break;
                case InventoryTransactionTypeEnum.TransferIn:
                case InventoryTransactionTypeEnum.TransferOut:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferIn ||
                    p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.TransferOut && p.OrderNumber.Length == 11)
                    .OrderByDescending(m => m.OrderNumber)
                    .FirstOrDefault();
                    prefix = "TO-";
                    break;
                case InventoryTransactionTypeEnum.Returns:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Returns && p.OrderNumber.Length == 11)
                   .OrderByDescending(m => m.OrderNumber).FirstOrDefault();
                    prefix = "RO-";
                    break;
                case InventoryTransactionTypeEnum.Wastage:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.Wastage && p.OrderNumber.Length == 11)
                  .OrderByDescending(m => m.OrderNumber).FirstOrDefault();
                    prefix = "WO-";
                    break;
                case InventoryTransactionTypeEnum.AdjustmentIn:
                case InventoryTransactionTypeEnum.AdjustmentOut:
                    lastOrder = _currentDbContext.Order.Where(p => p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentIn ||
                     p.InventoryTransactionTypeId == (int)InventoryTransactionTypeEnum.AdjustmentOut && p.OrderNumber.Length == 11)
                     .OrderByDescending(m => m.OrderNumber)
                     .FirstOrDefault();
                    prefix = "AO-";

                    break;
                case InventoryTransactionTypeEnum.Exchange:
                    prefix = "EO-";
                    break;
            }

            if (lastOrder != null)
            {

                var lastNumber = lastOrder.OrderNumber.Replace("PO-", string.Empty);
                lastNumber = lastNumber.Replace("SO-", string.Empty);
                lastNumber = lastNumber.Replace("MO-", string.Empty);
                lastNumber = lastNumber.Replace("TO-", string.Empty);
                lastNumber = lastNumber.Replace("DO-", string.Empty);
                lastNumber = lastNumber.Replace("RO-", string.Empty);
                lastNumber = lastNumber.Replace("WO-", string.Empty);
                lastNumber = lastNumber.Replace("EO-", string.Empty);
                lastNumber = lastNumber.Replace("AO-", string.Empty);
                lastNumber = lastNumber.Replace("ON-", string.Empty);

                int n;
                bool isNumeric = int.TryParse(lastNumber, out n);

                if (isNumeric == true)
                {
                    var lastOrderNumber = (int.Parse(lastNumber) + 1).ToString("00000000");
                    return prefix + lastOrderNumber;
                }
                else
                {
                    return prefix + "00000001";
                }
            }
            else
            {
                return prefix + "00000001";
            }
        }
        public string GenerateNextProductCode(int tenantId)
        {
            var _currentDbContext = new ApplicationContext();
            var tenant = _currentDbContext.Tenants.Find(tenantId);
            if (tenant != null && tenant.ProductCodePrefix != null)
            {
                var product = _currentDbContext.ProductMaster.Where(m => m.SKUCode.Contains(tenant.ProductCodePrefix) && m.TenantId == tenantId).OrderByDescending(m => m.SKUCode).FirstOrDefault();
                if (product != null)
                {
                    int ValidSkuCode = 0;
                    var lastCode = product.SKUCode.Split(new[] { tenant.ProductCodePrefix, tenant.ProductCodePrefix.ToLower() }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                    if (int.TryParse(lastCode, out ValidSkuCode))
                    {
                        if (lastCode != null)
                        {

                            return tenant.ProductCodePrefix + (int.Parse(lastCode) + 1).ToString("00000");
                        }
                    }
                }
                return tenant.ProductCodePrefix + "00001";
            }

            return "ITM-100001";
        }

        public void CreateWebSiteSyncLog(DateTime RequestTime, string requesturl, int ErrorCode, bool synced, int resultCount, DateTime ResponseTime, int SiteId)
        {
            var dbcontext = new ApplicationContext();
            TenantWebsitesSyncLog syncLog = new TenantWebsitesSyncLog();
            syncLog.RequestTime = RequestTime;
            syncLog.RequestUrl = requesturl;
            syncLog.ErrorCode = ErrorCode;
            syncLog.Synced = synced;
            syncLog.ResultCount = resultCount;
            syncLog.ResponseTime = ResponseTime;
            syncLog.SiteID = SiteId;
            dbcontext.TenantWebsitesSyncLog.Add(syncLog);
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
                writer.WriteLine(GenerateXmlPrestaShopStockUpdate(stock_Availables));
                writer.Close();
                rsp = req.GetResponse();
                StreamReader sr = new StreamReader(rsp.GetResponseStream());
                Response = sr.ReadToEnd();
                sr.Close();

            }
            catch (WebException webEx)
            {
                ErrorSignal.FromCurrentContext().Raise(webEx);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);
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
                var globalapis = _currentDbContext.GlobalApis.FirstOrDefault(u => u.ApiTypes == ApiTypes.DPD);
                url = globalapis?.ApiUrl;
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

                throw;
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
                var globalapis = _currentDbContext.GlobalApis.FirstOrDefault(u => u.ApiTypes == ApiTypes.DPD);
                if (globalapis == null || string.IsNullOrEmpty(globalapis.ApiUrl) || string.IsNullOrEmpty(globalapis.ApiKey) || string.IsNullOrEmpty(globalapis.AccountNumber))
                {
                    return "Api Configuration is invalid, Either Api url, Api key or Api account fields are empty";
                }
                if (globalapis.ExpiryDate.HasValue && globalapis.ExpiryDate.Value.Day != DateTime.Today.Day)
                {
                    globalapis.ApiKey = GetDPDGeoSession(globalapis);

                }
                url = globalapis.ApiUrl + "shipment";
                DpdReponseViewModel dpdResponse = new DpdReponseViewModel();
                DpdErrorViewModel errorViewModel = new DpdErrorViewModel();
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("GeoSession", globalapis.ApiKey);
                httpWebRequest.Headers.Add("GeoClient", "account/" + globalapis.AccountNumber);
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

                throw;
            }

            return "Data Posted";

        }

        public string GetDPDGeoSession(GlobalApi globalApi)
        {
            var _currentDbContext = new ApplicationContext();
            string authorization = GetEncodeUserNameBas64(globalApi.UserName, globalApi.Password);
            WebResponse httpResponse = null;
            string url = globalApi?.ApiUrl;
            url = url.Replace("shipping", "user") + "?action=login";
            GeoSession GeoSession = new GeoSession();
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + authorization);
            httpResponse = httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                GeoSession = JsonConvert.DeserializeObject<GeoSession>(result);
            }
            if (GeoSession != null)
            {
                var updateApi = _currentDbContext.GlobalApis.FirstOrDefault(u => u.Id == globalApi.Id);
                if (updateApi != null)
                {
                    updateApi.ExpiryDate = DateTime.Today.AddHours(23);
                    updateApi.ApiKey = GeoSession.data.geoSession;
                    updateApi.UpdatedDate = DateTime.UtcNow;
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
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(singleString);
            return System.Convert.ToBase64String(plainTextBytes);
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

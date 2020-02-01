using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Helpers;
using System.IO;
using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Ganedata.Core.Data.Helpers;

namespace Ganedata.Core.Data.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationContext>
    {
        string adminUserName = "Admin";

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            ContextKey = "Ganedata.Core.Data.ApplicationContext";
        }

        protected override void Seed(ApplicationContext context)
        {
            //if (System.Diagnostics.Debugger.IsAttached == false)
            //{
            //    System.Diagnostics.Debugger.Launch();
            //}

            //Core system Data Seed
            SeedCoreSystem(context);

            //Test Data Seed
            //SeedTest(context);


            //Customer Data Seeds
            // ***** only one customer seed should run at a time of first deployment deployment. *****
            //SeedEESmith(context);
            //SeedGaneIntranet(context);
            //SeedTheGelBottle(context);


            //var maxId = 1001;
            //if (context.Resources.Any())
            //{
            //    maxId = context.Resources.Max(m => m.ResourceId)+1;
            //}
            //// reseed Resources table to start Ids from 1001
            //context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('Resources', RESEED, "+ maxId + ");");
        }

        private void SeedCoreSystem(ApplicationContext context)
        {

            //Add Core Modules
            context.Modules.AddOrUpdate(m => m.Id, new Module()
            {
                Id = 1,
                ModuleName = "Core"
            });
            context.Modules.AddOrUpdate(m => m.Id,
                new Module()
                {
                    Id = 2,
                    ModuleName = "Sales Order"
                });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 3,
                   ModuleName = "Purchase Order"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 4,
                   ModuleName = "Works Order"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 5,
                   ModuleName = "Warehouse"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 6,
                   ModuleName = "Point Of Sale"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 7,
                   ModuleName = "Van Sales"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 8,
                   ModuleName = "Palleting"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 9,
                   ModuleName = "Human Resources"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 10,
                   ModuleName = "Time and Attendance"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 11,
                   ModuleName = "Captive Portal"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 12,
                   ModuleName = "Accounting"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 13,
                   ModuleName = "POD"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 14,
                   ModuleName = "Accounts"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 15,
                   ModuleName = "Products"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 16,
                   ModuleName = "OrdersCore"
               });
            context.Modules.AddOrUpdate(m => m.Id,
               new Module()
               {
                   Id = 17,
                   ModuleName = "Ecommerce"
               });

            //Add AuthActivityGroups
            using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Content/Seed/AuthActivityGroups.csv")))
            using (var reader = new StreamReader(fs))
            {

                //ignore headers 
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) throw new ArgumentNullException(nameof(line));
                    var values = line.Split(',');
                    var m = new AuthActivityGroup()
                    {
                        ActivityGroupId = Convert.ToInt32(values[0]),
                        ActivityGroupName = values[1],
                        ActivityGroupDetail = values[2],
                        ActivityGroupParentId = null,
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt32(values[10]),
                        UpdatedBy = Convert.ToInt32(values[11]),
                        IsActive = true,
                        IsDeleted = false,
                        TenantId = Convert.ToInt32(values[7]),
                        SortOrder = Convert.ToInt32(values[5]),
                        GroupIcon = values[6]
                    };
                    context.AuthActivityGroups.AddOrUpdate
                        (s => s.ActivityGroupId, m);
                }
            }

            // add AuthActivities
            using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Content/Seed/AuthActivities.csv")))
            using (var reader = new StreamReader(fs))
            {
                //ignore headers 
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) throw new ArgumentNullException(nameof(line));
                    var values = line.Split(',');
                    var p = new AuthActivity()
                    {
                        ActivityId = Convert.ToInt32(values[0]),
                        ActivityName = values[1],
                        ActivityController = values[2],
                        ActivityAction = values[3],
                        IsActive = Convert.ToBoolean(Convert.ToInt16(values[4])),
                        RightNav = Convert.ToBoolean(Convert.ToInt16(values[5])),
                        ExcludePermission = Convert.ToBoolean(Convert.ToInt16(values[6])),
                        SuperAdmin = Convert.ToBoolean(Convert.ToInt16(values[7])),
                        SortOrder = Convert.ToInt32(values[8]),
                        ModuleId = Convert.ToInt32(Convert.ToInt16(values[9])),
                        TenantId = Convert.ToInt32(values[10]),
                        DateCreated = DateTime.UtcNow,
                        DateUpdated = DateTime.UtcNow,
                        CreatedBy = Convert.ToInt32(values[13]),
                        UpdatedBy = Convert.ToInt32(values[14]),
                        IsDeleted = Convert.ToBoolean(Convert.ToInt16(values[15])),

                    };
                    context.AuthActivities.AddOrUpdate(m => m.ActivityId, p);
                }
            }

            // add AuthActivitiesGroupMaps
            using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Content/Seed/AuthActivityGroupMaps.csv")))
            using (var reader = new StreamReader(fs))
            {
                //ignore headers 
                string headerLine = reader.ReadLine();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) throw new ArgumentNullException(nameof(line));
                    var values = line.Split(',');
                    context.AuthActivityGroupMaps.AddOrUpdate
                    (m => m.ActivityGroupMapId,
                        new AuthActivityGroupMap
                        {
                            ActivityGroupMapId = Convert.ToInt32(values[0]),
                            ActivityId = Convert.ToInt32(values[1]),
                            ActivityGroupId = Convert.ToInt32(values[2]),
                            TenantId = Convert.ToInt32(values[3]),
                            DateCreated = DateTime.UtcNow,
                            DateUpdated = DateTime.UtcNow,
                            CreatedBy = Convert.ToInt32(values[6]),
                            UpdatedBy = Convert.ToInt32(values[7]),
                            IsDeleted = Convert.ToBoolean(Convert.ToInt16(values[8]))
                        }
                    );
                }
            }

            //Add Countries using csv file
            using (var fs = File.OpenRead(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../Content/Seed/Countries.csv")))
            using (var reader = new StreamReader(fs))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line == null) throw new ArgumentNullException(nameof(line));
                    var values = line.Split(',');
                    context.GlobalCountries.AddOrUpdate(c => new { c.CountryID, c.CountryName }, new GlobalCountry { CountryID = int.Parse(values[0]), CountryName = values[1], CountryCode = values[2] });
                }
            }

            //Add Currencies
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyName, new GlobalCurrency { CurrencyID = 1, CurrencyName = "GBP", Symbol = "£", CountryID = 1 });
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyName, new GlobalCurrency { CurrencyID = 2, CurrencyName = "Euro", Symbol = "€", CountryID = 81 });
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyName, new GlobalCurrency { CurrencyID = 3, CurrencyName = "USD", Symbol = "$", CountryID = 226 });


            // add Inventory Transaction Types.
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 1,
                    InventoryTransactionTypeName = "Goods In / Purchase Order",
                    OrderType = "Purchase Order",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 2,
                    InventoryTransactionTypeName = "Goods Out / Sales Order",
                    OrderType = "Sales Order",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 3,
                    InventoryTransactionTypeName = "Transfer In",
                    OrderType = "Transfer In",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 4,
                    InventoryTransactionTypeName = "Transfer Out",
                    OrderType = "Transfer Out",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 5,
                    InventoryTransactionTypeName = "Allocated",
                    OrderType = "Allocated",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 6,
                    InventoryTransactionTypeName = "Adjustment In",
                    OrderType = "Adjustment In",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 7,
                    InventoryTransactionTypeName = "Adjustment Out",
                    OrderType = "Adjustment Out",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 8,
                    InventoryTransactionTypeName = "Goods Out / Works Order",
                    OrderType = "Works Order",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 9,
                    InventoryTransactionTypeName = "Goods Out / Proforma",
                    OrderType = "Pro-forma",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 10,
                    InventoryTransactionTypeName = "Goods Out / Quote",
                    OrderType = "Quotation",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 11,
                    InventoryTransactionTypeName = "Goods Out / Loan",
                    OrderType = "Loan",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 12,
                    InventoryTransactionTypeName = "Goods In / Return",
                    OrderType = "Return",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 13,
                    InventoryTransactionTypeName = "Samples",
                    OrderType = "Samples",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 14,
                    InventoryTransactionTypeName = "Wastage",
                    OrderType = "Wastage",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 15,
                    InventoryTransactionTypeName = "Direct Sales",
                    OrderType = "Direct Sales",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 16,
                    InventoryTransactionTypeName = "Exchange",
                    OrderType = "Exchange",
                    CreatedBy = 1,
                    IsActive = true
                });
            context.InventoryTransactionTypes.AddOrUpdate(m => m.InventoryTransactionTypeId,
                new InventoryTransactionType
                {
                    InventoryTransactionTypeId = 17,
                    InventoryTransactionTypeName = "Wasted Return",
                    OrderType = "Wasted Return",
                    CreatedBy = 1,
                    IsActive = true
                });

            // Add Order Status
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 1, Status = "Active" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 2, Status = "Complete" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 3, Status = "Hold" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 4, Status = "Pending" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 6, Status = "Scheduled" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 5, Status = "Not Scheduled" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 7, Status = "Reallocation Required" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 8, Status = "Awaiting Authorisation" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 9, Status = "Cancelled" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 10, Status = "BeingPicked" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 11, Status = "AwaitingArrival" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 12, Status = "Approved" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 13, Status = "PostedToAccounts" });
            context.OrderStatus.AddOrUpdate(m => m.OrderStatusID, new OrderStatus { OrderStatusID = 14, Status = "Invoiced" });

            // add account status
            context.GlobalAccountStatus.AddOrUpdate(m => m.AccountStatusID,
                new GlobalAccountStatus { AccountStatusID = 1, AccountStatus = "Active" });
            context.GlobalAccountStatus.AddOrUpdate(m => m.AccountStatusID,
                new GlobalAccountStatus { AccountStatusID = 2, AccountStatus = "Inactive" });
            context.GlobalAccountStatus.AddOrUpdate(m => m.AccountStatusID,
               new GlobalAccountStatus { AccountStatusID = 3, AccountStatus = "On Stop" });

            // Add Global Measurment Types
            context.GlobalUOMTypes.AddOrUpdate(m => m.UOMTypeId,
                new GlobalUOMTypes { UOMTypeId = 1, UOMType = "General" });
            context.GlobalUOMTypes.AddOrUpdate(m => m.UOMTypeId,
                new GlobalUOMTypes { UOMTypeId = 2, UOMType = "Measurement" });

            // Add Global UOM
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 1, UOM = "Each", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 2, UOM = "Ton", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 3, UOM = "Pallet", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 4, UOM = "Pair", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 5, UOM = "Bag", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 6, UOM = "Box", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 7, UOM = "MM", UOMTypeId = 2 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 8, UOM = "CM", UOMTypeId = 2 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 9, UOM = "Inch", UOMTypeId = 2 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 10, UOM = "Case", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 11, UOM = "Meter", UOMTypeId = 2 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 12, UOM = "Kilo", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 13, UOM = "Pound", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 14, UOM = "Gram", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 15, UOM = "Stone", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 16, UOM = "Ounce", UOMTypeId = 1 });
            context.GlobalUOM.AddOrUpdate(m => m.UOMId, new GlobalUOM { UOMId = 17, UOM = "Foot", UOMTypeId = 2 });


            // Lot Option Codes
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId,
                new ProductLotOptionsCodes() { LotOptionCodeId = 1, Description = "None" });
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId,
                new ProductLotOptionsCodes() { LotOptionCodeId = 2, Description = "Display Days Remaining" });
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId,
                new ProductLotOptionsCodes() { LotOptionCodeId = 3, Description = "Display Percent Remaining" });

            // Lot Process Codes
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId,
               new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 1, Description = "None" });
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId,
                new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 2, Description = "Update lot status" });
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId,
                new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 3, Description = "Update lot grade" });

            // Add Weight Groups
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId,
                new GlobalWeightGroups { WeightGroupId = 1, Description = "Light", Weight = 20 });
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId,
               new GlobalWeightGroups { WeightGroupId = 2, Description = "Medium", Weight = 30 });
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId,
               new GlobalWeightGroups { WeightGroupId = 3, Description = "Heavy", Weight = 50 });

            // Add Global Tax
            context.GlobalTax.AddOrUpdate(m => m.TaxID,
                new GlobalTax
                {
                    TaxID = 1,
                    TaxName = "VAT Standard (T1)",
                    TaxDescription = "Applied on Most goods and services in UK",
                    PercentageOfAmount = 20,
                    CountryID = 1
                });
            context.GlobalTax.AddOrUpdate(m => m.TaxID,
                new GlobalTax
                {
                    TaxID = 2,
                    TaxName = "VAT Reduced (T5)",
                    TaxDescription =
                        "Some goods and services, eg children’s car seats and some energy-saving materials in the home",
                    PercentageOfAmount = 5,
                    CountryID = 1
                });
            context.GlobalTax.AddOrUpdate(m => m.TaxID,
                new GlobalTax
                {
                    TaxID = 3,
                    TaxName = "VAT Zero (T0)",
                    TaxDescription = "Zero-rated goods and services, eg most food and children’s clothes",
                    PercentageOfAmount = 0,
                    CountryID = 1
                });
            context.GlobalTax.AddOrUpdate(m => m.TaxID,
                new GlobalTax
                {
                    TaxID = 4,
                    TaxName = "Exampt Transactions (T2)",
                    TaxDescription = "Zero-rated goods and services, eg most food and children’s clothes",
                    PercentageOfAmount = 0,
                    CountryID = 1
                });
            context.GlobalTax.AddOrUpdate(m => m.TaxID,
                new GlobalTax
                {
                    TaxID = 5,
                    TaxName = "Non VAT Transactions (T9)",
                    TaxDescription = "Zero-rated goods and services, eg most food and children’s clothes",
                    PercentageOfAmount = 0,
                    CountryID = 1
                });

            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 1, Name = "Queued" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 2, Name = "Awaiting" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 3, Name = "Accepted" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 4, Name = "Declined" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 5, Name = "Completed" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 5, Name = "FailedToComplete" });
            context.MarketJobStatus.AddOrUpdate(m => m.Id, new MarketJobStatus() { Id = 7, Name = "Cancelled" });

            context.AccountTransactionTypes.AddOrUpdate(m => m.AccountTransactionTypeId, new AccountTransactionType() { AccountTransactionTypeId = 1, Description = "Credit" });
            context.AccountTransactionTypes.AddOrUpdate(m => m.AccountTransactionTypeId, new AccountTransactionType() { AccountTransactionTypeId = 2, Description = "Debit" });
            context.AccountTransactionTypes.AddOrUpdate(m => m.AccountTransactionTypeId, new AccountTransactionType() { AccountTransactionTypeId = 3, Description = "Refund" });
            context.AccountTransactionTypes.AddOrUpdate(m => m.AccountTransactionTypeId, new AccountTransactionType() { AccountTransactionTypeId = 4, Description = "CreditNote" });
            context.AccountTransactionTypes.AddOrUpdate(m => m.AccountTransactionTypeId, new AccountTransactionType() { AccountTransactionTypeId = 5, Description = "Discount" });

            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 1, Description = "Broken" });
            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 2, Description = "Expired" });
            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 3, Description = "Repairable Fault" });

            context.AccountPaymentModes.AddOrUpdate(m => m.AccountPaymentModeId, new AccountPaymentMode() { AccountPaymentModeId = 1, Description = "Cash" });
            context.AccountPaymentModes.AddOrUpdate(m => m.AccountPaymentModeId, new AccountPaymentMode() { AccountPaymentModeId = 2, Description = "Card" });
            context.AccountPaymentModes.AddOrUpdate(m => m.AccountPaymentModeId, new AccountPaymentMode() { AccountPaymentModeId = 3, Description = "Online Transfer" });
            context.AccountPaymentModes.AddOrUpdate(m => m.AccountPaymentModeId, new AccountPaymentMode() { AccountPaymentModeId = 4, Description = "Bank Deposit" });

            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 1, Status = "Active" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 2, Status = "Complete" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 3, Status = "Dispatched" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 4, Status = "Loaded" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 5, Status = "Delivered" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 6, Status = "Invoiced" });
            context.OrderProcessStatuses.AddOrUpdate(m => m.OrderProcessStatusId, new OrderProcessStatus() { OrderProcessStatusId = 7, Status = "Posted to Accounts" });

            context.SaveChanges();

            //// create trigger to delete old entity framework transactions
            context.Database.ExecuteSqlCommand("IF OBJECT_ID('TRG_DeleteOldEntityTransactions') IS NOT NULL DROP TRIGGER TRG_DeleteOldEntityTransactions");
            context.Database.ExecuteSqlCommand("CREATE TRIGGER TRG_DeleteOldEntityTransactions ON __TransactionHistory AFTER INSERT AS BEGIN delete from __TransactionHistory where id in (select top(5) id from __TransactionHistory WITH (NOLOCK) " +
                "where CreationTime < DATEADD(minute, -30, GETDATE()))  END");

            // create trigger to delete old terminal logs
            context.Database.ExecuteSqlCommand("IF OBJECT_ID('TRG_DeleteOldTerminalLogs') IS NOT NULL DROP TRIGGER TRG_DeleteOldTerminalLogs");
            context.Database.ExecuteSqlCommand("CREATE TRIGGER TRG_DeleteOldTerminalLogs ON TerminalsLogs AFTER INSERT AS BEGIN delete from TerminalsLogs where TerminalLogId in (select top(5) TerminalLogId from TerminalsLogs WITH (NOLOCK) " +
                "where DateCreated < DATEADD(DAY, -30, GETDATE())) END");

            // create function for spliting the string in database
            var dropcommandforfucntion = "IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[SplitString]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))DROP FUNCTION[dbo].[SplitString]";
            context.Database.ExecuteSqlCommand(dropcommandforfucntion);
            var createcommandforfunction = "CREATE FUNCTION SplitString (@Input NVARCHAR(MAX)) RETURNS @Output TABLE(Item NVARCHAR(4000)) AS BEGIN DECLARE @StartIndex INT, @EndIndex INT, @Character CHAR(1) = ',' SET @StartIndex = 1" +
                              "IF SUBSTRING(@Input, LEN(@Input) -1, LEN(@Input)) <> @Character BEGIN SET @Input = @Input + @Character END WHILE CHARINDEX(@Character, @Input) > 0 BEGIN SET @EndIndex = CHARINDEX(@Character, @Input)" +
                               "INSERT INTO @Output(Item) SELECT SUBSTRING(@Input, @StartIndex, @EndIndex -1) SET @Input = SUBSTRING(@Input, @EndIndex + 1, LEN(@Input)) END RETURN END";
            context.Database.ExecuteSqlCommand(createcommandforfunction);



            //// create trigger to delete old elamh error logs
            //context.Database.ExecuteSqlCommand("IF OBJECT_ID('TRG_DeleteOldElmahLogs') IS NOT NULL DROP TRIGGER TRG_DeleteOldElmahLogs");
            //context.Database.ExecuteSqlCommand("CREATE TRIGGER TRG_DeleteOldElmahLogs ON ELMAH_Error AFTER INSERT AS BEGIN delete from ELMAH_Error where TimeUtc < DATEADD(DAY, -30, GETDATE()) END");

            //// create trigger to delete old auth user logins and login activities
            //// Alter foreign key constraint to cascade delete before trigger to delete child table data as well       
            //context.Database.ExecuteSqlCommand("ALTER TABLE [dbo].[AuthUserLoginActivities] DROP CONSTRAINT [FK_dbo.AuthUserLoginActivities_dbo.AuthUserLogins_UserLoginId]");
            //context.Database.ExecuteSqlCommand("ALTER TABLE [dbo].[AuthUserLoginActivities]  WITH CHECK ADD  CONSTRAINT [FK_dbo.AuthUserLoginActivities_dbo.AuthUserLogins_UserLoginId] FOREIGN KEY([UserLoginId]) REFERENCES[dbo].[AuthUserLogins]([UserLoginId]) ON DELETE CASCADE");
            //context.Database.ExecuteSqlCommand("ALTER TABLE [dbo].[AuthUserLoginActivities] CHECK CONSTRAINT [FK_dbo.AuthUserLoginActivities_dbo.AuthUserLogins_UserLoginId]");
            //context.Database.ExecuteSqlCommand("IF OBJECT_ID('TRG_DeleteOldUserLogins') IS NOT NULL DROP TRIGGER TRG_DeleteOldUserLogins");
            //context.Database.ExecuteSqlCommand("CREATE TRIGGER TRG_DeleteOldUserLogins ON AuthUserLogins AFTER INSERT AS BEGIN delete from AuthUserLogins where DateLoggedIn < DATEADD(DAY, -90, GETDATE()) END");

            //// create trigger to delete old terminal geo location data
            //context.Database.ExecuteSqlCommand("IF OBJECT_ID('TRG_DeleteOldGeoLocations') IS NOT NULL DROP TRIGGER TRG_DeleteOldGeoLocations");
            //context.Database.ExecuteSqlCommand("CREATE TRIGGER TRG_DeleteOldGeoLocations ON TerminalGeoLocations AFTER INSERT AS BEGIN delete from TerminalGeoLocations where Date < DATEADD(DAY, -90, GETDATE()) END");
        }

        private void SeedEESmith(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantId = 1,
                TenantName = "EE Smith Contracts",
                TenantDayPhone = "0116 270 6946",
                TenantFax = "",
                TenantAddress1 = "25 Morris Rd",
                TenantCity = "Leicester",
                TenantPostalCode = "LE2 6AL",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "EE Smith Contracts").FirstOrDefault().TenantId;
            ////Add User
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = adminUserName,
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = true
                });
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = "Test",
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = false
                });

            // Add Location
            context.TenantWarehouses.AddOrUpdate(m => new { m.WarehouseName, m.TenantId },
                new TenantLocations
                {
                    WarehouseId = 1,
                    TenantId = CurrentTenantId,
                    WarehouseName = "Headquarter",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "25 Morris Rd",
                    PostalCode = "LE2 6AL",
                    AddressLine2 = "",
                    AddressLine3 = "",
                    City = "Leicester"
                });

            context.SaveChanges();

            //Tenant Configuration
            context.TenantConfigs.AddOrUpdate(m => new { m.TenantId }, new TenantConfig()
            {
                TenantId = CurrentTenantId,
                PoReportFooterMsg1 = "No Additional Items To Be Added Without Authorisation",
                EnforceMinimumProductPrice = false,
                AlertMinimumProductPrice = true,
                AlertMinimumPriceMessage = "Selling Price cannot be less than the minimum threshold price.",
                WorksOrderScheduleByAmPm = true,
                WorksOrderScheduleByMarginHours = 2,
                DateCreated = DateTime.UtcNow
            });

            // Add Departments
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Management",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId,
                });
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Sales",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId
                });

            // add Tenant Modules 
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 1,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 9,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 10,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

            //Add TenantEmailTemplateVariables
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "CompanyName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountCode",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountRemittancesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountStatementsContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountInvoicesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountMarketingContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderId",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderStatus",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "BillingAccountToEmail",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderResourceName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderTimeslot",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksTenantName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorkPropertyAddress",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobSubTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksSlaJobPriorityName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksPropertyContactNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ScheduledDate",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
               new TenantEmailTemplateVariable()
               {
                   VariableName = "CustomMessage",
                   TenantId = CurrentTenantId
               });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
               new TenantEmailTemplateVariable()
               {
                   VariableName = "AccountPurchasingContactName",
                   TenantId = CurrentTenantId
               });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "EE Smith",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });
            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "PSV Standards",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });

            context.SaveChanges();

            // add case account for van sales / direct sale
            var defaultCashAccount = new Account()
            {
                CompanyName = "Default Cash Account",
                AccountCode = "Default001",
                CreatedBy = context.AuthUsers.First().UserId,
                DateCreated = DateTime.UtcNow,
                CountryID = tenant.CountryID ?? 0,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = 2,
                TaxID = 4
            };
            context.Account.AddOrUpdate(m => m.CompanyName, defaultCashAccount);
            context.SaveChanges();

            context.VehicleInspectionCheckLists.AddOrUpdate(m => new { m.Name },

                new VehicleInspectionCheckList() { Name = "Exterior Wings & Load Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tactograph Unit", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Rear/Side Lights & Markers", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speed Limiter", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speedometer", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Spray Suppression", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "5th Wheel Couplings Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Lighting", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Floor Covering", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },

                new VehicleInspectionCheckList() { Name = "Steering", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Mirrors & Glass", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Brakes", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Windscreen Wipers/ Washers", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Heating/Ventilation", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Horn", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Excessive Engine Exhaust Smoke", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Warning Lamps", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Lights, Reflectors & Indicators", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "First Aid Kit", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fire Extinguisher", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Doors & Exits", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Interior (Seat belts)", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Exterior", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Emergency Exit Hammer", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Number Plates", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fuel, Oil and Waste Leaks", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tyre & Wheel Fixings", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow }
                );

            //add consignment types
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Standard",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Priority",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Pre Ten",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Collection",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 0,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow

                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Standard",
                    PostageTypeId = 1,
                    PercentageOfPrice = 5,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended",
                    PostageTypeId = 1,
                    PercentageOfPrice = 2,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended Two Years",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 10,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });


            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (var item in exception.EntityValidationErrors)
                {
                    var errors = item.ValidationErrors;
                    foreach (var error in errors)
                        EventLog.WriteEntry("Warehouse Seeding", item.Entry.Entity.ToString() + " > " + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
        }

        private void SeedGaneIntranet(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantId = 1,
                TenantName = "GaneData Ltd.",
                TenantDayPhone = "0333 323 0202",
                TenantFax = "",
                TenantAddress1 = "Airedale House",
                TenantAddress2 = "Clayton Wood Rise",
                TenantCity = "Leeds",
                TenantPostalCode = "LS16 6RF",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "GaneData Ltd.").FirstOrDefault().TenantId;
            ////Add User
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = adminUserName,
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = true
                });
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = "Test",
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = false
                });

            // Add Location
            context.TenantWarehouses.AddOrUpdate(m => new { m.WarehouseName, m.TenantId },
                new TenantLocations
                {
                    WarehouseId = 1,
                    TenantId = CurrentTenantId,
                    WarehouseName = "Head Office",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "Airedale House",
                    AddressLine2 = "Clayton Wood Rise",
                    PostalCode = "LE2 6AL",
                    City = "Leeds"
                });

            context.SaveChanges();

            //Tenant Configuration
            context.TenantConfigs.AddOrUpdate(m => new { m.TenantId }, new TenantConfig()
            {
                TenantId = CurrentTenantId,
                PoReportFooterMsg1 = "No Additional Items To Be Added Without Authorisation",
                EnforceMinimumProductPrice = false,
                AlertMinimumProductPrice = true,
                AlertMinimumPriceMessage = "Selling Price cannot be less than the minimum threshold price.",
                WorksOrderScheduleByAmPm = true,
                WorksOrderScheduleByMarginHours = 2,
                DateCreated = DateTime.UtcNow
            });

            // Add Departments
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Management",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId,
                });
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Sales",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId
                });

            // add Tenant Modules 
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 1,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 9,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 10,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

            //Add TenantEmailTemplateVariables
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "CompanyName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountCode",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountRemittancesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountStatementsContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountInvoicesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountMarketingContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderId",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderStatus",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "BillingAccountToEmail",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderResourceName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderTimeslot",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksTenantName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorkPropertyAddress",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobSubTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksSlaJobPriorityName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksPropertyContactNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ScheduledDate",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "CustomMessage",
                    TenantId = CurrentTenantId
                });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "Gane",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });
            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "PSV Standards",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });

            context.SaveChanges();

            // add case account for van sales / direct sale
            var defaultCashAccount = new Account()
            {
                CompanyName = "Default Cash Account",
                AccountCode = "Default001",
                CreatedBy = context.AuthUsers.First().UserId,
                DateCreated = DateTime.UtcNow,
                CountryID = tenant.CountryID ?? 0,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = 2,
                TaxID = 4
            };
            context.Account.AddOrUpdate(m => m.CompanyName, defaultCashAccount);
            context.SaveChanges();

            context.VehicleInspectionCheckLists.AddOrUpdate(m => new { m.Name },

                new VehicleInspectionCheckList() { Name = "Exterior Wings & Load Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tactograph Unit", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Rear/Side Lights & Markers", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speed Limiter", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speedometer", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Spray Suppression", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "5th Wheel Couplings Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Lighting", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Floor Covering", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Steering", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Mirrors & Glass", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Brakes", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Windscreen Wipers/ Washers", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Heating/Ventilation", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Horn", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Excessive Engine Exhaust Smoke", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Warning Lamps", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Lights, Reflectors & Indicators", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "First Aid Kit", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fire Extinguisher", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Doors & Exits", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Interior (Seat belts)", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Exterior", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Emergency Exit Hammer", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Number Plates", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fuel, Oil and Waste Leaks", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tyre & Wheel Fixings", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow }
                );

            //add consignment types
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Standard",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Priority",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Pre Ten",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Collection",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 0,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow

                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Standard",
                    PostageTypeId = 1,
                    PercentageOfPrice = 5,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended",
                    PostageTypeId = 1,
                    PercentageOfPrice = 2,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended Two Years",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 10,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });


            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (var item in exception.EntityValidationErrors)
                {
                    var errors = item.ValidationErrors;
                    foreach (var error in errors)
                        EventLog.WriteEntry("Warehouse Seeding", item.Entry.Entity.ToString() + " > " + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
        }

        private void SeedTheGelBottle(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantId = 1,
                TenantName = "The GelBottle INC",
                TenantDayPhone = "0333 323 0202",
                TenantFax = "",
                TenantAddress1 = "1, G3 Business Park",
                TenantAddress2 = "Dolphin Rd",
                TenantCity = "Shoreham-by-Sea",
                TenantPostalCode = "BN43 6AN",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "The GelBottle INC").FirstOrDefault().TenantId;
            ////Add User
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = adminUserName,
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = true
                });
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = "Test",
                    UserPassword = GaneStaticAppExtensions.GetMd5("br4PrE"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = false
                });

            // Add Location
            context.TenantWarehouses.AddOrUpdate(m => new { m.WarehouseName, m.TenantId },
                new TenantLocations
                {
                    WarehouseId = 1,
                    TenantId = CurrentTenantId,
                    WarehouseName = "Head Office",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "1, G3 Business Park",
                    AddressLine2 = "Dolphin Rd",
                    PostalCode = "BN43 6AN",
                    City = "Shoreham-by-Sea"
                });

            context.SaveChanges();

            //Tenant Configuration
            context.TenantConfigs.AddOrUpdate(m => new { m.TenantId }, new TenantConfig()
            {
                TenantId = CurrentTenantId,
                PoReportFooterMsg1 = "No Additional Items To Be Added Without Authorisation",
                EnforceMinimumProductPrice = false,
                AlertMinimumProductPrice = true,
                AlertMinimumPriceMessage = "Selling Price cannot be less than the minimum threshold price.",
                WorksOrderScheduleByAmPm = true,
                WorksOrderScheduleByMarginHours = 2,
                DateCreated = DateTime.UtcNow
            });

            // Add Departments
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Management",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId,
                });
            context.TenantDepartments.AddOrUpdate(m => new { m.DepartmentName, m.TenantId },
                new TenantDepartments
                {
                    DepartmentName = "Sales",
                    DateCreated = DateTime.UtcNow,
                    TenantId = CurrentTenantId
                });

            // add Tenant Modules 
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 1,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 9,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = 10,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId },
                new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

            //Add TenantEmailTemplateVariables
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "CompanyName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountCode",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountRemittancesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountStatementsContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountInvoicesContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "AccountMarketingContactName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderId",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "OrderStatus",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "BillingAccountToEmail",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderResourceName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksOrderTimeslot",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksTenantName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorkPropertyAddress",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksJobSubTypeDescription",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksSlaJobPriorityName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "WorksPropertyContactNumber",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ScheduledDate",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "CustomMessage",
                    TenantId = CurrentTenantId
                });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "TheGelBottle",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });
            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "PSV Standards",
                TenantId = CurrentTenantId,
                DateCreated = DateTime.UtcNow
            });

            context.SaveChanges();

            // add case account for van sales / direct sale
            var defaultCashAccount = new Account()
            {
                CompanyName = "Default Cash Account",
                AccountCode = "Default001",
                CreatedBy = context.AuthUsers.First().UserId,
                DateCreated = DateTime.UtcNow,
                CountryID = tenant.CountryID ?? 0,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = 2,
                TaxID = 4
            };
            context.Account.AddOrUpdate(m => m.CompanyName, defaultCashAccount);
            context.SaveChanges();

            context.VehicleInspectionCheckLists.AddOrUpdate(m => new { m.Name },

                new VehicleInspectionCheckList() { Name = "Exterior Wings & Load Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tactograph Unit", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Rear/Side Lights & Markers", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speed Limiter", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Speedometer", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Spray Suppression", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "5th Wheel Couplings Security", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Lighting", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Saloon Floor Covering", VehicleInspectionTypeId = 1, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Steering", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Mirrors & Glass", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Brakes", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Windscreen Wipers/ Washers", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Heating/Ventilation", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Horn", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Excessive Engine Exhaust Smoke", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Warning Lamps", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Lights, Reflectors & Indicators", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "First Aid Kit", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fire Extinguisher", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Doors & Exits", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Interior (Seat belts)", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Body Exterior", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Emergency Exit Hammer", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Number Plates", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Fuel, Oil and Waste Leaks", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow },
                new VehicleInspectionCheckList() { Name = "Tyre & Wheel Fixings", VehicleInspectionTypeId = 2, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow }
                );

            //add consignment types
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "DPD",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "DPD Priority",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "DPD Pre Ten",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.ConsignmentTypes.AddOrUpdate(m => new { m.ConsignmentType, m.TenantId },
                new OrderConsignmentTypes
                {
                    ConsignmentType = "Collection",
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 0,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow

                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Standard",
                    PostageTypeId = 1,
                    PercentageOfPrice = 5,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended",
                    PostageTypeId = 1,
                    PercentageOfPrice = 2,
                    FixedPrice = 0,
                    IsPercent = true,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "Extended Two Years",
                    PostageTypeId = 1,
                    PercentageOfPrice = 0,
                    FixedPrice = 10,
                    IsPercent = false,
                    HotSwap = false,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow
                });


            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException exception)
            {
                foreach (var item in exception.EntityValidationErrors)
                {
                    var errors = item.ValidationErrors;
                    foreach (var error in errors)
                        EventLog.WriteEntry("Warehouse Seeding", item.Entry.Entity.ToString() + " > " + error.PropertyName + ": " + error.ErrorMessage);
                }
            }
        }

    }
}


using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Entities.Helpers;
using System;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

            //Customer Data Seeds
            // ***** only one customer seed should run at a time of first deployment. Should be commented out after first run *****
            //SeedOrderingApp(context);


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
                        ModuleId = (TenantModuleEnum)Convert.ToInt16(values[9]),
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
                    context.GlobalCountries.AddOrUpdate(c => c.CountryID, new GlobalCountry { CountryID = int.Parse(values[0]), CountryName = values[1], CountryCode = values[2] });
                }
            }

            //Add Currencies
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyID, new GlobalCurrency { CurrencyID = 1, CurrencyName = "GBP", Symbol = "£", CountryID = 1 });
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyID, new GlobalCurrency { CurrencyID = 2, CurrencyName = "Euro", Symbol = "€", CountryID = 81 });
            context.GlobalCurrencies.AddOrUpdate(c => c.CurrencyID, new GlobalCurrency { CurrencyID = 3, CurrencyName = "USD", Symbol = "$", CountryID = 226 });

            // Add Global Measurment Types
            context.GlobalUOMTypes.AddOrUpdate(m => m.UOMTypeId, new GlobalUOMTypes { UOMTypeId = 1, UOMType = "General" });
            context.GlobalUOMTypes.AddOrUpdate(m => m.UOMTypeId, new GlobalUOMTypes { UOMTypeId = 2, UOMType = "Measurement" });

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
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId, new ProductLotOptionsCodes() { LotOptionCodeId = 1, Description = "None" });
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId, new ProductLotOptionsCodes() { LotOptionCodeId = 2, Description = "Display Days Remaining" });
            context.ProductLotOptionsCodes.AddOrUpdate(m => m.LotOptionCodeId, new ProductLotOptionsCodes() { LotOptionCodeId = 3, Description = "Display Percent Remaining" });

            // Lot Process Codes
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId, new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 1, Description = "None" });
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId, new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 2, Description = "Update lot status" });
            context.ProductLotProcessTypeCodes.AddOrUpdate(m => m.LotProcessTypeCodeId, new ProductLotProcessTypeCodes() { LotProcessTypeCodeId = 3, Description = "Update lot grade" });

            // Add Weight Groups
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId, new GlobalWeightGroups { WeightGroupId = 1, Description = "Light", Weight = 20 });
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId, new GlobalWeightGroups { WeightGroupId = 2, Description = "Medium", Weight = 30 });
            context.GlobalWeightGroups.AddOrUpdate(m => m.WeightGroupId, new GlobalWeightGroups { WeightGroupId = 3, Description = "Heavy", Weight = 50 });

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
                    TaxName = "Exempt Transactions (T2)",
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

            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 1, Description = "Broken" });
            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 2, Description = "Expired" });
            context.WastageReasons.AddOrUpdate(m => m.Id, new WastageReason() { Id = 3, Description = "Repairable Fault" });

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
                    ModuleId = TenantModuleEnum.Core,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.HumanResources,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.TimeAndAttendance,
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
                AccountStatusID = AccountStatusEnum.Active,
                TaxID = 4,
                OwnerUserId = 1
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

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    ModuleId = TenantModuleEnum.Core,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.HumanResources,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.TimeAndAttendance,
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
                AccountStatusID = AccountStatusEnum.Active,
                TaxID = 4,
                OwnerUserId = 1
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

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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

        private void SeedNghAssetTracking(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantName = "Northampton General Hospital",
                TenantDayPhone = "01604 634700",
                TenantFax = "",
                TenantAddress1 = "Cliftonville",
                TenantAddress2 = "",
                TenantCity = "Northampton",
                TenantPostalCode = "NN1 5BD",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "Northampton General Hospital").FirstOrDefault().TenantId;
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
                    TenantId = CurrentTenantId,
                    WarehouseName = "Head Office",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "Cliftonville",
                    AddressLine2 = "",
                    PostalCode = "NN1 5BD",
                    City = "Northampton"
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
                    ModuleId = TenantModuleEnum.Core,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.HumanResources,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.TimeAndAttendance,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.AssetTracking,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

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
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "UserName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ConfirmationLink",
                    TenantId = CurrentTenantId
                });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "NGH",
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
                CountryID = tenant.CountryID ?? 1,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = AccountStatusEnum.Active,
                TaxID = 4,
                OwnerUserId = 1
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

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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

        private void SeedLink(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantName = "Creative Emporium",
                TenantDayPhone = "0870 750 0057",
                TenantFax = "",
                TenantAddress1 = "4 Whitehall Cross",
                TenantAddress2 = "",
                TenantCity = "Leeds",
                TenantPostalCode = "LS12 5XE",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "Creative Emporium").FirstOrDefault().TenantId;
            ////Add User
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = adminUserName,
                    UserPassword = GaneStaticAppExtensions.GetMd5("123456"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = true
                });
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = "Test",
                    UserPassword = GaneStaticAppExtensions.GetMd5("123456"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = false
                });

            // Add Location
            context.TenantWarehouses.AddOrUpdate(m => new { m.WarehouseName, m.TenantId },
                new TenantLocations
                {
                    TenantId = CurrentTenantId,
                    WarehouseName = "Head Office",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "4 Whitehall Cross",
                    AddressLine2 = "",
                    PostalCode = "LS12 5XE",
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
                    ModuleId = TenantModuleEnum.Core,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.HumanResources,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.TimeAndAttendance,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.AssetTracking,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

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
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "UserName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ConfirmationLink",
                    TenantId = CurrentTenantId
                });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "Creative",
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
                CountryID = tenant.CountryID ?? 1,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = AccountStatusEnum.Active,
                TaxID = 4,
                OwnerUserId = 1
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

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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

        private void SeedOrderingApp(ApplicationContext context)
        {
            //Add Tenant
            var tenant = new Tenant
            {
                TenantName = "Ice Stone",
                TenantDayPhone = "01274 744 826",
                TenantFax = "",
                TenantAddress1 = "Imperial house springmill street",
                TenantAddress2 = "",
                TenantCity = "Bradford",
                TenantPostalCode = "BD5 7HF",
                TenantSubDmoain = "ganedev",
                IsActive = true,
                CurrencyID = 1,
                CountryID = 1,
                ProductCodePrefix = "ITM-1"
            };
            context.Tenants.AddOrUpdate(m => m.TenantName, tenant);


            context.SaveChanges();
            int CurrentTenantId = context.Tenants.Where(x => x.TenantName == "Ice Stone").FirstOrDefault().TenantId;
            ////Add User
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = adminUserName,
                    UserPassword = GaneStaticAppExtensions.GetMd5("123456"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = true
                });
            context.AuthUsers.AddOrUpdate(m => new { m.UserName, m.TenantId },
                new AuthUser
                {
                    UserName = "Test",
                    UserPassword = GaneStaticAppExtensions.GetMd5("123456"),
                    IsActive = true,
                    TenantId = CurrentTenantId,
                    DateCreated = DateTime.UtcNow,
                    SuperUser = false
                });

            // Add Location
            context.TenantWarehouses.AddOrUpdate(m => new { m.WarehouseName, m.TenantId },
                new TenantLocations
                {
                    TenantId = CurrentTenantId,
                    WarehouseName = "Head Office",
                    CountryID = 1,
                    IsActive = true,
                    SortOrder = 1,
                    DateCreated = DateTime.UtcNow,
                    AddressLine1 = "Imperial house springmill street",
                    AddressLine2 = "",
                    PostalCode = "BD5 7HF",
                    City = "Bradford"
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
                    ModuleId = TenantModuleEnum.Core,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.HumanResources,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.TimeAndAttendance,
                    TenantId = CurrentTenantId
                });
            context.TenantModules.AddOrUpdate(m => new { m.ModuleId, m.TenantId },
                new TenantModules()
                {
                    ModuleId = TenantModuleEnum.AssetTracking,
                    TenantId = CurrentTenantId
                });

            // Add Price Groups
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "CASH", Percent = 0, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });
            context.TenantPriceGroups.AddOrUpdate(m => new { m.Name, m.TenantId }, new TenantPriceGroups { Name = "SDIL", Percent = 3, TenantId = CurrentTenantId, DateCreated = DateTime.UtcNow });

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
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "UserName",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "ConfirmationLink",
                    TenantId = CurrentTenantId
                });
            context.TenantEmailTemplateVariables.AddOrUpdate(m => new { m.VariableName, m.TenantId },
                new TenantEmailTemplateVariable()
                {
                    VariableName = "TransactionReferenceNumber",
                    TenantId = CurrentTenantId
                });


            context.VehicleInspectionTypes.AddOrUpdate(m => new { m.TypeName }, new VehicleInspectionType()
            {
                TypeName = "Ice Stone",
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
                CountryID = tenant.CountryID ?? 1,
                TenantId = tenant.TenantId,
                CurrencyID = tenant.CurrencyID,
                PriceGroupID = 1,
                AccountStatusID = AccountStatusEnum.Active,
                TaxID = 4,
                OwnerUserId = 1
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

            // Add Common Warranty types
            context.TenantWarranty.AddOrUpdate(m => new { m.WarrantyName, m.TenantId },
                new TenantWarranty
                {
                    WarrantyName = "None",
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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
                    DeliveryMethod = DeliveryMethods.DPD,
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


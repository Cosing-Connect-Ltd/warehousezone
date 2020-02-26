using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public class MarketServices : IMarketServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IAccountServices _accountServices;
        private readonly IUserService _userService;
        private readonly IProductServices _productServices;
        private readonly IMapper _mapper;

        public MarketServices(IApplicationContext currentDbContext, IAccountServices accountServices, IUserService userService, IProductServices productServices, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _accountServices = accountServices;
            _userService = userService;
            _productServices = productServices;
            _mapper = mapper;
        }

        #region Markets
        public MarketListViewModel GetAllValidMarkets(int tenantId)
        {
            var result = new MarketListViewModel();

            var markets = _currentDbContext.Markets.Where(m => m.TenantId == tenantId && m.IsDeleted != true);
            markets.ForEach(m =>
                {
                    result.Markets.Add(_mapper.Map(m, new MarketViewModel() { AllowDelete = m.IsDeleted != true }));
                });

            result.ResultsCount = markets.Count();

            return result;
        }


        public IEnumerable<object> GetAllValidMarkets(int tenantId, int warehouseId)
        {


            var markets = _currentDbContext.Markets.Where(m => m.TenantId == tenantId && m.IsDeleted != true).Select(u => new
            {
                MarketId = u.Id,
                MarketName = u.Name

            });




            return markets;
        }



        public MarketViewModel GetMarketById(int marketId)
        {
            var item = _currentDbContext.Markets.First(m => m.Id == marketId && m.IsDeleted != true);

            return _mapper.Map(item, new MarketViewModel());
        }


        public Market SaveMarket(Market market, int userId)
        {
            if (market.Id > 0)
            {
                var newMarket = _currentDbContext.Markets.Find(market.Id);
                _currentDbContext.Entry(newMarket).State = EntityState.Detached;
                market.UpdateUpdatedInfo(userId);
                market.CreatedBy = newMarket.CreatedBy;
                market.DateCreated = newMarket.DateCreated;
                _currentDbContext.Entry(market).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return market;
            }
            market.UpdateCreatedInfo(userId);
            _currentDbContext.Entry(market).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return market;
        }

        public void DeleteMarket(int marketId, int userId)
        {
            var market = _currentDbContext.Markets.First(m => m.Id == marketId);
            if (market != null)
            {
                market.IsDeleted = true;
                market.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(market).State = EntityState.Modified;
                var marketCustomer = _currentDbContext.MarketCustomers.Where(u => u.MarketId == marketId).ToList();
                if (marketCustomer.Count > 0)
                {
                    marketCustomer.ToList().ForEach(u => u.IsDeleted = true);
                    marketCustomer.ToList().ForEach(u => u.DateUpdated = DateTime.UtcNow);
                    marketCustomer.ToList().ForEach(u => u.UpdatedBy = userId);
                    //_currentDbContext.Entry(marketCustomer).State = EntityState.Modified;
                }
                _currentDbContext.SaveChanges();
            }

        }

        public MarketCustomersViewModel GetMarketCustomersById(int marketId, int tenantId, string searchText = null)
        {
            var mc = _currentDbContext.MarketCustomers.Where(r => r.MarketId == marketId && r.IsDeleted != true && r.TenantId == tenantId)
                 .Select(c => new CustomerAccountViewModel()
                 {
                     AccountId = c.AccountId,
                     AccountName = c.Customer.CompanyName,
                     AccountAddressLine1 = c.Customer.AccountAddresses.FirstOrDefault().AddressLine1,
                     AccountAddressPostCode = c.Customer.AccountAddresses.FirstOrDefault().PostCode,
                     IsSkippable = c.IsSkippable,
                     SkipFromDate = c.SkipFromDate,
                     SkipToDate = c.SkipToDate,
                     SortOrder = c.SortOrder,
                     VisitFrequency = c.VisitFrequency
                 }).OrderBy(x => x.SortOrder).ToList();

            var accIds = mc.Select(x => x.AccountId);

            var ac = _currentDbContext.Account.Where(x => !accIds.Contains(x.AccountID) && x.TenantId == tenantId && (searchText == null || x.CompanyName.Contains(searchText) || x.AccountCode.Contains(searchText))).Take(20)
                .Select(x => new CustomerAccountViewModel()
                {
                    AccountId = x.AccountID,
                    AccountName = x.CompanyName,
                    AccountAddressLine1 = x.AccountAddresses.FirstOrDefault().AddressLine1,
                    VisitFrequency = MarketCustomerVisitFrequency.Weekly
                }).ToList();

            MarketCustomersViewModel marketCustomers = new MarketCustomersViewModel();

            marketCustomers.SelectedCustomers = _mapper.Map(mc, new List<CustomerAccountViewModel>());
            marketCustomers.AvailableCustomers = _mapper.Map(ac, new List<CustomerAccountViewModel>());

            var routeViewModel = marketCustomers;
            routeViewModel.MarketId = marketId;

            return routeViewModel;
        }

        public MarketCustomersViewModel SaveMarketCustomer(MarketCustomersViewModel model, int userId, int TenantId)
        {
            var marketId = model.MarketId;
            if (model.MarketId > 0)
            {
                var existingAccountIds = model.MarketCustomerAccounts.Select(a => a.AccountId);

                var existingAccounts = _currentDbContext.MarketCustomers.Where(m => m.MarketId == marketId && existingAccountIds.Contains(m.AccountId)).ToList();
                foreach (var account in existingAccounts)
                {
                    var marketRouteItem = model.MarketCustomerAccounts.FirstOrDefault(a => a.AccountId == account.AccountId);
                    account.SortOrder = marketRouteItem.SortOrder;
                    account.IsSkippable = marketRouteItem.IsSkippable;
                    account.SkipFromDate = marketRouteItem.SkipFromDate;
                    account.SkipToDate = marketRouteItem.SkipToDate;
                    account.VisitFrequency = marketRouteItem.VisitFrequency;
                    account.TenantId = TenantId;
                    _currentDbContext.Entry(account).State = EntityState.Modified;
                }

                var newAccounts = model.MarketCustomerAccounts.Where(m => !existingAccounts.Select(a => a.AccountId).Contains(m.AccountId));
                foreach (var account in newAccounts)
                {
                    MarketCustomer cust = new MarketCustomer();

                    cust.MarketId = marketId;
                    cust.AccountId = account.AccountId;
                    cust.IsSkippable = account.IsSkippable;
                    cust.SkipFromDate = account.SkipFromDate;
                    cust.SkipToDate = account.SkipToDate;
                    cust.SortOrder = account.SortOrder;
                    cust.VisitFrequency = account.VisitFrequency;
                    cust.TenantId = TenantId;
                    cust.UpdateCreatedInfo(userId);
                    _currentDbContext.Entry(cust).State = EntityState.Added;
                }

                var deletedAccounts = _currentDbContext.MarketCustomers.Where(m => m.MarketId == marketId && !existingAccountIds.Contains(m.AccountId)).ToList();
                foreach (var account in deletedAccounts)
                {
                    _currentDbContext.Entry(account).State = EntityState.Deleted;
                }

                _currentDbContext.SaveChanges();
            }
            _currentDbContext.SaveChanges();
            return _mapper.Map(model, new MarketCustomersViewModel());
        }

        #endregion

        #region MarketRoute

        public MarketRouteListViewModel GetAllValidMarketRoutes(int tenantId)
        {
            var result = new MarketRouteListViewModel();

            var routes = _currentDbContext.MarketRoutes.Where(m => m.TenantId == tenantId && m.IsDeleted != true).ToList();

            if (routes == null) return result;

            foreach (var route in routes)
            {
                var routeMarketCount = route.MarketRouteMap.Count();
                var routeViewModel = _mapper.Map(route, new MarketRouteViewModel());

                if (routeMarketCount <= 0)
                {
                    routeViewModel.AllowDelete = true;
                }

                result.MarketRouteViewModel.Add(routeViewModel);

            }

            return result;
        }

        public MarketRouteViewModel GetMarketRouteById(int routeId)
        {
            var result = new MarketRouteViewModel();

            var route = _currentDbContext.MarketRoutes.Find(routeId);

            if (route != null)
            {
                return result = _mapper.Map(route, new MarketRouteViewModel());
            }

            else
            {
                return result;
            }

        }

        public MarketRoute SaveMarketRoute(MarketRoute marketRoute, int userId)
        {
            if (marketRoute.Id > 0)
            {
                var newMarket = _currentDbContext.MarketRoutes.Find(marketRoute.Id);
                _currentDbContext.Entry(newMarket).State = EntityState.Detached;
                marketRoute.UpdateUpdatedInfo(userId);
                marketRoute.CreatedBy = newMarket.CreatedBy;
                marketRoute.DateCreated = newMarket.DateCreated;
                _currentDbContext.Entry(marketRoute).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return marketRoute;
            }
            marketRoute.UpdateCreatedInfo(userId);
            _currentDbContext.Entry(marketRoute).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return marketRoute;
        }

        public void DeleteMarketRoute(int marketRouteId, int userId)
        {
            var marketRoute = _currentDbContext.MarketRoutes.Find(marketRouteId);
            if (marketRoute != null)
            {
                marketRoute.IsDeleted = true;
                marketRoute.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(marketRoute).State = EntityState.Modified;
                _currentDbContext.SaveChanges();

                var routeSchedules = _currentDbContext.MarketRouteSchedules.Where(x => x.RouteId == marketRouteId).ToList();

                foreach (var item in routeSchedules)
                {
                    item.IsCanceled = true;
                    _currentDbContext.Entry(item).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
            }
        }



        public RouteMarketsViewModel GetRouteMarketsById(int routeId, int tenantId, string searchText = null)
        {
            var mc = _currentDbContext.MarketRouteMap.Where(r => r.MarketRouteId == routeId)
                 .Select(c => new MarketRouteAccountViewModel()
                 {
                     MarketId = c.MarketId,
                     MarketName = c.Market.Name,
                     SortOrder = c.SortOrder
                 }).OrderBy(x => x.SortOrder).ToList();

            var accIds = mc.Select(x => x.MarketId);

            var ac = _currentDbContext.Markets.Where(x => !accIds.Contains(x.Id) && x.TenantId == tenantId && (searchText == null || x.Name.Contains(searchText) || x.Description.Contains(searchText)))
                .Select(x => new MarketRouteAccountViewModel()
                {
                    MarketId = x.Id,
                    MarketName = x.Name,

                }).ToList();

            RouteMarketsViewModel marketRoutes = new RouteMarketsViewModel();

            marketRoutes.SelectedMarkets = _mapper.Map(mc, new List<MarketRouteAccountViewModel>());
            marketRoutes.AvailableMarkets = _mapper.Map(ac, new List<MarketRouteAccountViewModel>());

            var routeViewModel = marketRoutes;
            routeViewModel.RouteId = routeId;

            return routeViewModel;
        }

        public RouteMarketsViewModel SaveRouteMarkets(RouteMarketsViewModel model, int userId)
        {
            var routeId = model.RouteId;
            if (model.RouteId > 0)
            {
                var existingMarketIds = model.MarketRouteAccounts.Select(a => a.MarketId);

                var existingMaps = _currentDbContext.MarketRouteMap.Where(m => m.MarketRouteId == routeId && existingMarketIds.Contains(m.MarketId)).ToList();
                foreach (var map in existingMaps)
                {
                    var marketRouteItem = model.MarketRouteAccounts.FirstOrDefault(a => a.MarketId == map.MarketId);
                    map.SortOrder = marketRouteItem.SortOrder;
                    map.MarketRouteId = model.RouteId;
                    map.MarketId = marketRouteItem.MarketId;

                    _currentDbContext.Entry(map).State = EntityState.Modified;
                }

                var newMarkets = model.MarketRouteAccounts.Where(m => !existingMaps.Select(a => a.MarketId).Contains(m.MarketId));
                foreach (var account in newMarkets)
                {
                    MarketRouteMap cust = new MarketRouteMap();

                    cust.MarketRouteId = routeId;
                    cust.SortOrder = account.SortOrder;
                    cust.MarketId = account.MarketId;
                    _currentDbContext.Entry(cust).State = EntityState.Added;
                }

                var deletedAccounts = _currentDbContext.MarketRouteMap.Where(m => m.MarketRouteId == routeId && !existingMarketIds.Contains(m.MarketId)).ToList();
                foreach (var account in deletedAccounts)
                {
                    _currentDbContext.Entry(account).State = EntityState.Deleted;
                }

                _currentDbContext.SaveChanges();
            }
            _currentDbContext.SaveChanges();

            return model;
        }

        #endregion

        #region Market Vehicle
        public MarketVehicleListViewModel GetAllValidMarketVehicles(int tenantId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            var result = new MarketVehicleListViewModel();

            var markets = _currentDbContext.MarketVehicles.Where(m => m.TenantId == tenantId && (includeIsDeleted || m.IsDeleted != true) && (!reqDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= reqDate));
            markets.ForEach(m =>
            {
                result.MarketVehicles.Add(_mapper.Map(m, new MarketVehicleViewModel()));
            });

            result.ResultsCount = markets.Count();

            return result;
        }
        public MarketVehicleViewModel GetMarketVehicleById(int marketVehicleId)
        {
            var item = _currentDbContext.MarketVehicles.First(m => m.Id == marketVehicleId && m.IsDeleted != true);

            return _mapper.Map(item, new MarketVehicleViewModel());
        }

        public MarketVehicleViewModel GetMarketVehicleByVehicleNumber(string marketVehicleNumber)
        {
            var item = _currentDbContext.MarketVehicles.First(m => m.VehicleIdentifier == marketVehicleNumber && m.IsDeleted != true);

            return _mapper.Map(item, new MarketVehicleViewModel());
        }

        public MarketVehicleViewModel SaveMarketVehicle(MarketVehicle marketVehicle, int userId)
        {
            if (marketVehicle.Id > 0)
            {
                var newMarketVehicle = _currentDbContext.MarketVehicles.Find(marketVehicle.Id);
                newMarketVehicle.UpdateUpdatedInfo(userId);
                newMarketVehicle.Description = marketVehicle.Description;
                newMarketVehicle.Name = marketVehicle.Name;
                newMarketVehicle.VehicleIdentifier = marketVehicle.VehicleIdentifier;
                _currentDbContext.Entry(newMarketVehicle).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return _mapper.Map(newMarketVehicle, new MarketVehicleViewModel());
            }
            marketVehicle.UpdateCreatedInfo(userId);
            _currentDbContext.Entry(marketVehicle).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return _mapper.Map(marketVehicle, new MarketVehicleViewModel());
        }
        public void DeleteMarketVehicle(int marketVehicleId, int userId)
        {
            var marketVehicle = _currentDbContext.MarketVehicles.First(m => m.Id == marketVehicleId);
            if (marketVehicle != null)
            {
                marketVehicle.IsDeleted = true;
                marketVehicle.UpdateUpdatedInfo(userId);
                _currentDbContext.Entry(marketVehicle).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
        }
        #endregion


        #region Market Jobs

        public List<MarketJobAllocationModel> GetAllResourceJobs(int userId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            var resourceId = _userService.GetResourceIdByUserId(userId);
            var resourceJobs = _currentDbContext.MarketJobAllocations.Where(m => m.ResourceId == resourceId && (includeIsDeleted || (m.MarketJob.IsDeleted != true && m.IsDeleted != true)) && (!reqDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= reqDate));
            return resourceJobs.Select(m => new MarketJobAllocationModel()
            {
                MarketJobId = m.MarketJobId,
                ResourceId = resourceId,
                Name = m.MarketJob.Name,
                Description = m.MarketJob.Description,
                LatestJobStatusId = m.MarketJobStatusId,
                Reason = (MarketJobStatusEnum)m.MarketJobStatusId == MarketJobStatusEnum.Declined ? m.Reason : m.Reason,
                ActionDate = m.DateUpdated,
                IsDeleted = m.MarketJob.IsDeleted == true || m.IsDeleted == true ? true : false

            }).ToList();
        }

        public IQueryable<MarketJob> GetAllValidMarketJobs(int tenantId, MarketJobStatusEnum? statusEnum = null)
        {
            //var results = new List<MarketJobViewModel>();

            var statusId = 0;
            if (statusEnum.HasValue)
            {
                statusId = (int)statusEnum.Value;
            }

            var marketJobs = _currentDbContext.MarketJobs.Where(m => m.TenantId == tenantId && m.IsDeleted != true && (statusEnum == 0 || m.LatestJobStatusId == statusId));

            //if (marketJobs.Any())
            //{
            //    marketJobs.ForEach(m =>
            //    {
            //        var model = _mapper.Map(m, new MarketJobViewModel());
            //        model.DisplayText = m.MarketRoute != null ? m.MarketRoute.Description : "";
            //        var jobAllocation = _currentDbContext.MarketJobAllocations.FirstOrDefault(x => x.MarketJobId == m.Id && (x.MarketJobStatusId != (int)MarketJobStatusEnum.Cancelled));
            //        if (jobAllocation != null)
            //        {
            //            model.ResourceName = jobAllocation.Resource.Name;
            //            model.ResourceID = jobAllocation.ResourceId;
            //        }
            //        results.Add(model);
            //    });
            //}
            return marketJobs;
        }
        public MarketJobViewModel GetMarketJobById(int marketJobId)
        {
            var model = new MarketJobViewModel();
            var item = _currentDbContext.MarketJobs.FirstOrDefault(m => m.Id == marketJobId && m.IsDeleted != true);

            if (item != null)
            {
                model = _mapper.Map(item, new MarketJobViewModel());

                var jobAllocation = GetLatestAllocationForJob(marketJobId);
                if (jobAllocation != null)
                {
                    model.ResourceID = jobAllocation.ResourceId;
                    model.ResourceName = jobAllocation.Resource.Name;
                }
                if (item.MarketRoute != null)
                {
                    model.MarketName = item.Market.Name;
                }
            }

            return model;
        }


        private MarketJobAllocation GetLatestAllocationForJob(int jobId)
        {
            return _currentDbContext.MarketJobAllocations.OrderByDescending(x => x.DateCreated)
                   .FirstOrDefault(x => x.MarketJobId == jobId &&
                                        (x.MarketJobStatusId != (int)MarketJobStatusEnum.Declined || x.MarketJobStatusId != (int)MarketJobStatusEnum.Cancelled ||
                                         x.MarketJobStatusId != (int)MarketJobStatusEnum.Completed));
        }

        public MarketJobViewModel SaveMarketJob(MarketJob marketJob, int? resourceId, int? latestjobStatusId, int userId, int tenantId)
        {
            if (marketJob.Id == 0)
            {
                marketJob.LatestJobStatusId = latestjobStatusId;
                marketJob.Market = null;
                _currentDbContext.MarketJobs.Add(marketJob);
                marketJob.UpdateCreatedInfo(userId);
                _currentDbContext.SaveChanges();
            }

            else
            {
                var job = _currentDbContext.MarketJobs.Find(marketJob.Id);
                if (job != null)
                {
                    job.Name = marketJob.Name;
                    job.Description = marketJob.Description;
                    job.MarketRouteId = marketJob.MarketRouteId;
                    job.UpdateUpdatedInfo(userId);
                    _currentDbContext.Entry(job).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                    marketJob = job;
                }
            }

            if (resourceId.HasValue)
            {
                var allJobs = _currentDbContext.MarketJobAllocations.Where(m => m.ResourceId != resourceId.Value && m.MarketJobId != marketJob.Id && m.MarketJobStatusId != (int)MarketJobStatusEnum.Cancelled);
                if (allJobs.Any())
                {
                    foreach (var job in allJobs)
                    {
                        job.MarketJobStatusId = (int)MarketJobStatusEnum.Cancelled;
                        _currentDbContext.Entry(job).State = EntityState.Modified;
                    }
                    _currentDbContext.SaveChanges();
                }

                UpdateMarketJobAllocation(marketJob.Id, resourceId.Value, tenantId, (int)MarketJobStatusEnum.Allocated).ConfigureAwait(true);
            }

            return _mapper.Map(marketJob, new MarketJobViewModel());
        }

        public async Task<MarketJobViewModel> UpdateMarketJobAllocationApi(int marketJobId, int userId, int tenantId, int? latestJobStatusId = null, string reason = null, DateTime? actionDate = null,
            double? latitude = null, double? longitude = null, string terminalSerial = null)
        {
            var resourceId = _userService.GetResourceIdByUserId(userId);
            return await UpdateMarketJobAllocation(marketJobId, resourceId, userId, tenantId, latestJobStatusId ?? (int)MarketJobStatusEnum.Allocated, reason, actionDate, latitude, longitude);
        }

        public async Task<MarketJobViewModel> UpdateMarketJobAllocation(int marketJobId, int resourceId, int userId, int tenantId, int? latestJobStatusId = null, string reason = null, DateTime? actionDate = null, double? latitude = null, double? longitude = null, string terminalSerial = null)
        {
            var marketJob = _currentDbContext.MarketJobs.Find(marketJobId);
            marketJob.UpdatedBy = userId;
            marketJob.DateUpdated = DateTime.UtcNow;
            marketJob.LatestJobStatusId = latestJobStatusId;

            var marketJobAllocation = GetLatestAllocationForJob(marketJobId);

            if (resourceId == 0)
            {
                var allJobs = _currentDbContext.MarketJobAllocations.Where(m => m.MarketJobId == marketJobId && m.MarketJobStatusId != (int)MarketJobStatusEnum.Cancelled);
                if (allJobs.Any())
                {
                    foreach (var job in allJobs)
                    {
                        job.MarketJobStatusId = (int)MarketJobStatusEnum.Cancelled;
                        _currentDbContext.Entry(job).State = EntityState.Modified;
                    }
                }
                marketJob.LatestJobStatusId = (int)MarketJobStatusEnum.UnAllocated;
                marketJob.LatestJobAllocationId = null;
            }
            else
            {

                if (latestJobStatusId.HasValue && marketJobAllocation != null)
                {
                    marketJobAllocation.MarketJobStatusId = (int)latestJobStatusId.Value;
                    marketJobAllocation.ResourceId = resourceId;
                    marketJobAllocation.DeviceSerial = terminalSerial;

                    if (latestJobStatusId == (int)MarketJobStatusEnum.Completed)
                    {
                        marketJobAllocation.Latitude = latitude;
                        marketJobAllocation.Longitude = longitude;
                        marketJobAllocation.ActionDate = actionDate ?? DateTime.UtcNow;
                        marketJobAllocation.Reason = reason;
                    }
                    if (latestJobStatusId == (int)MarketJobStatusEnum.FailedToComplete)
                    {
                        marketJobAllocation.Reason = "Failed to complete: " + reason;
                    }
                    if (latestJobStatusId == (int)MarketJobStatusEnum.Declined)
                    {
                        marketJobAllocation.Reason = reason;
                        marketJobAllocation.ActionDate = actionDate ?? DateTime.UtcNow;
                        marketJobAllocation.Latitude = latitude;
                        marketJobAllocation.Longitude = longitude;
                    }
                    _currentDbContext.Entry(marketJobAllocation).State = EntityState.Modified;
                }
                else
                {
                    marketJobAllocation = new MarketJobAllocation()
                    {
                        CreatedBy = userId,
                        DateCreated = actionDate ?? DateTime.UtcNow,
                        MarketJobId = marketJobId,
                        ResourceId = resourceId,
                        MarketJobStatusId = (int)MarketJobStatusEnum.Allocated,
                        TenantId = tenantId,
                        DateUpdated = actionDate ?? DateTime.UtcNow,
                        DeviceSerial = terminalSerial
                    };

                    _currentDbContext.Entry(marketJobAllocation).State = EntityState.Added;
                    marketJob.LatestJobStatusId = (int)MarketJobStatusEnum.Allocated;
                }

                _currentDbContext.SaveChanges();
                marketJob.LatestJobAllocationId = marketJobAllocation.Id;

            }
            marketJobAllocation = new MarketJobAllocation()
            {
                CreatedBy = userId,
                DateCreated = actionDate ?? DateTime.UtcNow,
                MarketJobId = marketJobId,
                ResourceId = resourceId,
                MarketJobStatusId = (int)MarketJobStatusEnum.Allocated,
                TenantId = tenantId,
                DateUpdated = actionDate ?? DateTime.UtcNow,
                DeviceSerial = terminalSerial
            };
            _currentDbContext.SaveChanges();
            marketJob.LatestJobAllocationId = marketJobAllocation.Id;
            _currentDbContext.Entry(marketJob).State = EntityState.Modified;

            await _currentDbContext.SaveChangesAsync();

            return GetMarketJobById(marketJobId);
        }

        public async Task<MarketJobViewModel> AcceptMarketJob(int marketJobId, int userId, int tenantId, string terminalSerial = null, double? latitude = null, double? longitude = null)
        {
            return await UpdateMarketJobAllocationApi(marketJobId, userId, tenantId, (int)MarketJobStatusEnum.Accepted, null, DateTime.UtcNow, latitude, longitude, terminalSerial);
        }
        public async Task<MarketJobViewModel> DeclineMarketJob(int marketJobId, int userId, int tenantId, string declineReason, string terminalSerial = null, double? latitude = null, double? longitude = null)
        {
            return await UpdateMarketJobAllocationApi(marketJobId, userId, tenantId, (int)MarketJobStatusEnum.Declined, declineReason, DateTime.UtcNow, latitude, longitude, terminalSerial);
        }

        public async Task<MarketJobViewModel> CompleteMarketJob(int marketJobId, int userId, int tenantId, string reason, string terminalSerial = null, double? latitude = null, double? longitude = null)
        {
            return await UpdateMarketJobAllocationApi(marketJobId, userId, tenantId, (int)MarketJobStatusEnum.Completed, reason, DateTime.UtcNow, latitude, longitude, terminalSerial);
        }

        public async Task<MarketJobViewModel> CancelMarketJob(int marketJobId, int userId, int tenantId, string cancelReason, string terminalSerial = null, double? latitude = null, double? longitude = null)
        {
            return await UpdateMarketJobAllocationApi(marketJobId, userId, tenantId, (int)MarketJobStatusEnum.Cancelled, cancelReason, DateTime.UtcNow, latitude, longitude, terminalSerial);
        }

        #endregion

        #region Market Stock Levels

        public List<MarketProductLevelViewModel> GetAllStockLevelsForMarket(int marketId)
        {
            var market = _currentDbContext.Markets.Find(marketId);
            var allProducts = _productServices.GetAllValidProductMasters(market.TenantId);
            var allStockLevels = _currentDbContext.ProductMarketStockLevel.Where(m => m.MarketId == marketId);
            var levels =
                from p in allProducts
                join a in allStockLevels on p.ProductId equals a.ProductMasterID into tmpGroups
                from g in tmpGroups.DefaultIfEmpty()
                select new MarketProductLevelViewModel()
                {
                    ProductName = p.Name,
                    ProductID = p.ProductId,
                    SKUCode = p.SKUCode,
                    MarketId = marketId,
                    ReOrderQuantity = p.ReorderQty ?? 0,
                    MinStockQuantity = g == null ? 0 : g.MinStockQuantity,
                    ProductMarketStockLevelID = g == null ? 0 : g.ProductMarketStockLevelID
                };

            return levels.ToList();
        }

        public ProductMarketStockLevel UpdateProductLevelsForMarkets(int marketId, int productId, decimal stockQty, int userId)
        {
            var stockLevel = _currentDbContext.ProductMarketStockLevel.FirstOrDefault(m => m.ProductMasterID == productId && m.MarketId == marketId && m.IsDeleted != true);
            var market = _currentDbContext.Markets.Find(marketId);
            if (stockLevel == null)
            {
                stockLevel = new ProductMarketStockLevel()
                {
                    ProductMasterID = productId,
                    TenantId = market.TenantId,
                    CreatedBy = userId,
                    DateCreated = DateTime.UtcNow,
                    DateUpdated = DateTime.UtcNow,
                    MinStockQuantity = stockQty,
                    MarketId = marketId,
                    UpdatedBy = userId
                };
                _currentDbContext.ProductMarketStockLevel.Add(stockLevel);
            }
            else
            {
                stockLevel.MinStockQuantity = stockQty;
                stockLevel.UpdatedBy = userId;
                stockLevel.DateUpdated = DateTime.UtcNow;
                _currentDbContext.Entry(stockLevel).State = EntityState.Modified;
            }
            _currentDbContext.SaveChanges();
            return stockLevel;
        }

        public IQueryable<MarketRouteProgress> GetMarketRouteProgresses(int? marketRouteId, DateTime dateTime, int tenantId)
        {
            return _currentDbContext.MarketRouteProgresses.Where(u => u.TenantId == tenantId);
        }

        public string GetMarketName(int accountId, int MarketId)
        {
            if (MarketId > 0)
            {
                return _currentDbContext.MarketCustomers.FirstOrDefault(u => u.AccountId == accountId && (MarketId == 0 || u.MarketId == MarketId) && u.IsDeleted != true)?.Market?.Name;
            }
            else
            {
                return string.Join(",", _currentDbContext.MarketCustomers.Where(u => u.AccountId == accountId && (MarketId == 0 || u.MarketId == MarketId) && u.IsDeleted != true).ToList().Select(u => u.Market?.Name));
            }

        }

        #endregion
    }
}
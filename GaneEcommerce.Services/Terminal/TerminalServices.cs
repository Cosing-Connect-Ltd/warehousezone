using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Ganedata.Core.Services
{
    public class TerminalServices : ITerminalServices
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMapper _mapper;

        public TerminalServices(IApplicationContext currentDbContext, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _mapper = mapper;
        }

        public IEnumerable<Terminals> GetAllTerminals(int tenantId)
        {
            var results = (from t in _currentDbContext.Terminals where (t.IsDeleted != true && t.TenantId == tenantId) select t).ToList();

            return results;
        }

        public IEnumerable<Terminals> GetAllTerminalsWithoutMobileLocationLinks(int tenantId, int? includeTerminalId = null)
        {
            var linkedTerminalIds = _currentDbContext.TenantWarehouses.Where(m => m.SalesTerminal != null).Select(x => x.SalesTerminalId).ToList();

            var results =
                (from t in _currentDbContext.Terminals
                 where (t.IsDeleted != true && t.TenantId == tenantId) && (!linkedTerminalIds.Contains(t.TerminalId) || includeTerminalId == t.TerminalId)
                 select t).ToList();
            return results;
        }

        public TenantLocations GetMobileLocationByTerminalId(int terminalId)
        {
            return _currentDbContext.TenantWarehouses.FirstOrDefault(x => x.SalesTerminalId == terminalId && x.IsDeleted != true);
        }

        public IQueryable<Terminals> GetAllTerminalsForGrid(int tenantId, int WarehouseId)
        {
            return _currentDbContext.Terminals.Where(e => e.IsDeleted != true && (e.WarehouseId == WarehouseId || e.TenantWarehous.ParentWarehouseId == WarehouseId));
        }

        public Terminals GetTerminalById(int terminalId)
        {
            return _currentDbContext.Terminals.Find(terminalId);
        }

        public Terminals GetActiveTerminalById(int terminalId)
        {
            return _currentDbContext.Terminals.Find(terminalId);
        }

        public int GetTerminalCountBySerial(string serial, int tenantId)
        {
            return _currentDbContext.Terminals.Where(e => e.TermainlSerial == serial && e.TenantId == tenantId && e.IsDeleted != true).Count();

        }

        public int GetTerminalCountBySerialNotEqualId(string serial, int terminalId, int tenantId)
        {
            return _currentDbContext.Terminals.Where(e => e.TermainlSerial == serial && e.TenantId == tenantId && e.IsDeleted != true && e.TerminalId != terminalId).Count();
        }

        public int SaveTerminal(Terminals terminal, int userId, int tenantId)
        {
            terminal.DateCreated = DateTime.UtcNow;
            terminal.DateUpdated = DateTime.UtcNow;
            terminal.CreatedBy = userId;
            terminal.UpdatedBy = userId;
            terminal.TenantId = tenantId;

            _currentDbContext.Entry(terminal).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return terminal.TerminalId;
        }

        public void UpdateTerminal(Terminals terminal, int userId, int tenantId)
        {
            terminal.DateUpdated = DateTime.UtcNow;
            terminal.UpdatedBy = userId;
            _currentDbContext.Entry(terminal).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public void DeleteTerminal(Terminals terminal, int userId)
        {
            terminal.IsDeleted = true;
            terminal.UpdatedBy = userId;
            terminal.DateUpdated = DateTime.UtcNow;
            _currentDbContext.Entry(terminal).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public Terminals GetTerminalBySerial(string serialNo)
        {
            return _currentDbContext.Terminals.FirstOrDefault(e => e.TermainlSerial.Trim().ToLower() == serialNo && e.IsActive == true && e.IsDeleted != true);
        }

        public bool IsOrderDetailLineExist(string skuCode, int orderId, DateTime dateReceived, int tenantId)
        {
            return _currentDbContext.OrderDetail.Any(e => e.ProductMaster.SKUCode == skuCode && e.OrderID == orderId && e.DateUpdated == dateReceived && e.TenentId == tenantId);
        }

        public bool IsOrderExistByOrderNoDate(string orderNo, DateTime dateReceived)
        {
            return _currentDbContext.Order.Any(e => e.OrderNumber == orderNo && e.DateUpdated == dateReceived);
        }

        public bool IsOrderProcessDetailExist(int orderDetailId)
        {
            return _currentDbContext.OrderProcessDetail.Any(x => x.OrderDetailID == orderDetailId && x.IsDeleted != true);
        }

        public TerminalsLog CreateTerminalLog(DateTime requestDate, int tenantId, int count, int terminalId, TerminalLogTypeEnum logType = TerminalLogTypeEnum.UsersSync)
        {
            var log = new TerminalsLog
            {
                TerminalLogId = Guid.NewGuid(),
                DateCreated = DateTime.UtcNow,
                DateRequest = requestDate,
                TenantId = tenantId,
                TerminalLogType = logType.ToString(),
                Ack = false,
                DateUpdated = DateTime.UtcNow,
                SentCount = count,
                TerminalId = terminalId
            };

            _currentDbContext.Entry(log).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return log;
        }

        public void SaveTerminalLog(TerminalsLog terminalLog, int tenantId)
        {
            terminalLog.TenantId = tenantId;
            _currentDbContext.Entry(terminalLog).State = EntityState.Added;
            _currentDbContext.SaveChanges();
        }

        public int SaveTerminalOrder(Order order, int userId, int tenantId, DateTime dateReceived, string jobNo)
        {
            order.CreatedBy = userId;
            order.UpdatedBy = userId;
            order.DateCreated = dateReceived;
            order.DateUpdated = dateReceived;
            order.TenentId = tenantId;
            order.OrderNumber = jobNo;
            _currentDbContext.Entry(order).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return order.OrderID;
        }

        public int SaveTerminalOrderDetail(OrderDetail orderDetail, int userId, int tenantId, DateTime dateReceived)
        {
            orderDetail.TenentId = tenantId;
            orderDetail.CreatedBy = userId;
            orderDetail.UpdatedBy = userId;
            orderDetail.DateCreated = dateReceived;
            orderDetail.DateUpdated = dateReceived;

            _currentDbContext.Entry(orderDetail).State = EntityState.Added;
            _currentDbContext.SaveChanges();
            return orderDetail.OrderDetailID;

        }

        public IQueryable<TerminalsLog> GetTerminalLogById(int terminalId)
        {
            return _currentDbContext.TerminalsLog.AsNoTracking().Where(x => x.TerminalId == terminalId);
        }

        public IQueryable<TerminalsLog> GetTerminalLogBySerial(string serialNo)
        {
            Terminals terminal = GetTerminalBySerial(serialNo);
            return _currentDbContext.TerminalsLog.Where(x => x.TerminalId == terminal.TerminalId);
        }

        public TerminalsLog GetTerminalLogByLogId(Guid terminalLogId)
        {
            return _currentDbContext.TerminalsLog.Find(terminalLogId);
        }

        public void UpdateTerminalLog(TerminalsLog terminalLog)
        {
            _currentDbContext.Entry(terminalLog).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }

        public int SaveTerminalGeoLocation(TerminalGeoLocationViewModel location)
        {
            var newLocation = new TerminalGeoLocation();
            _mapper.Map(location, newLocation);

            if (newLocation.LoggedInUserId == 0)
            {
                newLocation.LoggedInUserId = null;
            }

            _currentDbContext.TerminalGeoLocation.Add(newLocation);
            int res = _currentDbContext.SaveChanges();
            return res;
        }

        public List<TerminalGeoLocationViewModel> GetTerminalGeoLocations(int terminalId = 0)
        {
            var geoLocations = new List<TerminalGeoLocationViewModel>();

            if (terminalId == 0)
            {

                var locations = _currentDbContext.TerminalGeoLocation.ToList();
                geoLocations = _mapper.Map<List<TerminalGeoLocation>, List<TerminalGeoLocationViewModel>>(locations);

            }

            else
            {
                var locations = _currentDbContext.TerminalGeoLocation.Where(x => x.TerminalId == terminalId).ToList();
                geoLocations = _mapper.Map<List<TerminalGeoLocation>, List<TerminalGeoLocationViewModel>>(locations);
            }

            return geoLocations;
        }

        public List<TerminalCommandsQueue> GetTerminalCommandQueue(int terminalId, DateTime date, bool status = false, bool result = false)
        {
            return _currentDbContext.TerminalCommandsQueue.AsNoTracking().Where(x => (x.TerminalId == terminalId) && x.sent == status && x.IsDeleted != true
            && x.result == result && DbFunctions.TruncateTime(x.ExecutionDate) == DbFunctions.TruncateTime(date)).ToList();
        }

        public void SaveTerminalCommandQueueStatus(TerminalCommandsQueue queue)
        {
            _currentDbContext.Entry(queue).State = EntityState.Modified;
            _currentDbContext.SaveChanges();
        }


        public int SaveAssetLog(AssetLogViewModel assetLog, int terminalId, int tenantId)
        {
            var newAssetLog = new AssetLog();

            _mapper.Map(assetLog, newAssetLog);

            var assetId = _currentDbContext.Assets.Where(x => x.AssetTag.ToLower() == newAssetLog.ClientMac.ToLower()).FirstOrDefault()?.AssetId;

            if (assetId != null && assetId > 0)
            {
                newAssetLog.AssetId = assetId;
            }

            newAssetLog.AssetLogId = Guid.NewGuid();
            newAssetLog.DateCreated = DateTime.UtcNow;
            newAssetLog.TerminalId = terminalId;
            newAssetLog.TenantId = tenantId;

            _currentDbContext.AssetLog.Add(newAssetLog);
            int res = _currentDbContext.SaveChanges();
            return res;
        }

        public bool GetTerminalTransactionLogById(Guid logId, int terminalId)
        {
            bool result = false;
            var log = _currentDbContext.TerminalsTransactionsLog.FirstOrDefault(x => x.TransactionLogReference == logId && x.TerminalId == terminalId);

            if (log != null)
            {
                result = true;
            }

            return result;

        }

        public int SaveTerminalTransactionLogById(Guid logId, int terminalId)
        {
            var log = new TerminalsTransactionsLog();

            log.TerminalId = terminalId;
            log.TransactionLogReference = logId;
            log.DateCreated = DateTime.UtcNow;



            _currentDbContext.TerminalsTransactionsLog.Add(log);
            int res = _currentDbContext.SaveChanges();
            return res;
        }

        public bool CheckTransactionLog(Guid transactionId, int terminalId)
        {
            var TransactionLog = GetTerminalTransactionLogById(transactionId, terminalId);

            if (TransactionLog == true)
            {
                return true;
            }
            else
            {
                SaveTerminalTransactionLogById(transactionId, terminalId);
                return false;
            }
        }

        public DateTime GetTerminalSyncDate(DateTime reqDate, int tenantId)
        {
            int TerminalSyncDays = _currentDbContext.TenantConfigs.FirstOrDefault(x => x.TenantId == tenantId).TerminalSyncDays;
            if (reqDate.Date == new DateTime(2000, 01, 01) && TerminalSyncDays != 0) { reqDate = DateTime.UtcNow.AddDays(-TerminalSyncDays); }
            return reqDate;
        }
    }
}
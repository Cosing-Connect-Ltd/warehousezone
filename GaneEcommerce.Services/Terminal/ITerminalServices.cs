using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ganedata.Core.Entities;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;

namespace Ganedata.Core.Services
{
    public interface ITerminalServices
    {
        IEnumerable<Terminals> GetAllTerminals(int tenantId);
        IEnumerable<Terminals> GetAllTerminalsWithoutMobileLocationLinks(int tenantId, int? includeTerminalId = null);
        IQueryable<Terminals> GetAllTerminalsForGrid(int tenantId, int WarehouseId);
        Terminals GetTerminalById(int terminalId);
        Terminals GetActiveTerminalById(int terminalId);
        int SaveTerminal(Terminals terminal, int userId, int tenantId);
        void UpdateTerminal(Terminals terminal, int userId, int tenantId);
        void DeleteTerminal(Terminals terminals, int userId);
        int GetTerminalCountBySerial(string serial, int tenantId);
        int GetTerminalCountBySerialNotEqualId(string serial, int terminalId, int tenantId);
        Terminals GetTerminalBySerial(string serialNo);
        bool IsOrderExistByOrderNoDate(string orderNo, DateTime dateReceived);
        int SaveTerminalOrder(Order order, int userId, int tenantId, DateTime dateReceived, string jobNo);
        bool IsOrderDetailLineExist(string skuCode, int orderId, DateTime datereceived, int tenantId);
        int SaveTerminalOrderDetail(OrderDetail orderDetail, int userId, int tenantId, DateTime dateReceived);
        TerminalsLog CreateTerminalLog(DateTime requestDate, int tenantId, int count, int terminalId, TerminalLogTypeEnum logType = TerminalLogTypeEnum.UsersSync);
        void SaveTerminalLog(TerminalsLog terminalLog, int tenantId);
        bool IsOrderProcessDetailExist(int orderDetailId);
        IQueryable<TerminalsLog> GetTerminalLogById(int terminalId);
        TerminalsLog GetTerminalLogByLogId(Guid terminalLogId);
        IQueryable<TerminalsLog> GetTerminalLogBySerial(string serialNo);
        void UpdateTerminalLog(TerminalsLog terminalLog);
        TenantLocations GetMobileLocationByTerminalId(int terminalId);
        int SaveTerminalGeoLocation(TerminalGeoLocationViewModel location);
        List<TerminalGeoLocationViewModel> GetTerminalGeoLocations(int terminalId = 0);
        List<TerminalCommandsQueue> GetTerminalCommandQueue(int terminalId, DateTime date, bool status = false, bool result = false);
        void SaveTerminalCommandQueueStatus(TerminalCommandsQueue queue);
        int SaveAssetLog(AssetLogViewModel assetLog, int terminalId, int tenantId);
        bool GetTerminalTransactionLogById(Guid logId, int terminalId);
        int SaveTerminalTransactionLogById(Guid logId, int terminalId);
        bool CheckTransactionLog(Guid transactionId, int terminalId);
        DateTime GetTerminalSyncDate(DateTime reqDate, int tenantId);
    }
}

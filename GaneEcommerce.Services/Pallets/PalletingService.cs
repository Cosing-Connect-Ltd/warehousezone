using AutoMapper;
using Ganedata.Core.Data;
using Ganedata.Core.Data.Helpers;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Domain.ViewModels;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;

namespace Ganedata.Core.Services
{
    public class PalletingService : IPalletingService
    {
        private readonly IApplicationContext _currentDbContext;
        private readonly IMarketServices _marketServices;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private DataImportFactory _dataImportFactory;

        public PalletingService(IApplicationContext currentDbContext, IMarketServices marketServices, IUserService userService, IMapper mapper)
        {
            _currentDbContext = currentDbContext;
            _marketServices = marketServices;
            _userService = userService;
            _mapper = mapper;
            _dataImportFactory = new DataImportFactory();

        }
        public PalletProduct AddFulFillmentPalletProduct(PalletProductAddViewModel model)
        {
            var orderProcessDetail = _currentDbContext.OrderProcessDetail.First(m => m.OrderProcessDetailID == model.OrderProcessDetailID);
            var fulfillmentProduct = new PalletProduct()
            {
                OrderID = orderProcessDetail.OrderDetail.OrderID,
                OrderProcessDetailID = orderProcessDetail.OrderProcessDetailID,
                PalletID = model.CurrentPalletID,
                ProductID = model.ProductID,
                Quantity = model.PalletQuantity,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated
            };

            _currentDbContext.PalletProducts.Add(fulfillmentProduct);
            _currentDbContext.SaveChanges();

            return fulfillmentProduct;
        }

        public List<PalletProduct> GetFulFillmentPalletProductsForPallet(int palletId)
        {
            return _currentDbContext.PalletProducts.Where(m => m.PalletID == palletId && m.IsDeleted != true).ToList();
        }
        public List<PalletProductsSync> GetAllPalletProductsForSync(DateTime? afterDate)
        {
            var palletProducts = _currentDbContext.PalletProducts.AsNoTracking().Where(m => (!afterDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= afterDate) && m.IsDeleted != true);
            var result = palletProducts.Select(m => new PalletProductsSync()
            {
                AccountID = m.Pallet.RecipientAccountID,
                CurrentPalletID = m.PalletID,
                OrderProcessDetailID = m.OrderProcessDetailID ?? 0,
                OrderID = m.OrderID,
                PalletProductID = m.PalletProductID,
                ProductID = m.ProductID,
                PalletQuantity = m.Quantity
            }).ToList();
            return result;
        }
        public PalletDispatchInfoViewModel GetPalletDispatchDetailByPallet(int palletId)
        {
            var pallet = GetFulfillmentPalletById(palletId);
            var model = new PalletDispatchInfoViewModel();
            if (pallet == null || pallet.DateCompleted == null || pallet.PalletsDispatch == null)
            {
                return null;
            }
            model.PalletID = palletId;
            model.SentMethod = pallet.PalletsDispatch.DeliveryMethod != null ? pallet.PalletsDispatch.DeliveryMethod.ToString() : "";
            model.TrackingReference = pallet.PalletsDispatch.TrackingReference;
            if (pallet.PalletsDispatch.VehicleDriverResource != null)
            {
                model.DriverName = pallet.PalletsDispatch.VehicleDriverResource.Name;
            }
            model.VehicleNumber = pallet.PalletsDispatch.VehicleIdentifier;
            model.DispatchNotes = pallet.PalletsDispatch.DispatchNotes;
            model.DispatchEvidenceImages = pallet.PalletsDispatch.ProofOfDeliveryImageFilenames;
            model.DispatchDate = pallet.PalletsDispatch.DateCompleted.Value.ToString("dd/MM/yyyy HH:mm");
            return model;
        }

        public List<DeliveryMethods> GetAllSentMethods()
        {
            return Enum.GetValues(typeof(DeliveryMethods)).Cast<DeliveryMethods>().ToList();
        }

        public IEnumerable<TenantDeliveryService> GetAllDpdServices()
        {
            return _currentDbContext.TenantDeliveryServices.Where(u => u.IsDeleted != true);
        }
        public Pallet CreateNewPallet(int orderProcessId, int userId)
        {
            var orderProcess = _currentDbContext.OrderProcess.Find(orderProcessId);

            var pallet = new Pallet() { PalletNumber = GenerateNextPalletNumber(), DateCreated = DateTime.UtcNow, CreatedBy = userId, OrderProcessID = orderProcessId, RecipientAccountID = orderProcess.Order.AccountID };
            _currentDbContext.Pallets.Add(pallet);
            _currentDbContext.SaveChanges();
            return pallet;
        }

        public decimal GetFulfilledProductQuantity(int orderProcessDetailId)
        {
            return _currentDbContext.PalletProducts.Where(m => m.OrderProcessDetailID == orderProcessDetailId && m.IsDeleted != true).ToList().Sum(x => x.Quantity);
        }

        public string DispatchPallets(PalletDispatchViewModel dispatch, int userId)
        {
            string result = "";
            if (dispatch.PalletDispatchId > 0)
            {

                var item = _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == dispatch.PalletDispatchId);
                if (item != null)
                {
                    item.MarketVehicleID = dispatch.MarketVehicleID;
                    item.VehicleDriverResourceID = dispatch.MarketVehicleDriverID;
                    item.ProofOfDeliveryImageFilenames = dispatch.ProofOfDeliveryImageFilenames;
                    item.DispatchNotes = dispatch.DispatchNotes;
                    item.DispatchReference = dispatch.DispatchRefrenceNumber;
                    item.UpdatedBy = userId;
                    item.DateUpdated = DateTime.UtcNow;
                    item.TrackingReference = dispatch.TrackingReference;
                    item.DeliveryMethod = dispatch.DeliveryMethod;
                    item.NetworkCode = dispatch.DeliveryMethod == DeliveryMethods.DPD ? dispatch.NetworkCode : "";
                    _currentDbContext.Entry(item).State = EntityState.Modified;
                    if (!string.IsNullOrEmpty(item.OrderProcessID.ToString()))
                    {
                        int OrderProcessId = int.Parse(item.OrderProcessID.ToString());
                        var PalletList = _currentDbContext.Pallets.Where(u => u.OrderProcessID == OrderProcessId).ToList();
                        var orderProcess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == OrderProcessId);

                        foreach (var palletId in PalletList)
                        {
                            var pallet = _currentDbContext.Pallets.Find(palletId.PalletID);
                            if (pallet != null && pallet.PalletProducts.Count > 0)
                            {

                                pallet.DateCompleted = DateTime.UtcNow;
                                pallet.DateUpdated = DateTime.UtcNow;
                                pallet.CompletedBy = userId;
                                pallet.PalletsDispatch = item;


                                _currentDbContext.Entry(pallet).State = pallet.PalletsDispatchID > 0 ? EntityState.Modified : EntityState.Added;
                            }
                        }
                        if (orderProcess != null)
                        {
                            orderProcess.OrderProcessStatusId = (int)OrderProcessStatusEnum.Dispatched;
                            orderProcess.DateUpdated = DateTime.UtcNow;
                            orderProcess.UpdatedBy = userId;
                            _currentDbContext.Entry(orderProcess).State = EntityState.Modified;
                        }
                    }
                    _currentDbContext.SaveChanges();
                }
                if (dispatch.DeliveryMethod == DeliveryMethods.DPD)
                {
                    var model = MapModelWithRealValues(item.PalletsDispatchID, dispatch.NetworkCode);
                    result = _dataImportFactory.PostShipmentData(item.PalletsDispatchID, model);
                }
                return result;

            }
            else
            {
                var item = new PalletsDispatch
                {
                    CompletedBy = userId,
                    MarketVehicleID = dispatch.MarketVehicleID,
                    VehicleDriverResourceID = dispatch.MarketVehicleDriverID,
                    ProofOfDeliveryImageFilenames = dispatch.ProofOfDeliveryImageFilenames,
                    DispatchNotes = dispatch.DispatchNotes,
                    DispatchReference = dispatch.DispatchRefrenceNumber,
                    CreatedBy = userId,
                    NetworkCode = dispatch.DeliveryMethod == DeliveryMethods.DPD ? dispatch.NetworkCode : "",
                    DateCreated = DateTime.UtcNow,
                    OrderProcessID = int.Parse(!string.IsNullOrEmpty(dispatch.DispatchSelectedPalletIds) ? dispatch.DispatchSelectedPalletIds : "0"),
                    TrackingReference = dispatch.TrackingReference,
                    DeliveryMethod = dispatch.DeliveryMethod,
                    DateCompleted = DateTime.UtcNow,
                    DispatchStatus = PalletDispatchStatusEnum.Created
                };
                _currentDbContext.PalletsDispatches.Add(item);
                if (!string.IsNullOrEmpty(dispatch.DispatchSelectedPalletIds))
                {
                    int OrderProcessId = int.Parse(dispatch.DispatchSelectedPalletIds);
                    var PalletList = _currentDbContext.Pallets.Where(u => u.OrderProcessID == OrderProcessId).ToList();
                    var orderProcess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == OrderProcessId);

                    foreach (var palletId in PalletList)
                    {
                        var pallet = _currentDbContext.Pallets.Find(palletId.PalletID);
                        if (pallet != null && pallet.PalletProducts.Count > 0)
                        {

                            pallet.DateCompleted = DateTime.UtcNow;
                            pallet.DateUpdated = DateTime.UtcNow;
                            pallet.CompletedBy = userId;
                            pallet.PalletsDispatch = item;


                            _currentDbContext.Entry(pallet).State = EntityState.Modified;
                        }
                    }
                    if (orderProcess != null)
                    {
                        orderProcess.OrderProcessStatusId = (int)OrderProcessStatusEnum.Dispatched;
                        orderProcess.DateUpdated = DateTime.UtcNow;
                        orderProcess.UpdatedBy = userId;
                        _currentDbContext.Entry(orderProcess).State = EntityState.Modified;
                    }
                }
                _currentDbContext.SaveChanges();
                if (dispatch.DeliveryMethod == DeliveryMethods.DPD)
                {
                    var model = MapModelWithRealValues(item.PalletsDispatchID, dispatch.NetworkCode);
                    result = _dataImportFactory.PostShipmentData(item.PalletsDispatchID, model);
                }
                return result;
            }

        }

        public Pallet UpdatePalletProof(string palletNumber, byte[][] palletProofImages)
        {
            var pallet = _currentDbContext.Pallets.FirstOrDefault(m => m.PalletNumber == palletNumber);
            if (pallet != null)
            {
                var fileNames = new List<string>();
                foreach (var palletProofImage in palletProofImages)
                {
                    if (palletProofImage != null && (pallet.MobileToken.HasValue || pallet.PalletID > 0))
                    {
                        var filePath = HttpContext.Current.Server.MapPath("~/UploadedFiles/PalletProofs/");
                        var filename = Guid.NewGuid().ToString() + ".png";
                        var bw = new BinaryWriter(File.Open(filePath + filename, FileMode.OpenOrCreate));
                        bw.Write(palletProofImage);
                        bw.Close();
                        fileNames.Add(filename);
                    }
                }
                pallet.ProofOfLoadingImage = string.Join(",", fileNames);
                _currentDbContext.Entry(pallet).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
            }
            return pallet;
        }

        public Pallet UpdatePalletStatus(int palletId, bool isCompleted, Guid? newPalletToken = null, byte[][] palletProofImage = null)
        {
            var pallet = _currentDbContext.Pallets.FirstOrDefault(m => m.MobileToken == newPalletToken);

            if (palletId < 1)
            {
                pallet = pallet ?? new Pallet()
                {
                    PalletNumber = GenerateNextPalletNumber(),
                    CreatedBy = 0,
                    DateCreated = DateTime.UtcNow,
                    MobileToken = newPalletToken,
                    DateUpdated = DateTime.UtcNow,
                    ProofOfLoadingImage = null
                };

                _currentDbContext.Pallets.Add(pallet);
                _currentDbContext.SaveChanges();

                pallet = UpdatePalletProof(pallet.PalletNumber, palletProofImage);

                return pallet;
            }
            return null;
        }

        public Pallet GetFulfillmentPalletById(int palletId)
        {
            return _currentDbContext.Pallets.Find(palletId);
        }

        public Pallet GetFulfillmentPalletByNumber(string palletNumber)
        {
            return _currentDbContext.Pallets.FirstOrDefault(m => m.PalletNumber.Equals(palletNumber));
        }


        public List<PalletsDispatch> GetAllPalletsDispatch(int? dispatchId, DateTime? reqDate, int? orderProcessID = null)
        {
            return _currentDbContext.PalletsDispatches.Where(u => u.DateCompleted.HasValue && (!orderProcessID.HasValue || u.OrderProcessID == orderProcessID) && (!reqDate.HasValue || (u.DateUpdated ?? u.DateCreated) >= reqDate) && (!dispatchId.HasValue || u.PalletsDispatchID == dispatchId)).OrderByDescending(x => x.DateCreated).ToList();
        }

        public IEnumerable<PalletsDispatch> GetAllPalletsDispatch()
        {
            return _currentDbContext.PalletsDispatches.Where(u => u.DispatchStatus == PalletDispatchStatusEnum.Created).OrderByDescending(u => u.PalletsDispatchID);
        }

        public IQueryable<PalletViewModel> GetAllPallets(int? lastXdays = null, PalletStatusEnum? palletStatusEnum = null, int? orderProcessId = null, DateTime? reqDate = null, int? filterByPalletDetail = null, int? dispatchId = null)
        {
            var results = _currentDbContext.Pallets.Where(m => (!lastXdays.HasValue || (m.DateCreated > DbFunctions.AddDays(DateTime.UtcNow, -lastXdays.Value))) && (!reqDate.HasValue || (m.DateUpdated ?? m.DateCreated) >= reqDate) && (!dispatchId.HasValue || (m.PalletsDispatchID ?? m.PalletsDispatchID) == dispatchId)
            && m.IsDeleted != true);
            if (orderProcessId.HasValue)
            {
                results = results.Where(m => m.OrderProcessID == orderProcessId);
            }
            //if (palletStatusEnum != null)
            //{
            //    if (palletStatusEnum.Value == PalletStatusEnum.Completed)
            //    {
            //        results = results.Where(m => m.DateCompleted.HasValue);
            //    }
            //    else
            //    {
            //        results = results.Where(m => !m.DateCompleted.HasValue);
            //    }
            //}

            if (filterByPalletDetail.HasValue)
            {
                results = results.Where(u => u.PalletsDispatchID == filterByPalletDetail);
            }

            return results.Select(m => new PalletViewModel()
            {
                PalletNumber = m.PalletNumber,
                RecipientAccountID = m.RecipientAccountID,
                AccountName = m.RecipientAccount.CompanyName,
                AccountCode = m.RecipientAccount.AccountCode,
                DateCreated = m.DateCreated,
                DispatchTime = m.DateCompleted,
                PalletID = m.PalletID,
                Dispatch = m.PalletsDispatch,
                DateUpdated = m.DateUpdated,
                ProductCount = m.PalletProducts.Any() ? true : false,
                OrderProcessID = m.OrderProcessID,
                ScannedOnLoading = m.ScannedOnLoading,
                LoadingScanTime = m.LoadingScanTime,
                ScannedOnDelivered = m.ScannedOnDelivered,
                DeliveredScanTime = m.DeliveredScanTime,
                DispatchStatus = m.PalletsDispatchID

            });
        }

        public string GenerateNextPalletNumber(string prefixText = null)
        {
            var lastPallet = _currentDbContext.Pallets.OrderByDescending(p => p.PalletNumber).FirstOrDefault();

            var prefix = prefixText ?? "PT";
            var lastNumber = 1;
            if (lastPallet != null)
            {
                lastNumber = int.Parse(lastPallet.PalletNumber.Replace(prefix, ""));
            }

            if (lastPallet != null)
            {
                var nextPalletNumber = (lastNumber + 1).ToString("000000000");
                return prefix + nextPalletNumber;
            }
            else
            {
                return prefix + "000000001";
            }
        }

        public PalletSync DispatchPalletsFromHandheld(PalletSync currentDispatch, int userId)
        {
            int? resourceId = _userService.GetResourceIdByUserId(userId);
            if (resourceId == 0) { resourceId = null; }

            var item = new PalletsDispatch
            {
                CompletedBy = userId,
                MarketVehicleID = currentDispatch.PalletDispatchInfo.MarketVehicleID,
                VehicleDriverResourceID = resourceId,
                ProofOfDeliveryImageFilenames = currentDispatch.PalletDispatchInfo.ProofOfDeliveryImageFilenames,
                DispatchNotes = currentDispatch.PalletDispatchInfo.DispatchNotes,
                CreatedBy = userId,
                DateCreated = DateTime.UtcNow,
                TrackingReference = currentDispatch.PalletDispatchInfo.TrackingReference,
                DeliveryMethod = currentDispatch.PalletDispatchInfo.DeliveryMethod,
                DateCompleted = currentDispatch.DateCompleted,
                DispatchStatus = PalletDispatchStatusEnum.Created,
                OrderProcessID = currentDispatch.OrderProcessID
            };

            if (item.MarketVehicleID < 1)
            {
                var vehicle = _marketServices.GetMarketVehicleByVehicleNumber(currentDispatch.PalletDispatchInfo.VehicleIdentifier);
                if (vehicle == null)
                {
                    vehicle = _marketServices.SaveMarketVehicle(new MarketVehicle()
                    {
                        CreatedBy = userId,
                        TenantId = currentDispatch.TenantId,
                        DateCreated = currentDispatch.DateCreated,
                        Name = currentDispatch.PalletDispatchInfo.CustomVehicleModel,
                        VehicleIdentifier = currentDispatch.PalletDispatchInfo.VehicleIdentifier
                    }, userId);
                }
                item.MarketVehicleID = vehicle.Id;
            }

            _currentDbContext.PalletsDispatches.Add(item);
            _currentDbContext.SaveChanges();

            foreach (var palletId in currentDispatch.SelectedPallets)
            {
                var pallet = _currentDbContext.Pallets.Find(palletId);

                if (pallet != null)
                {
                    pallet.DateCompleted = DateTime.UtcNow;
                    pallet.DateUpdated = DateTime.UtcNow;
                    pallet.CompletedBy = userId;
                    pallet.PalletsDispatch = item;
                    _currentDbContext.Entry(pallet).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();

                    UpdatePalletProof(pallet.PalletNumber, currentDispatch.ProofOfLoadingImageBytes);
                }

            }

            currentDispatch.IsDispatched = true;
            currentDispatch.CompletedBy = userId;
            currentDispatch.DateCompleted = DateTime.UtcNow;

            //update order process status to dispatched as well
            var orderProcess = _currentDbContext.OrderProcess.Find(currentDispatch.OrderProcessID);
            orderProcess.OrderProcessStatusId = (int)OrderProcessStatusEnum.Dispatched;
            orderProcess.DateUpdated = DateTime.UtcNow;
            orderProcess.UpdatedBy = userId;
            _currentDbContext.Entry(orderProcess).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            currentDispatch.PalletDispatchInfo = _mapper.Map<PalletsDispatch, PalletDispatchSync>(item);

            return currentDispatch;
        }

        public bool DeletePallet(int palletId)
        {
            var pallet = _currentDbContext.Pallets.FirstOrDefault(u => u.PalletID == palletId);
            if (pallet != null)
            {
                pallet.IsDeleted = true;

                pallet.DateUpdated = DateTime.UtcNow;
                pallet.UpdatedBy = caCurrent.CurrentUser().UserId;
                if (pallet.PalletProducts != null)
                {
                    foreach (var pp in pallet.PalletProducts)
                    {
                        pp.IsDeleted = true;
                        pp.DateUpdated = DateTime.UtcNow;
                        pp.UpdatedBy = caCurrent.CurrentUser().UserId;
                        _currentDbContext.Entry(pp).State = EntityState.Modified;
                    }
                    _currentDbContext.Entry(pallet).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                    return true;
                }

            }

            return false;

        }

        public PalletDispatchProgress UpdateDispatchProgress(PalletDispatchProgress currentDispatch, int userId)
        {
            int? resourceId = _userService.GetResourceIdByUserId(userId);
            if (resourceId == 0) { resourceId = null; }

            var dispatch = _currentDbContext.PalletsDispatches.Find(currentDispatch.DispatchId);

            dispatch.DispatchStatus = currentDispatch.DispatchStatus;
            dispatch.DateUpdated = DateTime.UtcNow;
            dispatch.UpdatedBy = userId;
            dispatch.ReceiverName = currentDispatch.ReceiverName;


            if (currentDispatch.ReceiverSign != null && currentDispatch.DispatchStatus == PalletDispatchStatusEnum.Delivered)
            {
                var filePath = HttpContext.Current.Server.MapPath("~/UploadedFiles/DispatchSign/");
                var filename = Guid.NewGuid().ToString() + ".png";
                var bw = new BinaryWriter(File.Open(filePath + filename, FileMode.OpenOrCreate));
                bw.Write(currentDispatch.ReceiverSign);
                bw.Close();
                dispatch.ReceiverSign = filename;
            }

            _currentDbContext.Entry(dispatch).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            foreach (var palletId in currentDispatch.ScannedPalletSerials)
            {
                var pallet = _currentDbContext.Pallets.Find(palletId);

                if (pallet != null)
                {
                    pallet.DateCompleted = DateTime.UtcNow;
                    pallet.DateUpdated = DateTime.UtcNow;
                    pallet.CompletedBy = userId;
                    _currentDbContext.Entry(pallet).State = EntityState.Modified;

                    if (currentDispatch.DispatchStatus == PalletDispatchStatusEnum.Loaded)
                    {
                        pallet.ScannedOnLoading = true;
                        pallet.LoadingScanTime = DateTime.UtcNow;
                    }
                    else if (currentDispatch.DispatchStatus == PalletDispatchStatusEnum.Delivered)
                    {
                        pallet.ScannedOnDelivered = true;
                        pallet.DeliveredScanTime = DateTime.UtcNow;
                    }

                    _currentDbContext.SaveChanges();
                }

            }

            //update order process status
            var orderProcess = _currentDbContext.OrderProcess.Find(dispatch.OrderProcessID);
            if (currentDispatch.DispatchStatus == PalletDispatchStatusEnum.Loaded)
            {
                orderProcess.OrderProcessStatusId = 4;
                orderProcess.DateUpdated = DateTime.UtcNow;
            }
            else if (currentDispatch.DispatchStatus == PalletDispatchStatusEnum.Delivered)
            {
                orderProcess.OrderProcessStatusId = 5;
                orderProcess.DateUpdated = DateTime.UtcNow;
            }
            _currentDbContext.Entry(orderProcess).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            return currentDispatch;
        }

        public int DeletePalletProduct(int palletProductId, int userId)
        {
            var palletproduct = _currentDbContext.PalletProducts.FirstOrDefault(m => m.PalletProductID == palletProductId);
            int palletId = 0;
            if (palletproduct != null)
            {
                try
                {

                    palletproduct.IsDeleted = true;
                    palletproduct.DateUpdated = DateTime.UtcNow;
                    palletproduct.UpdatedBy = userId;
                    _currentDbContext.PalletProducts.Attach(palletproduct);
                    _currentDbContext.Entry(palletproduct).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                    palletId = palletproduct.PalletID;

                }
                catch (Exception)
                {
                    return palletId;
                }

            }
            return palletId;
        }

        public bool MarkedOrderProcessAsDispatch(int OrderProcessId)
        {
            var orderprocess = _currentDbContext.OrderProcess.FirstOrDefault(u => u.OrderProcessID == OrderProcessId);
            if (orderprocess != null)
            {
                orderprocess.OrderProcessStatusId = (int)OrderProcessStatusEnum.Dispatched;
                _currentDbContext.Entry(orderprocess).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return true;
        }


        public PalletsDispatch GetPalletsDispatchByDispatchId(int palletDispatchId)
        {
            return _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == palletDispatchId);
        }

        public bool UpdatePalletsDispatchStatus(int dispatchId, int? reourceId, int userID, bool Status = false)
        {
            var palletDispatch = _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == dispatchId);
            if (palletDispatch != null)
            {
                palletDispatch.DispatchStatus = Status ? PalletDispatchStatusEnum.Created : PalletDispatchStatusEnum.Scheduled;
                palletDispatch.UpdatedBy = userID;
                palletDispatch.DateUpdated = DateTime.UtcNow;
                palletDispatch.MarketVehicleID = reourceId;
                _currentDbContext.Entry(palletDispatch).State = EntityState.Modified;
                _currentDbContext.SaveChanges();
                return true;
            }
            return false;
        }

        private DpdShipmentDataViewModel MapModelWithRealValues(int DispatchId, string NetworkCode)
        {
            var palletDispatch = _currentDbContext.PalletsDispatches.FirstOrDefault(u => u.PalletsDispatchID == DispatchId);
            if (palletDispatch != null)
            {
                DpdShipmentDataViewModel viewModel = new DpdShipmentDataViewModel();
                viewModel.jobId = null;
                viewModel.collectionOnDelivery = false;
                viewModel.invoice = null;
                viewModel.collectionDate = DateTime.UtcNow.AddHours(2);
                Consignment consignment = new Consignment();
                consignment.consignmentNumber = null;
                consignment.consignmentRef = null;
                consignment.parcel = new List<object>();
                CollectionDetails collectionDetails = new CollectionDetails();
                ContactDetails contactDetails = new ContactDetails();
                contactDetails.contactName = palletDispatch.OrderProcess?.Order?.Tenant?.TenantName;
                contactDetails.telephone = palletDispatch.OrderProcess?.Order?.Tenant?.TenantDayPhone;
                Address1 address1 = new Address1();
                address1.countryCode = palletDispatch.OrderProcess?.Order?.Tenant?.Country?.CountryCode;
                address1.county = palletDispatch.OrderProcess?.Order?.Tenant?.TenantStateCounty;
                address1.postcode = palletDispatch.OrderProcess?.Order?.Tenant?.TenantPostalCode;
                address1.street = palletDispatch.OrderProcess?.Order?.Tenant?.TenantAddress1;
                address1.town = palletDispatch.OrderProcess?.Order?.Tenant?.TenantCity;
                address1.county = palletDispatch.OrderProcess?.Order?.Tenant?.TenantStateCounty;
                collectionDetails.address = address1;
                collectionDetails.contactDetails = contactDetails;
                collectionDetails.address = address1;
                DeliveryDetails deliveryDetails = new DeliveryDetails();
                ContactDetails2 contactDetails2 = new ContactDetails2();
                contactDetails2.contactName = palletDispatch.OrderProcess?.Order?.Account?.AccountContacts.FirstOrDefault()?.ContactName;
                contactDetails2.telephone = palletDispatch.OrderProcess?.Order?.Account?.AccountContacts.FirstOrDefault()?.TenantContactPhone;
                deliveryDetails.contactDetails = contactDetails2;
                Address2 address2 = new Address2();
                address2.countryCode = palletDispatch.OrderProcess?.Order?.Tenant?.Country?.CountryCode;
                address2.postcode = palletDispatch.OrderProcess?.Order?.ShipmentAddressPostcode;
                address2.street = palletDispatch.OrderProcess?.Order?.ShipmentAddressLine1;
                address2.town = string.IsNullOrEmpty(palletDispatch.OrderProcess?.Order?.ShipmentAddressLine3) ? palletDispatch.OrderProcess?.Order?.ShipmentAddressLine2 : palletDispatch.OrderProcess?.Order?.ShipmentAddressLine3;
                deliveryDetails.address = address2;
                NotificationDetails notificationDetails = new NotificationDetails();
                notificationDetails.email = palletDispatch.OrderProcess?.Order?.Account?.AccountEmail;
                notificationDetails.mobile = palletDispatch?.OrderProcess.Order?.Account?.Telephone;
                deliveryDetails.notificationDetails = notificationDetails;
                consignment.collectionDetails = collectionDetails;
                consignment.deliveryDetails = deliveryDetails;
                consignment.networkCode = NetworkCode;
                consignment.numberOfParcels = _currentDbContext.Pallets.Count(u => u.PalletsDispatchID == DispatchId);

                consignment.totalWeight = GetPalltedProductWeight(DispatchId);
                consignment.shippingRef1 = palletDispatch.OrderProcess?.Order?.OrderID.ToString();
                consignment.shippingRef2 = palletDispatch.OrderProcessID.ToString();
                consignment.shippingRef3 = DispatchId.ToString();
                viewModel.consignment = new List<Consignment>();
                viewModel.consignment.Add(consignment);

                return viewModel;
            }
            return default;
        }

        private int GetPalltedProductWeight(int DispatchId)
        {
            var palletId = _currentDbContext.Pallets.Where(u => u.PalletsDispatchID == DispatchId).Select(u => u.PalletID).ToList();
            var productIds = _currentDbContext.PalletProducts.Where(u => palletId.Contains(u.PalletID)).Select(u => u.ProductID).ToList();
            var weights = _currentDbContext.ProductMaster.Where(u => productIds.Contains(u.ProductId)).Sum(u => u.Weight);
            int weight = int.Parse(weights.ToString());
            if (weight == 0) weight = 1;
            return weight;
        }

        public PalletDispatchLabelPrintViewModel PalletDispatchForLabels(int tenantId, int userId)
        {
            var result = new PalletDispatchLabelPrintViewModel();
            var apiCredentials = _currentDbContext.GlobalApis.Where(x => x.TenantId == tenantId && x.ApiTypes == ApiTypes.DPD).FirstOrDefault();
            var palletDispatch = _currentDbContext.PalletsDispatches.Where(u => u.CreatedBy == userId && u.LabelPrintStatus == false
            && u.DeliveryMethod == DeliveryMethods.DPD && !String.IsNullOrEmpty(u.ShipmentId)).FirstOrDefault();

            if (apiCredentials != null && palletDispatch != null)
            {
                result.ApiUrl = apiCredentials.ApiUrl;
                result.GeoSession = apiCredentials.ApiKey;
                result.GeoAccount = apiCredentials.AccountNumber;
                result.ShipmentId = palletDispatch.ShipmentId;
            }


            return result;
        }

        public bool UpdateDispatchForLabelsStatus(string shipmentId)
        {
            var palletDispatch = _currentDbContext.PalletsDispatches.Where(u => u.ShipmentId == shipmentId).FirstOrDefault();

            if (palletDispatch != null)
            {
                palletDispatch.LabelPrintStatus = true;
            }

            _currentDbContext.SaveChanges();


            return true;
        }
    }
}
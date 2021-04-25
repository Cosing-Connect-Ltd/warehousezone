using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Ganedata.Core.Services
{
    public class StockTakeApiService : IStockTakeApiService
    {
        private readonly IProductServices _productService;
        private readonly IApplicationContext _context;
        private readonly ILookupServices _lookupServices;
        private readonly IProductLookupService _productLookupService;

        public StockTakeApiService(IProductServices productService, IApplicationContext context, ILookupServices lookupService, IProductLookupService productLookupService)
        {
            _productService = productService;
            _context = context;
            _lookupServices = lookupService;
            _productLookupService = productLookupService;
        }

        public async Task<StockTakeProductCodeScanResponse> RecordScannedProducts(StockTakeProductCodeScanRequest request, bool moveToNextProduct = false)
        {
            var type = ScannedCodeTypeEnum.ProductCode;

            var response = new StockTakeProductCodeScanResponse() { Response = new ResponseObject() { MoveToNextProduct = moveToNextProduct } };

            var stockDetail = new StockTakeDetail();

            if (request.LocationCode != null && request.LocationCode != "")
            {
                var locationId = _context.Locations.AsNoTracking().FirstOrDefault(x => x.LocationCode.Equals(request.LocationCode, StringComparison.CurrentCultureIgnoreCase))?.LocationId;
                if (locationId != null && locationId >= 0)
                {
                    stockDetail.LocationId = locationId;
                }
            }


            if (!string.IsNullOrEmpty(request.SerialNo))
            {
                var terminal = _context.Terminals.FirstOrDefault(m => m.TermainlSerial == request.SerialNo);
                if (terminal != null)
                {
                    request.WarehouseId = terminal.WarehouseId;
                    request.CurrentTenantId = terminal.TenantId;
                }
            }

            //UserExistence
            var user =
                await _context.AuthUsers.FirstOrDefaultAsync(
                    e => e.UserId == request.AuthUserId && e.IsActive == true && e.IsDeleted != true);
            if (user == null)
            {
                response.Response.Success = false;
                response.Response.FailureMessage = $"Unauthorised User detected. UserId = {request.AuthUserId}";
                return LogAndRespond(request, response);
            }

            var currentStockTake =
                _context.StockTakes.FirstOrDefault(e => e.IsDeleted != true && e.WarehouseId == request.WarehouseId &&
                                                        e.Status == 0);
            //Current Stocktake
            if (currentStockTake == null)
            {
                response.Response.Success = false;
                response.Response.FailureMessage =
                    $"There is no stock takes currently running. ProductCode = {request.ProductCode}";
                return LogAndRespond(request, response);
            }

            // check product code
            if (request.ProductCode != null)
            {
                request.ProductCode = request.ProductCode.Trim();
            }
            else
            {
                request.ProductCode = "";
            }


            var warehouse = _context.TenantWarehouses.FirstOrDefault(m => m.WarehouseId == request.WarehouseId);
            if (warehouse != null)
            {
                request.CurrentTenantId = warehouse.TenantId;
            }


            if (request.NotExistingItem == true)
            {

                if (!string.IsNullOrEmpty(request.ProductSerial))
                {
                    var serial = _productService.GetProductSerialBySerialCode(request.ProductSerial, request.CurrentTenantId);
                    var productById = _productService.GetProductMasterById(request.ProductId);

                    if (serial != null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Serial Code = {request.ProductSerial} already exists in system and can't be created again";
                        return LogAndRespond(request, response);
                    }
                    else if (request.ProductId == 0)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Please select Product for the serial and submit again";
                        return LogAndRespond(request, response);
                    }
                    else if (!productById.Serialisable)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Selected product is not serialised, please selct correct product";
                        return LogAndRespond(request, response);
                    }
                    else
                    {
                        var newSerial = new ProductSerialis
                        {
                            SerialNo = request.ProductSerial,
                            ProductId = request.ProductId,
                            CurrentStatus = InventoryTransactionTypeEnum.AdjustmentIn,
                            DateCreated = DateTime.UtcNow,
                            CreatedBy = request.AuthUserId,
                            UpdatedBy = request.AuthUserId,
                            TenentId = request.CurrentTenantId,
                            WarehouseId = request.WarehouseId

                        };

                        _context.Entry(newSerial).State = EntityState.Added;
                        _context.SaveChanges();
                        request.ProductCode = newSerial.SerialNo;
                    }
                }
                else if (!string.IsNullOrEmpty(request.PalletSerial))
                {
                    var serial = _productService.GetPalletByPalletSerial(request.PalletSerial, request.CurrentTenantId);
                    var productById = _productService.GetProductMasterById(request.ProductId);

                    if (serial != null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Serial Code = {request.PalletSerial} already exists in system and can't be created again";
                        return LogAndRespond(request, response);
                    }
                    else if (request.ProductId == 0)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Please select Product for the serial and submit again";
                        return LogAndRespond(request, response);
                    }
                    else if (!productById.ProcessByPallet)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Selected product is not processed by pallets, please selct correct product";
                        return LogAndRespond(request, response);
                    }
                    else
                    {
                        var productCases = _context.ProductMaster.AsNoTracking().FirstOrDefault(x => x.ProductId == request.ProductId).ProductsPerCase ?? 1;

                        var newSerial = new PalletTracking
                        {
                            PalletSerial = request.PalletSerial,
                            ProductId = request.ProductId,
                            Status = PalletTrackingStatusEnum.Created,
                            DateCreated = DateTime.UtcNow,
                            TenantId = request.CurrentTenantId,
                            WarehouseId = request.WarehouseId,
                            TotalCases = request.ScannedQuantity,
                            RemainingCases = request.ScannedQuantity
                        };

                        request.ScannedQuantity = request.ScannedQuantity * productCases;

                        _context.Entry(newSerial).State = EntityState.Added;
                        _context.SaveChanges();

                        request.ProductCode = newSerial.PalletSerial;
                    }
                }

                else
                {
                    var productBySKU = _productService.GetProductMasterByProductCode(request.ProductCode, request.CurrentTenantId);
                    if (request.ProductCode == null || request.NewProductName == null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"SKU and Name are mandatory fields";
                        return LogAndRespond(request, response);
                    }
                    else if (productBySKU != null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Product already exists with provided SKU";
                        return LogAndRespond(request, response);
                    }
                    else
                    {
                        var dimensionUoms = _lookupServices.GetAllValidGlobalUoms(EnumUomType.Dimensions).FirstOrDefault();
                        var taxes = _lookupServices.GetAllValidGlobalTaxes().FirstOrDefault();
                        var weightGroups = _lookupServices.GetAllValidGlobalWeightGroups().FirstOrDefault();
                        var lotOptionCodes = _productLookupService.GetAllValidProductLotOptionsCodes().FirstOrDefault();
                        var lotProcessTypeCodes = _productLookupService.GetAllValidProductLotProcessTypeCodes().FirstOrDefault();
                        var departments = _lookupServices.GetAllValidTenantDepartments(request.CurrentTenantId).FirstOrDefault();

                        ProductMaster newProd = new ProductMaster
                        {
                            SKUCode = request.ProductCode,
                            Name = request.NewProductName,
                            BarCode = request.NewProductBarcode,
                            BarCode2 = request.NewProductBarcode2,
                            Serialisable = request.IsSerialised,
                            ProcessByPallet = request.IsProcessByPallet,
                            CreatedBy = request.AuthUserId,
                            TenantId = request.CurrentTenantId,
                            DateCreated = DateTime.UtcNow,
                            UOMId = dimensionUoms.UOMId,
                            WeightGroupId = weightGroups.WeightGroupId,
                            LotOptionCodeId = lotOptionCodes.LotOptionCodeId,
                            LotProcessTypeCodeId = lotProcessTypeCodes.LotProcessTypeCodeId,
                            DepartmentId = departments.DepartmentId,
                            TaxID = taxes.TaxID,
                            ProdStartDate = DateTime.UtcNow,
                            DimensionUOMId = dimensionUoms.UOMId,
                            IsActive = true


                        };

                        if (string.IsNullOrEmpty(newProd.BarCode))
                        {
                            newProd.BarCode = newProd.SKUCode;
                        }

                        _context.Entry(newProd).State = EntityState.Added;
                        _context.SaveChanges();
                        request.ProductCode = newProd.SKUCode;

                        if (request.IsProcessByPallet || request.IsSerialised)
                        {
                            string prodType = request.IsSerialised == true ? "Serialised" : "Process By Pallet";
                            response.Response.Success = true;
                            response.Response.FailureMessage = $"Product is created successfully but you require to scan serials because it is {prodType} product";


                            if (request.IsProcessByPallet) { response.PalletSerialRequired = true; }
                            else { response.SerialRequired = true; }

                            return LogAndRespond(request, response);
                        }
                    }
                }
            }


            var quantity = 1;
            response.ScannedQuantity = quantity;

            if (string.IsNullOrEmpty(request.ProductCode))
            {
                request.ProductCode = request.PalletSerial;
            }

            var product = _productService.GetProductMasterByProductCode(request.ProductCode, request.CurrentTenantId);
            var productSerial = _productService.GetProductSerialBySerialCode(request.ProductCode, request.CurrentTenantId);
            var productPallet = _productService.GetPalletByPalletSerial(request.ProductCode, request.CurrentTenantId);

            if (product == null)
            {
                product = _productService.GetProductMasterByOuterBarcode(request.ProductCode, request.CurrentTenantId);
                if (product != null && product.ProductsPerCase > 0)
                {
                    quantity = quantity * (product.ProductsPerCase ?? 1);
                    request.ScannedQuantity = quantity;
                    response.ScannedQuantity = quantity;
                    type = ScannedCodeTypeEnum.ProductCode;
                }
            }
            else
            {
                type = ScannedCodeTypeEnum.ProductCode;
            }

            if (productSerial != null)
            {
                product = productSerial.ProductMaster;
                response.ProductSerial = productSerial.SerialNo;
                response.ProductCode = product.SKUCode;
                request.ProductSerial = productSerial.SerialNo;
                request.ProductCode = product.SKUCode;
                response.Response.SerialInsteadProduct = true;
                response.Response.Success = true;
                type = ScannedCodeTypeEnum.SerialCode;
            }

            if (productPallet != null)
            {
                product = productPallet.ProductMaster;
                quantity = (int)Math.Round(productPallet.RemainingCases) * (product.ProductsPerCase ?? 1);
                response.ProductCode = product.SKUCode;
                response.PalletSerial = productPallet.PalletSerial;
                response.Response.Success = true;
                type = ScannedCodeTypeEnum.PalletCode;
            }

            if (product == null)
            {
                response.Response.Success = false;
                response.Response.FailureMessage = $"Product with provided code cannot be found. Unknown Code = {request.ProductCode}";
                response.Response.ProductDontExist = true;
                response.ProductCode = request.ProductCode;
                response.Response.ProductCode = request.ProductCode;
                return LogAndRespond(request, response);
            }

            response.ProductId = product.ProductId;

            var inventoryStock =
                _context.InventoryStocks.FirstOrDefault(
                    e => e.WarehouseId == request.WarehouseId && e.TenantId == request.CurrentTenantId &&
                         e.ProductId == product.ProductId);

            if (inventoryStock == null)
            {
                response.ProductName = product.Name;
                response.ProductCode = product.SKUCode;
                response.ProductDescription = product.Name;
                response.InStock = 0;
                response.Response.ProductCode = product.SKUCode;
                response.Allocated = 0;
                response.Available = 0;
            }

            else
            {
                response.ProductName = product.Name;
                response.ProductCode = product.SKUCode;
                response.ProductDescription = product.Name;
                response.InStock = inventoryStock.InStock;
                response.Allocated = inventoryStock.Allocated;
                response.Available = inventoryStock.Available;
            }

            stockDetail.CreatedBy = request.AuthUserId;
            stockDetail.DateScanned = DateTime.UtcNow;
            stockDetail.ProductId = product.ProductId;
            stockDetail.ReceivedSku = product.SKUCode;
            stockDetail.Quantity = request.ScannedQuantity;

            stockDetail.WarehouseId = request.WarehouseId;
            stockDetail.StockTakeId = currentStockTake.StockTakeId;
            stockDetail.TenentId = currentStockTake.TenantId;
            if (!string.IsNullOrEmpty(request.BatchNumber) || request.ExpiryDate.HasValue)
            {
                stockDetail.BatchNumber = request.BatchNumber;
                stockDetail.ExpiryDate = request.ExpiryDate;
                response.ScannedQuantity = request.ScannedQuantity;
                response.BatchNumber = request.BatchNumber;
                response.ExpiryDate = request.ExpiryDate;

            }

            if (product.ProcessByCase == true)
            {
                stockDetail.Quantity = request.ScannedQuantity * (product.ProductsPerCase ?? 1);
                response.ScannedQuantity = request.ScannedQuantity;
            }

            //Product Serial Existence
            if ((product.Serialisable || productSerial != null) && type != ScannedCodeTypeEnum.PalletCode)
            {
                if (product.Serialisable == true && type == ScannedCodeTypeEnum.ProductCode)
                {
                    response.SerialRequired = true;
                }

                if (!string.IsNullOrEmpty(request.ProductSerial))
                {
                    response.ProductSerial = request.ProductSerial;
                    var productSerialToInsert = await _context.ProductSerialization.FirstOrDefaultAsync(
                        m => m.SerialNo.Equals(request.ProductSerial, StringComparison.CurrentCultureIgnoreCase));
                    if (productSerialToInsert != null && productSerialToInsert.ProductId != product.ProductId)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Product Serial belongs to different product (<b>{ productSerialToInsert.ProductMaster.Name }/{productSerialToInsert.ProductMaster.SKUCode}</b>). Your current ProductCode = {request.ProductCode}, Serial Code = {request.ProductSerial}";
                        return LogAndRespond(request, response);
                    }

                    if (productSerialToInsert == null)
                    {
                        var productInsteadSerial = await _context.ProductMaster.FirstOrDefaultAsync(e => e.SKUCode.Equals(request.ProductSerial,
                            StringComparison.CurrentCultureIgnoreCase));

                        if (productInsteadSerial != null)
                        {
                            response.Response.Success = false;
                            response.Response.FailureMessage = $"Product with provided serial cannot be found. But found different Product with this code. ProductCode = {productInsteadSerial.SKUCode},  ProductCode/Serial Code = {request.ProductCode}/{request.ProductSerial}";
                            request.ProductCode = productInsteadSerial.SKUCode;
                            request.ProductSerial = "";
                            return await RecordScannedProducts(request, true);
                        }


                        response.Response.Success = false;
                        response.Response.ProductDontExist = true;
                        response.ProductSerial = request.ProductSerial;
                        response.ProductCode = request.ProductCode;
                        response.Response.FailureMessage = $"Product with provided serial cannot be found. ProductCode = {request.ProductCode}, Serial Code = {request.ProductSerial}";
                        return LogAndRespond(request, response);
                    }

                    var scannedStockSerial = _context.StockTakeDetailsSerials.FirstOrDefault(m => m.ProductId == product.ProductId && m.IsDeleted != true && m.StockTakeDetail.StockTakeId == request.StockTakeId &&
                        m.ProductSerial.SerialNo.Equals(request.ProductSerial, StringComparison.CurrentCultureIgnoreCase));
                    if (scannedStockSerial != null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Product serial is already scanned. Please scan the next serial item. ProductCode = {request.ProductCode}, Serial Code = {request.ProductSerial}";
                        return LogAndRespond(request, response);
                    }

                    _context.Entry(stockDetail).State = EntityState.Added;
                    _context.SaveChanges();
                    var stockTakeDetailId = stockDetail.StockTakeDetailId;

                    response.StockTakeDetailId = stockTakeDetailId;
                    var stockDetailSerial = new StockTakeDetailsSerial()
                    {
                        ProductSerialId = productSerialToInsert.SerialID,
                        ProductId = product.ProductId,
                        StockTakeDetailId = stockTakeDetailId,
                        DateScanned = DateTime.UtcNow,
                        SerialNumber = request.ProductSerial,
                        CreatedBy = request.AuthUserId
                    };

                    _context.Entry(stockDetailSerial).State = EntityState.Added;
                    _context.SaveChanges();

                    return LogAndRespond(request, response);
                }
                else
                {
                    response.Response.Success = false;
                    response.Response.FailureMessage = $"Product is serialised. Please scan the serials. ProductCode = {request.ProductCode}";
                    return LogAndRespond(request, response);
                }
            }

            else if ((product.ProcessByPallet || productPallet != null) && type != ScannedCodeTypeEnum.SerialCode)
            {
                if (product.ProcessByPallet == true && type == ScannedCodeTypeEnum.ProductCode)
                {
                    response.PalletSerialRequired = true;
                }

                if (productPallet != null)
                {
                    request.PalletSerial = productPallet.PalletSerial;
                }


                if (!string.IsNullOrWhiteSpace(request.PalletSerial))
                {

                    response.PalletSerial = request.PalletSerial;

                    var palletSerial = await _context.PalletTracking.FirstOrDefaultAsync(
                        m => m.PalletSerial.Equals(request.PalletSerial, StringComparison.CurrentCultureIgnoreCase));

                    var palletSerialExistsInStocktake = _context.StockTakeDetailsPallets.FirstOrDefault(
                        x => x.PalletSerial.Equals(request.PalletSerial, StringComparison.CurrentCultureIgnoreCase) && x.StockTakeDetail.StockTakeId == request.StockTakeId && x.IsDeleted != true);

                    if (palletSerial == null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Pallet serial not found. Serial = {request.PalletSerial}";
                        return LogAndRespond(request, response);
                    }

                    else if (palletSerial != null && palletSerial.ProductId != product.ProductId)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Pallet serial belongs to a different product. Product = {palletSerial.ProductMaster.NameWithCode} Serial = {request.PalletSerial}";
                        return LogAndRespond(request, response);
                    }

                    else if (palletSerial != null && palletSerialExistsInStocktake != null)
                    {
                        response.Response.Success = false;
                        response.Response.FailureMessage = $"Pallet serial already scanned for this stocktake. Serial = {request.PalletSerial}";
                        return LogAndRespond(request, response);
                    }

                    stockDetail.Quantity = Math.Round(palletSerial.RemainingCases, 2) * (product.ProductsPerCase ?? 1);
                    response.ScannedQuantity = Convert.ToInt32(stockDetail.Quantity);


                    _context.Entry(stockDetail).State = EntityState.Added;
                    _context.SaveChanges();
                    var stockTakeDetailId = stockDetail.StockTakeDetailId;

                    response.StockTakeDetailId = stockTakeDetailId;
                    var stockDetailPalletSerial = new StockTakeDetailsPallets()
                    {
                        ProductPalletId = palletSerial.PalletTrackingId,
                        ProductId = product.ProductId,
                        StockTakeDetailId = stockTakeDetailId,
                        DateScanned = DateTime.UtcNow,
                        PalletSerial = request.PalletSerial,
                        CreatedBy = request.AuthUserId
                    };

                    _context.Entry(stockDetailPalletSerial).State = EntityState.Added;
                    _context.SaveChanges();

                    response.PalletSerial = request.PalletSerial;

                    return LogAndRespond(request, response);
                }

                else
                {
                    response.Response.Success = false;
                    response.Response.FailureMessage = $"Product is processed by pallet, Please scan pallet serials. ProductCode = {request.ProductCode}";
                    return LogAndRespond(request, response);
                }
            }

            else if ((product.RequiresBatchNumberOnReceipt == true || product.RequiresExpiryDateOnReceipt == true) && !request.BatchRequired)
            {
                if (request.LocationCode != null && request.LocationCode != "")
                {
                    response.ScannedQuantity = product.ProductsPerCase ?? 1;
                }


                response.Response.Success = false;
                response.BatchRequired = true;

                response.Response.FailureMessage = $"Product is required Batch number or Expiry date, Please add. ProductCode = {request.ProductCode}";
                return LogAndRespond(request, response);


            }

            else
            {
                //THIS CODE IS WHEN THE USER DECIDES TO JUST HAVE SINGLE ROW PER PRODUCT, INSTEAD OF SINGLE ROW PER SCAN
                _context.Entry(stockDetail).State = EntityState.Added;
                _context.SaveChanges();
                response.StockTakeDetailId = stockDetail.StockTakeDetailId;
            }

            return LogAndRespond(request, response);
        }

        public async Task<ResponseObject> UpdateStockTakeDetailQuantity(StockDetailQuantityUpdateRequest request)
        {
            var stockTakeDetail = await _context.StockTakeDetails.FirstOrDefaultAsync(m => m.StockTakeDetailId == request.StockTakeDetailId && (!m.IsDeleted.HasValue || m.IsDeleted == false));

            if (stockTakeDetail != null)
            {
                stockTakeDetail.Quantity = request.NewQuantity;
                if (stockTakeDetail.Product.ProcessByCase == true)
                {
                    stockTakeDetail.Quantity = request.NewQuantity * (stockTakeDetail.Product.ProductsPerCase ?? 1);

                }
                if (request.BatchNumber != null && request.BatchNumber != "")
                {
                    stockTakeDetail.BatchNumber = request.BatchNumber;
                }
                if (request.ExpiryDate != null && request.ExpiryDate != DateTime.MinValue)
                {
                    stockTakeDetail.ExpiryDate = request.ExpiryDate;
                }

                _context.Entry(stockTakeDetail).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return new ResponseObject() { Success = stockTakeDetail != null };
        }

        public async Task<ResponseObject> DeleteStockTakeDetail(StockDetailDeleteRequest request)
        {
            var stockTakeDetail = await _context.StockTakeDetails.FirstOrDefaultAsync(m => m.StockTakeDetailId == request.StockTakeDetailId && m.IsDeleted != true);

            if (stockTakeDetail != null)
            {
                //delete serials for that detail
                var stockTakeDetailSerials = _context.StockTakeDetailsSerials.Where(m => m.StockTakeDetailId == request.StockTakeDetailId && m.IsDeleted != true).ToList();

                foreach (var item in stockTakeDetailSerials)
                {
                    item.IsDeleted = true;
                    _context.Entry(item).State = EntityState.Modified;
                }

                //delete pallets for that detail
                var stockTakeDetailPallets = _context.StockTakeDetailsPallets.Where(m => m.StockTakeDetailId == request.StockTakeDetailId && m.IsDeleted != true).ToList();

                foreach (var item in stockTakeDetailPallets)
                {
                    item.IsDeleted = true;
                    _context.Entry(item).State = EntityState.Modified;
                }

                stockTakeDetail.IsDeleted = true;
                _context.Entry(stockTakeDetail).State = EntityState.Modified;
                _context.SaveChanges();
            }
            return new ResponseObject() { Success = stockTakeDetail != null };
        }

        public async Task<ResponseObject> CreateProductOnStockTake(ProductDetailRequest request)
        {
            request.ProductCode = string.IsNullOrEmpty(request.ProductCode) ? _productService.GenerateNextProductCode(request.TenantId) : request.ProductCode;

            if (_context.ProductMaster.Any(m => m.SKUCode.Equals(request.ProductCode, StringComparison.CurrentCultureIgnoreCase)))
            {
                return new ResponseObject() { Success = false, FailureMessage = "Product with the same code already exist." };
            }
            if (!_context.TenantConfigs.FirstOrDefault(u => u.TenantId == request.TenantId).AllowDuplicateProductName)
            {
                if (_context.ProductMaster.Any(m => m.Name.Equals(request.ProductName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    return new ResponseObject() { Success = false, FailureMessage = "Product with the same name already exist." };
                }
            }

            if (request.TenantId < 1)
            {
                return new ResponseObject() { Success = false, FailureMessage = "TenantId must be provided." };
            }

            var existingProduct = new ProductMaster
            {
                SKUCode = request.ProductCode,
                Name = request.ProductName,
                DateCreated = DateTime.UtcNow,
                ProdStartDate = DateTime.UtcNow,
                IsActive = true,
                TenantId = request.TenantId,
                IsDeleted = false,
                CreatedBy = 1
            };

            if (request.ProductGroupId.HasValue)
            {
                existingProduct.ProductGroupId = request.ProductGroupId;
            }



            existingProduct.BarCode = existingProduct.SKUCode;

            if (request.IsSerialised)
            {
                existingProduct.Serialisable = true;

                if (!string.IsNullOrEmpty(request.SerialCode))
                {
                    existingProduct.ProductSerialization.Add(new ProductSerialis()
                    {
                        SerialNo = request.SerialCode,
                        CurrentStatus = InventoryTransactionTypeEnum.PurchaseOrder,
                        BuyPrice = 0,
                        WarrantyID = 1,
                        CreatedBy = 1,
                        DateCreated = DateTime.UtcNow,
                        UpdatedBy = 1
                    });
                }
            }

            existingProduct.UOMId = 1;
            existingProduct.DimensionUOMId = 1;
            existingProduct.DepartmentId = request.ProductDepartmentId ?? 1;
            if (request.TaxIds.HasValue)
            {
                existingProduct.EnableTax = true;
            }
            existingProduct.TaxID = request.TaxIds ?? 1;
            existingProduct.LotOptionCodeId = 1;
            existingProduct.LotProcessTypeCodeId = 1;
            existingProduct.WeightGroupId = 1;
            existingProduct.Description = request.ProductDesc;
            _context.ProductMaster.Add(existingProduct);
            _context.SaveChanges();



            _context.InventoryStocks.Add(new InventoryStock()
            {
                ProductId = existingProduct.ProductId,
                InStock = 0,
                CreatedBy = 1,
                DateCreated = DateTime.UtcNow,
                IsActive = true,
                TenantId = request.TenantId,
                WarehouseId = 1
            });


            await _context.SaveChangesAsync().ConfigureAwait(false);
            var productGroup = _context.ProductGroups.FirstOrDefault(u => u.ProductGroupId == existingProduct.ProductGroupId)?.ProductGroup;
            return new ResponseObject() { ProductDontExist = false, Success = true, ResponseTime = DateTime.UtcNow, SerialRequired = existingProduct.Serialisable, ProductCode = existingProduct.SKUCode, ProductId = existingProduct.ProductId, ProductGroup = productGroup };
        }

        private StockTakeProductCodeScanResponse LogAndRespond(StockTakeProductCodeScanRequest request, StockTakeProductCodeScanResponse response)
        {
            var log = new StockTakeScanLog();
            request.LoadLog(log);
            response.LoadLog(log);
            _context.Entry(log).State = EntityState.Added;
            _context.SaveChanges();
            return response;
        }

        public StockTake GetStockTakeById(int id)
        {
            return _context.StockTakes.FirstOrDefault(e => e.IsDeleted != true && e.StockTakeId == id);
        }

        public StockTake StopStockTake(int stockTakeId)
        {
            StockTake model = _context.StockTakes.FirstOrDefault(e => e.StockTakeId == stockTakeId);
            model.DateUpdated = DateTime.UtcNow;
            model.EndDate = DateTime.UtcNow;
            model.Status = 1;

            _context.Entry(model).State = EntityState.Modified;
            _context.SaveChanges();
            return model;
        }

        public StockTake CreateStockTake(StockTake stockTake, int userId, int tenantId, int warehouseId)
        {
            stockTake.DateUpdated = DateTime.UtcNow;
            stockTake.UpdatedBy = userId;

            stockTake.DateCreated = DateTime.UtcNow;
            stockTake.DateUpdated = DateTime.UtcNow;
            stockTake.IsDeleted = false;
            stockTake.CreatedBy = userId;
            stockTake.UpdatedBy = userId;
            stockTake.TenantId = tenantId;
            stockTake.Status = 0;
            stockTake.StartDate = DateTime.UtcNow;
            stockTake.WarehouseId = warehouseId;

            _context.StockTakes.Add(stockTake);
            int Result = _context.SaveChanges();

            if (Result > 0)
            {

                // get all products in specific warehouse
                var Products = (from e in _context.ProductMaster
                                where e.IsDeleted != true && e.DontMonitorStock != true
                                select new
                                {
                                    ProductId = e.ProductId,
                                    SKUCode = e.SKUCode,
                                    IsSerialised = e.Serialisable,
                                    ProcessByPallet = e.ProcessByPallet
                                }).ToList();

                // get all the inventory and store in memory
                var Stocks = (from e in _context.InventoryStocks
                              where e.WarehouseId == warehouseId && e.TenantId == tenantId
                              select new
                              {
                                  InStock = e.InStock,
                                  ProductId = e.ProductId,
                              }).ToList();


                foreach (var Prod in Products)
                {
                    StockTakeSnapshot snap = new StockTakeSnapshot();

                    snap.StockTakeId = stockTake.StockTakeId;
                    snap.ProductId = Prod.ProductId;
                    snap.ReceivedSku = Prod.SKUCode;

                    decimal Qty = Stocks.Where(e => e.ProductId == Prod.ProductId).Select(x => x.InStock).FirstOrDefault();

                    snap.PreviousQuantity = Qty;
                    snap.UpdatedBy = userId;
                    snap.CreatedBy = userId;
                    snap.TenentId = tenantId;
                    snap.DateCreated = DateTime.UtcNow;
                    snap.DateUpdated = DateTime.UtcNow;

                    InventoryTransactionTypeEnum[] inStockStatuses = { InventoryTransactionTypeEnum.PurchaseOrder, InventoryTransactionTypeEnum.TransferIn, InventoryTransactionTypeEnum.AdjustmentIn, InventoryTransactionTypeEnum.Returns };

                    if (Prod.IsSerialised)
                    {
                        var prodSerials = _context.ProductSerialization.Where(p => p.ProductId == Prod.ProductId).ToList();
                        foreach (var item in prodSerials)
                        {
                            var serialSnap = new StockTakeSerialSnapshot
                            {
                                ProductId = Prod.ProductId,
                                ProductSerialId = item.SerialID,
                                StockTake = stockTake,
                                IsInStock = prodSerials.FirstOrDefault() != null &&
                                            inStockStatuses.Contains(prodSerials.First().CurrentStatus),
                                CurrentStatus = prodSerials.FirstOrDefault() != null
                                    ? prodSerials.First().CurrentStatus
                                    : 0
                            };
                            snap.StockTakeSerialSnapshots.Add(serialSnap);
                        }
                    }

                    if (Prod.ProcessByPallet)
                    {
                        var prodPallets = _context.PalletTracking.Where(p => p.ProductId == Prod.ProductId && p.Status != PalletTrackingStatusEnum.Completed).ToList();
                        foreach (var item in prodPallets)
                        {
                            var palletSnap = new StockTakePalletsSnapshot
                            {
                                ProductId = Prod.ProductId,
                                PalletTrackingId = item.PalletTrackingId,
                                StockTake = stockTake,
                                Status = item.Status,
                                RemainingCases = item.RemainingCases,
                                TotalCases = item.TotalCases

                            };
                            snap.StockTakePalletsSnapshot.Add(palletSnap);
                        }
                    }

                    _context.StockTakeSnapshot.Add(snap);
                }
                _context.SaveChanges();
            }
            return stockTake;
        }

        public StockTake UpdateStockTakeStatus(int stockTakeId, int userId, int statusId)
        {
            var stockTake = GetStockTakeById(stockTakeId);
            stockTake.DateUpdated = DateTime.UtcNow;
            stockTake.UpdatedBy = userId;
            stockTake.Status = statusId;
            _context.Entry(stockTake).State = EntityState.Modified;
            _context.SaveChanges();
            return stockTake;
        }

        public StockTake GetStockTakeByStatus(int warehouseId, int statusId, int TenantId)
        {
            return _context.StockTakes.FirstOrDefault(e => e.IsDeleted != true && e.WarehouseId == warehouseId && e.TenantId == TenantId &&
                                                           (e.Status == statusId));
        }
        public IEnumerable<StockTake> GetAllStockTakes(int warehouseId, int TenantId, DateTime? reqDate = null, bool includeIsDeleted = false)
        {
            return _context.StockTakes.Where(e => (includeIsDeleted || e.IsDeleted != true) && (e.WarehouseId == warehouseId || (e.TenantWarehouse.IsMobile == true && e.TenantWarehouse.ParentWarehouseId == warehouseId)) && (!reqDate.HasValue || (e.DateUpdated ?? e.DateCreated) >= reqDate) && e.TenantId == TenantId);
        }

        public List<StockTakeDetailsViewModel> GetStockTakeDetailsByStockTakeId(int stockTakeId)
        {
            return (from e in _context.StockTakeDetails
                    where e.StockTakeId == stockTakeId
                    group e by new { e.ReceivedSku } into eGroup
                    select new StockTakeDetailsViewModel
                    {
                        ProductId = eGroup.FirstOrDefault().ProductId,
                        Quantity = eGroup.Sum(a => a.Quantity),
                        DateScanned = eGroup.FirstOrDefault().DateScanned,
                        StockTakeDetailId = eGroup.FirstOrDefault().StockTakeDetailId,
                        Name = eGroup.FirstOrDefault().Product.Name,
                        Description = eGroup.FirstOrDefault().Product.Description,
                        ReceivedSku = eGroup.FirstOrDefault().Product.SKUCode,
                        Serialisable = eGroup.FirstOrDefault().Product.Serialisable
                    }).ToList();
        }

        public List<StockTakeSnapshotsViewModel> GetStockTakeSnapshotsByStockTakeId(int stockTakeId)
        {
            return (from e in _context.StockTakeSnapshot
                    join t in _context.ProductMaster on e.ProductId equals t.ProductId
                    where e.StockTakeId == stockTakeId && e.IsDeleted != true
                    select new StockTakeSnapshotsViewModel()
                    {
                        ProductId = e.ProductId,
                        ShDesc = t.Name,
                        ReceivedSku = e.ReceivedSku,
                        PreviousQuantity = e.PreviousQuantity
                    }).ToList();
        }

        public string GetStockTakeFullReport(int stockTakeId)
        {
            var sd = (from e in _context.StockTakeDetails where e.StockTakeId == stockTakeId && e.IsDeleted != true group e by new { e.ReceivedSku } into eGroup select eGroup);
            var ss = (from e in _context.StockTakeSnapshot join t in _context.ProductMaster on e.ProductId equals t.ProductId where e.StockTakeId == stockTakeId && t.DontMonitorStock != true && e.IsDeleted != true select e);

            string html = "";

            html += string.Format("<input type=\"hidden\" name=\"Id\" value=\"{0}\" />", stockTakeId);

            if (ss.Any())
            {
                if (sd.Any())
                {
                    html += string.Format("<div class=\"form-horizontal\">");
                    html += "<table width=\"770\" border=\"0\"> <col width=\"30\"> <col width=\"200\"> <col width=\"300\"> <col width=\"120\"> <col width=\"120\">";
                    html += "<strong><tr><th scope=\"col\">&nbsp</th><th scope=\"col\">SKU</th><th scope=\"col\">Short Desc</th><th scope=\"col\">Prev Qty</th><th scope=\"col\">New Qty</th></tr></strong>";

                    foreach (var st in ss)
                    {
                        string ShDesc = st.ProductMaster.Name;

                        if (ShDesc.Length > 30)
                        {
                            ShDesc = ShDesc.Substring(0, 30);
                        }
                        html += "<tr>";
                        var NewQty = st.NewQuantity;
                        html += string.Format("<td><input checked=\"checked\" type=\"checkbox\" name=\"stock\" id=\"{0}\" value=\"{0}-{1}-{2}\"></td>", st.ProductId, st.PreviousQuantity, NewQty);
                        html += string.Format("<td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td>", st.ReceivedSku, ShDesc, st.PreviousQuantity, NewQty);
                        html += "</tr>";
                    }
                    html += "</table>";
                    html += "</div>";
                }
                else
                {
                    html += "<p>No Items Scanned for Stocktake ....</p>";
                }
            }
            else
            {
                html += "<p>No Items/Products for Stocktake ....</p>";
            }
            return html;
        }

        public StockTakeDetail GetStockTakeDetailById(int stockTakeDetailId)
        {
            return _context.StockTakeDetails.Find(stockTakeDetailId);
        }

        public List<StockTakeDetail> GetUnAppliedStockTakeDetailById(int productId, int stockTakeId)
        {
            return _context.StockTakeDetails.Where(m => m.ProductId == productId && m.StockTakeId == stockTakeId && m.IsDeleted != true && m.IsApplied != true).ToList();
        }
        public StockTake ApplyStockTakeChanges(StockTakeApplyChangeRequest request, int userId)
        {
            var productIds = request.RequestItems.Select(m => m.InventoryStockId).ToList();
            var products = _context.ProductMaster.Where(m => productIds.Contains(m.ProductId)).ToList();

            var stockTakeSerials = _context.StockTakeDetailsSerials
                .Where(m => m.StockTakeDetail.StockTakeId == request.StockTakeId && m.IsDeleted != true).ToList();

            var productSerialisations =
                _context.ProductSerialization.Where(m => productIds.Contains(m.ProductId)).ToList();
            foreach (var item in request.RequestItems)
            {

                var inventoryProduct = products.FirstOrDefault(m => m.ProductId == item.InventoryStockId);


                if (inventoryProduct != null)
                {


                    if (inventoryProduct.Serialisable)
                    {
                        var serialStocktakeDetail = stockTakeSerials.Where(m =>  m.ProductId == inventoryProduct.ProductId).ToList();


                        var productSerials = productSerialisations.Where(m => m.ProductId == inventoryProduct.ProductId).ToList();

                        foreach (var productSerial in productSerials)
                        {
                            productSerial.CurrentStatus = serialStocktakeDetail.Select(s => s.SerialNumber).Contains(productSerial.SerialNo) ? InventoryTransactionTypeEnum.AdjustmentIn : InventoryTransactionTypeEnum.AdjustmentOut;
                            productSerial.DateUpdated = DateTime.UtcNow;
                            productSerial.UpdatedBy = userId;
                            _context.Entry(productSerial).State = EntityState.Modified;
                            _context.SaveChanges();

                            Inventory.StockTransaction(inventoryProduct.ProductId, productSerial.CurrentStatus, 1, null, serialStocktakeDetail.FirstOrDefault()?.StockTakeDetail?.LocationId, null, productSerial.SerialID);

                        }
                    }
                    else if (inventoryProduct.ProcessByPallet)
                    {
                        var palletSerials = _context.PalletTracking.Where(m => m.ProductId == inventoryProduct.ProductId && m.Status== PalletTrackingStatusEnum.Active).ToList();
                        var stocktakePallets = _context.StockTakeDetailsPallets.Where(x => x.StockTakeDetail.StockTakeId == request.StockTakeId);
                        foreach (var palletSerial in palletSerials)
                        {
                            decimal casesDifference = 0;
                            InventoryTransactionTypeEnum transType = 0;
                            int? locationId = null;

                            var stocktakePallet = stocktakePallets.FirstOrDefault(x => x.ProductPalletId == palletSerial.PalletTrackingId);

                            if (stocktakePallet != null)
                            {
                                locationId = stocktakePallet.StockTakeDetail.LocationId;

                                if (stocktakePallet.StockTakeDetail.Quantity > 0)
                                {
                                    var cases = stocktakePallet.StockTakeDetail.Quantity / (inventoryProduct.ProductsPerCase ?? 1);


                                    if (palletSerial.Status == PalletTrackingStatusEnum.Created || palletSerial.Status == PalletTrackingStatusEnum.Completed)
                                    {
                                        casesDifference = cases;
                                        transType = InventoryTransactionTypeEnum.AdjustmentIn;

                                        palletSerial.RemainingCases = cases;
                                        palletSerial.Status = PalletTrackingStatusEnum.Active;
                                        palletSerial.DateUpdated = DateTime.UtcNow;
                                        _context.Entry(palletSerial).State = EntityState.Modified;
                                        _context.SaveChanges();

                                    }

                                    else if (cases > palletSerial.RemainingCases)
                                    {

                                        casesDifference = cases - palletSerial.RemainingCases;
                                        transType = InventoryTransactionTypeEnum.AdjustmentIn;

                                        palletSerial.RemainingCases = cases;
                                        palletSerial.Status = PalletTrackingStatusEnum.Active;
                                        palletSerial.DateUpdated = DateTime.UtcNow;
                                        _context.Entry(palletSerial).State = EntityState.Modified;
                                        _context.SaveChanges();
                                    }
                                    else if (cases < palletSerial.RemainingCases)
                                    {
                                        casesDifference = palletSerial.RemainingCases - cases;
                                        transType = InventoryTransactionTypeEnum.AdjustmentOut;

                                        palletSerial.RemainingCases = cases;
                                        palletSerial.Status = PalletTrackingStatusEnum.Active;
                                        palletSerial.DateUpdated = DateTime.UtcNow;
                                        _context.Entry(palletSerial).State = EntityState.Modified;
                                        _context.SaveChanges();
                                    }
                                }
                            }

                            else
                            {
                                if (palletSerial.Status == PalletTrackingStatusEnum.Active)
                                {
                                    casesDifference = palletSerial.RemainingCases;
                                    transType = InventoryTransactionTypeEnum.AdjustmentOut;

                                    palletSerial.RemainingCases = 0;
                                    palletSerial.Status = PalletTrackingStatusEnum.Completed;
                                    palletSerial.DateUpdated = DateTime.UtcNow;
                                    _context.Entry(palletSerial).State = EntityState.Modified;
                                    _context.SaveChanges();
                                }
                            }

                            if (casesDifference > 0)
                            {
                                Inventory.StockTransaction(inventoryProduct.ProductId, transType, casesDifference * (palletSerial.ProductMaster.ProductsPerCase ?? 1),
                                    null, locationId, null, null, palletSerial.PalletTrackingId);
                            }
                        }
                    }

                    else
                    {
                        if (item.PreviousQuantity != item.CurrentQuantity)
                        {
                            var currentAdjustment = item.CurrentQuantity - item.PreviousQuantity;

                            if (currentAdjustment != 0)
                            {

                                InventoryTransactionTypeEnum inventoryTransactionTypeId = currentAdjustment < 0 ? InventoryTransactionTypeEnum.AdjustmentOut : InventoryTransactionTypeEnum.AdjustmentIn;

                                Inventory.StockTransaction(inventoryProduct.ProductId, inventoryTransactionTypeId, Math.Abs(currentAdjustment), null);

                            }

                        }
                    }

                    var productStockDetail = GetUnAppliedStockTakeDetailById(inventoryProduct.ProductId, request.StockTakeId);

                    if (productStockDetail.Count() > 0)
                    {

                        foreach (var detail in productStockDetail)
                        {
                            detail.IsApplied = true;
                            detail.DateApplied = DateTime.UtcNow;
                            _context.Entry(detail).State = EntityState.Modified;
                        }
                    }

                    var productSnapshot = _context.StockTakeSnapshot.FirstOrDefault(m => m.StockTakeId == request.StockTakeId && m.ProductId == inventoryProduct.ProductId && m.IsDeleted != true);

                    if (productSnapshot != null)
                    {
                        productSnapshot.NewQuantity = item.CurrentQuantity;
                        productSnapshot.DateUpdated = DateTime.UtcNow;
                        productSnapshot.UpdatedBy = userId;
                        _context.Entry(productSnapshot).State = EntityState.Modified;
                    }

                    _context.SaveChanges();

                }
            }

            var stockTake = GetStockTakeById(request.StockTakeId);
            if (stockTake != null)
            {
                stockTake.Status = 2;
                stockTake.DateUpdated = DateTime.UtcNow;
                stockTake.UpdatedBy = userId;
                _context.Entry(stockTake).State = EntityState.Modified;
            }

            _context.SaveChanges();
            return stockTake;
        }

        public List<StockTake> GetStockTakesInProgress(int warehouseId)
        {
            return _context.StockTakes
                .Where(e => e.IsDeleted != true && e.WarehouseId == warehouseId && (e.Status == 0 || e.Status == 1)).ToList();
        }
        public List<StockTake> GetStockTakesPendingOrStopped(int warehouseId)
        {
            return _context.StockTakes.Where(e => e.IsDeleted != true && e.WarehouseId == warehouseId && (e.Status == 0 || e.Status == 1)).ToList();
        }
        public List<StockTakeDetailsSerial> GetProductStockTakeSerials(int stockTakeId, int productId)
        {
            return _context.StockTakeDetailsSerials
                .Where(m => m.StockTakeDetail.StockTakeId == stockTakeId && m.ProductId == productId &&
                            m.IsDeleted != true).OrderByDescending(p => p.DateScanned).ToList();
        }


        public StockTakeReportResponse GetStockTakeReportById(int stockTakeId, int tenantId, int warehouseId, int userId, bool justVariations = false)
        {
            var response = new StockTakeReportResponse();

            var currentStockStake = GetStockTakeById(stockTakeId);

            if (currentStockStake == null) return response;

            response.CurrentStockTakeId = currentStockStake.StockTakeId;
            response.StockTakeStatusId = currentStockStake.Status;
            response.AllowApplyChanges = currentStockStake.Status == 1;
            response.StockTakeStatusString = (currentStockStake.Status == 1 ? "Pending" : "Changes Applied");
            response.CurrentStockTakeRef = currentStockStake.StockTakeReference;
            response.CurrentStockTakeDesc = currentStockStake.StockTakeDescription;
            response.CurrentStockTakeDate = currentStockStake.StartDate.ToString("dd/MM/yyyy HH:mm:ss");

            var sd = currentStockStake.StockTakeDetails.GroupBy(m => m.ProductId).Select(p => new { Id = p.Key, Quantity = p.Sum(q => q.Quantity) }).ToList();
            var ss = (from e in _context.StockTakeSnapshot join t in _context.ProductMaster on e.ProductId equals t.ProductId where e.StockTakeId == stockTakeId && t.DontMonitorStock != true select e).ToList();

            if (sd == null || !sd.Any())
            {
                response.HasError = true;
                response.ErrorMessage = "There is no stock snapshot found with provided id.";
                return response;
            }

            var allInventory = _productService.GetAllValidProductMasters(tenantId, null, true).ToList();

            var inventorySnapshotDetails = allInventory.Select(m => new StockTakeReportResponseItem()
            {
                ProductCode = m.SKUCode,
                ProductId = m.ProductId,
                ProductName = m.Name,
                IsSerialised = m.Serialisable,
                ProductDescription = m.Description,
                CurrentQuantity = sd.FirstOrDefault(p => p.Id == m.ProductId) != null ? sd.FirstOrDefault(p => p.Id == m.ProductId).Quantity : 0m,
                PreviousQuantity = ss.FirstOrDefault(p => p.ProductId == m.ProductId) != null ? ss.FirstOrDefault(p => p.ProductId == m.ProductId).PreviousQuantity : 0m
            }).ToList();

            response.StockTakeReportResponseItems = justVariations ? inventorySnapshotDetails.Where(m => m.CurrentQuantity != m.PreviousQuantity).ToList() : inventorySnapshotDetails;

            return response;
        }
        public bool DeleteStockTakeDetial(int stockTakeDetailId)
        {
            var result = _context.StockTakeDetails.Find(stockTakeDetailId);
            if (result != null)
            {
                result.IsDeleted = true;
                _context.StockTakeDetails.Attach(result);
                _context.Entry(result).State = EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        public IQueryable<object> GetDetialStock(int tenantId, int warehouseId, int stockTakeId)
        {
            return from sd in _context.StockTakeDetails
                   join sp in _context.StockTakeDetailsPallets
                   on sd.StockTakeDetailId equals sp.StockTakeDetailId
                   into stockDetailsGroups
                   join ss in _context.StockTakeDetailsSerials on sd.StockTakeDetailId equals ss.StockTakeDetailId
                   into stockDetailsGroup
                   from sdg in stockDetailsGroup.DefaultIfEmpty()
                   from sdgs in stockDetailsGroups.DefaultIfEmpty()
                   where sd.TenentId == tenantId && sd.WarehouseId == warehouseId && sd.StockTakeId == stockTakeId && sd.IsDeleted != true
                   select new
                   {
                       StockTakeDetailId = sd.StockTakeDetailId,
                       ProductName = sd.Product.Name,
                       ReceivedSku = sd.ReceivedSku,
                       Quantity = sd.Quantity,
                       DateScanned = sd.DateScanned,
                       DateApplied = sd.DateApplied,
                       PalletSerial = sdgs == null ? "" : sdgs.PalletSerial,
                       SerialNumber = sdg == null ? "" : sdg.SerialNumber,
                       BatchNumber = sd.BatchNumber,
                       ExpiryDate = sd.ExpiryDate
                   };



        }

        public StockTakeDetail CreateStockTakeDetail(StockTakeDetail detail)
        {
            _context.StockTakeDetails.Add(detail);
            _context.SaveChanges();
            return detail;
        }

        public StockTakeDetailsPallets CreateStockTakeDetailsPallets(StockTakeDetailsPallets detailPallet)
        {
            _context.StockTakeDetailsPallets.Add(detailPallet);
            _context.SaveChanges();
            return detailPallet;
        }

    }
}
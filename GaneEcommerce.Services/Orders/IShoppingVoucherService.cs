using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Data.WcfLinq.Helpers;
using Ganedata.Core.Data;
using Ganedata.Core.Entities.Domain;
using Ganedata.Core.Entities.Enums;
using Ganedata.Core.Models.AdyenPayments;

namespace Ganedata.Core.Services
{
    public interface IShoppingVoucherService
    {
        ShoppingVoucherValidationResponseModel ValidateVoucher(ShoppingVoucherValidationRequestModel request);
        ShoppingVoucherValidationResponseModel ApplyVoucher(ShoppingVoucherValidationRequestModel request);
        ShoppingVoucher GetShoppingVoucherById(int shoppingVoucherId);
        List<ShoppingVoucher> GetAllValidShoppingVouchers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false);
        ShoppingVoucher GetShoppingVoucherByVoucherCode(string voucherCode);
        ShoppingVoucher SaveShoppingVoucher(ShoppingVoucher voucher, int userId);
        void DeleteShoppingVoucher(int voucherId, int userId);
        List<ShoppingVoucherValidationResponseModel> GetAllValidUserVouchers(int userId, string email, string phone);
    }

    public class ShoppingVoucherService : IShoppingVoucherService
    {
        private readonly IApplicationContext _currentDbContext;

        public ShoppingVoucherService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public ShoppingVoucherValidationResponseModel MapVoucherToModel(ShoppingVoucher voucher)
        {
            var response = new ShoppingVoucherValidationResponseModel() { VerifiedTimestamp = DateTime.Now, DiscountFigure = 0, VoucherCode = voucher?.VoucherCode, Status = (int)ShoppingVoucherStatus.Active };

            if (voucher == null)
            {
                response.Status = (int)ShoppingVoucherStatus.Invalid;
                return response;
            }

            response.UserId = voucher.VoucherUserId ?? 0;
            response.VoucherTitle = voucher.VoucherTitle;
            response.MinimumOrderPrice = voucher.MinimumOrderPrice??0;

            if (voucher.VoucherExpiryDate < DateTime.Now)
            {
                response.Status = (int)ShoppingVoucherStatus.Expired;
            }

            if (voucher.MaximumAllowedUse > 0)
            {
                if (voucher.VoucherUsedCount >= voucher.MaximumAllowedUse)
                {
                    response.Status = (int)ShoppingVoucherStatus.UsedMaximum;
                }
            }
            else if (voucher.VoucherUsedDate.HasValue)
            {
                response.UsedDate = voucher.VoucherUsedDate;
                response.Status = (int)ShoppingVoucherStatus.Applied;
            }

            if (response.Status == (int)ShoppingVoucherStatus.Active)
            {
                response.DiscountFigure = voucher.DiscountFigure;
                response.DiscountType = (int)voucher.DiscountType;
            }

            response.RewardProductId = voucher.RewardProductId;

            return response;
        }

        public ShoppingVoucherValidationResponseModel ValidateVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var voucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                m.VoucherCode.Equals(request.VoucherCode) && (!m.VoucherUserId.HasValue || (request.UserId.HasValue && m.VoucherUserId == request.UserId)));
            return MapVoucherToModel(voucher);
        }

        public ShoppingVoucherValidationResponseModel ApplyVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var voucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                m.VoucherCode.Equals(request.VoucherCode) && ((!m.VoucherUserId.HasValue || (request.UserId.HasValue && m.VoucherUserId == request.UserId)))||(m.MaximumAllowedUse>0 && m.MaximumAllowedUse>m.VoucherUsedCount));
            if (voucher == null)
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Invalid
                };
            }
            voucher.VoucherUsedDate = DateTime.Now;
            voucher.UpdatedBy = request.UserId;
            voucher.DateUpdated = DateTime.Now;
            voucher.VoucherUsedCount = (voucher.MaximumAllowedUse ?? 0) + 1;
            _currentDbContext.Entry(voucher).State = EntityState.Modified;
            _currentDbContext.SaveChanges();

            var usage = new ShoppingVoucherUsage()
            {
                CreatedBy = request.UserId??0,
                DateCreated = DateTime.Now,
                ShoppingVoucher = voucher,
                OrderId = request.OrderId,
                VoucherUserId = request.UserId
            };
            _currentDbContext.ShoppingVoucherUsages.Add(usage);
            _currentDbContext.SaveChanges();

            return new ShoppingVoucherValidationResponseModel()
            {
                DiscountFigure = voucher.DiscountFigure,
                DiscountType = (int)voucher.DiscountType,
                Status = (int)ShoppingVoucherStatus.Applied,
                VerifiedTimestamp = DateTime.Now,
                VoucherCode = voucher.VoucherCode
            };
        }

        public ShoppingVoucher GetShoppingVoucherById(int shoppingVoucherId)
        {
            return _currentDbContext.ShoppingVouchers.FirstOrDefault(m => m.ShoppingVoucherId == shoppingVoucherId);
        }

        public List<ShoppingVoucher> GetAllValidShoppingVouchers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false)
        {
            return _currentDbContext.ShoppingVouchers.Where(m =>
                (includeDeleted || m.IsDeleted != true) && (tenantId == 0 || m.TenantId == tenantId) &&
                (!onlyUpdatedAfter.HasValue || (m.DateUpdated ?? m.DateCreated) > onlyUpdatedAfter)).ToList();
        }

        public ShoppingVoucher GetShoppingVoucherByVoucherCode(string voucherCode)
        {
            return _currentDbContext.ShoppingVouchers.FirstOrDefault(m => m.VoucherCode == voucherCode);
        }

        public ShoppingVoucher SaveShoppingVoucher(ShoppingVoucher voucher, int userId)
        {
            if (voucher.TenantId == 0)
            {
                voucher.TenantId = null;
            }

            if (voucher.WarehouseId == 0)
            {
                voucher.WarehouseId = null;
            }

            if (voucher.ShoppingVoucherId > 0)
            {
                voucher.UpdatedBy = userId;
                voucher.DateUpdated = DateTime.Now;
                _currentDbContext.Entry(voucher).State = EntityState.Modified;
                _currentDbContext.Entry(voucher).Property(m => m.DateCreated).IsModified = false;
                _currentDbContext.Entry(voucher).Property(m => m.CreatedBy).IsModified = false;
            }
            else
            {
                voucher.CreatedBy = userId;
                voucher.DateCreated = DateTime.Now;
                _currentDbContext.ShoppingVouchers.Add(voucher);
            }

            _currentDbContext.SaveChanges();
            return voucher;
        }

        public void DeleteShoppingVoucher(int voucherId, int userId)
        {
            var voucher = GetShoppingVoucherById(voucherId);
            voucher.IsDeleted = true;
            SaveShoppingVoucher(voucher, userId);
        }

        public List<ShoppingVoucherValidationResponseModel> GetAllValidUserVouchers(int userId, string email, string phone)
        {
            return _currentDbContext.ShoppingVouchers.Where(m =>
                m.IsDeleted != true && userId != 0 && m.VoucherUser!=null 
                && (m.VoucherUser.UserEmail!=null && m.VoucherUser.UserEmail.ToLower().Equals(email.ToLower())) 
                && (m.VoucherUser.UserMobileNumber!=null && m.VoucherUser.UserMobileNumber.EndsWith(phone.Trim()) )).Select(MapVoucherToModel).ToList();
        }
    }
}
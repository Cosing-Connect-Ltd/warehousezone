using System;
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
    }

    public class ShoppingVoucherService : IShoppingVoucherService
    {
        private readonly IApplicationContext _currentDbContext;

        public ShoppingVoucherService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }
        public ShoppingVoucherValidationResponseModel ValidateVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var voucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                m.VoucherCode.Equals(request.VoucherCode) && (!m.VoucherUserId.HasValue || (request.UserId.HasValue && m.VoucherUserId == request.UserId)));

            var response = new ShoppingVoucherValidationResponseModel(){ VerifiedTimestamp = DateTime.Now, DiscountFigure = 0, VoucherCode = request.VoucherCode, Status = (int)ShoppingVoucherStatus.Active };

            if (voucher == null)
            {
                response.Status = (int) ShoppingVoucherStatus.Invalid;
                return response;
            }

            if (voucher.VoucherExpiryDate < DateTime.Now)
            {
                response.Status = (int)ShoppingVoucherStatus.Expired;
            }
            if (voucher.VoucherUsedDate.HasValue)
            {
                response.Status = (int)ShoppingVoucherStatus.Applied;
            }

            if (voucher.MaximumAllowedUse > 0 && voucher.VoucherUsedCount >= voucher.MaximumAllowedUse)
            {
                response.Status = (int)ShoppingVoucherStatus.UsedMaximum;
            }

            if (response.Status == (int) ShoppingVoucherStatus.Active)
            {
                response.DiscountFigure = voucher.DiscountFigure;
                response.DiscountType = (int) voucher.DiscountType;
            }

            return response;
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
    }
}
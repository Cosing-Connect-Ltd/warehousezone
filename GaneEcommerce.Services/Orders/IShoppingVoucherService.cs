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

            if (voucher == null)
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Invalid
                };
            }

            if (voucher.VoucherExpiryDate < DateTime.Now)
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Expired
                };
            }
            if (voucher.VoucherUsedDate.HasValue)
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Applied
                };
            }

            return new ShoppingVoucherValidationResponseModel()
            {
                DiscountFigure = voucher.DiscountFigure,
                DiscountType = (int)voucher.DiscountType,
                Status = (int)ShoppingVoucherStatus.Applied,
                VerifiedTimestamp = DateTime.Now,
                VoucherCode = voucher.VoucherCode
            };
        }

        public ShoppingVoucherValidationResponseModel ApplyVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var voucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                m.VoucherCode.Equals(request.VoucherCode) && (!m.VoucherUserId.HasValue || (request.UserId.HasValue && m.VoucherUserId == request.UserId)));
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
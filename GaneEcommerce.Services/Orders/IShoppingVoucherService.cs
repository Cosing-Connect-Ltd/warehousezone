using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.WebPages;
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
        ShoppingVoucherValidationResponseModel AddFriendReferralVoucher(ShoppingVoucherValidationRequestModel request);
        ShoppingVoucherValidationResponseModel ApplyVoucher(ShoppingVoucherValidationRequestModel request);
        ShoppingVoucher GetShoppingVoucherById(int shoppingVoucherId);
        List<ShoppingVoucher> GetAllValidShoppingVouchers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false);
        ShoppingVoucher GetShoppingVoucherByVoucherCode(string voucherCode);
        ShoppingVoucher SaveShoppingVoucher(ShoppingVoucher voucher, int userId);
        void DeleteShoppingVoucher(int voucherId, int userId);
        List<ShoppingVoucherValidationResponseModel> GetAllValidUserVouchers(int userId, string email, string phone);
        bool LoadDefaultSystemVouchersForNewUser(int userId);
        void TriggerRewardsBasedForCurrentVoucher(int userId, int oldPoint, int newPoint, int? orderId = 0, bool noVouchers = false);
    }

    public class ShoppingVoucherService : IShoppingVoucherService
    {
        private readonly IApplicationContext _currentDbContext;

        private static int ReferralFreeRewardProductId = (WebConfigurationManager.AppSettings["ReferralFreeRewardProductId"] ??"290").AsInt();
        private static int LoyaltyPoint400RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint400RewardProductId"] ?? "290").AsInt();
        private static int LoyaltyPoint800RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint800RewardProductId"] ?? "341").AsInt();
        private static int LoyaltyPoint1200RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint1200RewardProductId"] ?? "558").AsInt();

        public static bool EnableDiscountForFirstOnlineOrder = true;
        public static decimal? FirstOrderOnlineDiscountMinimumOrderValue = 5;
        public static int FirstOrderOnlineDiscountPercent = (WebConfigurationManager.AppSettings["LoyaltyPoint1200RewardProductId"] ?? "50").AsInt();
        public static string FirstOrderOnlineDiscountVoucher = "FIRSTONLINEORDER";

        public static bool EnableDiscountForFirstDeliveryOrder = true;
        public static int FirstOrderHomeDeliveryDiscountPercent = (WebConfigurationManager.AppSettings["LoyaltyPoint1200RewardProductId"] ?? "25").AsInt();
        public static string FirstOrderHomeDeliveryDiscountVoucher= "FIRSTHOMEDELORDER";
         
        public ShoppingVoucherService(IApplicationContext currentDbContext)
        {
            _currentDbContext = currentDbContext;
        }

        public ShoppingVoucherValidationResponseModel MapVoucherToModel(ShoppingVoucher voucher)
        {
            var response = new ShoppingVoucherValidationResponseModel()
            {
                VerifiedTimestamp = DateTime.Now, DiscountFigure = 0, VoucherCode = voucher?.VoucherCode, Status = (int)ShoppingVoucherStatus.Active
            };

            if (voucher == null)
            {
                response.Status = (int)ShoppingVoucherStatus.Invalid;
                return response;
            }

            response.UserId = voucher.VoucherUserId ?? 0;
            response.VoucherTitle = voucher.VoucherTitle;
            response.MinimumOrderPrice = voucher.MinimumOrderPrice??0;
            response.ExpiryDate = voucher.VoucherExpiryDate;

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

        public ShoppingVoucherValidationResponseModel AddFriendReferralVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var referredUser= _currentDbContext.AuthUsers.FirstOrDefault(m =>
                m.PersonalReferralCode != null && m.PersonalReferralCode.ToLower().Equals(request.VoucherCode.ToLower()));
            if (referredUser == null)
            {
                var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == request.UserId);
                if (user != null)
                {
                    user.ReferralConfirmed = true;
                    _currentDbContext.Entry(user).State = EntityState.Modified;
                }
                _currentDbContext.SaveChanges();

                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Invalid
                };
            }
            else
            {
                var requestedUser = _currentDbContext.AuthUsers.FirstOrDefault(m =>m.UserId==request.UserId);
                if (_currentDbContext.ShoppingVouchers.Any(m =>  m.CreatedBy == request.UserId && requestedUser.ReferralConfirmed))
                {
                    return new ShoppingVoucherValidationResponseModel()
                    {
                        DiscountFigure = 0,
                        Status = (int)ShoppingVoucherStatus.Expired
                    };
                }

                var product = _currentDbContext.ProductMaster.FirstOrDefault(m => m.ProductId == ReferralFreeRewardProductId);
                var voucher = new ShoppingVoucher()
                {
                    VoucherUserId = referredUser.UserId,
                    VoucherCode = request.VoucherCode,
                    CreatedBy = request.UserId??0,
                    DateCreated = DateTime.Now,
                    DiscountType = ShoppingVoucherDiscountTypeEnum.FreeProduct,
                    RewardProductId = ReferralFreeRewardProductId,
                    DiscountFigure = 0,
                    VoucherTitle = $"Referral Free Product ({product?.Name})",
                    MaximumAllowedUse = 1
                };
                _currentDbContext.ShoppingVouchers.Add(voucher);

                var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == request.UserId);
                if (user != null)
                {
                    user.ReferralConfirmed = true;
                    _currentDbContext.Entry(user).State = EntityState.Modified;
                }

                _currentDbContext.SaveChanges();
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int)ShoppingVoucherStatus.Active
                };
            }
        }
        public string GetNextUniquePersonalVoucherCode()
        {
            var code = string.Empty;
            var isNewCode = false;
            while (!isNewCode)
            {
                code = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                isNewCode = !_currentDbContext.ShoppingVouchers.Any(m => m.VoucherCode != null && m.VoucherCode.ToUpper().Equals(code));
            }
            return code;
        }

        private AccountRewardPoint GetAccountRewardPoint(int accountId, int userId, int? orderId, int rewardPoint, int productId)
        {
            return new AccountRewardPoint()
            {
                DateCreated = DateTime.Now,
                AccountID = accountId,
                OrderID = orderId??0,
                CreatedBy = userId,
                OrderDateTime = DateTime.Now,
                PointsEarned = rewardPoint,
                UserID = userId,
                RewardProductId = productId
            };
        }

        public void TriggerRewardsBasedForCurrentVoucher(int userId, int oldPoint, int newPoint, int? orderId = null, bool noVouchers=false)
        {
            var user = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == userId && m.IsActive && m.IsDeleted != true);
            if (user != null && user.AccountId.HasValue)
            {
                var account = _currentDbContext.Account.FirstOrDefault(m => m.AccountID == user.AccountId);
                if (account != null)
                {
                    if (noVouchers)
                    {
                        var defaultVouchers = _currentDbContext.ShoppingVouchers.Where(m => m.VoucherUserId == userId && m.VoucherUsedDate == null
                                                                                                                      && (m.VoucherCode.ToLower().Equals(FirstOrderHomeDeliveryDiscountVoucher) || m.VoucherCode.ToLower().Equals(FirstOrderOnlineDiscountVoucher)));
                        foreach (var voucher in defaultVouchers)
                        {
                            voucher.IsDeleted =true;
                            voucher.DateUpdated = DateTime.Now;
                            _currentDbContext.Entry(voucher).State = EntityState.Modified;
                        }
                        _currentDbContext.SaveChanges();
                    }

                    if (oldPoint < 400 && newPoint >= 400 && newPoint<800)
                    {
                        var product400 = _currentDbContext.ProductMaster.FirstOrDefault(m =>  m.ProductId == LoyaltyPoint400RewardProductId);
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        var voucher400 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint400RewardProductId, $"400 Loyalty Points - Free ({product400?.Name})");
                        _currentDbContext.ShoppingVouchers.Add(voucher400);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -400, LoyaltyPoint400RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);
                        _currentDbContext.SaveChanges();
                        return;
                    }
                    if (oldPoint < 800 && newPoint >= 800 && newPoint < 1200)
                    {
                        var product800 = _currentDbContext.ProductMaster.FirstOrDefault(m => m.ProductId == LoyaltyPoint800RewardProductId);
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        var voucher800 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint800RewardProductId, $"800 Loyalty Points - Free ({product800?.Name})");
                        _currentDbContext.ShoppingVouchers.Add(voucher800);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -800, LoyaltyPoint800RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);
                        _currentDbContext.SaveChanges();
                        return;
                    }
                    if (oldPoint < 1200 && newPoint >= 1200)
                    {
                        var product1200 = _currentDbContext.ProductMaster.FirstOrDefault(m => m.ProductId == LoyaltyPoint1200RewardProductId);
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        var voucher1200 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint1200RewardProductId, $"1200 Loyalty Points - Free ({product1200?.Name})");
                        _currentDbContext.ShoppingVouchers.Add(voucher1200);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -1200, LoyaltyPoint1200RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);

                        account.AccountLoyaltyPoints = newPoint - 1200;
                        _currentDbContext.Entry(account).State = EntityState.Modified;
                        _currentDbContext.SaveChanges();
                        return;
                    }
                }

            }

        }

        private ShoppingVoucher GetVoucher(string voucherCode, int userId, ShoppingVoucherDiscountTypeEnum discountType,  int createdUserId, decimal discountFigure = 0, int? rewardProductId = null, string title="")
        {
            var voucher = new ShoppingVoucher()
            {
                VoucherUserId = userId,
                VoucherCode = voucherCode,
                CreatedBy = createdUserId,
                DateCreated = DateTime.Now,
                DiscountType = discountType,
                DiscountFigure = discountFigure,
                RewardProductId = rewardProductId,
                VoucherTitle = title,
                MaximumAllowedUse = 1
            };
            return voucher;
        }

        public bool LoadDefaultSystemVouchersForNewUser(int userId)
        {
            var newlyApplied = false;
            if (EnableDiscountForFirstOnlineOrder && !_currentDbContext.ShoppingVouchers.Any(m => m.VoucherCode.ToLower().Equals(FirstOrderOnlineDiscountVoucher) && m.VoucherUserId == userId))
            {
                var firstOnlineOrderVoucher = GetVoucher(FirstOrderOnlineDiscountVoucher, userId,
                    ShoppingVoucherDiscountTypeEnum.Percentage, userId, FirstOrderOnlineDiscountPercent, null, FirstOrderOnlineDiscountPercent+ "% off first order");
                firstOnlineOrderVoucher.MinimumOrderPrice = FirstOrderOnlineDiscountMinimumOrderValue;
                firstOnlineOrderVoucher.VoucherExpiryDate = DateTime.Now.AddDays(10);
                _currentDbContext.ShoppingVouchers.Add(firstOnlineOrderVoucher);
                _currentDbContext.SaveChanges();
                newlyApplied = true;
            }

            if (EnableDiscountForFirstDeliveryOrder && !_currentDbContext.ShoppingVouchers.Any(m => m.VoucherCode.ToLower().Equals(FirstOrderHomeDeliveryDiscountVoucher) && m.VoucherUserId == userId))
            {
                var firstOnlineOrderVoucher = GetVoucher(FirstOrderHomeDeliveryDiscountVoucher, userId,
                    ShoppingVoucherDiscountTypeEnum.Percentage, userId, FirstOrderHomeDeliveryDiscountPercent, null, FirstOrderHomeDeliveryDiscountPercent+"% off first order");
                firstOnlineOrderVoucher.VoucherExpiryDate = DateTime.Now.AddDays(10);
                _currentDbContext.ShoppingVouchers.Add(firstOnlineOrderVoucher);
                _currentDbContext.SaveChanges();
                newlyApplied = true;
            }
            return newlyApplied;
        }

        public ShoppingVoucherValidationResponseModel ApplyVoucher(ShoppingVoucherValidationRequestModel request)
        {
            var voucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                m.VoucherCode.Equals(request.VoucherCode) 
                && (!m.VoucherUserId.HasValue || (request.UserId.HasValue && m.VoucherUserId == request.UserId))
                && m.VoucherUsedDate == null
                && ((m.MaximumAllowedUse>0 && m.MaximumAllowedUse>m.VoucherUsedCount)|| m.MaximumAllowedUse == null || m.VoucherUsedCount == null));
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

            //Only one of the voucher can be used, not both
            if (voucher.VoucherCode.Equals(FirstOrderOnlineDiscountVoucher, StringComparison.CurrentCultureIgnoreCase) || voucher.VoucherCode.Equals(FirstOrderHomeDeliveryDiscountVoucher, StringComparison.CurrentCultureIgnoreCase))
            {
                var relatedVoucher = _currentDbContext.ShoppingVouchers.FirstOrDefault(m =>
                    (m.VoucherCode.ToLower().Equals(FirstOrderOnlineDiscountVoucher.ToLower()) ||
                    m.VoucherCode.ToLower().Equals(FirstOrderHomeDeliveryDiscountVoucher.ToLower())) && m.VoucherUsedDate == null && m.VoucherUserId==request.UserId);
                if (relatedVoucher != null)
                {
                    relatedVoucher.VoucherUsedDate = DateTime.Now;
                    _currentDbContext.Entry(relatedVoucher).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }
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

        public ShoppingVoucher GetShoppingVoucherById(int shoppingVoucherId)
        {
            return _currentDbContext.ShoppingVouchers.FirstOrDefault(m => m.ShoppingVoucherId == shoppingVoucherId);
        }

        public List<ShoppingVoucher> GetAllValidShoppingVouchers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false)
        {
            return _currentDbContext.ShoppingVouchers.Where(m =>
                (includeDeleted || m.IsDeleted != true) && (tenantId == 0 || (m.TenantId == tenantId || m.TenantId==null)) &&
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
            var vouchers = _currentDbContext.ShoppingVouchers.Where(m =>
                m.IsDeleted != true && userId != 0 && m.VoucherUser!=null && m.VoucherUsedDate ==null
                && (m.VoucherUser.UserEmail!=null && m.VoucherUser.UserEmail.ToLower().Equals(email.ToLower())) 
                && (m.VoucherUser.UserMobileNumber!=null && m.VoucherUser.UserMobileNumber.EndsWith(phone.Trim()) )).Select(MapVoucherToModel).ToList();
            return vouchers;
        }
    }
}
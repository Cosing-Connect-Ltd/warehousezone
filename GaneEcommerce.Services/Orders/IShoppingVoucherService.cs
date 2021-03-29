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
        RewardPointTrigger GetRewardTriggerById(int id);
        RewardPointTrigger SaveRewardTrigger(RewardPointTrigger trigger, int userId);

        List<RewardPointTrigger> GetAllValidRewardTriggers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false);
    }

    public class ShoppingVoucherService : IShoppingVoucherService
    {
        private readonly IApplicationContext _currentDbContext;

        private static int ReferralFreeRewardProductId = (WebConfigurationManager.AppSettings["ReferralFreeRewardProductId"] ?? "309").AsInt();
        private static int LoyaltyPoint400RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint400RewardProductId"] ?? "309").AsInt();
        private static int LoyaltyPoint800RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint800RewardProductId"] ?? "94").AsInt();
        private static int LoyaltyPoint1200RewardProductId = (WebConfigurationManager.AppSettings["LoyaltyPoint1200RewardProductId"] ?? "381").AsInt();

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
            response.RewardProductCategory = voucher.RewardProductCategory;
            response.RewardProductId = voucher.RewardProductId;

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
            var requestedUser = _currentDbContext.AuthUsers.FirstOrDefault(m => m.UserId == request.UserId);

            if (string.IsNullOrWhiteSpace(request.VoucherCode) && request.UserId > 0)
            {
                if (requestedUser != null)
                {
                    requestedUser.ReferralConfirmed = true;
                    _currentDbContext.Entry(requestedUser).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }

                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int) ShoppingVoucherStatus.Applied
                };
            }

            var referredUser = _currentDbContext.AuthUsers.FirstOrDefault(m =>
                m.PersonalReferralCode != null && m.PersonalReferralCode.ToLower().Equals(request.VoucherCode.ToLower()));

            if (referredUser == null)
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int) ShoppingVoucherStatus.Invalid
                };
            }

            if (_currentDbContext.ShoppingVouchers.Any(m => m.CreatedBy == request.UserId && requestedUser.ReferralConfirmed))
            {
                return new ShoppingVoucherValidationResponseModel()
                {
                    DiscountFigure = 0,
                    Status = (int) ShoppingVoucherStatus.Expired
                };
            }

            var product = _currentDbContext.ProductMaster.FirstOrDefault(m => m.ProductId == ReferralFreeRewardProductId);
            var voucher = new ShoppingVoucher()
            {
                VoucherUserId = referredUser.UserId,
                VoucherCode = request.VoucherCode,
                CreatedBy = request.UserId ?? 0,
                DateCreated = DateTime.Now,
                DiscountType = ShoppingVoucherDiscountTypeEnum.FreeProduct,
                RewardProductId = ReferralFreeRewardProductId,
                DiscountFigure = 0,
                VoucherTitle = $"Referral Free Product ({product?.Name})",
                MaximumAllowedUse = 1,
                RewardProductCategory = RewardProductCategoryEnum.Milkshake
            };
            _currentDbContext.ShoppingVouchers.Add(voucher);

            if (requestedUser != null)
            {
                requestedUser.ReferralConfirmed = true;
                _currentDbContext.Entry(requestedUser).State = EntityState.Modified;
            }

            _currentDbContext.SaveChanges();
            return new ShoppingVoucherValidationResponseModel()
            {
                DiscountFigure = 0,
                Status = (int) ShoppingVoucherStatus.Active
            };
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
            var rewardTriggers = _currentDbContext.RewardPointTriggers
                .Where(m => m.TriggerType == VoucherDiscountTriggerEnum.OnReachingPoints)
                .OrderBy(m => m.LoyaltyPointToTrigger).ToList();

            var point1 = rewardTriggers[0];
            var point2 = rewardTriggers[1];
            var point3 = rewardTriggers[2];

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

                    if (oldPoint < point1.LoyaltyPointToTrigger && newPoint >= point1.LoyaltyPointToTrigger && newPoint< point2.LoyaltyPointToTrigger)
                    {
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        
                        var voucher400 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint400RewardProductId, $"{point1.LoyaltyPointToTrigger} Loyalty Points - Free Milkshake", RewardProductCategoryEnum.Milkshake);
                        voucher400.RewardProductId = point1.ShoppingVoucher.RewardProductId;
                        voucher400.DiscountType = point1.ShoppingVoucher.DiscountType;
                        voucher400.DiscountFigure = point1.ShoppingVoucher.DiscountFigure;

                        _currentDbContext.ShoppingVouchers.Add(voucher400);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -(point1.LoyaltyPointToTrigger??400), LoyaltyPoint400RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);
                        _currentDbContext.SaveChanges();
                    }
                    else if (oldPoint < point2.LoyaltyPointToTrigger && newPoint >= point2.LoyaltyPointToTrigger && newPoint < point3.LoyaltyPointToTrigger)
                    {
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        var voucher800 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint800RewardProductId, $"{point2.LoyaltyPointToTrigger} Loyalty Points - Free Cheesecake", RewardProductCategoryEnum.CheeseCake);
                        voucher800.RewardProductId = point2.ShoppingVoucher.RewardProductId;
                        voucher800.DiscountType = point2.ShoppingVoucher.DiscountType;
                        voucher800.DiscountFigure = point2.ShoppingVoucher.DiscountFigure;
                        _currentDbContext.ShoppingVouchers.Add(voucher800);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -(point2.LoyaltyPointToTrigger ?? 800), LoyaltyPoint800RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);
                        _currentDbContext.SaveChanges();
                    }
                    else if (oldPoint < point3.LoyaltyPointToTrigger && newPoint >= point3.LoyaltyPointToTrigger)
                    {
                        var freeItemVoucher = GetNextUniquePersonalVoucherCode();
                        var voucher1200 = GetVoucher(freeItemVoucher, userId,
                            ShoppingVoucherDiscountTypeEnum.FreeProduct, userId, 0, LoyaltyPoint1200RewardProductId, $"{point3.LoyaltyPointToTrigger} Loyalty Points - Free Choc Cookie Dough", RewardProductCategoryEnum.CookieDough);
                        voucher1200.RewardProductId = point3.ShoppingVoucher.RewardProductId;
                        voucher1200.DiscountType = point3.ShoppingVoucher.DiscountType;
                        voucher1200.DiscountFigure = point3.ShoppingVoucher.DiscountFigure;
                        _currentDbContext.ShoppingVouchers.Add(voucher1200);

                        var rewardUse = GetAccountRewardPoint(account.AccountID, userId, orderId, -(point3.LoyaltyPointToTrigger ?? 1200), LoyaltyPoint1200RewardProductId);
                        _currentDbContext.AccountRewardPoints.Add(rewardUse);
                        _currentDbContext.SaveChanges();
                    }

                    if (newPoint > (point3.LoyaltyPointToTrigger ?? 1200))
                    {
                        newPoint = newPoint - (point3.LoyaltyPointToTrigger ?? 1200);
                    }

                    account.AccountLoyaltyPoints = newPoint;
                    _currentDbContext.Entry(account).State = EntityState.Modified;
                    _currentDbContext.SaveChanges();
                }

            }

        }

        private ShoppingVoucher GetVoucher(string voucherCode, int userId, ShoppingVoucherDiscountTypeEnum discountType,  int createdUserId, decimal discountFigure = 0, int? rewardProductId = null, string title= "", RewardProductCategoryEnum rewardProductCategory = RewardProductCategoryEnum.Milkshake)
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
                MaximumAllowedUse = 1,
                VoucherExpiryDate = DateTime.Now.AddDays(30),
                RewardProductCategory = rewardProductCategory
            };
            return voucher;
        }

        public bool LoadDefaultSystemVouchersForNewUser(int userId)
        {
            var newlyApplied = false;
            if (EnableDiscountForFirstOnlineOrder && !_currentDbContext.ShoppingVouchers.Any(m => m.VoucherCode.ToLower().Equals(FirstOrderOnlineDiscountVoucher) && m.VoucherUserId == userId))
            {
                var firstOnlineOrderVoucher = GetVoucher(FirstOrderOnlineDiscountVoucher, userId,
                    ShoppingVoucherDiscountTypeEnum.Percentage, userId, FirstOrderOnlineDiscountPercent, null, FirstOrderOnlineDiscountPercent+ "% off first on any order");
                firstOnlineOrderVoucher.MinimumOrderPrice = FirstOrderOnlineDiscountMinimumOrderValue;
                firstOnlineOrderVoucher.VoucherExpiryDate = DateTime.Now.AddDays(30);
                _currentDbContext.ShoppingVouchers.Add(firstOnlineOrderVoucher);
                _currentDbContext.SaveChanges();
                newlyApplied = true;
            }

            if (EnableDiscountForFirstDeliveryOrder && !_currentDbContext.ShoppingVouchers.Any(m => m.VoucherCode.ToLower().Equals(FirstOrderHomeDeliveryDiscountVoucher) && m.VoucherUserId == userId))
            {
                var firstOnlineOrderVoucher = GetVoucher(FirstOrderHomeDeliveryDiscountVoucher, userId,
                    ShoppingVoucherDiscountTypeEnum.Percentage, userId, FirstOrderHomeDeliveryDiscountPercent, null, FirstOrderHomeDeliveryDiscountPercent+"% off first delivery order");
                firstOnlineOrderVoucher.VoucherExpiryDate = DateTime.Now.AddDays(30);
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
        public RewardPointTrigger GetRewardTriggerById(int id)
        {
            return _currentDbContext.RewardPointTriggers.FirstOrDefault(m => m.RewardPointTriggerId == id);
        }

        public List<ShoppingVoucher> GetAllValidShoppingVouchers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false)
        {
            return _currentDbContext.ShoppingVouchers.Where(m =>
                (includeDeleted || m.IsDeleted != true) && (tenantId == 0 || (m.TenantId == tenantId || m.TenantId==null)) &&
                (!onlyUpdatedAfter.HasValue || (m.DateUpdated ?? m.DateCreated) > onlyUpdatedAfter)).OrderByDescending(m=> m.DateCreated).ToList();
        }

        public List<RewardPointTrigger> GetAllValidRewardTriggers(int tenantId, DateTime? onlyUpdatedAfter = null, bool includeDeleted = false)
        {
            return _currentDbContext.RewardPointTriggers.Where(m =>
                (includeDeleted || m.IsDeleted != true) && (tenantId == 0 || (m.TenantId == tenantId || m.TenantId == null)) &&
                (!onlyUpdatedAfter.HasValue || (m.DateUpdated ?? m.DateCreated) > onlyUpdatedAfter)).OrderByDescending(m => m.DateCreated).ToList();
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

        public RewardPointTrigger SaveRewardTrigger(RewardPointTrigger trigger, int userId)
        {
            if (trigger.RewardPointTriggerId > 0)
            {
                trigger.UpdatedBy = userId;
                trigger.DateUpdated = DateTime.Now;
                _currentDbContext.Entry(trigger).State = EntityState.Modified;
                _currentDbContext.Entry(trigger).Property(m => m.DateCreated).IsModified = false;
                _currentDbContext.Entry(trigger).Property(m => m.CreatedBy).IsModified = false;
            }
            else
            {
                trigger.CreatedBy = userId;
                trigger.DateCreated = DateTime.Now;
                _currentDbContext.RewardPointTriggers.Add(trigger);
            }

            _currentDbContext.SaveChanges();
            return trigger;
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
                && (m.VoucherUser.UserMobileNumber!=null && m.VoucherUser.UserMobileNumber.EndsWith(phone.Trim()) )).OrderByDescending(m=> m.DateCreated).Select(MapVoucherToModel).ToList();
            return vouchers;
        }
    }
}
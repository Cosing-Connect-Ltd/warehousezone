using Ganedata.Core.Services;

namespace WarehouseEcommerce.Helpers
{
    public class LayoutHelpers
    {

        public static bool UserLoginStatus()
        {
            bool status = false;

            caUser user = caCurrent.CurrentWebsiteUser();
            if (user.AuthUserStatus == true) status = true;

            return status;
        }

        public static bool IsSuperUser()
        {
            bool isSuperUser = false;

            caUser user = caCurrent.CurrentWebsiteUser();
            if (user.AuthUserStatus == true) isSuperUser = user.SuperUser == true;

            return isSuperUser;
        }
    }
}

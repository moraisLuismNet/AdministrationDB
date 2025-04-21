using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AdministrationDataBase.Helpers
{
    public static class AdminHelper
    {
        public static void SetAdminEmailInViewBag(Controller controller, IConfiguration config)
        {
            var adminEmail = config["Mail:AdminEmail"];
            controller.ViewBag.AdminEmail = adminEmail;
        }

        public static bool IsAdmin(ClaimsPrincipal user, IConfiguration config)
        {
            var adminEmail = config["Mail:AdminEmail"];
            var userEmail = user.FindFirst(ClaimTypes.Email)?.Value;

            return userEmail == adminEmail;
        }
    }
}

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace HashTag.Infrastructure.Extensions
{
    public static class RouteBuilderExtensions
    {
        public static void RegisterRoutes(this IRouteBuilder routes)
        {
            routes.MapRoute("Register", "register",
                new { controller = "auth", action = "register" });

            routes.MapRoute("SignIn", "signin/{returnUrl?}",
                new { controller = "auth", action = "signin" });

            routes.MapRoute("SignOut", "signout",
                new { controller = "auth", action = "signout" });

            routes.MapRoute("AccessDenied", "accessdenied",
                new { controller = "auth", action = "accessdenied" });

            routes.MapRoute("Admin", "admin",
                new { controller = "pages", action = "admin" });

            routes.MapRoute("Profile", "profile/{userName?}",
                new { controller = "pages", action = "profile" });

            routes.MapRoute("Search", "search/{hashtag?}",
                new { controller = "pages", action = "search" });

            routes.MapRoute("SetPassword", "setpassword",
                new { controller = "manage", action = "setpassword" });

            routes.MapRoute("EditUser", "edit",
                new { controller = "manage", action = "edituser" });

            routes.MapRoute("Default", "{controller=pages}/{action=search}");
        }
    }
}
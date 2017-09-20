using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MarketingOnline
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                "company",
                "cong-ty/{pro}/{dis}/{str}/{keyword}/{page}",
                new { controller = "company", action = "index", pro = UrlParameter.Optional, dis = UrlParameter.Optional, str = UrlParameter.Optional, keyword = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
                "view company ",
                "doanh-nghiep/{name}-{id}",
                new { controller = "company", action = "GetDetails", name = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "view detail news",
                "{name}-{id}",
                new { controller = "news", action = "GetDetails", name = UrlParameter.Optional, id = UrlParameter.Optional }
            );
            routes.MapRoute(
                "view news",
                "tin/{keyword}-{page}",
                new { controller = "news", action = "List", keyword = UrlParameter.Optional, page = UrlParameter.Optional }
            );
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

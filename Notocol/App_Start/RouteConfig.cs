using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;

using Notocol.Controllers;

namespace Notocol
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //This is required for Service Stack
            //routes.IgnoreRoute("api/{*pathInfo}");
            

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Web API Session Enabled Route Configurations start
            routes.MapHttpRoute(
                name: "annotationAPIStoreFeaturesRoute",
                routeTemplate: "app/features",
                defaults: new { controller = "ChromeExtensionStore", action = "Features" }
            ).RouteHandler = new SessionStateRouteHandler();
            
            routes.MapHttpRoute(
                name: "annotationAPIStoreStatusRoute",
                routeTemplate: "app",
                defaults: new { controller = "ChromeExtensionStore", action = "app" }
            ).RouteHandler = new SessionStateRouteHandler();

            routes.MapHttpRoute(
                name: "annotationAPIStoreInfoRoute",
                routeTemplate: "api",
                defaults: new { controller = "annotations", action = "storeInfo", id = RouteParameter.Optional }
            ).RouteHandler = new SessionStateRouteHandler();


            routes.MapHttpRoute(
                name: "wsSocketHandler",
                routeTemplate: "ws",
                defaults: new { controller = "ws", action = "Get", id = RouteParameter.Optional }
            ).RouteHandler = new SessionStateRouteHandler();

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            ).RouteHandler = new SessionStateRouteHandler();
            ////Web API Session Enabled Route Configurations end here


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}

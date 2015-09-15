using Newtonsoft.Json;
using System.Web.Http;

namespace Notocol
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Web API routes
            //All the routes to web APIs for extension have been moved to routeconfig.cs

            config.Formatters.JsonFormatter.SerializerSettings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
        }
    }
}

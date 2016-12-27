using Accenture.Security.Eso.Service;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Services.Host.IIS
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
#if DEBUG
            EnableCrossSiteRequests(config);
#endif
            // Remove default XML handler
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            // Web API routes
            config.MapHttpAttributeRoutes();
            RegisterAPIRoutes(config);
        }

        private static void RegisterAPIRoutes(HttpConfiguration config)
        {
            #region Announcement

            config.Routes.MapHttpRoute(
                name: "GetAnnouncementsByFacility",
                routeTemplate: "CMS/getAnnouncementsByFacility",
                defaults: new { controller = "AnnouncementService", action = "GetAnnouncementsByFacility" });

            #endregion

            #region Session

            config.Routes.MapHttpRoute(
                name: "getSessionsByLocation",
                routeTemplate: "CMS/getSessionsByLocation",
                defaults: new { controller = "SessionService", action = "getSessionsByLocation" });

            config.Routes.MapHttpRoute(
                name: "getSessionsByFacility",
                routeTemplate: "CMS/getSessionsByFacility",
                defaults: new { controller = "SessionService", action = "getSessionsByFacility" });

            #endregion                       
        }

        private static void EnableCrossSiteRequests(HttpConfiguration config)
        {
            var cors = new EnableCorsAttribute(origins: "*", headers: "*", methods: "*");
            config.EnableCors(cors);
        }
    }
}

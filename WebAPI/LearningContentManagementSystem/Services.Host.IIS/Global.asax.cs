using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Services.Host.IIS
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        }
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }
    }
}

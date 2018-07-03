using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Atimo;
using System.Threading;
using System.Globalization;
using System;
using ATIMO.Helpers.ModelsUtils;

namespace ATIMO
{
    public class MvcApplication : HttpApplication
    {


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


#if !DEBUG

            GlobalFilters.Filters.Add(new RequireHttpsAttribute());
#endif

            FaturamentoHelper.StartSendEmailMonitor();

        }

        protected void Application_End(object sender, EventArgs e)
        {
            FaturamentoHelper.StopSendEmailMonitor();
        }
    }
}
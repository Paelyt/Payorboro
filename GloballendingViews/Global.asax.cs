using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace GloballendingViews
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
         {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RouteTable.Routes.AppendTrailingSlash = true;
        }

        protected void Application_BeginRequest()
        {
            RouteTable.Routes.AppendTrailingSlash = true;
            //    if (HttpContext.Current.Request.Url.AbsolutePath != "/" && HttpContext.Current.Request.Url.AbsolutePath.EndsWith("/"))
            //    {
            //        string redirect = HttpContext.Current.Request.Url.AbsolutePath;
            //        redirect = redirect.Remove(redirect.Length - 1);
            //        Response.Clear();
            //        Response.Status = "301 Moved Permanently";
            //        Response.AddHeader("Location", redirect);
            //        Response.End();
            //}
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ExcellentMarketResearch.Models;

namespace ExcellentMarketResearch
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }
        //protected void Application_BeginRequest(object sender, EventArgs e)
        protected void Application_BeginRequest()
        {
            if (!Context.Request.IsSecureConnection && !Context.Request.Url.ToString().Contains("localhost"))
                Response.Redirect(Context.Request.Url.ToString().Replace("http:", "https:"));

            //if ((Request.Url.Scheme != "https" || Request.Url.AbsoluteUri.Contains("www.")) && !Request.IsLocal)
            //{
            //    Response.RedirectPermanent("https://www.excellentmarketresearch.com");
            //}

            //ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

            //if ((!string.IsNullOrEmpty(Request.Url.AbsoluteUri)))
            //{
            //    var redirecturl = (from l in db.RedirectUrls
            //                       where l.SourceUrl == Request.Url.AbsoluteUri
            //                       select l.TargetUrl).FirstOrDefault();
            //    if (redirecturl == null)
            //    {

            //    }
            //    else
            //        Response.RedirectPermanent(redirecturl);
            //}
        }
    }
}
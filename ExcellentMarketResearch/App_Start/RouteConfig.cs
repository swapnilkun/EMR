using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ExcellentMarketResearch
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.RouteExistingFiles = true;
            routes.MapRoute(
             name: "Sitemap1",
             url: "sitemap{PageNo}.xml",
             defaults: new { controller = "Sitemap", action = "SiteMapReports", PageNo = UrlParameter.Optional },
              namespaces: new[] { "ExcellentMarketResearch.Controllers" }
         );

            routes.MapRoute(
                name: "SitemapIndex",
                url: "sitemap.xml",
                defaults: new { controller = "Sitemap", action = "Index" },
                 namespaces: new[] { "ExcellentMarketResearch.Controllers" }
            );
            routes.MapRoute(
              name: "PrivacyAndCookies",
              url: "privacy-policy",
              defaults: new { controller = "Home", action = "PrivacyAndCookies" },
              namespaces: new[] { "ExcellentMarketResearch.Controllers" }
           );

            routes.MapRoute(
           name: "TermsAndCondition",
           url: "terms-conditions",
           defaults: new { controller = "Home", action = "TermsAndCondition" },
           namespaces: new[] { "ExcellentMarketResearch.Controllers" }
        );

            routes.MapRoute(
          name: "ContactUs",
          url: "contactus",
          defaults: new { controller = "ContactUs", action = "Index" },
          namespaces: new[] { "ExcellentMarketResearch.Controllers" }
       );
            routes.MapRoute(
            name: "AboutUs",
            url: "aboutus",
            defaults: new { controller = "Home", action = "About" },
            namespaces: new[] { "ExcellentMarketResearch.Controllers" }
         );
            routes.MapRoute(
             name: "AllLatestNews",
             url: "latest/news/",
             defaults: new { controller = "News", action = "AllNews" },
             namespaces: new[] { "ExcellentMarketResearch.Controllers" }
          );

            routes.MapRoute(
            name: "searchedreports",
            url: "report/search-report/{searchkey}",
            defaults: new { controller = "Report", action = "SearchedReports", searchkey = UrlParameter.Optional },
            namespaces: new[] { "ExcellentMarketResearch.Controllers" }
         );

            routes.MapRoute(
               name: "ReportDetails",
               url: "report/{Reporturl}",
               defaults: new { controller = "Report", action = "ReportDetail", Reporturl = UrlParameter.Optional },
               namespaces: new[] { "ExcellentMarketResearch.Controllers" }
            );
            routes.MapRoute(
                        name: "PublisherRelatedReports",
                        url: "publisher/{puburl}",
                        defaults: new { controller = "Report", action = "PublisherRelatedReports", puburl = UrlParameter.Optional },
                         namespaces: new[] { "ExcellentMarketResearch.Controllers" }
                    );

            routes.MapRoute(
           name: "CategoryRelatedReports",
           url: "category/{caturl}",
           defaults: new { controller = "Report", action = "CategoryRelatedReports", caturl = "" },
           namespaces: new[] { "ExcellentMarketResearch.Controllers" }
           );

            routes.MapRoute(
                 name: "MainCategories",
                 url: "all-category",
                 defaults: new { controller = "Category", action = "MainCategories" },
                        namespaces: new[] { "ExcellentMarketResearch.Controllers" }
                   );

            routes.MapRoute(
              name: "AllLatestReport",
              url: "latest/reports/",
              defaults: new { controller = "Report", action = "AllReports" },
              namespaces: new[] { "ExcellentMarketResearch.Controllers" }
           );

            routes.MapRoute(
               name: "LatestReport",
               url: "latest-report/",
               defaults: new { controller = "Report", action = "LatestReport" },
               namespaces: new[] { "ExcellentMarketResearch.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "ExcellentMarketResearch.Controllers" }
            );
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;
using ExcellentMarketResearch.Models;
using PagedList;
using PagedList.Mvc;

namespace ExcellentMarketResearch.Models
{
    public class Sitemap
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        int PageSize = 3000;



        public string SiteMapReports(int? id)
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream);

            //DataSet ds = new reports().GetSiteMap(
            //                  PageSize
            //                , PageNo);

            var reports = (from l in db.ReportMasters
                           orderby l.CreatedDate descending
                           select new AllPublishedReports
                           {
                               ReportUrl = l.ReportUrl,

                           }).ToPagedList(id ?? 1, PageSize);

            DataSet ds = new DataSet();
            if (reports.Count() > 0)
            {
                writer.WriteStartDocument();
                String text = "type='text/xsl' href='/css/gss.xsl'";
                writer.WriteProcessingInstruction("xml-stylesheet", text);
                //writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xml' href='~/css/gss.xsl'");
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
                //writer.WriteAttributeString("xmlns", "mrs", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
                writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
                writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

                writer.WriteStartElement("url");
                writer.WriteStartElement("loc");
                writer.WriteString("http://localhost:1103/");
                writer.WriteEndElement();
                writer.WriteStartElement("changefreq");
                writer.WriteString("daily");
                writer.WriteEndElement();
                writer.WriteEndElement();

                for (var i = 0; i < reports.Count(); i++)
                {
                    writer.WriteStartElement("url");
                    writer.WriteStartElement("loc");
                    //writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" +
                    //      HttpContext.Current.Request.Url.Host + "/report/" + reports[i].ReportUrl);
                    //writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" +
                    //    HttpContext.Current.Request.Url.Host+"/report/" + reports[i].ReportUrl);
                    writer.WriteString("http://localhost:1103" + "/report/" + reports[i].ReportUrl);
                    writer.WriteEndElement();
                    writer.WriteStartElement("changefreq");
                    writer.WriteString("daily");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                stream.Position = 0;

            }
            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }


        public string GenerateIndex()
        {
            MemoryStream stream = new MemoryStream();
            XmlWriter writer = XmlWriter.Create(stream);

            writer.WriteStartDocument();
            //writer.WriteProcessingInstruction("xml-stylesheet", "type='text/xml' href='gss.xsl'");
            String text = "type='text/xsl' href='/css/gss.xsl'";
            writer.WriteProcessingInstruction("xml-stylesheet", text);
            writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");
            //writer.WriteAttributeString("xmlns", "mrs", null, "http://www.sitemaps.org/schemas/sitemap/0.9");
            writer.WriteAttributeString("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
            writer.WriteAttributeString("xsi", "schemaLocation", null, "http://www.sitemaps.org/schemas/sitemap/0.9 http://www.sitemaps.org/schemas/sitemap/0.9/sitemap.xsd");

            writer.WriteStartElement("url");
            writer.WriteStartElement("loc");
            writer.WriteString("http://localhost:1103/");
            writer.WriteEndElement();
            writer.WriteStartElement("changefreq");
            writer.WriteString("daily");
            writer.WriteEndElement();
            writer.WriteEndElement();

            var reportcount = db.ReportMasters.Count();

            if (reportcount > 0)
            {
                double xml = Convert.ToDouble(reportcount) / Convert.ToDouble(PageSize);

                for (var i = 1; i <= Math.Ceiling(xml); i++)
                {
                    writer.WriteStartElement("url");
                    writer.WriteStartElement("loc");
                    writer.WriteString("http://localhost:1103/sitemap" + i + ".xml");

                    //writer.WriteString(HttpContext.Current.Request.Url.Scheme + "://" +
                    //        HttpContext.Current.Request.Url.Host + "/sitemap-report-" + i + ".xml");

                    writer.WriteEndElement();
                    writer.WriteStartElement("changefreq");
                    writer.WriteString("daily");
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndDocument();
            writer.Flush();
            stream.Position = 0;
            StreamReader read = new StreamReader(stream);
            return read.ReadToEnd();
        }

    }
}
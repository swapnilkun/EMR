using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using ExcellentMarketResearch.Models.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Report/

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public ActionResult Index()
        {
            ViewBag.activemenu = "Report";
            return View();
        }
        public string DDLGetparents(int catid)
        {
            List<int> arr = new List<int>();
            arr.Add(catid);
            int? parent = catid;
            while (parent != 0)
            {
                parent = db.CategoryMasters.Where(x => x.CategoryId == parent).Select(x => x.ParentCategoryId).FirstOrDefault();
                if (parent > 0)
                    arr.Add((int)parent);
            }
            
          var i=arr.Last();
          var caturlimag = db.CategoryMasters.Where(x => x.CategoryId == i).Select(x => x.CategoryUrl).FirstOrDefault();
          return caturlimag;
        }
        [OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult LatestReportsForNews()
        {
            var LatestPublishedReports = (from l in db.spLatestReport()
                                          select new ReportVM
                                          {
                                              FullDescription = System.Text.RegularExpressions.Regex.Replace(l.LongDescritpion, @"<[^>]+>|&nbsp;", "").Trim(),
                                              CategoryName = l.CategoryName,
                                              ReportId = l.ReportId,
                                              ReportTitle = l.ReportTitle,
                                              ReportUrl = l.ReportUrl,
                                              PriceSingleUser = l.SinglePrice,
                                              PublishingDate = Convert.ToDateTime(l.PublishingDate)
                                          }).ToList();
            return PartialView(LatestPublishedReports);
        }
        public ActionResult AllReports(int? pageno)
        {
            //var Allreports = (from l in db.ReportMasters
            //                  join c in db.CategoryMasters
            //                  on l.CategoryId equals c.CategoryId
            //                  join p in db.PublisherMasters 
            //                  on l.PublisherId equals p.PublisherId
            //                  orderby l.CreatedDate descending
            //                  //where l.FullDescription.Replace("<[^>]+>|&nbsp;", "").Trim().Contains(@"<[^>]+>|&nbsp;", "")
            //                  select new ReportVM
            //                  {
            //                      ReportTitle = l.ReportTitle,
            //                      ReportUrl = l.ReportURL,
            //                      FullDescription = l.FullDescription.Substring(0, 300),
            //                      NumberOfPage = l.NumberOfPage,
            //                      CategoryId = l.CategoryId,
            //                      PublishingDate = l.PublishingDate,
            //                      PriceSingleUser = l.PriceSingleUser,
            //                      CategoryName = c.CategoryName,
            //                      CategoryUrl=c.CategoryUrl,
            //                      PublisherName=p.CompanyName,
            //                      PublisherId=p.PublisherId,
            //                      PublishingUrl=p.PublisherUrl
            //                  }).ToList();
            ViewBag.activemenu = "Report";
            ObjectParameter count = new ObjectParameter("p_Count", 0);

            var Allreports = (from r in db.spNewAllLatestReport(pageno ?? 1, count).ToList()
                              select new ReportVM
                              {
                                  ReportTitle = r.ReportTitle,
                                  CategoryId = r.CategoryId,
                                  PublishingDate = Convert.ToDateTime(r.PublishingDate),
                                  ReportUrl = r.ReportUrl,
                                  PriceSingleUser = r.SinglePrice,
                                  FullDescription = r.LongDescription,
                                  NumberOfPage = r.NumberOfPages,
                                  CategoryName = r.CategoryName,
                                  CategoryUrl = r.CategoryUrl,
                                  PublisherName = r.PublisherName,
                                  PublisherId = r.PublisherId,
                                  PublishingUrl = r.publisherUrl,
                                  RepImageCat = 0
                              }).ToList();

            foreach(var item in Allreports)
            {
                item.ReportImage = DDLGetparents(item.CategoryId);
            }

            var result = Allreports.Select(x => new ReportVM
            {
                ReportTitle = x.ReportTitle,
                ReportUrl = x.ReportUrl,
                FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                NumberOfPage = x.NumberOfPage,
                CategoryId = x.CategoryId,
                PublishingDate = x.PublishingDate,
                PriceSingleUser = x.PriceSingleUser,
                CategoryName = x.CategoryName,
                CategoryUrl = x.CategoryUrl,
                PublisherName = x.PublisherName,
                PublisherId = x.PublisherId,
                PublishingUrl = x.PublishingUrl,
                ReportImage = x.ReportImage
            }).ToList();

            var reports = new StaticPagedList<ReportVM>(result, pageno ?? 1, 10, Convert.ToInt32(count.Value));
           
            ViewBag.Totalreports = reports.TotalItemCount;
            ViewBag.Title = "All Latest Market Research Reports | Excellent Market Research";
            ViewBag.Description = "Excellent Market Research  Provides All Latest Market Research Reports of Different Categories and Custom Reports";
            ViewBag.Keywords = "Latest Reports, Trending Reports, Custom Reports, ";
           
            return View(reports);

        }
        //s[OutputCache(Duration = 60, VaryByParam = "none")]
        public ActionResult LatestReport()
        {
            var latestreport = (from l in db.spLatestReport()
                                select new ReportVM
                                {
                                    ReportId = l.ReportId,
                                    ReportTitle = l.ReportTitle,
                                    ReportUrl = l.ReportUrl,
                                    FullDescription = Regex.Replace(l.LongDescritpion, @"<[^>]+>|&nbsp;", "").Trim(),
                                    PriceSingleUser = l.SinglePrice,
                                    PublishingDate = Convert.ToDateTime(l.PublishingDate),
                                    CategoryName = l.CategoryName,
                                    PublisherId = l.PublisherId,
                                    PublisherName = l.PublisherName
                                }).ToList();

            return PartialView(latestreport);
        }
        public ActionResult ReportDetail(string Reporturl)
        {
            Models.ViewModel.ReportDetailsVM report = new Models.ViewModel.ReportDetailsVM();

            report.ReportDetails = (from l in db.ReportMasters
                                    join c in db.CategoryMasters on l.CategoryId equals c.CategoryId
                                    join rt in db.ReportTypes on l.ReportTypeId equals rt.ReportTypeId
                                    join p in db.PublisherMasters on l.PublishereId equals p.PublisherId
                                    where l.ReportUrl == Reporturl
                                    select new ReportVM
                                    {
                                        ReportId = l.ReportId,
                                        ReportTitle = l.ReportTitle,
                                        ReportUrl = l.ReportUrl,
                                        CategoryId = l.CategoryId,
                                        CategoryName = c.CategoryName,
                                        FullDescription = l.LongDescritpion,
                                        TableofContent = l.TableOfContent,
                                        NumberOfPage = l.NumberOfPages,
                                        PriceSingleUser = l.SinglePrice,
                                        PriceMultiUser = l.MultiUserPrice,
                                        PriceCUL = l.CorporateUserPrice,
                                        PublishingDate = l.PublishingDate,
                                        PublisherName = p.ContactName,
                                        PublisherId = p.PublisherId,
                                        PublishingUrl = p.publisherUrl

                                    }
                                        ).FirstOrDefault();
            if (report.ReportDetails == null || report.ReportDetails.ReportUrl != Reporturl)
            {
                //throw new HttpException(404, "Page Not Found");
                return HttpNotFound();
            }
            report.BreadcrumbCategory = db.Database.SqlQuery<BreadsCum>("exec  spBreadsCum @CategoryId", new SqlParameter("@CategoryId", report.ReportDetails.CategoryId)).ToList();
            return View(report);
        }

        public ActionResult SearchedReports(int? pageno, string searchkey, ReportMaster r, string sortby)
        {
            /* Report Search  by its ReportTitle */
            searchkey = searchkey.Trim();
            if (!string.IsNullOrEmpty(searchkey))
            {
                #region ReportTitleOnly
                var pricesort = sortby;

                var SearchedReport = (from x in db.ReportMasters
                                   join c in db.CategoryMasters
                                   on x.CategoryId equals c.CategoryId
                                   join p in db.PublisherMasters
                                   on x.PublishereId equals p.PublisherId
                                   where x.ReportTitle.Contains(searchkey)
                                   orderby x.PublishingDate descending
                                   select new ReportVM
                                   {
                                       ReportTitle = x.ReportTitle,
                                       ReportUrl = x.ReportUrl,
                                       FullDescription = x.LongDescritpion.Substring(0, 300),
                                       NumberOfPage = x.NumberOfPages,
                                       PriceSingleUser = x.SinglePrice,
                                       PublishingDate = x.PublishingDate,
                                       CategoryName = c.CategoryName,
                                       CategoryUrl = c.CategoryUrl,
                                       PublisherName = p.ContactName,
                                       PublisherId = p.PublisherId,
                                       PublishingUrl = p.publisherUrl,
                                       CategoryId=c.CategoryId,
                                       ReportImage=""
                                   }).ToList();
                foreach (var item in SearchedReport)
                {
                    item.ReportImage = DDLGetparents(item.CategoryId);
                }
                var Reports = (from z in SearchedReport
                               select new ReportVM
                               {
                                   ReportTitle = z.ReportTitle,
                                   ReportUrl = z.ReportUrl,
                                   FullDescription = Regex.Replace(z.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                                   NumberOfPage = z.NumberOfPage,
                                   PriceSingleUser = z.PriceSingleUser,
                                   PublishingDate = z.PublishingDate,
                                   CategoryName = z.CategoryName,
                                   CategoryUrl = z.CategoryUrl,
                                   PublisherId = z.PublisherId,
                                   PublisherName = z.PublisherName,
                                   PublishingUrl = z.PublishingUrl,
                                   ReportImage=z.ReportImage
                               }).ToPagedList(pageno ?? 1, 10);

                if (Reports.Count > 0)
                {
                    //var z = new StaticPagedList<ReportDetails>(Reports, pageno ?? 1, 10, Reports.TotalItemCount);
                    ViewBag.TotalReports = Reports.Count;
                    ViewBag.SearchedKeyword = searchkey;
                    return View(Reports);
                }
                else
                {
                    ViewBag.NoReport = "Reports not found for such keyword !....";
                    return View();
                }

                #endregion ReportTitle
            }
            else
                // return View(r);
                return HttpNotFound();
        }
        public ActionResult CategoryRelatedReports(string caturl, int? pageno)
        {

            // check Where it is parent category or child Category 

            ReportVM ObjReortVM = new ReportVM();

            var categoryID = db.CategoryMasters.Where(c => c.CategoryUrl == caturl).Select(c => c.CategoryId).FirstOrDefault();
            int catid = Convert.ToInt32(categoryID);


            var parentcategory = db.CategoryMasters.Where(x => x.CategoryId == catid && x.ParentCategoryId == 0).Select(x => x.CategoryName).FirstOrDefault();

            ViewBag.CategoryName = parentcategory + " " + "Market Research Reports";

            if (!string.IsNullOrEmpty(parentcategory))
            {
                // Reports with Parent and child wised 


                ObjectParameter count = new ObjectParameter("p_Count", 0);

                var RelatedReportsOfSamefamilly = (from r in db.spCategoryRelatedReport(catid, pageno ?? 1, count)
                                                   select new ReportVM
                                                   {
                                                       ReportTitle = r.ReportTitle,
                                                       CategoryId = r.CategoryId,
                                                       PublishingDate = Convert.ToDateTime(r.PublishingDate),
                                                       ReportUrl = r.ReportUrl,
                                                       PriceSingleUser = r.SinglePrice,
                                                       FullDescription = r.LongDescription,
                                                       NumberOfPage = r.NumberOfPages,
                                                       CategoryName = r.CategoryName,
                                                       MetaTitle = r.MetaTitle,
                                                       MetaKeywords = r.Keywords,
                                                       MetaDescription = r.MetaDescription,
                                                       CategoryUrl = r.CategoryUrl,
                                                       ShortCatDesc = r.ShortDescription,
                                                       LongCatDesc = r.LongDescription,
                                                       PublisherName = r.PublisherName,
                                                       ReportImage=DDLGetparents(r.CategoryId)
                                                   }).ToList();

                var catreports = (from x in RelatedReportsOfSamefamilly
                                  select new ReportVM
                                  {
                                      ReportTitle = x.ReportTitle,
                                      CategoryId = x.CategoryId,
                                      PublishingDate = Convert.ToDateTime(x.PublishingDate),
                                      ReportUrl = x.ReportUrl,
                                      PriceSingleUser = x.PriceSingleUser,
                                      FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                                      NumberOfPage = x.NumberOfPage,
                                      CategoryName = x.CategoryName,
                                      MetaTitle = x.MetaTitle,
                                      MetaKeywords = x.MetaKeywords,
                                      MetaDescription = x.MetaDescription,
                                      CategoryUrl = x.CategoryUrl,
                                      ShortCatDesc = x.ShortCatDesc,
                                      LongCatDesc = x.LongCatDesc,
                                      PublisherId = x.PublisherId,
                                      PublisherName = x.PublisherName,
                                      ReportImage=x.ReportImage
                                  }).ToList();

                var reports = new StaticPagedList<ReportVM>(catreports, pageno ?? 1, 10, Convert.ToInt32(count.Value));

                ObjReortVM = (from c in db.CategoryMasters
                              where c.CategoryId == catid
                              select new ReportVM
                              {
                                  ShortCatDesc = c.ShortDescription,
                                  LongCatDesc = c.LongDescription
                              }).FirstOrDefault();
                //ViewBag.Totalreports = Convert.ToInt32(count.Value);
                ViewBag.ShortDesc = ObjReortVM.ShortCatDesc;
                ViewBag.LongDesc = ObjReortVM.LongCatDesc;
                ViewBag.Title = parentcategory + " Market Research Report and Industry Analysis Market Research Report and Industry Analysis & Forecast - Market Research Trade";
                ViewBag.Description = "";
                ViewBag.Keywords = "";
                return View(reports);

            }
            else
            {
                // Only CategoryWised Reports 
                var childCatName = db.CategoryMasters.Where(x => x.CategoryId == catid).Select(x => x.CategoryName).FirstOrDefault();
                ViewBag.CategoryName = childCatName + " " + "Market Research Reports";
                var Relatedreports = (from r in db.ReportMasters
                                      join c in db.CategoryMasters on r.CategoryId equals c.CategoryId
                                      join p in db.PublisherMasters on r.PublishereId equals p.PublisherId
                                      where c.CategoryId == catid
                                      orderby r.CreatedDate descending
                                      select new ReportVM
                                      {
                                          ReportTitle = r.ReportTitle,
                                          ReportUrl = r.ReportUrl,
                                          CategoryId = r.CategoryId,
                                          CategoryName = c.CategoryName,
                                          ShortCatDesc = c.ShortDescription,
                                          LongCatDesc = c.LongDescription,
                                          PublisherName = p.ContactName,
                                          FullDescription = r.LongDescritpion.Substring(0, 300),
                                          PublishingDate = r.PublishingDate,
                                          PublisherId = r.PublishereId,
                                          CategoryUrl = c.CategoryUrl,
                                          PublishingUrl = p.publisherUrl

                                      }).ToList();

                var reports = (from x in Relatedreports
                               select new ReportVM
                               {
                                   ReportTitle = x.ReportTitle,
                                   ReportUrl = x.ReportUrl,
                                   CategoryId = x.CategoryId,
                                   CategoryName = x.CategoryName,
                                   ShortCatDesc = x.ShortCatDesc,
                                   LongCatDesc = x.LongCatDesc,
                                   FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                                   PublishingDate = x.PublishingDate,
                                   PublisherId = x.PublisherId,
                                   PublisherName = x.PublisherName,
                                   CategoryUrl = x.CategoryUrl,
                                   PublishingUrl = x.PublishingUrl
                               }).ToPagedList(pageno ?? 1, 10);

                //ObjectParameter count = new ObjectParameter("p_Count", 0);

                //var RelatedReportsOfSamefamilly = (from r in db.ChildCategoryRelatedReport(catid, pageno ?? 1, count).ToList()
                //                                   select new ReportVM
                //                                   {
                //                                       ReportTitle = r.ReportTitle,
                //                                       CategoryId = r.CategoryId,
                //                                       PublishingDate = Convert.ToDateTime(r.PublishingDate),
                //                                       ReportUrl = r.ReportUrl,
                //                                       PriceSingleUser = r.PriceSingleUser,
                //                                       FullDescription = r.FullDescription,
                //                                       NumberOfPage = r.NumberOfPage,
                //                                       CategoryName = r.CategoryName,
                //                                       MetaTitle = r.MetaTitle,
                //                                       MetaKeywords = r.MetaKeywords,
                //                                       MetaDescription = r.MetaDescription,
                //                                       CategoryUrl = r.CategoryUrl,
                //                                       ShortCatDesc = r.ShortDescription,
                //                                       LongCatDesc = r.LongDescription,
                //                                       PublisherId = r.PublisherId,
                //                                       PublisherName = r.CompanyName
                //                                   }).ToList();

                //var catreports = (from x in RelatedReportsOfSamefamilly
                //                  select new ReportVM
                //                  {
                //                      ReportTitle = x.ReportTitle,
                //                      CategoryId = x.CategoryId,
                //                      PublishingDate = Convert.ToDateTime(x.PublishingDate),
                //                      ReportUrl = x.ReportUrl,
                //                      PriceSingleUser = x.PriceSingleUser,
                //                      FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                //                      NumberOfPage = x.NumberOfPage,
                //                      CategoryName = x.CategoryName,
                //                      MetaTitle = x.MetaTitle,
                //                      MetaKeywords = x.MetaKeywords,
                //                      MetaDescription = x.MetaDescription,
                //                      CategoryUrl = x.CategoryUrl,
                //                      ShortCatDesc = x.ShortCatDesc,
                //                      LongCatDesc = x.LongCatDesc,
                //                      PublisherId = x.PublisherId,
                //                      PublisherName = x.PublisherName
                //                  }).ToList();

                //var reports = new StaticPagedList<ReportVM>(catreports, pageno ?? 1, 10, Convert.ToInt32(count.Value));
                ViewBag.Title = ViewBag.CategoryName + " Market Research Report and Industry Analysis & Forecast - Market Research Trade";
                ViewBag.Description = "";
                ViewBag.Keywords = "";
                return View(reports);
            }

        }

        public ActionResult PublisherRelatedReports(string puburl, int? pageno)
        {
            var pubreport = (from r in db.ReportMasters
                             join p in db.PublisherMasters on r.PublishereId equals p.PublisherId
                             join c in db.CategoryMasters on r.CategoryId equals c.CategoryId
                             where p.publisherUrl == puburl
                             orderby r.CreatedDate
                             select new ReportVM
                             {
                                 ReportTitle = r.ReportTitle,
                                 ReportUrl = r.ReportUrl,
                                 CategoryId = r.CategoryId,
                                 CategoryName = c.CategoryName,
                                 ShortCatDesc = c.ShortDescription,
                                 LongCatDesc = c.LongDescription,
                                 PublisherName = p.ContactName,
                                 FullDescription = r.LongDescritpion.Substring(0, 300),
                                 PublishingDate = r.PublishingDate,
                                 PublisherId = r.PublishereId,
                                 CategoryUrl = c.CategoryUrl
                             }).ToList();


            var pubrelatedreport = (from x in pubreport
                                    select new ReportVM
                                    {
                                        ReportTitle = x.ReportTitle,
                                        ReportUrl = x.ReportUrl,
                                        CategoryId = x.CategoryId,
                                        CategoryName = x.CategoryName,
                                        ShortCatDesc = x.ShortCatDesc,
                                        LongCatDesc = x.LongCatDesc,
                                        FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
                                        PublishingDate = x.PublishingDate,
                                        PublisherId = x.PublisherId,
                                        PublisherName = x.PublisherName,
                                        CategoryUrl = x.CategoryUrl
                                    }).ToPagedList(pageno ?? 1, 10);


            ReportVM reportvm = new ReportVM();

            reportvm = (from p in db.PublisherMasters
                        where p.publisherUrl == puburl
                        select new ReportVM
                        {
                            LongPublisherDesc = p.LongDescription,
                            ShortPublisherDesc = p.ShortDescription
                        }).FirstOrDefault();
            //ViewBag.ShortPubDesc = reportvm.ShortPublisherDesc;
            //ViewBag.LongPubDesc = reportvm.LongPublisherDesc;
            return View(pubrelatedreport);

        }
       
        //public JsonResult GetReports(string ReportKey)
        //{
        //    /* For  Autocomplete textbox  */
        //    List<string> Reports;
        //    Reports = db.ReportMasters.Where(x => x.ReportTitle.Contains(ReportKey)).Select(y => y.ReportTitle).Take(35).ToList();
        //    return Json(Reports, JsonRequestBehavior.AllowGet);
        //}
        //public ActionResult CategoryHierarchicalRepots(string cat, int? pageno)
        //{
        //    //ReportDetailsModelclassClass1 r = new ReportDetailsModelclassClass1();

        //    //var Category = db.CategoryMasters.Where(x => x.CategoryURL == cat).Select(x=>x).ToList();

        //    var Category = (from l in db.CategoryMasters
        //                    where l.CategoryUrl == cat
        //                    select l).FirstOrDefault();

        //    ViewBag.CategoryName = Category.CategoryName;
        //    ViewBag.MetaTitle = Category.MetaTitle;
        //    ViewBag.Keywords = Category.MetaKeywords;
        //    ViewBag.MetaDescription = Category.MetaDescription;
        //    var catid = Category.CategoryId;


        //    if (!string.IsNullOrEmpty(Category.CategoryName))
        //    {
        //        //var catid = cat.Split('-').Last();

        //        var categoryid = Convert.ToInt32(catid);

        //        if (categoryid <= 0)
        //        {
        //            ViewBag.NotExistsReports = "No Reports Found !...";
        //        }
        //        else
        //        {
        //            /* Category Related Reports */

        //            //  var RelatedRepotsofSameCategory = db.Database.SqlQuery<AllPublishedReports>("exec CategoryRelatedReport @CategoryName,@pageno", new SqlParameter("@CategoryName",cat), new SqlParameter("@pageno", pageno ?? 1)).ToPagedList(pageno ?? 1, 10);

        //            //var RelatedReportsOfSamefamilly = db.CategoryRelatedReport(cat, pageno ?? 1).ToPagedList(pageno ?? 1, 10);

        //            ObjectParameter count = new ObjectParameter("p_Count", 0);

        //            var RelatedReportsOfSamefamilly = (from r in db.CategoryRelatedReport(categoryid, pageno ?? 1, count).ToList()
        //                                               select new ReportVM
        //                                               {
        //                                                   ReportTitle = r.ReportTitle,
        //                                                   CategoryId = r.CategoryId,
        //                                                   PublishingDate = Convert.ToDateTime(r.PublishingDate),
        //                                                   ReportUrl = r.ReportUrl,
        //                                                   PriceSingleUser = r.PriceSingleUser,
        //                                                   FullDescription = r.FullDescription,
        //                                                   NumberOfPage = r.NumberOfPage,
        //                                                   CategoryName = r.CategoryName,
        //                                                   MetaTitle = r.MetaTitle,
        //                                                   MetaKeywords = r.MetaKeywords,
        //                                                   MetaDescription = r.MetaDescription,
        //                                                  CategoryUrl =r.CategoryUrl
        //                                               }).ToList();

        //            var catreports = (from x in RelatedReportsOfSamefamilly
        //                              select new ReportVM
        //                              {
        //                                  ReportTitle = x.ReportTitle,
        //                                  CategoryId = x.CategoryId,
        //                                  PublishingDate = Convert.ToDateTime(x.PublishingDate),
        //                                  ReportUrl = x.ReportUrl,
        //                                  PriceSingleUser = x.PriceSingleUser,
        //                                  FullDescription = Regex.Replace(x.FullDescription, @"<[^>]+>|&nbsp;", "").Trim(),
        //                                  NumberOfPage = x.NumberOfPage,
        //                                  CategoryName = x.CategoryName,
        //                                  MetaTitle = x.MetaTitle,
        //                                  MetaKeywords = x.MetaKeywords,
        //                                  MetaDescription = x.MetaDescription,
        //                                  CategoryUrl=x.CategoryUrl
        //                              }).ToList();
        //            var reports = new StaticPagedList<ReportVM>(catreports, pageno ?? 1, 10, Convert.ToInt32(count.Value));

        //            ViewBag.Totalreports = Convert.ToInt32(count.Value);
        //            return View(reports);
        //        }
        //        return View();
        //    }
        //    else
        //    {
        //        return HttpNotFound();
        //    }

        //}

    }
}

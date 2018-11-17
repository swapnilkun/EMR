using ExcellentMarketResearch.Areas.Admin.Models.DAL;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using PagedList;
using PagedList.Mvc;


namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class AdminReportController : Controller
    {
        //
        // GET: /Admin/Report/

        private ReportRepository _ObjReportRepository;

       public AdminReportController()
        {
            _ObjReportRepository = new ReportRepository();
        }

        public ActionResult Index()
        {
            return View();
        }
        // GET: /Admin/Report/
        //ReportRepository _ObjReportRepository = new ReportRepository();

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

      //  [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportIndex(int? pageno, string searchkey, int? pagesize)
        {
            if (!String.IsNullOrEmpty(searchkey))
            {
                // return View(db.ReportMasters.Where(x => x.ReportTitle.Contains(ReportTitle)).ToList().ToPagedList(pageno ?? 1, 4));
                ViewBag.totalsearchedreports = db.ReportMasters.Count(x => x.ReportTitle.Contains(searchkey));

                //var realsearch = searchkey.Replace("/" + searchkey + "/", searchkey);
                var z = (from l in db.ReportMasters
                         orderby l.CreatedDate descending
                         where l.ReportTitle.Contains(" " + searchkey + " ") || l.ReportTitle.Contains(searchkey + " ") || l.ReportTitle.Contains(" " + searchkey) && l.IsActive == true
                         select new ReportVM
                         {
                             ReportId = l.ReportId,
                             ReportTitle = l.ReportTitle
                         }).ToPagedList(pageno ?? 1, pagesize ?? 10);

                //   var rep = new StaticPagedList<ReportMaster>(z, pageno ?? 1, 10, z.TotalItemCount);
                return View(z);
            }
            else
            {

                //   var reports = db.ReportMasters.OrderBy(x => x.PublishingDate).Select(x => x.ReportTitle).ToPagedList(pageno ?? 1, 10);
                var reports = (from l in db.ReportMasters
                               where l.IsActive == true
                               orderby l.CreatedDate descending
                               select new ReportVM
                               {
                                   ReportTitle = l.ReportTitle,
                                   ReportId = l.ReportId
                               }
                               ).ToPagedList(pageno ?? 1, 10);

                return View(reports);
            }

            //var report = db.ReportMasters.Where(x=>x.IsActive==true).ToList();
            //return View(report);
        }
        [HttpGet]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportCreate()
        {
            return View();
        }
        [HttpPost]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportCreate(ReportVM r, HttpPostedFileBase file)
        {
            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string fileExt = System.IO.Path.GetExtension(ImageName);


                //checks whether file is is of type .jpg or bellow mensioned otherwise returns  message ...

                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".gif" && fileExt != ".png")
                {
                    ViewBag.ReportImage = "Only image formats (jpg, png, gif) are accepted ";
                    return View(r);
                }
                else
                {
                    string physicalPath = Server.MapPath("/Images/" + ImageName);
                    string imgpath = ("/Images/" + ImageName);
                    r.ReportImage = imgpath;
                    file.SaveAs(physicalPath);
                }
            }
            if (ModelState.IsValid)
            {
                var reportTitleExist = db.ReportMasters.Where(x => x.ReportTitle == r.ReportTitle).Select(x => x.ReportTitle).FirstOrDefault();

                if (!string.IsNullOrEmpty(reportTitleExist))
                {
                    ViewBag.ReportTitleExist = "Duplicate Report Title !...";
                    return View(r);
                }
                else
                {
                    if (r.ReportUrl == null)
                    {
                        r.ReportUrl = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(r.ReportTitle);
                    }
                    _ObjReportRepository.InsertReport(r);
                    return RedirectToAction("ReportIndex");
                }
            }
            else
                return View(r);

        }
        [HttpGet]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportEdit(int id)
        {
            var catid = db.ReportMasters.Where(x => x.ReportId == id).Select(x => x.CategoryId).FirstOrDefault();

            var z = _ObjReportRepository.EditGetReport(id);
            var TotalCat = _ObjReportRepository.DDLGetparents(catid);
            ViewBag.ParentCategory = _ObjReportRepository.GetParent(catid);
            ViewBag.ChildCategory = _ObjReportRepository.Getchild(catid);
            ViewBag.ChildOfChild = _ObjReportRepository.GetChilofChild(catid);
            return View(z);
        }
        [HttpPost]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportEdit(ReportVM r)
        {
            if (_ObjReportRepository.EditPostReport(r))
            {
                return RedirectToAction("ReportIndex");
            }
            else
            {
                //var catid = db.ReportMasters.Where(x => x.ReportId ==r.ReportId).Select(x => x.CategoryId).FirstOrDefault();
                ViewBag.DuplicateReportTitle = "Duplicate Report Title !...";
                ViewBag.ParentCategory = _ObjReportRepository.GetParent(r.CategoryId);
                ViewBag.ChildCategory = _ObjReportRepository.Getchild(r.CategoryId);
                ViewBag.ChildOfChild = _ObjReportRepository.GetChilofChild(r.CategoryId);
                return View(r);
            }

        }
        [HttpGet]
        //[CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportDetails(int id)
        {
            //var reportdetails = db.ReportMasters.Where(x => x.ReportId == id).FirstOrDefault();
          
            return View(_ObjReportRepository.GetReportById(id));
        }
        [HttpGet]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportDelete(int id)
        {
            var reportdetails = db.ReportMasters.Where(x => x.ReportId == id).FirstOrDefault();
            return View(reportdetails);
        }
        [HttpPost]
      //  [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        [ActionName(name: "ReportDelete")]
        public ActionResult ReportDelete1(int id)
        {
            
            return RedirectToAction("ReportIndex");
        }
        public ActionResult GetChildCategory(int id)
        {
            var z = db.CategoryMasters.Where(x => x.ParentCategoryId == id).Select(x => new
            {
                Value = x.CategoryId,
                Text = x.CategoryName
            }).ToList();
            return Json(z);
        }
        public ActionResult GetChildOfChild(int id)
        {
            var z = db.CategoryMasters.Where(x => x.ParentCategoryId == id).Select(x => new
            {
                Value = x.CategoryId,
                Text = x.CategoryName
            }).ToList();
            return Json(z);
        }

        //public IEnumerable<ExcelReport> GetReportData(DateTime FromDate, DateTime ToDate)
        //{
        //    var reportlist = (from l in db.ReportMasters
        //                      join c in db.CategoryMasters on l.CategoryId equals c.CategoryId
        //                      orderby l.CreatedDate descending
        //                      where l.CreatedDate >= FromDate.Date && FromDate.Date <= ToDate.Date
        //                      select new ExcelReport
        //                      {
        //                          ReportId = l.ReportId,
        //                          ReportTitle = l.ReportTitle,
        //                          FullDescription = l.FullDescription,
        //                          PublishingDate = l.PublishingDate,
        //                          CategoryName = c.CategoryName,
        //                          ReportUrl = "http://www.marketresearchtrade.com/report/" + l.ReportURL,
        //                          CreatedDate = l.CreatedDate
        //                      }).ToList();

        //    return reportlist;
        //}
        //public ActionResult ExportData(DateTime FromDate, DateTime ToDate)
        //{
        //    GridView gv = new GridView();

        //    var reportlist = (from l in db.ReportMasters
        //                      join c in db.CategoryMasters on l.CategoryId equals c.CategoryId
        //                      orderby l.CreatedDate descending
        //                      where l.CreatedDate >= FromDate.Date && FromDate.Date <= ToDate.Date
        //                      select new ExcelReport
        //                      {
        //                          ReportId = l.ReportId,
        //                          ReportTitle = l.ReportTitle,
        //                          FullDescription = l.FullDescription,
        //                          //htmlWrite.WriteLine("<td valign='top' nowrap >=\"" + ds.Tables[0].Rows[i][j].ToString().Replace("\r\n\r\n", "\" & CHAR(10)&CHAR(10) & \"").Replace("\r\n", "\" & CHAR(10) & \"").Replace(",", ",\"&\"").Replace("<pre>", "").Replace("</pre>", "") + "\"</td>");
        //                          PublishingDate = l.PublishingDate,
        //                          CategoryName = c.CategoryName,
        //                          ReportUrl = "http://www.marketresearchtrade.com/report/" + l.ReportURL,
        //                          CreatedDate = l.CreatedDate
        //                      }).ToList();
        //    foreach (var item in reportlist)
        //    {

        //        string CONCATENATE = "=CONCATENATE(";
        //        IEnumerable<string> Split = ZSubstring(item.FullDescription.Replace("<pre>", "").Replace("</pre>", ""), 200);

        //        foreach (string str in Split)
        //            CONCATENATE += "\"" + str + "\", ";

        //        CONCATENATE = CONCATENATE.Substring(0, CONCATENATE.Length - 2).Replace("\n", "\" & CHAR(10) & \"") + ")";
        //        item.FullDescription = CONCATENATE;

        //    }
        //    gv.DataSource = reportlist;
        //    gv.DataBind();
        //    Response.ClearContent();
        //    Response.Buffer = true;
        //    Response.AddHeader("content-disposition", "attachment; filename=ReportSheet.xls");
        //    Response.ContentType = "application/ms-excel";
        //    Response.Charset = "";
        //    StringWriter sw = new StringWriter();
        //    HtmlTextWriter htw = new HtmlTextWriter(sw);
        //    gv.RenderControl(htw);
        //    Response.Output.Write(sw.ToString());
        //    Response.Flush();
        //    Response.End();
        //    return View(reportlist);
        //    //return RedirectToAction("ReportIndex");
        //}
        //public string[] ZSubstring(string toSplit, int chunkSize)
        //{
        //    int stringLength = toSplit.Length;

        //    int chunksRequired = (int)Math.Ceiling((decimal)stringLength / (decimal)chunkSize);
        //    var stringArray = new string[chunksRequired];

        //    int lengthRemaining = stringLength;

        //    for (int i = 0; i < chunksRequired; i++)
        //    {
        //        int lengthToUse = Math.Min(lengthRemaining, chunkSize);
        //        int startIndex = chunkSize * i;
        //        stringArray[i] = toSplit.Substring(startIndex, lengthToUse);

        //        lengthRemaining = lengthRemaining - lengthToUse;
        //    }

        //    return stringArray;

        //}



    }
}

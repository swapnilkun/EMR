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
using System.Text;
using ExcellentMarketResearch.Areas.Admin.Models;


namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class ReportController : Controller
    {
        //
        // GET: /Admin/Report/

        private ReportRepository _ObjReportRepository;

        public ReportController()
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

        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportIndex(int? pageno, string searchkey, int? pagesize, DateTime? FromDate, DateTime? ToDate,string categoryId)
        {

            int catid = Convert.ToInt32(categoryId);
            if (FromDate != null)
            {
                var reports = (from r in db.ReportMasters
                               where r.CreatedDate >= FromDate && (r.CategoryId==catid)
                               select new ExcelReport
                               {
                                   ReportId = r.ReportId,
                                   ReportTitle = r.ReportTitle,
                                   ReportUrl = "https://www.excellentmarketresearch.com/report/" + r.ReportUrl,
                                   // TableOfContent = r.TableOfContent,
                                   FullDescription = r.LongDescritpion,
                                   PublishingDate = r.PublishingDate,
                                   ReportInquiryUrl = "https://www.excellentmarketresearch.com/report/" + r.ReportUrl + "#inquiry",
                                   RequestSampleUrl = "https://www.excellentmarketresearch.com/report/" + r.ReportUrl + "#reqsample"
                               }).ToList();
                Session["ExcelData"] = reports;
                var x = reports.ToPagedList(pageno ?? 1, pagesize ?? 10);
                return View(x);
            }

            if (!String.IsNullOrEmpty(searchkey))
            {
                // return View(db.ReportMasters.Where(x => x.ReportTitle.Contains(ReportTitle)).ToList().ToPagedList(pageno ?? 1, 4));
                ViewBag.totalsearchedreports = db.ReportMasters.Count(x => x.ReportTitle.Contains(searchkey));

                //var realsearch = searchkey.Replace("/" + searchkey + "/", searchkey);
                var z = (from l in db.ReportMasters
                         orderby l.CreatedDate descending
                         where l.ReportTitle.Contains(" " + searchkey + " ") || l.ReportTitle.Contains(searchkey + " ") || l.ReportTitle.Contains(" " + searchkey) && l.IsActive == true
                         select new ExcelReport
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
                               select new ExcelReport
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
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportCreate()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
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
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
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
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
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
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportDetails(int id)
        {
            //var reportdetails = db.ReportMasters.Where(x => x.ReportId == id).FirstOrDefault();

            return View(_ObjReportRepository.GetReportById(id));
        }
        [HttpGet]
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult ReportDelete(int id)
        {
            var reportdetails = db.ReportMasters.Where(x => x.ReportId == id).FirstOrDefault();
            return View(reportdetails);
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader,ReportCreater", "Create,Delete")]
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

        public ActionResult DownloadSheet()
        {
            List<ExcelReport> report = (List<ExcelReport>)Session["ExcelData"];
            try
            {
                string str = DateTime.Now.ToString();
                Response.ClearContent();
                Response.AddHeader("content-disposition", "attachment;filename=" + str + "CSV.csv");
                Response.ContentType = "application/text";
                StringBuilder stringbuilder = new StringBuilder();
                stringbuilder.Append(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\n", "Sr.No", "Report ID", "Report Title", "Report Url", "Full Description", "Publishing Date", "Inquiry Url", "Sample Url"));
                for (int index = 0; index < report.Count; index++)
                {
                    stringbuilder.Append(string.Format("\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\n", (index + 1), report[index].ReportId, report[index].ReportTitle, report[index].ReportUrl, report[index].FullDescription, report[index].PublishingDate, report[index].ReportInquiryUrl, report[index].RequestSampleUrl));
                }
                Response.Write(stringbuilder.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {

            }
            return RedirectToAction("ReportIndex", "Report");
        }
        [HttpPost]
        public ActionResult ExportData(DateTime FromDate, DateTime ToDate)
        {
            GridView gv = new GridView();

            var reportlist = (from l in db.ReportMasters
                              join c in db.CategoryMasters on l.CategoryId equals c.CategoryId
                              orderby l.CreatedDate descending
                              where l.CreatedDate >= FromDate.Date && FromDate.Date <= ToDate.Date
                              select new ExcelReport
                              {
                                  ReportId = l.ReportId,
                                  ReportTitle = l.ReportTitle,
                                  FullDescription = l.LongDescritpion,
                                  //htmlWrite.WriteLine("<td valign='top' nowrap >=\"" + ds.Tables[0].Rows[i][j].ToString().Replace("\r\n\r\n", "\" & CHAR(10)&CHAR(10) & \"").Replace("\r\n", "\" & CHAR(10) & \"").Replace(",", ",\"&\"").Replace("<pre>", "").Replace("</pre>", "") + "\"</td>");
                                  PublishingDate = l.PublishingDate,
                                  CategoryName = c.CategoryName,
                                  ReportUrl = "#report/" + l.ReportUrl,
                                  CreatedDate = l.CreatedDate
                              }).ToList();
            foreach (var item in reportlist)
            {

                string CONCATENATE = "=CONCATENATE(";
                IEnumerable<string> Split = ZSubstring(item.FullDescription.Replace("<pre>", "").Replace("</pre>", ""), 200);

                foreach (string str in Split)
                    CONCATENATE += "\"" + str + "\", ";

                CONCATENATE = CONCATENATE.Substring(0, CONCATENATE.Length - 2).Replace("\n", "\" & CHAR(10) & \"") + ")";
                item.FullDescription = CONCATENATE;

            }
            gv.DataSource = reportlist;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ReportSheet.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View(reportlist);
            //return RedirectToAction("ReportIndex");
        }
        public string[] ZSubstring(string toSplit, int chunkSize)
        {
            int stringLength = toSplit.Length;

            int chunksRequired = (int)Math.Ceiling((decimal)stringLength / (decimal)chunkSize);
            var stringArray = new string[chunksRequired];

            int lengthRemaining = stringLength;

            for (int i = 0; i < chunksRequired; i++)
            {
                int lengthToUse = Math.Min(lengthRemaining, chunkSize);
                int startIndex = chunkSize * i;
                stringArray[i] = toSplit.Substring(startIndex, lengthToUse);

                lengthRemaining = lengthRemaining - lengthToUse;
            }

            return stringArray;

        }



    }
}

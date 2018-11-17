using ExcellentMarketResearch.Areas.Admin.Models.DAL;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Areas.Admin.Models;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {

            private NewsRepository _ObjNewsRepository;
        public NewsController()
        {
            _ObjNewsRepository = new NewsRepository();
        }


        //NewsRepository ObjNewsRepository = new NewsRepository();
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
       [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsIndex()
        {
            return View(_ObjNewsRepository.GetNews());
        }
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsCreate()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsCreate(NewsVM news,HttpPostedFileBase file)
        {

            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string fileExt = System.IO.Path.GetExtension(ImageName);


                //checks whether file is is of type .jpg or bellow mensioned otherwise returns  message ...

                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".gif" && fileExt != ".png")
                {
                    ViewBag.NewsImage = "Only image formats (jpg, png, gif) are accepted ";
                    return View(news);
                }
                else
                {
                    string physicalPath = Server.MapPath("/Images/" + ImageName);
                    string imgpath = ("/Images/" + ImageName);
                    news.NewsImage = imgpath;
                    file.SaveAs(physicalPath);
                }
            }

            var newstitle = db.NewsMasters.Where(x => x.NewsTitle == news.NewsTitle).Select(x => x.NewsTitle).FirstOrDefault();

            if (newstitle == null)
            {
                _ObjNewsRepository.InsertNews(news);
                return RedirectToAction("NewsIndex");
            }
            else
            {
                ViewBag.DuplicateNewsTitle = "Duplicate NewsTitle !...";
                return View(news);
            }
        }
        
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsEdit(int id)
        {
            return View(_ObjNewsRepository.EditNews(id));
        }

        [HttpPost]
        [ValidateInput(false)]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsEdit(NewsVM news)
        {
            var newsTitlewiseid = db.NewsMasters.Where(x => x.NewsTitle == news.NewsTitle).Select(x => x.NewsId).FirstOrDefault();

            if (newsTitlewiseid == news.NewsId || newsTitlewiseid == 0)
            {
                if (news.NewsURL == null)
                {
                    var newsurl =ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(news.NewsTitle);
                    news.NewsURL = newsurl;
                }
                var newsUrlwiseid = db.NewsMasters.Where(x => x.NewsUrl == news.NewsURL).Select(x => x.NewsId).FirstOrDefault();

                if (newsUrlwiseid == news.NewsId || newsUrlwiseid == 0)
                {
                    _ObjNewsRepository.EditpostNews(news);
                    return RedirectToAction("NewsIndex");
                }
                else
                {
                    ViewBag.NewsURL = "Duplicate News Url ....";
                    return View(news);
                }
            }
            else
            {
                ViewBag.DuplicateNewsTitle = "Duplicate News Title ....";
                return View(news);
            }
        }
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsDetails(int id)
        {
            
            return View(_ObjNewsRepository.GetNewsById(id));

        }
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsDelete(int id)
        {
            return View(_ObjNewsRepository.GetNewsById(id));
        }
        [HttpPost]
        [ActionName(name: "NewsDelete")]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult NewsDelete1(int id)
        {
            var n = db.NewsMasters.Where(x => x.NewsId == id).Select(x => x).FirstOrDefault();
            n.IsActive = false;
            db.Entry(n).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewsIndex");
        }
    }
}

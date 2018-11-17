using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class AdminloginController : Controller
    {
        //
        // GET: /Admin/Adminlogin/
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginVM log)
        {
            if (ModelState.IsValid)
            {
                int? UserID = IsUserExist(log);
                if (UserID != null && UserID > 0)
                {
                    Session["UserId"] = UserID;
                    return RedirectToAction("Index", "Adminlogin");
                }
                else 
                {
                    return RedirectToAction("loginfailed");
                }

            }
            return View(log);
        }

        public ActionResult loginfailed()
        {
            return View();
        }
        public int? IsUserExist(LoginVM log)
        {

            int userId = (from l in db.UserMasters
                          where l.EmailAddress == log.EmailId && l.Password == log.Pwd
                          select l.UserId).FirstOrDefault();

            if (userId > 0)
            {
                return userId;
            }
            else return 0;
        }
        public ActionResult Logout(LoginVM log)
        {
            log.EmailId = "";
            log.Pwd = "";
            Session.Abandon();
            return RedirectToAction("Login", "Adminlogin");
        }

    }
}

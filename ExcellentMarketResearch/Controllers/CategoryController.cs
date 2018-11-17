using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Category/

        public ActionResult Index()
        {
            return View();
        }
        public CategoryDataRepository _ObjCatrepository;


        public CategoryController()
        {
            _ObjCatrepository = new CategoryDataRepository();
        }
       
        public ActionResult MainCategories()
        {
            var maincat = _ObjCatrepository.GetMainCategories();
            return View(maincat);
        }

        public ActionResult PartialCat()
        {
            var maincat = _ObjCatrepository.GetMainCategories();
            return PartialView(maincat);
        }
    }
}

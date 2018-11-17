using ExcellentMarketResearch.Areas.Admin.Models.DAL;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        //
        // GET: /Admin/Category/
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public CategoryRepository _ObjCategoryRepository;
        public CategoryController()
        {
            _ObjCategoryRepository = new CategoryRepository();
        }

       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CategoryIndex(int? pageno, string CategoryName)
        {

            if (string.IsNullOrEmpty(CategoryName))
            {
                return View((_ObjCategoryRepository.GetCategory()).ToPagedList(pageno ?? 1, 10));
            }
            else
            {
              return View(_ObjCategoryRepository.SerchedCategory(CategoryName).ToPagedList(pageno ?? 1, 10));
              
            }
        }
        [HttpGet]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CreateCategory()
        {
            return View();
        }
        [HttpPost]
        //[CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CreateCategory(CategoryVM catvm, HttpPostedFileBase file)
        {
            #region File Extension of image is proper or not
            if (file != null && !string.IsNullOrEmpty(file.FileName))
            {
                string ImageName = System.IO.Path.GetFileName(file.FileName);
                string fileExt = System.IO.Path.GetExtension(ImageName);

                //checks whether file is is of type .jpg or bellow mensioned otherwise returns  message ...

                if (fileExt != ".jpg" && fileExt != ".jpeg" && fileExt != ".gif" && fileExt != ".png")
                {
                    ViewBag.CategoryImage = "Only image formats (jpg, png, gif) are accepted ";
                    return View(catvm);
                }
                else
                {
                    // saving the file of image into the folder of Images in projects ... 

                    string physicalPath = Server.MapPath("/Images/" + ImageName);
                    string imgpath = ("/Images/" + ImageName);
                    catvm.CategoryIcon = imgpath;
                    file.SaveAs(physicalPath);
                }
            }
            # endregion File Extension of image is proper or not

            #region assign created by and created date
            catvm.CreatedBy = 1;//Convert.ToInt32(QYGroupRepository.Areas.Admin.Models.CommonCode.MySession());
            catvm.CreatedDate = DateTime.Now;
            #endregion assign created by and created date

            if (_ObjCategoryRepository.IsCategoryNameExist(catvm))
            {
                ViewBag.DuplicateTitle = "Duplicate Category Name !....";
                return View(catvm);
            }
            else if (!string.IsNullOrEmpty(catvm.CategoryURL))
            {
                _ObjCategoryRepository.InsertCategory(catvm);
            }
            else if (string.IsNullOrEmpty(catvm.CategoryURL))
            {
                catvm.CategoryURL = ExcellentMarketResearch.Areas.Admin.Models.Common.GenerateSlug(catvm.CategoryName);
                _ObjCategoryRepository.InsertCategory(catvm);
            }
            return RedirectToAction("CategoryIndex");
        }
        [HttpGet]
      //  [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CategoryEdit(int id)
        {
            var catdata = _ObjCategoryRepository.EditCategory(id);
            ViewBag.ParentCategories = _ObjCategoryRepository.Getparentcat(id);
            ViewBag.ChildCategories = _ObjCategoryRepository.Getchildcat(id);
            return View(catdata);
        }

        [HttpPost]
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        [ValidateInput(false)]
        public ActionResult CategoryEdit(CategoryVM cat)
        {
            bool x = _ObjCategoryRepository.EditCategoryPost(cat);
            if (x == true)
            {
                ViewBag.DuplicateCategoryName = "Duplicate CategoryName ..!";
                ViewBag.ParentCategories = _ObjCategoryRepository.Getparentcat(cat.CategoryId);
                ViewBag.ChildCategories = _ObjCategoryRepository.Getchildcat(cat.CategoryId);
                return View(cat);
            }
            else
            {
                return RedirectToAction("CategoryIndex");
            }
        }
       // [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CategoryDetails(int id)
        {
            var catdetails = _ObjCategoryRepository.GetCategoryById(id);
            return View(catdetails);
        }

        [HttpGet]
      //  [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CategoryDelete(int id)
        {
            var catm = _ObjCategoryRepository.GetCategoryById(id);
            return View(catm);
        }

        [HttpPost]
        [ActionName("CategoryDelete")]
      //  [CustomAuthorization("ReportUploader,ReportCreater", "Create,Delete")]
        public ActionResult CategoryDelete1(int id)
        {
            _ObjCategoryRepository.DeleteCategory(id);

            return RedirectToAction("CategoryIndex");
        }
        public List<CategoryMaster> GetParent()
        {
            //var parentcat = db.CategoryMasters.Where(x => x.ParentCategoryId == 0).OrderBy(x => x.CategoryName).ToList();
            return _ObjCategoryRepository.GetParent();
        }

        public ActionResult GetChild(int id)
        {
            var childcat = db.CategoryMasters.Where(x => x.ParentCategoryId == id).Select(x => new
            {
                Text = x.CategoryName,
                Value = x.CategoryId
            }).ToList();
           // var childcat = _ObjCategoryRepository.GetChild(id);
           // return Json(childcat,JsonRequestBehavior.AllowGet);
            return Json(childcat);
        }
  
     
    }
}

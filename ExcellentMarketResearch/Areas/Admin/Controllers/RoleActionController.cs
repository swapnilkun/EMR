using ExcellentMarketResearch.Areas.Admin.Models.DAL;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Areas.Admin.Models;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class RoleActionController : Controller
    {
        //
        // GET: /Admin/RoleAction/

        public ActionResult Index()
        {
            return View();
        }
         private RoleActionRepository _ObjRoleActionRepository;

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public RoleActionController()
        {
            _ObjRoleActionRepository = new RoleActionRepository();
        }
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionIndex()
        {
            var RoleAction = _ObjRoleActionRepository.GetRoleAction();
            return View(RoleAction);
        }
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionCreate()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionCreate(RoleActionVM roleaction, int[] actions)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _ObjRoleActionRepository.InsertAction(roleaction, actions);
                    return RedirectToAction("RoleActionIndex", "RoleAction");
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

            }
            return View("RoleActionIndex");
        }

        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionDetail(int id)
        {
            var Detail = _ObjRoleActionRepository.DetailAction(id);
            return View(Detail);
        }
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionEdit(int id = 0)
        {
            var EditGet = _ObjRoleActionRepository.GetRoleActionsEdit(id);
            return View(EditGet);
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionEdit(int id, RoleMaster rolemaster, int[] actions)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    _ObjRoleActionRepository.UpdateAction(id, rolemaster, actions);
                    return RedirectToAction("RoleActionIndex");
                }
                catch (DbEntityValidationException dbEx)
                {
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                }

            }
            return RedirectToAction("RoleAction", "RoleActionIndex");
        }
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionDelete(int id)
        {
            return View(_ObjRoleActionRepository.DetailAction(id));
        }
        [ActionName("RoleActionDelete")]
        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult RoleActionDelete1(int id)
        {
            _ObjRoleActionRepository.DeleteAction(id);
            return RedirectToAction("RoleActionIndex");
        }

    }
}

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
    public class UserRoleController : Controller
    {
        //
        // GET: /Admin/UserRole/


        UserRoleRepository ObjUserRoleRepository = new UserRoleRepository();
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleIndex()
        {
            return View(ObjUserRoleRepository.GetUserRole());
        }
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleCreate()
        {
            return View();
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleCreate(UserRoleVM userrole, int[] Roles)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    ObjUserRoleRepository.InsertUserRole(userrole, Roles);
                    return RedirectToAction("UserRoleIndex");
                }
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
            return View(userrole);
        }
        [HttpGet]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleEdit(int id)
        {
            var x = ObjUserRoleRepository.EditingData(id);
            return View(x);
        }
        [HttpPost]
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleEdit(UserRoleVM userrole, int[] Roles)
        {
            ObjUserRoleRepository.UpdateUserRole(userrole.UserId, userrole, Roles);
            return RedirectToAction("UserRoleIndex");
        }
        [CustomAuthentication("ReportUploader", "Create,Edit,Delete")]
        public ActionResult UserRoleDetail(int id)
        {
            var x = ObjUserRoleRepository.DetailUserRole(id);
            return View(x);
        }

    }
}

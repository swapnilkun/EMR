using ExcellentMarketResearch.Areas.Admin.Models.DAL;
using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Areas.Admin.Controllers
{
    public class ActionController : Controller
    {
        //
        // GET: /Admin/Action/
        private ActionRepository _ObjActionRepository;
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();
        public ActionController()
        {
            _ObjActionRepository = new ActionRepository();
        }

        public ActionResult ActionIndex()
        {
            return View(_ObjActionRepository.GetAction());
        }
        [HttpGet]
        public ActionResult CreateAction()
        {
            return View();
        }
        [HttpPost]
        public ActionResult CreateAction(ActionVM actionvm)
        {
            _ObjActionRepository.InsertAction(actionvm);
            return RedirectToAction("ActionIndex");
        }
        public ActionResult EditAction(int id = 0)
        {
            ActionMaster actionmaster = db.ActionMasters.Find(id);
            if (actionmaster == null)
            {
                return HttpNotFound();
            }
            return View(actionmaster);
        }

        //
        // POST: /Admin/Action/Edit/5

        [HttpPost]
        public ActionResult EditAction(ActionVM actionvm)
        {
            if (ModelState.IsValid)
            {
                _ObjActionRepository.UpdateAction(actionvm);
                return RedirectToAction("ActionIndex");
            }
            return View(actionvm);
        }
        //
        // GET: /Admin/Action/Delete/5

        public ActionResult DeleteAction(int id = 0)
        {
            var actiondetail = _ObjActionRepository.GetActionById(id);
            if (actiondetail == null)
            {
                return HttpNotFound();
            }
            return View(actiondetail);
        }

        //
        // POST: /Admin/Action/Delete/5

        [HttpPost, ActionName("DeleteAction")]
        public ActionResult DeleteConfirmed(int id)
        {
            _ObjActionRepository.DeleteAction(id);
            return RedirectToAction("ActionIndex");
        }


    }
}

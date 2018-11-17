using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class RoleActionRepository
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public IEnumerable<spSelectRoleAction_Result> GetRoleAction()
        {
            return db.spSelectRoleAction().ToList();
        }
        public void InsertAction(RoleActionVM Roleaction, int[] actions)
        {

            db.spRoleActionInsert(
                                          Roleaction.RoleId,
                                          Roleaction.RoleName,
                                          Roleaction.IsActive,
                                          Roleaction.CreatedBy = 1,//(int)HttpContext.Current.Session["userid"],
                                          Roleaction.ModifiedBy,
                                          Roleaction.CreatedDate = DateTime.Now,
                                          Roleaction.ModifiedDate,
                                          string.Join(",", actions)
                                          );

            db.SaveChanges();

        }
        //public static IEnumerable<int> StringToIntList(string str)
        //{
        //    if (String.IsNullOrEmpty(str))
        //        yield break;

        //    foreach (var s in str.Split(','))
        //    {
        //        int num;
        //        if (int.TryParse(s, out num))
        //            yield return num;
        //    }
        //}

        public void UpdateAction(int id, RoleMaster rolemaster, int[] actions)
        {

            db.spRoleActionUpdate(
                                  id,
                                  rolemaster.RoleName,
                                  rolemaster.IsValid,
                                  rolemaster.CreatedBy,
                                  rolemaster.ModifiedBy = 1,//(int)HttpContext.Current.Session["userid"],
                                  rolemaster.CreatedDate,
                                  rolemaster.ModifiedDate = DateTime.Now,
                                  string.Join(",", actions)
                                  );

        }
        public void DeleteAction(int RoleId)
        {
            var RolesActions = db.spRoleActionDelete(RoleId);
        }
        public RoleActionVM GetRoleActionsEdit(int RoleId)
        {
            var actionids = (from l in db.RoleActionRelations
                             where l.RId == RoleId
                             select l.AId).ToArray();

            RoleActionVM r = new RoleActionVM();

            r.ActionId = actionids;

            var roleactiondata = (from rm in db.RoleMasters
                                  join ra in db.RoleActionRelations
                                  on rm.RoleId equals ra.RId
                                  where rm.RoleId == ra.RId
                                  select new
                                  {
                                      RoleId = ra.RId,
                                      RoleName = rm.RoleName,
                                      IsActive = rm.IsValid,
                                      CreatedBy = rm.CreatedBy,
                                      CreatedDate = rm.CreatedDate,
                                  }).FirstOrDefault();

            r.RoleId = roleactiondata.RoleId;
            r.IsActive = roleactiondata.IsActive;
            r.RoleName = roleactiondata.RoleName;
            r.CreatedBy = roleactiondata.CreatedBy;
            r.CreatedDate = roleactiondata.CreatedDate;
            return r;

        }
        public spDetailsRoleAction_Result DetailAction(int RoleId)
        {
            var x = db.spDetailsRoleAction(RoleId).SingleOrDefault();
            return x;
        }
        public IList<ActionMaster> GetActions()
        {
            return db.ActionMasters.ToList();
        }
    }
}
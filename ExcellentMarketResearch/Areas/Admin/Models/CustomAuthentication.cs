using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Models;
namespace ExcellentMarketResearch.Areas.Admin.Models
{
    public class CustomAuthentication:AuthorizeAttribute
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public string Roles {get;set;}
        public string Actions{get;set;}

        string[] _roles;
        string[] _actions;

        public CustomAuthentication(string Roles, string Actions)
        {
            _roles = Roles.Split(new char[]{ ','});
            _actions = Actions.Split(new char[] { ',' });
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (HttpContext.Current.Session["userid"] != null)
            {
               int userid = (int)HttpContext.Current.Session["userid"];
               var roles = (from l in db.RoleMasters
                            join u in db.UserRoleRelations on l.RoleId equals u.URoleId
                            where u.UserId == userid
                            select l.RoleName).ToList();
               if (_roles.Count(x => roles.Contains(x)) > 0)
               {
                   var actions = (from a in db.ActionMasters
                                  join ra in db.RoleActionRelations on a.ActionId equals ra.AId
                                  join u in db.UserRoleRelations on ra.RId equals u.URoleId
                                  where u.UserId == userid
                                  select a.ActionName
                                  ).ToList();
                   if (_actions.Count(x => actions.Contains(x)) <= 0)
                   {
                       filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                       {
                           controller = "Adminlogin",
                           action = "login",
                           area = "Admin"
                       }));
                   }
               }
               else
               {
                   filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                   {
                       controller = "Adminlogin",
                       action = "login",
                       area = "Admin"
                   }));
               }
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(new System.Web.Routing.RouteValueDictionary(new
                {
                    controller = "Adminlogin",
                    action = "login",
                    area = "Admin"
                }));
            }
        }
    
    }
}
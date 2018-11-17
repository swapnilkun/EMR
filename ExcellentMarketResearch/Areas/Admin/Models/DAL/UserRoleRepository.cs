using ExcellentMarketResearch.Areas.Admin.Models.ViewModel;
using ExcellentMarketResearch.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.DAL
{
    public class UserRoleRepository : IUserRoleRepository
    {
        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public IEnumerable<spUserRoleSelect_Result> GetUserRole()
        {
            return db.spUserRoleSelect().ToList();
        }
        public void InsertUserRole(UserRoleVM userrole , int[] Roles)
        {
           
            try
            {
                db.spUserRoleInsert(
                            userrole.UserId,
                            userrole.UserFName,
                            userrole.UserLName,
                            userrole.CurrentAddress,
                            userrole.State,
                            userrole.City,
                            userrole.MobileNumber,
                            userrole.PermanentAddress,
                            userrole.Gender,
                            userrole.CompanyName,
                            userrole.EmailId,
                            userrole.PWD,
                            userrole.IsActive,
                            userrole.CreatedBy=10,
                            userrole.ModifiedBy,
                            userrole.CreatedDate = DateTime.Now,
                            userrole.ModifiedDate,
                            string.Join(",", Roles));
                db.SaveChanges();
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
        public void UpdateUserRole(int id, UserRoleVM userrole, int[] Roles)
        {
            db.spUserRoleUpdate(
                id
            , userrole.UserFName
           , userrole.UserLName
           , userrole.CurrentAddress
           , userrole.State
           , userrole.City
           , userrole.MobileNumber
           , userrole.PermanentAddress
           , userrole.Gender
           , userrole.CompanyName
           , userrole.EmailId
           , userrole.PWD
           , userrole.IsActive
           , userrole.CreatedBy
           , userrole.ModifiedBy
           , userrole.CreatedDate
           , userrole.ModifiedDate
           , string.Join(",", Roles)
                           );
            db.SaveChanges();


        }


        public spDetailsUserRole_Result DetailUserRole(int id)
        {
            return db.spDetailsUserRole(id).FirstOrDefault();
        }
        public List<RoleMaster> GetRoles()
        {
            return db.RoleMasters.Where(x => x.IsValid == true).ToList();
        }
        public UserRoleVM EditingData(int id)
        {
            var Roleids = (from ra in db.UserRoleRelations
                           where ra.UserId == id
                           select ra.URoleId).ToArray();

            UserMaster um = db.UserMasters.Find(id);

            UserRoleVM userreg = new UserRoleVM();

            userreg.RoleId = Roleids;
            userreg.UserId = um.UserId;
            userreg.UserFName = um.UserFName;
            userreg.UserLName = um.UserLName;
            userreg.Gender = um.Gender;
            userreg.EmailId = um.EmailAddress;
            userreg.MobileNumber = um.ContactNumber;
            userreg.PermanentAddress = um.PermnentAddress;
            userreg.CurrentAddress = um.CurrentAddress;
            userreg.PWD = um.Password;
            userreg.CreatedBy = um.CreatedBy;
            userreg.CreatedDate = um.CreatedDate;
            userreg.IsActive = um.IsActive;
            userreg.ModifiedBy = 1;
            userreg.ModifiedDate = DateTime.Now;
            return userreg;
        }
    }
}
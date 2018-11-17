using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class UserRoleVM
    {
        public int? UserRoleId { get; set; }
        public int UserId { get; set; }


        [Required(ErrorMessage = "First Name should not be Empty")]
        [Display(Name = "First Name")]
        public string UserFName { get; set; }

        [Required(ErrorMessage = "Last Name should not be Empty")]
        [Display(Name = "Last Name")]
        public string UserLName { get; set; }

        [Required(ErrorMessage = "Email-Id should not be Empty")]
        [Display(Name = "Email-Id")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Mobile Number should not be Empty")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        [Required(ErrorMessage = "First should not be Empty")]
        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Permanet Address should not be Empty")]
        [Display(Name = "Permanet Address")]
        public string PermanentAddress { get; set; }

        [Required(ErrorMessage = "Current Address should not be Empty")]
        [Display(Name = "Current Address")]
        public string CurrentAddress { get; set; }

        [Required(ErrorMessage = "Password have to give")]
        [Display(Name = "Password")]
        public string PWD { get; set; }

        public string City { get; set; }
        public string State { get; set; }

        //[Required(ErrorMessage = "Role have to be select")]
        [Display(Name = "Roles Of User")]
        public int[] RoleId { get; set; }

        public string CompanyName { get; set; }

        public int CreatedBy { get; set; }

        public int? ModifiedBy { get; set; }

        //Initially means first time creation it must be null
        [Column(TypeName = "datetime2")]
        public System.DateTime CreatedDate { get; set; }

        [Column(TypeName = "datetime2")]
        public System.DateTime? ModifiedDate { get; set; }


        public bool IsActive { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Areas.Admin.Models.ViewModel
{
    public class LoginVM
    {
        [Key]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Email Id Required")]
        [Display(Name = "Email ID")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
                                    ErrorMessage = "Email Format is wrong")]
        [StringLength(50, ErrorMessage = "Less than 50 characters")]
        public string EmailId { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Password Required")]
        [Display(Name = "Password")]
        [StringLength(30, ErrorMessage = ":Maximum than 30 characters")]
        [Editable(false)]

        public string Pwd { get; set; }
    }
}
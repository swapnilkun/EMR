using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Models.ViewModel
{
    public class ContactUsVM
    {
        public int CustomerId { get; set; }

        [Display(Name = "Customer Name")]
        [MaxLength(30), MinLength(2)]
        [Required(ErrorMessage = "Customer name should not be empty")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email-ID Required:")]
        [Display(Name = "Email Id:")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
                 ErrorMessage = "Email Format is wrong")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Phone Number should not be empty")]
        public string PhoneNumber { get; set; }

        public string Company { get; set; }

        public int? ReportId { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Customer Message should not be empty")]
        public string CustomerMessage { get; set; }

        [Required(ErrorMessage = "Enter Verification Code")]
        [Display(Name = "Verification Code:")]
        public string CaptchaCode { get; set; }

        //[Required(ErrorMessage = "Area code not be empty")]
        //public string AreaCode { get; set; }

        public string RealCaptcha { get; set; }

        public string Country { get; set; }
    }
}
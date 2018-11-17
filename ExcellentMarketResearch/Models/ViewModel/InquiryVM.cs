using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Models.ViewModel
{
   
        public class InquiryVM
        {

            ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

            public int CustomerId { get; set; }

            [Display(Name = "Customer Name")]
            [MaxLength(30), MinLength(2)]
            [Required(ErrorMessage = " Name must be required.")]
            public string Name { get; set; }

            [Required(ErrorMessage = "Email-Id is required.")]
            [Display(Name = "Email Id:")]
            [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
                     ErrorMessage = "Please enter valid email address.")]
            public string EmailId { get; set; }

            [Display(Name = "Designation")]
            public string Designation { get; set; }

            [Display(Name = "Company")]
            [Required(ErrorMessage = " Company name reuired.")]
            public string Company { get; set; }

            //[Required(ErrorMessage = "Enter Verification Code")]
            //[Display(Name = "Verification Code:")]
            public string CaptchaCode { get; set; }

            [Display(Name = "Customer Message")]
            //[Required(ErrorMessage = "Customer Message should not be empty")]
            public string CustomerMessage { get; set; }

            [Display(Name = "Country Name")]
            //[Required(ErrorMessage = "Country name should not be empty")]
            public string Country { get; set; }

            [Required(ErrorMessage = "Phone Number required.")]
            public string PhoneNumber { get; set; }

            public int ReportId { get; set; }

            public string RealCaptcha { get; set; }

            public string FormType { get; set; }

            public string ReportTitle { get; set; }

            [Required(ErrorMessage = "Country code not be empty")]
            public string AreaCode { get; set; }

            public string ReportUrl { get; set; }

            public struct DDLStructure
            {
                public String Value { get; set; }
                public String Text { get; set; }

            }
            public List<Country> GetCountries()
            {
                var countries = db.Countries.OrderBy(x => x.name).ToList();
                return countries;
            }
            //public List<Country> GetPhoneCode()
            //{
            //    var phonecodes = db.Countries.OrderBy(x => x.name).ToList();
            //    return
            //}
        }

    
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ExcellentMarketResearch.Models.ViewModel
{
    public class BuyingVM
    {
        public int CustomerId { get; set; }

        public int ReportId { get; set; }

        [Display(Name = "Customer Name")]
        [Required(ErrorMessage = "Name is required."), MaxLength(100, ErrorMessage = "Name should not be more than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email id is required."), MaxLength(50, ErrorMessage = "Email id should not be more than 50 characters.")]
        [Display(Name = "Email Id:")]
        [RegularExpression(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$",
        ErrorMessage = "Email Format is wrong")]
        public string EmailId { get; set; }

        [Display(Name = "Designation")]
        [Required(ErrorMessage = "Designation/Title is required."), MaxLength(20, ErrorMessage = "Designation/Title should not be more than 20 characters.")]
        public string Designation { get; set; }

        [Display(Name = "Company")]
        [Required(ErrorMessage = "Company is required."), MaxLength(50, ErrorMessage = "Company name should not be more than 50 characters.")]
        public string Company { get; set; }

        //[Required(ErrorMessage = "Captcha is required."), MaxLength(8, ErrorMessage = "Captcha should not be more than 5 characters.")]
        //[Display(Name = "Verification Code:")]
        public string CaptchaCode { get; set; }

        [Display(Name = "Customer Message")]
        public string CustomerMessage { get; set; }

        [Display(Name = "Country Name")]
        [Required(ErrorMessage = "Country is required."), MaxLength(20, ErrorMessage = "Country should not be more than 20 characters.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Phone Number is required."), MaxLength(15, ErrorMessage = "Phone # should not be more than 15 characters.")]
        public string PhoneNumber { get; set; }



       // [Required(ErrorMessage = "User name should not be empty")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "State is required."), MaxLength(20, ErrorMessage = "State should not be more than 20 characters.")]
        public string State { get; set; }

        [Required(ErrorMessage = "City is required."), MaxLength(20, ErrorMessage = "City should not be more than 20 characters.")]
        public string City { get; set; }

        
        public string Type { get; set; }

        [Required(ErrorMessage = "Address is required."), MaxLength(150, ErrorMessage = "Address should not be more than 150 characters.")]
        public string Address { get; set; }

        [Required(ErrorMessage = "ZIP is required."), MaxLength(10, ErrorMessage = "ZIP should not be more than 10 characters.")]
        public string Zipcode { get; set; }

        public string Paymentmode { get; set; }

        public string RealCaptcha { get; set; }

        [Required(ErrorMessage = "Area code is required")]
        public string AreaCode { get; set; }

        public string IPAddress { get; set; }

        //   public IList<OrderSummary> OrderSummary { get; set; }

        public string GuId { get; set; }

        public string ReportUrl { get; set; }//report URL

        public string l { get; set; }//selected report type

        public string TransactionId { get; set; }

        public string ReportTitle { get; set; }

        // public decimal? Price { get; set; }

        public string PaymentThrough { get; set; }

        //public PriceVM Prices { get; set; }

    }
}
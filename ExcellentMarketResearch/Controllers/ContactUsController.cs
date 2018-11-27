using ExcellentMarketResearch.Models;
using ExcellentMarketResearch.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Newtonsoft.Json.Linq;
using ExcellentMarketResearch.Models.PaymentGateway;

namespace ExcellentMarketResearch.Controllers
{
    public class ContactUsController : Controller
    {
        //
        // GET: /ContactUs/

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        public ActionResult ContactusThanks()
        {
            if (Session["Name"] == null)
            {
                return HttpNotFound();
            }
            else
            {
                ViewBag.Name = Session["Name"].ToString();
                return View();
            }
        }
        public ActionResult GetinTouch()
        {
            ContactUsVM cnt = new ContactUsVM();
            Session["Captcha"] = DrawCaptcha();
            var PlainText = Session["Captcha"].ToString();
            var EncryCaptcha = ExcellentMarketResearch.Areas.Admin.Models.Common.Encrypt(PlainText);
            cnt.ReportId = 0;
            cnt.RealCaptcha = EncryCaptcha;
            return PartialView(cnt);
        }
        public ActionResult Index()
        {
            ContactUsVM cnt = new ContactUsVM();
            Session["Captcha"] = DrawCaptcha();
            var PlainText = Session["Captcha"].ToString();
            var EncryCaptcha = ExcellentMarketResearch.Areas.Admin.Models.Common.Encrypt(PlainText);
            cnt.ReportId = 0;
            cnt.RealCaptcha = EncryCaptcha;
            return View(cnt);
        }

        [HttpPost]
        public ActionResult Index(ContactUsVM eq)
        {
            InquiryVM e = new InquiryVM();
            Emailsending objEmailsending = new Emailsending();
            var response = Request["g-recaptcha-response"];
            var catptchastatus = false;
            string secreatekey = "6LdU_nUUAAAAAD6JiuKTysnVW6Aa4D5SU0z1Fl4u";
            var client = new WebClient();
            string resstring = string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secreatekey, response);
            var result = client.DownloadString(resstring);
            if (result.ToLower().Contains("false"))
            {
                catptchastatus = false;
            }
            else
            {
                catptchastatus = true;
            }

            //int FormTypeId=4;
            string Publisher = string.Empty;
            if (catptchastatus)
            {
                if (ModelState.IsValid)
                {
                    // cap = Session["Captcha"].ToString();
                    var cap = ExcellentMarketResearch.Areas.Admin.Models.Common.Decrypt(eq.RealCaptcha);

                    Publisher = "!";

                    //if (eq.CaptchaCode == cap)
                    //{
                    CustomerInquiry cst = new CustomerInquiry();

                    cst.CustomerMessage = eq.CustomerMessage;
                    cst.EmailId = eq.EmailId;
                    cst.Name = eq.Name;
                    // eq.AreaCode += "-" + eq.PhoneNumber;
                    cst.PhoneNumber = eq.PhoneNumber;
                    cst.ReportId = eq.ReportId;
                    cst.Country = eq.Country;
                    //cst.CaptchaCode = eq.CaptchaCode;

                    string ReportTitle = string.Empty;
                    string ReportUrl = string.Empty;

                    try
                    {
                        db.CustomerInquiries.Add(cst);
                        //db.Entry(cst).State = EntityState.Added;

                        db.SaveChanges();

                        //  QYGroupRepository.PaymentGateway.Emailsending objEmailsending = new QYGroupRepository.PaymentGateway.Emailsending();




                        //Auto Mailer
                        objEmailsending.SendEmail("sales@excellentmarketresearch.com", "Sales", cst.EmailId, "balasahebpatil1612@gmail.com", "", "ExcellentMarketResearch.com  : ContactUs" + " ", GenerateMailBody_ContactUs_AutoReply(cst.Name, ReportTitle));

                        //To company
                        objEmailsending.SendEmail("sales@excellentmarketresearch.com", "Sales", "sales@excellentmarketresearch.com,balasahebpatil1612@gmail.com", "", "md@excellentmarketresearch.com", "ExcellentMarketResearch.com " + " : " + "Contact Us", GenerateMailBody_ContactUs(ReportTitle, cst.Name, cst.EmailId, cst.PhoneNumber, "!", "!", "!", cst.CustomerMessage));

                        Session["Name"] = cst.Name;

                        return RedirectToAction("ContactusThanks", "ContactUs");
                        //return RedirectToRoute(new
                        //{
                        //    controller = "InquiryForm",
                        //    action = "Index",
                        //    reporrtid = cst.ReportId
                        //});


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


                    //Return the if model not valid
                    return View();
                }
            }


            return View(eq);
        }
        public string DrawCaptcha()
        {
            StringBuilder randomText = new StringBuilder();

            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";

            Random r = new Random();

            for (int j = 0; j <= 5; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }

            return randomText.ToString();
        }

        //Auto Company
        private string GenerateMailBody_ContactUs(string ReportTitle, string Name, string EmailID, string ContactNo, string NameOfCompany, string CountryName, string Designation, string CustomerMessage)
        {
            string result = "";
            result = "Dear Admin,<br /><br />" + "<table>";
            result += ReportTitle != "" ? "<tr> <td valign='top' width='30%'><b>Report Title</b></td>   <td valign='top' width='2%'><b> : </b></td> <td valign='top' width='68%'>" + ReportTitle + "</td> </tr>" : "";
            result += Name != "" ? "<tr> <td valign='top'><b>Customer Name</b></td>  <td valign='top'><b> : </b></td> <td valign='top'>" + Name + "</td> </tr>" : "";
            result += EmailID != "" ? "<tr> <td valign='top'><b>Email ID</b></td>       <td valign='top'><b> : </b></td> <td valign='top'>" + EmailID + "</td> </tr>" : "";
            result += ContactNo != "" ? "<tr> <td valign='top'><b>Phone</b></td>          <td valign='top'><b> : </b></td> <td valign='top'>" + ContactNo + "</td> </tr>" : "";
            result += NameOfCompany != "" ? "<tr> <td valign='top'><b>Company Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + NameOfCompany + "</td> </tr>" : "";
            result += Designation != "" ? "<tr> <td valign='top'><b>Designation</b></td>    <td valign='top'><b> : </b></td> <td valign='top'>" + Designation + "</td> </tr>" : "";
            result += CountryName != "" ? "<tr> <td valign='top'><b>Country Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + CountryName + "</td> </tr>" : "";
            result += CustomerMessage != "" ? "<tr> <td valign='top'><b>Enquiry Text</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + CustomerMessage + "</td> </tr>" : "";
            result += "<tr> <td valign='top'><b>Publisher</b></td><td valign='top'><b> : </b></td> <td valign='top'>" + "Market Research Trade" + "</td> </tr>";
            result += "<tr> <td valign='top'><b>IP Address</b></td><td valign='top'><b> : </b></td> <td valign='top'>" + ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress() + "</td> </tr>";
            result += "</table>";
            return result;
        }
        //Auto mailer
        public string GenerateMailBody_ContactUs_AutoReply(string Name, string ReportTitle)
        {
            string result = "";
            if (ReportTitle == "")
            {
                result = "Dear " + Name + ","
                     + "<br /><br />Thank you for your interest in <b>" + "ExcellentMarketResearch.Com" + "</b>."
                     + "<br /><br />We'll contact you soon to serve your research needs."
                     + "<b><br /><br />Warm regards,"
                     + "<br />Robert Smith | Corporate Sales Specialist,USA"
                     + "<br />Direct line: + 1-312-588-9716"
                     + "<br />" + "excellentmarketresearch.com"
                     + "<br />E-mail: robertsmith@excellentmarketresearch.com | Web: " + "excellentmarketresearch.com" + "</b>";
            }
            else
            {
                result = "Dear " + Name + ","
                    + "<br /><br />Thank you for your interest in our research report, <b>" + ReportTitle + "</b>."
                    //+ "<br /><br />I will share the sample pages shortly."
                    //+ "<br /><br />For your reference please find the below link."
                    //+ "<br /><br />" + "QYGroup.biz".Substring(0, "QYGroup.biz".Length - 1) + ReportURL
                    + "<br /><br />We'll contact you soon to serve your research needs."
                    + "<b><br /><br />Warm regards,"
                    + "<br />Joel John | Corporate Sales Specialist,USA"
                    + "<br />Direct line: + 1-445-45-54#"
                    + "<br />" + "excellentmarketresearch.com"
                    + "<br />E-mail: josss@excellentmarketresearch | Web: " + "excellentmarketresearch.com" + "</b>";
            }
            return result;
        }

        //private string GenerateMailBody_RequestSample(string ReportTitle, string Name, string EmailID, string ContactNo, string NameOfCompany, string CountryName, string Designation, string CustomerMessage)
        //{
        //    string result = "";
        //    result = "Dear Admin,<br /><br />" + "<table>";
        //    result += ReportTitle != "" ? "<tr> <td valign='top' width='30%'><b>Report Title</b></td>   <td valign='top' width='2%'><b> : </b></td> <td valign='top' width='68%'>" + ReportTitle + "</td> </tr>" : "";
        //    result += Name != "" ? "<tr> <td valign='top'><b>Customer Name</b></td>  <td valign='top'><b> : </b></td> <td valign='top'>" + Name + "</td> </tr>" : "";
        //    result += EmailID != "" ? "<tr> <td valign='top'><b>Email ID</b></td>       <td valign='top'><b> : </b></td> <td valign='top'>" + EmailID + "</td> </tr>" : "";
        //    result += ContactNo != "" ? "<tr> <td valign='top'><b>Phone</b></td>          <td valign='top'><b> : </b></td> <td valign='top'>" + ContactNo + "</td> </tr>" : "";
        //    result += NameOfCompany != "" ? "<tr> <td valign='top'><b>Company Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + NameOfCompany + "</td> </tr>" : "";
        //    result += Designation != "" ? "<tr> <td valign='top'><b>Designation</b></td>    <td valign='top'><b> : </b></td> <td valign='top'>" + Designation + "</td> </tr>" : "";
        //    result += CountryName != "" ? "<tr> <td valign='top'><b>Country Name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + CountryName + "</td> </tr>" : "";
        //    result += CustomerMessage != "" ? "<tr> <td valign='top'><b>Enquiry Text</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + CustomerMessage + "</td> </tr>" : "";
        //    result += "<tr> <td valign='top'><b>Publisher</b></td><td valign='top'><b> : </b></td> <td valign='top'>" + "Market Research Trade" + "</td> </tr>";
        //    result += "<tr> <td valign='top'><b>IP Address</b></td><td valign='top'><b> : </b></td> <td valign='top'>" + QYGroupRepository.PaymentGateway.IPAddress.GetIPAddress() + "</td> </tr>";
        //    result += "</table>";
        //    return result;
        //}

        //public string GenerateMailBody_RequestSample_AutoReply(string Name, string ReportTitle)
        //{
        //    string result = "";
        //    if (ReportTitle == "")
        //    {
        //        result = "Dear " + Name + ","
        //             + "<br /><br />Thank you for your interest in <b>" + "Excellentmarket.biz" + "</b>."
        //            //+ "<br /><br />For your reference please find the below link."
        //            //+ "<br /><br />" + "QYGroup.biz"
        //            //+ "<br /><br />I'll contact you soon to serve your research needs."
        //             + "<br /><br />We'll contact you soon to serve your research needs."
        //             + "<b><br /><br />Warm regards,"
        //             + "<br />Joel John | Corporate Sales Specialist,USA"
        //             + "<br />Direct line: + 1-855-465-4651"
        //             + "<br />" + "marketresearchtrade.com"
        //             + "<br />E-mail: jl@qyroup.biz | Web: " + "ExcellentMarketResearch.com" + "</b>";
        //    }
        //    else
        //    {
        //        result = "Dear " + Name + ","
        //            + "<br /><br />Thank you for your interest in our research report, <b>" + ReportTitle + "</b>."
        //            //+ "<br /><br />I will share the sample pages shortly."
        //            //+ "<br /><br />For your reference please find the below link."
        //            //+ "<br /><br />" + "QYGroup.biz".Substring(0, "QYGroup.biz".Length - 1) + ReportURL
        //            + "<br /><br />We'll contact you soon to serve your research needs."
        //            + "<b><br /><br />Warm regards,"
        //            + "<br />Joel John | Corporate Sales Specialist,USA"
        //            + "<br />Direct line: + 1-8556156"
        //            + "<br />" + "marketresearchtrade.com"
        //            + "<br />E-mail: jja@qygroup.biz | Web: " + " ExcellentMarketResearch.com" + "</b>";
        //    }
        //    return result;
        //}

    }
}

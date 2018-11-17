using ExcellentMarketResearch.Models;
using ExcellentMarketResearch.Models.PaymentGateway;
using ExcellentMarketResearch.Models.ViewModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Controllers
{
    public class InquiryFormController : Controller
    {
        //
        // GET: /InquiryForm/


        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        string cap = string.Empty;
        string text = string.Empty;

        public ActionResult Index()
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

        [HttpGet]
        public ActionResult InquiryForm(int? ReportID, string FormType)
        {
            InquiryVM Enquiredata = new InquiryVM();

            if (ReportID > 0)
            {
                var Reports = (from l in db.ReportMasters
                               where l.ReportId == ReportID
                               select new
                               {
                                   l.ReportUrl,
                                   l.ReportTitle
                               }
                               ).FirstOrDefault();

                Enquiredata.ReportTitle = Reports.ReportTitle.ToString();
                Enquiredata.ReportUrl = Reports.ReportUrl.ToString();
            }
            else
            {
                Enquiredata.ReportTitle = "!";
                Enquiredata.ReportUrl = "!";

            }
            if (FormType == "SampleRequestForm")
            {
                ViewBag.FormTitle = "<b>Please submit the form will contact you within 24 hours. :</b><div style='font-size:15px;width:100%;'><label style='color:red; display:inline;'>*</label> Indicates required fields</div>";
            }
            else
            {
                ViewBag.FormTitle = "<b>Please submit the form will contact you within 24 hours. :</b><div style='font-size:15px;width:100%;'><label style='color:red; display:inline;'>*</label> Indicates required fields</div>";
            }

            Session["Captcha"] = DrawCaptcha();
            var PlainText = Session["Captcha"].ToString();
            var EncryCaptcha = ExcellentMarketResearch.Areas.Admin.Models.Common.Encrypt(PlainText);
            Enquiredata.RealCaptcha = EncryCaptcha;
            Enquiredata.ReportId = ReportID ?? 0;
            Enquiredata.FormType = FormType;

            return PartialView(Enquiredata);

        }

        public ActionResult SampleRequest(int? ReportID)
        {
            InquiryVM Enquiredata = new InquiryVM();

            if (ReportID > 0)
            {
                var Reports = (from l in db.ReportMasters
                               where l.ReportId == ReportID
                               select new
                               {
                                   l.ReportUrl,
                                   l.ReportTitle
                               }
                               ).FirstOrDefault();

                Enquiredata.ReportTitle = Reports.ReportTitle.ToString();
                Enquiredata.ReportUrl = Reports.ReportUrl.ToString();
            }
            else
            {
                Enquiredata.ReportTitle = "!";
                Enquiredata.ReportUrl = "!";

            }
            //if (FormType == "SampleRequestForm")
            //{
            //    ViewBag.FormTitle = "<b>Please fill your details below, to receive sample report:</b><div style='font-size:15px;width:100%;'><label style='color:red; display:inline;'>*</label> Indicates required fields</div>";
            //}
            //else
            //{
            //    ViewBag.FormTitle = "<b>Please submit the below form, to get more about this report:</b><div style='font-size:15px;width:100%;'><label style='color:red; display:inline;'>*</label> Indicates required fields</div>";
            //}
            ViewBag.FormTitle = "<b>Please submit the form will contact you within 24 hours. :</b><div style='font-size:15px;width:100%;'><label style='color:red; display:inline;'>*</label> Indicates required fields</div>";
            Session["Captcha"] = DrawCaptcha();
            var PlainText = Session["Captcha"].ToString();
            var EncryCaptcha = ExcellentMarketResearch.Areas.Admin.Models.Common.Encrypt(PlainText);
            Enquiredata.RealCaptcha = EncryCaptcha;
            Enquiredata.ReportId = ReportID ?? 0;
            Enquiredata.FormType = "SampleRequestForm";

            return PartialView(Enquiredata);
        }
        public ActionResult CaptchaValidation(string inputcapcha, string RealCaptcha)
        {
            //   HttpCookie reqCookies = Request.Cookies["CaptchaInfo"];

            var realcaptcha = ExcellentMarketResearch.Areas.Admin.Models.Common.Decrypt(RealCaptcha);

            if (realcaptcha == inputcapcha)
                return Json(true);
            else
                return Json(false);

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

        [HttpPost]
        public ActionResult InquiryForm(InquiryVM eq)
        {

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
            // ViewBag.Message = status ? "Google recaptcha validation success" : "Google recaptcha validation fails";
            CustomerInquiry e = new CustomerInquiry();
            var Formname = string.Empty;
            int FormTypeId;
            string Publisher = string.Empty;
            if (catptchastatus == true)
            {
                if (ModelState.IsValid)
                {
                    // cap = Session["Captcha"].ToString();
                    cap = ExcellentMarketResearch.Areas.Admin.Models.Common.Decrypt(eq.RealCaptcha);

                    if (eq.ReportId > 0)
                    {
                        var Publish = (from l in db.ReportMasters
                                       join p in db.PublisherMasters on l.PublishereId equals p.PublisherId
                                       where l.ReportId == eq.ReportId
                                       select p).FirstOrDefault();
                        Publisher = Publish.PublisherName;
                    }
                    else
                    {
                        Publisher = "!";
                    }

                    //if (eq.CaptchaCode == cap)
                    //{
                    CustomerInquiry cst = new CustomerInquiry();
                    var IpAddress = ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress();
                    cst.Company = eq.Company;
                    cst.Country = eq.Country;
                    cst.CustomerMessage = eq.CustomerMessage;
                    cst.Designation = eq.Designation;
                    cst.EmailId = eq.EmailId;
                    cst.Name = eq.Name;
                    eq.AreaCode += "-" + eq.PhoneNumber;
                    cst.PhoneNumber = eq.AreaCode;
                    cst.ReportId = eq.ReportId;
                    cst.CaptchaCode = eq.CaptchaCode;
                    cst.FormType = eq.FormType;
                    cst.CaptchaCode = "121321";
                    if (eq.FormType == "InquiryForm")
                    {
                        Formname = "Inquiry";
                        FormTypeId = 1;
                    }
                    else if (eq.FormType == "SampleRequestForm")
                    {
                        Formname = "Request Sample";
                        FormTypeId = 2;
                    }
                    else if (eq.FormType == "ContactUs")
                    {
                        FormTypeId = 4;
                    }
                    else
                    {
                        // Checkout page

                        FormTypeId = 3;
                    }

                    try
                    {
                        db.CustomerInquiries.Add(cst);
                        //db.Entry(cst).State = EntityState.Added;
                        db.SaveChanges();

                        // QYGroupRepository.PaymentGateway.Emailsending objEmailsending = new QYGroupRepository.PaymentGateway.Emailsending();
                        // string ipAddress = QYGroupRepository.PaymentGateway.IPAddress.GetIPAddress();
                        // Task.Run(() => new CRMWebService.WebServiceSoapClient().InsertUpdateKey(0, eq.ReportId, eq.ReportTitle, FormTypeId, 34, 1, 1, eq.ReportUrl, ipAddress, eq.Name, eq.EmailId, eq.AreaCode, eq.Company, eq.Designation, "!", "!", eq.Country, "!", eq.CustomerMessage, 1, "!", "!", "!", Publisher, 38, "BW&Zk^HfZ44P339nEzqrrawY4HL_VXw-5f+%8b4Hdw?$?m$G*!+kCGLK%3JjDn-74NY*LyhdJr6RAte&8MBWy6F2j82+qn7ap&DB@z-*q3sdH*#D-kwACucyaM7vzet4pSa?m^xnP@3zN5K9=*L6WLpDurTSuVTR3Hd&3XLHJnCcR!h*dL#fQhp^*#25LEFrMTt@z&8RWdf^CQcj!QrQU^WkdC5$Ub$8qnu!g7?*$$4%%M9?8spAugyCzZg5@dLGBNS_^7?x3VczR75J&=+9yFDVg*Qpd@R^_Jz-GtWgHxv4Kf$=2pxT@bqhx%aqgzZAN6RzZZ%rNX7km3fu$h?Z=+V3b_MQPLAxJBVT!=Ta+7Xd?CF3#4w44L@HU%nf4m#y-d2vgn6Gp2t7w!qFY%kN#y6DNAy#TbrZnqnjMtgeAd%BHSm9H29z4G_?qnBHE5J2EyutZ2RSh?P2fUE-sF8bNFdre@G^qQ??JzJuDCT3hby2py#+yfg*jC%&YBkrutHs"));

                        //Auto Mailer
                        Task.Run(() => objEmailsending.SendEmail("sales@excellentmarketresearch.com", "Sales", cst.EmailId, "", "balasahebpatil1612@gmail.com", "excellentmarketresearch.com : " + eq.ReportTitle, objEmailsending.GenerateMailBody_RequestSample_AutoReply(cst.Name, eq.ReportTitle)));

                        //To company
                        // Task.Run(() => objEmailsending.SendEmail("sales@excellentmarketresearcg.com", "Sales", "balasahebpatil1612@gmail.com", "", "", "ExcellentMarketResearch.com" + " : " + Formname,objEmailsending.GenerateMailBody_RequestSample(eq.ReportTitle, cst.Name, cst.EmailId, cst.PhoneNumber, cst.Company, cst.Country, cst.Designation, cst.CustomerMessage)));
                        Task.Run(() => objEmailsending.SendEmail("sales@excellentmarketresearch.com", "Sales", "sales@excellentmarketresearch.com", "", "", "excellentMarketResearch.com" + " : " + Formname, objEmailsending.GenerateMailBody_RequestSample(eq.ReportTitle, cst.Name, cst.EmailId, cst.PhoneNumber, cst.Company, cst.Country, cst.Designation, cst.CustomerMessage, IpAddress)));

                        Session["Name"] = cst.Name;

                        return RedirectToAction("Index", "InquiryForm", new { reporrtid = cst.ReportId });

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
                    //}
                    //Return the if model not valid
                    return View(eq);
                }
                return View(eq);
            }
            return View(eq);
        }

        public FileResult GetCaptchaImage(string EncryText)
        {

            //    Session["Captcha"] = GetRandomText();
            //Session["Captcha"] = DrawCaptcha();
            //text = Session["Captcha"].ToString();
            text = ExcellentMarketResearch.Areas.Admin.Models.Common.Decrypt(EncryText);
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            Font font = new Font("Arial", 15);
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20);

            drawing = Graphics.FromImage(img);

            //Color backColor = Color.Tomato;
            Color textColor = Color.White;
            //paint the background
            //drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 20, 10);

            drawing.Save();

            font.Dispose();
            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            img.Dispose();

            return File(ms.ToArray(), "image/png");

        }

        //public string GenerateMailBody_RequestSample(string reporttitle, string name, string emailid, string contactno, string nameofcompany, string countryname, string designation, string customermessage, string ipaddress)
        //{
        //    string result = "";
        //    result = "dear admin,<br /><br />" + "<table>";
        //    result += reporttitle != "" ? "<tr> <td valign='top' width='30%'><b>report title</b></td>   <td valign='top' width='2%'><b> : </b></td> <td valign='top' width='68%'>" + reporttitle + "</td> </tr>" : "";
        //    result += name != "" ? "<tr> <td valign='top'><b>customer name</b></td>  <td valign='top'><b> : </b></td> <td valign='top'>" + name + "</td> </tr>" : "";
        //    result += emailid != "" ? "<tr> <td valign='top'><b>email id</b></td>       <td valign='top'><b> : </b></td> <td valign='top'>" + emailid + "</td> </tr>" : "";
        //    result += contactno != "" ? "<tr> <td valign='top'><b>phone</b></td>          <td valign='top'><b> : </b></td> <td valign='top'>" + contactno + "</td> </tr>" : "";
        //    result += nameofcompany != "" ? "<tr> <td valign='top'><b>company name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + nameofcompany + "</td> </tr>" : "";
        //    result += designation != "" ? "<tr> <td valign='top'><b>designation</b></td>    <td valign='top'><b> : </b></td> <td valign='top'>" + designation + "</td> </tr>" : "";
        //    result += countryname != "" ? "<tr> <td valign='top'><b>country name</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + countryname + "</td> </tr>" : "";
        //    result += customermessage != "" ? "<tr> <td valign='top'><b>enquiry text</b></td>   <td valign='top'><b> : </b></td> <td valign='top'>" + customermessage + "</td> </tr>" : "";
        //    result += "<tr> <td valign='top'><b>publisher</b></td>      <td valign='top'><b> : </b></td> <td valign='top'>" + "Excellent Market Research " + "</td> </tr>";
        //    result += "<tr> <td valign='top'><b>ip address</b></td>     <td valign='top'><b> : </b></td> <td valign='top'>" + ipaddress == null ? ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress() : ipaddress + "</td> </tr>";
        //    result += "</table>";
        //    return result;
        //}


        //public string GenerateMailBody_RequestSample_AutoReply(string Name, string ReportTitle)
        //{
        //    string result = "";
        //    if (ReportTitle == "")
        //    {
        //        result = "Dear " + Name + ","
        //             + "<br /><br />Thank you for your interest in <b>" + "ExcellentMarketResearch.com" + "</b>."
        //            //+ "<br /><br />For your reference please find the below link."
        //            //+ "<br /><br />" + "QYGroup.biz"
        //            //+ "<br /><br />I'll contact you soon to serve your research needs."
        //             + "<br /><br />We'll contact you soon to serve your research needs."
        //             + "<b><br /><br />Warm regards,"
        //             + "<br />Miler Jhon | Corporate Sales Specialist,USA"
        //             + "<br />Direct line: + 1888-868686-68686#"
        //             + "<br />" + "excellentmarketresearch.com"
        //             + "<br />E-mail: sales@excellentmarketresearch.com | Web: " + "excellentmarketresearch.com" + "</b>";
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
        //            + "<br />Miler Jhon | Corporate Sales Specialist,USA"
        //            + "<br />Direct line: +  1888-868686-68686#"
        //            + "<br />" + "excellentmarketresearch.com"
        //            + "<br />E-mail: sales@excellentmarketresearch.com  | Web: " + "excellentmarketresearch.com" + "</b>";
        //    }
        //    return result;
        //}

        public ActionResult GetCountryCode(string countryname)
        {
            var countrycode = db.Countries.Where(x => x.nicename == countryname).Select(x => x.phonecode).FirstOrDefault();
            return Json(countrycode);
        }
    }
}

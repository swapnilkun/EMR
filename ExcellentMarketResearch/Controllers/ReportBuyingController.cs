﻿using ExcellentMarketResearch.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExcellentMarketResearch.Models;
using ExcellentMarketResearch.Models.PaymentGateway;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Text;
using PayPal;
using PayPal.Api;

namespace ExcellentMarketResearch.Controllers
{
    public class ReportBuyingController : Controller
    {
        //
        // GET: /ReportBuying/

        ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

        //  Emailsending objEmailsending = new Emailsending();

        string generatedtext = string.Empty;
        string realCaptcha = string.Empty;
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ValidateCaptcha(string inputcapcha)
        {
            string z = Session["Captchacode"].ToString();

            return Json(z == inputcapcha);

        }

        [HttpGet]
        public ActionResult CheckoutForm(int reportid, string buynow)
        {

            if (reportid.ToString() != null && (!string.IsNullOrEmpty(buynow)))
            {

                BuyingVM ObjBuy = new BuyingVM();


                ObjBuy.ReportId = reportid;

                var report = (from l in db.ReportMasters
                              where l.ReportId == reportid
                              select l).FirstOrDefault();
                ObjBuy.ReportTitle = report.ReportTitle;
                ObjBuy.ReportUrl = report.ReportUrl;
                ObjBuy.Type = buynow == "0" ? "Single User" : buynow == "1" ? "Multi User" : "Corporate User";
                ObjBuy.Price = (decimal)(buynow == "0" ? report.SinglePrice : (buynow == "1" ? report.MultiUserPrice : report.CorporateUserPrice));

                return View(ObjBuy);
            }
            else
                return RedirectToAction("ReportDetail", "ReportsCollection");
        }


        // POST: /EnquiryForm/Create

        [HttpPost]
        public ActionResult CheckoutForm(BuyingVM ObjBuy)
        {
            //  var MailbodyMethodCall = new QYGroupRepository.PaymentGateway.Emailsending();

            if (ModelState.IsValid)
            {
                string realCaptcha = Session["Captchacode"].ToString();
                string Publisher = string.Empty;
                if (ObjBuy.ReportId > 0)
                {
                    var publish = (from l in db.ReportMasters
                                   join p in db.PublisherMasters on l.PublishereId equals p.PublisherId
                                   where l.ReportId == ObjBuy.ReportId
                                   select p).FirstOrDefault();
                    Publisher = publish.PublisherName;
                }
                if (ObjBuy.CaptchaCode == realCaptcha)
                {
                    //JavaScriptSerializer serializer = new JavaScriptSerializer();
                    //var s = serializer.Serialize(Enquiredata);
                    //BuyingInfo cst = serializer.Deserialize<BuyingInfo>(s);
                    //CategoryMaster categorymaster = serializer.Deserialize<CategoryMaster>(s);

                    ObjBuy.GuId = System.Guid.NewGuid().ToString();
                    ObjBuy.IPAddress = ExcellentMarketResearch.Models.PaymentGateway.IPAddress.GetIPAddress();


                    BuyingInfo binfo = new BuyingInfo();

                    binfo.Address = ObjBuy.Address;
                    binfo.Name = ObjBuy.Name;
                    binfo.AreaCode = ObjBuy.AreaCode;
                    binfo.CustomerMessage = ObjBuy.CustomerMessage;
                    binfo.ReportTitle = ObjBuy.ReportTitle;
                    binfo.ReportUrl = ObjBuy.ReportUrl;
                    binfo.EmailId = ObjBuy.EmailId;
                    binfo.Company = ObjBuy.Company;
                    binfo.CaptchaCode = ObjBuy.CaptchaCode;
                    binfo.Country = ObjBuy.Country;
                    binfo.Designation = ObjBuy.Designation;
                    binfo.State = ObjBuy.State;
                    binfo.City = ObjBuy.City;
                    binfo.Price = ObjBuy.Price;
                    binfo.PhoneNumber = ObjBuy.PhoneNumber;
                    binfo.Type = ObjBuy.Type;
                    binfo.ReportId = ObjBuy.ReportId;
                    binfo.Zipcode = ObjBuy.Zipcode;
                    binfo.Paymentmode = ObjBuy.Paymentmode;
                    binfo.GuId = ObjBuy.GuId;
                    binfo.IPAddress = ObjBuy.IPAddress;
                    try
                    {
                        db.BuyingInfoes.Add(binfo);
                        //  db.Entry(cst).State = EntityState.Added;
                        db.SaveChanges();

                        //The lead information goes to CRM here......
                        //  new CRMWebService.WebServiceSoapClient().InsertUpdateKey(0, ObjBuy.ReportId, ObjBuy.ReportTitle, 3, 34, 1, 1, ObjBuy.ReportUrl, QYGroupRepository.PaymentGateway.IPAddress.GetIPAddress(), ObjBuy.Name, ObjBuy.EmailId, ObjBuy.AreaCode, ObjBuy.Company, ObjBuy.Designation, "!", ObjBuy.State, ObjBuy.Country, ObjBuy.Zipcode, ObjBuy.CustomerMessage, 1, ObjBuy.GuId, "!", "!", Publisher, 38, "BW&Zk^HfZ44P339nEzqrrawY4HL_VXw-5f+%8b4Hdw?$?m$G*!+kCGLK%3JjDn-74NY*LyhdJr6RAte&8MBWy6F2j82+qn7ap&DB@z-*q3sdH*#D-kwACucyaM7vzet4pSa?m^xnP@3zN5K9=*L6WLpDurTSuVTR3Hd&3XLHJnCcR!h*dL#fQhp^*#25LEFrMTt@z&8RWdf^CQcj!QrQU^WkdC5$Ub$8qnu!g7?*$$4%%M9?8spAugyCzZg5@dLGBNS_^7?x3VczR75J&=+9yFDVg*Qpd@R^_Jz-GtWgHxv4Kf$=2pxT@bqhx%aqgzZAN6RzZZ%rNX7km3fu$h?Z=+V3b_MQPLAxJBVT!=Ta+7Xd?CF3#4w44L@HU%nf4m#y-d2vgn6Gp2t7w!qFY%kN#y6DNAy#TbrZnqnjMtgeAd%BHSm9H29z4G_?qnBHE5J2EyutZ2RSh?P2fUE-sF8bNFdre@G^qQ??JzJuDCT3hby2py#+yfg*jC%&YBkrutHs");

                        if (ObjBuy.Paymentmode == "wireTransfer")
                        {
                            //Auto Mailer
                            //  objEmailsending.SendEmail("sales@xyz.com", "Sales", ObjBuy.EmailId, "", "naveen.p@xyz.com", "Payment Initiated : " + ObjBuy.ReportTitle, MailbodyMethodCall.GenerateMailBody_PaymentInitiated_AutoReply(ObjBuy.ReportTitle, ObjBuy.Name));
                            //To company
                            //                            objEmailsending.SendEmail(ObjBuy.EmailId, ObjBuy.Name, "payments@xyz.com", "", "md@xyz.com,naveen.p@xyz.com", "xyz.com" + " : Payment Initiated(Wire Transfer)", MailbodyMethodCall.GenerateMailBody_PaymentInitiated(ObjBuy.ReportTitle, ObjBuy.Name, ObjBuy.EmailId, ObjBuy.PhoneNumber, ObjBuy.Company, "", "", ObjBuy.Country, ""));


                        }
                        else
                        {
                            //Auto Mailer
                            //                          objEmailsending.SendEmail("sales@marketresearchtrade.com", "Sales", ObjBuy.EmailId, "", "balasaheb.p@marketresearchstore.com", "Payment Initiated : " + ObjBuy.ReportTitle, GenerateMailBody_PaymentInitiated_AutoReply(ObjBuy.ReportTitle, ObjBuy.Name));

                            //To company
                            //                        objEmailsending.SendEmail(ObjBuy.EmailId, ObjBuy.Name, "payments@xyx.com", "", "md@xyz.com,naveen.p@xyz.com", "xyz.com" + " : Payment Initiated(pay pal )", GenerateMailBody_PaymentInitiated(ObjBuy.ReportTitle, ObjBuy.Name, ObjBuy.EmailId, ObjBuy.PhoneNumber, ObjBuy.Company, "", "", ObjBuy.Country, ""));



                            //The paypalpage will appear to the user or buer ....
                            //Paypal._PayPal(ObjBuy);

                            PaymentWithPaypal(ObjBuy, null);
                            //return RedirectToAction("PaymentWithPaypal", "ReportBuying", new {ObjBuy, Cancel = false });
                            //return RedirectToAction("PaymentWithPaypal");

                        }



                        //  return RedirectToAction("Index");

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
                else
                    ModelState.AddModelError("", "Verification Code is Incorrect");

            }
            Session["Name"] = ObjBuy.Name;
            return RedirectToAction("Index", "InquiryForm", new { reportid = ObjBuy.ReportId });
        }

        public string GetCaptcha()
        {
            StringBuilder randomText = new StringBuilder();

            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyz";

            Random r = new Random();

            for (int j = 0; j <= 5; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            //Session["Captchacode"] = randomText.ToString();

            return randomText.ToString();
        }

        //public ActionResult PayPalProcess(PaymentLibrary.PayPal.PayPalResponse response)
        //{
        //    var Mailbody = new QYGroupRepository.PaymentGateway.Emailsending();
        //    string PaymentStatus = string.Empty;
        //    BuyingVM buy = new BuyingVM();
        //    var userdata = (from b in db.BuyingInfoes
        //                    where b.GuId == response.guid
        //                    select b).FirstOrDefault();
        //    var userReport = (from r in db.ReportMasters
        //                      where r.ReportId == userdata.ReportId
        //                      select new { r.ReportTitle, r.ReportURL }).FirstOrDefault();

        //    buy.Name = userdata.Name;
        //    buy.ReportTitle = userReport.ReportTitle.ToString();
        //    buy.ReportUrl = userReport.ReportURL.ToString();
        //    buy.Company = userdata.Company;
        //    buy.EmailId = userdata.EmailId;
        //    buy.Country = userdata.Country;
        //    buy.IPAddress = userdata.IpAddress;
        //    buy.PhoneNumber = userdata.PhoneNumber;
        //    buy.Designation = userdata.Designation;


        //    if (Paypal.PayPalProcess(response))
        //    {


        //        PaymentStatus = "Dear Admin, Payment made for <br /><br />";

        //        //Auto Mailer
        //        objEmailsending.SendEmail("sales@marketresearchtrade.com", "Sales", userdata.EmailId, "", "balasaheb.p@marketresearchstore.com", "Market Research Trade :Payment Confirmation ", Mailbody.GenerateMailBody_RequestSample(PaymentStatus, buy));

        //        //To company
        //        objEmailsending.SendEmail(userdata.EmailId, userdata.Name, "payments@marketresearchstore.com", "", "balasaheb.p@marketresearchstore.com", "Market Research Trade " + " :Payment Confirmation(Pay Pal) ", new QYGroupRepository.PaymentGateway.Emailsending().GenerateMailBody_RequestSample_AutoReply(userdata.Name.ToString(), userReport.ReportTitle.ToString()));


        //        return RedirectToAction("Sucess", "Paymentprocess");
        //    }
        //    else
        //    {
        //        PaymentStatus = "Dear Admin, Payment canceled or unapproved for report<br /><br />";


        //        //To Buyer
        //        objEmailsending.SendEmail("sales@marketresearchtrade.com", "Sales", userdata.EmailId, "", "balasaheb.p@marketresearchstore.com", "Market Research Trade : " + " : Payment Cancel(Pay Pal)", GenerateMailBody_PaypalError_AutoReply(buy.Name, buy.ReportTitle, buy.ReportUrl));

        //        //To company
        //        objEmailsending.SendEmail(userdata.EmailId, userdata.Name, "payments@marketresearchstore.com", "", "balasaheb.p@marketresearchstore.com", "Market Research Trade" + " : Payment Cancel(Pay Pal)", new QYGroupRepository.PaymentGateway.Emailsending().GenerateMailBody_PaypalError_AutoReply(userdata.Name.ToString(), userReport.ReportTitle.ToString(), userReport.ReportURL.ToString()));

        //        return RedirectToAction("Failure", "Paymentprocess");
        //    }

        //}

        public FileResult CaptchaImage()
        {
            Session["Captchacode"] = GetCaptcha();

            generatedtext = Session["Captchacode"].ToString();

            //first, create a dummy bitmap just to get a graphics object
            System.Drawing.Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            Font font = new Font("Arial", 15);
            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(generatedtext, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20);

            drawing = Graphics.FromImage(img);

            //Color backColor = Color.SeaShell;
            Color backColor = Color.White;
            Color textColor = Color.Red;
            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(generatedtext, font, textBrush, 20, 10);

            drawing.Save();

            font.Dispose();
            textBrush.Dispose();
            drawing.Dispose();

            MemoryStream ms = new MemoryStream();
            img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            img.Dispose();

            return File(ms.ToArray(), "image/png");

        }


        private string GenerateMailBody_PaymentInitiated(string ReportTitle, string Name, string EmailID, string ContactNo, string NameOfCompany, string Address, string State, string CountryName, string ZipCode)
        {
            string result = "";
            result = "Dear Admin, </br> Payment for report <b> " + ReportTitle + " </b> <br/>"
                    + "<table> <tr><td> <b> Customer Name - </b> </td><td> " + Name + " </td></tr>"
                        + "<tr><td><b> Email Id -</b> </td><td>" + EmailID + "</td></tr>"
                        + "<tr><td><b> Phone -</b></td><td> " + ContactNo + " </td></tr>"
                        + "<tr><td><b> Name of company -</b></td><td> " + NameOfCompany + " </td></tr>"
                        + "<tr><td><b>Address</b></td><td>" + Address + "</td></tr>"
                        + "<tr><td><b>State</b></td><td>" + State + "</td></tr>"
                        + "<tr><td><b>Country Name -</b></td><td>" + CountryName + " </td></tr>"
                        + "<tr><td>Zip Code<b></b></td><td>" + ZipCode + "</td></tr>"
                        + "<tr><td><b>IP Address -</b> </td><td>" + "QYGroupRepository.PaymentGateway.IPAddress.GetIPAddress()" + "</td></tr>"
                    + "</table>";
            return result;
        }

        private string GenerateMailBody_PaymentInitiated_AutoReply(string ReportTitle, string Name)
        {
            string result = "";
            result = "Dear " + Name + ","
                + "<br/><br/>Thank you for your interest in our research report " + ReportTitle + "."
                + "<br />Let us know in any problem while payment."
                + "<br/><br/>Thanks";
            return result;
        }

        private string GenerateMailBody_PaypalError_AutoReply(string Name, string ReportTitle, string ReportURL)
        {
            string result = "";

            result = "Dear " + Name + ","
                + "<br /><br />You canceled payment for report,"
                + "<br /><b>" + ReportTitle + "</b>."
                + "<br />" + "https://www.excellentmarketresearch.com/report/" + ReportURL
                + "<br /><br />Did you experienced problem in our service?"
                + "<br /><br />Let us know."
                + "<b><br /><br />Warm regards,"
                + "<br />Joel John | Corporate Sales Specialist,USA"
                + "<br />Direct line: + 1-855-465-4651"
                + "<br />excellentmarketresearch"
                + "<br />E-mail: sales@excellentmarketresearch.biz | Web: " + "https://www.excellentmarketresearch.com" + "</b>"
                + "<br /><br />Thanks,"
                + "<br />excellentmarketresearch.com";

            return result;
        }

        public ActionResult PaymentWithPaypal(BuyingVM buyingVM, string Cancel = null)
        {
            //getting the apiContext  
            PayPal.Api.APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/ReportBuying/PaymentSuccess?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + buyingVM.GuId, buyingVM);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    Response.Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return View("SuccessView");
        }

        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(PayPal.Api.APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(PayPal.Api.APIContext apiContext, string redirectUrl, BuyingVM buyingVM)
        {
            //create itemlist and add item objects to it 

            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            // Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = buyingVM.ReportTitle.ToString(),// buyingVM.ReportTitle,
                currency = "USD",
                price = buyingVM.Price.ToString(),//(buyingVM.Price).ToString(),
                quantity = "1",
                sku = "sku"
            });
            var transaction = new Transaction()
            {
                amount = new Amount()
                {
                    currency = "USD",
                    total = buyingVM.Price.ToString()

                },
                item_list = itemList,

                description = "This is the payment transaction description.",

                invoice_number = GetRandomInvoiceNumber()
            };

            var payer = new Payer()
            {
                payment_method = "paypal"
            };


            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };


            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = new List<Transaction>() { transaction },
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);

            /*
            //create itemlist and add item objects to it  
            //var pricedemo = 10;
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            // Adding Item Details like name, currency, price etc
            itemList.items.Add(new Item()
            {
                name = "Global 3-Methyl-1",// buyingVM.ReportTitle,
                currency = "USD",
                price = "1",//(buyingVM.Price).ToString(),
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "1",
                shipping = "1",
                subtotal = "1"
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total ="1",// pricedemo.ToString(),//buyingVM.Price.ToString(), // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            transactionList.Add(new Transaction()
            {
                description = "Transaction description",
                invoice_number = "your generated invoice number", //Generate an Invoice No  
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
            */
        }
        public static string GetRandomInvoiceNumber()
        {
            return new Random().Next(999999).ToString();
        }

        public ActionResult PaymentSuccess(string responce)
        {

            string querystring = string.Empty;
            foreach (var q in Request.QueryString.AllKeys)
            {
                querystring += q + "=" + Request.QueryString[q] + "&";

            }
         
            string paypalGuid = Request.QueryString["guid"];
            string paymentId = Request.QueryString["paymentId"];
            string token = Request.QueryString["token"];
            string PayerID = Request.QueryString["PayerID"];
            string Cancel = Request.QueryString["cancel"];
            bool paymentcancel = false;

            if (IsValidUser(paypalGuid))
            {

            }

            if (Cancel == "true")
            {
                paymentcancel = true;
            }
            else
            {
                paymentcancel = false;
            }

            return View();
        }
        public bool IsValidUser(string paypalGuid)
        {
            string valid = db.BuyingInfoes.Where(x => x.GuId == paypalGuid).Select(x => x.Name).FirstOrDefault();
            if (!string.IsNullOrEmpty(valid))
            {
                return true;
            }
            else
                return false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using ExcellentMarketResearch.Models.ViewModel;
using PaymentLibrary;
using PaymentLibrary.Common;
using PaymentLibrary.PayPal;

namespace ExcellentMarketResearch.Models.PaymentGateway
{
    public class Paypal
    {
        public static void _PayPal(BuyingVM buynow)
        {

            PayPalConfig config = PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("~/paypalconfig/paypal.config"));
            config.guid = buynow.GuId;
            List<PaymentLibrary.PayPal.Item> items = new List<PaymentLibrary.PayPal.Item>();
            //foreach (OrderSummary orderSummary in buynow.OrderSummary)
            //{
            //    //if price is not set or null then do not add to order summary
            //    if (orderSummary.Price == null)
            //        continue;

            //    PaymentLibrary.PayPal.Item itm = new PaymentLibrary.PayPal.Item();
            //    itm.Name = orderSummary.ReportTitle.ZSubstring(0, 20);
            //    itm.Quantity = orderSummary.Quantity;
            //    itm.Price = orderSummary.Price;
            //    items.Add(itm);
            //}

            items.Add(new Item
            {
                Name = buynow.ReportTitle.Length > 20 ? buynow.ReportTitle.Substring(0, 20) : buynow.ReportTitle,
                Quantity = 1,
                Price = buynow.Price
            });

            // PaymentLibrary.PayPal.Token tkn = PaymentLibrary.PayPal.PayPal.GetToken(config, items, true);
            PaymentLibrary.PayPal.Token tkn = GetToken(config, items, true);

            if (!string.IsNullOrEmpty(tkn.L_ERRORCODE0))
            {
                log4net.LogManager.GetLogger("Error").Error("Error at SaveBuynowDetails. PayPal \nErrorCode - " + tkn.L_ERRORCODE0 + "\nError Message - " + tkn.L_SHORTMESSAGE0 + "\n" + tkn.L_LONGMESSAGE0 + "\nFull Data - " + Newtonsoft.Json.JsonConvert.SerializeObject(buynow));
                //return new Utility.Message { MessageText = tkn.L_LONGMESSAGE0 };

            }

            PaymentLibrary.PayPal.PayPal.RedirectUser(tkn);

            // return new Utility.Message { MessageText = "Payment Initiated" };

        }
        public static Token GetToken(PayPalConfig cnf, List<Item> items, bool isSandbox = false)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(!isSandbox ? "https://api-3t.paypal.com/nvp?" : "https://api-3t.sandbox.paypal.com/nvp?");
            PayPalRequest request = new PayPalRequest
            {
                METHOD = "SetExpressCheckout",
                VERSION = !isSandbox ? "109.0" : "104.0",
                USER = cnf.User,
                PWD = cnf.Password,
                SIGNATURE = cnf.Signature,
                Items = new Dictionary<string, object>()
            };
            int num = 0;
            foreach (Item item in items)
            {
                request.Items.Add("L_PAYMENTREQUEST_0_NAME" + num, item.Name);
                request.Items.Add("L_PAYMENTREQUEST_0_NUMBER" + num, num);
                request.Items.Add("L_PAYMENTREQUEST_0_QTY" + num, item.Quantity);
                request.Items.Add("L_PAYMENTREQUEST_0_AMT" + num, item.TotalAmount);
                num++;
            }
            request.PAYMENTREQUEST_0_AMT = items.Sum<Item>(x => x.TotalAmount);
            request.PAYMENTREQUEST_0_CURRENCYCODE = "USD";
            request.RETURNURL = cnf.ReturnUrl + "?guid=" + cnf.guid;
            request.CANCELURL = cnf.CancelUrl + "?guid=" + cnf.guid;
            request.PAYMENTREQUEST_0_PAYMENTACTION = "Sale";
            request.SOLUTIONTYPE = "Sole";
            request.LANDINGPAGE = "Billing";
            request.BRANDNAME = cnf.CompanyName;
            builder.Append(QuerystringSerializer.Serialize(request));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebResponse response = WebRequest.Create(builder.ToString()).GetResponse();
            StreamReader reader1 = new StreamReader(response.GetResponseStream());
            string s = reader1.ReadToEnd();
            reader1.Close();
            response.Close();
            return QuerystringSerializer.Deserialize<Token>(HttpContext.Current.Server.UrlDecode(s), "", false);
        }

        public static bool PayPalProcess(PaymentLibrary.PayPal.PayPalResponse paypalResponse)
        {
            ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

            if (paypalResponse != null && (!string.IsNullOrEmpty(paypalResponse.PAYERID) || !string.IsNullOrEmpty(paypalResponse.guid)))
            {
                BuyingInfo b = new BuyingInfo();

                if (string.IsNullOrEmpty(paypalResponse.PAYERID))
                    log4net.LogManager.GetLogger("Error").Error("PayerID not found OR Response is null OR guid is not found.\nData - " + Newtonsoft.Json.JsonConvert.SerializeObject(paypalResponse));

                //TODO: Get buyer from table using guid

                var buyer = GetBuyerByGuId(paypalResponse.guid);

                //bool IsBuyerExist= buyer.Count(x=>x.GuId==paypalResponse.guid)>0?true:false;
                bool IsBuyerExist = buyer != null ? true : false;

                //TODO: Check buyer if exist or not
                if (IsBuyerExist == null)
                {
                    // log4net.logmanager.getlogger("error").error("buyer not found.\ndata - " + newtonsoft.json.jsonconvert.serializeobject(paypalresponse));
                    return false;
                }


                ValidResponse vResponse = PaymentLibrary.PayPal.PayPal.IsPaymentValid(paypalResponse,
                    PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/paypalconfig/paypal.config"), false), false);

                if (vResponse.IsValid)
                {
                    //TODO: update status of payment transaction to success

                    var updatestatus = db.BuyingInfoes.Where(x => x.GuId == paypalResponse.guid).FirstOrDefault();
                    b.PaymentTransaction = true;
                    updatestatus.PaymentTransaction = b.PaymentTransaction;
                    db.Entry(updatestatus).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
                Stream s = new MemoryStream();
                TextWriter t = new StreamWriter(s);

                serializer.Serialize(t, paypalResponse);
                TextReader r = new StreamReader(s);

                //TODO: Save error to db

                //_saveStatus(, 'f', vResponse.Reason + "|ErrorCode - " + vResponse.ErrorCode + "|PaypalResponse - " + r.ReadToEnd());

                var saveError = db.BuyingInfoes.Where(x => x.GuId == paypalResponse.guid).FirstOrDefault();
                b.PaymentTransaction = false;
                saveError.PaymentTransaction = b.PaymentTransaction;
                saveError.ErrorReason = vResponse.Reason;
                saveError.ErrorCode = vResponse.ErrorCode;
                db.Entry(saveError).State = EntityState.Modified;
                db.SaveChanges();

                return false;
            }
            return false;
        }

        public static BuyingInfo GetBuyerByGuId(string guid)
        {

            ExcellentMarketResearchEntities db = new ExcellentMarketResearchEntities();

            //var buyer = db.BuyingInfoes.Where(x => x.GuId == guid).Select new BuyingInfo(


            //).FirstOrDefault();

            var buyer = (from l in db.BuyingInfoes
                         where l.GuId == guid
                         select l).FirstOrDefault();

            return buyer;
        }

    }

    public class PayPalResponse
    {
        public string PAYERID { get; set; }

        public string guid { get; set; }

        public string TOKEN { get; set; }

        public string ACK { get; set; }

        public string OrderID { get; set; }

        public string PaymentID { get; set; }

        public string Intent { get; set; }

        public string ReturnUrl { get; set; }

        public string PaymentStatus { get; set; }
    }
}
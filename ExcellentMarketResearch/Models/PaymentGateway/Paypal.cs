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

            PayPalConfig config = PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/paypalconfig/paypal.config"));
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

            //PaymentLibrary.PayPal.Token tkn = PaymentLibrary.PayPal.PayPal.GetToken(config, items);
            PaymentLibrary.PayPal.Token tkn = GetToken(config, items);

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
            StringBuilder stringBuilder = new StringBuilder();
            string value = (!isSandbox) ? "https://api-3t.paypal.com/nvp?" : "https://api-3t.sandbox.paypal.com/nvp?";
            stringBuilder.Append(value);
            PayPalRequest payPalRequest = new PayPalRequest();
            payPalRequest.METHOD = "SetExpressCheckout";
            payPalRequest.VERSION = ((!isSandbox) ? "109.0" : "104.0");
            payPalRequest.USER = cnf.User;
            payPalRequest.PWD = cnf.Password;
            payPalRequest.SIGNATURE = cnf.Signature;
            payPalRequest.Items = new Dictionary<string, object>();
            int num = 0;
            foreach (Item current in items)
            {
                payPalRequest.Items.Add("L_PAYMENTREQUEST_0_NAME" + num, current.Name);
                payPalRequest.Items.Add("L_PAYMENTREQUEST_0_NUMBER" + num, num);
                payPalRequest.Items.Add("L_PAYMENTREQUEST_0_QTY" + num, current.Quantity);
                payPalRequest.Items.Add("L_PAYMENTREQUEST_0_AMT" + num, current.TotalAmount);
                num++;
            }
            PayPalRequest arg_162_0 = payPalRequest;
            //Func<Item, decimal?> arg_15D_1;
            //if ((arg_15D_1 =PayPal.<>c.<>9__0_0) == null)
            //{
            //    arg_15D_1 = (PayPal.<>c.<>9__0_0 = new Func<Item, decimal?>(PayPal.<>c.<>9.<GetToken>b__0_0));
            //}
            //arg_162_0.PAYMENTREQUEST_0_AMT = items.Sum(arg_15D_1);
            payPalRequest.PAYMENTREQUEST_0_CURRENCYCODE = "USD";
            payPalRequest.RETURNURL = cnf.ReturnUrl + "?guid=" + cnf.guid;
            payPalRequest.CANCELURL = cnf.CancelUrl + "?guid=" + cnf.guid;
            payPalRequest.PAYMENTREQUEST_0_PAYMENTACTION = "Sale";
            payPalRequest.SOLUTIONTYPE = "Sole";
            payPalRequest.LANDINGPAGE = "Billing";
            payPalRequest.BRANDNAME = cnf.CompanyName;
            stringBuilder.Append(QuerystringSerializer.Serialize(payPalRequest));
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            WebResponse expr_1FE = WebRequest.Create(stringBuilder.ToString()).GetResponse();
            StreamReader expr_209 = new StreamReader(expr_1FE.GetResponseStream());
            string s = expr_209.ReadToEnd();
            expr_209.Close();
            expr_1FE.Close();
            return QuerystringSerializer.Deserialize<Token>(HttpContext.Current.Server.UrlDecode(s), "", false);
        }

        public static bool PayPalProcess(PayPalResponse paypalResponse)
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
}
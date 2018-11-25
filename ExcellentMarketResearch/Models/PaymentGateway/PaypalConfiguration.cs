using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PayPal;
using PayPal.Api;

namespace ExcellentMarketResearch.Models.PaymentGateway
{
    public static class PaypalConfiguration
    {
        //Variables for storing the clientID and clientSecret key  
        public readonly static string ClientId;
        public readonly static string ClientSecret;
        //Constructor  
        static PaypalConfiguration()
        {
            //PayPalConfig config = PayPalConfig.GetConfiguration(HttpContext.Current.Server.MapPath("/Paypal/paypal.config"));
            var config = GetConfig();
            ClientId = config["clientId"];
            ClientSecret = config["clientSecret"];
        }
        // getting properties from the web.config  
        public static Dictionary<string, string> GetConfig()
        {
            return PayPal.Api.ConfigManager.Instance.GetProperties();
        }
        private static string GetAccessToken()
        {
            // getting accesstocken from paypal  
            string accessToken = new OAuthTokenCredential(ClientId, ClientSecret, GetConfig()).GetAccessToken();
            return accessToken;
        }
        public static PayPal.Api.APIContext GetAPIContext()
        {
            // return apicontext object by invoking it with the accesstoken  
            PayPal.Api.APIContext apiContext = new PayPal.Api.APIContext(GetAccessToken());
            apiContext.Config = GetConfig();
            return apiContext;
        }
    }
}
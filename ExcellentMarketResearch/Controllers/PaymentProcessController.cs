using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExcellentMarketResearch.Controllers
{
    public class PaymentProcessController : Controller
    {
        //
        // GET: /PaymentProcess/

        public ActionResult Success()
        {
            return View();
        }
        public ActionResult Failure()
        {
            return View();
        }
        public ActionResult Cancel()
        {
            return View();
        }

    }
}

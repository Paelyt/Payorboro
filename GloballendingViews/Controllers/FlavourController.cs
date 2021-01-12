using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class FlavourController : Controller
    {
        // GET: Flavour
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

         [HttpGet]
        public ActionResult GetAllFlavour()
        {
            try {

                var obj = new Wrapper() { flavour = new List<string>() { "Vanilla", "red velvet", "rainbow", "carrot", "rainbow" },
                    topping = new List<string>() { "sprinkles", "sugar carrots", "bacon", "Happy Birthday" },
                    frosting = new List<string>() { "cream cheese", "chocolate", "vanilla", "maple" }
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult PriceCalculator(string size,string flavour,string topping,string frosting)
        {
            try
            {
                var total = 0;
                if (size == "large" && flavour == "rainbow" && topping == "sprinkles" && frosting == "vanilla")
                {
                    total = 17;
                }
                var obj = new wrapp()
                {

                    Total = total,
                };

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

    }

    public class Wrapper
    {
        [JsonProperty("tags")]
        public List<string> flavour { get; set; }

        public List<string> topping { get; set; }

        public List<string> frosting { get; set; }
    }

    public class wrapp
    {
        [JsonProperty("tags")]
        public double Total { get; set; }

       
    }
}
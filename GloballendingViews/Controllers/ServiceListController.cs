using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class ServiceListController : Controller
    {
        // GET: ServiceList
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ServiceList()
        {
            try
            {
                int user = 1027;
                ViewBag.ServiceList = (from a in db.CustomerServices
                                where a.Customer_FK == user
                               select a).ToList();
                
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult ServiceList(FormCollection form)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
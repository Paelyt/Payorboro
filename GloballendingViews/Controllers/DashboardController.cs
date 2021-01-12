using ClosedXML.Excel;
using DataAccess;
using GloballendingViews.Classes;
using GloballendingViews.HelperClasses;
using GloballendingViews.Models;
using GloballendingViews.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using Newtonsoft.Json.Linq;

namespace GloballendingViews.Controllers
{
    public class DashboardController : Controller
    {
        string user = "";
        string id = "";
        DataAccess.DataReaders DataReaders = new DataReaders();
        DataAccess.DataCreator DataCreators = new DataCreator();
        DataAccess.GlobalTransactEntitiesData gb = new GlobalTransactEntitiesData();
        Pag pages = new Pag();
        DataAccess.UserRole userroles = new DataAccess.UserRole();
        PageAuthentication _pa = new PageAuthentication();
        List<string> rol = new List<string>();
        DataAccess.DataReaders _dr = new DataAccess.DataReaders();
        public static int Userfk;
        static int Flag;
        // GET: Dashboard

        LoanViewModel lvm = new LoanViewModel();

        [HttpGet]
        public ActionResult Dashboard()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var books = new List<Paytv.Menus>();
                var mc = DataReaders.getUserID(appUser);
                // int user = 1027;
                ViewBag.ServiceList = (from a in gb.CustomerServices
                                       where a.Customer_FK == mc.id && a.isVissible == 1
                                       select a).ToList();
                ViewBag.MerchantServiceType = (from b in gb.MerchantServiceTypes select b).ToList();
                GetMenus();
                getTopicons();
                // For Bank Users 
                var bankFk = DataReaders.getBankByUser(mc.Email);
                if (bankFk.BankFk == null || bankFk.BankFk == 0)
                {
                    TempData["Flg"] = "0";
                    return View();
                }
                else if (bankFk.BankFk != null || bankFk.BankFk != 0)
                {
                    return RedirectToAction("DashboardSummary", "Dashboard");
                }

                //
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Dashboard(FormCollection form)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var books = new List<Paytv.Menus>();
                var mc = DataReaders.getUserID(appUser);

                int val = Convert.ToInt16(Request.QueryString["val"]);
                if (val != 0)
                {
                    ViewBag.ServicesListx = DataReaders.MerchantServiceList(val);
                    return Json(new
                    {
                        Success = "true",
                        Data = ViewBag.ServicesListx
                    });
                    //return Json(new { Success = "false" });

                }
                GetMenus();
                getTopicons();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult AddServiceList()
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


        public ActionResult EditService(int id)
        {
            try
            {
                if (id != 0)
                {
                    var Record = DataReaders.getServiceList(id);
                    Classes.Paytv.PaytvObj _pv = new Classes.Paytv.PaytvObj();

                    // _pv.CustomerID = 

                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public ActionResult DeleteService(int id)
        {
            try
            {
                CustomerServices _cs = new CustomerServices();
                if (id != 0)
                {
                    _cs.ID = id;

                    var IDVal = DataCreators.UpdateCustomerServiceList(id);
                    if (IDVal != null)
                    {
                        TempData["Msg"] = "Record Deleted";
                        return RedirectToAction("Dashboard", "Dashboard");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult AddServiceList(FormCollection form, Classes.Paytv.PaytvObj _pv)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var books = new List<Paytv.Menus>();
                var mc = DataReaders.getUserID(appUser);
                DataAccess.CustomerService _cs = new DataAccess.CustomerService();
                var ServicesList = Convert.ToInt16(form["ServicesList"]);
                var ServiceType = DataReaders.GetAllServicesByID(ServicesList);

                var SubServicesList = Convert.ToString(form["SubServicesList"]);
                //var CustomerId = Convert.ToString(form["CustomerId"]);
                var CustomerId = _pv.CustomerID;
                _cs.CustomerID = CustomerId;
                _cs.CustomerIDLabel = getCustomerIdLabel(ServiceType);
                _cs.Customer_FK = mc.id;
                _cs.DateCreated = DateTime.Today;
                _cs.PackageType = SubServicesList;
                _cs.PackageTypeIDLabel = "";
                _cs.isVissible = 1;
                PowerController pwr = new PowerController();
                //var value = pwr.checkCustomerServices(CustomerId, MFK);
                //if (value == false)
                //{

                //}
                gb.CustomerServices.Add(_cs);
                gb.SaveChanges();
                if (_cs.ID != 0)
                {
                    TempData["Msg"] = "ServiceList Added";
                    return RedirectToAction("Dashboard", "Dashboard");
                }
                else
                {
                    TempData["Msg"] = "Pleas Try Again";
                    return View();
                }
                // var q = DataCreator.InsertServiceList(_cs);
                // return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getCustomerIdLabel(string ServList)
        {
            try
            {
                string Label = "";
                if (ServList == "Power")
                {
                    Label = "Meter Number";
                }
                if (ServList == "Data")
                {

                    Label = "Device Number";
                }
                if (ServList == "Cable TV")
                {

                    Label = "Smartcard Number";
                }
                if (ServList == "Airtime")
                {

                    Label = "Phone Number";
                }


                return Label;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }

        }
        public string GetMenus()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return null;
                }

                var mc = DataReaders.getUserID(appUser);
                if (mc.BankFk == null || mc.BankFk == 0)
                {
                    TempData["Flg"] = "0";
                }
                    //the line does the same thing
                    var ids = (from a in gb.UserRoles where a.UserId == mc.id select a.RoleId).ToList();
                var roles = DataReaders.getUserRoles(ids.Cast<int>().ToList());
                foreach (var item in roles)
                { rol.Add(item.RoleName); }

                var results = (from p in gb.Pags
                               join pa in gb.PageAuthentications on p.PageName equals pa.PageName
                               join ph in gb.pageHeaders on p.pageHeader equals ph.id
                               join r in gb.Roles on pa.RoleId equals r.RoleId

                               where rol.Contains(r.RoleName)
                               select new Paytv.Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.RoleId,
                                   pageheader = ph.page_header,
                                   pageurl = p.PageUrl,
                               }).Distinct();

                var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());
                //August 13 Added This to Test
                //HttpRequest request = System.Web.HttpContext.Current.Request;
                //string url = request.Url.ToString();

                //string[] dspilt = url.ToString().Trim().Split('/');
                //var rawurl = "/" + dspilt[dspilt.Length - 1];

                //ViewBag.Menus = results;

                //var pagrol = DataReaders.ValidateRole(results,rawurl);

                //if(pagrol == false)
                //{
                //    var defaulturl = ConfigurationManager.AppSettings["DefaultUrl"];
                //    Response.Redirect(defaulturl);
                //    return ViewBag.Menus = Menus;
                //}

                return ViewBag.Menus = Menus;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void LoadMeltProperties()
        {


            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            if (appUser == null) return;

            var mc = DataReaders.getUserID(appUser);

            List<string> result = new List<string>();
            lvm.Report = (from a in gb.TransactionLogs
                          join c in gb.PaymentLogs on a.ID equals c.ID
                          where a.ID == mc.id

                          select new Paytv.Report
                          {
                              Amount = a.Amount.ToString(),
                              CustomerID = a.CustomerID,
                              CustomerName = a.CustomerName,
                              Customer_FK = a.Customer_FK.ToString(),
                              Merchant_FK = a.Merchant_FK.ToString(),
                              ReferenceNumber = a.ReferenceNumber,
                              ServiceCharge = a.ServiceCharge.ToString(),
                              ServiceDetails = a.ServiceDetails,
                              TransactionType = a.TransactionType.ToString(),
                              //DateTime.Parse(TrnDate) = a.TrnDate,
                              TrxToken = a.TrxToken,
                              ValueDate = a.ValueDate,
                              ValueTime = a.ValueTime,
                              ResponseCode = c.ResponseCode.ToString(),
                              Description = c.ResponseDescription,
                          }).ToList();



        }

        public bool HideOrShowPage()
        {
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();

            var appUser = user;
            if (appUser == null)
            {
                return false;
            }

            var mc = DataReaders.getUserID(appUser);
            //the line does the same thing
            var ids = (from a in gb.UserRoles where a.UserId == mc.id select a.RoleId).ToList();
            var roles = DataReaders.getUserRoles(ids.Cast<int>().ToList());
            foreach (var item in roles)
            { rol.Add(item.RoleName); }

            var results = (from p in gb.Pags
                           join pa in gb.PageAuthentications on p.PageName equals pa.PageName
                           join ph in gb.pageHeaders on p.pageHeader equals ph.id
                           join r in gb.Roles on pa.RoleId equals r.RoleId
                           where rol.Contains(r.RoleName)

                           select new

                           {
                               pageName = pa.PageName,
                               roleid = pa.RoleId,
                               pageheader = ph.page_header,
                               pageurl = p.PageUrl,

                           }).ToList();
            var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());
            ViewBag.Menus = Menus;

            if (Menus.Count > 0)
            {
                return true;
            }
            return false;
        }

        [HttpGet]
        public ActionResult Index()
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var books = new List<Paytv.Menus>();
                var mc = DataReaders.getUserID(appUser);
                if (mc.BankFk == null || mc.BankFk == 0)
                {
                    TempData["Flg"] = "0";
                }
                WebLog.Log("mc " + mc);
                //the line does the same thing
                var ids = (from a in gb.UserRoles where a.UserId == mc.id select a.RoleId).ToList();
                WebLog.Log("ids " + mc.id);
                var roles = DataReaders.getUserRoles(ids.Cast<int>().ToList());
                WebLog.Log("roles " + roles);
                foreach (var item in roles)
                {
                    rol.Add(item.RoleName);
                    WebLog.Log("item.RoleName " + item.RoleName);
                }

                var results = (from p in gb.Pags
                               join pa in gb.PageAuthentications on p.PageName equals pa.PageName
                               join ph in gb.pageHeaders on p.pageHeader equals ph.id
                               join r in gb.Roles on pa.RoleId equals r.RoleId

                               where rol.Contains(r.RoleName)
                               select new Paytv.Menus
                               {
                                   pageName = pa.PageName,
                                   roleid = (int)pa.RoleId,
                                   pageheader = ph.page_header,
                                   pageurl = p.PageUrl,

                               }).ToList();
                WebLog.Log("Results " + results);
                var Menus = results.ToList().Distinct().GroupBy(k => (k.pageheader)).OrderBy(k => k.Key).ToDictionary(k => k.Key, v => v.ToList());
                ViewBag.Menus = Menus;
                WebLog.Log("Menu " + Menus.ToString());
                WebLog.Log("ViewBag.Menus" + ViewBag.Menus);
                WebLog.Log("ViewBag.Menus" + ViewBag.Menus.Count);
                var dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null) return View();

                    //var mc = DataReaders.getUserID(appUser);
                    // This is done on Dec 15 2020

                   


                    id = mc.id.ToString();
                    Session["userid"] = id;
                    // getRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ID equals c.ID
                                    where a.Customer_FK == mc.id
                                    orderby c.TrnDate descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList().Take(10);
                    // .OrderByDescending(a => a.TrnDate).
                    Session["AllTransaction"] = ViewBag.Data;
                    WebLog.Log("ViewBag.Data " + ViewBag.Data);
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult index(FormCollection form)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var books = new List<Paytv.Menus>();
                var mc = DataReaders.getUserID(appUser);
                WebLog.Log("mc " + mc);
                //the line does the same thing
                GetMenus();
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);

                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null) return View();

                    //var mc = DataReaders.getUserID(appUser);

                    //var BankCode = _dr.GetBankCode();

                    string Userid = Convert.ToString(mc.id);
                    var BankUserCode = ConfigurationManager.AppSettings["BankUserCode"];
                    if(BankUserCode.Length > 0)
                    {
                        var db = GetBankCustomerTransaction(Userid, BankUserCode, fromDate, toDate);


                    }
                    else
                    //


                    id = mc.id.ToString();
                    Session["userid"] = id;
                    // getRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ID equals c.ID
                                    where a.Customer_FK == mc.id
                                    && c.TrnDate >= fromDate
                                    && c.TrnDate <= toDate
                                    orderby c.TrnDate
                                    //descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList();

                    Session["AllTransaction"] = ViewBag.Data;
                    WebLog.Log("ViewBag.Data " + ViewBag.Data);
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<DataAccess.DataReaders.Report> GetBankCustomerTransaction(string userid,string BankUserCode, DateTime fromDate, DateTime toDate)
        {
            try
            {
               // var BankUserCode = ConfigurationManager.AppSettings["BankUserCode"];
               // var date = Convert.ToString(form["txtDatePicker1"]);
               // var dates = Convert.ToString(form["txtDatePicker"]);
               // date = date + " - " + dates;
               // WebLog.Log("date" + date);
               // string[] parts = date.Split(' ');
               // WebLog.Log("Parts" + parts);
               // var strFromDate = date.Before("-").Trim();
               // WebLog.Log("strFromDate" + strFromDate);
               // var strToDate = date.After("-").Trim();
               // WebLog.Log("strToDate" + strToDate);

               // var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
              //  WebLog.Log("fromDate" + fromDate);
               // var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);

                //var CustomerTransactions = _dr.GetBanktransactions(Convert.ToInt16(userid),Convert.ToInt16(BankUserCode),fromDate,toDate);
                var CustomerTransactions = _dr.BankUsersTransaction(Convert.ToInt16(userid), Convert.ToInt16(BankUserCode), fromDate, toDate);

                if(CustomerTransactions == null)
                {
                    return null;
                }
                return CustomerTransactions;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            
        }


        public void getRecord()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null) return;

                var mc = DataReaders.getUserID(appUser);


                //var date = txtDatePicker.Text;
                var date = "10-13-2018 - 12-13-2019";
                string[] parts = date.Split(' ');
                var strFromDate = date.Before("-").Trim();
                var strToDate = date.After("-").Trim();


                var fromDate = DateTime.ParseExact(parts[0], "MM/dd/yyyy", new CultureInfo("en-US"));
                var toDate = DateTime.ParseExact(parts[2], "MM/dd/yyyy", new CultureInfo("en-US"));


                var trxs = DataReaders.getCustomerTransactionSum(mc.id);
                TempData["lblTotalTrxAmount"] = "0";
                TempData["lblTotalSuccessfulTrxAmount"] = "0";
                TempData["lblTotalFailedTrxAmount"] = "0";
                TempData["lblTotalPendingTrxAmount"] = "0";


                if (trxs == null) return;

                /*Total Transaction DashBoard Item*/
                var trxRange = trxs.Where(x => (x.TrnDate) >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList();
                TempData["lblTotalTrxAmount"] = $"{trxRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";
                TempData["lblTotalSummary"] = $"from {trxRange.Count} transactions.";

                // Total Successful Transaction DashBoard Item
                var trxSuccessRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "successful").ToList();
                TempData["lblTotalSuccessfulTrxAmount"] = $"{trxSuccessRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalSuccessfulSummary"] = $"from {trxSuccessRange.Count} successful transactions.";

                //Total Failed Transaction DashBoard Item
                var trxFailedRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "Failed").ToList();
                TempData["lblTotalFailedTrxAmount"] = $"{trxFailedRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalFailedSummary"] = $"from {trxFailedRange.Count} failed transactions.";

                //Total Pending Transaction DashBoard Item
                var trxPendingRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "Processing").ToList();

                TempData["lblTotalPendingTrxAmount"] = $"{trxPendingRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalPendingSummary"] = $"from {trxPendingRange.Count} pending transactions.";

                /*Top Ten Transactions GridView DashBoard Item*/

                if (trxRange.Count <= 0) return;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        public void getTopicons()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null) return;

                var mc = DataReaders.getUserID(appUser);
                var trxs = DataReaders.getCustomerTransactionSum(mc.id);
                TempData["WalletPoint"] = "0";
                TempData["ReferalCode"] = "0";
                TempData["lastTransaction"] = "0";
                TempData["NoOfReferal"] = "0";
                ViewBag.NoOfReferal = "0";
                ViewBag.WalletPoint = "0";
                ViewBag.ReferalCode = "0";
                ViewBag.lastTransaction = "0";

                if (trxs == null) return;

                /*Total Transaction DashBoard Item*/
                var ReferalCode = DataReaders.GetReferalCode(mc.id);
                TempData["ReferalCode"] = ReferalCode;
                ViewBag.ReferalCode = ReferalCode;
                var Walletpoint = DataReaders.GetWalletballance(mc.id);
                TempData["WalletPoint"] = Walletpoint;
                ViewBag.WalletPoint = Walletpoint;
                var LastTransaction = DataReaders.GetLastSuccesfulTransaction(mc.id);

                TempData["lastTransaction"] = LastTransaction;
                ViewBag.lastTransaction = LastTransaction;

                var NoOFReferals = DataReaders.GetNoOfReferals(mc.id);
                ViewBag.NoOfReferal = NoOFReferals;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }

        public List<string> GetTransactions()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return null;
                }

                var mc = DataReaders.getUserID(appUser);
                var query = DataReaders.getCustomerTransactionRecords(mc.id);

                return query.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }

        /* public string GetUserDetails()
         {
             id = Convert.ToString(Session["userid"]);
             var userid = Convert.ToInt16(id);
            // ViewBag.Data = DataReaders.getUserProfile(userid);
             ViewBag.Data = (from a in gb.Users
                             where a.id == userid
                             select new
                             {

                                 firstname = a.firstname,
                                 lastname = a.lastname,
                                 Phone = a.Phone,
                                 Email = a.Email,
                             }).ToList();
             return ViewBag.Data;
         }*/

        [HttpGet]
        public ActionResult EditProfile()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }

                id = Convert.ToString(Session["userid"]);
                var userid = Convert.ToInt16(id);
                //ViewBag.Data = DataReaders.getUserProfile(userid);
                ViewBag.Data = (from a in gb.Users
                                where a.id == userid
                                select new AccountsModel
                                {
                                    id = a.id,
                                    firstname = a.firstname,
                                    lastname = a.lastname,
                                    Phone = a.Phone,
                                    Email = a.Email,
                                }).ToList();

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult EditProfile(FormCollection form)
        {
            try
            {
                User users = new DataAccess.User();
                users.id = Convert.ToInt16(form["id"]);
                users.firstname = Convert.ToString(form["firstname"]);
                users.lastname = Convert.ToString(form["lastname"]);
                users.Email = Convert.ToString(form["Email"]);
                users.Phone = Convert.ToString(form["Phone"]);
                users.pasword = Convert.ToString(form["pasword"]);
                users.confirmPassword = Convert.ToString(form["confirmPassword"]);
                users.Referal = Convert.ToString(form["Referal"]);
                var Result = DataCreators.UpdateProfile(users);
                if (Result != "0")
                {
                    TempData["message"] = "Profile Updated";
                    return RedirectToAction("EditProfile", "Dashboard");

                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString
                    ());
                return null;
            }
        }

        /*  [HttpGet]
          public ActionResult BuyDirectPower()
          {
              try
              {
                  var LoggedInuser = new LogginHelper();
                  user = LoggedInuser.LoggedInUser();

                  var appUser = user;
                  if (appUser == null)
                  {
                      return RedirectToAction("Index", "Home");
                  }
                  GetMenus();

                  return View();
              }
              catch (Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());
                  return null;
              }
          }
          [HttpPost]
          public ActionResult BuyDirectPower(FormCollection form)
          {
              try
              {

                  var LoggedInuser = new LogginHelper();
                  user = LoggedInuser.LoggedInUser();

                  var appUser = user;
                  if (appUser == null)
                  {
                      return RedirectToAction("Index", "Home");
                  }
                  GetMenus();
                  var refNum = Convert.ToString(form["refNum"]);
                  var MerchankFk = Convert.ToString(form["MerchantFk"]);
                  var PayRes = Classes.Power.PostBuyPowerJson(refNum, Convert.ToInt16(MerchankFk));
                  return View();
              }
              catch (Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());
                  return null;
              }
          }

  */


        [HttpGet]
        public ActionResult BuyPower()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();

                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult BuyPower(FormCollection form)
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();
                CustomerTransaction _ct = new CustomerTransaction();
                PaymentLog _PL = new PaymentLog();
                TransactionLog _tR = new TransactionLog();
                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                var refNum = Convert.ToString(form["refNum"]);
                var mMFK = _dr.getMerchantFk(refNum);
                var PayRes = Classes.Power.PostBuyPowerJson(refNum, Convert.ToInt16(mMFK));
                dynamic resp = JObject.Parse(PayRes);
                var Record = _dr.GetTransactionLog(refNum);
                if(Record == null )
                {
                    TempData["ErrMsg"] = "Transaction Failed !";
                    return View();
                }
                var validate = _dr.GetCustTransac(refNum);
                if(validate != null || validate != "")
                {
                    TempData["ErrMsg"] = "Transaction Already Succesful !";
                    return View();
                }
                if(resp?.respCode == "00")
                {
                    _PL.ReferenceNumber = refNum;
                    _PL.ResponseCode = "00";
                    _PL.TrxToken = refNum;
                    _PL.ResponseDescription = "Succesful";
                    DataCreator _dc = new DataCreator();
                    _dc.updateDashRequeryPaymentLog(_PL);
                    
                    _ct.Amount = Record.Amount;
                    _ct.CusPay2response = Convert.ToString(resp);
                    _ct.TrnDate = Record.TrnDate;
                    _ct.ServiceDetails = Record.ServiceDetails;
                    _ct.ServiceCharge = Record.ServiceCharge;
                    _ct.ReferenceNumber = Record.ReferenceNumber;
                    _ct.TransactionType = Record.TransactionType;
                    _ct.Merchant_FK = Record.Merchant_FK;
                    _ct.ValueDate = Record.ValueDate;
                    _ct.ValueTime = Record.ValueTime;

                    _dc.Customertransaction(_ct);
                    _tR.ReferenceNumber = refNum;
                    _tR.ServiceValueDetails1 = resp?.units;
                    _tR.ServiceValueDetails2 = resp?.value;
                    _tR.ServiceValueDetails3 = resp?.customerAdddress;
                    _dc.updateDashRequeryTransactLog(_tR);
                    TempData["SucMsg"] = "Transaction Sucessful !";
                }
                if(resp?.respCode != "00")
                {
                    TempData["ErrMsg"] = resp?.respDescription;
                    return View();
                }
                else if (resp?.respCode == null)
                {
                    TempData["ErrMsg"] = "Please Try Again !";
                    return View();
                }
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult EditPassword()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult EditPassword(FormCollection form)
        {
            try
            {

                string EncrypPassword = "";
                User users = new DataAccess.User();
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    string password = Convert.ToString(form["password"]);
                    users.pasword = Convert.ToString(form["newPassword"]);
                    users.confirmPassword = Convert.ToString(form["confirmPassword"]);
                    EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                }
                if (users.confirmPassword != users.pasword)
                {
                    GetMenus();
                    TempData["message"] = "New Password and Confirm Password Must Match";
                    return View();
                }
                else
                {
                    var NewEncrypPassword = new CryptographyManager().ComputeHash(users.pasword, HashName.SHA256);
                    users.pasword = NewEncrypPassword;
                    users.Email = Session["id"].ToString();
                    var valid = DataReaders.loggedIn(users.Email, EncrypPassword);
                    users.pasword = NewEncrypPassword;
                    users.confirmPassword = NewEncrypPassword;
                    string Email = users.Email;
                    User id = DataReaders.getUserID(Email);
                    users.id = id.id;
                    if (valid == true)
                    {

                        DataCreators.UpdatePassword(users);
                        TempData["message"] = "Password Succesfully Updated";
                        GetMenus();
                    }

                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ViewReceipt()
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

        [HttpGet]
        public ActionResult GetPaymentResponse(string id)
        {
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();
            string img = "";
            string imgName = "";
            var appUser = user;
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            //GetMenus();
            PaytvController _pv = new PaytvController();
            PowerController _pw = new PowerController();
            var RecieptVal = DataReaders.GetCustReceipt(id);
            if (RecieptVal.Count == 0)
            {
                return RedirectToAction("SucessfulTransaction");
            }
            Classes.Power.Receipt Receipt = new Classes.Power.Receipt();
            Receipt.CustomerID = RecieptVal[0];
            Receipt.Amount = Convert.ToDouble(RecieptVal[3]);
            Receipt.ServiceCharge = (Convert.ToDouble(RecieptVal[2]));
            Receipt.CustomerName = RecieptVal[1];
            Receipt.Phone = RecieptVal[5];
            Receipt.TrnDate = Convert.ToDateTime(RecieptVal[4]);
            Receipt.ReferenceNumber = RecieptVal[6];
            Receipt.ServiceDetails = RecieptVal[7];
            string Discoid = RecieptVal[8];
            if (id.StartsWith("4"))
            {
                img = _pv.LoadImage(Discoid);
                imgName = _pv.getDiscoName(Discoid);
            }
            else
            {
                img = _pw.LoadImage(Discoid);
                imgName = _pw.getDiscoName(Discoid);
            }

            Double TotalAmount = Convert.ToDouble(Receipt.ServiceCharge + Receipt.Amount);
            TempData["Amount"] = TotalAmount;
            TempData["message"] = img;
            TempData["BillerName"] = imgName;
            return View(Receipt);
        }


        [HttpGet]
        public ActionResult GetDetails(string Refnum)
        {
            var LoggedInuser = new LogginHelper();
            user = LoggedInuser.LoggedInUser();
            string img = "";
            string imgName = "";
            var appUser = user;
            if (appUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            PaytvController _pv = new PaytvController();
            PowerController _pw = new PowerController();
            var RecieptVal = DataReaders.GetDetails(Refnum);
            if (RecieptVal == null)
            {
                return RedirectToAction("TransactionLog");
            }
            Classes.Power.Receipt Receipt = new Classes.Power.Receipt();

            Receipt.CustomerID = RecieptVal.CustomerID;
            Receipt.Amount = Convert.ToDouble(RecieptVal.Amount);
            Receipt.ServiceCharge = RecieptVal.ServiceCharge != null ? RecieptVal.ServiceCharge : 0;
            Receipt.CustomerName = RecieptVal.CustomerName;
            Receipt.Phone = RecieptVal.CustomerPhone;
            Receipt.ReferenceNumber = RecieptVal.ReferenceNumber;
            Receipt.ServiceDetails = RecieptVal.ServiceDetails;
            Receipt.TransactionStatus_FK = RecieptVal.TransactionStatus_FK;
            ViewBag.Token = RecieptVal.ServiceValueDetails1;
            Receipt.ServiceValueDetails2 = RecieptVal.ServiceValueDetails2;
            Receipt.TrnDate = RecieptVal.TrnDate;
            string Discoid = RecieptVal.Merchant_FK.ToString();

            if (Discoid.Length > 0)
            {
                img = _pv.LoadPowerImage(Discoid);
                imgName = _pv.getPowerDiscoName(Discoid);
            }
            else
            {
                // img = _pw.LoadImage(Discoid);
                // imgName = _pw.getDiscoName(Discoid);
            }

            Double TotalAmount = Convert.ToDouble(Receipt.ServiceCharge + Receipt.Amount);
            TempData["Amount"] = TotalAmount;
            TempData["message"] = img;
            TempData["BillerName"] = imgName;

            return View(Receipt);
        }

        [HttpPost]
        public ActionResult BankRequery(Classes.Power.Receipt Recpt, FormCollection form)
        {
            try
            {
                string RefNum = Request.QueryString["Refnum"];

                string Refnum = Convert.ToString(form["RefNum"]);
                GetTransactionDetails(Refnum/*, Recpt*/);
                // GetDetails(Refnum);
                // return View();
                return RedirectToAction("GetDetails", new
                {
                    @Refnum = Refnum
                });
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        //  [HttpPost]
        [HttpGet]
        public ActionResult PowerRequery(string id/*, Classes.Power.Receipt Recpt*/)
        {
            try
            {

                // GetTransactionDetails(id,Recpt);
               
                GetRequeryTransactionDetails(id);
                return RedirectToAction("RequeryTransaction");
                //updateTransaction();
                //return RedirectToAction("GetDetails", new
                //{
                //    @Refnum = Refnum
                //});
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public dynamic GetRequeryTransactionDetails(string RefNum)
        {
            try
            {
                Classes.Power.Receipt Recpt = new Classes.Power.Receipt();
                var GetTransaction = _dr.GetRequeryData(RefNum);
                Recpt.ReferenceNumber = GetTransaction.ReferenceNumber;
                var refNumber = Recpt.ReferenceNumber;
                Recpt.Amount = (double)GetTransaction.Amount;
                var amount = Recpt.Amount;

                Recpt.Merchant_FK = Convert.ToInt16(GetTransaction.Merchant_FK);
                /*bool valid = ProcessResponse(RefNum);
                if (valid != true)
                {
                    return RedirectToAction("RequeryTransaction");
                }
                */
                dynamic Resp = Classes.Power.GetTransactionDetails(Recpt);
                // i am here
                Resp = JObject.Parse(Resp);
                //DataAccess.TransactionRecord TLog = new DataAccess.TransactionRecord();
                TransactionLog TLog = new TransactionLog();
                if (Resp?.respCode == "00")
                {
                    TLog.ReferenceNumber = Recpt.ReferenceNumber;
                    TLog.TrnDate = GetTransaction.TrnDate;
                    TLog.Merchant_FK = GetTransaction.Merchant_FK;

                    /* For banks 
                    var RespUpt = DataCreators.UpdateTransacRecord(Resp, TLog);
                     */
                    DataCreator _dc = new DataCreator();
                    TLog.ReferenceNumber = refNumber;
                    TLog.ServiceValueDetails1 = Resp?.units;
                    TLog.ServiceValueDetails2 = Resp?.value;
                    TLog.ServiceValueDetails3 = Resp?.customerAdddress;

                    TempData["Msg"] = "Units" + " "  + " " + Resp?.units + " "  + " " + "Token" + " " + Resp?.value ;

                    _dc.updateDashRequeryTransactLog(TLog);

                    PaymentLog _pl = new PaymentLog();
                    _pl.ReferenceNumber = TLog.ReferenceNumber;
                    _pl.ResponseCode = "00";
                    _pl.ResponseDescription = "successful";
                    _pl.TrxToken = refNumber;
                    _pl.TrnDate = GetTransaction.TrnDate;
                    DataCreators.UpdatePaymentLog(_pl);
                    var RespUpt = DataCreators.UpdateCustomerTransacRecord(Resp,TLog);

                }
                if (Resp?.respCode != "00")
                {
                    TempData["Msg"] = Resp?.description;
                }
                    return Resp;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }

        public dynamic GetTransactionDetails(string RefNum)
        {
            try
            {
                Classes.Power.Receipt Recpt = new Classes.Power.Receipt();
                var GetTransaction = _dr.GetBankRequeryData(RefNum);
                Recpt.ReferenceNumber = GetTransaction.ReferenceNumber;
                var refNumber = Recpt.ReferenceNumber;
                Recpt.Amount = (double)GetTransaction.Amount;
                var amount = Recpt.Amount;

                Recpt.Merchant_FK = Convert.ToInt16(GetTransaction.Merchant_FK);
                dynamic Resp = Classes.Power.GetTransactionDetails(Recpt);
                // i am here
                Resp = JObject.Parse(Resp);
                DataAccess.TransactionRecord TLog = new DataAccess.TransactionRecord();
                if (Resp?.respCode == 00)
                {
                    TLog.ReferenceNumber = Resp?.receiptNumber;
                    var RespUpt = DataCreators.UpdateTransacRecord(Resp, TLog);
                }
                return Resp;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }

        [HttpGet]
        public ActionResult RequeryTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();

                    /*  var ct = (from a in gb.CustomerTransactions where a.ReferenceNumber.StartsWith("PWV") select a.ReferenceNumber).ToList();
                      var result = (from c in gb.PaymentLogs
                                    where c.ResponseCode == "00" && c.ReferenceNumber.StartsWith("PWV")
                                    select c.ReferenceNumber).ToList();
                      var filteredList = result.Except(ct);
                      var rec =   (from a in gb.TransactionLogs
                                                join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                                where filteredList.Contains(a.ReferenceNumber)
                                                orderby c.TrnDate
                                                descending
                                                select new Paytv.Report
                                                {
                                                    Amount = a.Amount.ToString(),
                                                    CustomerID = a.CustomerID,
                                                    CustomerName = a.CustomerName,
                                                    Customer_FK = a.Customer_FK.ToString(),
                                                    Merchant_FK = a.Merchant_FK.ToString(),
                                                    ReferenceNumber = a.ReferenceNumber,
                                                    ServiceCharge = a.ServiceCharge.ToString(),
                                                    ServiceDetails = a.ServiceDetails,
                                                    TransactionType = a.TransactionType.ToString(),
                                                    TrnDate = a.TrnDate.ToString(),
                                                    TrxToken = a.TrxToken,
                                                    ValueDate = a.ValueDate,
                                                    ValueTime = a.ValueTime,
                                                    ResponseCode = c.ResponseCode.ToString(),
                                                    Description = c.ResponseDescription,
                                               }).ToList().Take(10);
                      ViewBag.Data = rec;*/
                     
                    var rec = (from a in gb.TransactionLogs
                               join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                               where c.ResponseCode == "00" && a.ServiceValueDetails2 == null && a.ReferenceNumber.StartsWith("PWV")
                               orderby c.TrnDate
                               descending
                               select new Paytv.Report
                               {
                                   Amount = a.Amount.ToString(),
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   Customer_FK = a.Customer_FK.ToString(),
                                   Merchant_FK = a.Merchant_FK.ToString(),
                                   ReferenceNumber = a.ReferenceNumber,
                                   ServiceCharge = a.ServiceCharge.ToString(),
                                   ServiceDetails = a.ServiceDetails,
                                   TransactionType = a.TransactionType.ToString(),
                                   TrnDate = a.TrnDate.ToString(),
                                   TrxToken = a.TrxToken,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   ResponseCode = c.ResponseCode.ToString(),
                                   Description = c.ResponseDescription,
                               }).ToList().Take(10);
                    ViewBag.Data = rec;
                    // var f = result.Except(yui);
                    GetMenus();
                    Session["AllTransaction"] = ViewBag.Data;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult RequeryTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //if (Session["id"] != null || Session["id"].ToString() != "")
                //{
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                  /*var ct = (from a in gb.CustomerTransactions where a.ReferenceNumber.StartsWith("PWV") select a.ReferenceNumber).ToList();
                    var result = (from c in gb.PaymentLogs
                                  where c.ResponseCode == "00" && c.ReferenceNumber.StartsWith("PWV")
                                  select c.ReferenceNumber).ToList();
                    var f = result.Except(ct);
                    var filteredList = result.Except(ct);
                    var rec = (from a in gb.TransactionLogs
                               join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                               where filteredList.Contains(a.ReferenceNumber)
                                && c.TrnDate >= fromDate
                                && c.TrnDate <= toDate
                                orderby c.TrnDate
                              descending
                               select new Paytv.Report
                               {
                                   Amount = a.Amount.ToString(),
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   Customer_FK = a.Customer_FK.ToString(),
                                   Merchant_FK = a.Merchant_FK.ToString(),
                                   ReferenceNumber = a.ReferenceNumber,
                                   ServiceCharge = a.ServiceCharge.ToString(),
                                   ServiceDetails = a.ServiceDetails,
                                   TransactionType = a.TransactionType.ToString(),
                                   TrnDate = a.TrnDate.ToString(),
                                   TrxToken = a.TrxToken,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   ResponseCode = c.ResponseCode.ToString(),
                                   Description = c.ResponseDescription,
                               }).ToList().Take(10);
                    ViewBag.Data = rec;
                    */
                    var rec = (from a in gb.TransactionLogs
                               join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                               where c.ResponseCode == "00" && a.ServiceValueDetails2 == null && a.ReferenceNumber.StartsWith("PWV")
                                && c.TrnDate >= fromDate
                                && c.TrnDate <= toDate
                               orderby c.TrnDate
                             descending
                               select new Paytv.Report
                               {
                                   Amount = a.Amount.ToString(),
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   Customer_FK = a.Customer_FK.ToString(),
                                   Merchant_FK = a.Merchant_FK.ToString(),
                                   ReferenceNumber = a.ReferenceNumber,
                                   ServiceCharge = a.ServiceCharge.ToString(),
                                   ServiceDetails = a.ServiceDetails,
                                   TransactionType = a.TransactionType.ToString(),
                                   TrnDate = a.TrnDate.ToString(),
                                   TrxToken = a.TrxToken,
                                   ValueDate = a.ValueDate,
                                   ValueTime = a.ValueTime,
                                   ResponseCode = c.ResponseCode.ToString(),
                                   Description = c.ResponseDescription,
                               }).ToList().Take(10);
                GetMenus();
                ViewBag.Data = rec;
                Session["Records"] = ViewBag.Data;
                Session["AllTransaction"] = ViewBag.Data;
                    return View();
               // }

                //else
                //{
                //    return RedirectToAction("Index", "Home");
                //}
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult SucessfulTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.ResponseCode == "00"
                                    orderby c.TrnDate
                                    descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList().Take(10);
                    GetMenus();
                    Session["AllTransaction"] = ViewBag.Data;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult SucessfulTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.ResponseCode == "00"
                                    && c.TrnDate >= fromDate
                                    && c.TrnDate <= toDate
                                    orderby c.TrnDate
                                    //descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList();
                    GetMenus();
                    Session["AllTransaction"] = ViewBag.Data;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpGet]
        public ActionResult LendTransactions()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.ResponseCode == "00" && a.ReferenceNumber.StartsWith("PWL")
                                    orderby c.TrnDate
                                    descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList().Take(10);
                    GetMenus();
                    Session["AllTransaction"] = ViewBag.Data;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult LendTransactions(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null)  
                    //return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.ResponseCode == "00" && a.ReferenceNumber.StartsWith("PWL")
                                    && c.TrnDate >= fromDate
                                    && c.TrnDate <= toDate
                                    orderby c.TrnDate
                                    //descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList();
                    GetMenus();
                    Session["AllTransaction"] = ViewBag.Data;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
















        public void getAllRecord()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null) return;

                var mc = DataReaders.getUserID(appUser);


                //var date = txtDatePicker.Text;
                var date = "10-13-2018 - 12-13-2019";
                string[] parts = date.Split(' ');
                var strFromDate = date.Before("-").Trim();
                var strToDate = date.After("-").Trim();


                var fromDate = DateTime.ParseExact(parts[0], "MM/dd/yyyy", new CultureInfo("en-US"));
                var toDate = DateTime.ParseExact(parts[2], "MM/dd/yyyy", new CultureInfo("en-US"));


                var trxs = DataReaders.getAllTransactionSum(mc.id);
                TempData["lblTotalTrxAmount"] = "0";
                TempData["lblTotalSuccessfulTrxAmount"] = "0";
                TempData["lblTotalFailedTrxAmount"] = "0";
                TempData["lblTotalPendingTrxAmount"] = "0";


                if (trxs == null) return;

                /*Total Transaction DashBoard Item*/
                var trxRange = trxs.Where(x => (x.TrnDate) >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList();
                TempData["lblTotalTrxAmount"] = $"{trxRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";
                TempData["lblTotalSummary"] = $"from {trxRange.Count} transactions.";

                // Total Successful Transaction DashBoard Item
                var trxSuccessRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "successful").ToList();
                TempData["lblTotalSuccessfulTrxAmount"] = $"{trxSuccessRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalSuccessfulSummary"] = $"from {trxSuccessRange.Count} successful transactions.";

                //Total Failed Transaction DashBoard Item
                var trxFailedRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "Failed").ToList();
                TempData["lblTotalFailedTrxAmount"] = $"{trxFailedRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalFailedSummary"] = $"from {trxFailedRange.Count} failed transactions.";

                //Total Pending Transaction DashBoard Item
                var trxPendingRange = trxs.Where(x => x.TrnDate >= fromDate.AddDays(-1) && x.TrnDate <= toDate.AddDays(1)).ToList().Where(x => x.ResponseDescription == "Processing").ToList();

                TempData["lblTotalPendingTrxAmount"] = $"{trxPendingRange.Sum(x => decimal.Parse(x.Amount.ToString())):N}";

                TempData["lblTotalPendingSummary"] = $"from {trxPendingRange.Count} pending transactions.";

                /*Top Ten Transactions GridView DashBoard Item*/

                if (trxRange.Count <= 0) return;

            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }


        [HttpGet]
        public ActionResult Requery()
        {
            try
            {

                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult Requery(FormCollection form)
        {
            try
            {
                PaytvController _ptv = new PaytvController();
                var RefNum = Convert.ToString(form["RefNum"]);
                if (RefNum == null || RefNum == "")
                {
                    return RedirectToAction("Requery");
                }
                var TransRef = DataReaders.ValidatePayTrancLog(RefNum);
                if (TransRef == false)
                {
                    TempData["message"] = "Invalid Reference Number";
                    return RedirectToAction("Requery");
                }
                if (TransRef == true)
                {
                    dynamic requery = _ptv.paySubscribtion(RefNum);
                    requery = JObject.Parse(requery);
                    if (requery?.returnCode == "0")
                    {
                        WebLog.Log("requery" + requery);
                        WebLog.Log("requery?.returnCode" + requery?.returnCode);
                        var Response = InsertCustTransac(requery);
                        ViewBag.Data = Convert.ToString(requery);
                    }
                    else
                    {
                        ViewBag.Data = Convert.ToString(requery);
                    }

                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string InsertCustTransac(dynamic requery)
        {
            try
            {
                DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
                string RefNum = requery?.tranNum;
                WebLog.Log("RefNum" + RefNum);
                var valid = _dr.GetCustTransac(RefNum);
                //WebLog.Log("valid" + valid.Amount);
                if (valid == null)
                {
                    var CustomerRecord = _dr.GetTransactionLog(RefNum);
                    WebLog.Log("RefNum" + RefNum);
                    _ct.CusPay2response = requery?.returnMsg;
                    WebLog.Log("_ct.CusPay2response" + _ct.CusPay2response);
                    _ct.Amount = CustomerRecord.Amount;
                    WebLog.Log("_ct.Amount" + _ct.Amount);
                    _ct.CustomerID = CustomerRecord.CustomerID;
                    WebLog.Log("_ct.CustomerID" + _ct.CustomerID);
                    _ct.Merchant_FK = CustomerRecord.Merchant_FK;
                    WebLog.Log("_ct.Merchant_FK" + _ct.Merchant_FK);
                    _ct.ReferenceNumber = CustomerRecord.ReferenceNumber;
                    WebLog.Log("_ct.ReferenceNumber" + _ct.ReferenceNumber);
                    _ct.TrnDate = CustomerRecord.TrnDate;
                    WebLog.Log("_ct.TrnDate" + _ct.TrnDate);
                    _ct.ValueDate = CustomerRecord.ValueDate;
                    WebLog.Log("_ct.ValueDate" + _ct.ValueDate);
                    _ct.ValueTime = CustomerRecord.ValueTime;
                    WebLog.Log("_ct.ValueTime" + _ct.ValueTime);
                    _ct.CustomerName = CustomerRecord.CustomerName;
                    WebLog.Log("_ct.CustomerName" + _ct.CustomerName);
                    _ct.TransactionType = CustomerRecord.TransactionType;
                    WebLog.Log("_ct.TransactionType" + _ct.TransactionType);
                    _ct.ServiceDetails = CustomerRecord.ServiceDetails;
                    WebLog.Log(" _ct.ServiceDetails" + _ct.ServiceDetails);
                    _ct.ServiceCharge = CustomerRecord.ServiceCharge;
                    WebLog.Log("_ct.ServiceCharge" + _ct.ServiceCharge);
                }
                DataAccess.DataCreator dc = new DataAccess.DataCreator();
                dc.InitiateCustomerTransaction(_ct);
                var resp = updateTransLog(RefNum);

                return resp;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }

        public string updateTransLog(string RefNum)
        {
            try
            {
                DataAccess.DataCreator _dc = new DataCreator();
                DataAccess.PaymentLog _pL = new PaymentLog();
                _pL.ReferenceNumber = RefNum;
                var CustRef = _dc.updateRequeryPaymentLog(_pL);
                return CustRef;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }




        [HttpPost]
        public ActionResult TransactionLog(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var MerchantFK = Convert.ToInt16(form["Merchant"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    // getAllRecord();
                    Flag = 0;
                    ViewBag.Data = DataReaders.getTransactionLogByDate(Userfk, fromDate, toDate,Flag,MerchantFK);

                    Session["AllTransaction"] = ViewBag.Data;
                    ViewBag.merchantList = _dr.getBankMerchant();
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult AdminTransactionLog(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var MerchantFK = Convert.ToInt16(form["Merchant"]);
                var PartnerFk = Convert.ToInt32(form["Patner"]);
               // int MFK = Convert.ToInt32(PartnerFk);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    // getAllRecord();
                    Flag = 0;
                    ViewBag.Data = DataReaders.getTransactionLogByDate(PartnerFk, fromDate, toDate, Flag, MerchantFK);

                    Session["AllTransaction"] = ViewBag.Data;
                    ViewBag.merchantList = _dr.getBankMerchant();
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult DailyTransactionLog(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var MerchantFK = Convert.ToInt16(form["Merchant"]);
                WebLog.Log("Merchank_fk"+ MerchantFK);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    // getAllRecord();
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    // ViewBag.Data = DataReaders.getDailyTransactionLogByDate(Userfk, fromDate, toDate);
                    ViewBag.Data = DataReaders.getDailyTransactionLog(Userfk, fromDate, toDate, Flag,MerchantFK);
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    Session["AllTransaction"] = ViewBag.Data;
                    ViewBag.merchantList = _dr.getBankMerchant();
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log("ex message"+ ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult DailyTransactionLog()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    Userfk = Convert.ToInt16(id);
                  
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    var MerchantFK = 0;
                    ViewBag.merchantList = _dr.getBankMerchant();
                    ViewBag.Data = DataReaders.getDailyTransactionLog(Userfk,from,to,Flag, MerchantFK);

                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult TransactionLog()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    Userfk = Convert.ToInt16(id);

                    //ViewBag.Data = DataReaders.getTransactionLog(Userfk);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    var MerchantFK = 0;
                    ViewBag.Data = DataReaders.getTransactionLogByDate(Userfk, from, to, Flag,MerchantFK);
                    ViewBag.merchantList = _dr.getBankMerchant();
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult AdminTransactionLog()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    Userfk = Convert.ToInt16(id);
                    ViewBag.Partner = _dr.GetAllPatners();
                    //ViewBag.Data = DataReaders.getTransactionLog(Userfk);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    var MerchantFK = 0;
                    ViewBag.Data = DataReaders.getTransactionLogByDate(Userfk, from, to, Flag, MerchantFK);
                    ViewBag.merchantList = _dr.getBankMerchant();
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult AllTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //       user = LoggedInuser.LoggedInUser();

                    //       var appUser = user;
                    //       if (appUser == null) //return View();
                    //      return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    /*ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ID equals c.ID*/
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    orderby c.TrnDate descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                        Phone = c.CustomerPhoneNumber,
                                    }).ToList().Take(10);

                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }








        protected bool ProcessResponse(string trxRef)
        {
           
            var SecretKey = ConfigurationManager.AppSettings["SecretKey"];
            TransactionalInformation trans;
            try
            {
                dynamic obj = new JObject();
                obj.trxRef = trxRef.Trim();
              
                var json = obj.ToString();
              
                string url = ConfigurationManager.AppSettings["FlutterWave_RequeryByRef"];
                //re-query payment gateway for the transaction.
                var reqData = "{\"TrxRef\":\"" + trxRef + "\"}";
               
                string serverResponse = string.Empty;

                var plainText = (trxRef  + SecretKey);

                var hash = new CryptographyManager().ComputeHash(plainText, HashName.SHA256);
                PaytvController _pv = new PaytvController();
                var isPaid = PaytvController.PaymentRequery(reqData, url, hash, out serverResponse, out trans);
                var jvalue = JObject.Parse(serverResponse);

                dynamic jvalues = JObject.Parse(serverResponse);
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() == "00" && isPaid == true)
                {


                    return true;
                }

                return false;
              
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }



        }

        [HttpGet]
        public ActionResult CustomerTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null) //return View();
                    //    return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    Session["AllTransaction"] = "";
                    getAllRecord();
                    GetMenus();


                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult AllTransaction(FormCollection form)
        {
            try
            {


                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    // getAllRecord();
                    /* ViewBag.Data = (from a in gb.TransactionLogs
                                     join c in gb.PaymentLogs on a.ID equals c.ID*/
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.TrnDate >= fromDate
                    && c.TrnDate <= toDate
                                    orderby c.TrnDate
                                    //descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                        Phone = c.CustomerPhoneNumber,
                                    }).ToList();
                    //  DataView dv =  ViewBag.Data;
                    //if (dv.Count > 0)
                    //{
                    //    btnExport.Visible = true;
                    //}
                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Exportoexcel()
        {
            ExportToExcel();
            GetMenus();
            return View("AllTransaction");
        }


        [HttpGet]
        public ActionResult AdminDailyTransactionLog()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    Userfk = Convert.ToInt16(id);

                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    var MerchantFK = 0;
                    ViewBag.Partner = _dr.GetAllPatners();
                    ViewBag.merchantList = _dr.getBankMerchant();
                    ViewBag.Data = DataReaders.getDailyTransactionLog(Userfk, from, to, Flag, MerchantFK);

                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult AdminDailyTransactionLog(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var MerchantFK = Convert.ToInt32(form["Merchant"]);
                var PartnerFk = Convert.ToInt32(form["Patner"]);
                int MFK = Convert.ToInt32(PartnerFk);
                WebLog.Log("Merchank_fk" + MerchantFK);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    // getAllRecord();
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    ViewBag.Partner = _dr.GetAllPatners();
                    ViewBag.Data = DataReaders.getDailyTransactionLog(PartnerFk, fromDate, toDate, Flag, MerchantFK);
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    Session["AllTransaction"] = ViewBag.Data;
                    ViewBag.merchantList = _dr.getBankMerchant();
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

               
            }
            catch (Exception ex)
            {
                WebLog.Log("ex message" + ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public void ExportToExcel()
        {
            //DataTable dt = new DataTable("GridView_Data");
            var gv = new GridView();
            gv.DataSource = Session["AllTransaction"];
            gv.DataBind();

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=AllTransaction.xls");
            Response.ContentType = "application/ms-excel";

            Response.Charset = "";
            StringWriter objStringWriter = new StringWriter();
            HtmlTextWriter objHtmlTextWriter = new HtmlTextWriter(objStringWriter);

            gv.RenderControl(objHtmlTextWriter);

            Response.Output.Write(objStringWriter.ToString());
            Response.Flush();
            Response.End();



        }

        [HttpPost]
        public ActionResult CustomerTransaction(FormCollection form)
        {
            try
            {
                string CustomerID = Convert.ToString(form["CustomerID"]);
                WebLog.Log("One" + CustomerID);

                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);

                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    // getAllRecord();
                    ViewBag.Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                    where c.CustomerID == CustomerID
                                    && c.TrnDate >= fromDate
                                    && c.TrnDate <= toDate

                                    orderby c.TrnDate descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList();
                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<getAllUserAndRoles> getAllBankUsersAndRoles(long Bankid)
        {
            //---- Create object of our entities class.
            try
            {
                User users = new DataAccess.User();
                LoanViewModel lvm = new LoanViewModel();
                var user = "tolutl@yahoo.com";
                users.Email = user;
                users.id = DataReaders.selectUserID(users);

                var results = (from a in gb.UserRoles
                               join c in gb.Roles on a.RoleId equals c.RoleId
                               join b in gb.Users on a.UserId equals b.id
                               where c.isVissible == 2 && b.BankFk == Bankid
                               select new Models.getAllUserAndRoles { userid = b.id, roleid = c.RoleId, rolename = c.RoleName, email = b.Email, id = a.id }).ToList();
                var model = new LoanViewModel
                {
                    getAllUserAndRoless = results.ToList(),

                };
                lvm.getAllUserAndRoless = results;
                return lvm.getAllUserAndRoless.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getAllUserAndRoles> getAllUsersAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                User users = new DataAccess.User();
                LoanViewModel lvm = new LoanViewModel();
                var user = "tolutl@yahoo.com";
                users.Email = user;
                users.id = DataReaders.selectUserID(users);

                var results = (from a in gb.UserRoles
                               join c in gb.Roles on a.RoleId equals c.RoleId
                               join b in gb.Users on a.UserId equals b.id
                               //where a.UserId == users.id
                               select new Models.getAllUserAndRoles { userid = b.id, roleid = c.RoleId, rolename = c.RoleName, email = b.Email, id = a.id }).ToList();
                var model = new LoanViewModel
                {
                    getAllUserAndRoless = results.ToList(),

                };
                lvm.getAllUserAndRoless = results;
                return lvm.getAllUserAndRoless.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public List<getAllPagesAndRoles> getAllPagesAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                var results = (from a in gb.PageAuthentications
                               join c in gb.Roles on a.RoleId equals c.RoleId

                               select new Models.getAllPagesAndRoles

                               {
                                   id = a.id,
                                   rolename = c.RoleName,
                                   roleid = c.RoleId,
                                   pageName = a.PageName,


                               }).ToList();


                var model = new LoanViewModel
                {
                    getAllPagesAndRoless = results.ToList(),

                };
                lvm.getAllPagesAndRoless = results;
                return lvm.getAllPagesAndRoless.ToList();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult UserRoles()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                DataAccess.Role rol = new DataAccess.Role();
                //var value = "";
                string userid = Request.QueryString["value"];
                TempData["userid"] = userid;
                if (userid == "" || userid == null)
                {

                    userid = "2016";
                }
                User users = new DataAccess.User();

                var value = userid;
                users.id = Convert.ToInt16(value);
                var Bankid = DataReaders.getBank(appUser);
                ViewBag.listUser = DataReaders.getUserByBank(Bankid);
                
                //  ViewBag.listUser = DataReaders.getAllUser();
                //getAllUsersRoles(value);
                var result = (from a in gb.UserRoles
                              join c in gb.Roles on a.RoleId equals c.RoleId
                              join b in gb.Users on a.UserId equals b.id
                              where a.UserId == users.id

                              select new Models.GetAssignRoles { userid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.Email }).ToList();

                var userroles = DataReaders.buildNamesList(users);
                //var roles = DataReaders.buildAllRoleList();
                var roles = DataReaders.buildAllBankRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = (from p in gb.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();



                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                // ViewBag.Data = getAllUsersAndRoles();
                ViewBag.Data = getAllBankUsersAndRoles(Bankid);
                GetMenus();
                return View(model);
            }


            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult UserRoles(LoanViewModel lvm, FormCollection form)
        {
            try
            {

                //int rolval = Convert.ToInt16(Request.QueryString["value"]);
                //string Pageid = Request.QueryString[0];
                //string sde = Request.QueryString.AllKeys.ToString();
                //int vca = Convert.ToInt16(TempData["userid"]);
                
                int rolval = Convert.ToInt16(TempData["userid"]);
                if (rolval == 0)
                {

                    TempData["message"] = "Please Select A User";

                    return RedirectToAction("UserRoles");

                }
                int roleid = Convert.ToInt16(form["roleid"]);
                int userid = Convert.ToInt16(form["id"]);
                userroles.RoleId = roleid;
                //userroles.UserId = userid;
                userroles.UserId = rolval;
                userroles.RoleId = DataReaders.selectRolesByName(userroles);
                if (userroles.RoleId != 0)
                {
                    TempData["message"] = "User Already Have This Role";
                    //ViewBag.listUser = DataReaders.getAllUser();
                    return RedirectToAction("UserRoles");
                    //return View("UsernRoles");
                }
                else
                {
                    userroles.RoleId = roleid;
                    //userroles.UserId = userid;
                    userroles.UserId = rolval;
                    userroles.IsVissible = 1;
                    userroles.dates = DateTime.Now;

                    DataCreators.InsertUserRoles(userroles);
                    TempData["message"] = "User n Roles Created Succesfully";
                    lvm.getAllUserAndRoless = getAllUsersAndRoles();

                }
                //return View("");
                return RedirectToAction("UserRoles");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }






        [HttpGet]
        public ActionResult UsernRoles()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                DataAccess.Role rol = new DataAccess.Role();
                //var value = "";
                string userid = Request.QueryString["value"];
                TempData["userid"] = userid;
                if (userid == "" || userid == null)
                {

                    userid = "2016";
                }
                User users = new DataAccess.User();

                var value = userid;
                users.id = Convert.ToInt16(value);

                ViewBag.listUser = DataReaders.getAllUser();
                //getAllUsersRoles(value);
                var result = (from a in gb.UserRoles
                              join c in gb.Roles on a.RoleId equals c.RoleId
                              join b in gb.Users on a.UserId equals b.id
                              where a.UserId == users.id

                              select new Models.GetAssignRoles { userid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.Email }).ToList();

                var userroles = DataReaders.buildNamesList(users);
                var roles = DataReaders.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = (from p in gb.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();
                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = getAllUsersAndRoles();

                GetMenus();
                return View(model);
            }


            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult UsernRoles(LoanViewModel lvm, FormCollection form)
        {
            try
            {

                //int rolval = Convert.ToInt16(Request.QueryString["value"]);
                //string Pageid = Request.QueryString[0];
                //string sde = Request.QueryString.AllKeys.ToString();
                //int vca = Convert.ToInt16(TempData["userid"]);
                int rolval = Convert.ToInt16(TempData["userid"]);
                if (rolval == 0)
                {

                    TempData["message"] = "Please Select A User";

                    return RedirectToAction("UsernRoles");

                }
                int roleid = Convert.ToInt16(form["roleid"]);
                int userid = Convert.ToInt16(form["id"]);
                userroles.RoleId = roleid;
                //userroles.UserId = userid;
                userroles.UserId = rolval;
                userroles.RoleId = DataReaders.selectRolesByName(userroles);
                if (userroles.RoleId != 0)
                {
                    TempData["message"] = "User Already Have This Role";
                    //ViewBag.listUser = DataReaders.getAllUser();
                    return RedirectToAction("UsernRoles");
                    //return View("UsernRoles");
                }
                else
                {
                    userroles.RoleId = roleid;
                    //userroles.UserId = userid;
                    userroles.UserId = rolval;
                    userroles.IsVissible = 1;
                    userroles.dates = DateTime.Now;

                    DataCreators.InsertUserRoles(userroles);
                    TempData["message"] = "User n Roles Created Succesfully";
                    lvm.getAllUserAndRoless = getAllUsersAndRoles();

                }
                //return View("");
                return RedirectToAction("UsernRoles");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }



        [HttpGet]
        public ActionResult PagenRoles()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                var value = "";
                string Pageid = Request.QueryString["value"];
                TempData["Pageid"] = Pageid;
                if (Pageid == "" || Pageid == null)
                {

                    Pageid = "2016";
                }

                ViewBag.listPage = DataReaders.getAllPage();

                //getAllPagesAndRoles();
                //var value = 2016;
                value = Pageid;
                pages.id = Convert.ToInt16(value);
                var pag = "";
                var selectedItem = DataReaders.getSelectedPage(pages.id);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    pages.PageName = pag;
                }

                var result = (from a in gb.PageAuthentications
                              join c in gb.Roles on a.RoleId equals c.RoleId
                              join b in gb.Pags on a.PageName equals b.PageName
                              //  where b.id == pages.id
                              where b.id == pages.id
                              select new Models.GetAssignPages { pageid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName }).ToList();


                var pageroles = DataReaders.buildPagesList(pages);
                var roles = DataReaders.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = (from p in gb.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();


                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                ViewBag.Data = getAllPagesAndRoles();
                GetMenus();
                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult PagenRoles(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                //int pagval = Convert.ToInt16(Request.QueryString["value"]);
                //string Pageid = Request.QueryString["value"];
                int pagval = Convert.ToInt16(TempData["Pageid"]);
                if (pagval == 0)
                {

                    TempData["message"] = "Please Select A Page";

                    return RedirectToAction("PagenRoles");

                }
                var pag = "";
                int roleid = Convert.ToInt16(form["RoleId"]);
                int pageid = Convert.ToInt16(form["id"]);
                // var selectedItem = DataReaders.getSelectedPage(pageid);
                var selectedItem = DataReaders.getSelectedPage(pagval);
                if (selectedItem != null)
                {
                    pag = selectedItem;

                }
                _pa.RoleId = roleid;
                _pa.PageName = pag;
                _pa.RoleId = DataReaders.selectPageRolesByName(_pa);
                if (_pa.RoleId != 0)
                {
                    TempData["message"] = "Page Already Have This Role";
                    return RedirectToAction("PagenRoles");
                }
                else
                {
                    _pa.RoleId = roleid;
                    _pa.PageName = pag;
                    _pa.isVissible = "1";


                    DataCreators.InsertPageRoles(_pa);
                    TempData["message"] = "Page Role Created Succesfully";
                    return RedirectToAction("PagenRoles");
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult EditUser(FormCollection form, int id, LoanViewModel lvm)
        {
            try
            {

                /*  var LoggedInuser = new LogginHelper();
                     user = LoggedInuser.LoggedInUser();

                     var appUser = user;
                     if (appUser == null)
                     {
                         return RedirectToAction("Index", "Home");
                   }*/
                getAllRecord();
                var user = DataReaders.getAllUser();
                if (id != 0)
                {
                    var currentuser = DataReaders.getUserProfile(id);
                    User users = new DataAccess.User();
                    ViewBag.firstname = currentuser.firstname;
                    ViewBag.lastname = currentuser.lastname;
                    ViewBag.Email = currentuser.Email;
                    ViewBag.Phone = currentuser.Phone;
                    ViewBag.id = id;
                    /* lvm.AccountsModel.Email = Convert.ToString(currentuser.Email);
                       lvm.AccountsModel.lastname =currentuser.lastname;
                       lvm.AccountsModel.Email = currentuser.Email;
                       lvm.AccountsModel.Phone = currentuser.Phone;*/

                    /*users.Email = lvm.AccountsModel.Email;
                      users.firstname = lvm.AccountsModel.firstname;
                      users.lastname = lvm.AccountsModel.lastname;
                      users.Phone = lvm.AccountsModel.Phone;*/
                    // DataCreators.UpdateUsers(users);
                }
                ViewBag.Data = user;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateUser(FormCollection form)
        {
            try
            {
                getAllRecord();
                var user = DataReaders.getAllUser();
                User users = new DataAccess.User();
                users.firstname = Convert.ToString(form["firstname"]);
                users.lastname = Convert.ToString(form["lastname"]);
                users.Phone = Convert.ToString(form["Phone"]);
                users.Email = Convert.ToString(form["Email"]).Trim();
                users.id = Convert.ToInt16(form["id"]);
                var id = DataCreators.UpdateProfiles(users);
                if (id != null)
                {
                    TempData["message"] = "User Updated Succesfully";
                    return RedirectToAction("CreateUser");
                }
                if (id == null)
                {
                    TempData["message"] = "Error Updating Data ! Please Try Again";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreatePage()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                getAllRecord();
                var pag = DataReaders.getAllPage();
                ViewBag.listPageHeader = DataReaders.getAllPageHeader();
                ViewBag.Data = pag;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreatePage(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                if (lvm != null)
                {
                    string Pagename = lvm.PageModel.PageName;
                    string PageUrl = lvm.PageModel.PageUrl;
                    int PageHeader = Convert.ToInt16(form["ids"]);
                    bool val = DataReaders.ValidatePage(Pagename);
                    if (val == true)
                    {
                        TempData["Message"] = "Page Already Exist";

                        return RedirectToAction("CreatePage");
                    }
                    else if (val == false)
                    {
                        // lvm.PageModel. = DateTime.Now;
                        lvm.PageModel.isvisible = 1;
                        lvm.PageModel.PageName = Pagename;
                        lvm.PageModel.PageUrl = PageUrl;
                        DataAccess.Pag pages = new DataAccess.Pag();
                        pages.PageName = lvm.PageModel.PageName;
                        pages.PageUrl = lvm.PageModel.PageUrl;
                        pages.isvisible = lvm.PageModel.isvisible;
                        pages.pageHeader = PageHeader;
                        int id = DataCreators.CreatePages(pages);
                        TempData["Message"] = "Page Succesful !";
                    }
                    else
                    {
                        TempData["Message"] = "Pages Not Succesful Please Try Again!";
                    }
                }
                return Redirect("CreatePage");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditPage(FormCollection form, int id, LoanViewModel lvm)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                getAllRecord();
                var role = DataReaders.getAllPage();
                if (id != 0)
                {
                    var currentuser = DataReaders.getPage(id);
                    Pag pages = new DataAccess.Pag();
                    ViewBag.Pagename = currentuser.PageName;
                    ViewBag.Pageurl = currentuser.PageUrl;
                    ViewBag.id = id;
                }
                // ViewBag.Data = user;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdatePage(FormCollection form)
        {
            try
            {
                var page = DataReaders.getAllPage();
                DataAccess.Pag pages = new DataAccess.Pag();
                pages.id = Convert.ToInt16(form["id"]);
                pages.PageName = Convert.ToString(form["pagename"]);
                pages.PageUrl = Convert.ToString(form["pageurl"]);
                var id = DataCreators.UpdatePages(pages);
                if (id != null)
                {
                    TempData["message"] = "Pages Updated Succesfully";
                    return RedirectToAction("CreatePage");
                }
                if (id == null)
                {
                    TempData["message"] = "Error Updating Data ! Please Try Again";
                }
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreateRole()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                getAllRecord();
                var users = DataReaders.getAllRole();

                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreateRole(LoanViewModel lvm)
        {
            try
            {
                if (lvm != null)
                {
                    string value = lvm.RoleModel.RoleName;


                    bool val = DataReaders.ValidateRole(value);
                    if (val == true)
                    {
                        TempData["Message"] = "Role Already Exist";

                        return RedirectToAction("CreateRole");
                    }
                    else if (val == false)
                    {

                        lvm.RoleModel.Date = DateTime.Now;
                        lvm.RoleModel.isVissible = 1;
                        lvm.RoleModel.RoleName = value;
                        DataAccess.Role roles = new DataAccess.Role();
                        roles.RoleName = lvm.RoleModel.RoleName;
                        roles.Date = lvm.RoleModel.Date;
                        roles.isVissible = lvm.RoleModel.isVissible;
                        int id = DataCreators.CreateRole(roles);
                        TempData["Message"] = "Role Succesful !";
                    }
                    else
                    {
                        TempData["Message"] = "Roles Not Succesful Please Try Again!";
                    }
                }

                return Redirect("CreateRole");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult EditRole(FormCollection form, int id, LoanViewModel lvm)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                getAllRecord();
                var role = DataReaders.getAllRole();
                if (id != 0)
                {
                    var currentuser = DataReaders.getRole(id);
                    User users = new DataAccess.User();
                    ViewBag.Rolename = currentuser.RoleName;
                    ViewBag.id = id;
                }
                ViewBag.Data = user;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpPost]
        public ActionResult UpdateRole(FormCollection form)
        {
            try
            {
                var role = DataReaders.getAllRole();
                DataAccess.Role roles = new DataAccess.Role();
                roles.RoleId = Convert.ToInt16(form["id"]);
                roles.RoleName = Convert.ToString(form["Rolename"]);

                var id = DataCreators.UpdateRole(roles);
                if (id != null)
                {
                    TempData["message"] = "Roles Updated Succesfully";
                    return RedirectToAction("CreateRole");
                }
                if (id == null)
                {
                    TempData["message"] = "Error Updating Data ! Please Try Again";
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreateUser()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                getAllRecord();
                var users = DataReaders.getAllUser();

                ViewBag.Data = users;
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreateUser(LoanViewModel lvm)
        {
            try
            {

                if (lvm != null)
                {
                    string value = lvm.AccountsModel.Email;
                    string password = lvm.AccountsModel.pasword;
                    string confirmpass = lvm.AccountsModel.confirmPassword;

                    var EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                    password = EncrypPassword;
                    lvm.AccountsModel.confirmPassword = password;
                    lvm.AccountsModel.pasword = password;
                    lvm.AccountsModel.Email = value;

                    bool val = DataReaders.Validate(value);
                    if (val == true)
                    {
                        TempData["Message"] = "User Already Exist";

                        return View("CreateUser");
                    }
                    else if (val == false)
                    {

                        lvm.AccountsModel.Date = DateTime.Now;
                        lvm.AccountsModel.DateTim = DateTime.Today;
                        lvm.AccountsModel.isVissibles = 1;

                        User users = new DataAccess.User();
                        users.confirmPassword = lvm.AccountsModel.confirmPassword;
                        users.pasword = lvm.AccountsModel.pasword;
                        users.Email = lvm.AccountsModel.Email;
                        users.Date = lvm.AccountsModel.Date;
                        users.DateTim = lvm.AccountsModel.DateTim;
                        users.isVissibles = lvm.AccountsModel.isVissibles;
                        users.firstname = lvm.AccountsModel.firstname;
                        users.lastname = lvm.AccountsModel.lastname;
                        users.Phone = lvm.AccountsModel.Phone;
                        users.Referal = lvm.AccountsModel.Referal;
                        int id = DataCreators.CreateUser(users);
                        userroles.RoleId = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultUser"]);
                        userroles.UserId = users.id;
                        userroles.IsVissible = 1;
                        DataCreators.InsertUserRoles(userroles);
                        TempData["Message"] = "Registration Succesful !";
                    }
                    else
                    {
                        TempData["Message"] = "Registration Not Succesful Please Try Again!";
                    }
                }
                return Redirect("CreateUser");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult BankUsers()
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                User users = new DataAccess.User();
                var mc = DataReaders.getUserID(appUser);
                var value = mc.id;
                users.id = Convert.ToInt16(value);
                var Bankid = DataReaders.getBank(appUser);
                ViewBag.listUser = DataReaders.getAllUserByBank(Bankid);
               // ViewBag.Data = DataReaders.getAllBankAdminUsers();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult CreateBankAdmin()
        {
            try
            {

                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                // getAllRecord();

                ViewBag.bankid = Convert.ToInt16(Request.QueryString["value"]);
                TempData["bankid"] = ViewBag.bankid;
                var Bank = DataReaders.getAllBank();

                ViewBag.listBank = Bank;
                ViewBag.Data = DataReaders.getAllBankAdminUsers();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CreateBankAdmin(LoanViewModel lvm, FormCollection form)
        {
            try
            {



                if (lvm != null)
                {
                    int bankid = Convert.ToInt16(TempData["bankid"]);
                    string value = lvm.AccountsModel.Email;
                    string password = lvm.AccountsModel.pasword;
                    string confirmpass = lvm.AccountsModel.confirmPassword;

                    var EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                    password = EncrypPassword;
                    lvm.AccountsModel.confirmPassword = password;
                    lvm.AccountsModel.pasword = password;
                    lvm.AccountsModel.Email = value;

                    bool val = DataReaders.Validate(value);
                    if (val == true)
                    {
                        TempData["Message"] = "User Already Exist";
                        var Bank = DataReaders.getAllBank();
                        ViewBag.listBank = Bank;
                        return View("CreateBankAdmin");
                    }
                    else if (val == false)
                    {

                        lvm.AccountsModel.Date = DateTime.Now;
                        lvm.AccountsModel.DateTim = DateTime.Today;
                        lvm.AccountsModel.isVissibles = 1;

                        User users = new DataAccess.User();
                        users.confirmPassword = lvm.AccountsModel.confirmPassword;
                        users.pasword = lvm.AccountsModel.pasword;
                        users.Email = lvm.AccountsModel.Email;
                        users.Date = lvm.AccountsModel.Date;
                        users.DateTim = lvm.AccountsModel.DateTim;
                        users.isVissibles = lvm.AccountsModel.isVissibles;
                        users.firstname = lvm.AccountsModel.firstname;
                        users.lastname = lvm.AccountsModel.lastname;
                        users.Phone = lvm.AccountsModel.Phone;
                        users.Referal = lvm.AccountsModel.Referal;
                        users.BankFk = bankid;
                        int id = DataCreators.CreateUser(users);

                    }
                    else
                    {
                        TempData["Message"] = "Registration Not Succesful Please Try Again!";
                    }
                }
                // Added This on 09-july-2019
                GetMenus();
                return Redirect("CreateBankAdmin");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult TransactionStatus()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null) //return View();
                    //    return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult TransactionStatus(FormCollection form)
        {
            try
            {
                string CustomerID = Convert.ToString(form["CustomerID"]);
                WebLog.Log("One" + CustomerID);
                var ReferenceNum = Convert.ToString(form["txtDatePicker1"]);
                Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                PayObj.CustomerID = CustomerID;
                // PayObj.tr
                var valid = DataReaders.GetCustomer(CustomerID, ReferenceNum);
                if (valid == false)
                {
                    TempData["message"] = "Invalid Reference OR Smartcard Number";
                    return RedirectToAction("TransactionStatus");
                }
                if (valid == true)
                {
                    Paytv _paytv = new Paytv();
                    var CustomerDetails = _paytv.TransactionStatus(ReferenceNum, CustomerID);
                    ViewBag.Data = CustomerDetails;
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                }
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult StartimesQueryBalance()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    //var LoggedInuser = new LogginHelper();
                    //user = LoggedInuser.LoggedInUser();

                    //var appUser = user;
                    //if (appUser == null) //return View();
                    //    return RedirectToAction("Index", "Home");
                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult StartimesQueryBalance(FormCollection form)
        {
            try
            {
                string CustomerID = Convert.ToString(form["CustomerID"]);
                WebLog.Log("One" + CustomerID);
                var ReferenceNum = Convert.ToString(form["txtDatePicker1"]);
                Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                PayObj.CustomerID = CustomerID;
                // PayObj.tr
                var valid = DataReaders.GetCustomer(CustomerID, ReferenceNum);
                if (valid == false)
                {
                    TempData["message"] = "Invalid Reference OR Smartcard Number";
                    return RedirectToAction("TransactionStatus");
                }
                if (valid == true)
                {
                    Paytv _paytv = new Paytv();
                    dynamic Queryval = new JObject();
                    Queryval.CustomerID = CustomerID;
                    Queryval.ReferenceNum = ReferenceNum;
                    var CustomerDetails = _paytv.GetBalance(Queryval);
                    ViewBag.Data = CustomerDetails;
                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                }
                return View();

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ReportsByMerchant(FormCollection form)
        {
            try
            {
                var MerchantFk = "";
                var type = "";
                var type1 = "";
                List<int> intList = null;
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    MerchantFk = Convert.ToString(form["Merchant"]);
                    type = Convert.ToString(form["All"]);
                    type1 = Convert.ToString(form["sucess"]);
                    if (type == null && type1 == null)
                    {
                        TempData["message"] = "Checkbox not checked";
                        GetMenus();
                        ViewBag.merchantList = _dr.getAllMerchant();
                        return View();
                    }
                    List<string> result = MerchantFk.Split(',').ToList();
                    int x = 0;
                    intList = result.Where(str => int.TryParse(str, out x)).Select(str => x).ToList();
                    if (intList == null)
                    {
                        TempData["message"] = "Please Select a merchant";
                        GetMenus();
                        ViewBag.merchantList = _dr.getAllMerchant();
                        return View();
                    }
                    type = GetType(type, type1);


                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    if (type == "00")
                    {
                        ViewBag.Merchant = (from a in gb.TransactionLogs
                                            join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                            where c.ResponseCode == type
                                            && c.TrnDate >= fromDate
                                            && c.TrnDate <= toDate
                                            && intList.Contains((int)a.Merchant_FK)
                                            orderby c.TrnDate
                                            //descending
                                            select new Paytv.Report
                                            {
                                                Amount = a.Amount.ToString(),
                                                CustomerID = a.CustomerID,
                                                CustomerName = a.CustomerName,
                                                Customer_FK = a.Customer_FK.ToString(),
                                                Merchant_FK = a.Merchant_FK.ToString(),
                                                ReferenceNumber = a.ReferenceNumber,
                                                ServiceCharge = a.ServiceCharge.ToString(),
                                                ServiceDetails = a.ServiceDetails,
                                                TransactionType = a.TransactionType.ToString(),
                                                TrnDate = a.TrnDate.ToString(),
                                                TrxToken = a.TrxToken,
                                                ValueDate = a.ValueDate,
                                                ValueTime = a.ValueTime,
                                                ResponseCode = c.ResponseCode.ToString(),
                                                Description = c.ResponseDescription,
                                            }).ToList();
                        GetMenus();
                        ViewBag.merchantList = _dr.getAllMerchant();
                        Session["AllTransaction"] = ViewBag.Merchant;
                        return View();
                    }
                }
                if (type == "*")
                {
                    ViewBag.Merchant = (from a in gb.TransactionLogs
                                        join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                        where c.TrnDate >= fromDate
                                        && c.TrnDate <= toDate
                                        && intList.Contains((int)a.Merchant_FK)
                                        orderby c.TrnDate
                                        //descending
                                        select new Paytv.Report
                                        {
                                            Amount = a.Amount.ToString(),
                                            CustomerID = a.CustomerID,
                                            CustomerName = a.CustomerName,
                                            Customer_FK = a.Customer_FK.ToString(),
                                            Merchant_FK = a.Merchant_FK.ToString(),
                                            ReferenceNumber = a.ReferenceNumber,
                                            ServiceCharge = a.ServiceCharge.ToString(),
                                            ServiceDetails = a.ServiceDetails,
                                            TransactionType = a.TransactionType.ToString(),
                                            TrnDate = a.TrnDate.ToString(),
                                            TrxToken = a.TrxToken,
                                            ValueDate = a.ValueDate,
                                            ValueTime = a.ValueTime,
                                            ResponseCode = c.ResponseCode.ToString(),
                                            Description = c.ResponseDescription,
                                        }).ToList();
                    GetMenus();
                    ViewBag.merchantList = _dr.getAllMerchant();
                    Session["AllTransaction"] = ViewBag.Merchant;
                    return View();
                }

                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult LendReportsByMerchant()
        {
            try
            {

                ViewBag.merchantList = _dr.getAllMerchant();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult LendReportsByMerchant(FormCollection form)
        {
            try
            {
                var MerchantFk = "";
                var type = "";
                var type1 = "";
                List<int> intList = null;
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    MerchantFk = Convert.ToString(form["Merchant"]);
                    type = Convert.ToString(form["All"]);
                    type1 = Convert.ToString(form["sucess"]);
                    if (type == null && type1 == null)
                    {
                        TempData["message"] = "Checkbox not checked";
                        GetMenus();
                        ViewBag.merchantList = _dr.getAllMerchant();
                        return View();
                    }
                    List<string> result = MerchantFk.Split(',').ToList();
                    int x = 0;
                    intList = result.Where(str => int.TryParse(str, out x)).Select(str => x).ToList();
                    if (intList == null)
                    {
                        TempData["message"] = "Please Select a merchant";
                        GetMenus();
                        ViewBag.merchantList = _dr.getAllMerchant();
                        return View();
                    }
                    type = GetType(type, type1);


                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    if (type == "00")
                    {
                        ViewBag.Data = (from a in gb.CustomerTransactions

                                        where a.TransactionType == 3

                                        select new Paytv.Report
                                        {
                                            Amount = a.Amount.ToString(),
                                            CustomerID = a.CustomerID,
                                            CustomerName = a.CustomerName,
                                            Customer_FK = a.Customer_FK.ToString(),
                                            Merchant_FK = a.Merchant_FK.ToString(),
                                            ReferenceNumber = a.ReferenceNumber,
                                            ServiceCharge = a.ServiceCharge.ToString(),
                                            ServiceDetails = a.ServiceDetails,
                                            TransactionType = a.TransactionType.ToString(),
                                            TrnDate = a.TrnDate.ToString(),

                                            ValueDate = a.ValueDate,
                                            ValueTime = a.ValueTime,

                                        }).ToList().Take(10);

                        Session["AllTransaction"] = ViewBag.Data;
                        GetMenus();
                    }
                }
                if (type == "*")
                {
                    ViewBag.Merchant = (from a in gb.TransactionLogs
                                        join c in gb.PaymentLogs on a.ReferenceNumber equals c.ReferenceNumber
                                        where c.TrnDate >= fromDate
                                        && c.TrnDate <= toDate
                                        && intList.Contains((int)a.Merchant_FK) && a.ReferenceNumber.Contains("PWL")
                                        orderby c.TrnDate
                                        //descending
                                        select new Paytv.Report
                                        {
                                            Amount = a.Amount.ToString(),
                                            CustomerID = a.CustomerID,
                                            CustomerName = a.CustomerName,
                                            Customer_FK = a.Customer_FK.ToString(),
                                            Merchant_FK = a.Merchant_FK.ToString(),
                                            ReferenceNumber = a.ReferenceNumber,
                                            ServiceCharge = a.ServiceCharge.ToString(),
                                            ServiceDetails = a.ServiceDetails,
                                            TransactionType = a.TransactionType.ToString(),
                                            TrnDate = a.TrnDate.ToString(),
                                            TrxToken = a.TrxToken,
                                            ValueDate = a.ValueDate,
                                            ValueTime = a.ValueTime,
                                            ResponseCode = c.ResponseCode.ToString(),
                                            Description = c.ResponseDescription,
                                        }).ToList();
                    GetMenus();
                    ViewBag.merchantList = _dr.getAllMerchant();
                    Session["AllTransaction"] = ViewBag.Merchant;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string GetType(string val, string val1)
        {
            try
            {
                string retVal = "";
                if (val != null)
                {
                    retVal = val;
                }
                if (val1 != null)
                {
                    retVal = val1;
                }
                if (retVal == "All")
                {
                    retVal = "*";
                }
                if (retVal == "sucess")
                {
                    retVal = "00";
                }
                return retVal;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }
        [HttpGet]
        public ActionResult ReportsByMerchant()
        {
            try
            {

                ViewBag.merchantList = _dr.getAllMerchant();
                GetMenus();
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult LendTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();

                    
                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    ViewBag.Data = DataReaders.LendTransaction(Userfk, from, to,Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult LendTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);


                    //ViewBag.Data = DataReaders.LendTransactionByDate(Userfk, fromDate, toDate);
                    int Flag = 0;
                    ViewBag.Data = DataReaders.LendTransaction(Userfk, fromDate, toDate, Flag);
                    //descending


                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult VendTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();

                    
                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    ViewBag.Data = DataReaders.VendTransaction(Userfk, from, to,Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult VendTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    //ViewBag.Data = DataReaders.VendTransactionByDate(Userfk, fromDate, toDate,Flag);
                    ViewBag.Data = DataReaders.VendTransaction(Userfk, fromDate, toDate, Flag);

                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }




        [HttpGet]
        public ActionResult AdminVendTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();
                    ViewBag.Partner = _dr.GetAllPatners();

                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    ViewBag.Data = DataReaders.VendTransaction(Userfk, from, to, Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult AdminVendTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var PartnerFk = Convert.ToInt32(form["Patner"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    //ViewBag.Data = DataReaders.VendTransactionByDate(Userfk, fromDate, toDate,Flag);
                    ViewBag.Data = DataReaders.VendTransaction(PartnerFk, fromDate, toDate, Flag);

                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        // Bank Dashboard


        [HttpGet]
        public ActionResult CheckTransaction()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();


                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-1);
                    Flag = 1;
                    //ViewBag.Data = DataReaders.CheckTransaction(Userfk, from, to, Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CheckTransaction(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                var custid = Convert.ToString(form["CustomerID"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                  
                    ViewBag.Data = DataReaders.CheckTransaction(Userfk,fromDate,toDate,custid);

                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult DashboardSummary()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();


                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    ViewBag.Data = DataReaders.DashboardSummary(Userfk, from, to, Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult DashboardSummary(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    //ViewBag.Data = DataReaders.VendTransactionByDate(Userfk, fromDate, toDate,Flag);
                    ViewBag.Data = DataReaders.DashboardSummary(Userfk, fromDate, toDate, Flag);

                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        [HttpGet]
        public ActionResult SaleMargin()
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                user = LoggedInuser.LoggedInUser();

                var appUser = user;
                if (appUser == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (Session["id"] != null || Session["id"].ToString() != "")
                {

                    var mc = DataReaders.getUserID(appUser);
                    id = mc.id.ToString();
                    Session["userid"] = id;
                    getAllRecord();

                    Userfk = Convert.ToInt16(id);
                    var to = DateTime.Today;
                    var from = DateTime.Today.AddDays(-100);
                    Flag = 1;
                    ViewBag.Data = DataReaders.getSaleMarginReport(Userfk,from,to,Flag);
                    Session["AllTransaction"] = ViewBag.Data;
                    GetMenus();
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
                // return View()
                //  return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult SaleMargin(FormCollection form)
        {
            try
            {
                var date = Convert.ToString(form["txtDatePicker1"]);
                var dates = Convert.ToString(form["txtDatePicker"]);
                date = date + " - " + dates;
                WebLog.Log("date" + date);
                string[] parts = date.Split(' ');
                WebLog.Log("Parts" + parts);
                var strFromDate = date.Before("-").Trim();
                WebLog.Log("strFromDate" + strFromDate);
                var strToDate = date.After("-").Trim();
                WebLog.Log("strToDate" + strToDate);

                var fromDate = DateTime.ParseExact(parts[0], "dd/MM/yyyy", new CultureInfo("en-US"));
                WebLog.Log("fromDate" + fromDate);
                var toDate = DateTime.ParseExact(parts[2], "dd/MM/yyyy", new CultureInfo("en-US")).AddDays(1);
                WebLog.Log("toDate" + toDate);
                if (Session["id"] != null || Session["id"].ToString() != "")
                {
                    WebLog.Log("Session[id]" + Session["id"].ToString());
                    var LoggedInuser = new LogginHelper();
                    WebLog.Log("LoggedInUser" + LoggedInuser);
                    user = LoggedInuser.LoggedInUser();
                    WebLog.Log("user" + user);

                    var appUser = user;
                    WebLog.Log("appUser" + appUser);
                    if (appUser == null) //return View();
                        return RedirectToAction("Index", "Home");
                    WebLog.Log("appUser" + appUser);

                    var mc = DataReaders.getUserID(appUser);
                    WebLog.Log("mc" + mc);
                    id = mc.id.ToString();
                    WebLog.Log("id" + id);
                    Session["userid"] = id;
                    WebLog.Log("Session[userid]" + mc);
                    Userfk = Convert.ToInt16(id);
                    Flag = 0;
                    ViewBag.Data = DataReaders.getSaleMarginReport(Userfk,fromDate, toDate,Flag);
                    Session["AllTransaction"] = ViewBag.Data;

                    WebLog.Log("ViewBag.Data" + ViewBag.Data);
                    GetMenus();
                    WebLog.Log("GetMenu" + GetMenus());
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }

                // return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        /*  protected bool ValidatePagePermission()
          {
              try
              {
                  users.Email = Convert.ToString(Session["id"]);

                  users.id = dataprovider.selectUserID(users);
                  var id = users.id;
                  ViewState["id"] = id;
                  var ids = (from a in paytrx.UserRoles where a.UserId == users.id select a.RoleId).ToList();

                  Session["roleid"] = ids;
                  var userFk = Convert.ToInt32(ViewState["id"]);
                  if (userFk == 0)
                  //if user has not logged in.               
                  {
                      //var shome = System.Web.Configuration.WebConfigurationManager.AppSettings["LoginPage"];
                      // Response.Redirect(shome, false);
                  }

                  var roleids = Session["roleid"];
                  var url = Request.Url;

                  string[] dspilt = url.ToString().Trim().Split('/');

                  var rawurl = dspilt[dspilt.Length - 1];
                  var isAllowed = dataprovider.GetUserPageValidation(ids.Cast<int>().ToList(), rawurl);
                  return isAllowed;
              }
              catch (Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());
                  return true;
              }
          }

      */
    }

    public class SaleMarginRep
    {
        public int id { get; set; }
        public DateTime TrasactionDate { get; set; }
        public int Transaction { get; set; }
        public double TotalLoanAmount { get; set; }
        public double SalesMargin { get; set; }
        public double SalesMarginBank { get; set; }

    }
}

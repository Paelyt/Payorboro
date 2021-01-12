using DataAccess;
using GloballendingViews.Classes;
using GloballendingViews.HelperClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace GloballendingViews.Controllers
{
    public class PaytvController : Controller
    {
        // GET: Cabletv
        DataAccess.DataCreator _dc = new DataAccess.DataCreator();
        DataAccess.DataReaders _dr = new DataAccess.DataReaders();
        Paytv _paytv = new Paytv();
        DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
        DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
        DataAccess.PaymentLog _pL = new DataAccess.PaymentLog();
        string returnMsg = "";
        string soapResult = "";
        string TELDealID = "";
        string returnCode = "000";
        string Error = "";
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        string user = "tolutl@yahoo.com";
        LogginHelper LoggedInuser = new LogginHelper();
        Utility newutil = new Utility();
        public ActionResult TestReceipt()
        {
            return View();
        }
        public ActionResult texts()
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
        public ActionResult Index()
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
        public ActionResult Indexs()
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

        [HttpPost]
        public ActionResult test(FormCollection form)
        {
            try
            {
                string val = Request.QueryString["val"];
                if (val != null)
                {
                    var satId = _dr.getSatid(val);
                    ViewBag.ServicesList = _dr.GetAllServicesBySat(satId);
                    return Json(new { Success = "true", Data = ViewBag.ServicesList });
                    //return Json(new { Success = "false" });

                }
                return View();
            }
            catch (Exception e)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult test()
        {
            try
            {
                ViewBag.Services = _dr.GetAllServices();


                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult Buy(FormCollection form)
      {
      try
      {
     string Phone = Convert.ToString(form["Phone"]);
     string Phones = "";
     string val = Request.QueryString["val"];
     if (val != null)
      {
       var satId = _dr.getSatid(val);
       ViewBag.ServicesList = _dr.GetAllServicesBySat(satId);
       return Json(new { Success = "true", Data = ViewBag.ServicesList });
        //return Json(new { Success = "false" });

       }

                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                    DateTime Dates = DateTime.Now;
                    int userid = _dr.GetUserIdByEmail(user);
                    Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                    Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
                    //dynamic PayObj = new JObject();
                    PayObj.paymentType = Convert.ToInt32(form["Recuring"]);
                    PayObj.CustomerID = Convert.ToString(form["CustomerId"]);
                    PayObj.Amount = Convert.ToString(form["Amount"]);
                    PayObj.Service = Convert.ToString(form["ServiceList"]);
                    PayObj.Bouquet = Convert.ToString(form["Services"]);
                    PayObj.Phone = Convert.ToString(form["Phone"]);
                    PayObj.Email = Convert.ToString(form["Email"]);
                    PaytvObj.Phone = PayObj.Phone;
                    PaytvObj.Email = PayObj.Email;
                    if (PayObj.Bouquet == "Antena(DTT)")
                    {
         PaytvObj.MerchantId = Convert.ToInt16(ConfigurationManager.AppSettings["DTT"]);
                    }
                 if (PayObj.Bouquet == "Satelite Dish(DTH)")
                    {
                  PaytvObj.MerchantId = Convert.ToInt16(ConfigurationManager.AppSettings["DTT"]);
                    }
                   // PaytvObj.MerchantId = 100;
                    string Amount = PayObj.Amount;
                    string CustomerID = PayObj.CustomerID;
                    PaytvObj.CustomerID = PayObj.CustomerID;
                    PaytvObj.Bouquet = PayObj.Bouquet;
                    PaytvObj.ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                    PaytvObj.Service = PayObj.Service;
                    PaytvObj.BillerImg = Convert.ToString(form["BillerName"]);
                    PaytvObj.BillerName = Convert.ToString(form["ImageUrl"]);
                    PaytvObj.Amount = Amount;
                    PaytvObj.paymentType = PayObj.paymentType;
                    ViewBag.PaytvObj = PaytvObj;
                    TempData["PaytvObj"] = PaytvObj;
                    if (Amount == "" || CustomerID == "")
                    { return View(); }
                    bool Result =newutil.ValidateNum(Amount, CustomerID);
                    if (Result == false)
                    {
                        return View();
                    }
                   
                    if (PayObj.paymentType == 0)
                    {
                        ViewBag.Services = _dr.GetAllServices();
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Payment Type!";
                        return Redirect("/Paytv/Buy/");

                    }
                    var isValid =Utility.ValidatePhoneNumber(Phone, out Phones);
                    if (isValid == false)
                    {
                        /*  ViewBag.Services = _dr.GetAllServices();
                          TempData["PaytvObj"] = PaytvObj;
                          TempData["message"] = PaytvObj.BillerName;
                          TempData["BillerName"] = PaytvObj.BillerImg;
                          TempData["Msg"] = "Check Your Smartcard Number and Try Again!";
                          return View();*/

                        ViewBag.Services = _dr.GetAllServices();
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Phone Number!";
                        return Redirect("/Paytv/Buy/");
                    }

                    if (Result == true)
                    {
                        var CustomerDetails = _paytv.GetCustomerInfo(PayObj);
                   WebLog.Log("CustomerDetails :" + CustomerDetails.ToString());
                        if (CustomerDetails == "Unable to connect to the remote server")
                        {
                            ViewBag.Services = _dr.GetAllServices();
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Internet Connection";
                            return Redirect("/Paytv/Buy/");
                            // return View();
                        }
                        if (CustomerDetails == "The operation has timed out")
                        {
                            ViewBag.Services = _dr.GetAllServices();
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Internet Connection";
                            return Redirect("/Paytv/Buy/");
                            // return View();
                        }
                        XmlDocument xd = new XmlDocument();
                        xd.LoadXml(CustomerDetails);
                        XmlTextReader reader = default(XmlTextReader);
                        XmlReader xReader = XmlReader.Create(new StringReader(CustomerDetails));
                        using (StringReader stringReader = new StringReader(CustomerDetails))
                        {
                            reader = new XmlTextReader(stringReader);
                            while (reader.Read())
                            {
                                switch (reader.NodeType)
                                {
                                    case XmlNodeType.Element:
                                        if ((reader.Name == "returnCode"))
                                        {
                                            returnCode = reader.ReadElementString();

                                        }
                                        if ((reader.Name == "returnMsg"))
                                        {
                                            returnMsg = reader.ReadElementString();
                                        }
                                        if ((reader.Name == "customerName"))
                                        {
                                            returnMsg = reader.ReadElementString();
                                            var Name = returnMsg;
                                            PaytvObj.customerName = Name;
                                        }

                                        if ((reader.Name == "transactionlNo"))
                                        {

                                            var tranNum = reader.ReadElementString();
                                            PaytvObj.transactionlNo = tranNum;
                                        }
                                        break;
                                    case XmlNodeType.Text:

                                        break;
                                    case XmlNodeType.EndElement:

                                        break;
                                }
                            }
                        }

                        if (returnCode == "0")
                        {
                           
                            _ct.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                            var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
                            if (valid == false)
                            {
                            string RefNum =  InsertTransactionLog(PaytvObj);
                                // Yes i should

                               /* _tL.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                                var valids = _dr.ValidateCusTrancLog(_tL.ReferenceNumber);
                                if (valid == false)
                                {
                                    _tL.Amount = Convert.ToDouble(PaytvObj.Amount);
                                    _tL.CustomerID = PaytvObj.CustomerID;
                                    _tL.CustomerName = PaytvObj.customerName;
                                    _tL.Customer_FK = userid;
                                    _tL.Merchant_FK = 1;
                                    _tL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["Convfee"]);
                                    _tL.TrnDate = Dates;
                                    _tL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                                    _tL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                                    _tL.ServiceDetails = PaytvObj.Service;
                                    _tL.TransactionType = 1;
                                    _tL.PaymentType = PaytvObj.paymentType;
                                    
                                }
                                _dc.InitiateTransacLog(_tL);

                                _pL.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                                var validpl = _dr.ValidatePayTrancLog(_tL.ReferenceNumber);
                                if (valid == false)
                                {
                                    _pL.CustomerPhoneNumber = PaytvObj.Phone;
                                    _pL.Amount = Convert.ToDouble(PaytvObj.Amount);
                                    _pL.CustomerID = PaytvObj.CustomerID;
                                    _pL.CustomerName = PaytvObj.customerName;

                                    _pL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["Convfee"]);
                                    _pL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                                    _pL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                                    _pL.ReferenceNumber = PaytvObj.transactionlNo;
                                    _pL.TrnDate = Dates;
                                    _pL.CustomerEmail = PaytvObj.Email;
                                    _pL.ResponseDescription = "initiated";
                                }
                                var RefNum = _pL.ReferenceNumber;
                                _dc.InitiatePaymentLog(_pL);
                                */
                                return RedirectToAction("Checkout", new { @RefNum = RefNum });
                            }
                            else
                            {
                                return RedirectToAction("Checkout");
                            }

                        }
                        if (returnCode != "0")
                        {
                            /*19 Nov 2018*/
                            ViewBag.Services = _dr.GetAllServices();
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Smartcard Number and Try Again!";
                            return Redirect("/Paytv/Buy/");
                            /*******/
                            /* ViewBag.Services = _dr.GetAllServices();
                                     return View();*/


                        }
                        if (TELDealID != "")
                        {
                            Error = returnMsg;
                            TempData["ErrorMsg"] = Error;
                            // login(SessionID);
                        }
                        else if (TELDealID == "")
                        {
                            Error = "PLEASE CHECK CONNECTION";
                            TempData["ErrorMsg"] = Error;
                        }

                    }
                }
                return View();
            }
            catch (Exception ex)
            {
                string Biller = Request.QueryString["Biller"];
                // TempData["message"] = LoadImage(Biller);
                // TempData["BillerName"] = getDiscoName(Biller);
                // ViewBag.Services = _dr.GetAllServices();
                WebLog.Log(ex.Message.ToString());
                return RedirectToAction("Buy");
            }
        }
        public string InsertTransactionLog(Paytv.PaytvObj PaytvObj)
        {
            try
            {
                string RefNum = "";
                string user = LoggedInuser.LoggedInUser();
                // i commented ou this Line
                // user = "tolutl@yahoo.com";
                if (user != null)
                {
                    //DateTime Dates = DateTime.Now;
                    DateTime Dates = Utility.getCurrentLocalDateTime();
                    int userid = _dr.GetUserIdByEmail(user);
                    _ct.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                    var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
                    _tL.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                    var valids = _dr.ValidateCusTrancLog(_tL.ReferenceNumber);
                    if (valid == false)
                    {
                    _tL.Amount = Convert.ToDouble(PaytvObj.Amount);
                    decimal Amt = Convert.ToDecimal(_tL.Amount);
                    _tL.CustomerID = PaytvObj.CustomerID;
                    _tL.CustomerName = PaytvObj.customerName;
                    _tL.Customer_FK = userid;
                    _tL.Merchant_FK = PaytvObj.MerchantId;
                    decimal ConvFee = getConvFee(_ct.ReferenceNumber,Amt);
                   _tL.ServiceCharge = Convert.ToDouble(ConvFee);
                        // _tL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["Convfee"]);
                    _tL.TrnDate = Dates;
                    _tL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                    _tL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                        _tL.ServiceDetails = PaytvObj.Service;
                        _tL.TransactionType = 1;
                        _tL.PaymentType = PaytvObj.paymentType;
                        // Added On March 20 2019
                        _tL.CustomerPhone = PaytvObj.Phone;
                        _tL.CustomerName = PaytvObj.customerName;
                    }
                    _dc.InitiateTransacLog(_tL);

                    _pL.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                    var validpl = _dr.ValidatePayTrancLog(_tL.ReferenceNumber);
                    if (valid == false)
                    {
                        _pL.CustomerPhoneNumber = PaytvObj.Phone;
                        _pL.Amount = Convert.ToDouble(PaytvObj.Amount);
                        _pL.CustomerID = PaytvObj.CustomerID;
                        _pL.CustomerName = PaytvObj.customerName;

                        _pL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["Convfee"]);
                        _pL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                        _pL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                        _pL.ReferenceNumber = PaytvObj.transactionlNo;
                        _pL.TrnDate = Dates;
                        _pL.CustomerEmail = PaytvObj.Email;
                        _pL.ResponseDescription = "initiated";
                    }
                     RefNum = _pL.ReferenceNumber;
                    _dc.InitiatePaymentLog(_pL);
                }
                return RefNum;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }
        public string getValues()
        {
            try
            {

                return null;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Receipt(dynamic Response)
        {
            try
            {
   Classes.Paytv.PaytvObj Receipt = new Classes.Paytv.PaytvObj();
   dynamic RecieptObj = new JObject();
   string RefNum = Convert.ToString(Response?.tranNum);
   var RecieptVal = _dr.GetCustReceipt(RefNum);
    if(Response?.returnCode != 0)
                {
                    // TempData["ErrMsg"] = Response?.returnMsg;
                   // TempData["ErrMsg"] = "Transaction Failed Please Contact our Customer Care";
                }
    
                Receipt.CustomerID = RecieptVal[0];
                Receipt.Amount = RecieptVal[3];
                Receipt.ConvFee = RecieptVal[2];
                Receipt.customerName = RecieptVal[1];
                Receipt.Phone = RecieptVal[5];
                Receipt.transactionlNo = RecieptVal[6];
                Receipt.Service = RecieptVal[7];
               /* if (TempData["ErrMsg"] != null)
                {
                    TempData["ErrMsg"] = "Please Contact Admin ! ";
                }*/
                return View(Receipt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
     
   public string paySubscribtion(string refNum)
        {
            try
            {
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                dynamic Phones = new JObject();
                var Phone = _dr.GetPhone(refNum);
                Phones.Phone = Phone;
                var Payresponse = _paytv.PaySubscription(payObj, Phones);
                //string refxc = "420020181112173325kaWX3";
                //var Payresponse = _dr.getXml(refxc);
               /* _ct.CusPay2response = Payresponse;
                _ct.ReferenceNumber = refNum;
                var UpdateResponse = _dc.UpdateCustomerTransac(_ct);*/
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(Payresponse);
                XmlTextReader reader = default(XmlTextReader);
                XmlReader xReader = XmlReader.Create(new StringReader(Payresponse));
                using (StringReader stringReader = new StringReader(Payresponse))
                {
                    reader = new XmlTextReader(stringReader);
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                if ((reader.Name == "returnCode"))
                                {
                                    returnCode = reader.ReadElementString();
                                    payObjx.returnCode = returnCode;
                                }
                                if ((reader.Name == "returnMsg"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    payObjx.returnMsg = returnMsg;
                                }
                                if ((reader.Name == "customerName"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    var Name = returnMsg;

                                }

                                if ((reader.Name == "transactionlNo"))
                                {

                                    var tranNum = reader.ReadElementString();
                                    payObjx.tranNum = tranNum;
                                    payObjx.Phone = Phone;

                                }
                                break;
                            case XmlNodeType.Text:

                                break;
                            case XmlNodeType.EndElement:

                                break;
                        }
                    }
                }
                var json = Convert.ToString(payObjx);
                return json;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }

        
        // test

        public string TestpaySubscribtion(string refNum)
        {
            try
            {
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                dynamic Phones = new JObject();
                var Phone = _dr.GetPhone(refNum);
                Phones.Phone = Phone;
                //var Payresponse = _paytv.PaySubscription(payObj, Phones);

                payObjx.returnCode = "00";
                payObjx.customerName = "Testing";
                payObjx.returnMsg = "Payment SuccessFull";
                payObjx.tranNum = "420045666";
                payObjx.Phone = Phone;
                var json = Convert.ToString(payObjx);
                return json;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }




        public string payDstvGotv(string refNum)
        {
            try
            {
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                dynamic Phones = new JObject();
                var Phone = _dr.GetPhone(refNum);
                Phones.Phone = Phone;
              //  payObj.Phone = Phone;
                var Payresponse = _paytv.payDstvGotv(payObj);
               
                dynamic JPayResponse = JObject.Parse(Payresponse);
                returnCode = JPayResponse?.respCode;
                payObjx.returnCode = returnCode;

                if (returnCode == "00")
                {
                    payObjx.returnCode = 0;
                }

                returnMsg = JPayResponse?.respDescription;
                payObjx.returnMsg = returnMsg;
                payObjx.respDescription = JPayResponse?.respDescription;
                payObjx.Balance = JPayResponse?.Balance;
                payObjx.Amount = JPayResponse?.Amount;
                payObjx.ServiceDescription = JPayResponse?.ServiceDescription;
                payObjx.CustomerMessage = JPayResponse?.CustomerMessage;
                var Name = JPayResponse?.CustomerName;
                payObjx.name = Name;
                payObjx.tranNum = refNum;
                payObjx.Phone = Phone;
                  
                var json = Convert.ToString(payObjx);
                return json;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }


        //public string TestpayDstvGotv(string refNum)
        //{
        //    try
        //    {
        //        dynamic payObjx = new JObject();
        //        dynamic payObj = _dr.PayLoad(refNum);
        //        dynamic Phones = new JObject();
        //        var Phone = _dr.GetPhone(refNum);
        //        Phones.Phone = Phone;
              
        //       // var Payresponse = _paytv.payDstvGotv(payObj);

        //      //  dynamic JPayResponse = JObject.Parse(Payresponse);
        //       // returnCode = JPayResponse?.respCode;
        //        payObjx.returnCode = "0";

        //       // returnMsg = JPayResponse?.CustomerMessage;
        //        payObjx.returnMsg = "Payment Succesful";
        //      //  var Name = JPayResponse?.CustomerName;
        //        payObjx.name = "Tolulope Oladipupo";
        //        payObjx.tranNum = refNum;
        //        payObjx.Phone = Phone;

        //        var json = Convert.ToString(payObjx);
        //        return json;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return "";
        //    }
        //}




        // Commented Out On 20/March/2019
        //  [HttpGet]
        // public ActionResult Buy(string Biller)
        //{
        //     try
        //     {
        //         /************* i added this to skip the paytv index page*/
        //         Biller = "1";
        //         /************ i added this to skip the paytv index page*/
        //         Classes.Paytv.PaytvObj _ptv = new Classes.Paytv.PaytvObj();
        //         if (TempData["message"] != null)
        //         {
        //             _ptv = (Paytv.PaytvObj)TempData["PaytvObj"];
        //             var a = TempData["message"];
        //             var b = TempData["BillerName"];
        //             //  TempData["Msg"] = "No Internet Connection";
        //             ViewBag.Services = _dr.GetAllServices();
        //             return View(_ptv);
        //         }
        //         string user = LoggedInuser.LoggedInUser();
        //         if (user == null || user == "")
        //         {
        //             user = LoggedInuser.getDefaultUser();
        //         }
        //         if (user != null)
        //         {
        //             // string satelite = "Antena(DTT)";
        //             //int satID = _dr.getSatid(satelite);

        //             ViewBag.ServicesList = _dr.GetAllServices();
        //             Biller = Request.QueryString["Biller"];
        //             /************* i added this to skip the paytv index page*/
        //             Biller = "100";
        //             /************ i added this to skip the paytv index page*/
        //             // bool Valid = ValidateNum();

        //             if (Biller == null)
        //             {
        //                 return Redirect("http://localhost:3346/Paytv/");

        //             }
        //             if (Biller != null || Biller != "")
        //             {
        //                 ViewBag.Services = _dr.GetAllServices();
        //                 // ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
        //                 if (ViewBag.Services == null)
        //                 {
        //                     return RedirectToAction("index", "Home");
        //                 }
        //                 TempData["message"] = LoadImage(Biller);
        //                 TempData["BillerName"] = getDiscoName(Biller);
        //                 return View();
        //              }


        //         }
        //         return View();
        //     }
        //     catch (Exception ex)
        //     {
        //         WebLog.Log(ex.Message.ToString());
        //         return null;
        //     }
        // }

        // This is the real one
        [HttpGet]
        public ActionResult Buy(string Biller)
        {
            try
            {
                /************* i added this to skip the paytv index page*/
               // Biller = "100";
               
                /************ i added this to skip the paytv index page*/
                Classes.Paytv.PaytvObj _ptv = new Classes.Paytv.PaytvObj();


                string user = LoggedInuser.LoggedInUser();
                if (user == null || user == "")
                {
                    user = LoggedInuser.getDefaultUser();
                }
                var UserRecord = new User();
                UserRecord = Utility.GetEmailAndPhone(user);
                if (TempData["message"] != null)
                {
                    _ptv = (Paytv.PaytvObj)TempData["PaytvObj"];
                    var a = TempData["message"];
                    var b = TempData["BillerName"];
                    //TempData["Msg"] = "No Internet Connection";
                    TempData["message"] = _ptv.BillerName; //LoadImage(Biller);
                    TempData["BillerName"] = _ptv.BillerImg; //getDiscoName(Biller);
                    ViewBag.Services = _dr.GetAllServices();

                    // For Email And Phone
                    if (UserRecord != null)
                    {
                        _ptv.Email = UserRecord.Email;
                        _ptv.Phone = UserRecord.Phone;
                    }
                    return View(_ptv);
                }

                if (user != null)
                {
                    // This is For Registered Users
                    UserRecord = Utility.GetEmailAndPhone(user);
                    // This is for user
                    if (UserRecord != null)
                    {
                        _ptv.Email = UserRecord.Email;
                        _ptv.Phone = UserRecord.Phone;
                    }
                    ViewBag.Services = _dr.GetAllServices();
                    // return View(_ptv);


                    string satelite = "Antena(DTT)";
                    int satID = _dr.getSatid(satelite);

                    /*  UserRecord = Utility.GetEmailAndPhone(user);
                      if (UserRecord != null)
                      {
                          _ptv.Email = UserRecord.Email;
                          _ptv.Phone = UserRecord.Phone;
                      }*/
                    ViewBag.ServicesList = _dr.GetAllServices();
                    Biller = Request.QueryString["Biller"];
                    /************* i added this to skip the paytv index page*/
                   // Biller = "100";
                    /************ i added this to skip the paytv index page*/
                    //bool Valid = ValidateNum();

                    if (Biller == null)
                    {
                          //return Redirect("http://localhost:3346/Paytv/");
                        //return Redirect("http://dev1.payorboro.com/Paytv/");
                        return Redirect("https://payorboro.com/Paytv/");
                    }
                    if (Biller != null || Biller != "")
                    {
                        ViewBag.Services = _dr.GetAllServices();
                        ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
                        if (ViewBag.Services == null)
                        {
                            return RedirectToAction("index", "Home");
                        }
                        TempData["message"] = LoadImage(Biller);
                        TempData["BillerName"] = getDiscoName(Biller);
                        return View(_ptv);


                    }

                }







                //if (user != null)
                //{
                //    string satelite = "Antena(DTT)";
                //    int satID = _dr.getSatid(satelite);

                //    UserRecord = Utility.GetEmailAndPhone(user);
                //    if (UserRecord != null)
                //    {
                //        _ptv.Email = UserRecord.Email;
                //        _ptv.Phone = UserRecord.Phone;
                //    }
                //    ViewBag.ServicesList = _dr.GetAllServices();
                //    Biller = Request.QueryString["Biller"];
                //    /************* i added this to skip the paytv index page*/
                //    Biller = "1";
                //    /************ i added this to skip the paytv index page*/
                //    //bool Valid = ValidateNum();

                //    if (Biller == null)
                //    {
                //        return Redirect("http://localhost:3346/Paytv/");

                //    }
                //    if (Biller != null || Biller != "")
                //    {
                //        ViewBag.Services = _dr.GetAllServices();
                //        ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
                //        if (ViewBag.Services == null)
                //        {
                //            return RedirectToAction("index", "Home");
                //        }
                //        TempData["message"] = LoadImage(Biller);
                //        TempData["BillerName"] = getDiscoName(Biller);
                //        return View(_ptv);


                //    }


                //}
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool ValidateNum(string Number, string Number1 = "")
        {
            try
        {
                Regex nonNumericRegex = new Regex(@"\D");
                if (nonNumericRegex.IsMatch(Number.ToString()) || nonNumericRegex.IsMatch(Number1.ToString()))
                {
                    //Contains non numeric characters.
                    return false;
                }
                return true;
         }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        public string LoadImage(string DiscoId)
        {
            try
            {

               var url = ConfigurationManager.AppSettings["ImageUrls"] + DiscoId + ".png";
                var uri = new Uri(url);
                var path = Path.GetFileName(uri.AbsolutePath);

                return url;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getDiscoName(string DiscoId)
        {
            try
            {
                string Disconame = "";
                switch (DiscoId)
                {
                    case "100":
                        Disconame = "StarTimes";
                        break;
                    case "13":
                        Disconame = "DSTV";
                        break;
                    case "14":
                        Disconame = "GOTV";
                        break;
                    case "4":
                        Disconame = "KWESE";
                        break;
                    default:
                        break;
                }
                TempData["Disconame"] = Disconame;
                return TempData["Disconame"].ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public string LoadPowerImage(string DiscoId)
        {
            try
            {

                var url = ConfigurationManager.AppSettings["PowerImageUrls"] + DiscoId + ".png";
                var uri = new Uri(url);
                var path = Path.GetFileName(uri.AbsolutePath);

                return url;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getPowerDiscoName(string DiscoId)
        {
            try
            {
                string Disconame = "";
                switch (DiscoId)
                {
                    case "1":
                        Disconame = "EKEDC";
                        break;
                    case "3":
                        Disconame = "EEDC";
                        break;
                    case "2":
                        Disconame = "IKEDC";
                        break;
                    case "8":
                        Disconame = "PHDC";
                        break;
                    case "7":
                        Disconame = "BEDC";
                        break;
                    case "4":
                        Disconame = "IEDC";
                        break;
                    default:
                        break;
                }
                TempData["Disconame"] = Disconame;
                return TempData["Disconame"].ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string getDiscoID(string DiscoName)
        {
            try
            {
                string Discoid = "";
                switch (DiscoName)
                {
                    case "StarTimes":
                        Discoid = "100";
                        break;
                    case "DSTV":
                        Discoid = "13";
                        break;
                    case "GOTV":
                        Discoid = "14";
                        break;
                    case "4":
                        Discoid = "KWESE";
                        break;
                    default:
                        break;
                }
                
                return Discoid.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult Checkout(string Biller)
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                if (user == null)
                {
                    ViewBag.Services = _dr.GetAllServices();
                    return RedirectToAction("Buy", "paytv");
                }
                if (user != null)
                {
                    
                    Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                    Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    string RefNum = Request.QueryString["RefNum"];
                    if (RefNum == null)
                    {
                        ViewBag.Services = _dr.GetAllServices();
                        return RedirectToAction("Buy", "paytv");
                    }
                    if (RefNum != null)
                    {
                       
                        // if am here;
                        //var CusTransac = _dr.GetCustTransac(RefNum);
                        var CusTransac = _dr.GetRecord(RefNum);

                        // I Added This on 14/May/2019

                        ViewBag.Wallet = GetWalletBalance((int)CusTransac.Customer_FK);

                        // I Added This 27/Nov/2018
                        paytvs.Phone = _dr.GetPhone(RefNum);
                        
                        paytvs.Amount = Convert.ToString(CusTransac.Amount);
                        
                        paytvs.CustomerID = CusTransac.CustomerID;
                        paytvs.Service = CusTransac.ServiceDetails;
                        paytvs.ConvFee = Convert.ToString(CusTransac.ServiceCharge);
                        paytvs.transactionlNo = CusTransac.ReferenceNumber;
                        paytvs.customerName = CusTransac.CustomerName;
                        paytvs.Bouquet = CusTransac.ServiceDetails;
                  paytvs.paymentType = _dr.GetCustomerpaytype(RefNum);
                        // Added on March 20 2019
                        paytvs.Phone = CusTransac.CustomerPhone;
                        string DiscoId = Convert.ToString(CusTransac.Merchant_FK);
                        if (CusTransac.Amount < 900)
                        {
                            paytvs.ConvFee = ConfigurationManager.AppSettings["NoConvFee"];
                            TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                        }

                        if (CusTransac.Amount >= 900)
                        {
                            TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                   }
                    //    TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                        paytvs.BillerName = LoadImage(DiscoId);
                        paytvs.BillerImg = getDiscoName(DiscoId);
                        TempData["message"] = paytvs.BillerName;
                        TempData["BillerName"] = paytvs.BillerImg;
                        // I Added This 27/Nov/2018
                        return View(paytvs);
                    }
                    //Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                    //Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];
                    Biller = paytvs.BillerName;

                    if (Biller == null || Biller == "")
                    {
                        return RedirectToAction("Index");
                    }
                    if (Biller != null || Biller != "")
                    {
                        // ViewBag.Services = _dr.GetAllServices();
                        // ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
                        // TempData["message"] = LoadImage(Biller);
                        TempData["message"] = paytvs.BillerName;
                        TempData["BillerName"] = paytvs.BillerImg;
                        return View(paytvs);
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


        [HttpGet]
        public ActionResult Checkouts(string Biller)
        {
            try
            {
                decimal TotalAmt = 0;
                string user = LoggedInuser.LoggedInUser();
                if (user == null)
                {
                    //ViewBag.Services = _dr.GetAllServices();
                    return RedirectToAction("Buys", "paytv");
                }
                if (user != null)
                {
                    Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                    Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    string RefNum = Request.QueryString["RefNum"];
                    if (RefNum == null)
                    {
                       // ViewBag.Services = _dr.GetAllServices();
                        return RedirectToAction("Buys", "paytv");
                    }
                    if (RefNum != null)
                    {
                        // if am here;
                        //var CusTransac = _dr.GetCustTransac(RefNum);
                        var CusTransac = _dr.GetRecord(RefNum);

                        // I Added This on 14/May/2019

                        ViewBag.Wallet = GetWalletBalance((int)CusTransac.Customer_FK);

                        // I Added This 27/Nov/2018
                        paytvs.Phone = _dr.GetPhone(RefNum);

                        paytvs.Amount = Convert.ToString(CusTransac.Amount);

                        paytvs.CustomerID = CusTransac.CustomerID;
                        paytvs.Service = CusTransac.ServiceDetails;
                        paytvs.ConvFee = Convert.ToString(CusTransac.ServiceCharge);
                        paytvs.transactionlNo = CusTransac.ReferenceNumber;
                        paytvs.customerName = CusTransac.ServiceValueDetails1;
                        paytvs.Bouquet = CusTransac.ServiceDetails;
                        paytvs.paymentType = _dr.GetCustomerpaytype(RefNum);
                        // Added on March 20 2019
                        paytvs.Phone = CusTransac.CustomerPhone;
                        paytvs.customerName = CusTransac.CustomerName;
                        string DiscoId = Convert.ToString(CusTransac.Merchant_FK);
                   decimal Amount = Convert.ToDecimal(paytvs.Amount);
                   TotalAmt = getTotalAmount(RefNum,Amount);
                        TempData["TotalAmt"] = TotalAmt;
                       /* if (CusTransac.Amount < 900)
                        {
                            paytvs.ConvFee = ConfigurationManager.AppSettings["NoConvFee"];
                            TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                        }

                        if (CusTransac.Amount >= 900)
                        {
                            TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                        }*/
                        //    TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);
                        paytvs.BillerName = LoadImage(DiscoId);
                        paytvs.BillerImg = getDiscoName(DiscoId);
                        TempData["message"] = paytvs.BillerName;
                        TempData["BillerName"] = paytvs.BillerImg;
                        // I Added This 27/Nov/2018
                        return View(paytvs);
                    }
                    //Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                    //Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];
                    Biller = paytvs.BillerName;

                    if (Biller == null || Biller == "")
                    {
                        return RedirectToAction("Index");
                    }
                    if (Biller != null || Biller != "")
                    {
                        // ViewBag.Services = _dr.GetAllServices();
                        // ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
                        // TempData["message"] = LoadImage(Biller);
                        TempData["message"] = paytvs.BillerName;
                        TempData["BillerName"] = paytvs.BillerImg;
                        return View(paytvs);
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
        public decimal getConvFee(string RefNum, decimal Amount)
        {
            try
            {
                decimal ConvFee = 0;
                if (RefNum.StartsWith("DSTV") || RefNum.StartsWith("GOTV"))
                {
                  ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["GOTVDSTVCONVFEE"]);
                   
                   return ConvFee;
                }
               else if (RefNum.StartsWith("VTU"))
                {
                    ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["NoConvFee"]);
                    return ConvFee;
                }
                else if(RefNum.StartsWith("4200"))
                {
                    if (Amount < 900)
                    {
                        ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["NoConvFee"]);
                        

                        return ConvFee;
                    }

                    if (Amount >= 900)
                    {
                        ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["ConvFee"]);
                        
                        return ConvFee;
                    }
                }
                return ConvFee;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public decimal getTotalAmount(string RefNum, decimal Amount)
        {
            try
            {
                decimal TotalAmount = 0;
                decimal ConvFee = 0;
        if (RefNum.StartsWith("DSTV") || RefNum.StartsWith("GOTV"))
                {
                    ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["GOTVDSTVCONVFEE"]);
                    TotalAmount = Amount + ConvFee;

                    return TotalAmount;
                }
                else
                {
                    if (Amount < 900)
                    {
                       ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["NoConvFee"]);
                        TotalAmount = Convert.ToDecimal(ConvFee) + Convert.ToDecimal(Amount);
                        
                        return TotalAmount;
                    }

                    if (Amount >= 900)
                    {
                        Convert.ToDouble(ConfigurationManager.AppSettings["ConvFee"]);
                        TotalAmount = Convert.ToDecimal(ConvFee) + Convert.ToDecimal(Amount);
                        return TotalAmount;
                    }
                }
                return TotalAmount;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        [HttpPost]
        public ActionResult Checkouts(Classes.Paytv.PaytvObj _pv)
        {
            try
            {
                int PayMethod = PaymentMethod(_pv.paymethod);
                string user = LoggedInuser.LoggedInUser();
                if (user != null && PayMethod == 0)
                {
             Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
             Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
             paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
             paytvs =(Classes.Paytv.PaytvObj)TempData["PaytvObj"];

                    string Firstname = _pv.customerName;
                    string Email = _pv.customerName;
                    string PhoneNumber = _pv.Phone;
                    string Amount = _pv.Amount;
                    string ConvFee = _pv.ConvFee;

                    //string ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                    var TotalAmt = Convert.ToDecimal(Amount) + Convert.ToDecimal(ConvFee);
                    string TotalAmts = TotalAmt.ToString();
                    var TranNum = _pv.transactionlNo;
                    int paymenttype = _dr.GetCustomerpaytype(TranNum);
                    int paymentplanId = _dr.GetPaymentPlanID(_pv.Bouquet);
                    bool isNum = Decimal.TryParse(TotalAmts, out TotalAmt);

                    if (isNum)
                    {
                        PaymentManager.Payment PayObj = new PaymentManager.Payment();
                        PayObj.PaymentType = paymenttype;
                        PayObj.RefNumber = TranNum; //System.DateTime.Now.ToString("yyyyMMddHmmss");
                        PayObj.amount = TotalAmt.ToString();
                        PayObj.customerid = "2";
                        PayObj.customerName = Firstname;
                        PayObj.emailaddress = Email;
                        PayObj.narration = $"{Firstname.Trim()} Payment of NGN {decimal.Parse(PayObj.amount)}";
                        PayObj.phoneNo = PhoneNumber;

                        PayObj.returnUrl = ConfigurationManager.AppSettings["PaymentReturnUrls"];
                        PayObj.PaymentPlanID = paymentplanId;
                        //PayObj.returnUrl = GetReturnUrl(PayObj.returnUrl);
                        string formObject = PaymentManager.GetPaymentPageDatails(PayObj);
                        if (formObject != "")
                        {
                            Response.Clear();
                            Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1"); Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
                            Response.Charset = "ISO-8859-1";
                            Response.Write(formObject);
                            Response.End();
                        }
                        else
                        {
                            TempData["Message"] = "Error Calling Payment Page - Code P002";
                        }

                    }
                    else
                    {

                        TempData["Message"] = "Error Calling Payment Page - Code P002";

                    }


                }
                else
                {
                    if (user != null && PayMethod == 1)
                    {
                        var userid = _dr.getUserID(user);
                        var WalletBalance = _dr.WalletBalance(userid.id);
                        double Amount = Convert.ToDouble(_pv.Amount);
                        if (WalletBalance >= Amount)
                        {
                            dynamic response = payDstvGotv(_pv.transactionlNo);
                            TempData["ErrMsg"] = "Transaction Successful!";
                            Receipt(response, _pv.transactionlNo);
                            _tL.ReferenceNumber = _pv.transactionlNo;
                            var id = _dc.InsertCustomerWallet(_tL);
                        }
                        else if (WalletBalance < Amount)
                        {
      Classes.Internetserviceprovider.InternetServiceObj _ptv = new Classes.Internetserviceprovider.InternetServiceObj();
                            _ptv.Amount = _pv.Amount;
                            _ptv.CustomerID = _pv.CustomerID;
                            _ptv.transactionlNo = _pv.transactionlNo;
                            _ptv.Service = _pv.Service;
                            _pv.ConvFee = _pv.ConvFee;
                            TempData["TotalAmt"] = _pv.Amount;
                            TempData["PaytvObj"] = _ptv;
                            TempData["Msg"] = "Low Balance In Wallet !";
                            return Redirect("/Paytv/Checkout/");
                        }
                        return RedirectToAction("WalletReceipt", new { @RefNum = _tL.ReferenceNumber });
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


        [HttpGet]
        public ActionResult WalletReceipt()
        {
            try
            {
                Classes.Paytv.Receipt Receipt = new Classes.Paytv.Receipt();
                string RefNum = Request.QueryString["RefNum"];
                if (RefNum == null)
                {

                    return RedirectToAction("BuyAirtime", "Airtime");
                }
                if (RefNum != null)
                {
                    TempData["ErrMsg"] = "Transaction successful ! Thank You";
                    var CusTransac = _dr.GetRecord(RefNum);
                 /*   Receipt.CustomerID = CusTransac.CustomerID;
                    Receipt.Amount = Convert.ToDouble(CusTransac.Amount);
                    TempData["Amount"] = Receipt.Amount;
                    Receipt.customerName = CusTransac.CustomerName;
                    Receipt.Phone = CusTransac.CustomerPhone;
                    Receipt.transactionlNo = CusTransac.ReferenceNumber;
                    Receipt.Service = CusTransac.ServiceDetails;
                    Receipt.ErrorMsg = TempData["ErrMsg"].ToString();
                    */
                    Receipt.CustomerID = CusTransac.CustomerID;
                    Receipt.Amount = Convert.ToDouble(CusTransac.Amount);
                    Receipt.ConvFee = Convert.ToString(CusTransac.ConvenienceFee);
                    //Receipt.ConvFee = "0";
                    Receipt.customerName = CusTransac.CustomerName;
                    Receipt.Phone = CusTransac.CustomerPhone;
                    Receipt.transactionlNo = CusTransac.ReferenceNumber;
                    Receipt.ServiceDetails = CusTransac.ServiceDetails;
                    TempData["Amount"] = Receipt.ConvFee + Receipt.Amount;
                    Receipt.ErrorMsg = TempData["ErrMsg"].ToString();

                }
                return View(Receipt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        [HttpPost]
        public ActionResult Checkout(Classes.Paytv.PaytvObj _pv)
        {
            try
            {
                int PayMethod = PaymentMethod(_pv.paymethod);
                string user = LoggedInuser.LoggedInUser();
                if (user != null && PayMethod == 0)
                {
                 Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                 Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                 paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];

                    string Firstname = _pv.customerName;
                    string Email = _pv.customerName;
                    string PhoneNumber = _pv.Phone;
                    string Amount = _pv.Amount;
                    string ConvFee = _pv.ConvFee;
                    
                    //string ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                    var TotalAmt = Convert.ToDecimal(Amount) + Convert.ToDecimal(ConvFee);
                    string TotalAmts = TotalAmt.ToString();
                    var TranNum = _pv.transactionlNo;
                    int paymenttype = _dr.GetCustomerpaytype(TranNum);
                    int paymentplanId = _dr.GetPaymentPlanID(_pv.Bouquet);
                    bool isNum = Decimal.TryParse(TotalAmts, out TotalAmt);

                    if (isNum)
                    {
                        PaymentManager.Payment PayObj = new PaymentManager.Payment();
                        PayObj.PaymentType = paymenttype;
                        PayObj.RefNumber = TranNum; //System.DateTime.Now.ToString("yyyyMMddHmmss");
                        PayObj.amount = TotalAmt.ToString();
                        PayObj.customerid = "2";
                        PayObj.customerName = Firstname;
                        PayObj.emailaddress = Email;
                        PayObj.narration = $"{Firstname.Trim()} Payment of NGN {decimal.Parse(PayObj.amount)}";
                        PayObj.phoneNo = PhoneNumber;

                        PayObj.returnUrl = ConfigurationManager.AppSettings["PaymentReturnUrls"];
                        PayObj.PaymentPlanID = paymentplanId;
                        //PayObj.returnUrl = GetReturnUrl(PayObj.returnUrl);
                        string formObject = PaymentManager.GetPaymentPageDatails(PayObj);
                        if (formObject != "")
                        {
                             /*string RefNum = "420020181227072633oNl8R";
                                  // I Added This today .
                            Requery(RefNum);*/
                       // Today I Added This For Requery*/
                            Response.Clear();
                            Response.ContentEncoding = Encoding.GetEncoding("ISO-8859-1"); Response.HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
                            Response.Charset = "ISO-8859-1";
                            Response.Write(formObject);
                            Response.End();
                        }
                        else
                        {
                            TempData["Message"] = "Error Calling Payment Page - Code P002";
                        }

                    }
                    else
                    {

                        TempData["Message"] = "Error Calling Payment Page - Code P002";

                    }


                }
                else
                {
                    if (user != null && PayMethod == 1)
                    {
                        var userid = _dr.getUserID(user);
                        var WalletBalance = _dr.WalletBalance(userid.id);
                        double Amount = Convert.ToDouble(_pv.Amount);
                        if (WalletBalance >= Amount)
                        {
                            dynamic response = TestpaySubscribtion(_pv.transactionlNo);
                            TempData["ErrMsg"] = "Transaction Successful!";
                            Receipt(response, _pv.transactionlNo);
                            _tL.ReferenceNumber = _pv.transactionlNo;
                            var id = _dc.InsertCustomerWallet(_tL);
                        }
                        else if (WalletBalance < Amount)
                        {
                            Classes.Internetserviceprovider.InternetServiceObj _ptv = new Classes.Internetserviceprovider.InternetServiceObj();
                            _ptv.Amount = _pv.Amount;
                            _ptv.CustomerID = _pv.CustomerID;
                            _ptv.transactionlNo = _pv.transactionlNo;
                            _ptv.Service = _pv.Service;
                            _pv.ConvFee = _pv.ConvFee;
                            TempData["TotalAmt"] = _pv.Amount;
                            TempData["PaytvObj"] = _ptv;
                            TempData["Msg"] = "Low Balance In Wallet !";
                            return Redirect("/Paytv/Checkout/");
                        }
                        return RedirectToAction("WalletReceipt", new { @RefNum = _tL.ReferenceNumber });
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


        [HttpGet]
        public ActionResult Receipt(dynamic Response, string refnum)
        {
            try
            {
                Classes.Paytv.PaytvObj Receipt = new Classes.Paytv.PaytvObj();
                //Classes.Power.Receipt Receipt = new Power.Receipt();
                dynamic RecieptObj = new JObject();
                //string RefNum = Convert.ToString(Response?.tranNum);
                //var RecieptVal = _dr.GetCustReceipt(RefNum);
                var RecieptVal = _dr.GetCustReceipt(refnum);
                if (Response?.returnCode != 1)
                {
                    //  TempData["ErrMsg"] = "Transaction Failed Please Contact our Customer Care";
                    //1st march
                    // TempData["ErrMsg"] = Response?.returnMsg;
                }
                Receipt.CustomerID = RecieptVal[0];
                Receipt.Amount = RecieptVal[3];
                Receipt.ConvFee = RecieptVal[2];
                //Receipt.ConvFee = "0";
                Receipt.customerName = RecieptVal[1];
                Receipt.Phone = RecieptVal[5];
                Receipt.transactionlNo = RecieptVal[6];
                Receipt.Service = RecieptVal[7];
                Receipt.ErrorMsg = TempData["ErrMsg"].ToString();
                
                return View(Receipt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult Requery(string RefNum)
        {
            try
           {
                //string refNum = jvalues?.data?.trxref;
                var PayRes = paySubscribtion(RefNum);
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        protected void ProcessResponse()
        {
            var trxRef = Request.QueryString["trxRef"];
            var trxToken = Request.QueryString["trxToken"];
            var SecretKey = ConfigurationManager.AppSettings["SecretKey"];
            TransactionalInformation trans;
            try
            {
                dynamic obj = new JObject();
                obj.trxRef = trxRef.Trim();
                obj.trxToken = trxToken.Trim();
                var json = obj.ToString();
                // string url = "http://www.paytrx.org/api/web/GetTransactionRecord";
                // string url = "http://paytrx.net/api/web/GetTransactionRecord";
                string url = ConfigurationManager.AppSettings["FlutterWave_Requery"];
                //re-query payment gateway for the transaction.
                var reqData = "{\"TrxRef\":\"" + trxRef + "\"" + ",\"TrxToken\":\"" + trxToken + "\"}";
                //  var url = ConfigurationManager.AppSettings["FlutterWave_Requery"];
                //var url = FlutteWaveEngine.FlutterWaveRequery;
                string serverResponse = string.Empty;

                var plainText = (trxRef + trxToken + SecretKey);

                var hash = new CryptographyManager().ComputeHash(plainText, HashName.SHA256);
                var isPaid = PaymentRequery(reqData, url, hash, out serverResponse, out trans);
                var jvalue = JObject.Parse(serverResponse);

                dynamic jvalues = JObject.Parse(serverResponse);
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() == "00" && isPaid == true)
                {
                    
                    var id = UpdateTransaction(jvalues);
                    if (id != null)
                    {
                   string refNum = jvalues?.data?.trxref;
                   var PayRes = "";
                   if (refNum.StartsWith("DSTV") || refNum.StartsWith("GOTV"))
                        {
                             PayRes = payDstvGotv(refNum);
                        }
                        else
                        {
                             PayRes = paySubscribtion(refNum);
                        }
                   dynamic Pay = JObject.Parse(PayRes);
                 if (Pay?.returnCode == 0)
                    {
                            InsertCustomerTransaction(refNum, PayRes);
                            /* var val = _dr.GetRecord(refNum);
                             int userid = Convert.ToInt16(val.Customer_FK);
                             MyUtility.insertWallet(userid,val);
                             CalculateDiscount(userid,val);
                             _ct.Merchant_FK = val.Merchant_FK;
                             _ct.ReferenceNumber = val.ReferenceNumber;
                             _ct.ServiceCharge = val.ServiceCharge;
                             _ct.ServiceDetails = val.ServiceDetails;
                             _ct.TransactionType = val.TransactionType;
                             _ct.TrnDate = val.TrnDate;
                             _ct.ValueDate = val.ValueDate;
                             _ct.ValueTime = val.ValueTime;
                             _ct.Amount = val.Amount;
                             _ct.CustomerID = val.CustomerID;
                             _ct.Customer_FK = val.Customer_FK;
                            _ct.CusPay2response = Convert.ToString(PayRes);
                            _dc.InitiateCustomerTransaction(_ct);
                           */
                            Receipt(Pay);
                 TempData["SucMsg"] = "Transaction Succesful !";
                        }
                 else if (Pay?.returnCode != 0)
                 {
                            //  TempData["ErrMsg"] = "Payment Successful But Please Contact our Customer Care";
                    
                            Receipt(Pay);
                            TempData["SucMsg"] = "Successful " + Pay?.respDescription;

                 }
                }
                }
                else
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00" || isPaid == false)
               {
                     TempData["ErrMsg"] = "Transaction Failed Please Try Again ! " + jvalues?.data?.resp_desc;
                    //TempData["ErrMsg"] = jvalues?.data?.resp_desc;
                    var id = UpdateTransaction(jvalues);
                 if (id != null)
               {
                 string refNum = jvalues?.data?.trxref;
                 dynamic payObjx = new JObject();
                 payObjx.returnCode = jvalues?.data?.resp_code;
                 payObjx.returnMsg = jvalues?.data?.resp_desc;
                 payObjx.tranNum = jvalues?.data?.trxref;
                // dynamic Pay = JObject.Parse(payObjx);
                // TempData["ErrMsg"] = "Transaction Failed Please Try Again ! ";
              
                        //Receipt(Pay);
                        Receipt(payObjx);
                        TempData["ErrMsg"] = "Transaction Failed Please Try Again ! " + jvalues?.data?.resp_desc;


                    }
                    
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //lblStatusMsg.Text = ex.Message.ToString();
            }
        }


        public string InsertCustomerTransaction(string refNum, dynamic PayRes)
        {
            try
            {
                var val = _dr.GetRecord(refNum);
                int userid = Convert.ToInt16(val.Customer_FK);
                MyUtility.insertWallet(userid, val);
                CalculateDiscount(userid, val);
                _ct.Merchant_FK = val.Merchant_FK;
                _ct.ReferenceNumber = val.ReferenceNumber;
                _ct.ServiceCharge = val.ServiceCharge;
                _ct.ServiceDetails = val.ServiceDetails;
                _ct.TransactionType = val.TransactionType;
                _ct.TrnDate = val.TrnDate;
                _ct.ValueDate = val.ValueDate;
                _ct.ValueTime = val.ValueTime;
                _ct.Amount = val.Amount;
                _ct.CustomerID = val.CustomerID;
                _ct.Customer_FK = val.Customer_FK;
                _ct.CusPay2response = Convert.ToString(PayRes);
                _dc.InitiateCustomerTransaction(_ct);

                return "";
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string UpdateTransaction(dynamic PayObj)
        {
            try
            {
                _tL.TrxToken = PayObj?.data?.trxtoken;
                _tL.ReferenceNumber = PayObj?.data?.trxref;
                _dc.UpdateTransacLog(_tL);
                _pL.PaymentReference = PayObj?.data?.payment_ref;
                _pL.TrxToken = PayObj?.data?.trxref;
                _pL.TrnDate = PayObj?.data?.updated_at;
                _pL.ResponseCode = PayObj?.data?.resp_code;
                _pL.ResponseDescription = PayObj?.data?.resp_desc;
                _pL.ReferenceNumber = PayObj?.data?.trxref;
                var id = _dc.UpdatePaymentLog(_pL);
                return id;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult GetPaymentResponse()
        {
            try
            {
                ProcessResponse();
                Classes.Paytv.Receipt Receipt = new Classes.Paytv.Receipt();
                dynamic RecieptObj = new JObject();
                if(TempData["ErrMsg"] != null)
                {
                // TempData["ErrMsg"] = "Transaction Not Succesfull. Please Contact Our Customer Care Representative ";
                }
                string RefNum = Request.QueryString["trxRef"];
                
                var RecieptVal = _dr.GetCustReceipt(RefNum);
                // I Added This today April 17
                Receipt.TrnDate = Convert.ToDateTime(RecieptVal[4]);
                Receipt.CustomerID = RecieptVal[0];
                Receipt.Amount = Convert.ToDouble(RecieptVal[3]);
                Receipt.ServiceCharge = Convert.ToDouble(RecieptVal[2]);
                Receipt.CustomerName = RecieptVal[1];
                Receipt.Phone = RecieptVal[5];
                Receipt.ReferenceNumber = RecieptVal[6];
                Receipt.ServiceDetails = RecieptVal[7];
                string Discoid = RecieptVal[8];
                string img = LoadImage(Discoid);
                string imgName = getDiscoName(Discoid);
                Double TotalAmount = Convert.ToDouble(Receipt.ServiceCharge + Receipt.Amount);
               // TempData["Amount"] = TotalAmount;
                // I Added this Line newlyy 03/April/2019
                TempData["Amount"] = MyUtility.ConvertToCurrency(TotalAmount.ToString());
                TempData["message"] = img;
                TempData["BillerName"] = imgName;
                return View(Receipt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        private static HttpWebRequest ReturnHeaderParameters(string method, string url, string hashValue, bool isPayEngine)
        {

            //if (req == null) return null;
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = method.ToUpper();
            req.Timeout = Timeout.Infinite;
            req.KeepAlive = true;

            if (isPayEngine)
            {
                //var PublicKey = "pk_8LHVMWLJDXAV8CPEZB9M2YGQ7CMMTWMKZGTTQCKATSFXZNJGGSES0GV9";
                req.Headers.Add($"PaelytAuth:{hashValue}");
                // req.Headers.Add($"PaelytAuth:{PublicKey}");
                req.Headers.Add($"PublicKey: {ConfigurationManager.AppSettings["PublicKey"]}");

                return req;
            }


            return req;
        }

        public static bool PaymentRequery(string postData, string url, string hashValue, out string serverResponse, out TransactionalInformation trans)
        {
            trans = new TransactionalInformation();
            serverResponse = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string responseString;
            try
            {
                _req = ReturnHeaderParameters("POST", url, hashValue, true);

                var data = Encoding.ASCII.GetBytes(postData);
                _req.ContentType = "application/json;charset=UTF-8";
                _req.ContentLength = data.Length;
                HttpStatusCode statusCode;
                using (var stream = _req.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                    using (_response = (HttpWebResponse)_req.GetResponse())
                    {
                        statusCode = _response.StatusCode;
                        responseString = new StreamReader(_response?.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        serverResponse = responseString;
                    }
                }
                var jvalue = JObject.Parse(responseString);
                //value to check. status code 200
                var httpStatusCode = (int)statusCode;
                if (httpStatusCode == 200)
                {
                    trans.ReturnMessage.Add((string)jvalue["data"]["resp_desc"]);
                    return trans.IsAuthenicated = ((string)jvalue["data"]["resp_code"]).Equals("00");
                }
            }
            //to read the body of the server _response when status code != 200
            catch (WebException exec)
            {
                var response = (HttpWebResponse)exec.Response;
                var dataStream = response.GetResponseStream();
                trans.ReturnMessage.Add(exec.Message);
                if (dataStream == null) return trans.IsAuthenicated;
                using (var tReader = new StreamReader(dataStream))
                {
                    responseString = tReader.ReadToEnd();
                }
                //trans.ReturnMessage.Add(exec.Message);
                var jvalue = JObject.Parse(responseString);
                // ErrorMessage error = new JavaScriptSerializer().Deserialize<ErrorMessage>(responseString);
                trans.ReturnMessage.Add((string)jvalue["data"]["resp_desc"]);
                trans.IsAuthenicated = false;
            }
            catch (Exception ex)
            {
                trans.ReturnMessage.Add(ex.Message);
                trans.IsAuthenicated = false;
            }
            return trans.IsAuthenicated;
        }


        public static bool ValidatePhoneNumber(string phoneNumber, out string validPhoneNumber)
        {
            bool _isValid = false; validPhoneNumber = ""; string myPhoneNumber = phoneNumber.Replace("+", ""); try
            {
                if (myPhoneNumber.Length < 11) { return false; }
                // myPhoneNumber.Length > 11               
                if (myPhoneNumber.Substring(0, 3) == "234" 
                    && myPhoneNumber.Length < 13)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) != "234" && myPhoneNumber.Length > 11)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) == "234" && myPhoneNumber.Length > 13)
                {
                    return false;
                }
                if (myPhoneNumber.Length == 11)
                {
                    validPhoneNumber = "234" + myPhoneNumber.Substring(1, 10);
                }
                if (myPhoneNumber.Length == 13)
                {
                    validPhoneNumber = myPhoneNumber;
                }
                _isValid = true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                _isValid = false;
            }
            return _isValid;
        }
        [HttpGet]
        public ActionResult Buys(string Biller)
        {
            try
            {
                WebLog.Log("Buys one");
                
                WebLog.Log("Biller" + Biller);
          Classes.Paytv.PaytvObj _ptv = new Classes.Paytv.PaytvObj();
                var val = Request.QueryString["Biller"];
                WebLog.Log("val1" + val);
                if (val != null )
                {
                    WebLog.Log("val2" + val);
                    JArray Resp = getPriceLists(val);
                    WebLog.Log("Resp" + Resp);
                    ViewBag.PriceList = Resp;
                    
                    if (ViewBag.PriceList == null)
                    {
                        return RedirectToAction("index", "Paytv");
                    }
                    if (Resp.Count == 1)
                    {
                        return RedirectToAction("index", "Paytv");
                    }
                 }
                string user = LoggedInuser.LoggedInUser();
                WebLog.Log("user" + user);
                if (user == null || user == "")
                {
                    WebLog.Log("user" + user);
                    user = LoggedInuser.getDefaultUser();
                    WebLog.Log("user" + user);
                }
                var UserRecord = new User();
                WebLog.Log("UserRecord" + UserRecord);
                UserRecord = Utility.GetEmailAndPhone(user);
                WebLog.Log("UserRecord" + UserRecord);
                if (TempData["message"] != null)
                {
                    WebLog.Log("Enter Here");
                    _ptv = (Paytv.PaytvObj)TempData["PaytvObj"];
                    WebLog.Log("_ptv" + _ptv);
                    var a = TempData["message"];
                    WebLog.Log("a" + a);
                    var b = TempData["BillerName"];
                    WebLog.Log("b" + b);
                    // Added Today 03/April/2019
                    TempData["message"] = _ptv.BillerName; //LoadImage(Biller);
                    TempData["BillerName"] = _ptv.BillerImg; //getDiscoName(Biller);

                    val = Convert.ToString(_ptv.MerchantId);
                    WebLog.Log("val" + val);
                    JArray Resp = getPriceLists(val);
                    WebLog.Log("Resp" + Resp);
                    ViewBag.PriceList = Resp;
                    /*  TempData["message"] = LoadImage(Biller);
                        TempData["BillerName"] = getDiscoName(Biller);
                    */
                    // For Email And Phone
                    if (UserRecord != null)
                    {
                        _ptv.Email = UserRecord.Email;
                        _ptv.Phone = UserRecord.Phone;
                        WebLog.Log("_ptvEmail" + _ptv.Email);
                        WebLog.Log("_ptvPhone" + _ptv.Phone);
                    }
                    return View(_ptv);
                }

                if (user != null)
                {
                    // This is For Registered Users
                    UserRecord = Utility.GetEmailAndPhone(user);
                    WebLog.Log("UserRecord" + UserRecord);
                    // This is for user
                    if (UserRecord != null)
                    {
                        _ptv.Email = UserRecord.Email;
                        _ptv.Phone = UserRecord.Phone;
                        WebLog.Log("ptv.Email1" + _ptv.Email);
                        WebLog.Log("ptv.Phone" + _ptv.Phone);
                    }
                   // ViewBag.Services = _dr.GetAllServices();
                    // return View(_ptv);


                 
                    Biller = Request.QueryString["Biller"];
                    WebLog.Log("Biller" + Biller);
                    /************* i added this to skip the paytv index page*/
                    // Biller = "100";
                    /************ i added this to skip the paytv index page*/
                    //bool Valid = ValidateNum();

                    if (Biller == null)
                    {
                        string Basetvurl = ConfigurationManager.AppSettings["PayTvBaseUrl"];
              return Redirect(Basetvurl);
                    }
                    if (Biller != null || Biller != "")
                    {
                       // ViewBag.Services = _dr.GetAllServices();
                        //ViewBag.ServicesList = _dr.GetAllServicesBySat(satID);
                        if (ViewBag.PriceList == null)
                        {
                            return RedirectToAction("index", "Home");
                        }
                        TempData["message"] = LoadImage(Biller);
                        WebLog.Log("TempData" + TempData["message"]);
                        TempData["BillerName"] = getDiscoName(Biller);
                        WebLog.Log("message" + TempData["BillerName"]);
                        return View(_ptv);


                    }

                }
                
                return View();

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                    return null;
            }
        }

        public JArray getPriceLists(string val)
        {
            try
            {
                dynamic Resp = new JObject();
                JArray Respons = new JArray();
                if (val != null || val != "")
                {
                    WebLog.Log("val" + val);
                    dynamic Obj = new JObject();
                    string MerchantFK = val;
                    if ( MerchantFK == "13")
                    {
                        WebLog.Log("MerchantFK" + MerchantFK);
                        Obj.MerchantFk = MerchantFK;
                        Obj.beneficiary = ConfigurationManager.AppSettings["DefaultDstv"];
                    }
                    if (MerchantFK == "14")
                    {
                        WebLog.Log("MerchantFK" + MerchantFK);
                        Obj.MerchantFk = MerchantFK;
                        Obj.beneficiary = ConfigurationManager.AppSettings["DefaultGotv"];
                    }
                    var Response = Paytv.GetPriceList(Obj);
                    WebLog.Log("Response" + val);
                    Resp = JObject.Parse(Response);
                    //Respons = JArray.Parse(Response);
                    WebLog.Log("Resp" + val);
                    //dynamic results = JObject.Parse(Response);
                   //
                    string Respp = Resp.ToString();
                    JObject PriceList = JObject.Parse(Respp);

                    string name = (string)PriceList["respCode"]; 
                    Respons = JArray.Parse(PriceList["plans"].ToString()); 
                       
                   
                    return Respons;
                }
                return Respons;
              
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Buys(string Billers, FormCollection form)
        {
           
            try
            {
                string Phone = Convert.ToString(form["Phone"]);
                string Phones = "";
                Billers = Request.QueryString["Biller"];
                string MerchantFK = "";
                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                    DateTime Dates = DateTime.Now;
                    int userid = _dr.GetUserIdByEmail(user);
                    Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                    Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
                    //dynamic PayObj = new JObject();
                    PayObj.paymentType = Convert.ToInt32(form["Recuring"]);
                    PayObj.CustomerID = Convert.ToString(form["CustomerId"]);
                    PayObj.Amount = Convert.ToString(form["Amount"]);
                    var Service = Convert.ToString(form["ServiceList"]);

                    var amount = Service.After("-").Trim();
                   // var Bouquet = Service.Before("-").Trim();
                   var Bouquet = Convert.ToString(form["ServiceList"]);
                    amount = Regex.Replace(amount, "[^.0-9]", "");
                    PayObj.Service = Bouquet;
                    PayObj.Bouquet = Bouquet;
                    //Convert.ToString(form["Services"]);
                    PayObj.Phone = Convert.ToString(form["Phone"]);
                    PayObj.Email = Convert.ToString(form["Email"]);
                    PaytvObj.Phone = PayObj.Phone;
                    PaytvObj.Email = PayObj.Email;
                    PayObj.Amount =amount;
                    
                    string Amount = PayObj.Amount;
                    string CustomerID = PayObj.CustomerID;
                    PaytvObj.CustomerID = PayObj.CustomerID;
                    PaytvObj.Bouquet = PayObj.Bouquet;
                    PaytvObj.ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                    PaytvObj.Service = PayObj.Service;
                    PaytvObj.BillerImg = Convert.ToString(form["BillerName"]);
                    PaytvObj.MerchantId=Convert.ToInt16(getDiscoID(PaytvObj.BillerImg));
                    PayObj.MerchantFk = Convert.ToString(PaytvObj.MerchantId);
                    MerchantFK = PayObj.MerchantFk;
                    PaytvObj.BillerName = Convert.ToString(form["ImageUrl"]);
                    PaytvObj.Amount = Amount;
                    PaytvObj.paymentType = PayObj.paymentType;
                    var transactionlNo = MyUtility.GenerateRefNo();
                    if (MerchantFK == "13")
                    {
                        PaytvObj.transactionlNo = "DSTV"+transactionlNo;
                    }
                    if (MerchantFK == "14")
                    {
                        PaytvObj.transactionlNo = "GOTV" + transactionlNo;
                    }
                    ViewBag.PaytvObj = PaytvObj;
                    TempData["PaytvObj"] = PaytvObj;
                    if (Amount == "" || CustomerID == "")
                    {
                        JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;
                        return View();
                    }
                    bool Result = newutil.ValidateNum(Amount, CustomerID);
                    if (Result == false)
                    {
                        JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;
                        return View();
                    }

                    if (PayObj.paymentType == 0)
                    {
                        // ViewBag.Services = _dr.GetAllServices();
                       /* JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;*/
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Payment Type!";
                        return Redirect("/Paytv/Buys/");

                    }
                    var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                    if (isValid == false)
                    {
                     //ViewBag.Services = _dr.GetAllServices();
                       /* JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;*/
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Phone Number!";
                        return Redirect("/Paytv/Buys/");
                    }

                    if (Result == true)
                    {
                        var CustomerDetails = _paytv.ValidateCableTVRequest(PayObj);
                        var paysd = PayObj.ToString();
                      
                        WebLog.Log("CustomerDetails :" + CustomerDetails.ToString());
                        dynamic JResponse = JObject.Parse(CustomerDetails);
                        if (JResponse == null)
                        {
                            // ViewBag.Services = _dr.GetAllServices();
                           /* JArray Resp = getPriceLists(MerchantFK);
                            ViewBag.PriceList = Resp;*/
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Internet Connection";
                            return Redirect("/Paytv/Buys/");
                            // return View();
                        }

                  if (JResponse != null && JResponse?.respCode == "031")
                        {
                            // ViewBag.Services = _dr.GetAllServices();
                            /* JArray Resp = getPriceLists(MerchantFK);
                             ViewBag.PriceList = Resp;*/
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = JResponse?.respDescription;
                            return Redirect("/Paytv/Buys/");
                            // return View();
                        }


                        if (JResponse != null && JResponse?.respCode == "00")
                        {

                            _ct.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                            var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
                            if (valid == false)
                            {
                                PaytvObj.customerName = JResponse?.CustomerName;
                                string RefNum = InsertTransactionLog(PaytvObj);
                               
                                return RedirectToAction("Checkouts", new { @RefNum = RefNum });
                            }
                            else
                            {
                                return RedirectToAction("Checkouts");
                            }

                        }
                        if (JResponse?.respCode != "00")
                        {
                            /*19 Nov 2018*/
                            //  ViewBag.Services = _dr.GetAllServices();
                           /* JArray Resp = getPriceLists(MerchantFK);
                            ViewBag.PriceList = Resp;*/
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Smartcard Number and Try Again!";
                            return Redirect("/Paytv/Buys/");
                           
                        }
                        if (TELDealID != "")
                        {
                            Error = returnMsg;
                            TempData["ErrorMsg"] = Error;
                            // login(SessionID);
                        }
                        else if (TELDealID == "")
                        {
                            Error = "PLEASE CHECK CONNECTION";
                            TempData["ErrorMsg"] = Error;
                        }

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

        public int PaymentMethod(string Method)
        {
            try
            {
                int value = 0;
                if (Method == "1")
                {
                    value = 1;
                }
                else
                {
                    return value;
                }

                return value;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


        public Double GetWalletBalance(int userid)
        {
            try
            {
                var WalletBalance = _dr.WalletBalance(userid);
                 if(userid == 3028)
                //if (userid == 00)
                {
                    WalletBalance = 0;
                }
                return WalletBalance;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return Convert.ToDouble(ex.StackTrace);
            }
        }

        public double CalculateDiscount(int userid, TransactionLog trxRecord)
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                if (user != null || user != "" || user != ConfigurationManager.AppSettings["PaelytEmail"])
                {
                    var DiscountAmount = 0;
                    var DiscountValue = 0;
                    MyUtility.SaveWallet(trxRecord, "Discount", userid, DiscountValue, DiscountAmount);

                }
                return 0;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }


    }
}
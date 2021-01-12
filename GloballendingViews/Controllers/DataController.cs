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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class DataController : Controller
    {
        // GET: Data
        LogginHelper LoggedInuser = new LogginHelper();
        Paytv paytv = new Paytv();
        PaytvController paytvcontroller = new PaytvController();
        Data data = new Data();
        string user = "tolutl@yahoo.com";
        DataReaders _dr = new DataReaders();
        string returnMsg = "";
        string soapResult = "";
        string TELDealID = "";
        string returnCode = "000";
        string Error = "";

        DataCreator _dc = new DataCreator();
        DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
        DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
        DataAccess.PaymentLog _pL = new DataAccess.PaymentLog();
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        Utility newutil = new Utility();


        public ActionResult Index()
        {
            return View();
        }

        public string getDiscoID(string DiscoName)
        {
            try
            {
                string Discoid = "";
                switch (DiscoName)
                {
                    case "Smile":
                        Discoid = "200";
                        break;
                    case "Airtel":
                        Discoid = "201";
                        break;
                    case "MTN":
                        Discoid = "202";
                        break;
                    case "9Mobile":
                        Discoid = "203";
                        break;
                    case "Glo":
                        Discoid = "204";
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


    
        public string getReferenceNumber(string MercFk)
        {
            try
            {
                string Prefix = "";
                switch (MercFk)
                {
                    case "200":
                        Prefix = "SDP";
                        break;
                    case "201":
                        Prefix = "ADP";
                        break;
                    case "202":
                        Prefix = "MDP";
                        break;
                    case "203":
                        Prefix = "9DP";
                        break;
                    case "204":
                        Prefix = "GDP";
                        break;
                    default:
                        break;
                }

                return Prefix;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult Buy(string Billers, FormCollection form)
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
                    Service = Service.Before("+").Trim();
                    var Bouquet = Convert.ToString(form["ServiceList"]);
                    Bouquet = Service.Before("+").Trim();
                    var amount = PayObj.Amount;
                    PayObj.Service = Bouquet;
                    PayObj.Bouquet = Bouquet;
                    //Convert.ToString(form["Services"]);
                    PayObj.Phone = Convert.ToString(form["Phone"]);
                    PayObj.Email = Convert.ToString(form["Email"]);
                    PaytvObj.Phone = PayObj.Phone;
                    PaytvObj.Email = PayObj.Email;
                    PayObj.Amount = amount;

                    string Amount = PayObj.Amount.Trim();
                    string CustomerID = PayObj.CustomerID;
                    PaytvObj.CustomerID = PayObj.CustomerID;
                    PaytvObj.Bouquet = PayObj.Bouquet;
                    PaytvObj.ConvFee = ConfigurationManager.AppSettings["SMILEConvFee"];
                    PaytvObj.Service = PayObj.Service;
                    PaytvObj.BillerImg = Convert.ToString(form["BillerName"]);
                    PaytvObj.MerchantId = Convert.ToInt16(getDiscoID(PaytvObj.BillerImg));
                    PayObj.MerchantFk = Convert.ToString(PaytvObj.MerchantId);
                    MerchantFK = PayObj.MerchantFk;
                    PaytvObj.BillerName = Convert.ToString(form["ImageUrl"]);
                    PaytvObj.Amount = Amount;
                    PaytvObj.paymentType = PayObj.paymentType;
                    var transactionlNo = MyUtility.GenerateRefNo();
                    //if (MerchantFK == "200")
                    //{
                    //    PaytvObj.transactionlNo = "SMILE" + transactionlNo;
                    //}

                    MerchantFK = getReferenceNumber(MerchantFK);
                    PaytvObj.transactionlNo = MerchantFK + transactionlNo;
                    ViewBag.PaytvObj = PaytvObj;
                    TempData["PaytvObj"] = PaytvObj;
                    if (Amount == "" || CustomerID == "")
                    {
                        JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;
                        return View();
                    }
                    bool Result = false;
                    if (PayObj.MerchantFk == "201" || PayObj.MerchantFk == "202" || PayObj.MerchantFk == "203" || PayObj.MerchantFk == "204")
                    {
                         Result = newutil.ValidateAmt(Amount);
                        TempData["ADP"] = "fix";
                    }
                    else
                    {
                        Result = newutil.ValidateNum(Amount, CustomerID);

                    }

                    if (Result == false)
                    {
                        JArray Resp = getPriceLists(MerchantFK);
                        ViewBag.PriceList = Resp;
                        return View();
                    }

                    if (PayObj.paymentType == 0)
                    {
                        
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Payment Type!";
                        return Redirect("/Data/Buy/");

                    }
                    var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                    if (isValid == false)
                    {
                       
                        TempData["PaytvObj"] = PaytvObj;
                        TempData["message"] = PaytvObj.BillerName;
                        TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Phone Number!";
                        return Redirect("/Data/Buy/");
                    }

                    if (Result == true)
                    {
                     var CustomerDetails = data.ValidateCableTVRequest(PayObj);
                     var paysd = PayObj.ToString();

           WebLog.Log("CustomerDetails :" + CustomerDetails.ToString());
                        dynamic JResponse = JObject.Parse(CustomerDetails);
                        if (JResponse == null)
                        {
                           
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = "Check Your Internet Connection";
                            return Redirect("/Data/Buy/");
                            // return View();
                        }

                        if (JResponse != null && JResponse?.respCode == "031")
                        {
                            
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = JResponse?.respDescription;
                            return Redirect("/Data/Buy/");
                            // return View();
                        }


                        if (JResponse != null && JResponse?.respCode == "00")
                        {

                            _ct.ReferenceNumber = Convert.ToString(PaytvObj.transactionlNo);
                            var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
                            if (valid == false)
                            {
                                PaytvObj.Service = Service;
                                PaytvObj.Bouquet = Bouquet;
                                PaytvObj.customerName = JResponse?.customerName;
                                string RefNum = InsertTransactionLog(PaytvObj);

                                return RedirectToAction("Checkout", new { @RefNum = RefNum });
                            }
                            else
                            {
                                return RedirectToAction("Checkout");
                            }

                        }
                        if (JResponse?.respCode != "00")
                        {
                            
                            TempData["PaytvObj"] = PaytvObj;
                            TempData["message"] = PaytvObj.BillerName;
                            TempData["BillerName"] = PaytvObj.BillerImg;
                            TempData["Msg"] = JResponse?.respDescription;
                            return Redirect("/Data/Buy/");

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
                        decimal ConvFee = getConvFee(_ct.ReferenceNumber, Amt);
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
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }

        public decimal getConvFee(string RefNum, decimal Amount)
        {
            try
            {
                decimal ConvFee = 0;
                
                
                if (RefNum.StartsWith("SMILE"))
                {
                    if (Amount < 900)
                    {
                        ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["SMILENoConvFee"]);


                        return ConvFee;
                    }

                    if (Amount >= 900)
                    {
                        ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["SMILEConvFee"]);

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

        [HttpGet]
        public ActionResult Buy(string Biller)
        {
            try
            {

                
                WebLog.Log("Buys one");
                WebLog.Log("Biller" + Biller);
                Classes.Paytv.PaytvObj _ptv = new Classes.Paytv.PaytvObj();
                var val = Request.QueryString["Biller"];
                WebLog.Log("val1" + val);
                if (val != null)
                {
                    if (val == "201" || val == "202" || val == "203" || val == "204")
                    {

                        TempData["ADP"] = "fix";
                    }
                    WebLog.Log("val2" + val);
                    JArray Resp = getPriceLists(val);
                    WebLog.Log("Resp" + Resp);
                    ViewBag.PriceList = Resp;

                    if (ViewBag.PriceList == null)
                    {
                        return RedirectToAction("index", "Data");
                    }
                    if (Resp.Count == 1)
                    {
                        return RedirectToAction("index", "Data");
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
                    
                    Biller = Request.QueryString["Biller"];
                    WebLog.Log("Biller" + Biller);
                    /************* i added this to skip the paytv index page*/
                    // Biller = "100";
                    /************ i added this to skip the paytv index page*/
                    //bool Valid = ValidateNum();

                    if (Biller == null)
                    {
                     string Basetvurl = ConfigurationManager.AppSettings["DataBaseUrl"];
                        return Redirect(Basetvurl);
                    }
                    if (Biller != null || Biller != "")
                    {
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
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string LoadImage(string DiscoId)
        {
            try
            {
               var url = ConfigurationManager.AppSettings["DataImageUrls"] + DiscoId + ".jpg";
              //var uri = new Uri(url);
              //var path = Path.GetFileName(uri.AbsolutePath);

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
                    case "200":
                        Disconame = "Smile";
                        break;
                    case "201":
                        Disconame = "Airtel";
                        break;
                    case "202":
                        Disconame = "Mtn";
                        break;
                    case "203":
                        Disconame = "9Mobile";
                        break;
                    case "204":
                        Disconame = "Glo";
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
                    var smileFk =(ConfigurationManager.AppSettings["SmileMerchantFk"]);
                    var AirtelFk = (ConfigurationManager.AppSettings["AirtelMerchantFk"]);
                    var MTNFk = (ConfigurationManager.AppSettings["MTNMerchantFk"]);
                    var NINEMobileFk = (ConfigurationManager.AppSettings["9MobileMerchantFk"]);
                    var GloMerchantFk = (ConfigurationManager.AppSettings["GloMerchantFk"]);
                    if (MerchantFK == smileFk || MerchantFK == AirtelFk || MerchantFK == MTNFk || MerchantFK == NINEMobileFk || MerchantFK == GloMerchantFk)
                    {
                     WebLog.Log("MerchantFK" + MerchantFK);
                     
                     Obj.MerchantFk = MerchantFK;
                    }
                    var Response = Data.GetPriceList(Obj);
                    WebLog.Log("Response" + val);
                    Resp = JObject.Parse(Response);
                    //Respons = JArray.Parse(Response);
                    WebLog.Log("Resp" + val);
                    //dynamic results = JObject.Parse(Response);
                    //
                    string Respp = Resp.ToString();
                    JObject PriceList = JObject.Parse(Respp);

                    string name = (string)PriceList["respCode"];
                    Respons = JArray.Parse(PriceList["BundleList"].ToString());


                    return Respons;
                }
                return Respons;

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
            decimal TotalAmt = 0;
            string user = LoggedInuser.LoggedInUser();
            if (user == null)
            {
            return RedirectToAction("Buy", "Data");
            }
                if (user != null)
                {
                 Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                 Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                 string RefNum = Request.QueryString["RefNum"];
                 if (RefNum == null)
                 {
                 
                  return RedirectToAction("Buy", "Data");
                 }
                 if (RefNum != null)
                 {
                        // if am here;
                        //var CusTransac = _dr.GetCustTransac(RefNum);
                        var CusTransac = _dr.GetRecord(RefNum);

                        // I Added This on 14/May/2019

                  ViewBag.Wallet = paytvcontroller.GetWalletBalance((int)CusTransac.Customer_FK);

                        // I Added This 27/Nov/2018
                        paytvs.Phone = _dr.GetPhone(RefNum);

                        paytvs.Amount = Convert.ToString(CusTransac.Amount);

                        paytvs.CustomerID = CusTransac.CustomerID;
                        paytvs.Service = CusTransac.ServiceDetails;
                        paytvs.ConvFee = Convert.ToString   (CusTransac.ServiceCharge);
                        paytvs.transactionlNo = CusTransac.ReferenceNumber;
                        paytvs.customerName = CusTransac.ServiceValueDetails1;
                        paytvs.Bouquet = CusTransac.ServiceDetails;
                        paytvs.paymentType = _dr.GetCustomerpaytype(RefNum);
                        // Added on March 20 2019
                        paytvs.Phone = CusTransac.CustomerPhone;
                        paytvs.customerName = CusTransac.CustomerName;
                        
                        string DiscoId = Convert.ToString (CusTransac.Merchant_FK);
                        decimal Amount = Convert.ToDecimal(paytvs.Amount);
                        TotalAmt = getTotalAmount(Amount);
                        TempData["TotalAmt"] = TotalAmt;

                        // Amount and Total Amount in string 
                        paytvs.Amounts = MyUtility.ConvertToCurrency(Amount.ToString());
                        paytvs.TotalAmt = MyUtility.ConvertToCurrency(TotalAmt.ToString());
                        TempData["TotalAmts"] = paytvs.TotalAmt;
                       // Ends Here 
                        paytvs.BillerName = LoadImage(DiscoId);
                        paytvs.BillerImg = getDiscoName(DiscoId);
                        TempData["message"] = paytvs.BillerName;
                        TempData["BillerName"] = paytvs.BillerImg;
                        // I Added This 27/Nov/2018
                        return View(paytvs);
                    }
                    
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];
                    Biller = paytvs.BillerName;

                    if (Biller == null || Biller == "")
                    {
                        return RedirectToAction("Index");
                    }
                    if (Biller != null || Biller != "")
                    {
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


        [HttpPost]
        public ActionResult Checkout(Classes.Paytv.PaytvObj _pv)
        {
            try
            {
                int PayMethod = paytvcontroller.PaymentMethod(_pv.paymethod);
                string user = LoggedInuser.LoggedInUser();
                if (user != null && PayMethod == 0)
                {
                    Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                    Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs= (Classes.Paytv.PaytvObj)TempData["PaytvObj"];

                    string Firstname = _pv.customerName;
                    string Email = _pv.customerName;
                    string PhoneNumber = _pv.Phone;
                    string Amount =Convert.ToString(_pv.Amounts).Replace(",", "").Split('.')[0].Trim();
                    //_pv.Amount;
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

      PayObj.returnUrl = ConfigurationManager.AppSettings["DataPaymentReturnUrls"];
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
                            dynamic response = payData(_pv.transactionlNo);
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
                            return Redirect("/Data/Checkout/");
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
        public ActionResult Receipt(dynamic Response)
        {
            try
            {
                Classes.Paytv.PaytvObj Receipt = new Classes.Paytv.PaytvObj();
                dynamic RecieptObj = new JObject();
                string RefNum = Convert.ToString(Response?.tranNum);
                var RecieptVal = _dr.GetCustReceipt(RefNum);
                if (Response?.returnCode != 0)
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
        public decimal getTotalAmount(decimal Amount)
        {
            try
            {
                decimal TotalAmount = 0;
                decimal ConvFee = 0;
               
               
                    if (Amount < 900)
                    {
                        ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["SMILENoConvFee"]);
                        TotalAmount = Convert.ToDecimal(ConvFee) + Convert.ToDecimal(Amount);

                        return TotalAmount;
                    }

                    if (Amount >= 900)
                    {
                      ConvFee = Convert.ToDecimal(ConfigurationManager.AppSettings["SMILEConvFee"]);
                        TotalAmount = Convert.ToDecimal(ConvFee) + Convert.ToDecimal(Amount);
                        return TotalAmount;
                    }
                
                return TotalAmount;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

         public string payData(string refNum)
          {
            try
            {
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                dynamic Phones = new JObject();
                var Phone = _dr.GetPhone(refNum);
                Phones.Phone = Phone;
                //  payObj.Phone = Phone;
                var Payresponse = data.payData(payObj);

                dynamic JPayResponse = JObject.Parse(Payresponse);
                returnCode = JPayResponse?.respCode;
                payObjx.returnCode = returnCode;

                if (returnCode == "00")
                {
                    payObjx.returnCode = 0;
                }

                returnMsg = JPayResponse?.respDescription;
               // payObjx.returnMsg = returnMsg;
                payObjx.respDescription = JPayResponse?.respDescription;
                payObjx.Status = JPayResponse?.status;
                payObjx.customerAddress = JPayResponse?.customerAddress;
                payObjx.customerPhone = JPayResponse?.customerPhone;
                payObjx.customerEmail = JPayResponse?.customerEmail;
                var Name = JPayResponse?.customerName;
                payObjx.name = Name;
                payObjx.tranNum = refNum;
                payObjx.Phone = Phone;
                payObjx.ServiceDescription = payObj.ServiceDetails;

                var json = Convert.ToString(payObjx);
                return json;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
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
                     // id = "SMILE15729531222722";
                      if (id != null)
                        {
                        string refNum = jvalues?.data?.trxref;
                       // refNum = "SMILE15729531222722";
                        var PayRes = "";
                        if (refNum != null || refNum != "")
                        {
                            PayRes = payData(refNum);
                        }
                        
                        dynamic Pay = JObject.Parse(PayRes);
                        if (Pay?.returnCode == 0)
                        {
                            InsertCustomerTransaction(refNum, PayRes);
                            
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
                if (TempData["ErrMsg"] != null)
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


    }
}
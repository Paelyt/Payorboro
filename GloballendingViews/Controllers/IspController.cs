using GloballendingViews.Classes;
using GloballendingViews.HelperClasses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace GloballendingViews.Controllers
{

    public class IspController : Controller
    {

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
        Internetserviceprovider isp = new Internetserviceprovider();
        Utility newutil = new Utility();
        string MethodName = "";
        // GET: Isp
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
        public ActionResult Buy()
        {
            try
            {
                string Biller = "6";
                TempData["message"] = LoadImage(Biller);
                TempData["BillerName"] = getDiscoName(Biller);

                Internetserviceprovider.InternetServiceObj cusdataobj =
                new Internetserviceprovider.InternetServiceObj();
                cusdataobj = (Internetserviceprovider.InternetServiceObj)TempData["cusdataobj"];
                MethodName = "planchangelist";
                var CustomerID = cusdataobj.CustomerID;

                HttpWebRequest request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                //var val = ChekMyIP();
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        soapResult = soapResult.Replace(@"\", "");
                        var soapResults = soapResult.Substring(1);
                        string sp = soapResults.Substring(0, soapResults.Length - 1);
                        JArray jObj = JArray.Parse(sp);
                        ViewBag.Data = jObj.ToList();

                        // var value = jObj[0];

                    }
                }
               
                //SoapResult = SoapResult.Replace(@"\", "");
                //JArray jObj = JArray.Parse(SoapResult);
                //ViewBag.Data = jObj;
                return View(cusdataobj);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Buy(Internetserviceprovider.InternetServiceObj cusdata,FormCollection form)
        {
            try
            {
           Internetserviceprovider.InternetServiceObj cusdataobj =
           new Internetserviceprovider.InternetServiceObj();
                string service = "";
     cusdataobj.Amount = cusdata.Amount;
     cusdataobj.ConvFee = ConfigurationManager.AppSettings["SpectranetConvFee"];
          cusdataobj.CustomerID = cusdata.CustomerID;
          cusdataobj.Phone = cusdata.Phone;
                service = Convert.ToString(form["Services"]);
                var Bouquet = service.Before("+").Trim();
                service = service.After("+").Trim();
               
                cusdataobj.Service = service;
                
                cusdataobj.customerName = cusdata.customerName;
                cusdataobj.Bouquet = Bouquet;
                cusdataobj.Email = cusdata.Email;
                cusdataobj.paymentPlanId = cusdata.paymentPlanId;
                cusdataobj.paymentType = Convert.ToInt32(form["Recuring"]);
                string Phone = cusdataobj.Phone;
                string Phones = "";
                 service = cusdataobj.Service;
                string PaymentType = Convert.ToString(cusdataobj.paymentType);

                    // For Top-Up 
                if (cusdataobj.Service == "Top-up")
                {
                    cusdataobj.Pin = cusdata.Pin;
                    cusdataobj.Voucher = cusdata.Voucher;
                    if(cusdataobj.Pin == null)
                    {
                        TempData["Msg"] = "Input Pin";
                        RedirectToAction("Buy");
                    }
                    if (cusdataobj.Voucher == null)
                    {
                        TempData["Msg"] = "Input Voucher";
                        RedirectToAction("Buy");
                    }

                }
                //For Phone Numbers
                var isValid = Utility.ValidatePhoneNumber(Phone, out Phones);
                if (isValid == false)
                {

                    TempData["cusdataobj"] = cusdataobj;
                    TempData["Msg"] = "Invalid Phone Number!";
                    return Redirect("/Isp/Buy/");
                }

                //Service Type
                if(service == "Select Service type" )
                {
                    TempData["cusdataobj"] = cusdataobj;
                    TempData["Msg"] = "Please Select Service Type!";
                    return Redirect("/Isp/Buy/");
                }
                // Payment Type
                if (PaymentType == "Select Payment type")
                {
                    TempData["cusdataobj"] = cusdataobj;
                    TempData["Msg"] = "Please Select Payment Type!";
                    return Redirect("/Isp/Buy/");
                }
                TempData["cusdataobj"] = cusdataobj;
                return RedirectToAction("Checkout");
                //return View(cusdataobj);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult Check()
        {
            try
            {
                string Biller = "6";
                TempData["message"] = LoadImage(Biller);
                TempData["BillerName"] = getDiscoName(Biller);
                string user = LoggedInuser.LoggedInUser();
                if (user == null || user == "")
                {
                    user = LoggedInuser.getDefaultUser();
                }
                if (user != null)
                {
                    if (Biller == null || Biller == "")
                    {
                        return Redirect("http://localhost:3346/");

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
        public ActionResult Check(FormCollection form)
        {
      try
       {
                Internetserviceprovider.InternetServiceObj cusdataobj =
                    new Internetserviceprovider.InternetServiceObj();
                string soapResult = "";

                  MethodName = ConfigurationManager.AppSettings["Fetchuser"];
                  var CustomerID = Convert.ToString(form["CustomerId"]);
                  Internetserviceprovider.CustomerObj ispObject = new Internetserviceprovider.CustomerObj();

                  //var CustomerID = ispObject.CustomerID;
                  Internetserviceprovider intnets = new Internetserviceprovider();
                  HttpWebRequest request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                  request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                  //var val = ChekMyIP();
                  using (WebResponse response = request.GetResponse())
                  {
                 using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                      {
          soapResult = rd.ReadToEnd();
      soapResult = soapResult.Replace(@"\", "");
     var  soapResults =
                   soapResult.Substring(1);

             string sp = soapResults.Substring(0, soapResults.Length - 1);
                         // JArray jObj = JArray.Parse(sp);


                        // var value = jObj[0];
                       // soapResult = soapResult.Replace(@"\", "");
                        dynamic jobj = JObject.Parse(sp);
                        if (jobj != null && $"{jobj?.soapenvBody?.ns1processResponse?.processReturn?.errorCode}".ToLower() == "0")
                        {
                   var soapObj = jobj?.soapenvBody?.ns1processResponse?.processReturn?.keyParam?.keyParam;
                            var lastname = "";
                            var firstname = "";
                            foreach (var item in soapObj)
                            {
                                var keys = item.key;
                                //if(keys == "lastTotalTime")
                                //{
                                //    var keyvalue = item.value;
                                //}
                                if (keys == "userName")
                                {
                                    cusdataobj.userName = item.value;
                                    var value = item.value;
                                }
                                if (keys == "lastName")
                                {
                                    lastname = item.value;
                                    //var value = item.value;
                                }
                                if (keys == "email")
                                {
                                    cusdataobj.Email = item.value;
                                    var value = item.value;
                                }
                                if (keys == "userId")
                                {
                                    cusdataobj.CustomerID = item.value;
                                }
                                if (keys == "firstName")
                                {
                                    firstname = item.value;
                                    //var value = item.value;
                                }
                                cusdataobj.customerName = firstname + lastname;
                                TempData["cusdataobj"] = cusdataobj;

                                //return RedirectToAction("");
                            }
                            return RedirectToAction("Buy");
                        }
                       else if (jobj == null)
                        {
                            TempData["message"] = "Please Try Again";
                            return RedirectToAction("Check");
                        }
                        else if (jobj != null && $"{jobj?.soapenvBody?.ns1processResponse?.processReturn?.errorCode}".ToLower() == "-103")
                        {
                      TempData["Msg"] = $"{jobj?.soapenvBody?.ns1processResponse?.processReturn?.errorMessage}".ToLower();
                        return RedirectToAction("Check");
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


        public string ChekMyIP()
        {
            return GetIPAddress();
        }
        protected string GetIPAddress()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (!string.IsNullOrEmpty(ipAddress))
            {
                string[] addresses = ipAddress.Split(',');
                if (addresses.Length != 0)
                {
                    return addresses[0];
                }
            }

            return context.Request.ServerVariables["REMOTE_ADDR"];
        }

        [HttpGet]
        public ActionResult Checkout()
        {
            try
           {
                string Biller = "6";
                TempData["message"] = LoadImage(Biller);
                TempData["BillerName"] = getDiscoName(Biller);
                Internetserviceprovider.InternetServiceObj cusdataobj =
               new Internetserviceprovider.InternetServiceObj();
           cusdataobj = (Internetserviceprovider.InternetServiceObj)TempData["cusdataobj"];
            double TotalAmt = Convert.ToDouble(cusdataobj.Amount) + Convert.ToDouble(cusdataobj.ConvFee);
                TempData["TotalAmt"] = TotalAmt;
                cusdataobj.transactionlNo = isp.SerialNum();
                InsertTransactionLog(cusdataobj);
                return View(cusdataobj);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Checkout(FormCollection form, Internetserviceprovider.InternetServiceObj cusdata)
        {
            try
            {
                Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];

                string Firstname = cusdata.customerName;
                string Email = cusdata.customerName;
                string PhoneNumber = cusdata.Phone;
                string Amount = cusdata.Amount;
                string ConvFee = cusdata.ConvFee;
                string RefNum = cusdata.transactionlNo;
                cusdata.transactionlNo = RefNum;
                //string ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                var TotalAmt = Convert.ToDecimal(Amount) + Convert.ToDecimal(ConvFee);
                string TotalAmts = TotalAmt.ToString();
                var TranNum = cusdata.transactionlNo;
                // int paymenttype = _dr.GetCustomerpaytype(TranNum);
                int paymenttype = 1;
                // int paymentplanId = _dr.GetPaymentPlanID(_pv.Bouquet);
                int paymentplanId = 2;
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

                    PayObj.returnUrl = ConfigurationManager.AppSettings["SpecPaymentReturnUrls"];
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


            
                return View();
            }
            catch (Exception ex)
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
                       // var Customer = _dr.GetRecord(refNum);
                        var PayRes = paySubscribtion(refNum);
                        dynamic Pay = JObject.Parse(PayRes);
                        if (Pay?.returnCode == 0)
                        {
                            var val = _dr.GetRecord(refNum);
                            // For Wallet
                            int userid = Convert.ToInt16(val.Customer_FK);
                            MyUtility.insertWallet(userid, val);
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

                            Receipt(Pay);

                        }
                        else if (Pay?.returnCode != 0)
                        {
                            Receipt(Pay);
                            // TempData["ErrMsg"] = "Please Contact our Customer Care";
                        }
                    }
                }
                else
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00" || isPaid == false)
                {
                    var id = UpdateTransaction(jvalues);
                    if (id != null)
                    {
                        string refNum = jvalues?.data?.trxref;
                        dynamic payObjx = new JObject();
                        payObjx.returnCode = jvalues?.data?.resp_code;
                        payObjx.returnMsg = jvalues?.data?.resp_desc;
                        payObjx.tranNum = jvalues?.data?.trxref;
                        dynamic Pay = JObject.Parse(payObjx);
                        Receipt(Pay);
                       // TempData["ErrMsg"] = "Please Contact our Customer Care";
                     }

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //lblStatusMsg.Text = ex.Message.ToString();
            }
        }


        public string InsertTransactionLog(Internetserviceprovider.InternetServiceObj IspObj)
        {
            try
            {
                string RefNum = "";
                //string user = LoggedInuser.LoggedInUser();
                //if (user != null)
                //{
                IspObj.transactionlNo = IspObj.transactionlNo;
                    DateTime Dates = DateTime.Now;
                // int userid = _dr.GetUserIdByEmail(user);
                int userid = 001;
                    _ct.ReferenceNumber = Convert.ToString(IspObj.transactionlNo);
                    var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
                    _tL.ReferenceNumber = Convert.ToString(IspObj.transactionlNo);
                    var valids = _dr.ValidateCusTrancLog(_tL.ReferenceNumber);
                    if (valid == false)
                    {
                        _tL.Amount = Convert.ToDouble(IspObj.Amount);
                        _tL.CustomerID = IspObj.CustomerID;
                        _tL.CustomerName = IspObj.customerName;
                        _tL.Customer_FK = userid;
                        _tL.Merchant_FK = 1;
                        _tL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["SpectranetConvFee"]);
                        _tL.TrnDate = Dates;
                        _tL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                        _tL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                        _tL.ServiceDetails = IspObj.Service;
                        _tL.TransactionType = 1;
                        _tL.PaymentType = IspObj.paymentType;
                    _tL.Pin = IspObj.Pin;
                    _tL.Voucher = IspObj.Voucher;
                    _tL.ServiceCode = IspObj.Bouquet;
                    }
                    _dc.InitiateTransacLog(_tL);

                    _pL.ReferenceNumber = Convert.ToString(IspObj.transactionlNo);
                    var validpl = _dr.ValidatePayTrancLog(_tL.ReferenceNumber);
                    if (valid == false)
                    {
                        _pL.CustomerPhoneNumber = IspObj.Phone;
                        _pL.Amount = Convert.ToDouble(IspObj.Amount);
                        _pL.CustomerID = IspObj.CustomerID;
                        _pL.CustomerName = IspObj.customerName;

                        _pL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["SpectranetConvFee"]);
                        _pL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                        _pL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                        _pL.ReferenceNumber = IspObj.transactionlNo;
                        _pL.TrnDate = Dates;
                        _pL.CustomerEmail = IspObj.Email;
                        _pL.ResponseDescription = "initiated";
                   // }
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

        public string paySubscribtion(string refNum)
        {
            try
            {
                string resp = "";
                var Customer = _dr.GetRecord(refNum);
                if (Customer.ServiceDetails == "Renew Plan")
                {
                    resp = RenewPlan(Customer.CustomerID);
                   
                }else   if (Customer.ServiceDetails == "Top-up")
                    {
                        resp = Topup(Customer.CustomerID,  Customer.Voucher, Customer.Pin);
                    }
                else 
                {
                   resp = ChangePlan(Customer.CustomerID, Customer.ServiceCode);
                }
               
                DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
              

                dynamic jobj = JObject.Parse(resp);
                var transactionId = jobj?.soapenvBody?.ns1processResponse?.processReturn?.keyParam?.keyParam;
                var ErrorCode =$"{jobj?.soapenvBody?.ns1processResponse?.processReturn?.errorCode}".ToLower();
                var value = transactionId[0].value;
                _tL.PatnerRefNumber = value;
                _tL.ReferenceNumber = refNum;
                _tL.PatnerResponseCode = ErrorCode;
                _dc.updateTransactionLog(_tL);
             
                return resp.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
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
                    TempData["ErrMsg"] = "transaction Not Succesfull. Please Contact Our Customer Care Representative ";
                }
                string RefNum = Request.QueryString["trxRef"];

                var RecieptVal = _dr.GetCustReceipt(RefNum);

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
                TempData["Amount"] = TotalAmount;
                TempData["message"] = img;
                TempData["BillerName"] = imgName;
                // I Added This One

                string Biller = "6";
                TempData["message"] = LoadImage(Biller);
                TempData["BillerName"] = getDiscoName(Biller);
                return View(Receipt);
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
                    TempData["ErrMsg"] = Response?.returnMsg;
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
                    case "1":
                        Disconame = "StarTimes";
                        break;
                    case "2":
                        Disconame = "DSTV";
                        break;
                    case "3":
                        Disconame = "GOTV";
                        break;
                    case "4":
                        Disconame = "KWESE";
                        break;
                    case "6":
                        Disconame = "Spectranet";
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

        public static HttpWebRequest CreateWebRequest(string UserID)
        {
            //string baseurl = ConfigurationManager.AppSettings["EKEDC_Endpoint"] + MethodName;
            //WebLog.Log("Base Url: " + baseurl);
            string baseurl = System.Configuration.ConfigurationManager.AppSettings["SpectranetKnowYourOffer"];

            baseurl = baseurl.Replace("{$UserID}", UserID.ToString()).Trim();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.ContentType = "application/json";
            //webRequest.Headers.Add(@"SOAP:Action");
            //webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            //webRequest.Accept = "text/xml";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webRequest.Method = "GET";
            webRequest.Credentials = new NetworkCredential("admin", "1234");

            return webRequest;
        }


        public string RenewPlan(string UserID)
        {
            try
            {
                string soapObj = "";
                MethodName = "renewplan";
                var CustomerID = UserID;

                HttpWebRequest request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName);
                //var val = ChekMyIP();
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        soapResult = soapResult.Replace(@"\", "");
                        var soapResults = soapResult.Substring(1);
         string sp = soapResults.Substring(0, soapResults.Length - 1);
                       // JArray jObj = JArray.Parse(sp);
                        dynamic jObj = JObject.Parse(sp);
                        // ViewBag.Data = jObj.ToList();
                        soapObj = jObj?.soapenvBody?.ns1processResponse?.processReturn?.errorCode;
                    }
                }
                 return soapObj ;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string ChangePlan(string UserID,string plan)
        {
            try
            {
                    //453
                    MethodName = plan;
                    var CustomerID = UserID;
                var responses = "";
                    HttpWebRequest request = Internetserviceprovider.CreateWebReq(CustomerID, MethodName);
                    request = Internetserviceprovider.CreateWebReq(CustomerID, MethodName);
                    //var val = ChekMyIP();
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                        {
                            soapResult = rd.ReadToEnd();
                            soapResult = soapResult.Replace(@"\", "");
                            var soapResults = soapResult.Substring(1);
                            string sp = soapResults.Substring(0, soapResults.Length - 1);
                        //JArray jObj = JArray.Parse(sp);
                        var jObj = JObject.Parse(sp);
                        responses = jObj.ToString();
                        }
                    }


                    return responses;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string Topup(string UserID,string voucher, string pin)
        {
            try
            {
                string soapObj = "";
                MethodName = "refillaccountwithrcv";
                var CustomerID = UserID;
               
                HttpWebRequest request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName,voucher, pin);
                request = Internetserviceprovider.CreateWebRequest(CustomerID, MethodName, voucher, pin);
                //var val = ChekMyIP();
                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        soapResult = soapResult.Replace(@"\", "");
                        var soapResults = soapResult.Substring(1);
                        string sp = soapResults.Substring(0, soapResults.Length - 1);
                        // JArray jObj = JArray.Parse(sp);
                        dynamic jObj = JObject.Parse(sp);
                        // ViewBag.Data = jObj.ToList();
                        soapObj = jObj?.soapenvBody?.ns1processResponse?.processReturn?.errorCode;
                    }
                }
                return soapObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
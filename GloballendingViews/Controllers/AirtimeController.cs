using DataAccess;
using DocumentFormat.OpenXml.Drawing;
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
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class AirtimeController : Controller
    {
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        PaytvController paytvcontroller = new PaytvController();
        Internetserviceprovider.InternetServiceObj AirtimeObj = new Internetserviceprovider.InternetServiceObj();
        string token = "", message = "", vice_id = "";
        DataAccess.DataCreator _dc = new DataAccess.DataCreator();
        DataAccess.DataReaders _dr = new DataAccess.DataReaders();
        Paytv _paytv = new Paytv();
        DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
        DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
        DataAccess.PaymentLog _pL = new DataAccess.PaymentLog();
        Utility newutil = new Utility();
        LogginHelper LoggedInuser = new LogginHelper();
       
        // GET: Airtime
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult BuyAirtime()
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                if (user == null || user == "")
                {
                    user = LoggedInuser.getDefaultUser();
                }
                if (user != null)
                {
                    Classes.Internetserviceprovider.InternetServiceObj _ptv = new Classes.Internetserviceprovider.InternetServiceObj();
                    // if (TempData["message"] != null)
                    if (TempData["PaytvObj"] != null)
                    {
      _ptv = (Internetserviceprovider.InternetServiceObj)TempData["PaytvObj"];
                     var a = TempData["message"];
                     var b = TempData["BillerName"];
                     //TempData["Msg"] = "No Internet Connection";

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

        public int getmerchantID(string services)
        {
            try
            {
                int MerchantID = 0;
                if(services == "MTNVTU")
                {
                   return MerchantID = 301;
                }
                if (services == "GLOVTU")
                {
                    MerchantID = 302;
                }
                if (services == "ETISALATVTU")
                {
                    MerchantID = 303;
                }
                if (services == "AIRTELVTU")
                {
                    MerchantID = 304;
                }
               
                return MerchantID;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        [HttpPost]
        public ActionResult BuyAirtime(FormCollection form, Internetserviceprovider.InternetServiceObj AirtimeObj)
        {
            try
            {
                dynamic Jobj = new JObject();
                string Paymenttype = "";
                Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
                // i added this line today for Validation 
                Classes.Internetserviceprovider.InternetServiceObj _ptv = new Classes.Internetserviceprovider.InternetServiceObj();
                Classes.Internetserviceprovider.InternetServiceObj Airtime = new Classes.Internetserviceprovider.InternetServiceObj();
                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                    

                   // dynamic Airtime = new JObject();
                   
                    Airtime.Amount = AirtimeObj.Amount;
                    Airtime.Phone = AirtimeObj.CustomerID;
                    Airtime.CustomerID = AirtimeObj.CustomerID;

                    Paymenttype = Convert.ToString(form["paymenttype"]);
                    Airtime.paymentType = Convert.ToInt16(Paymenttype);
                   var service = Convert.ToString(form["Recuring"]);
                    if (service.StartsWith("Select"))
                    {
                        TempData["PaytvObj"] = Airtime;
                        //TempData["message"] = PaytvObj.BillerName;
                        //TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Please Select Airtime !";
                        return Redirect("/Airtime/BuyAirtime/");
                    }
                    service = service.After("Airtime/").Trim();
                    service = service.Before(".").Trim();
                    Airtime.Service = service;
                    AirtimeObj.Service = Airtime.Service;
                    string services = AirtimeObj.Service;


                    string amount = Airtime.Amount;
                    string phone = Airtime.Phone;
                   
                    if (amount == "" || phone == "")
                    { return View(); }
                    bool Result = newutil.ValidateNum(amount, phone);
                    if (Result == false)
                    {
                        return View();
                    }
                    string Phones = "";
                    bool ValidAmount = newutil.ValidateAmount(amount);
                    if (ValidAmount == false)
                    {
                        TempData["PaytvObj"] = Airtime;
                        // TempData["message"] = PaytvObj.BillerName;
                        //TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Amount Must Be Than N50!";
                        return Redirect("/Airtime/BuyAirtime/");
                    }
                    var isValid = Utility.ValidatePhoneNumber(phone, out Phones);
                    if (isValid == false)
                    {
                        TempData["PaytvObj"] = Airtime;
                        // TempData["message"] = PaytvObj.BillerName;
                        //TempData["BillerName"] = PaytvObj.BillerImg;
                        TempData["Msg"] = "Invalid Phone Number!";
                        return Redirect("/Airtime/BuyAirtime/");
                    }

                    PaytvObj.Amount = AirtimeObj.Amount;
                    PaytvObj.CustomerID = AirtimeObj.CustomerID;
                    PaytvObj.Service = AirtimeObj.Service;
                    PaytvObj.transactionlNo = Utility.GenerateAlphanumericUniqueId(30);
                    PaytvObj.transactionlNo = "VTU" + PaytvObj.transactionlNo;
                    PaytvObj.Phone = AirtimeObj.Phone;
                    PaytvObj.ConvFee = ConfigurationManager.AppSettings["payviceConvFee"];
                    PaytvObj.paymentType = Convert.ToInt16(Paymenttype);
                    PaytvObj.MerchantId = getmerchantID(services);
                   // PaytvObj.MerchantId = 300;
                    // var RefNum = PaytvObj.transactionlNo;

                    string RefNum = paytvcontroller.InsertTransactionLog(PaytvObj);
                    return RedirectToAction("Checkout", new { @RefNum = RefNum });
                    //return View(PaytvObj);
                    //   }


                    // }
                    // return View();
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
                
                int PayMethod = PaymentMethod(_pv.paymethod);
                
                string user = LoggedInuser.LoggedInUser();
                if (user != null && PayMethod == 0)
                {
                   
                   Classes.Paytv.PaytvObj paytv = new Paytv.PaytvObj();
                   Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];

                    string Firstname = _pv.CustomerID;
                    string Email = _pv.CustomerID;
                    string PhoneNumber = _pv.CustomerID;
                    string Amount = _pv.Amount;
                    string ConvFee = _pv.ConvFee;

                    //string ConvFee = ConfigurationManager.AppSettings["ConvFee"];
                    var TotalAmt = Convert.ToDecimal(Amount) + Convert.ToDecimal(ConvFee);
                    string TotalAmts = TotalAmt.ToString();
                    var TranNum = _pv.transactionlNo;
                    // i have to uncomment out this Line
                    int paymenttype = _dr.GetCustomerpaytype(TranNum);
                    // int paymenttype = 1;
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

                        PayObj.returnUrl = ConfigurationManager.AppSettings["PavicePaymentReturnUrl"];
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
                dynamic response = paySubscribtion(_pv.transactionlNo);
                TempData["ErrMsg"] = "Transaction Successful!";
                Receipt(response, _pv.transactionlNo);
                _tL.ReferenceNumber = _pv.transactionlNo;
                var id = _dc.InsertCustomerWallet(_tL);
                 }
                 else if(WalletBalance < Amount)
                 {
         Classes.Internetserviceprovider.InternetServiceObj _ptv = new Classes.Internetserviceprovider.InternetServiceObj();
                  _ptv.Amount = _pv.Amount;
                  _ptv.CustomerID = _pv.CustomerID;
                  _ptv.transactionlNo = _pv.transactionlNo;
                  _ptv.Service = _pv.Service;
                   _pv.ConvFee = "0";
                  TempData["TotalAmt"] = _pv.Amount;
                  TempData["PaytvObj"] = _ptv;
                  TempData["Msg"] = "Low Balance In Wallet !";
                  return Redirect("/Airtime/Checkout/");
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
        public ActionResult Checkout(string Biller,Paytv.PaytvObj paytv)
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                
                if (user != null)
                {
                  
                    Classes.Paytv.PaytvObj paytvs = new Paytv.PaytvObj();
                    string RefNum = Request.QueryString["RefNum"];
                   // RefNum = "payvice01137f85-e105-4839-936f-b14f53";
                if (RefNum == null)
                {
                    
                    return RedirectToAction("BuyAirtime", "Airtime");
                }
                if (RefNum != null)
                {
                        
                   // if am here;
                   // var CusTransac = _dr.GetCustTransac(RefNum);
                    var CusTransac = _dr.GetRecord(RefNum);

                        // I Added This on 14/May/2019

                        ViewBag.Wallet = GetWalletBalance((int)CusTransac.Customer_FK);

                        // I Added This 27/Nov/2018
                        //paytvs.Phone = _dr.GetPhone(RefNum);
                        paytvs.Amount = Convert.ToString(CusTransac.Amount);
                          paytvs.CustomerID = CusTransac.CustomerID;
                          paytvs.Service = CusTransac.ServiceDetails;
                          paytvs.ConvFee = Convert.ToString(CusTransac.ServiceCharge);
                          paytvs.transactionlNo = CusTransac.ReferenceNumber;
                          paytvs.customerName = CusTransac.CustomerName;
                          paytvs.Bouquet = CusTransac.ServiceDetails;
                          paytvs.paymentType = _dr.GetCustomerpaytype(RefNum);
                          string DiscoId = Convert.ToString(CusTransac.Merchant_FK);
                          paytvs.paymentType = _dr.GetCustomerpaytype(RefNum);
                          double Amount = Convert.ToDouble(paytvs.Amount);

                      /*  paytvs.Amount = "50";
                        paytvs.CustomerID = "08077755537";
                        paytvs.Service = "GLOVTV";
                        paytvs.ConvFee = "20";
                        paytvs.transactionlNo = "VTU122a39b7-202c-444e-92ad-967a6986";
                        paytvs.customerName = "08077755537";
                        paytvs.Bouquet = "GLOVTU";
                        paytvs.paymentType = 1;
                        string DiscoId = "1";
                        paytvs.paymentType = 1;
                        double Amount = Convert.ToDouble(paytvs.Amount);
                        */
                      //  string DiscoId = Convert.ToString(CusTransac.Merchant_FK);
                        if (Amount >= 50)
                        {
                            paytvs.ConvFee = ConfigurationManager.AppSettings["NoConvFee"];
                            TempData["TotalAmt"] = Convert.ToDecimal(paytvs.ConvFee) + Convert.ToDecimal(paytvs.Amount);

                        }

                   return View(paytvs);
                    }
                    
                    paytv = (Classes.Paytv.PaytvObj)ViewBag.PaytvObj;
                    paytvs = (Classes.Paytv.PaytvObj)TempData["PaytvObj"];
                    Biller = paytvs.BillerName;

                  

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
                if(Method == "1")
                {
                    value = 1;
                }
                else 
                {
                    return value ;
                }

                return value;
            }
            catch(Exception ex)
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

        //[HttpPost]
        //public ActionResult LoadImage()
        //{
        //    try
        //    {

        //        string val = Request.QueryString["val"];
        //        if (val != null)
        //        {
        //            var ImgID = LoadImage(val);
        //          //ViewBag.Image = _dr.GetAllServicesBySat(satId);
        //            return Json(new { Success = "true", Data = ImgID });
        //            //return Json(new { Success = "false" });

        //        }
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return null;
        //    }
        //}

        public string LoadImage(string DiscoId)
        {
            try
            {
              var url = ConfigurationManager.AppSettings["AirtimeImageUrls"] + DiscoId + ".png";
                var uri = new Uri(url);
                return url;
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
                    WebLog.Log("1" );
                    TempData["ErrMsg"] = "Transaction successful ! Thank You";
                    string refNum = jvalues?.data?.trxref;
                   // var id = UpdateTransaction(jvalues);
                    bool checkTrxtoken = _dr.CheckTranToken(refNum);
                    // if (id != null && checkTrxtoken == false)
                    if (checkTrxtoken == false)
                    {
                        WebLog.Log("2");
                        // string refNum = jvalues?.data?.trxref;

                        var PayRes = paySubscribtion(refNum);
                        dynamic Pay = JObject.Parse(PayRes);
                        //  if (Pay?.returnCode == 1)
                        if (Pay?.respCode == "00")
                        {
                         _tL.ReferenceNumber = refNum;
                         _tL.PatnerRefNumber = Pay?.tranNum;
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
                            _dc.UpdateTransac(_tL);
                            var id = UpdateTransaction(jvalues);
                            TempData["ErrMsg"] = "Transaction successful ! Thank You";
                            Receipt(Pay,refNum);

                        }
                        else if (Pay?.returnCode != "00")
                        {
   TempData["ErrMsg"] = "Payment Successful! Please Contact our Customer Care";
                            Receipt(Pay,refNum);
         
                        }
                    }
                }
                else
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00" || isPaid == false)
                {
     TempData["ErrMsg"] = "Transaction Failed ! Please Try Again";
                    var id = UpdateTransaction(jvalues);
                    if (id != null)
                    {
                        string refNum = jvalues?.data?.trxref;
                        dynamic payObjx = new JObject();
                        payObjx.returnCode = jvalues?.data?.resp_code;
                        payObjx.returnMsg = jvalues?.data?.resp_desc;
                        payObjx.tranNum = jvalues?.data?.trxref;
                        dynamic Pay = JObject.Parse(payObjx);
                        TempData["ErrMsg"] = "Transaction Failed ! Please Try Again";
                        Receipt(Pay,refNum);

                       
                    }

                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //lblStatusMsg.Text = ex.Message.ToString();
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



        ////  For Test
        //public string paySubscribtion(string refNum)
        //{
        //    try
        //    {
        //        dynamic payObjx = new JObject();
        //        dynamic payObj = _dr.PayLoad(refNum);
        //        //dynamic Phones = new JObject();
        //        //var Phone = _dr.GetPhone(refNum);
        //        //Phones.Phone = Phone;
        //        //var Payresponse = _paytv.PaySubscription(payObj, Phones);

        //        dynamic Jobj = new JObject();
        //        string Paymenttype = "";
        //        Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
        //        Jobj.vice_id = ConfigurationManager.AppSettings["payvicewalletid"];
        //        Jobj.user_name = ConfigurationManager.AppSettings["payviceUsername"];
        //        string PostUrl = ConfigurationManager.AppSettings["payvicelookupurl"];
        //        var json = Jobj.ToString();
        //        // var data = Utility.DoPost(json, $"{PostUrl}", token);
        //        // var data = "{\"vice_id\":\"76058956\",\"token\":\"e0c65d0140e0f65ecce7ac233357df9263f16c581785fd8a6fdfb2f21a369ae8\",\"status\":0,\"message\":\"Authenticated successfully\"}";
        //        var Response = "0";

        //        // var Response = JObject.Parse(data);

        //        //if (Response != null && Response?.status == "0")
        //        if (Response == "0")
        //        {
        //            //token = Response?.token;
        //            //message = Response?.message;
        //            //vice_id = Response?.vice_id;
        //            token = "e0c65d0140e0f65ecce7ac233357df9263f16c581785fd8a6fdfb2f21a369ae890";
        //            message = "Authenticated successfully";
        //            vice_id = "76058956";

        //            dynamic Airtime = new JObject();
        //            Airtime.vice_id = ConfigurationManager.AppSettings["payvicewalletid"];
        //            Airtime.user_name = ConfigurationManager.AppSettings["payviceUsername"];
        //            Airtime.amount = payObj.Amount;
        //            Airtime.phone = payObj.CustomerID;
        //            Airtime.service = payObj.ServiceDetails;
        //            Airtime.paymentType = payObj.PaymentType;
        //            //Paymenttype = Convert.ToString(form["paymenttype"]);
        //            //var service = Convert.ToString(form["Recuring"]);
        //            ////service = service.Before(",").Trim();
        //            //service = service.After("Airtime/").Trim();
        //            //service = service.Before(".").Trim();
        //            //Airtime.service = service;
        //            //AirtimeObj.Service = Airtime.service;
        //            Airtime.auth = ConfigurationManager.AppSettings["payvicecode"];
        //            Airtime.token = token;
        //            Airtime.pwd = ConfigurationManager.AppSettings["payvicepassword"];
        //            var AirtimeURL = ConfigurationManager.AppSettings["payviceAirtimeupurl"];
        //            var Airtimes = Airtime.ToString();
        //            // var AirtimeRespons = Utility.DoPost(Airtimes, $"{AirtimeURL}");
        //            // var AirtimeRespons = "message":"Topup completed successfully","status":1,"date":"18\/02\/2019 15:51:22","txn_ref":"5c6ad47aae06f";
        //            //dynamic AirtimeRespons = new JObject();
        //            //  var AirtimeResponse = JObject.Parse(AirtimeRespons);
        //            var AirtimeResponse = "1";
        //            // if (AirtimeResponse != null && AirtimeResponse?.status == "1")
        //            if (AirtimeResponse != null && AirtimeResponse == "1")
        //            {
        //                //payObjx.tranNum = AirtimeResponse?.txn_ref;
        //                //payObjx.returnCode = AirtimeResponse?.status;
        //                //payObjx.returnMsg = AirtimeResponse?.message;
        //                //payObjx.date = AirtimeResponse?.date;

        //                payObjx.tranNum = "5c6ad47aae06f90123";
        //                payObjx.returnCode = "1";
        //                payObjx.returnMsg = "Topup completed successfully";
        //                payObjx.date = "18/02/2019 15:51:22";

        //            }
        //        }

        //        var jsons = Convert.ToString(payObjx);
        //        return jsons;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex.Message.ToString());
        //        return "";
        //    }
        //}

        public string paySubscribtions(string refNum)
        {
            try
            {
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                // var ValidateRefNum = _dr.validateRefNum(refNum);

                dynamic Jobj = new JObject();
                string Paymenttype = "";
                Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
                Jobj.vice_id = ConfigurationManager.AppSettings["payvicewalletid"];
                Jobj.user_name = ConfigurationManager.AppSettings["payviceUsername"];
                string PostUrl = ConfigurationManager.AppSettings["payvicelookupurl"];
                var json = Jobj.ToString();
                var data = Utility.DoPost(json, $"{PostUrl}", token);

                var Response = JObject.Parse(data);

                if (Response != null && Response?.status == "0")

                {
                    token = Response?.token;
                    message = Response?.message;
                    vice_id = Response?.vice_id;

                    dynamic Airtime = new JObject();
                    Airtime.vice_id = ConfigurationManager.AppSettings["payvicewalletid"];
                    Airtime.user_name = ConfigurationManager.AppSettings["payviceUsername"];
                    Airtime.amount = payObj.Amount;
                    Airtime.phone = payObj.CustomerID;
                    Airtime.service = payObj.ServiceDetails;
                    //Airtime.paymentType = payObj.PaymentType;

                    Airtime.auth = ConfigurationManager.AppSettings["payvicecode"];
                    Airtime.token = token;
                    Airtime.pwd = ConfigurationManager.AppSettings["payvicepassword"];
                    var AirtimeURL = ConfigurationManager.AppSettings["payviceAirtimeupurl"];
                    var Airtimes = Airtime.ToString();
                    var AirtimeRespons = Utility.DoPost(Airtimes, $"{AirtimeURL}");

                    //dynamic AirtimeRespons = new JObject();
                    var AirtimeResponse = JObject.Parse(AirtimeRespons);

                    if (AirtimeResponse != null && AirtimeResponse?.status == "1")
                    {
                        payObjx.tranNum = AirtimeResponse?.txn_ref;
                        payObjx.returnCode = AirtimeResponse?.status;
                        payObjx.returnMsg = AirtimeResponse?.message;
                        payObjx.date = AirtimeResponse?.date;
                    }
                }

                var jsons = Convert.ToString(payObjx);
                return jsons;
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
                string agentID = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                string email = ConfigurationManager.AppSettings["agentEmail"];
                dynamic payObjx = new JObject();
                dynamic payObj = _dr.PayLoad(refNum);
                dynamic obj = new JObject();
                string valueTime = DateTime.Now.ToString("Hmmss");
                obj.refNumber = refNum; //"Agent" + DateTime.Now.ToString("yyyyMMdd") + valueTime;
                
                obj.amount = payObj.Amount;
                obj.phoneNumber = payObj.CustomerID;
                obj.serviceType = payObj.ServiceDetails;
                               var plainText = (agentID + agentKey + obj.refNumber);
                var builder = new StringBuilder();
                builder.Append(agentID).Append(agentKey).Append(obj.phoneNumber);
                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);
                obj.hashValue = hash;
                var json = obj.ToString();
              
                string sessionID = Paytv.GetSessionID();
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentID).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);
                var PostUrl = ConfigurationManager.AppSettings["BuyAirtime"];
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentID, agentKey, signature, sessionID);
                return data;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                //Label1.Text = ex.Message.ToString();
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
                    Receipt.CustomerID = CusTransac.CustomerID;
                    Receipt.Amount = Convert.ToDouble(CusTransac.Amount);
                    TempData["Amount"] = Receipt.Amount;
                    Receipt.customerName = CusTransac.CustomerName;
                    Receipt.Phone = CusTransac.CustomerPhone;
                    Receipt.transactionlNo = CusTransac.ReferenceNumber;
                    Receipt.Service = CusTransac.ServiceDetails;
                    Receipt.ErrorMsg = TempData["ErrMsg"].ToString();
                    
                }
                return View(Receipt);
            }
            catch(Exception ex)
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
                string RefNum = Request.QueryString["trxRef"];
                WebLog.Log("0");
                ProcessResponse();
                WebLog.Log("4");
                Classes.Paytv.Receipt Receipt = new Classes.Paytv.Receipt();
                    dynamic RecieptObj = new JObject();
                    if (TempData["ErrMsg"] != null)
                    {
                    Receipt.ErrorMsg = TempData["ErrMsg"].ToString();
                    WebLog.Log("5" + Receipt.ErrorMsg);
                }
                //string RefNum = Request.QueryString["trxRef"];
                Receipt.ErrorMsg = TempData["ErrMsg"].ToString();
                WebLog.Log("6" + Receipt.ErrorMsg);
                var RecieptVal = _dr.GetCustReceipt(RefNum);
                WebLog.Log("7" + RecieptVal);
                Receipt.CustomerID = RecieptVal[0];
                WebLog.Log("8" + Receipt.CustomerID);
                // Receipt.Amount = Convert.ToDouble(RecieptVal[3]);
                Receipt.Amount = 0;
                WebLog.Log("9" + Receipt.Amount);
                Receipt.ServiceCharge = Convert.ToDouble(RecieptVal[2]);
                WebLog.Log("Receipt.ServiceCharge" + Receipt.ServiceCharge);
                // Receipt.ServiceCharge = Convert.ToDouble("0");
                Receipt.CustomerName = RecieptVal[1];
                WebLog.Log("Receipt.CustomerName" + Receipt.CustomerName);
                Receipt.Phone = RecieptVal[5];
                WebLog.Log("Receipt.Phone" + Receipt.Phone);
                Receipt.ReferenceNumber = RecieptVal[6];
                WebLog.Log("Receipt.refrence" + Receipt.ReferenceNumber);
                Receipt.ServiceDetails = RecieptVal[7];
                WebLog.Log("Receipt.ServiceDetails" + Receipt.ServiceDetails);
                string Discoid = RecieptVal[8];
                WebLog.Log("Discoid" + Discoid);
                string img = LoadImage(Discoid);
                    string imgName = getDiscoName(Discoid);
                    Double TotalAmount = Convert.ToDouble(Receipt.ServiceCharge + Receipt.Amount);
                    TempData["Amount"] = TotalAmount;
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
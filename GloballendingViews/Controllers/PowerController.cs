using DataAccess;
using GloballendingViews.Classes;
using GloballendingViews.HelperClasses;
using GloballendingViews.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class PowerController : Controller
    {
        Utility newutil = new Utility();
        DataAccess.DataCreator _dc = new DataAccess.DataCreator();
        DataAccess.DataReaders _dr = new DataAccess.DataReaders();
        DataAccess.DataReaders datareader = new DataAccess.DataReaders();
        DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
        DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
        DataAccess.PaymentLog _pL = new DataAccess.PaymentLog();
        LogginHelper LoggedInuser = new LogginHelper();
        string Error = "";
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        Power _pw = new Power();
        // GET: Power
       /* [HttpGet]
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
        }*/
        [HttpGet]
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
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult Test()
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
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult BuyPower(string DiscoId)
        {
            try
            {

             if (DiscoId == null || DiscoId == "")
            {
                DiscoId = Request.QueryString["Disco"];

                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);
                      return View();
                   // return RedirectToAction("Index","Power");
             }
               
            if (TempData["Disconame"].ToString() == null || TempData["Disconame"].ToString() == "")
                {
                    return View("Index");
                }
                if (DiscoId != null || DiscoId != "")
                {
             Models.Power powers = new Models.Power();
             // DiscoId = Request.QueryString["Disco"];
            TempData["message"] = LoadImage(DiscoId);
            TempData["Disconame"] = getDiscoName(DiscoId);
            dynamic payObject = TempData["PayObj"];
            powers.CustomerID = payObject.CustomerID;
            powers.Amount = payObject.Amount;
            powers.Email = payObject.Email;
            powers.Merchant_fk = payObject.MerchantFk;
            powers.Phone = payObject.Phone;
            return View(powers);
                   // return View();

                }
                
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


       
        public string getDiscoName(string DiscoId)
        {
            string Disconame = "";
            switch (DiscoId)
            {
                case "1":
                    Disconame = "EKEDC";
                    break;
                case "2":
                    Disconame = "IKEDC";
                    break;
                case "3":
                    Disconame = "EEDC";
                    break;
                case "4":
                    Disconame = "IBEDC";
                    break;
                case "5":
                    Disconame = "KEDCO";
                    break;
                case "6":
                    Disconame = "YEDC";
                    break;
                case "7":
                    Disconame = "BEDC";
                    break;
                case "8":
                    Disconame = "PEDC";
                    break;
                case "9":
                    Disconame = "KEDC";
                    break;
                case "10":
                    Disconame = "AEDC";
                    break;
                case "11":
                    Disconame = "JEDC";
                    break;
                default:
                    break;
            }
            TempData["Disconame"] = Disconame;
            return TempData["Disconame"].ToString();
        }

        public string getDiscoId(string Disconame)
        {
            string DiscoId = "";
            switch (Disconame)
            {
                case "EKEDC":
                    DiscoId = "1";
                    break;
                case "IKEDC":
                    DiscoId = "2";
                    break;
                case "EEDC":
                    DiscoId = "3";
                    break;
                case "IBEDC":
                    DiscoId = "4";
                    break;
                case "YEDC":
                    DiscoId = "6";
                    break;
                case "BEDC":
                    DiscoId = "7";
                    break;
                case "PEDC":
                    DiscoId = "8";
                    break;
                case "KEDC":
                    DiscoId = "9";
                    break;
                case "AEDC":
                    DiscoId = "10";
                    break;
                case "KEDCO":
                    DiscoId = "5";
                    break;
                case "JEDC":
                    DiscoId = "11";
                    break;
                default:
                    break;
            }
            TempData["Disconame"] = DiscoId;
            return TempData["Disconame"].ToString();
        }

        public string LoadImage(string DiscoId)
        {
           try
         {
         // var url = "http://localhost:3346/Styles/images/powerProviders/" + DiscoId + ".jpg";
            var url = ConfigurationManager.AppSettings["ImageUrl"] + DiscoId + ".png";
            var uri = new Uri(url);
            var path = Path.GetFileName(uri.AbsolutePath);

                return url;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult BuyPower(FormCollection form
          , Models.Power pw)
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
                    Power.CustomerObj PayObj = new Power.CustomerObj();
                    //PayObj.MerchantFk = "1";
                    string Phones = "";
                    var MerchantFk = "";
                    var disconame = Convert.ToString(form["Image"]);
                    var discoid = getDiscoId(disconame);
                    PayObj.MerchantFk = discoid;
                    MerchantFk = discoid;
                    PayObj.MeterType = Convert.ToString(form["MeterType"]);
                   
                    PayObj.Phone = pw.Phone;
                    PayObj.CustomerID = pw.CustomerID;
                    PayObj.Email = pw.Email;
                    PayObj.Amount = pw.Amount;
                    
                    if (PayObj.Amount == "" || PayObj.CustomerID == "")
                    {
                        return View();
                    }
                    /* if (PayObj.MeterType == "Select-Meter-Type")
                     {
                         TempData["PayObj"] = PayObj;
                         var discname = getDiscoName(PayObj.MeterType);
                         TempData["message"] = discname;
                         TempData["Disconame"] = getDiscoId(discname);

                         return RedirectToAction("BuyPower", new
                         {
                             @DiscoId = MerchantFk
                         });
                     }
                     */
                    // bool CustID = newutil.ValidateCustID( PayObj.CustomerID);
                    if(MerchantFk == null || MerchantFk == "")
                    {
                        return RedirectToAction("index", "Power");
                        
                    }
                    bool Result = newutil.ValidateAmt(PayObj.Amount);
                    if (Result == false)
                    {
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        });

                    }
                    
                    var isValid = Utility.ValidatePhoneNumber(PayObj.Phone, out Phones);
                    if (isValid == false)
                    {
                        TempData["PayObj"] = PayObj;
                        var discname = getDiscoName(PayObj.MeterType);
                        TempData["message"] = discname;
                        TempData["Disconame"] = getDiscoId(discname);
                        // return Redirect("/Power/BuyPower/");
                        //return View();
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        });
                    }
                    /* PayObj.CustomerID = Convert.ToString(form["txtAccountNum"]);
                     PayObj.Phone = Convert.ToString(form["txtPhone"]);
                     PayObj.Email = Convert.ToString(form["txtEmail"]);
                     PayObj.Amount = Convert.ToString(form["txtAmount"]);*/
                    // PayObj.Imgurl = Convert.ToString(form["txturl"]);

                    var CustomerDetails = Power.GetCustomerDetails(PayObj);
                    // string id = "1";
                    // CustomerDetails = datareader.getJson(id);

                    if (CustomerDetails == null || CustomerDetails == "")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Please Try Again";
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    TempData["PayObj"] = PayObj;
                    dynamic jvalues = JObject.Parse(CustomerDetails.ToString());
                    double Amount = Convert.ToDouble(PayObj.Amount);
                    
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "00" && Amount >= Convert.ToDouble(jvalues?.MinimumAmount))
                    {
                        TempData["PayObj"] = PayObj;
                        LoanViewModel model = new LoanViewModel();

                        Models.Power power = JsonConvert.DeserializeObject<Models.Power>(CustomerDetails);
                        power.CustReference = "PWV" + Utility.TransactionGenerator();
                        power.Amount = PayObj.Amount;
                        power.Phone = PayObj.Phone;
                        power.Merchant_fk = PayObj.MerchantFk;
                        MerchantFk = power.Merchant_fk;
                        power.TransactionType = PayObj.MeterType;
                        power.TransactionType = PayObj.MeterType;
                        model.powerModel = power;
                        var a = power.AccountNumber;
                        TempData["power"] = power;
                      
                      return RedirectToAction("Checkout");
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "0X")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "310")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() != "00")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && Amount < Convert.ToDouble(jvalues?.MinimumAmount))
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Amount To Low";
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    
                    if (jvalues == null || jvalues == "")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        return RedirectToAction("BuyPower", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
           
        }

        public string InsertTransactionLog(Models.Power powers)
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
                    int userid = _dr.GetUserIdByEmail(user);
                    //if (powers.ResponseCode == "00")
                   // {
                        DateTime Dates = DateTime.Now;
                        // PaytvObj.transactionlNo = _ct.ReferenceNumber;
                        _ct.ReferenceNumber = Convert.ToString(powers.CustReference);
                       

                  _tL.ReferenceNumber = Convert.ToString(powers.CustReference);
                            var valids = _dr.ValidateCusTrancLog(_tL.ReferenceNumber);
                            if (valids == false)
                            {
                   _tL.Amount = Convert.ToDouble(powers.Amount);
                   _tL.CustomerID = powers.CustomerID;
                   _tL.CustomerName = powers.CustomerName;
                   _tL.Customer_FK = userid;
                  _tL.Merchant_FK = Convert.ToInt16(powers.Merchant_fk);
                   _tL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["PowerConvFee"]);
                                _tL.TrnDate = Dates;
                                _tL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                                _tL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                                _tL.ServiceDetails = powers.BusinessDistrict;
                   _tL.TransactionType = Convert.ToInt16(powers.TransactionType);
                   _tL.PaymentType = Convert.ToInt16(powers.PaymentType);
                        // I added this two 18-March-2019
                        _tL.CustomerEmail = powers.Email;
                        _tL.CustomerPhone = powers.Phone;
                            }
                            _dc.InitiateTransacLog(_tL);

                            _pL.ReferenceNumber = Convert.ToString(powers.CustReference);
                            var validpl = _dr.ValidatePayTrancLog(_tL.ReferenceNumber);
                            if (validpl == false)
                            {
                                _pL.CustomerPhoneNumber = powers.Phone;
                                _pL.Amount = Convert.ToDouble(powers.Amount);
                                _pL.CustomerID = powers.CustomerID;
                                _pL.CustomerName = powers.CustomerName;

                                _pL.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["PowerConvFee"]);
                                _pL.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                                _pL.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                                _pL.ReferenceNumber = powers.CustReference;
                                _pL.TrnDate = Dates;
                                _pL.CustomerEmail = powers.Email;
                            }
                            var RefNum = _pL.ReferenceNumber;
                            _dc.InitiatePaymentLog(_pL);

                        }


                   // }
                   
                
                return "";
            }
            
            
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


     /*   public string InsertCustomerTransaction(DataAccess.CustomerTransaction _ct)
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                    int userid = _dr.GetUserIdByEmail(user);
                    if (powers.ResponseCode == "00")
               {
                        DateTime Dates = DateTime.Now;
      // PaytvObj.transactionlNo = _ct.ReferenceNumber;
   //_ct.ReferenceNumber = Convert.ToString(_ct.CustReference);
    var valid = _dr.ValidateCusTranc(_ct.ReferenceNumber);
        if (valid == false)
                        {
                        
                            _ct.Amount = Convert.ToDouble(_ct.Amount);
                            _ct.CustomerID = _ct.CustomerID;
                            _ct.CustomerName = _ct.CustomerName;
                            _ct.Customer_FK = userid;
                            _ct.Merchant_FK = 1;
                            _ct.ServiceCharge = Convert.ToDouble(ConfigurationManager.AppSettings["Convfee"]);
                            _ct.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                            _ct.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                            _ct.ServiceDetails = powers.BusniessDistrict;
                            _ct.TransactionType = 1;
                            _ct.TrnDate = Dates;
                            _dc.InitiateCustomerTransaction(_ct);
                         }


                    }

                }
                return "";
            }


            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        */


        [HttpGet]
        public ActionResult CustomerDetails()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Checkout(string DiscoId)
        {
            try
            {
              
       Models.Power powers = new Models.Power();
     if (DiscoId != null || DiscoId != "")
     {
                     Power.CustomerObj _co = new Power.CustomerObj();
                     _co = (Classes.Power.CustomerObj)TempData["PayObj"];
                    
                    powers = (Models.Power)TempData["power"];
                    //This is For Customer Wallet
                    string user = LoggedInuser.LoggedInUser();
                    if (user == null || user == "")
                    {
                        user = LoggedInuser.getDefaultUser();
                    }
                        int userid = _dr.GetUserIdByEmail(user);
                        ViewBag.Wallet = GetWalletBalance((int)userid);
                    // Ends Here
                    DiscoId = powers.Merchant_fk;
         powers.ConvFee = ConfigurationManager.AppSettings["PowerConvFee"];
          TempData["message"] = LoadImage(DiscoId);
          TempData["Disconame"] = getDiscoName(DiscoId);
          TempData["ErrorMessage"] = powers.ResponseDesrciption;
          TempData["power"] = powers;
                    TempData["TotalAmt"] = Convert.ToDecimal(powers.ConvFee) + Convert.ToDecimal(powers.Amount);
                   // TempData["TotalAmt"] = MyUtility.ConvertToCurrency(TempData["TotalAmt"].ToString());
                    TempData.Keep("power");

         
          return View(powers);
            }
          powers = (Models.Power)TempData["power"];
          if(powers.ToString() == null)
         {
           return View();
         }
  if (powers.CustomerID == null || powers.CustomerID == "")
                {
                    return View();
                }
                else
                {
         DiscoId = powers.Merchant_fk;
        TempData["message"] = LoadImage(DiscoId);
        TempData["Disconame"] = getDiscoName(DiscoId);
                    TempData["TotalAmt"] = Convert.ToDecimal(powers.ConvFee) + Convert.ToDecimal(powers.Amount);
                    return View(powers);
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Checkout(Models.Power power,LoanViewModel lvm)
        {
          

      try
       {
                InsertTransactionLog(power);
                int PayMethod = PaymentMethod(power.paymethod);
                string user = LoggedInuser.LoggedInUser();
                if (power.CustomerID == null || power.CustomerID == "")
                {
                    RedirectToAction("Index");
                }
                
                if (power.CustomerName != null || power.CustomerName != "")
                {
                     Power.CustomerObj _co = new Power.CustomerObj();
                     _co = (Classes.Power.CustomerObj)TempData["PayObj"];
                             power = (Models.Power)TempData["power"];

                             if(power == null)
                             {
                                 power = (Models.Power)Session["power"];
                                 object values = TempData.Peek("power");
                                 power = lvm.powerModel;
                                 return View("Index");

                             }
                    if (user != null && PayMethod == 0)
                    {
                        string DiscoId = power.Merchant_fk;
                        TempData["message"] = LoadImage(DiscoId);
                        TempData["Disconame"] = getDiscoName(DiscoId);


                        dynamic PayObjs = new JObject();
                        Classes.Power.CustomerObj pw = new Classes.Power.CustomerObj();
                        pw = (Classes.Power.CustomerObj)TempData["PayObj"];
                        Power.CustomerObj PayObj = new Power.CustomerObj();
                        PayObj.agentID = power.AgentID;
                        PayObj.Phone = power.Phone;
                        PayObj.CustomerID = power.CustomerID;
                        PayObj.Email = power.Email;
                        PayObj.Amount = power.Amount;
                        PayObj.CustomerName = power.CustomerName;
                        PayObj.MerchantFk = power.Merchant_fk;
                        PayObj.MeterType = power.TransactionType;

                        TempData["PayRespone"] = PayObj;
                        ViewBag.power = TempData["power"];
                        Session["power"] = TempData["power"];
                        /// This is for Payment Page
                        /// 


                        string Firstname = PayObj.CustomerName;
                        string Email = PayObj.Email;
                        string PhoneNumber = PayObj.Phone;
                        string Amount = PayObj.Amount;
                        string ConvFee = ConfigurationManager.AppSettings["PowerConvFee"];
                        string refernceNum = power.CustReference;
                        int paymentplanId = 1;



                        var TotalAmt = Convert.ToDecimal(Amount) + Convert.ToDecimal(ConvFee);
                        string TotalAmts = TotalAmt.ToString();
                        var TranNum = refernceNum;
                        int paymenttype = 1;//_dr.GetCustomerpaytype(TranNum);


                        bool isNum = Decimal.TryParse(TotalAmts, out TotalAmt);

                        if (isNum)
                        {
                     PaymentManager.Payment PayObject = new PaymentManager.Payment();
                     PayObject.PaymentType = paymenttype;
                     PayObject.RefNumber = TranNum; //System.DateTime.Now.ToString("yyyyMMddHmmss");
                     PayObject.amount = TotalAmt.ToString();
                     PayObject.customerid = "2";
                     PayObject.customerName = Firstname;
                     PayObject.emailaddress = Email;
                     PayObject.narration = $"{Firstname.Trim()} Payment of NGN {decimal.Parse(PayObject.amount)}";
                     PayObject.phoneNo = PhoneNumber;
                     PayObject.returnUrl = ConfigurationManager.AppSettings["PowerPaymentReturnUrl"];
                     PayObject.PaymentPlanID = paymentplanId;
                            //PayObj.returnUrl = GetReturnUrl(PayObj.returnUrl);
                            string formObject = PaymentManager.GetPaymentPageDatails(PayObject);
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
                            double Amount = Convert.ToDouble(power.Amount);
                            if (WalletBalance >= Amount)
                            {
                               // dynamic response = payDstvGotv(_pv.transactionlNo);
           dynamic response  = Power.PostBuyPowerJson(power.CustReference, Convert.ToInt16(power.Merchant_fk));
            TempData["ErrMsg"] = "Transaction Successful!";
            Receipt(response, power.CustReference);
            _tL.ReferenceNumber = power.CustReference;
             dynamic Pay = JObject.Parse(response);
             _tL.ServiceValueDetails1 = Pay?.units;
             _tL.ServiceValueDetails2 = Pay?.value;
             _tL.ServiceValueDetails3 = Pay?.customerAdddress;
             var bsstToken = Pay?.bsstToken;
             var bsstTokenUnits = Pay?.bsstTokenUnits;
             bsstToken = bsstToken + "" + "" + bsstTokenUnits;
             _tL.ThirdPartyCode = bsstToken;
              _dc.UpdateTransaction(_tL);
              var id = _dc.InsertCustomerWallet(_tL);
                            }
                            else if (WalletBalance < Amount)
                            {
                                Models.Power _ptv = new Models.Power();
                                TempData["TotalAmt"] = power.Amount;
                                // TempData["PaytvObj"] = _ptv;
                                TempData["power"] = power;
                                TempData["ErrorMessage"] = "Low Balance In Wallet !";
                                TempData["ErrMsg"] = "Low Balance In Walllet !";
                                return Redirect("/Power/Checkout/");
                            }
                            return RedirectToAction("WalletReceipt", new { @trxRef = _tL.ReferenceNumber });
                        }
                    }
                }

                /// This is For Payment Page






    /*  var PayRespone = Power.PostBuyPowerJson(PayObj);
      dynamic jvalues = JObject.Parse(PayRespone);
     if (jvalues != null && $"{jvalues?.ResponseCode}".ToLower() == "00")
         {
         LoanViewModel model = new LoanViewModel();
         TempData["val"] = model.powerModel;
         return RedirectToAction("Checkout");
         }
        if (jvalues != null && $"{jvalues?.ResponseCode}".ToLower() != "00")
         {
                   
   TempData["ErrorMessage"] = jvalues?.ResponseDesrciption;
    return RedirectToAction("Checkout", new
           {
                        @DiscoId = pw.MerchantFk
           }
         );
       }
          */   
                return View();
        }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult WalletReceipt(Classes.Power.Receipt Receipt)
        {
            try
            {
               // Classes.Power.Receipt Receipt = new Classes.Power.Receipt();
                string RefNum = Request.QueryString["trxRef"];
                if (RefNum == null)
                {
                   return RedirectToAction("BuyPower", "Power");
                }
                if(RefNum != null)
                {
                    var CusTransac = _dr.GetRecord(RefNum);
                    Receipt.CustomerID = CusTransac.CustomerID;
                    Receipt.Amount = Convert.ToDouble(CusTransac.Amount);
                    Receipt.ConvFee = Convert.ToString(CusTransac.ServiceCharge);
                    Double ConvFee = Convert.ToDouble(Receipt.ConvFee);
                    Receipt.customerName = CusTransac.CustomerName;
                    Receipt.Phone = CusTransac.CustomerPhone;
                    Receipt.transactionlNo = CusTransac.ReferenceNumber;
                    ViewBag.BonusToken = CusTransac.ThirdPartyCode; 
                    Receipt.ServiceValueDetails1 = CusTransac.ServiceValueDetails1;
                    Receipt.ServiceValueDetails2 = CusTransac.ServiceValueDetails2;
                    Receipt.ServiceDetails = CusTransac.ServiceDetails;
                    TempData["Amount"] = ConvFee + Receipt.Amount;
                   // Receipt.ErrorMsg = TempData["ErrMsg"].ToString();

                }
                return View(Receipt);
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
                WebLog.Log("Number A");
                ProcessResponse();
                
                Classes.Power.Receipt Receipt = new Classes.Power.Receipt();
                dynamic RecieptObj = new JObject();
                if (TempData["ErrMsg"] != null)
                {
                    //TempData["ErrMsg"] = "transaction Not Succesfull. Please Contact Our Customer Care Representative ";
                }
                string RefNum = Request.QueryString["trxRef"];

                var RecieptVal = _dr.GetCustReceipt(RefNum);

          Receipt.CustomerID = RecieptVal[0];
          Receipt.Amount = Convert.ToDouble(RecieptVal[3]);
          Receipt.ServiceCharge = Convert.ToDouble(RecieptVal[2]);
          Receipt.NewAmount = MyUtility.ConvertToCurrency(RecieptVal[2]);
          Receipt.CustomerName = RecieptVal[1];
          Receipt.Phone = RecieptVal[5];
          Receipt.ReferenceNumber = RecieptVal[6];
          Receipt.ServiceDetails = RecieptVal[7];
          Receipt.TrnDate = Convert.ToDateTime(RecieptVal[4]);
          //Receipt.Address = Receipt.Address;
          Receipt.ServiceValueDetails1 = RecieptVal[9];
                Receipt.ServiceValueDetails2 = RecieptVal[10];
                Receipt.TransactionType = Convert.ToInt16(RecieptVal[14]);
                Receipt.ServiceValueDetails2 = RecieptVal[10];
                ViewBag.BonusToken = RecieptVal[12];
                if (Receipt.TransactionType == 2)
                {
                    if (Receipt.ServiceValueDetails1 == null || Receipt.ServiceValueDetails1 == "")
                    {
                        Receipt.ServiceValueDetails1 = "N/A";
                    }

                    if (Receipt.ServiceValueDetails2 == null || Receipt.ServiceValueDetails2 == "")
                    {
                        Receipt.ServiceValueDetails2 = "N/A";
                    }

                    if (RecieptVal[12] == null || RecieptVal[12] == "")
                    {
                        ViewBag.BonusToken = "N/A";
                    }
                }
                Receipt.ServiceValueDetails3 = RecieptVal[11];
                Receipt.TransactionStatus_FK = Convert.ToInt16(RecieptVal[13]);
               
                string Discoid = RecieptVal[8];
                string img = LoadImage(Discoid);
                string imgName = getDiscoName(Discoid);
                double TotalAmount = Convert.ToDouble(Receipt.ServiceCharge + Receipt.Amount);
                Receipt.TotalAmt = MyUtility.ConvertToCurrency(TotalAmount.ToString());
                ViewBag.ResetToken = RecieptVal[15];
                ViewBag.ConfigureToken = RecieptVal[16];

                TempData["Amount"] = Receipt.TotalAmt;
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
            WebLog.Log("method01" + method);
            //if (req == null) return null;
            var req = (HttpWebRequest)WebRequest.Create(url);
            WebLog.Log("req02" + req);
            req.Method = method.ToUpper();
            WebLog.Log("req03" + req);
            req.Timeout = Timeout.Infinite;
            WebLog.Log("req04" + req);
            req.KeepAlive = true;
            WebLog.Log("reqKeepALive05" + req.KeepAlive);

            if (isPayEngine)
            {
                //var PublicKey = "pk_8LHVMWLJDXAV8CPEZB9M2YGQ7CMMTWMKZGTTQCKATSFXZNJGGSES0GV9";
                req.Headers.Add($"PaelytAuth:{hashValue}");
                WebLog.Log("hashValue" + hashValue);
                // req.Headers.Add($"PaelytAuth:{PublicKey}");
                req.Headers.Add($"PublicKey: {ConfigurationManager.AppSettings["PublicKey"]}");
                WebLog.Log("Publickey" + ConfigurationManager.AppSettings["PublicKey"]);
                WebLog.Log("req" + req);
                return req;
            }


            return req;
        }

        public static bool PaymentRequery(string postData, string url, string hashValue, out string serverResponse, out TransactionalInformation trans)
        {
            WebLog.Log("postdata1" + postData);
            WebLog.Log("url2" + url);
            WebLog.Log("hashvalue3" + hashValue);
            
            trans = new TransactionalInformation();
            WebLog.Log("trans4" + trans);
            serverResponse = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string ClientID = "";
            string responseString;
            try
            {
               
                _req = ReturnHeaderParameters("POST", url, hashValue, true);
                WebLog.Log("_req5" + _req);
                WebLog.Log("urlnew" + url);
                WebLog.Log("postdatenew" + postData);
                WebLog.Log("hashvaluenew" + hashValue);
                WebLog.Log("ClientIDnew" + ClientID);
                responseString = Paymentquery(postData, url,hashValue,  ClientID );
              
                dynamic jvalue = JObject.Parse(responseString);
                WebLog.Log("jvalue6" + jvalue);
                serverResponse = jvalue.ToString();
                WebLog.Log("serverResponse7" + serverResponse);
                WebLog.Log("jvalue8" + jvalue);
                WebLog.Log("jvalue8" + jvalue);
                if (jvalue?.data?.resp_code == "00")
                {
                    WebLog.Log("jvalue?.data?.resp_code" + jvalue);
                    trans.IsAuthenicated = true;
                    WebLog.Log("trans.IsAuthenicated" + trans.IsAuthenicated);
                }
              
            }
            //to read the body of the server _response when status code != 200
            catch (WebException exec)
            {
                var response = (HttpWebResponse)exec.Response;
                WebLog.Log("response20" + response);
                var dataStream = response.GetResponseStream();
                WebLog.Log("dataStream21" + dataStream);
                trans.ReturnMessage.Add(exec.Message);
                if (dataStream == null) return trans.IsAuthenicated;
                using (var tReader = new StreamReader(dataStream))
                {
                    WebLog.Log("tReader22" + tReader);
                    responseString = tReader.ReadToEnd();
                }
                //trans.ReturnMessage.Add(exec.Message);
                var jvalue = JObject.Parse(responseString);
                WebLog.Log("jvalue23" + jvalue);
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


        [HttpGet]
        public ActionResult BuyDirectPower()
        {
            try
            {

                return View();
            }
            catch(Exception ex)
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
                var refNum = Convert.ToString(form["refNum"]);
                var MerchankFk = Convert.ToString(form["MerchantFk"]);
                var PayRes = Power.PostBuyPowerJson(refNum, Convert.ToInt16(MerchankFk));
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public static string Paymentquery(string postData, string url, string hashValue, string ClientID = "")
        {
            try
            {
                WebLog.Log("urlnv" + url);
                WebLog.Log("postdatev" + postData);
                WebLog.Log("hashvaluev" + hashValue);
                WebLog.Log("ClientIDv" + ClientID);
                var resonse = Utility.DoPaymentRequeryPosts(postData,url,hashValue,ClientID);
                WebLog.Log("response Payment Requery " + _response);
                return resonse;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }
        protected void ProcessResponse()
        {
            var trxRef = Request.QueryString["trxRef"];
            var trxToken = Request.QueryString["trxToken"];
            var SecretKey = ConfigurationManager.AppSettings["SecretKey"];
            //WebLog.Log("number 0" + 0);
            WebLog.Log("trxRef " + trxRef);
            //WebLog.Log("trxToken " + trxToken);
            WebLog.Log("SecretKey " + SecretKey);
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
                WebLog.Log("url" + url);
                //re-query payment gateway for the transaction.
                var reqData = "{\"TrxRef\":\"" + trxRef + "\"" + ",\"TrxToken\":\"" + trxToken + "\"}";
                WebLog.Log("reqData" + reqData);
                //  var url = ConfigurationManager.AppSettings["FlutterWave_Requery"];
                //var url = FlutteWaveEngine.FlutterWaveRequery;
                string serverResponse = string.Empty;

                var plainText = (trxRef + trxToken + SecretKey);
                WebLog.Log("plainText" + plainText);
                var hash = new CryptographyManager().ComputeHash(plainText, HashName.SHA256);
                WebLog.Log("hash" + hash);
                var isPaid = PaymentRequery(reqData, url, hash, out serverResponse, out trans);
                WebLog.Log("isPaid" + isPaid.ToString());
                WebLog.Log("serverResponse" + serverResponse);
                var jvalue = JObject.Parse(serverResponse);
                WebLog.Log("jvalue" + jvalue);
                dynamic jvalues = JObject.Parse(serverResponse);
                WebLog.Log("jvalue" + jvalue);
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() == "00" && isPaid == true)
                {
                    // TempData["ErrMsg"] = "Transaction Successful";
                    TempData["SucMsg"] = "Transaction Succesful !";
                    WebLog.Log("number 1" + 1);
                    string refNum = jvalues?.data?.trxref;
                    WebLog.Log("number 2" + 2);
                    // var id = UpdateTransaction(jvalues);
                    bool checkTrxtoken = _dr.CheckTranToken(refNum);
                    WebLog.Log("number 3" + 3);
                    if (checkTrxtoken == false)
                    {
                        WebLog.Log("number 4" + 4);

                        // string refNum = jvalues?.data?.trxref;
                        var val = _dr.GetRecord(refNum);
                        WebLog.Log("val" + val);
                            // for Wallet
                            int userid = Convert.ToInt16(val.Customer_FK);
                        WebLog.Log("userid" + userid);
                        MyUtility.insertWallet(userid, val);
                        WebLog.Log("val" + val);
                        var PayRes = Power.PostBuyPowerJson(refNum, Convert.ToInt16(val.Merchant_FK));
                        WebLog.Log("refNum" + refNum);
                        WebLog.Log("val.Merchant_FK" + val.Merchant_FK);

                        dynamic Pay = JObject.Parse(PayRes);
                        WebLog.Log("Pay" + Pay);
                        if (Pay?.respCode == "00")
                            {
                                // var val = _dr.GetRecord(refNum);
                                _ct.CustomerName = val.CustomerName;
                            WebLog.Log("_ct.CustomerName" + _ct.CustomerName);
                            _ct.Merchant_FK = val.Merchant_FK;
                            WebLog.Log("_ct.Merchant_FK" + _ct.Merchant_FK);
                            _ct.ReferenceNumber = val.ReferenceNumber;
                            WebLog.Log("_ct.ReferenceNumber" + _ct.ReferenceNumber);
                            _ct.ServiceCharge = val.ServiceCharge;
                            WebLog.Log("_ct.ServiceCharge" + _ct.ServiceCharge);
                            _ct.ServiceDetails = val.ServiceDetails;
                            WebLog.Log("_ct.ServiceDetails" + _ct.ServiceDetails);
                            _ct.TransactionType = val.TransactionType;
                            WebLog.Log("_ct.TransactionType" + _ct.TransactionType);
                            _ct.TrnDate = val.TrnDate;
                            WebLog.Log("_ct.TrnDate" + _ct.TrnDate);
                            _ct.ValueDate = val.ValueDate;
                            WebLog.Log("_ct.ValueDate" + _ct.ValueDate);
                            _ct.ValueTime = val.ValueTime;
                            WebLog.Log("_ct.ValueTime" + _ct.ValueTime);
                            _ct.Amount = val.Amount;
                            WebLog.Log("_ct.Amount" + _ct.Amount);
                            _ct.CustomerID = val.CustomerID;
                            WebLog.Log("_ct.CustomerID" + _ct.CustomerID);
                            _ct.Customer_FK = val.Customer_FK;
                            WebLog.Log("_ct.Customer_FK" + _ct.Customer_FK);
                            _tL.ServiceValueDetails1 = Pay?.units;
                            WebLog.Log("_tL.ServiceValueDetails1" + _tL.ServiceValueDetails1);
                            _tL.ServiceValueDetails2 = Pay?.value;
                            WebLog.Log("_tL.ServiceValueDetails2" + _tL.ServiceValueDetails2);
                            _tL.ServiceValueDetails3 = Pay?.customerAdddress;
                            WebLog.Log("_tL.ServiceValueDetails3" + _tL.ServiceValueDetails3);
                            _ct.CusPay2response = Convert.ToString(Pay);
                            WebLog.Log("_ct.CusPay2response" + _ct.CusPay2response);
                            _dc.InitiateCustomerTransaction(_ct);
                            _tL.ReferenceNumber = _ct.ReferenceNumber;
                            //** For Bsst Token /**/
                            var bsstToken = Pay?.bsstToken; 
                            var bsstTokenUnits = Pay?.bsstTokenUnits;
                            bsstToken = bsstToken  + "" + "" + bsstTokenUnits;
                            _tL.ThirdPartyCode = bsstToken;
                            //** For Bsst Token /**/
                            WebLog.Log("_tL.ReferenceNumber" + _tL.ReferenceNumber);

                            // 25/Nov/2020
                            var cToken = Pay?.value2;
                            var RToken = Pay?.value1;
                            _tL.Pin = cToken;
                            _tL.ServiceCode = RToken;

                            // Ends Here
                            _dc.updateTransactionLogs(_tL);
                         var id = UpdateTransaction(jvalues);
                            WebLog.Log("id" + id);
                          Receipt(Pay, refNum);
                          TempData["SucMsg"] = "Transaction Succesful !";
                            }
                           // else if (Pay?.returnCode != 0)
                           else if (Pay?.respCode != "00")
                            {
                           
                            Receipt(Pay, refNum);
                            TempData["ErrMsg"] = "Payment Successful ! Please contact us for token" + Pay?.respDescription;
                        }
                        //}
                    }
                }
                else
                if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00" || isPaid == false)
                {
                    TempData["ErrMsg"] = "Transaction Failed ! Please Try Again" +  jvalues?.data?.resp_desc;

                    var id = UpdateTransactions(jvalues);
                    if (id != null)
                    {
                        string refNum = jvalues?.data?.trxref;
                        dynamic payObjx = new JObject();
                        payObjx.returnCode = jvalues?.data?.resp_code;
                        payObjx.returnMsg = jvalues?.data?.resp_desc;
                        payObjx.tranNum = jvalues?.data?.trxref;
                        //  dynamic Pay = JObject.Parse(payObjx);
                        // Receipt(Pay, refNum);
                        Receipt(payObjx, refNum);

       TempData["ErrMsg"] = "Transaction Failed ! Please Try Again" +     jvalues?.data?.resp_desc;
                    }

                }
            }
            catch (Exception ex)
            {
                WebLog.Log("What happin");
                WebLog.Log(ex.Message.ToString());
                //lblStatusMsg.Text = ex.Message.ToString();
            }
        }

        public ActionResult RequeryPower()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpPost]
        public ActionResult Requery(Classes.Power.Receipt Recpt,FormCollection form)
        {
            try
            {
               
                string RefNum = Convert.ToString(form["RefNum"]);
                RefNum = "PWV973258324154";
                var valid = _dr.CheckTLog(RefNum);
                //GetTransactionDetails(Recpt);
                if(valid == true)
                {
                GetTransactionDetails(RefNum,Recpt);
                }
                // GetPaymentResponse();
                // return View();
                return RedirectToAction("GetPaymentResponse", new
                {
                    @trxRef = RefNum
                });
                
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        //public dynamic GetTransactionDetails(Classes.Power.Receipt Recpt)
        public dynamic GetTransactionDetails(string RefNum,Classes.Power.Receipt Recpt)
        {
            try
            {
                var GetTransaction = _dr.GetRecord(RefNum);
                Recpt.ReferenceNumber = GetTransaction.ReferenceNumber;
                var refNumber = Recpt.ReferenceNumber;
                Recpt.Amount = (double)GetTransaction.Amount;
                var amount = Recpt.Amount;
                /* string disconame = Convert.ToString(TempData["BillerName"]);
                 var merchantFK = getDiscoId(disconame);
                 Recpt.Merchant_FK = Convert.ToInt16(merchantFK);*/
                Recpt.Merchant_FK = Convert.ToInt16(GetTransaction.Merchant_FK);
                dynamic Resp = Power.GetTransactionDetails(Recpt);
                Resp = JObject.Parse(Resp);
                // i am here
                if (Resp?.respCode == 00)
                {
                    DataAccess.TransactionLog TLog = new DataAccess.TransactionLog();
                    TLog.ReferenceNumber = Resp?.receiptNumber;
                    var RespUpt = _dc.UpdateTransacLogs(Resp, TLog);
                }
               return Resp;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
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

        public string UpdateTransactions(dynamic PayObj)
        {
            try
            {
                _tL.TrxToken = PayObj?.data?.trxtoken;
                _tL.ReferenceNumber = PayObj?.data?.trxref;
                _dc.UpdateTransacLog(_tL);
                _pL.PaymentReference = PayObj?.data?.payment_ref;
               // _pL.TrxToken = PayObj?.data?.trxref;
                _pL.TrnDate = PayObj?.data?.updated_at;
                _pL.ResponseCode = PayObj?.data?.resp_code;
                _pL.ResponseDescription = PayObj?.data?.resp_desc;
                _pL.ReferenceNumber = PayObj?.data?.trxref;
                var id = _dc.UpdatePaymentLogs(_pL);
                return id;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult Receipt(dynamic Response,string trxRef)
        {
            try
            {
                Classes.Power.Receipt Receipt = new Classes.Power.Receipt();
                dynamic RecieptObj = new JObject();
                string RefNum = Convert.ToString(trxRef);
                var RecieptVal = _dr.GetCustReceipt(trxRef);
                if (Response?.respCode != "00")
                {
                    //17/sept/2019
                   // TempData["ErrMsg"] = Response?.returnMsg;
                }
                Receipt.CustomerID = RecieptVal[0];
                Receipt.Amount = Convert.ToDouble(RecieptVal[3]);
                Receipt.ConvFee = RecieptVal[2];
                Receipt.customerName = RecieptVal[1];
                Receipt.Phone = RecieptVal[5];
                Receipt.transactionlNo = RecieptVal[6];
                Receipt.Service = RecieptVal[7];
               // Receipt.Address = Response?.customerAdddress;
                Receipt.Token = Response?.value;
               // Receipt.ServiceValueDetails1 = RecieptVal[9];
              //  Receipt.ServiceValueDetails2 = RecieptVal[10];

                Receipt.ServiceValueDetails1 = RecieptVal[9];
                if (Receipt.ServiceValueDetails1 == null || Receipt.ServiceValueDetails1 == "")
                {
                    Receipt.ServiceValueDetails1 = "N/A";
                }
                Receipt.ServiceValueDetails2 = RecieptVal[10];
                if (Receipt.ServiceValueDetails2 == null || Receipt.ServiceValueDetails2 == "")
                {
                    Receipt.ServiceValueDetails2 = "N/A";
                }
                Receipt.ServiceValueDetails3 = RecieptVal[11];
                ViewBag.BonusToken = RecieptVal[12];
                if (RecieptVal[12] == null || RecieptVal[12] == "")
                {
                    ViewBag.BonusToken = "N/A";
                }
                ViewBag.Address = Response?.customerAdddress;
                ViewBag.Token = Response?.value;

                ViewBag.ResetToken =  Response?.value1;
                ViewBag.ConfigureToken =  Response?.value2;
             
                return View(Receipt);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult Boro(string DiscoId)
        {
            try
            {
                if (DiscoId == null || DiscoId == "")
                {
                    DiscoId = Request.QueryString["Disco"];

                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);
                    return View();
                    
                }

                if (TempData["Disconame"].ToString() == null || TempData["Disconame"].ToString() == "")
                {
                    return View("Index");
                }
                if (DiscoId != null || DiscoId != "")
                {
                    Models.Power powers = new Models.Power();
                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);
                    dynamic payObject = TempData["PayObj"];
                    powers.CustomerID = payObject.CustomerID;
                    powers.Amount = payObject.Amount;
                    powers.Email = payObject.Email;
                    powers.Merchant_fk = payObject.MerchantFk;
                    powers.Phone = payObject.Phone;
                    return View(powers);
                 
                }

                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        

        [HttpPost]
        public ActionResult Boro(FormCollection form, Models.Power pw)
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
                    Power.CustomerObj PayObj = new Power.CustomerObj();
                    string Phones = "";
                    var MerchantFk = "";
                    var disconame = Convert.ToString(form["Image"]);
                    var discoid = getDiscoId(disconame);
                    PayObj.MerchantFk = discoid;
                    MerchantFk = discoid;
                    PayObj.MeterType = Convert.ToString(form["MeterType"]);

                    PayObj.Phone = pw.Phone;
                    PayObj.CustomerID = pw.CustomerID;
                    PayObj.Email = pw.Email;
                    PayObj.Amount = pw.Amount;

                    if (PayObj.Amount == "" || PayObj.CustomerID == "")
                    {
                        return View();
                    }
                    
                   // I added This One Today March 18-march-2019
                   bool Res = Power.verifyPay(PayObj.Amount);
                    if (Res == false)
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Please enter an amount in multiple of N500.";
                        return RedirectToAction("Boro", new
                    {
                      @DiscoId = MerchantFk
                    });

                    }
                    // And it Ends Here
                    bool Result = newutil.ValidateNum(PayObj.Amount, PayObj.CustomerID);
                    if (Result == false)
                    {
                     return RedirectToAction("Boro", new
                     {
                            @DiscoId = MerchantFk
                     });

                    }
                    var isValid = Utility.ValidatePhoneNumber(PayObj.Phone, out Phones);
                    if (isValid == false)
                    {
                        TempData["PayObj"] = PayObj;
                        var discname = getDiscoName(PayObj.MeterType);
                        TempData["message"] = discname;
                        TempData["Disconame"] = getDiscoId(discname);
                       
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        });
                    }

                    //Validate Customer Service//
                  int MFK = Convert.ToInt16(MerchantFk);
                   var CustomerID = Convert.ToString(PayObj.CustomerID);
                   var value = checkCustomerServices(CustomerID,MFK);
                    if (value == false)
                    {
                        var res = InsertCustomerService(PayObj,disconame);
                    }
                    //*********************************//
                    // var CustomerDetails = Power.GetCustomerDetails(PayObj);
                var CustomerDetails = Power.CheckMeterEligibilty(PayObj);

                 if (CustomerDetails == null || CustomerDetails == "")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Please Try Again";
                        return RedirectToAction("Boro", new
                       {
                        @DiscoId = MerchantFk
                       });
                    }
                    TempData["PayObj"] = PayObj;
                    dynamic jvalues = JObject.Parse(CustomerDetails.ToString());
                    double Amount = Convert.ToDouble(PayObj.Amount);
                      var MinimunBoro = ConfigurationManager.AppSettings["BoroMinimum"];
                    var BoroPowerConvFee = ConfigurationManager.AppSettings["BoroPowerConvFee"];
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "00" && Amount >= Convert.ToDouble(MinimunBoro) && Amount <= Convert.ToDouble(jvalues?.CreditLimit) )
                    {
                        TempData["PayObj"] = PayObj;
                        LoanViewModel model = new LoanViewModel();

                        Models.Power power = JsonConvert.DeserializeObject<Models.Power>(CustomerDetails);
                        power.CustReference = "PWL" + Utility.TransactionGenerator();
               //var RealAmount = Power.ClaculateFifteenpercent(Amount);
               //         var ConvFee = Amount - RealAmount;
               //         PayObj.Amount = RealAmount.ToString();
                        power.Amount = PayObj.Amount;
                        power.Phone = PayObj.Phone;
                        power.Merchant_fk = PayObj.MerchantFk;
                        MerchantFk = power.Merchant_fk;
                        power.TransactionType = PayObj.MeterType;
                        power.TransactionType = PayObj.MeterType;
                        power.Phone = PayObj.Phone;
                        power.Email = PayObj.Email;
                        model.powerModel = power;
                        var a = power.AccountNumber;
                        TempData["power"] = power;

                        // return RedirectToAction("Checkout");
                        return RedirectToAction("CheckoutBoro");
                    }

                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "091")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }

                   
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "0X")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "310")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() != "00")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                    if (jvalues != null && Amount > Convert.ToDouble(jvalues?.CreditLimit))
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Amount Must Not Exceed Credit Limit";
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }

                    if (jvalues != null && Amount < Convert.ToDouble(MinimunBoro))
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Minimum Lend Amount is N1,500.00";
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }
                      if (jvalues == null || jvalues == "")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        return RedirectToAction("Boro", new
                        {
                            @DiscoId = MerchantFk
                        }
                        );
                    }

                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string InsertCustomerService(dynamic PayObj,string disconame)
        {
            try
            {
            DataAccess.CustomerService _custServ = new DataAccess.CustomerService();
            _custServ.CustomerID = PayObj.CustomerID;
            _custServ.Merchant_FK = Convert.ToInt16(PayObj.MerchantFk);
            _custServ.CustomerIDLabel = disconame;
            _custServ.DateCreated = Utility.GetCurrentDateTime();
            _custServ.isVissible = 1;

            var custserv = _dc.InsertServiceList(_custServ);
            return null;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return ex.Message.ToString();
            }
        }
        [HttpGet]

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


        public ActionResult BoroPower()
        {
            try
            {
                //TempData["LogMsg"] = "";
                string user = LoggedInuser.LoggedInUser();
                // if (user == null || user == "" || user == ConfigurationManager.AppSettings["PaelytEmail"] )
                if (user == null || user == "")
                {
                   // TempData["LogMsg"] = "You Must Be Logged In to Borrow Power";
                    return RedirectToAction("Signin", "Home");
                    //, new {  massage = TempData["LogMsg"] });

                }
                /*  else if(user != null || user != "")
                  {
                      return View();
                  }
                 */
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult CheckEligibility(FormCollection form, Models.Power pw)
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
                    Power.CustomerObj PayObj = new Power.CustomerObj();
                    string Phones = "";
                    var MerchantFk = "";
                    var discoid = Convert.ToString(form["Disco"]);
                    PayObj.MerchantFk = discoid;
                    MerchantFk = discoid;
                    PayObj.MeterType = Convert.ToString(form["MeterType"]);

                    PayObj.Phone = pw.Phone;
                    PayObj.CustomerID = pw.CustomerID;
                    PayObj.Email = pw.Email;
                    PayObj.Amount = pw.Amount;

                    if (PayObj.Amount == "" || PayObj.CustomerID == "")
                    {
                      return View();
                    }

                    var CustomerDetails = Power.CheckMeterEligibilty(PayObj);

                    TempData["PayObj"] = PayObj;
                    dynamic jvalues = JObject.Parse(CustomerDetails.ToString());
                   
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "00")
                    {
                        string Amount = jvalues?.CreditLimit;
                        var Amt = MyUtility.ConvertToCurrency(Amount);
                        TempData["CreditLimit"] ="Customer Credit Limit : N" + Amt + " worth of power";
                        TempData["PayObj"] = PayObj;
                        return View();
                    }

                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() != "00")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        return View();
                        
                    }
                    return View();
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        [HttpGet]
        public ActionResult CheckEligibility()
        {
            try
            {
                //TempData["LogMsg"] = "";
                string user = LoggedInuser.LoggedInUser();
              
                if (user == null || user == "" )
                {
                   // TempData["LogMsg"] = "You Must Be Logged In to Borrow Power";
                    return RedirectToAction("Signin", "Home");
                    //, new {  massage = TempData["LogMsg"] });
                   
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
        public ActionResult CheckoutBoro(string DiscoId)
        {
            try
            {

                Models.Power powers = new Models.Power();
                if (DiscoId != null || DiscoId != "")
                {
                    Power.CustomerObj _co = new Power.CustomerObj();
                    _co = (Classes.Power.CustomerObj)TempData["PayObj"];
                    powers = (Models.Power)TempData["power"];
                    DiscoId = powers.Merchant_fk;
                    var Amount = Convert.ToDouble(powers.Amount);
                    var RealAmount = Power.ClaculateFifteenpercent(Amount);
                    var ConvFee = Amount - RealAmount;
                   // powers.Amount = RealAmount.ToString();
                    powers.ConvFee = ConvFee.ToString();
                    powers.Borovalue = RealAmount;
                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);
                    TempData["ErrorMessage"] = powers.ResponseDesrciption;
                    TempData["power"] = powers;
                    TempData["TotalAmt"] = RealAmount.ToString();
                    TempData.Keep("power");
                    return View(powers);
                }
                powers = (Models.Power)TempData["power"];
                if (powers.ToString() == null)
                {
                    return View();
                }
                if (powers.CustomerID == null || powers.CustomerID == "")
                {
                    return View();
                }
                else
                {
                    DiscoId = powers.Merchant_fk;
                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);
                    TempData["TotalAmt"] = Convert.ToDecimal(powers.ConvFee) + Convert.ToDecimal(powers.Amount);
                    return View(powers);
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool checkCustomerServices(string customerID,int MerchanFk)
        {
            try
            {
               var valid = _dr.checkCustomerServices(customerID,MerchanFk);
                return valid;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        [HttpPost]
        public ActionResult CheckoutBoro(Models.Power power, LoanViewModel lvm)
        {
           try
            {
                InsertTransactionLog(power);
                string MerchantFk = "";
                if (power.CustomerID == null || power.CustomerID == "")
                {
                    RedirectToAction("Index");
                }
                
                if (power.CustomerName != null || power.CustomerName != "")
                {
                    Power.CustomerObj _co = new Power.CustomerObj();
                    _co = (Classes.Power.CustomerObj)TempData["PayObj"];
                    //power = (Models.Power)TempData["power"];

                  if (power == null)
                 {
                        power = (Models.Power)Session["power"];
                        object values = TempData.Peek("power");
                        power = lvm.powerModel;
                        return View("BoroPower");
                 }
                    string DiscoId = power.Merchant_fk;
                    TempData["message"] = LoadImage(DiscoId);
                    TempData["Disconame"] = getDiscoName(DiscoId);

                    dynamic PayObjs = new JObject();
                   Classes.Power.CustomerObj pw = new Classes.Power.CustomerObj();
                    pw = (Classes.Power.CustomerObj)TempData["PayObj"];
                    Power.CustomerObj PayObj = new Power.CustomerObj();
                    PayObj.agentID = power.AgentID;
                    PayObj.Phone = power.Phone;
                    PayObj.CustomerID = power.CustomerID;
                    PayObj.Email = power.Email;
                    PayObj.Amount = power.Amount;
                    PayObj.CustomerName = power.CustomerName;
                    PayObj.MerchantFk = power.Merchant_fk;
                    PayObj.MeterType = power.TransactionType;
                    PayObj.Borovalue = Convert.ToString(power.Borovalue);
                    string Borovalue = PayObj.Borovalue;
                    TempData["PayRespone"] = PayObj;
                    ViewBag.power = TempData["power"];
                    Session["power"] = TempData["power"];
                    /// This is for Payment Page
                    /// 


                    string Firstname = PayObj.CustomerName;
                    string Email = PayObj.Email;
                    string PhoneNumber = PayObj.Phone;
                    string Amount = PayObj.Amount;
                    string ConvFee = ConfigurationManager.AppSettings["PowerConvFee"];
                    string refernceNum = power.CustReference;

                    var CustomerDetails = Power.PostBoroPowerJson(refernceNum, Borovalue);


                    if (CustomerDetails == null || CustomerDetails == "")
                    {
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Please Try Again";
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
               var Amounts = Convert.ToDouble(power.Amount);
               var RealAmount = Power.ClaculateFifteenpercent(Amounts);
               var ConvFees = Amounts - RealAmount;
               TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
                    }
                    TempData["PayObj"] = PayObj;
                    dynamic jvalues = JObject.Parse(CustomerDetails.ToString());
                  
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "00" )
                    {
                    TempData["PayObj"] = PayObj;
                    LoanViewModel model = new LoanViewModel();
                    Models.Power powers = JsonConvert.DeserializeObject<Models.Power>(CustomerDetails);
                  // powers.CustReference = Utility.TransactionGenerator();
                        powers.Amount = PayObj.Amount;
                        powers.Phone = PayObj.Phone;
                        powers.Merchant_fk = PayObj.MerchantFk;
                        MerchantFk = powers.Merchant_fk;
                        powers.TransactionType = PayObj.MeterType;
                        powers.TransactionType = PayObj.MeterType;
                        model.powerModel = powers;
                        var a = power.AccountNumber;
                        TempData["power"] = power;
                        _tL.ReferenceNumber = power.CustReference;
                        string ReferenceNum = _tL.ReferenceNumber;
                        _tL.Voucher = jvalues?.value;
                       _tL.ServiceValueDetails2 = jvalues?.units;
                        _tL.ServiceValueDetails3 = jvalues?.amount;
                        string id =  UpdateTransactionLog(_tL);
                        _ct.Amount = Convert.ToDouble(Amount);
                        _ct.CusPay2response = Convert.ToString(jvalues);
                        _ct.ReferenceNumber = _tL.ReferenceNumber;
                        _ct.Merchant_FK = Convert.ToInt16(MerchantFk);
                        _ct.ServiceDetails = "";
                        _ct.CustomerName = Firstname;
                        _ct.CustomerID = PayObj.CustomerID;
                        _ct.TransactionType = 3;
                       
                        var resr = _dc.UpdatePaymentLog(refernceNum);
                       
                        string CustTransid = InsertCustomertransaction(_ct);
                        
                        /*if(id != null || id != "") {
                             BoroReceipt(_tL.ReferenceNumber);
                             return RedirectToAction("BoroReceipt");
                         }*/
                        if(id != null || id != "") {
                            return RedirectToAction("BoroReceipt", new
                            {
                                @refNum = ReferenceNum
                            });
                        }
                        // return RedirectToAction("CheckoutBoro");
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "0X")
                    {
                        _tL.ResponseBody = Convert.ToString(jvalues);
                        string ids = _dc.UpdateTransactionLogResBody(_tL);
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
                        var Amounts = Convert.ToDouble(power.Amount);
                        var RealAmount = Power.ClaculateFifteenpercent(Amounts);
                        var ConvFees = Amounts - RealAmount;
                        TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() == "310")
                    {
                        _tL.ResponseBody = Convert.ToString(jvalues);
                        string ids = _dc.UpdateTransactionLogResBody(_tL);
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = Convert.ToString(jvalues?.respDescription);
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
                        var Amounts = Convert.ToDouble(power.Amount);
                        var RealAmount = Power.ClaculateFifteenpercent(Amounts);
                        var ConvFees = Amounts - RealAmount;
                        TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
                    }
                    if (jvalues != null && $"{jvalues?.respCode}".ToLower() != "00")
                    {
                        _tL.ResponseBody = Convert.ToString(jvalues);
                        string ids = _dc.UpdateTransactionLogResBody(_tL);
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
                        var Amounts = Convert.ToDouble(power.Amount);
                        var RealAmount = Power.ClaculateFifteenpercent(Amounts);
                        var ConvFees = Amounts - RealAmount;
                        TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
                    }
                    if (jvalues != null )
                    {
                        _tL.ResponseBody = Convert.ToString(jvalues);
                        string ids = _dc.UpdateTransactionLogResBody(_tL);
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = "Amount To Low";
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
                        var Amounts = Convert.ToDouble(power.Amount);
                        var RealAmount = Power.ClaculateFifteenpercent(Amounts);
                        var ConvFees = Amounts - RealAmount;
                        TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
                    }

                    if (jvalues == null || jvalues == "")
                    {
                        _tL.ResponseBody = Convert.ToString(jvalues);
                        string ids = _dc.UpdateTransactionLogResBody(_tL);
                        TempData["PayObj"] = PayObj;
                        TempData["ErrorMessage"] = jvalues?.respDescription;
                        //return RedirectToAction("CheckoutBoro", new
                        //{
                        //    @DiscoId = MerchantFk
                        //}
                        //);
                        var Amounts = Convert.ToDouble(power.Amount);
                        var RealAmount = Power.ClaculateFifteenpercent(Amounts);
                        var ConvFees = Amounts - RealAmount;
                        TempData["TotalAmt"] = RealAmount.ToString();
                        return View(power);
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

        public string UpdateTransactionLog(DataAccess.TransactionLog tLog)
        {
            try
            {
                string id =  _dc.UpdateTransactionLogx(tLog);
                return id;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }


      
        public string InsertCustomertransaction(DataAccess.CustomerTransaction cLog)
        {
            try
            {
               
                cLog.TrnDate = DateTime.Now;
                cLog.ValueDate = DateTime.Now.ToString("yyyy:mm:ss");
                cLog.ValueTime = DateTime.Now.ToString("yyyy:mm:ss");
                string id = _dc.UpdateCustTransactionLogx(cLog);
                return id;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return "";
            }
        }
        
        public ActionResult BoroReceipt(string refNum)
        {
            try
            {
                Classes.Power.Receipt Rec = new Classes.Power.Receipt();
                var getReceiptVal = _dr.GetRecord(refNum);
                Rec.CustomerID = getReceiptVal.CustomerID;
                Rec.CustomerName = getReceiptVal.CustomerName;
                Rec.Address = getReceiptVal.CustomerIDAddress;
                Rec.Phone = getReceiptVal.CustomerPhone;
                Rec.Amount = Convert.ToDouble(getReceiptVal.Amount);
                Rec.ServiceValueDetails2 = getReceiptVal.ServiceValueDetails2;
                Rec.ServiceValueDetails3 = getReceiptVal.ServiceValueDetails3;
                Rec.Voucher = getReceiptVal.Voucher;
                ViewBag.Token = Rec.Voucher;
                Rec.ReferenceNumber = getReceiptVal.ReferenceNumber;
                string MerchFK = Convert.ToString(Rec.Merchant_FK);
                var discname = getDiscoName(MerchFK);
                TempData["message"] = discname;

                return View(Rec);
            }
            catch(Exception ex)
            {
               // WebLog(ex.Message.ToString());
                return null;
            }
        }



     /*   public static bool PaymentRequery(string postData, string url, string hashValue, out string serverResponse, out TransactionalInformation trans)
        {
            WebLog.Log("postdata1" + postData);
            WebLog.Log("url2" + url);
            WebLog.Log("hashvalue3" + hashValue);

            trans = new TransactionalInformation();
            WebLog.Log("trans4" + trans);
            serverResponse = string.Empty;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            string responseString;
            try
            {
                _req = ReturnHeaderParameters("POST", url, hashValue, true);
                WebLog.Log("req5" + _req);
                WebLog.Log("url6" + url);
                WebLog.Log("hashvalue7" + hashValue);
                var data = Encoding.ASCII.GetBytes(postData);
                WebLog.Log("data8" + data);
                _req.ContentType = "application/json;charset=UTF-8";
                WebLog.Log("_req.ContentType9" + _req.ContentType);
                _req.ContentLength = data.Length;
                WebLog.Log("_req.ContentLength10" + _req.ContentLength);
                HttpStatusCode statusCode;
                //WebLog.Log("statusCode11" + statusCode.ToString());
                using (var stream = _req.GetRequestStream())
                {
                    WebLog.Log("stream11");
                    stream.Write(data, 0, data.Length);
                    using (_response = (HttpWebResponse)_req.GetResponse())
                    {
                        WebLog.Log("_response12" + _response);
                        statusCode = _response.StatusCode;
                        WebLog.Log("statusCode13" + statusCode);
                        responseString = new StreamReader
                            (_response?.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                        WebLog.Log("responseString14" + responseString);
                        serverResponse = responseString;
                        WebLog.Log("serverResponse15" + serverResponse);
                    }
                }
              
                var jvalue = JObject.Parse(responseString);
                WebLog.Log("jvalue16" + jvalue);
                //value to check. status code 200
                var httpStatusCode = (int)statusCode;
                WebLog.Log("httpStatusCode17" + httpStatusCode);
                if (httpStatusCode == 200)
                {
                    WebLog.Log("httpStatusCode18" + httpStatusCode);
                    trans.ReturnMessage.Add((string)jvalue["data"]["resp_desc"]);
                    WebLog.Log("(string)jvaluedata19" + (string)jvalue["data"]["resp_desc"]);
                    return trans.IsAuthenicated = ((string)jvalue["data"]["resp_code"]).Equals("00");

                }
            }
            //to read the body of the server _response when status code != 200
            catch (WebException exec)
            {
                var response = (HttpWebResponse)exec.Response;
                WebLog.Log("response20" + response);
                var dataStream = response.GetResponseStream();
                WebLog.Log("dataStream21" + dataStream);
                trans.ReturnMessage.Add(exec.Message);
                if (dataStream == null) return trans.IsAuthenicated;
                using (var tReader = new StreamReader(dataStream))
                {
                    WebLog.Log("tReader22" + tReader);
                    responseString = tReader.ReadToEnd();
                }
                //trans.ReturnMessage.Add(exec.Message);
                var jvalue = JObject.Parse(responseString);
                WebLog.Log("jvalue23" + jvalue);
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
        }*/

    }
}
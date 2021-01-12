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
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;

namespace GloballendingViews.Api
{
    public class StartimeController : Controller
    {
        // GET: Startime
        dynamic payObject = new JObject();
        string soapResult = "";
        string Resp = "";
        string MethodName = "";
        string returnMsg = "";
        string TELDealID = "";
        string returnCode = "000";
        string Error = "";
        Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
        Paytv.PaytvObj PaytvObj = new Paytv.PaytvObj();
        DataAccess.Agent AgentUser = new DataAccess.Agent();
        DataAccess.CustomerTransaction _ct = new DataAccess.CustomerTransaction();
        DataAccess.TransactionLog _tL = new DataAccess.TransactionLog();
        DataAccess.PaymentLog _pL = new DataAccess.PaymentLog();
        DataAccess.DataReaders _dr = new DataAccess.DataReaders();
        DataAccess.DataCreator _dc = new DataAccess.DataCreator();
        public ActionResult Index()
        {
            return View();
        }
       [HttpPost]
        public string ValidatePayRequest(string CustomerID)
        {
            try
            {
                dynamic AgentObj = new JObject();
                AgentObj.CustomerID = CustomerID;
                
                Internetserviceprovider.InternetServiceObj cusdataobj =
                   new Internetserviceprovider.InternetServiceObj();
               
                MethodName = ConfigurationManager.AppSettings["Fetchuser"];
                // var CustomerID = AgentObj.CustomerID;
                
                Internetserviceprovider.CustomerObj ispObject = new Internetserviceprovider.CustomerObj();
                //Customer Validation
                if (string.IsNullOrEmpty(CustomerID))
                {
                    soapResult =
                      new JObject(
                          new JProperty("status", "error"),
                          new JProperty("message", "Empty CustomerID"),
                          new JProperty("data", new JObject())).ToString();
                    //return Json(soapResult);
                    return soapResult.ToString();
                }

                var req = Request;
                var headers = req.Headers;
                string myAgentID = "";
                try
                {
                    myAgentID = headers.GetValues("agentID").First() ?? "";
                }
                catch
                {
                    WebLog.Log("Cannot Read AgentID as header");
                }

                var agentID = headers.GetValues("agentID").First() ?? "";
                var agentKey = headers.GetValues("agentKey").First() ?? "";
                var signature = headers.GetValues("signature").First() ?? "";

                AgentObj.AgentID = agentID;
                AgentObj.AgentKey = agentKey;
                AgentObj.Signature = signature;
                var Authenticated = IsTransactionAutheticated(AgentObj);

                if (Authenticated == false)
                {
                    soapResult =
                     new JObject(
                         new JProperty("status", "Error"),
                         new JProperty("message", "Authentication Failed"),
                         new JProperty("data", new JObject())).ToString();
                    // return Json(soapResult);
                    return soapResult.ToString();
                }
               // var CustomerID = ispObject.CustomerID;
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
                        var soapResults = soapResult.Substring(1);
                    }

                }
                if (soapResult == null)
                    //return Json(soapResult);
                    return soapResult.ToString();
                if (soapResult != null)
                    // return Json(soapResult);
                    return soapResult.ToString();
                // return Json(soapResult);
                    return soapResult.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                var obj =
                           new JObject(
                               new JProperty("status", "error"),
                               new JProperty("message", $"{ex.Message}"),
                               new JProperty("data", new JObject()));
                //return Json(obj);
                return obj.ToString();
            }
        }


        [HttpPost]
        public string ValidateStartimeRequest(string CustomerID,string Service,string Bouquet)
        {
            try
            {
               
                PayObj.CustomerID = CustomerID;
                PayObj.Bouquet = Bouquet;
                PayObj.Service = Service;
                
                Paytv _paytv = new Paytv();
                dynamic payObjx = new JObject();
                //Customer Validation
                if (string.IsNullOrEmpty(CustomerID))
                {
                    soapResult =
                      new JObject(
                          new JProperty("status", "error"),
                          new JProperty("message", "Empty CustomerID"),
                          new JProperty("data", new JObject())).ToString();
                   
                    return soapResult.ToString();
                }
                dynamic AgentObj = new JObject();
                var req = Request;
                var headers = req.Headers;
                string myAgentID = "";
                try
                {
                    myAgentID = headers.GetValues("agentID").First() ?? "";
                }
                catch
                {
                    WebLog.Log("Cannot Read AgentID as header");
                }

                var agentID = headers.GetValues("agentID").First() ?? "";
                var agentKey = headers.GetValues("agentKey").First() ?? "";
                var signature = headers.GetValues("signature").First() ?? "";

                AgentObj.AgentID = agentID;
                AgentObj.AgentKey = agentKey;
                AgentObj.Signature = signature;
                var Authenticated = IsTransactionAutheticated(AgentObj);

                if (Authenticated == false)
                {
                    soapResult =
                     new JObject(
                         new JProperty("status", "Error"),
                         new JProperty("message", "Authentication Failed"),
                         new JProperty("data", new JObject())).ToString();

                    return soapResult.ToString();
                }
               // var CustomerID = ispObject.CustomerID;
                payObjx.agentID = agentID;
              //  payObjx.PayResCode = 0;
                soapResult = _paytv.GetCustomerInfo(PayObj);

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(soapResult);
                XmlTextReader reader = default(XmlTextReader);
                XmlReader xReader = XmlReader.Create(new StringReader(soapResult));
                if (soapResult == null)
                {
                    return soapResult.ToString();
                }
                if (soapResult != null)
                {
                    using (StringReader stringReader = new StringReader(soapResult))
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
                                        // PaytvObj.customerName = Name;
                       payObjx.customername = Name;
                                    }
                                    if ((reader.Name == "balance"))
                                    {
                                        returnMsg = reader.ReadElementString();
                                        payObjx.Balance = returnMsg;
                                    }
                                   
                                    if ((reader.Name == "smartCardCode"))
                                    {
                                        returnMsg = reader.ReadElementString();
                                        var Name = returnMsg;
                                        payObjx.smartCardCode = Name;
                                    }
                                    if ((reader.Name == "canOrderProductInfos"))
                                    {
                                        returnMsg = reader.ReadElementString();
                                        var canOrderProductInfos = returnMsg;
                                        payObjx.smartCardCode = canOrderProductInfos;
                                    }
                                    if ((reader.Name == "transactionlNo"))
                                    {

                        var tranNum = reader.ReadElementString();
                        PaytvObj.transactionlNo = tranNum;
                        payObjx.transactionlNo = tranNum;
                                        //var json = new JavaScriptSerializer().Serialize(PaytvObj);

                                        //soapResult = json;
                        //payObjx.returnCode = 0;
                        //payObjx.orderedProductsDesc = "Basic Bouqet";
                        //payObjx.subscriberStatus = "0";
                        //payObjx.returnMsg = "moving";
                        //payObjx.Amount = "200";
                        //payObjx.CustomerName = "tolulope";
                        //payObjx.smartCardCode = "12322222";
                        //                payObjx.tranNum = "vbvvbbbb";
                        //                payObjx.Phone = "08077755537";
                        //                payObjx.agentID = "uuuuuuu";
     string json = Newtonsoft.Json.JsonConvert.SerializeObject(payObjx);
                                        soapResult = json;
                                    }
                                    break;
                                case XmlNodeType.Text:

                                    break;
                                case XmlNodeType.EndElement:

                                    break;
                            }
                        }
                    }
                    //  insertTransactionLog(payObjx);
                    QueryStartimeBalance(payObjx);
                    return soapResult.ToString();
                }
                return soapResult.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                var obj =
                           new JObject(
                               new JProperty("status", "error"),
                               new JProperty("message", $"{ex.Message}"),
                               new JProperty("data", new JObject()));
                //return Json(obj);
                return obj.ToString();
            }
        }


        public string InsertTransactionLog()
        {
            try
            {
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        [HttpPost]
        public string BuyStartimeRequest(string CustomerID, string Amount, string Phone,string Bouqet)
        {
            try
            {
                Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                dynamic payObjx = new JObject();
                PayObj.CustomerID = CustomerID;
                PayObj.Amount = Amount;
                PayObj.Phone = Phone;
                PayObj.Bouquet = Bouqet;
               // UpdateCustomerTransaction(payObject);
                Paytv _paytv = new Paytv();
                //Customer Validation
                if (string.IsNullOrEmpty(CustomerID))
                {
                    soapResult =
                      new JObject(
                          new JProperty("status", "error"),
                          new JProperty("message", "Empty CustomerID"),
                          new JProperty("data", new JObject())).ToString();
                    return soapResult.ToString();
                }
                dynamic AgentObj = new JObject();
                var req = Request;
                var headers = req.Headers;
                string myAgentID = "";
                try
                {
                    myAgentID = headers.GetValues("agentID").First() ?? "";
                }
                catch
                {
                    WebLog.Log("Cannot Read AgentID as header");
                }

                var agentID = headers.GetValues("agentID").First() ?? "";
                var agentKey = headers.GetValues("agentKey").First() ?? "";
                var signature = headers.GetValues("signature").First() ?? "";

                AgentObj.AgentID = agentID;
                AgentObj.AgentKey = agentKey;
                AgentObj.Signature = signature;
                var Authenticated = IsTransactionAutheticated(AgentObj);

                if (Authenticated == false)
                {
                    soapResult =
                     new JObject(
                         new JProperty("status", "Error"),
                         new JProperty("message", "Authentication Failed"),
                         new JProperty("data", new JObject())).ToString();
                    // return Json(soapResult);
                    return soapResult.ToString();
                }
                payObjx.agentID = agentID;
                payObjx.Phone = Phone;
                payObjx.smartCardCode = CustomerID;
                payObjx.Amount = PayObj.Amount;
                payObjx.PayResCode = payObjx.returnCode;
                payObjx.Bouqet = PayObj.Bouquet;
                soapResult = _paytv.PaySubscription(PayObj,Phone);

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(soapResult);
                XmlTextReader reader = default(XmlTextReader);
                XmlReader xReader = XmlReader.Create(new StringReader(soapResult));
                using (StringReader stringReader = new StringReader(soapResult))
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
                                if ((reader.Name == "Phone"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    payObjx.Phone = returnMsg;

                                }
                                
                                if ((reader.Name == "transactionNo"))
                                {

                         var tranNum = reader.ReadElementString();
                         //payObjx.tranNum = tranNum;
                         payObjx.transactionlNo = tranNum;
                                    //payObjx.Phone = Phone;
                                    //var json = new JavaScriptSerializer().Serialize(payObjx);

                                    // soapResult = json;
                                    string json = Newtonsoft.Json.JsonConvert.SerializeObject(payObjx);
                                    soapResult = json;
                                }
                                break;
                            case XmlNodeType.Text:

                                break;
                            case XmlNodeType.EndElement:

                                break;
                        }
                    }
                }

                  //insertTransactionLog(payObjx);
                QueryStartimeBalance(payObjx);
                return soapResult.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                var obj =
                           new JObject(
                               new JProperty("status", "error"),
                               new JProperty("message", $"{ex.Message}"),
                               new JProperty("data", new JObject()));
               
                return obj.ToString();
           }
        }




        public string QueryStartimeBalance(dynamic payObjx)
        {
            try
            {
                Paytv.CustomerObj PayObj = new Paytv.CustomerObj();
                //dynamic payObjx = new JObject();
                PayObj.CustomerID = payObjx.smartCardCode;
                PayObj.Amount = payObjx.Amount;
                PayObj.Phone = payObjx.Phone;
                
                string transactionID = payObjx.transactionlNo;
                //string transactionID = "movingtotestEnvironment";
                //PayObj.Service = payObjx.transactionlNo;
                PayObj.Service = transactionID;
                // UpdateCustomerTransaction(payObject);
                Paytv _paytv = new Paytv();
                //Customer Validation
                if (string.IsNullOrEmpty(PayObj.CustomerID))
               {
                    soapResult =
                      new JObject(
                          new JProperty("status", "error"),
                          new JProperty("message", "Empty CustomerID"),
                          new JProperty("data", new JObject())).ToString();
                    return soapResult.ToString();
                }
                dynamic AgentObj = new JObject();
                // var req = Request;
                // var headers = req.Headers;
                // string myAgentID = "";
                // try
                // {
                //     myAgentID = headers.GetValues("agentID").First() ?? "";
                // }
                // catch
                // {
                //     WebLog.Log("Cannot Read AgentID as header");
                // }

                // var agentID = headers.GetValues("agentID").First() ?? "";
                // var agentKey = headers.GetValues("agentKey").First() ?? "";
                // var signature = headers.GetValues("signature").First() ?? "";

                // AgentObj.AgentID = agentID;
                // AgentObj.AgentKey = agentKey;
                // AgentObj.Signature = signature;
                // var Authenticated = IsTransactionAutheticated(AgentObj);

                // if (Authenticated == false)
                // {
                //     soapResult =
                //      new JObject(
                //          new JProperty("status", "Error"),
                //          new JProperty("message", "Authentication Failed"),
                //          new JProperty("data", new JObject())).ToString();
                //     // return Json(soapResult);
                //     return soapResult.ToString();
                // }
                // payObjx.agentID = agentID;
                //// payObjx.Phone = Phone;
                payObjx.PayResCode = payObjx.returnCode;
                soapResult = _paytv.GetBalance(PayObj);

                XmlDocument xd = new XmlDocument();
                xd.LoadXml(soapResult);
                XmlTextReader reader = default(XmlTextReader);
                XmlReader xReader = XmlReader.Create(new StringReader(soapResult));
                using (StringReader stringReader = new StringReader(soapResult))
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
                                if ((reader.Name == "balance"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    payObjx.Amount = returnMsg;
                                }
                                if ((reader.Name == "customerName"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    var Name = returnMsg;
                                    payObjx.CustomerName = Name;
                                }
                                if ((reader.Name == "smartCardCode"))
                                {
                                    returnMsg = reader.ReadElementString();
                                    var Name = returnMsg;
                                    payObjx.smartCardCode = Name;
                                }
                                if ((reader.Name == "TELDealID"))
                                {

                    var tranNum = reader.ReadElementString();
                    payObjx.tranNum = tranNum;
                    //payObjx.Phone = Phone;
                   //var json = new JavaScriptSerializer().Serialize(payObjx);

                    // soapResult = json;
                   string json = Newtonsoft.Json.JsonConvert.SerializeObject(payObjx);
                   soapResult = json;
                                }
                   break;
                   case XmlNodeType.Text:

                   break;
                   case XmlNodeType.EndElement:

                                break;
                        }
                    }
                }

                insertTransactionLog(payObjx);

                return soapResult.ToString();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                var obj =
                           new JObject(
                               new JProperty("status", "error"),
                               new JProperty("message", $"{ex.Message}"),
                               new JProperty("data", new JObject()));

                return obj.ToString();
            }
        }

        public void insertTransactionLog(dynamic Jobj)
        {
            try
            {
/*payObjx.returnCode
payObjx.orderedProductsDesc
payObjx.subscriberStatus
payObjx.returnMsg
payObjx.Amount
payObjx.CustomerName
payObjx.smartCardCode
payObjx.tranNum
payObjx.Phone
payObjx.agentID
*/
                //Customer Log
                _ct.Amount = Jobj.Amount;
                _ct.ReferenceNumber = Jobj.tranNum;
                _ct.ServiceDetails = Jobj.Bouqet ;
                _ct.ValueDate = DateTime.Today.ToString();
                _ct.TrnDate = DateTime.Today;
                _ct.ValueTime = DateTime.Today.TimeOfDay.ToString();
                string agentID = Jobj.agentID;
                string MerchantID = _dr.GetMerchantID(agentID);
                _ct.Merchant_FK = Convert.ToInt16(MerchantID);
                _ct.CusPay2response = Convert.ToString(Jobj);
                _ct.CustomerID = Jobj.smartCardCode;
                _ct.CustomerName = Jobj.CustomerName;
                _dc.InitiateCustomerTransaction(_ct);
                //Transaction Log 
                _tL.Amount = Jobj.Amount;
                _tL.ReferenceNumber = Jobj.tranNum;
                _tL.ServiceDetails = Jobj.Bouqet;
                _tL.ValueDate = DateTime.Today.ToString();
                _tL.TrnDate = DateTime.Today;
                _tL.ValueTime = DateTime.Today.TimeOfDay.ToString();
                //_tL.Merchant_FK = _dr.GetMerchantID(agentID);
                _tL.Merchant_FK = Convert.ToInt16(MerchantID);
                _tL.CustomerID = Jobj.smartCardCode;
                _tL.CustomerName = Jobj.CustomerName;
                _dc.InitiateTransacLog(_tL);
               
               // _pL.ServiceCharge = ConfigurationManager.AppSettings["SpectranetKnowYourOffer"];
                if (Jobj.PayResCode == 0)
                {
                  _pL.ResponseCode = "00";
                  _pL.ResponseDescription = "successful";

                    //Payment Log
                    _pL.Amount = Jobj.Amount;
                    _pL.ReferenceNumber = Jobj.tranNum;
                    _pL.CustomerPhoneNumber = Jobj.Phone;
                    _pL.ValueDate = DateTime.Today.ToString();
                    _pL.TrnDate = DateTime.Today;
                    _pL.ValueTime = DateTime.Today.TimeOfDay.ToString();
                    _pL.CustomerID = Jobj.smartCardCode;
                    _dc.InitiatePaymentLog(_pL);
                }
               
            }
            catch(Exception ex)
            {

            }
        }
        //public HttpResponseMessage ValidatePayRequest(dynamic payObject)
        //{
        //    try
        //    {
        //        // dynamic payObject = new JObject();
        //        if (payObject == null) return ValidatePayResponse(null, TrxResponse.InvalidRequest);

        //        // Customer Validation
        //        var Resp = "";
        //        if (Resp == null) return ValidatePayResponse(payObject, TrxResponse.InvalidMerchant);



        //        publicKey = payObject.PublicKey;
        //        // macKey = merchant.SecretKey;
        //        customerName = payObject.CustomerName;
        //        phone = payObject.Phone;
        //        email = payObject.Email;
        //        currency = payObject.Currency;
        //        narration = payObject.Narration;
        //        product = payObject.Product;

        //        //CustomerId Validation
        //        if (string.IsNullOrEmpty(payObject.CustomerId)) return ValidatePayResponse(payObject, TrxResponse.InvalidCustomerId);
        //        var customerId = payObject.CustomerId;

        //        //Amount Validation
        //        if (string.IsNullOrWhiteSpace(payObject.Amount)) return ValidatePayResponse(payObject, TrxResponse.MissingTransactionAmount);
        //        decimal amount;
        //        //amount = 2000;
        //        var isValidAmount = decimal.TryParse(payObject.Amount, out amount);
        //        if (!isValidAmount || amount <= 0) return ValidatePayResponse(payObject, TrxResponse.InvalidTransactionAmount);

        //        //Transaction Validation
        //        if (string.IsNullOrWhiteSpace(payObject.TrxRef))
        //            return ValidatePayResponse(payObject, TrxResponse.MissingTransactionReference);

        //        var trx = dataprovider.GetByTransactionRef(payObject.TrxRef);

        //        if (trx != null)
        //        {
        //            var TrxRef = trx.TrxRef;
        //            var trxStatus = trx.Status;

        //            //var isTrxRefDuplicate = trx != null && trx.Status != Convert.ToInt16(TrxStatus.Initiated);
        //            var isTrxRefDuplicate = TrxRef != null && trxStatus != Convert.ToString(TrxStatus.Initiated);
        //            if (isTrxRefDuplicate) return ValidatePayResponse(payObject, TrxResponse.DuplicateTransactionReference);
        //        }
        //        var trxRef = payObject.TrxRef;

        //        //Return Url Validation
        //        if (string.IsNullOrWhiteSpace(payObject.ReturnUrl)) return ValidatePayResponse(payObject, TrxResponse.MissingReturnUrl);
        //        var isValidUrl = payObject.ReturnUrl.IsValidUrl();
        //        if (!isValidUrl) return ValidatePayResponse(payObject, TrxResponse.InvalidReturnUrl);
        //        var returnUrl = payObject.ReturnUrl;

        //        //Hash Validation
        //        if (string.IsNullOrWhiteSpace(payObject.Hash)) return ValidatePayResponse(payObject, TrxResponse.MissingTransactionHash);

        //        var plainText = $"{publicKey}{product}{trxRef}{customerId}{customerName}{email}{phone}{amount}{currency}{narration}{returnUrl}{macKey}";

        //        var hash = payObject.Hash;
        //        var isRequestValid = new CryptographyManager().VerifyHash(plainText, hash, HashName.SHA256);
        //        return ValidatePayResponse(payObject, Convert.ToBoolean(isRequestValid) ? TrxResponse.Successful : TrxResponse.DataIntegrityError);
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //        return ValidatePayResponse(payObject, TrxResponse.TransactionError);
        //    }
        //}
        //public HttpResponseMessage ValidatePayResponse(dynamic payObject, TrxResponse trxResponse)
        //{
        //    var obj =
        //        new JObject(
        //                   new JProperty("status", "success"),
        //                   new JProperty("message", $"{SplitCase(trxResponse.ToString())}"),
        //                   new JProperty("data",
        //                   new JObject(
        //                       new JProperty("trxref", $"{payObject.TrxRef}"),
        //                       new JProperty("trxtoken", $"{Utility.RandomString(20)}"),
        //                       new JProperty("resp_code", $"{((int)trxResponse).ToString().PadLeft(2, '0')}"),
        //                       new JProperty("resp_desc", $"{SplitCase(trxResponse.ToString())}"),
        //                       new JProperty("value_date", $"{DateTime.Now:G}")))
        //                   );
        //    return Json(obj);
        //}

        public new HttpResponseMessage Json(object value)
        {
            try
            {

                var resp = JsonSerialize(value);
                var response = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(resp, Encoding.UTF8, "application/json")
                };
                return response;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return null;
            }
        }
        public static string JsonSerialize(object obj)
        {
            try
            {
                var stringifiedObj = JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                        //ContractResolver = new NHibernateContractResolver()
                    });
                return stringifiedObj;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return null;
            }
        }
        public static string SplitCase(string source)
        {
            var builder = new StringBuilder();
            foreach (var c in source)
            {
                if (char.IsUpper(c) && builder.Length > 0) builder.Append(' ');
                builder.Append(c);
            }
            source = builder.ToString();
            return source;
        }
       private static bool IsTransactionAutheticated(dynamic AgentObj)
        {
           dynamic agObj = new DataAccess.Agent();
            HttpRequestMessage req = new HttpRequestMessage();
            try
            {
                
            var agentID = AgentObj.AgentID;
            var agentKey = AgentObj.AgentKey;
            string signature = AgentObj.Signature;
            string agtKey = agentKey;
            string agtID = agentID;
            
            WebLog.Log("agentID:" + agentID + " agentKey:" + agentKey + " signature:" + signature);
                //get details from database.

                DataAccess.GlobalTransactEntitiesData globallending = new DataAccess.GlobalTransactEntitiesData();
                var trxRecord = globallending.AgentUsers.Where(x => x.AgentID == agtID && x.AgentKey == agtKey).FirstOrDefault();
            
                string myEmail = "";
               
                if (trxRecord != null)
                {
                    WebLog.Log("agentID0:" + agentID + " agentKey0:" + agentKey + " signature:" + signature);

                    // myEmail = trxRecord.EmailAddress;
                    myEmail = trxRecord.EmailAddress;
                    string plainText = agentID + agentKey + myEmail;

            CryptographyManager cryp = new CryptographyManager();
                    
            if (!cryp.VerifyHash(plainText, signature,HashName.SHA256))
              {
                        WebLog.Log("VerifyHash error:");
                        return false;
              }
                    agObj = trxRecord;
                    WebLog.Log("VerifyHash: sucess:");
                    return true;

                }
                //  WebLog.Log("agentID2:");
                return false;

            }
            catch (HttpRequestException ex)
            {
                WebLog.Log(ex.Message + "##AgentTransactController:ValidateCustomerID" + ex.StackTrace);
                WebLog.Log("agentIDx:error here");
                return false;
            }
        }
   
    }
}
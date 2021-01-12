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

namespace GlobalLending.Api
{
    public class UtilityController : Controller
    {

        dynamic payObject = new JObject();
        string publicKey = "", macKey = "", customerName = "", phone = "", email = "", currency = "", narration = "", product = "";
        DataAccess.Agent AgentUser = new DataAccess.Agent();
        // GET: Utility
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
                string soapResult = "";
                var Resp = "";
                string MethodName = "";
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
                          new JProperty("data", new JObject(soapResult))).ToString();
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
                var stringifiedObj = JsonConvert.SerializeObject(obj, Formatting.Indented,
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
                var signature = AgentObj.Signature;
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

                     myEmail = trxRecord.EmailAddress;
                    
                    string plainText = agentID + agentKey + myEmail;

                    CryptographyManager cryp = new CryptographyManager();

                    if (!cryp.VerifyHash(plainText, signature, HashName.SHA256))
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


        public enum TrxResponse
        {
            Successful = 00,
            PendingOtpvalidation = 02,
            OtpValidationFailed = 03,
            TransactionError = 050,
            InvalidMerchant = 100,
            InvalidCustomerId = 101,
            MissingTransactionAmount = 102,
            InvalidTransactionAmount = 103,
            DuplicateTransactionReference = 104,
            InvalidTransactionReference = 105,
            InvalidReturnUrl = 106,
            MissingTransactionHash = 107,
            DataIntegrityError = 108,
            InvalidGateway = 109,
            MissingTransactionReference = 110,
            MissingReturnUrl = 111,
            InvalidTransactionCurrency = 112,
            MissingTransactionCurrency = 113,
            UserCancelled = 114,
            NoSuchRequest = 115,
            TransactionFailed = 116,
            GatewayAuthenticationError = 117,
            TransactionAmountMismatch = 118,
            InvalidProductReference = 119,
            TransactionProcessing = 120,
            DataNotAvailable = 121,
            InvalidRequest = 122,
            TransactionRecordNotFound = 123,
            IssuerOrSwitchInoperative = 124,
            AuthorizationFailed = 125,
            InvalidPublikeyORSecretKey = 121
        }

    }
}
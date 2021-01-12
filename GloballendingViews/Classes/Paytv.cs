using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GloballendingViews.HelperClasses;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Net;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace GloballendingViews.Classes
{
    public class Paytv
    {
        string returnMsg = "";
        string soapResult = "";
        string TELDealID = "";
        string returnCode = "000";
        string transactionNum = DateTime.Now.ToString("yyyyMMddHHmmss");
        string Error = "";
        string TransactionNo = "";
        string Plaincode = ConfigurationManager.AppSettings["Plaincode"];
        string payerId = ConfigurationManager.AppSettings["PayerID"];
        string payerPwd = "";


        public class CustomerObj
        {
            
            public string MeterType { get; set; }
            public string CustomerID { get; set; }

            public string Phone { get; set; }

            public string Email { get; set; }

            public string Amount { get; set; }

            public string agentkey { get; set; }

            public string agentID { get; set; }

            public string MerchantFk { get; set; }

            public string CustomerName { get; set; }

            public string Bouquet { get; set; }

            public string Service { get; set; }

            public int paymentType { get; set; }

            public int paymentPlanId { get; set; }

        }


        public class Menus
        {
            public string pageName { get; set; }

            public int roleid { get; set; }

            public string pageheader { get; set; }

            public string pageurl { get; set; }


        }

        public class PaytvObj
        {

            public string transactionlNo { get; set; }
            public string customerName { get; set; }
            public string Amount { get; set; }
            public string CustomerID { get; set; }
            public string Bouquet { get; set; }
            public string Service { get; set; }

            public string ConvFee { get; set; }

            public string BillerName { get; set; }

            public string BillerImg { get; set; }

            public string Phone { get; set; }

            public string Email { get; set; }

            public int paymentType { get; set; }

            public int paymentPlanId { get; set; }

            public int MerchantId { get; set; }

            public string ErrorMsg { get; set; }
            public string paymethod { get; set; }

            public string TotalAmt { get; set; }
            public string Amounts { get; set; }
        }
        public class Receipt
        {
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
            public int Customer_FK { get; set; }
            public double Amount { get; set; }
            public Double ServiceCharge { get; set; }
            public DateTime TrnDate { get; set; }
            public int TransactionType { get; set; }
            public string ValueDate { get; set; }
            public string ValueTime { get; set; }
            public string ServiceDetails { get; set; }
            public int Merchant_FK { get; set; }
            public string ReferenceNumber { get; set; }
            public string Phone { get; set; }
            public string Service { get; set; }
            public string transactionlNo { get; set; }
            public string customerName { get; set; }
            public string ConvFee { get; set; }
            public string ErrorMsg { get; set; }
        }

        public class Report
        {
            public string Amount { get; set; }
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string Customer_FK { get; set; }
            public string Merchant_FK { get; set; }
            public string ReferenceNumber { get; set; }
            public string ServiceCharge { get; set; }
            public string ServiceDetails { get; set; }
            public string TransactionType { get; set; }
            public string TrnDate { get; set; }
            public string TrxToken { get; set; }
            public string ValueDate { get; set; }
            public string ValueTime { get; set; }

            public string ResponseCode { get; set; }

            public string Description { get; set; }

            public string Phone { get; set; }
        }



        public static string GetSessionID()
        {
            try
            {
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                string EmailAddress = ConfigurationManager.AppSettings["agentEmail"];

                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(EmailAddress);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);

                dynamic obj = new JObject();
                obj.agentkey = agentKey;
                obj.signature = signature;

                var json = obj.ToString();
                var PostUrl = ConfigurationManager.AppSettings["GetSessionID"];

                PostUrl = System.Configuration.ConfigurationManager.AppSettings["GetSessionID"];

                PostUrl = PostUrl.Replace("{$agentid}", agentid.ToString()).Trim().Replace("{$agentkey}", agentKey.ToString()).Trim().Replace("{$signature}", signature.ToString()).Trim();

                var data = Utility.DoGet1($"{PostUrl}", agentid, agentKey, signature);
                dynamic session = JObject.Parse(data);
                var sessionID = session?.sessionid;
                return sessionID;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        


         public string ValidateCableTVRequest(CustomerObj cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                string agentID = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                string email = ConfigurationManager.AppSettings["agentEmail"];
                var MerchantFk = cusObj.MerchantFk;
                var MeterType = cusObj.MeterType;
              
                obj.beneficiary = cusObj.CustomerID;
                obj.MerchantFK = MerchantFk;
            

                var plainText = (agentID + agentKey + obj.customerId);
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentID).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                //obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("json::" + json);
                string sessionID = GetSessionID();
                var PostUrl = ConfigurationManager.AppSettings["ValidateCableTVRequest"];
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentID, agentKey, signature, sessionID);
               
              
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string GetCustomerDetails(CustomerObj cusObj)
        {
            try
            {
                dynamic obj = new JObject();
 string agentID = ConfigurationManager.AppSettings["agentID"];
 string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var MerchantFk = cusObj.MerchantFk;
                var MeterType = cusObj.MeterType;
                obj.agentkey = agentKey ;
                obj.agentID = agentID;
                obj.customerId = cusObj.CustomerID;
                obj.MerchantFK = MerchantFk;
                obj.accountType = MeterType;
              

                var plainText = (agentID + agentKey + obj.customerId);
                var builder = new StringBuilder();
                builder.Append(agentID).Append(agentKey).Append(obj.customerId);
               
                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA256);
                obj.hashValue = hash;
                var json = obj.ToString();
                var PostUrl = ConfigurationManager.AppSettings["ValidateCustomerID"];
                var data = Utility.DoPosts(json, $"{PostUrl}");
                return data;

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public static string PostBuySubcriptionJson(CustomerObj cusObj)
        {
            try
            {
                string agentID = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];

       dynamic obj = new JObject();
       string valueTime = DateTime.Now.ToString("Hmmss");
        obj.refNumber ="Agent"+ DateTime.Now.ToString("yyyyMMdd") + valueTime;
       obj.customername = cusObj.CustomerName;
       obj.amount = cusObj.Amount;
       obj.agentkey = agentKey;
       obj.agentID = agentID;
       obj.customerId = cusObj.CustomerID;
       obj.MerchantFK = cusObj.MerchantFk;
       obj.accountType = cusObj.MeterType;

     var plainText = (agentID + agentKey + obj.customerId);
     var builder = new StringBuilder();
     builder.Append(agentID).Append(agentKey).Append(obj.customerId);
     var hash = new CryptographyManager().ComputeHash(builder.ToString(),HashName.SHA256);
        obj.hashValue = hash;
        var json = obj.ToString();
      var PostUrl = ConfigurationManager.AppSettings["Buypower"];
        var data = Utility.DoPosts(json, $"{PostUrl}");
          return data;
     }
            catch (Exception ex)
     {
         WebLog.Log(ex.Message.ToString());
        //Label1.Text = ex.Message.ToString();
         return null;
     }
     }
        public string TransactionNumb()
        {
            try
            {
                
                return DateTime.Now.ToString("yyyyMMddHHmmss");

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        
        public string SerialNum()
        {
            try
            {
                string RefNum = Utility.RandomString(5);
                return RefNum;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string GetPriceList(dynamic cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var email = ConfigurationManager.AppSettings["agentEmail"];
                //obj.agentkey = agentKey;
                obj.beneficiary = cusObj.beneficiary;
                obj.MerchantFK = cusObj.MerchantFk;
                //obj.serviceType = 1;
               

                //var builder = new StringBuilder();
                //builder.Append(agentid).Append(agentKey).Append(obj.customerId);

                //var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA256);

                // For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                //obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("json::" + json);
                string sessionID = GetSessionID();
                WebLog.Log("SessionID" + sessionID);
                var PostUrl = ConfigurationManager.AppSettings["GetCableTVPriceList"]; 
               
                 var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);

                WebLog.Log("PostUrl" + PostUrl + agentid + agentKey);
                WebLog.Log("json" + json);
                WebLog.Log("UrlPosted" + PostUrl);
                WebLog.Log("data" + data);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public string payDstvGotv(dynamic cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var email = ConfigurationManager.AppSettings["agentEmail"];
                //obj.agentkey = agentKey;
                obj.customerId = cusObj.CustomerID;
                obj.MerchantFK = cusObj.Merchant_FK;
                obj.Amount = cusObj.Amount;
                obj.Bouquet = cusObj.ServiceDetails;
                obj.CustomerPhone = cusObj.CustomerPhone;
                obj.refNumber = cusObj.ReferenceNumber;
               
                var builder = new StringBuilder();
                builder.Append(agentid).Append(agentKey).Append(obj.customerId).Append(obj.Amount);
               // var plainText = (agentid + agentKey + obj.CustomerID + obj.Amount);
                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("BuyPayLoad" + json);
                string sessionID = GetSessionID();
                WebLog.Log("SessionID" + sessionID);
                var PostUrl = ConfigurationManager.AppSettings["BuyCableTV"];
                WebLog.Log("BuyPayLoad" + json);
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);


                WebLog.Log("BuyPayLoad" + json);
                WebLog.Log("PostUrl" + PostUrl + agentid + agentKey);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public string GetCustomerInfo(CustomerObj _cobj)
        {
            try
            {
                dynamic payObj = new JObject();
                string payerPwd = new CryptographyManager().ComputeHash(Plaincode, HashName.MD5);
                TransactionNo = payerId + TransactionNumb() + SerialNum();//"420020181227072633oNl8R"; 
                payObj.CustomerID =_cobj.CustomerID; 
                //"02146506386"; 
                payObj.ServiceAmt = _cobj.Amount;
                payObj.Service = _cobj.Service;
                payObj.Bouquet = _cobj.Bouquet;
                var smartCardCode = _cobj.CustomerID;
                string methodName = ConfigurationManager.AppSettings["querySubscriberInfo"];
                WebLog.Log("method Name :" + methodName.ToString());
                HttpWebRequest request = CreateWebRequest(methodName);
                WebLog.Log("request Url :" + request.RequestUri + request.ServerCertificateValidationCallback);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var xmlObject = File.ReadAllText(HttpContext.Current.Server.MapPath("/Xmlfiles/StattimeGetCustomerInfo.xml"));
                xmlObject = xmlObject.Replace("$payerID", payerId);
                xmlObject = xmlObject.Replace("$payerPwd", payerPwd);
                xmlObject = xmlObject.Replace("$smartCardCode", smartCardCode);
                xmlObject = xmlObject.Replace("$transactionNo", TransactionNo);
                soapEnvelopeXml.LoadXml(xmlObject);
                WebLog.Log("xmlObject:" + xmlObject);
                using (Stream stream = request.GetRequestStream())
                {
                    WebLog.Log("stream:" + stream);
                    soapEnvelopeXml.Save(stream);
                    WebLog.Log("stream:" + stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    WebLog.Log("response:" + response);
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        WebLog.Log("soapResult:" + soapResult);
                        soapResult = rd.ReadToEnd();
                        returnMsg = soapResult;
                        WebLog.Log("soapResult:" + soapResult);
                        WebLog.Log("returnMsg:" + returnMsg);
                    }
                    
                }
                return returnMsg;
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                Error = ex.Message;
                return Error;
            }
        }


       // public string GetBalance(CustomerObj _cobj)
       public string GetBalance(dynamic _cobj)
        {
            try
            {
                dynamic payObj = new JObject();
                string payerPwd = new CryptographyManager().ComputeHash(Plaincode, HashName.MD5);
                TransactionNo = Convert.ToString(_cobj.ReferenceNum); //
                payObj.CustomerID = _cobj.CustomerID;
               
                //payObj.ServiceAmt = _cobj.Amount;
                //payObj.Service = _cobj.Service;
                //payObj.Bouquet = _cobj.Bouquet;
                string smartCardCode = _cobj.CustomerID;
                string methodName = ConfigurationManager.AppSettings["queryBalance"];
                WebLog.Log("method Name :" + methodName.ToString());
                HttpWebRequest request = CreateWebRequest(methodName);
                WebLog.Log("request Url :" + request.RequestUri + request.ServerCertificateValidationCallback);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var xmlObject = File.ReadAllText(HttpContext.Current.Server.MapPath("/Xmlfiles/queryBalance.xml"));
                xmlObject = xmlObject.Replace("$payerID", payerId);
                xmlObject = xmlObject.Replace("$payerPwd", payerPwd);
                xmlObject = xmlObject.Replace("$smartCardCode", smartCardCode);
                xmlObject = xmlObject.Replace("$transactionNo", TransactionNo);
                soapEnvelopeXml.LoadXml(xmlObject);
                WebLog.Log("xmlObject:" + xmlObject);
                using (Stream stream = request.GetRequestStream())
                {
                    WebLog.Log("stream:" + stream);
                    soapEnvelopeXml.Save(stream);
                    WebLog.Log("stream:" + stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    WebLog.Log("response:" + response);
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        WebLog.Log("soapResult:" + soapResult);
                        soapResult = rd.ReadToEnd();
                        returnMsg = soapResult;
                        WebLog.Log("soapResult:" + soapResult);
                        WebLog.Log("returnMsg:" + returnMsg);
                    }

                }
                return returnMsg;
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                Error = ex.Message;
                return Error;
            }
        }

        public string TransactionStatus(string referenceNum,string CustId)
        {
            try
            {
                dynamic payObj = new JObject();
                string payerPwd = new CryptographyManager().ComputeHash(Plaincode, HashName.MD5);
                TransactionNo = referenceNum;
                payObj.CustomerID = CustId;
                var smartCardCode = CustId;
                string methodName = ConfigurationManager.AppSettings["querySubscriberInfo"];
                WebLog.Log("method Name :" + methodName.ToString());
                HttpWebRequest request = CreateWebRequest(methodName);
                WebLog.Log("request Url :" + request.RequestUri + request.ServerCertificateValidationCallback);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var xmlObject = File.ReadAllText(HttpContext.Current.Server.MapPath("/Xmlfiles/StattimeGetCustomerInfo.xml"));
                xmlObject = xmlObject.Replace("$payerID", payerId);
                xmlObject = xmlObject.Replace("$payerPwd", payerPwd);
                xmlObject = xmlObject.Replace("$smartCardCode", smartCardCode);
                xmlObject = xmlObject.Replace("$transactionNo", TransactionNo);
                soapEnvelopeXml.LoadXml(xmlObject);
                WebLog.Log("xmlObject:" + xmlObject);
                using (Stream stream = request.GetRequestStream())
                {
                    WebLog.Log("stream:" + stream);
                    soapEnvelopeXml.Save(stream);
                    WebLog.Log("stream:" + stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    WebLog.Log("response:" + response);
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        WebLog.Log("soapResult:" + soapResult);
                        soapResult = rd.ReadToEnd();
                        returnMsg = soapResult;
                        WebLog.Log("soapResult:" + soapResult);
                        WebLog.Log("returnMsg:" + returnMsg);
                    }

                }
                return returnMsg;
            }

            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                Error = ex.Message;
                return Error;
            }
        }
        public string PaySubscription(dynamic payObj,dynamic Phones)
        {
            try
            {
                dynamic payObjs = new JObject();
                string payerPwd = new CryptographyManager().ComputeHash(Plaincode, HashName.MD5);
                TransactionNo = payerId + TransactionNumb() + SerialNum();
                payObjs.CustomerID = payObj.CustomerID;
                payObjs.ServiceAmt = payObj.Amount;
                payObjs.Service = payObj.ServiceDetails;
                payObjs.Bouquet = payObj.ServiceDetails;
                payObjs.customerTel = Phones.Phone;
                string methodName = ConfigurationManager.AppSettings["customerPay2"];
                string Amount = payObjs.ServiceAmt;
                string CustomerID = payObjs.CustomerID;
                string TransacNum = payObj.ReferenceNumber;
                DateTime TrnDate = (DateTime)payObj.TrnDate;
                //var Dates = Convert.ToString(payObj.TrnDate);
                // var Dates = "2018-11-11 19:32:59";
                var Dates = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string Phone = Phones.Phone;
                HttpWebRequest request = CreateWebRequest(methodName);
                XmlDocument soapEnvelopeXml = new XmlDocument();
                var xmlObject = File.ReadAllText(HttpContext.Current.Server.MapPath("/Xmlfiles/CustomerPay.xml"));
                xmlObject = xmlObject.Replace("$payerID", payerId);
                xmlObject = xmlObject.Replace("$payerPwd", payerPwd);
                xmlObject = xmlObject.Replace("$Phone", Phone);
                xmlObject = xmlObject.Replace("$smartCardCode", CustomerID );
                xmlObject = xmlObject.Replace("$fee", Amount);
                xmlObject = xmlObject.Replace("$transactionNo", TransacNum);
                xmlObject = xmlObject.Replace("$transferTime", Dates);
                soapEnvelopeXml.LoadXml(xmlObject);

                using (Stream stream = request.GetRequestStream())
                {
                    soapEnvelopeXml.Save(stream);
                }

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader rd = new StreamReader(response.GetResponseStream()))
                    {
                        soapResult = rd.ReadToEnd();
                        returnMsg = soapResult;
                    }

                }
                return returnMsg;
            }

            catch (Exception ex)
            {
                Error = ex.Message;
                return Error;
            }
        }

        public void Create()
        {
            //  X509Certificate2 cert = new X509Certificate2(@"C:/www.powernow.com.ng.pfx", "123456");
            X509Certificate2 cert = new X509Certificate2(@"C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/starCertificate.cer");
            cert.FriendlyName = "anAlias";

            X509Certificate2Collection cer = new X509Certificate2Collection();
            cer.Add(cert);
            byte[] bytes = cer.Export(X509ContentType.Pkcs12);

            System.IO.File.WriteAllBytes(@"C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/ClientTrustStore.jks", bytes);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("https://62.173.36.18:8443/stariboss-haiwai_proxy/electronicPaymentService?querySubscriberInfo");
            //req.ClientCertificates.Add(cert);
            req.Headers.Add(@"SOAP:Action");
            req.ContentType = "text/xml";
            req.Method = "POST";
            //req.ClientCertificates.Add(cert);
            
            using (Stream stm = req.GetRequestStream())
            {
                using (StreamWriter stmw = new StreamWriter(stm))
                {
                  //  stmw.Write(soap);
                    stmw.Close();
                }
            }
            try
            {
                WebResponse response = req.GetResponse();
                Stream responseStream = response.GetResponseStream();

                response = req.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream());
                string result = sr.ReadToEnd();
                sr.Close();

            }
            catch (Exception ex)
            {
                if (ex is WebException)
                {
                    WebException we = ex as WebException;
                    WebResponse webResponse = we.Response;
                    throw new Exception(ex.Message);
                }
            }
        }



        public HttpWebRequest CreateWebRequest(string MethodName)
        {

            string baseurl = ConfigurationManager.AppSettings["StartimesUrl"] + MethodName;
         //string baseurl = @"Http://62.173.36.18:8080/stariboss-haiwai_proxy/electronicPaymentService?" + MethodName;
            NEVER_EAT_POISON_Disable_CertificateValidation();
            HttpWebRequest webRequest = (HttpWebRequest)
            WebRequest.Create(baseurl);
            //webRequest.ClientCertificates.Add(cert);
            // SSLValidator.OverrideValidation();
            webRequest.Headers.Add(@"SOAP:Action");
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";

            return webRequest;
        }
        //public static HttpWebRequest CreateWebRequest(string MethodName)
        //{
//    string baseurl = "https://62.173.36.18:8443/stariboss-haiwai_proxy/electronicPaymentService?";
        //    baseurl = baseurl + MethodName;
        //    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        //    // X509Certificate2 Cert = new X509Certificate2("client.p12", "1234", X509KeyStorageFlags.MachineKeySet);
        //    X509Certificate2 signerCert2 = new X509Certificate2(@"C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/Startimes.pfx");
        //    // Create a new WebClient instance.
        //    //CertificateWebClient myWebClient = new CertificateWebClient(Cert);
        //    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(AcceptAllCertifications);
        //    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
        //    webRequest.Headers.Add(@"SOAP:Action");
        //    webRequest.ContentType = "text/xml;charset=\"utf-8\"";
        //    webRequest.Accept = "text/xml";
        //    webRequest.Method = "POST";

        //    webRequest.ClientCertificates.Add(new X509Certificate(signerCert2));

        // /*   webRequest.ClientCertificates = new X509Certificate2Collection(
        //    new X509Certificate2(
        //    "C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/ClientTrustStore.jks",
        //    "4da2africa"));
        //    webRequest.ClientCertificates = new X509Certificate2Collection(
        //    new X509Certificate2(
        //    "C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/ClientKeyStore.jks",
        //    "b2a9n9k8"));*/

        //    // webRequest.ClientCertificates.
        //    // webRequest.ClientCertificates = System.Security.Cryptography.X509Certificates.X509Certificate.CreateFromCertFile("");
        //    return webRequest;
        //}

        //public static bool AcceptAllCertifications(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors) { return true; }

        static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }

    }
}

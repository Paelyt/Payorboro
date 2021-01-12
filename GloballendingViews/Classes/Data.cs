using GloballendingViews.HelperClasses;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;

namespace GloballendingViews.Classes
{
    public class Data
    {
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
               // obj.beneficiary = cusObj.beneficiary;
                obj.MerchantFK = cusObj.MerchantFk;
                //obj.serviceType = 1;


                //var builder = new StringBuilder();
                //builder.Append(agentid).Append(agentKey).Append(obj.customerId);

                //var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA256);

                // For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                //var dt = DateTime.Now;
                 signaturetext.Append(agentid).Append(agentKey).Append(email);
               
                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                //obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("json::" + json);
                string sessionID = Paytv.GetSessionID();
                WebLog.Log("SessionID" + sessionID);
                var PostUrl = ConfigurationManager.AppSettings["GetSmileTVPriceList"];

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

        public string ValidateCableTVRequest(Paytv.CustomerObj cusObj)
            {
            try
            {
                dynamic obj = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var email = ConfigurationManager.AppSettings["agentEmail"];
                var MerchantFk = cusObj.MerchantFk;
               
                obj.customerId = cusObj.CustomerID;
                obj.MerchantFK = MerchantFk;

                var hashvalue = new StringBuilder();
                
                hashvalue.Append(agentid).Append(agentKey).Append(email);
                
                string signature = new CryptographyManager().ComputeHash(hashvalue.ToString(), HashName.SHA512);


                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(obj.customerId);

                var signatures = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);

                obj.hashValue = signatures;
               
                var json = obj.ToString();
                WebLog.Log("json::" + json);
                string sessionID = Paytv.GetSessionID();
                var PostUrl = ConfigurationManager.AppSettings["ValidateSmileCustomerID"];
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public string payData(dynamic cusObj)
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
                obj.customerName = cusObj.CustomerName;
                obj.bundleType = cusObj.ServiceDetails;
                obj.amount = cusObj.Amount;
                obj.CustomerPhone = cusObj.CustomerPhone;
                obj.refNumber = cusObj.ReferenceNumber;
                obj.MerchantFK = cusObj.Merchant_FK;

                // for Payload
                var hashValue = (agentid + agentKey + obj.customerId + obj.refNumber);

                hashValue = new CryptographyManager().ComputeHash(hashValue.ToString(), HashName.SHA512);

                //for header 
                //var builder = new StringBuilder();
                //builder.Append(agentid).Append(agentKey).Append(obj.customerId).Append(obj.Amount);
                
                //var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                
                //For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                obj.hashValue = hashValue;
                var json = obj.ToString();
                WebLog.Log("BuyData" + json);
                string sessionID = Paytv.GetSessionID();
                WebLog.Log("SessionID" + sessionID);
                var PostUrl = ConfigurationManager.AppSettings["BuyData"];
                WebLog.Log("BuyData" + json);
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);


                WebLog.Log("BuyData" + json);
                WebLog.Log("PostUrl" + PostUrl + agentid + agentKey);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

    }
}
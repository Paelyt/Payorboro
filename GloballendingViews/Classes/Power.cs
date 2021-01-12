using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GloballendingViews.HelperClasses;
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Text;

namespace GloballendingViews.Classes
{
    public class Power
    {
        
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

            public string Borovalue { get; set; }

        }

        public class PowerObj
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
            public string Address { get; set; }
            public string Token { get; set; }
            public string ServiceValueDetails1 { get; set; }

            public string ServiceValueDetails2 { get; set; }
            public string ServiceValueDetails3 { get; set; }

            public string Voucher { get; set; }

            public string NewAmount { get; set; }
            public string TotalAmt { get; set; }

            public string ErrorMsg { get; set; }

            public int TransactionStatus_FK { get; set; }
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
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string GetCustomerDetails(CustomerObj cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var MerchantFk = cusObj.MerchantFk;
                var MeterType = cusObj.MeterType;
                var email = ConfigurationManager.AppSettings["agentEmail"];
                obj.agentkey = agentKey ;
               // obj.agentID = agentID;
                obj.customerId = cusObj.CustomerID;
                obj.MerchantFK = MerchantFk;
                obj.accountType = MeterType;
                

               /* var plainText = (agentID + agentKey + obj.customerId);*/
                var builder = new StringBuilder();
                builder.Append(agentid).Append(agentKey).Append(obj.customerId);
               
                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);
                
                // For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);
               
               
                obj.hashValue = hash;
                var json = obj.ToString();
                WebLog.Log("json::" + json);
                string sessionID = GetSessionID();
                WebLog.Log("SessionID" + sessionID);
                var PostUrl = ConfigurationManager.AppSettings["ValidateCustomerIDnew"];
               // var data = Utility.DoPosts1(json, $"{PostUrl}",agentID,agentKey,signature,sessionID);
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);
                WebLog.Log("PostUrl" + PostUrl + agentid + agentKey);
                return data;

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        
         public static string PostBuyPowerJson(string refNum,int Merc_fk)
        {
        try
        {
               
      DataAccess.DataReaders _dr = new DataAccess.DataReaders();
               // CustomerObj cusObj;
       var CustomerInfo = _dr.GetTransactionLog(refNum);
       string agentID = ConfigurationManager.AppSettings["agentID"];
       string agentKey = ConfigurationManager.AppSettings["agentKey"];

                dynamic obj = new JObject();
                string valueTime = DateTime.Now.ToString("Hmmss");
                DateTime valTime = DateTime.Now;
                obj.agentkey = agentKey;
                obj.customerId = CustomerInfo.CustomerID;
                obj.accountType = CustomerInfo.TransactionType;
                obj.customerName = CustomerInfo.CustomerName.Trim();
                obj.transDate = CustomerInfo.TrnDate;
                obj.amount = CustomerInfo.Amount;
                obj.refNumber = CustomerInfo.ReferenceNumber;
                obj.MerchantFK = Merc_fk;
                //   obj.Phone = CustomerInfo.CustomerPhone;
                obj.phoneNumber = CustomerInfo.CustomerPhone;
                // obj.Email = CustomerInfo.CustomerEmail;
                obj.emailAddress = CustomerInfo.CustomerEmail;
                
                 var email =ConfigurationManager.AppSettings["agentEmail"];
               
                var builder = new StringBuilder();
     builder.Append(obj.customerId).Append(obj.amount).Append(agentID).Append(agentKey);
     var hash = new CryptographyManager().ComputeHash(builder.ToString(),HashName.SHA512);

              
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentID).Append(agentKey).Append(email);

               var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);
                obj.hashValue = hash;
               var json = obj.ToString();
               string sessionID = GetSessionID();
               var PostUrl = ConfigurationManager.AppSettings["Buypower"];
               var data = Utility.DoPosts1(json, $"{PostUrl}", agentID, agentKey, signature, sessionID);
                return data;
     }
      catch (Exception ex)
     {
         WebLog.Log(ex.Message.ToString());
         return null;
     }
     }

        public static string CheckMeterEligibilty(CustomerObj cusObj)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                var MerchantFk = cusObj.MerchantFk;
                var MeterType = cusObj.MeterType;
                var email = ConfigurationManager.AppSettings["agentEmail"];
                //obj.agentkey = agentKey;
                obj.customerId = cusObj.CustomerID;
                obj.accountType = MeterType;
                obj.MerchantFK = MerchantFk;
               


                var builder = new StringBuilder();
                builder.Append(agentid).Append(agentKey).Append(obj.customerId);

                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature
                string customerId = obj.customerId;
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                obj.hashValue = hash;
                var json = obj.ToString();
                string sessionID = GetSessionID();
                var PostUrl = ConfigurationManager.AppSettings["CheckMeterEligibilty"];
               
                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);
                return data;

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static string PostBoroPowerJson(string refNum,string Borovalue)
        {
            try
            {

                DataAccess.DataReaders _dr = new DataAccess.DataReaders();
                
                var CustomerInfo = _dr.GetTransactionLog(refNum);
                string AgentID = ConfigurationManager.AppSettings["agentID"];
                string AgentKey = ConfigurationManager.AppSettings["agentKey"];

                dynamic obj = new JObject();
              
                obj.customerId = CustomerInfo.CustomerID;
                obj.accountType = CustomerInfo.TransactionType;
                obj.customerName = CustomerInfo.CustomerName;
                obj.phonenumber = CustomerInfo.CustomerPhone;
                obj.emailaddress = CustomerInfo.CustomerEmail;
                obj.amount = CustomerInfo.Amount;
                //obj.amount = Borovalue;
                obj.refNumber = CustomerInfo.ReferenceNumber;
                obj.MerchantFK = CustomerInfo.Merchant_FK;
               
               var email = ConfigurationManager.AppSettings["agentEmail"];

                var builder = new StringBuilder();
                builder.Append(AgentID).Append(AgentKey).Append(obj.customerId).Append(obj.amount);
               
             var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);


                 var signaturetext = new StringBuilder();
                 signaturetext.Append(AgentID).Append(AgentKey).Append(email);

                 var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);
                //var signaturetext = new StringBuilder();
                //signaturetext.Append(agentid).Append(agentkey).Append(EmailAddress);

                //var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA256);
                obj.hashValue = hash;
                var json = obj.ToString();
                string sessionID = GetSessionID();
                var PostUrl = ConfigurationManager.AppSettings["BorrowPower"];
                var data = Utility.DoPosts1(json, $"{PostUrl}", AgentID,AgentKey, signature, sessionID);
                return data;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public static bool verifyPay(string amount)
        {
            try
            {
                if (amount.Length > 0)
                {
                    double multipleAmt = Convert.ToDouble(amount);
                    if (multipleAmt % 500 != 0)
                    {
                         
                        return false;
                    }
                }
                return true;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }

        public static Double ClaculateFifteenpercent(double Amount)
        {
            try
            {
                double Percent = Convert.ToDouble(ConfigurationManager.AppSettings["BoroPowerConvFee"]);
                var Result = Percent / 100 * Amount;
                Result = Amount - Result;
                return Result;
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        // This is the new One for today
        public static string GetTransactionDetails(Classes.Power.Receipt recpt)
        {
            try
            {
                dynamic obj = new JObject();
                dynamic headervalues = new JObject();
                string agentid = ConfigurationManager.AppSettings["agentID"];
                string agentKey = ConfigurationManager.AppSettings["agentKey"];
                //var refNumber = cusObj.MerchantFk;
                //var amount = cusObj.MeterType;
                var email = ConfigurationManager.AppSettings["agentEmail"];
                obj.agentkey = agentKey;
                obj.refNumber = recpt.ReferenceNumber;
                obj.amount = recpt.Amount;
                obj.MerchantFK = recpt.Merchant_FK;
                
                var builder = new StringBuilder();
                builder.Append(agentid).Append(agentKey).Append(obj.refNumber).Append(obj.amount);

                var hash = new CryptographyManager().ComputeHash(builder.ToString(), HashName.SHA512);

                // For The Signature
              
                var signaturetext = new StringBuilder();
                signaturetext.Append(agentid).Append(agentKey).Append(email);

                var signature = new CryptographyManager().ComputeHash(signaturetext.ToString(), HashName.SHA512);


                obj.hashValue = hash;
                var json = obj.ToString();
                string sessionID = GetSessionID();
                var PostUrl = ConfigurationManager.AppSettings["GetTransactionDetails"];

                var data = Utility.DoPosts1(json, $"{PostUrl}", agentid, agentKey, signature, sessionID);
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
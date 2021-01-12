using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;

namespace GloballendingViews.Classes
{
    public class Internetserviceprovider
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

        public class InternetServiceObj
        {
            public string userName { get; set; }
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

            public string Voucher { get; set; }

            public string Pin { get; set; }

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
        }


        public string SerialNum()
        {
            try
            {
                string RefNum = Utility.RandomString(5);
                return RefNum;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public static HttpWebRequest CreateWebRequest(string UserID, string MethodName, [Optional] string Rvc, [Optional] string Pin)
        {
            string baseurl = "";
            // For Other Base Url
            
           
                baseurl = System.Configuration.ConfigurationManager.AppSettings["SpectranetBaseUrl"];


                baseurl = baseurl.Replace("{$Method}", MethodName.ToString()).Trim().Replace("{$UserID}", UserID.ToString()).Trim();
            if (Rvc != null)
            {
                baseurl = System.Configuration.ConfigurationManager.AppSettings["SpectranetRvcplan"];

                baseurl = baseurl.Replace("{$Rvc}", Rvc.ToString()).Trim().Replace("{$Pin}", Pin.ToString()).Trim().Replace("{$UserID}", UserID.ToString()).Trim();
            }


            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.ContentType = "application/json";
            string Username = System.Configuration.ConfigurationManager.AppSettings["SpectranetUser"];
            string Password = System.Configuration.ConfigurationManager.AppSettings["SpectranetPassword"];
            //webRequest.Method = "GET";
            //webRequest.Credentials = new 
            //    NetworkCredential(Username,Password);
            webRequest.ContentType = "application/json";
            //webRequest.Headers.Add(@"SOAP:Action");
            //webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            //webRequest.Accept = "text/xml";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webRequest.Method = "GET";
            webRequest.Credentials = new NetworkCredential(Username, Password);

            return webRequest;
        }


        public static HttpWebRequest CreateWebReq(string UserID, string MethodName)
        {
            string baseurl = "";
           
            // For Spaecific Change Plan Url
           
                baseurl = System.Configuration.ConfigurationManager.AppSettings["SpectranetChangeplan"];
                baseurl = baseurl.Replace("{$desiredplan}", MethodName.ToString()).Trim().Replace("{$UserID}", UserID.ToString()).Trim();
           
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(baseurl);
            webRequest.ContentType = "application/json";
            string Username = System.Configuration.ConfigurationManager.AppSettings["SpectranetUser"];
            string Password = System.Configuration.ConfigurationManager.AppSettings["SpectranetPassword"];
           
            webRequest.ContentType = "application/json";
            
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            webRequest.Method = "GET";
            webRequest.Credentials = new NetworkCredential(Username, Password);

            return webRequest;
        }
    }
}
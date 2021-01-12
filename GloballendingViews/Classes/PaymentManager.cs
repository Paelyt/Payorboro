using GloballendingViews.HelperClasses;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;

namespace GloballendingViews.Classes
{
    public class PaymentManager
    {

        public class TransactionalInformation
        {
            public bool ReturnStatus { get; set; }
            public List<string> ReturnMessage { get; private set; }
            public int TotalRows { get; set; }
            public bool IsAuthenicated { get; set; }
            public string SortExpression { get; set; }
            public string SortDirection { get; set; }
            public int CurrentPageNumber { get; set; }

            public TransactionalInformation()
            {
                ReturnMessage = new List<string>();
                ReturnStatus = true;
                IsAuthenicated = false;
            }
        }
        public class Payment
        {
            public string customerid { get; set; }
            public string RefNumber { get; set; }
            public string customerName { get; set; }
            public string emailaddress { get; set; }
            public string phoneNo { get; set; }
            public string amount { get; set; }
            public string narration { get; set; }
            public string returnUrl { get; set; }

            public int PaymentType { get; set; }

            public int PaymentPlanID { get; set; }
        }
        public static string GetPaymentPageDatails(Payment payObj)
        {
            try
       {
                //  var merchant = _page.Merchant;
               
               var paymentUrl = GetPaymentEngineUrl(payObj.PaymentType);
               var publicKey = ConfigurationManager.AppSettings["PublicKey"];
               var product = "0001";
               var trxRef = payObj.RefNumber;
               var customerId = payObj.customerid;
               var customerName = payObj.customerName;
               var email = payObj.emailaddress;
               var phone = $"{payObj.phoneNo}";
               var amount = $"{decimal.Parse(payObj.amount)}";
               var currency = $"NGN";
               var narration = payObj.narration;
               var returnUrl = payObj.returnUrl;
               var Secretkey = ConfigurationManager.AppSettings["SecretKey"];
                // var macKey = "sk_CZWILQCEJXO4XNEEM6IG1ERB0QTFTZJQU2E1FXX6XFBSUAJRT6GN49TX";
                var planId = payObj.PaymentPlanID;
                var plainText = $"{publicKey}{product}{trxRef}{customerId}{customerName}{email}{phone}{amount}{currency}{narration}{returnUrl}{Secretkey}";
                //var plainText = (publicKey + trxRef + product +  customerName + email + customerId +
                //              amount + currency + returnUrl + phone + narration + macKey);
                var hash = new CryptographyManager().ComputeHash(plainText, HashName.SHA256);

                var formObject = File.ReadAllText(HttpContext.Current.Server.MapPath("~/UtilityFiles/PaymentForm.html"));
                formObject = formObject.Replace("$action", paymentUrl);
                formObject = formObject.Replace("$planId", $"{planId}");
                formObject = formObject.Replace("$publicKey", $"{publicKey}");
                formObject = formObject.Replace("$product", $"{product}");
                formObject = formObject.Replace("$trxRef", trxRef);
                formObject = formObject.Replace("$customerId", customerId);
                formObject = formObject.Replace("$customerName", customerName);
                formObject = formObject.Replace("$email", email);
                formObject = formObject.Replace("$phone", phone);
                formObject = formObject.Replace("$amount", amount);
                formObject = formObject.Replace("$currency", currency);
                formObject = formObject.Replace("$narration", narration);
                formObject = formObject.Replace("$returnUrl", returnUrl);
                formObject = formObject.Replace("$hash", hash);

                return formObject;


                //var request = (HttpWebRequest)WebRequest.Create(paymentUrl);
                //var data1 = Encoding.ASCII.GetBytes(formObject);
                //request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = data1.Length;

                //using (var stream = request.GetRequestStream())
                //{
                //    stream.Write(data1, 0, data1.Length);
                //}
                //var response = (HttpWebResponse)request.GetResponse();
                //var responseString =
                //         new StreamReader(response.GetResponseStream()).ReadToEnd();

            }
            catch (Exception ex)
            {

                WebLog.Log(ex);
                return "Error";
            }
        }

        public static string GetPaymentEngineUrl(int paytype)
        {
            var url = "";
            if (paytype == 1)
            {
                 url = ConfigurationManager.AppSettings["Payment_Engine_Url"];
            }
            if(paytype == 2)
            {
                 url = ConfigurationManager.AppSettings["Recurring_Payment_Engine_Url"];
            }
            return url;
         }

        
        
    }
    public class PaymentNotficationReq
    {
    }

    public class TransactionalInformation
    {
        public bool ReturnStatus { get; set; }
        public List<string> ReturnMessage { get; private set; }
        public int TotalRows { get; set; }
        public bool IsAuthenicated { get; set; }
        public string SortExpression { get; set; }
        public string SortDirection { get; set; }
        public int CurrentPageNumber { get; set; }

        public TransactionalInformation()
        {
            ReturnMessage = new List<string>();
            ReturnStatus = true;
            IsAuthenicated = false;
        }

        
}
}
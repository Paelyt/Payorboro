using GloballendingViews.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Web;
using System.Web.Mvc;

namespace GloballendingViews.Controllers
{
    public class StartimesController : Controller
    {
        // GET: Startimes
        public ActionResult Index(string MethodName)
{
            Classes.Paytv poo = new Paytv();
            poo.Create();
           

           
            //service.ClientCertificates.Add(cert);
            // cert.Import("Client.pfx", "", DefaultKeySet);
            ///service.ClientCredentials.ClientCertificate.Certificate = cert; ServicePointManager.ServerCertificateValidationCallback = null;
            ///


            using (new OperationContextScope(service.InnerChannel))
            {
                string strUsrName = ConfigurationManager.AppSettings["UserName"];
                string strPassword = ConfigurationManager.AppSettings["Password"];
                // We will use a custom class called UserInfo to be passed in as a MessageHeader
                Classes.servicelm.AuthSoapHd CALLCENTER = new servicelm.AuthSoapHd();
                CALLCENTER.CALLCENTER_PASSWORD = "StarCallCenter";
                CALLCENTER.CALLCENTER_USERNAME = "StarCallCenter";
               
               
                // Add a SOAP Header to an outgoing request
                MessageHeader aMessageHeader = MessageHeader.CreateHeader("CALLCENTER", "http://tempuri.org", CALLCENTER);
                OperationContext.Current.OutgoingMessageHeaders.Add(aMessageHeader);

                // Add a HTTP Header to an outgoing request
                HttpRequestMessageProperty requestMessage = new HttpRequestMessageProperty();
                requestMessage.Headers.Add("CALLCENTER_PASSWORD", strPassword);
                //["CALLCENTER_PASSWORD"] = strPassword;
                //requestMessage.Headers["CALLCENTER_USERNAME"] = strUsrName;
                requestMessage.Headers.Add("CALLCENTER_USERNAME", strUsrName);

                OperationContext.Current.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = requestMessage;
                
                var cnd = new Starboss.SubscriberQueryCondition
            {
                payerID = "4200",
                payerPwd = "abb409d3cd1bcf6585b34f2f461d0c8a",
                smartCardCode = "02146533082",
                transactionNo = "nnbnnbguyvkuygnehsh",
            };
            // SSLValidator.OverrideValidation();
            var xx = service.querySubscriberInfo(cnd);
            var a = xx;
               
            }
           
            
            return View();
        }

        public ActionResult Test()
        {

            return View();
        
    }


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

        X509Certificate2 GetCert()
        {
            var par = "C:/Users/Reliance Limited/Documents/Visual Studio 2015/Projects/GloballendingProject/GlobalLending/GloballendingViews/Cert/";
            var stx = System.IO.File.Open(Path.Combine(par, "starCertificate.cer"), FileMode.Open);
            using (BinaryReader br = new BinaryReader(stx))
            {
                return new X509Certificate2(br.ReadBytes((int)br.BaseStream.Length), "password");
            }
        }

        /*   public string GetServiceResponse()
           {

               string WebSvcEndpointConfigurationName = "querySubscriberInfo";
               Uri webSvcEndpointAddress = new Uri("https://62.173.36.18:8443/stariboss-haiwai_proxy/electronicPaymentService?");
               string webSvcCertificateThumbPrint = "748681ca3646ccc7c4facb7360a0e3baa0894cb5";

               string webSvcResponse = null;
               SomeWebServiceClient webServiceClient = null;

               try
               {
                   webServiceClient = new SomeWebServiceClient(WebSvcEndpointConfigurationName, new EndpointAddress(webSvcEndpointAddress));
                   webServiceClient.ClientCredentials.ClientCertificate.Certificate = GetCertificateByThumbprint(webSvcCertificateThumbPrint, StoreLocation.LocalMachine);

                   webSvcResponse = webServiceClient.GetServiceResponse();
               }
               catch (Exception ex)
               {
               }
               finally
               {
                   if (webServiceClient != null)
                   {
                       webServiceClient.Close();
                   }
               }
               return webSvcResponse;
           }
           private static X509Certificate2 GetCertificateByThumbprint(string certificateThumbPrint, StoreLocation certificateStoreLocation)
       {
           X509Certificate2 certificate = null;

           X509Store certificateStore = new X509Store(certificateStoreLocation);
           certificateStore.Open(OpenFlags.ReadOnly);


           X509Certificate2Collection certCollection = certificateStore.Certificates;
           foreach (X509Certificate2 cert in certCollection)
           {
               if (cert.Thumbprint != null && cert.Thumbprint.Equals(certificateThumbPrint, StringComparison.OrdinalIgnoreCase))
               {
                   certificate = cert;
                   break;
               }
           }

           if (certificate == null)
           {
               Log.ErrorFormat(CultureInfo.InvariantCulture, "Certificate with thumbprint {0} not found", certificateThumbPrint);
           }

           return certificate;
       }
   */
    }


}

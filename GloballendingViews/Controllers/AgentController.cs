using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GloballendingViews.Models;
using GloballendingViews.ViewModels;
using DataAccess;
using GloballendingViews.HelperClasses;
using System.Configuration;
using GloballendingViews.Classes;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Collections;

namespace GloballendingViews.Controllers
{
    public class AgentController : Controller
    {
        private static HttpWebRequest _req;
        private static HttpWebResponse _response;
        DataAccess.DataReaders DataReaders = new DataAccess.DataReaders();
        DataAccess.DataCreator DataCreators = new DataAccess.DataCreator();
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        GLobalClient gc = new GLobalClient();
        Customer customer = new Customer();
        CustomerServices _cs = new CustomerServices();
        string Email = "tolutl@yahoo.com";
        LogginHelper LoggedInuser = new LogginHelper();
        DataAccess.Agent agent = new DataAccess.Agent();
        DataAccess.User user = new DataAccess.User();
        // GET: Agent
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult AgentWallet()
        {
          try
            {
             string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
              int userid = DataReaders.GetUserIdByEmail(user);
                    var Balance = DataReaders.Balance(userid);
                    TempData["Balance"] = Balance;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Index");
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult CreditWallet()
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
        public ActionResult CreateAgent(LoanViewModel lvm)
        {
            try
            {

                agent.ContactEmail = lvm.AgentModel.ContactEmail;
                var valid = DataReaders.ValidateAgent(agent.ContactEmail);
                if (valid == false)
                {
                    TempData["Message"] = "Agent Already Exist!";
                    return View("Index");
                }
                else 
                if (valid == true) 
                {
                    user.pasword = lvm.AccountsModel.pasword;
                    user.confirmPassword = lvm.AccountsModel.confirmPassword;
                    bool validatePass = DataReaders.ValidatePassword(user.pasword, user.confirmPassword);
                    if (validatePass == true)
                    {
                        agent.ContactAddress = lvm.AgentModel.ContactAddress;
                        user.pasword = lvm.AccountsModel.pasword;
                        var EncrypPassword = new CryptographyManager().ComputeHash(user.pasword, HashName.SHA256);
                        user.pasword = EncrypPassword;
                       
                        user.firstname = lvm.AgentModel.FirstName; user.lastname = lvm.AgentModel.LastName; user.Phone = lvm.AgentModel.PhoneNumber; user.Email = lvm.AgentModel.ContactEmail; agent.NextOfKinAddress = lvm.AgentModel.NextOfKinAddress; agent.NextOfKinName = lvm.AgentModel.NextOfKinName; agent.NextOfKinPhoneNumber = lvm.AgentModel.NextOfKinPhoneNumber; agent.LastName = lvm.AgentModel.LastName; agent.FirstName = lvm.AgentModel.FirstName; agent.PhoneNumber = lvm.AgentModel.PhoneNumber;
                        agent.ValueDate = DateTime.Now.ToString("yyyy:mm:dd");
                        agent.ValueDate = DateTime.Now.ToString("HH:mm:ss");
                        lvm.AccountsModel.isVissibles = 1; user.isVissibles = 1;
                        lvm.AgentModel.DateCreated = DateTime.Now; agent.DateCreated = DateTime.Now;
                        db.Users.Add(user);
                        db.SaveChanges();
                        db.Agents.Add(agent);
                        db.SaveChanges();
                        TempData["Message"] = "Agent Succesfully Created";
                        return View("Index");
                    }
                    else
                    {
                        TempData["Message"] = "Password and confirm password must match";
                        return View("Index");
                    }
                }

                return View("Index");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreditWallet(LoanViewModel lvm,FormCollection form)
        {

            try
            {
                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                    int userid = DataReaders.GetUserIdByEmail(user);
        string Firstname = Convert.ToString(form["Firstname"]);
        string Email = Convert.ToString(form["Email"]);
        string PhoneNumber = Convert.ToString(form["PhoneNumber"]);
        string Amount = Convert.ToString(form["Amount"]);
        var TotalAmt = Convert.ToDecimal(Amount);
        bool isNum = Decimal.TryParse(Amount, out TotalAmt);

                    if (isNum)
                    {
                        PaymentManager.Payment PayObj = new PaymentManager.Payment();
                        PayObj.RefNumber = System.DateTime.Now.ToString("yyyyMMddHmmss");
                        PayObj.amount = TotalAmt.ToString();
                        PayObj.customerid = "2";
                        PayObj.customerName = Firstname;
                        PayObj.emailaddress = Email;
                        PayObj.narration = $"{Firstname.Trim()} Payment of NGN {decimal.Parse(PayObj.amount)}";
                        PayObj.phoneNo = PhoneNumber;
                        PayObj.returnUrl = ConfigurationManager.AppSettings["PaymentReturnUrl"];
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
                return View();  
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
                ProcessResponse();
                // return View("AgentWallet");
                return RedirectToAction("AgentWallet", "Agent");
            }
            catch(Exception ex)
            {
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
                 string url = "http://paytrx.net/api/web/GetTransactionRecord";

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
   if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() == "00")
                {
                    AgentAccount AgtAccount = new AgentAccount();
                    string user = LoggedInuser.LoggedInUser();
                    if (user != null)
                    {

                        int userid = DataReaders.GetUserIdByEmail(user);
                        AgtAccount.AgentUser_FK = userid;
                        AgtAccount.Credit = jvalues?.data?.amount;
                        AgtAccount.Debit = 0;
                        AgtAccount.ReferenceNumber = jvalues?.data?.trxref;
                        AgtAccount.TranxDate = jvalues?.data?.created_at;
                        AgtAccount.ValueDate = DateTime.Now.ToString("yyyy:mm:dd");
                        AgtAccount.ValueTime = DateTime.Now.ToString("HH:mm:ss");
                        db.AgentAccounts.Add(AgtAccount);
                        db.SaveChanges();
                        TempData["paymentmsg"] = "Payment Not Succesfull";
                    }
                }
                else
                    if (jvalue != null && $"{jvalues?.data?.resp_code}".ToLower() != "00")
                {
                  //var hash = ComputeHash(plainText);
                    TempData["Message"] = "Payment Not Succesfull";
                }
                }
              catch (Exception ex)
              {
                  //lblStatusMsg.Text = ex.Message.ToString();
              }
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

    }
}
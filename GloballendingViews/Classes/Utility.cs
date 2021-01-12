using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;
using System.Web;
using System.Security.Cryptography;
using System.Configuration;
using GloballendingViews.HelperClasses;
using System.Text.RegularExpressions;
using DataAccess;

namespace GloballendingViews.Classes
{
    public class Utility
    {
       // DataProvider dataprovider = new DataProvider();
     
    /*    public string SendText(string phone,string firstname,string lastname)
        {
            var tAlert = new TransactionAlert
            {
                PhoneNumber = phone,
                Sender = "Paytrx",
                Message = $"Dear {firstname?.ToUpper()} {lastname?.ToUpper()}, {ConfigurationManager.AppSettings["SmsWelcomeMessage"]}"
            };

            var smsResponse = NotificationService.SendSms(tAlert);

            return smsResponse;
        }
        */
     /*   public Boolean SendEmail(string firstname,string lastname,string email)
        {
            var bodyTxt = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
            bodyTxt = bodyTxt.Replace("$MerchantName", $"{firstname} {lastname}");
            var msgHeader = $"Welcome to Paybills";

            var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, email, null, null);
            return true;
        }*/
        public static DateTime GetCurrentDate()
        {
            try
            {
                return DateTime.Parse(DateTime.Now.ToString("D"));
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return DateTime.Today;
            }
        }
        public static DateTime GetCurrentTime()
        {
            try
            {
                return DateTime.Parse(DateTime.Now.ToString("T"));
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return DateTime.Today;
            }
        }

        public static string DoGets(string json, string url, [Optional]string hash)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(hash))
                    {
                        //client.Headers[HttpRequestHeader.Authorization] = hash;
                        client.Headers.Set("hash", hash);
                    }
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }
        // i commented this out for the system date
        //public static DateTime GetCurrentDateTime()
        //{
        //    try
        //    {
        //        return DateTime.Parse(DateTime.Now.ToString("U"));
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //        return DateTime.Today;
        //    }
        //}
        // server Time and Date
        public static DateTime GetCurrentDateTime()
        {
            try
            {
                // gives you current Time in server timeZone            
                var serverTime = DateTime.Now;
                // convert it to Utc using timezone setting of server computer            
                var utcTime = serverTime.ToUniversalTime();
                var tzi = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
                // convert from utc to local            
                var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
                return localTime;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                return DateTime.Today;

            }
        }

        public static DateTime getCurrentLocalDateTime()
        {
            // gives you current Time in server timeZone
            var serverTime = DateTime.Now;
            // convert it to Utc using timezone setting of server computer
            var utcTime = serverTime.ToUniversalTime();
            var tzi = TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
            // convert from utc to local
            var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, tzi);
            return localTime;
        }

        public static string Json(object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }

        

        private static readonly Random Random = new Random();
        public static string RandomString(int length, Mode mode = Mode.AlphaNumeric)
        {
            var characters = new List<char>();

            if (mode == Mode.Numeric || mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaNumericLower)
                for (var c = '0'; c <= '9'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericUpper || mode == Mode.AlphaUpper)
                for (var c = 'A'; c <= 'Z'; c++)
                    characters.Add(c);

            if (mode == Mode.AlphaNumeric || mode == Mode.AlphaNumericLower || mode == Mode.AlphaLower)
                for (var c = 'a'; c <= 'z'; c++)
                    characters.Add(c);

            return new string(Enumerable.Repeat(characters, length)
              .Select(s => s[Random.Next(s.Count)]).ToArray());
        }
        public static string GenerateAlphanumericUniqueId(int length)
        {
            var g = Guid.NewGuid();
            var random = g.ToString();
            return random.Substring(0, length);
        }
        public static string GenerateNumericUniqueId(int length)
        {
            var random = new Random((int)DateTime.Now.Ticks);
            var builder = new StringBuilder();
            for (var i = 0; i < length / 2; i++)
            {
                var s = Convert.ToString(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(s);
            }
            return builder.ToString();
        }
        public static IDictionary<int, string> GetAll<TEnum>() where TEnum : struct
        {
            var enumerationType = typeof(TEnum);

            if (!enumerationType.IsEnum)
                throw new ArgumentException("Enumeration type is expected.");

            var dictionary = new Dictionary<int, string>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                dictionary.Add(value, name);
            }
            return dictionary;
        }
        

        public static string DoGet(string json, string url, [Optional]string token)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    resp = client.UploadString(url, "GET", json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoGetBank(string url, [Optional]string token)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                    //resp = client.UploadString(url, "GET", json);
                  ServicePointManager.Expect100Continue = true;
                  ServicePointManager.SecurityProtocol =         SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }
        public static string DoPost(string json, string url, [Optional]string token)
        {
           
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = token;
                    }
                  //  ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoPosts(string json, string url, [Optional]string hash,[Optional]string ClientID)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(hash) && !string.IsNullOrWhiteSpace(ClientID))
                    {
                        //client.Headers[HttpRequestHeader.Authorization] = hash;
                        //client.Headers[HttpRequestHeader.Authorization] = ClientID;
                        //client.Headers[HttpRequestHeader.Authorization] = hash;
                        //client.Headers[HttpRequestHeader.Authorization] = ClientID;


                        //client.Headers.Add("Accept", "application/json");
                        //client.Headers.Add("Content-Type", "application/json");
                        client.Headers.Add("client_id", ClientID);
                        client.Headers.Add("signature", hash);
                    }
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        public static string DoPaymentRequeryPosts(string json, string url, [Optional]string hash, [Optional]string ClientID)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(hash) )
                       {
                        client.Headers.Add("PaelytAuth", hash);
                        client.Headers.Add("PublicKey", ConfigurationManager.AppSettings["PublicKey"]);
                      }
                    WebLog.Log("urlnew0" + url);
                    WebLog.Log("postdatenew0" + json);
                    WebLog.Log("hashvaluenew0" + hash);
                    WebLog.Log("ClientIDnew0" + ClientID);
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        public static string DoPosts1(string json, string url ,string agentID, string agentKey, string signature,string session)
        {
            string resp;
            WebLog.Log("o wole"+ session );
            WebLog.Log("Json" + json);
            try
            {
               using (var client = new WebClient())
            {
              client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(signature))
                    {
                     client.Headers.Add("agentID", agentID);
                     client.Headers.Add("agentKey", agentKey);
                     client.Headers.Add("signature", signature);
                     client.Headers.Add("sessionid", session);
                    }
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                //WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoGet1( string url, string agentID, string agentKey, string signature)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(signature))
                    {
                        client.Headers.Add("agentID", agentID);
                        client.Headers.Add("agentKey", agentKey);
                        client.Headers.Add("signature", signature);
                        
                    }
                   
                   // resp = client.UploadString(url, "GET");

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }



        public string createTokenSesion() {
            string token = $"sk_{RandomString(56).ToUpper()}";
            HttpContext.Current.Session["token"] = token;

            return token;

        }


        public static string TransactionGenerator()
        {
            int maxSize = 12;
            char[] chars = new char[62];
            string a;
            a = "123456789123";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length - 1)]);
            }
            return result.ToString();
        }
        public static string hashSHA512(string unhashedValue)
        {
            SHA512 shaM = new SHA512Managed();
            byte[] hash =
             shaM.ComputeHash(Encoding.ASCII.GetBytes(unhashedValue));

            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in hash)
            {
                stringBuilder.AppendFormat("{0:x2}", b);
            }
            return stringBuilder.ToString();
        }


        public static string DoPost(string url, [Optional]string Authorization, [Optional]string timestamp, [Optional]string nonce, [Optional]string signatureMethod, [Optional]string signature, [Optional]string terminalid, string json)
        {
            string resp;
            try
            {
                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(timestamp))
                    {
                        client.Headers[HttpRequestHeader.Authorization] = Authorization;
                        client.Headers.Set("Signature", signature);
                        client.Headers.Set("Timestamp", timestamp);
                        client.Headers.Set("Nonce", nonce);
                        client.Headers.Set("SignatureMethod", signatureMethod);
                        client.Headers.Set("TerminalID", terminalid);

                    }
                    resp = client.UploadString(url, "POST", json);
                }
            }
            catch (WebException wex)
            {
                //WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                // WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoGET(string url, [Optional]string Authorization, [Optional]string timestamp, [Optional]string nonce, [Optional]string signatureMethod, [Optional]string signature, [Optional]string terminalid, [Optional]string hash)
        {
            string resp;
            try
            {

                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(timestamp))
                    {

                        client.Headers[HttpRequestHeader.Authorization] = Authorization;
                        client.Headers.Set("Signature", signature);
                        client.Headers.Set("Timestamp", timestamp);
                        client.Headers.Set("Nonce", nonce);
                        client.Headers.Set("SignatureMethod", signatureMethod);
                        client.Headers.Set("TerminalID", terminalid);


                    }
                    else
                    {
                        client.Headers.Add("Hash", hash);
                    }
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                //  WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public static string DoGETs(string json, string url, [Optional]string Authorization, [Optional]string timestamp, [Optional]string nonce, [Optional]string signatureMethod, [Optional]string signature, [Optional]string terminalid)
        {
            string resp;
            try
            {

                using (var client = new WebClient())
                {
                    client.Headers[HttpRequestHeader.ContentType] = "application/json";
                    if (!string.IsNullOrWhiteSpace(timestamp))
                    {

                        client.Headers[HttpRequestHeader.Authorization] = Authorization;
                        client.Headers.Set("Signature", signature);
                        client.Headers.Set("Timestamp", timestamp);
                        client.Headers.Set("Nonce", nonce);
                        client.Headers.Set("SignatureMethod", signatureMethod);
                        client.Headers.Set("TerminalID", terminalid);


                    }
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                    resp = client.DownloadString(url);

                }
            }
            catch (WebException wex)
            {
                //  WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }

        public static string DoGET(string url)
        {
            string resp;
            try
            {

                using (var client = new WebClient())
                {
                    
                    client.Headers.Set("Auth", System.Configuration.ConfigurationManager.AppSettings["SpectranetAuth"]);
                    client.Headers.Set("User", System.Configuration.ConfigurationManager.AppSettings["SpectranetUser"]);
                    client.Headers.Set("Password", System.Configuration.ConfigurationManager.AppSettings["SpectranetPassword"]);


                    resp = client.DownloadString(url);
                }
            }
            catch (WebException wex)
            {
                //  WebLog.Log(wex);
                using (var response = (HttpWebResponse)wex.Response)
                {
                    var statusCode = response != null ? (int)response.StatusCode : 500;
                    if (statusCode == 500 && response == null) return null;
                    var dataStream = response?.GetResponseStream();
                    if (dataStream == null) return null;
                    using (var tReader = new StreamReader(dataStream))
                    {
                        resp = tReader.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex);
                resp = ex.Message;
            }
            return resp;
        }


        public bool ValidateAmt(string Number)
        {
            try
            {
                Regex nonNumericRegex = new Regex(@"\D");
                if (nonNumericRegex.IsMatch(Number.ToString())  )             {
                    //Contains non numeric characters.
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool ValidateCustID(string Number)
        {
            try
            {
                //Regex nonNumericRegex = new Regex(@"\D");
                //if (nonNumericRegex.IsMatch(Number.ToString()))
                //{
                //    //Contains non numeric characters.
                //    return false;
                //}

                var errorCounter = Regex.Matches(Number, @"[a-zA-Z]").Count;
                if (errorCounter > 0){
                   return false;
                }
                return true;

            
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool ValidateNum(string Number, string Number1 = "")
        {
            try
            {
                Regex nonNumericRegex = new Regex(@"\D");
                if (nonNumericRegex.IsMatch(Number.ToString()) || nonNumericRegex.IsMatch(Number1.ToString()))
                {
                    //Contains non numeric characters.
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }


        public bool ValidateAmount(string Number)
        {
            try
            {
                Double Num1 = Convert.ToDouble(Number);
                if ((Num1) < 50 )
                {
                    //Contains non numeric characters.
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return false;
            }
        }



        public static bool ValidatePhoneNumber(string phoneNumber, out string validPhoneNumber)
        {
            bool _isValid = false; validPhoneNumber = ""; string myPhoneNumber = phoneNumber.Replace("+", ""); try
            {
                if (myPhoneNumber.Length < 11) { return false; }
                // myPhoneNumber.Length > 11               
                if (myPhoneNumber.Substring(0, 3) == "234"
                    && myPhoneNumber.Length < 13)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) != "234" && myPhoneNumber.Length > 11)
                {
                    return false;
                }
                if (myPhoneNumber.Substring(0, 3) == "234" && myPhoneNumber.Length > 13)
                {
                    return false;
                }
                if (myPhoneNumber.Length == 11)
                {
                    validPhoneNumber = "234" + myPhoneNumber.Substring(1, 10);
                }
                if (myPhoneNumber.Length == 13)
                {
                    validPhoneNumber = myPhoneNumber;
                }
                _isValid = true;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message);
                _isValid = false;
            }
            return _isValid;
        }


        public static User GetEmailAndPhone(string Email)
        {
            try
            {
         DataAccess.GlobalTransactEntitiesData db = new DataAccess.GlobalTransactEntitiesData();
          User Record = db.Users.Where(x => x.Email == Email).FirstOrDefault();
                string Paelyt = ConfigurationManager.AppSettings["PaelytEmail"].Trim();
                if (Record.Email.Trim() == Paelyt)
                {
                    User newRecord = new User();
                    newRecord.Email = null;
                    newRecord.Phone = null;
                    return newRecord;
                   

                }
                if (Record == null)
                {
                    return null;
                }
                return Record;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
public class TransactionAlert
{
    public virtual string BaseUrl { get; set; }
    public virtual string Username { get; set; }
    public virtual string Password { get; set; }
    public virtual string Sender { get; set; }
    public virtual string PhoneNumber { get; set; }
    public virtual string Message { get; set; }
}

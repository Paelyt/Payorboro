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

namespace PaytrxApp.classes
{
    public class Utility
    {
        DataProvider dataprovider = new DataProvider();
        public string getSubscriber(int id)
        {
            try
            {
                var returnValue = "";
                var check = dataprovider.getpublicKeybymerIDs(id);
                //if ((check.ID != 0 || check.ID != Convert.ToInt16(null))) {
                if(check == true) {
                    var checks = dataprovider.getpublicKeybymerID(id);
                    var IfSubscribe = dataprovider.isEmail(checks.ID);
                    if (IfSubscribe != null)
                    {
                        if (IfSubscribe.IsEmail == 1 && IfSubscribe.IsText == 1)
                        {
                            returnValue = "11/" + IfSubscribe.Id;
                            return returnValue;
                        }
                        if (IfSubscribe.IsEmail == 0  && IfSubscribe.IsText == 1)
                        {
                            returnValue = "01/" + IfSubscribe.Id;
                            return returnValue;
                        }
                        if (IfSubscribe.IsEmail == 1 && IfSubscribe.IsText == 0 )
                        {
                            returnValue = "10/" + IfSubscribe.Id;
                            return returnValue;
                        }
                        if (IfSubscribe.IsEmail == 0  && IfSubscribe.IsText == 0 )
                        {
                            returnValue = "00/" + IfSubscribe.Id;
                            return returnValue;
                        }
                        return returnValue;
                    }
                    return returnValue;
                }

                else {
                    return "null";
                }
                
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());            return null;
            }

        }

        public string SendText(string phone,string firstname,string lastname)
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

        public Boolean SendEmail(string firstname,string lastname,string email)
        {
            var bodyTxt = File.ReadAllText(HttpContext.Current.Server.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
            bodyTxt = bodyTxt.Replace("$MerchantName", $"{firstname} {lastname}");
            var msgHeader = $"Welcome to Paybills";

            var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, email, null, null);
            return true;
        }
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

        public static string Json(object value)
        {
            return new JavaScriptSerializer().Serialize(value);
        }

        //public static string GetUniqueNumberByEntity(string entity)
        //{
        //    try
        //    {
        //        var uniqueNumber = new UniqueReferenceDAO().GetByEntity(entity);
        //        if (uniqueNumber == null)
        //        {
        //            uniqueNumber = new UniqueReference { Entity = $"{entity}", IsActive = true, UniqueSeed = 1 };
        //            new UniqueReferenceDAO().Save(uniqueNumber);
        //        }
        //        var value = uniqueNumber.UniqueSeed.ToString();
        //        uniqueNumber.UniqueSeed += 1;
        //        new UniqueReferenceDAO().Update(uniqueNumber);
        //        return value.PadLeft(10, '0');
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //    }
        //    return null;
        //}

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
        //public static string JsonSerialize(object obj)
        //{
        //    try
        //    {
        //        var stringifiedObj = JsonConvert.SerializeObject(obj, Formatting.Indented,
        //            new JsonSerializerSettings
        //            {
        //                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //                ContractResolver = new NHibernateContractResolver()
        //            });
        //        return stringifiedObj;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //        return null;
        //    }
        //}

        //public static T JsonDeserialize<T>(string json)
        //{
        //    var deserializedObj = default(T);
        //    try
        //    {
        //        deserializedObj = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
        //        {
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        //            ContractResolver = new NHibernateContractResolver()
        //        });
        //        return deserializedObj;
        //    }
        //    catch (Exception ex)
        //    {
        //        WebLog.Log(ex);
        //        return deserializedObj;
        //    }
        //}
        //public class NHibernateContractResolver : DefaultContractResolver
        //{
        //    protected override JsonContract CreateContract(Type objectType)
        //    {
        //        return base.CreateContract(typeof(NHibernate.Proxy.INHibernateProxy).IsAssignableFrom(objectType) ? objectType.BaseType : objectType);
        //    }
        //}

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

        public  string createTokenSesion() {
            string token = $"sk_{RandomString(56).ToUpper()}";
            HttpContext.Current.Session["token"] = token;

            return token;

        }


        public static string TransactionGenerator()
        {
            int maxSize = 6;
            char[] chars = new char[62];
            string a;
            a = "123456";
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

      
    }
}

using DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace GloballendingViews.Classes
{
    public static class MyUtility
    {
        


        public static string getReferralCode(string userID)
        {
            string referralcode = Left(GenerateRefNo(), 6);
            try
            {
                if (userID.Length == 1)
                {
                    referralcode = userID + Left(GenerateRefNo(), 5);
                }
                else if (userID.Length == 2)
                {
                    referralcode = userID + Left(GenerateRefNo(), 4);
                }
                else if (userID.Length == 3)
                {
                    referralcode = userID + Left(GenerateRefNo(), 3);
                }
                else if (userID.Length == 4)
                {
                    referralcode = userID + Left(GenerateRefNo(), 2);
                }
                else if (userID.Length == 5)
                {
                    referralcode = userID + Left(GenerateRefNo(), 1);
                }
                else
                {
                    referralcode = userID;
                }
            }
            catch
            {
                referralcode = userID;
            }
            return referralcode;
        }

        public static int SaveWallet(TransactionLog tLog, string narration, int userID, double discountValue, double discountamount = 0)
        {
            int walletID = 0;
            try
            {
                if (userID == 0)
                    return walletID;
                
                GlobalTransactEntitiesData pwr = new GlobalTransactEntitiesData();
                
        var qry = pwr.CustomerWallets
            .Where(x => x.RefNumber ==tLog.ReferenceNumber && x.User_FK==userID).ToList();

                if (qry == null || qry.Count == 0)
                {
                   
                    double walletPoint = Convert.ToDouble(tLog.Amount)   * 0.01 *  discountValue;
                    CustomerWallet cwObj = new CustomerWallet
                    {
                        Credit = discountamount == 0 ? walletPoint : discountamount,
                        CustomerID = tLog.CustomerID,
                        Debit = 0,
                        IsVisible = 1,
                        Narration = narration,
                        RefNumber = tLog.ReferenceNumber,
                        TrnDate = tLog.TrnDate,
                        User_FK = userID,
                        ValueDate = Utility.GetCurrentDateTime().ToString("yyyy/MM/dd")
                    };
                    pwr.CustomerWallets.Add(cwObj);
                    pwr.SaveChanges();
                    walletID = cwObj.ID;
                   // return cwObj.ID;
                }


}
            catch (Exception ex)
            {
               // WebLog.Log(ex.Message + "##", ex.StackTrace);
            }
            return walletID;
        }

        public static string getReferralCodeWithMyOwnReferralcode(string myOwnRefferalcode, out int userfk)
        {
            userfk = 0;
            string referralCode = "";
            try
            {
                if (myOwnRefferalcode.Length == 0)
                    return referralCode;

                GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
                var qry = db.Users
                    .Where(x => x.MyReferalCode == myOwnRefferalcode).ToList();

                if (qry == null || qry.Count == 0)
                {
                    return referralCode;
                }
                referralCode = qry.FirstOrDefault().Referal;
                userfk = qry.FirstOrDefault().id;
            }
            catch (Exception ex)
            {
               // WebLog.Log(ex.Message + "##", ex.StackTrace);
            }
            return referralCode;
        }

        public static string getMyReferralcode(int userID, out int referraluserid)
        {
            string referralCode = "";
            referraluserid = 0;
            try
            {
                if (userID == 0)
                    return referralCode;

               // PowerNowEntities pwr = new PowerNowEntities();
                GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
                var qry = db.Users
                    .Where(x => x.id == userID).ToList();

                if (qry == null || qry.Count == 0)
                {
                    return referralCode;
                }
                referralCode = qry.FirstOrDefault().Referal;
                referraluserid= qry.FirstOrDefault().id;
            }
            catch (Exception ex)
            {
               // WebLog.Log(ex.Message + "##", ex.StackTrace);
            }
            return referralCode;
        }

      
        public static void insertWallet(int userID,TransactionLog trxRecord)
        {
            if (userID > 0)
            {
                int referraluserid = 0;
                string myReferalcode = getMyReferralcode(userID, out referraluserid);
                if (myReferalcode == null || myReferalcode == "")
                {
                    myReferalcode = "0";
                }
                if (myReferalcode.Length > 3)
                {
                    double discountVal = Convert.ToDouble(ConfigurationManager.AppSettings["FirstLevelreferralPerVending"]);
                    SaveWallet(trxRecord, "Referral point", referraluserid, discountVal);
                   int myreferraluserid = 0;
                    string level1Referrer =getMyReferralcode(referraluserid, out myreferraluserid);
                    if (level1Referrer.Length > 0)
                    {
                        discountVal = Convert.ToDouble(ConfigurationManager.AppSettings["SecondLevelreferralPerVending"]);
                        SaveWallet(trxRecord, "Referral point", myreferraluserid, discountVal);
                    }

                }
            }

        }
        public static string Left(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length <= maxLength
                   ? value
                   : value.Substring(0, maxLength)
                   );
        }
        public static string Right(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value)) return value;
            maxLength = Math.Abs(maxLength);

            return (value.Length >= maxLength
                   ? value
                   : value.Substring(value.Length, maxLength)
                   );
        }



        public static double GetMax(double first, double second)
        {
            if (first > second)
            {
                return first;
            }

            else if (first < second)
            {
                return second;
            }
            else
            {
                return 999999999;
            }
        }

        public static int getRefferalLevel(string referralCode)
        {
            int refLevel = 0;
            try
            {
                if (referralCode.Length < 3)
                {
                    return refLevel;
                }
                GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
                // PowerNowEntities pwr = new PowerNowEntities();
                DataAccess.User user = new DataAccess.User();
                var trx = db.Users.Where(x => x.Referal == referralCode).ToList();
                if (trx == null || trx.Count == 0)
                    return refLevel;

                refLevel = 1;
                User usrObj = new DataAccess.User();
                usrObj = trx.FirstOrDefault();
                trx = db.Users.Where(x => x.Referal == usrObj.MyReferalCode).ToList();
                if (trx == null || trx.Count == 0)
                    return refLevel;
                refLevel = 2;
            }
            catch (Exception ex)
            {
               // WebLog.Log(ex.Message + "####", ex.StackTrace);
            }
            return refLevel;
        }
        public static int UnixTimeStampUtc()
        {
            var currentTime = DateTime.Now;
            var zuluTime = currentTime.ToUniversalTime();
            var unixEpoch = new DateTime(1970, 1, 1);
            var unixTimeStamp = (Int32)(zuluTime.Subtract(unixEpoch)).TotalSeconds;
            return unixTimeStamp;
        }
        public static string Create7DigitString(int xter)
        {
            var rng = new Random();
            var builder = new StringBuilder();
            while (builder.Length < xter)
            {
                builder.Append(rng.Next(10).ToString());
            }
            var refNumber = builder.ToString();
            return refNumber;
        }
        public static string ConvertToCurrency(string amount)
        {
            return string.Format("{0:N2}", Convert.ToDecimal(amount));
        }
        public static string GenerateRefNo()
        {

            return UnixTimeStampUtc().ToString() + Create7DigitString(4);
            //  return InstantTimeTicks + InstantTimeSeconds;
        }

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }
}
using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;


namespace DataAccess
{
    public class DataReaders

    {
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        User users = new User();
        UserRole userrole = new UserRole();


         string connectionStrings = ConfigurationManager.AppSettings["ConnectionString"];
        //string con = ConfigurationManager.AppSettings["Secret_Key"];

        public List<Report> GetBanktransactions(int userid, int BankUserCode,DateTime fromDate, DateTime toDate)
        {
            try
            {
              var Transaction =  (from a in db.TransactionLogs
                join c in db.PaymentLogs on a.ID equals c.ID
                where a.Customer_FK == userid
                && c.TrnDate >= fromDate
                && c.TrnDate <= toDate
                orderby c.TrnDate
               
                select new Report
                {
                    Amount = a.Amount.ToString(),
                    CustomerID = a.CustomerID,
                    CustomerName = a.CustomerName,
                    Customer_FK = a.Customer_FK.ToString(),
                    Merchant_FK = a.Merchant_FK.ToString(),
                    ReferenceNumber = a.ReferenceNumber,
                    ServiceCharge = a.ServiceCharge.ToString(),
                    ServiceDetails = a.ServiceDetails,
                    TransactionType = a.TransactionType.ToString(),
                    TrnDate = a.TrnDate.ToString(),
                    TrxToken = a.TrxToken,
                    ValueDate = a.ValueDate,
                    ValueTime = a.ValueTime,
                    ResponseCode = c.ResponseCode.ToString(),
                    Description = c.ResponseDescription,
                }).ToList();

                if(Transaction == null)
                {
                    return null;
                }

                return Transaction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<Report> GetBanktransactionsFromTransactionRecord(int userid, int BankUserCode, DateTime fromDate, DateTime toDate)
        {
            try
            {
             var Transaction = (from a in db.TransactionRecords where a.Customer_FK == userid   
                                                        && a.TrnDate >= fromDate
                                                        && a.TrnDate <= toDate
                                                        orderby a.TrnDate
                                       select new Report
                                       {
                                       Amount = a.Amount.ToString(),
                                       CustomerID = a.CustomerID,
                                       CustomerName = a.CustomerName,
                                       Customer_FK = a.Customer_FK.ToString(),
                                       Merchant_FK = a.Merchant_FK.ToString(),
                                       ReferenceNumber = a.ReferenceNumber,
                                       ServiceCharge = a.ServiceCharge.ToString(),
                                       ServiceDetails = a.ServiceDetails,
                                       TransactionType = a.TransactionType.ToString(),
                                       TrnDate = a.TrnDate.ToString(),
                                       TrxToken = a.TrxToken,
                                       ValueDate = a.ValueDate,
                                       ValueTime = a.ValueTime,
                                       ResponseCode = "",
                                       Description = ""
                                        }).ToList();

                if (Transaction == null)
                {
                    return null;
                }

                return Transaction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Report> BankUsersTransaction(int userid, int BankUserCode, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var Trans1 = GetBanktransactions(userid,BankUserCode,fromDate, toDate);
                var Trans2 = GetBanktransactionsFromTransactionRecord(userid, BankUserCode, fromDate, toDate);

                var Trans3 = Trans1.Concat(Trans2);
                if(Trans1 == null && Trans2 == null)
                {
                    return null;
                }

                return Trans3.ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public DataSet GetAllMarketChannels()
        {
            try
            {
                //WebLog.Log("connectionStrings : " + connectionStrings);
                //WebLog.Log("PayTrxxEntities : " + con);
                DataSet ds;


                ds = SqlHelper.ExecuteDataset(connectionStrings, CommandType.StoredProcedure, "GetAllMarketChannels");
                if (ds.Tables[0] == null)
                {
                    return null;
                }
                // DataView dv = new DataView(ds.Tables[0]);
               
                return ds;
            }
            catch (Exception ex)
            {
                //Utility.LogError(ex.Message.ToString(), ex.StackTrace.ToString());
                return null;
            }

        }

     
        public IEnumerable<MarketChannel> GetMarketChannelList()
        {
            try
            {
                DataSet rec = GetAllMarketChannels();
                var MC = (from C in rec.Tables[0].AsEnumerable()
                          select new MarketChannel
                          {
                              ID = (int)C[0],
                              Name = C[1].ToString(),
                              Description = C[2].ToString(),
                              IsVisible = (int)C[3],
                              ListOrderID = (int)C[4]
                          }
                          ).ToList();



                return MC.ToList();
            }
            catch (Exception ex)
            {
                
                return null;
            }
        }

        public bool checkCustomerServices(string CustomerID,int MerchantFk)
        {
            try
            {
              

        var CustomerService = (from a in db.CustomerServices where a.CustomerID == CustomerID && a.Merchant_FK == MerchantFk  select a).FirstOrDefault();
           if (CustomerService != null)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
      public List<CustomerService> getServiceList(int id)
        {
            try
            {
                var Record = (from a in db.CustomerServices where a.ID == id select a).ToList();

                if (Record == null)
                {
                    return null;
                }
                return Record;

            }
            catch(Exception ex)
            {
                return null; 
            }

         }
           public string GetAllServicesByID(int serviceid)
           {
            try
            {
                var Record = (from a in db.MerchantServiceTypes where a.ID == serviceid select a).FirstOrDefault();

                if (Record == null)
                {
                    return null;
                }
                return Record.Name.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TransactionLog getCustomerRec()
        {
            try
            {
                return null;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public bool ValidateRole(List<dynamic> menus, string value)
        {
            try
            {
                var sirec = menus.Find(x => x.pageurl == value);

                if (sirec != null)
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
               // WebLog.Log(ex.Message.ToString());
                return false;
            }
        }
        public TransactionLog GetRecord(string refNum)
        {
            try
            {
                var Record = (from a in db.TransactionLogs where a.ReferenceNumber == refNum select a).FirstOrDefault();

                if (Record == null)
                {
                    return null;
                }
                return Record;
            }
            catch(Exception ex)
            {
                return null;
            }
        }



        public TransactionRecord GetBankRequeryData(string refNum)
        {
            try
            {
                var Record = (from a in db.TransactionRecords where a.ReferenceNumber == refNum select a).FirstOrDefault();

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

        public TransactionLog GetRequeryData(string refNum)
        {
            try
            {
                var Record = (from a in db.TransactionLogs where a.ReferenceNumber == refNum select a).FirstOrDefault();

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


        public string GetWalletballance(int mid)
        {
            try
            {

               
                var Debit = (from a in db.CustomerWallets where a.User_FK == mid select a.Debit).ToList().Sum();
              
                var Credit = (from a in db.CustomerWallets where a.User_FK == mid select a.Credit).ToList().Sum();
                var Result = Credit - Debit;
                if (Result == null)
                {
                    Result = 0;
                    return Result.ToString();
                }
                return Result.ToString();
              
            }
            
            catch (Exception ex)
            {
                return null;
            }
        }


        public string GetLastSuccesfulTransaction(int mid)
        {
            try
            {
                var Amount = (from a in db.CustomerTransactions where a.Customer_FK == mid select a.Amount).FirstOrDefault();
                
                if (Amount == null)
                {
                    return null;
                }
                return Amount.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        
      public List<Merchant> MerchantServiceList(int mid)
        {
            try
            {
             var Record = (from a in db.Merchants where a.MerchantServiceType_FK == mid select a).ToList();

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
        
        public int GetNoOfReferals(int userid)
        {
            try
            {
            int CountReferals = 0;
      var MyreferalCode = (from a in db.Users where a.id == userid select a.MyReferalCode).FirstOrDefault();
           if (MyreferalCode == null)
           {
                    return 0;
           }
           else
           {
            CountReferals = (from a in db.Users where a.Referal == MyreferalCode select a).Count();
            }
                return CountReferals;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        public string GetReferalCode(int mid)
        {
            try
            {
                var Record = (from a in db.Users where a.id == mid select a.MyReferalCode).FirstOrDefault();

                if (Record == null)
                {
                    return null;
                }
                return Record.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetMerchantID(string AgentID)
        {
            try
            {
         var AgentKey = (from a in db.AgentUsers where a.AgentID == AgentID select a).FirstOrDefault();
             if (AgentKey == null)
                {
                    return null;
                }
                string AgtKey = Convert.ToString(AgentKey.ID);
                return AgtKey;
                
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public int selectUserIDs(User users)
        {

            try
            {
                var emails = (from a in db.Users where a.Email == users.Email select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.id = Convert.ToInt16(emails.id);
                return users.id;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public string getJson(string id)
        {
            try
            {
                var emails = (from a in db.CustomerTransactions where a.CustomerID == id  select a.CusPay2response).FirstOrDefault();

                if (emails == null)
                {
                    return null;
                }
                
                return emails;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int GetCustomerpaytype(string Refnum)
        {
            try
            {
                var Paymenttype = (from a in db.TransactionLogs where a.ReferenceNumber == Refnum select a.PaymentType).FirstOrDefault();

                if (Paymenttype == 0)
                {
                    return 0;
                }
                var val = Convert.ToInt32(Paymenttype);
                return val;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public int GetPaymentPlanID(string Bouquet)
        {
            try
            {
                var PaymentPlanID = (from a in db.StartimeServiceLists where a.PackageDescription == Bouquet select a.PaymentPlanId).FirstOrDefault();

                if (PaymentPlanID == 0)
                {
                    return 0;
                }
                var val = Convert.ToInt32(PaymentPlanID);
                return val;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
        
        public string checkEmail(string email)
        {


            try
            {
                var emails = (from a in db.Users where a.Email == email select a.Email).FirstOrDefault();

                if (emails == null)
                {
                    return null;
                }


                return emails;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }
        public User getUser(string value)
        {
            try
            {
                var user = (from a in db.Users where a.Email == value select a).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        public User getUsers(string value)
        {
            try
            {
                var user = (from a in db.Users where a.ResetPassword == value select a).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }
        public User checkValue(string value)
        {


            try
            {
                var resetPass = (from a in db.Users where a.ResetPassword == value select a).FirstOrDefault();

                if (resetPass == null)
                {
                    return null;
                }


                return resetPass;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        public List<Role> getAllRole()
        {
            try
            {
               
                 var role = (from a in db.Roles select a);

                if(role == null)
                {
                    return null;
                }
                return role.ToList();
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        public List<Pag> getAllPage()
        {
            try
            {

                var Pag = (from a in db.Pags select a);

                if (Pag == null)
                {
                    return null;
                }
                return Pag.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<pageHeader> getAllPageHeader()
        {
            try
            {

         var PagHeader = (from a in db.pageHeaders select a);

                if (PagHeader == null)
                {
                    return null;
                }
                return PagHeader.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public long getBank(string email)
        {
            try
            {
                var bankid = (from a in db.Users where a.Email == email select a.BankFk).FirstOrDefault();

                if (bankid == 0 || bankid == null)
                {
                    return 0;
                }
               // bankid = Convert.ToInt64(bankid);
                return (Int64)bankid;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }
        


        public List<User> getUserByBank(long Bankid)
        {
            try
            {

                var user = (from a in db.Users where a.BankFk == Bankid select a ).ToList();

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public User getBankByUser(string email)
        {
            try
            {

                var user = (from a in db.Users where a.Email == email select a).FirstOrDefault();

                if (user == null)
                {
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                return null;
            }
        }





        public dynamic getAllUserByBank(long Bankid)
        {
            try
            {

                var user = (from a in db.Users join b in db.Banks on a.BankFk equals b.ID where a.BankFk == Bankid select a).ToList();

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<User> getAllUser()
        {
            try
            {

                var user = (from a in db.Users select a);

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public List<Bank> getAllBank()
        {
            try
            {

                var Bank = (from a in db.Banks select a);

                if (Bank == null)
                {
                    return null;
                }
                return Bank.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        

        public List<User> getAllUserEmail()
        {
            try
            {

                var user = (from a in db.Users select a).ToList();

                if (user == null)
                {
                    return null;
                }
                return user.ToList();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public dynamic GetDetails(string RefNum)
        {
            try
            {

              
                var results = (from a in db.TransactionRecords where a.ReferenceNumber == RefNum select a).FirstOrDefault();
                    
                    
                   /* (from a in db.TransactionRecords
                               join c in db.TransactionStatus on a.TransactionStatus_FK equals c.EnumID 
                 where a.ReferenceNumber == RefNum
                               select new transacLog
                  {
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   Amount = a.Amount.Value,
                                   ServiceCharge = a.ServiceCharge.Value,
                                   TrnDate = a.TrnDate.Value,
                                   Phone = a.CustomerPhone.ToString(),
                                   ReferenceNumber = a.ReferenceNumber,
                                   ServiceDetails = a.ServiceDetails,
                                   ServiceID = a.Merchant_FK.Value,
                                   ServiceValueDetails1 = a.ServiceValueDetails1,
                                   ServiceValueDetails2 = a.ServiceValueDetails2,
                                   ServiceValueDetails3 = a.ServiceValueDetails3,
                                   ThirdPartyCode = a.ThirdPartyCode,
                  }).ToList();

                */
                return results;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool CheckTLog(string RefNum)
        {
            try
            {
                var results = (from a in db.PaymentLogs
                               join c in db.TransactionRecords on a.ReferenceNumber equals c.ReferenceNumber
                               where c.ReferenceNumber == RefNum && c.TransactionStatus_FK == 0 && a.ResponseCode == "00" select a );

                if (results == null)
                {
                    return false;
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public List<string> GetCustReceipt(string RefNum)
        {
            try
            {
                List<string> result = new List<string>();
                var results = (from a in db.PaymentLogs
                               join c in db.TransactionLogs on a.ReferenceNumber equals c.ReferenceNumber
                               //join m in db.Merchants on c.Merchant_FK equals m.ID
                               //COmmented Out today May/14/2019
                               // join m in db.Merchants on c.Merchant_FK equals m.MerchantID
                               where a.ReferenceNumber == RefNum
                               select new
                               {
                                   CustomerID = c.CustomerID,
                                   CustomerName = c.CustomerName,
                                   Amount = c.Amount,
                                   ServiceCharge = c.ServiceCharge,
                                   TrnDate = c.TrnDate,
                                   Phone = a.CustomerPhoneNumber.ToString(),
                                   ReferenceNumber = c.ReferenceNumber,
                                   ServiceDetails = c.ServiceDetails,
                                   ServiceID = c.Merchant_FK,
                                   ServiceValueDetails1 = c.ServiceValueDetails1,
                                   ServiceValueDetails2 = c.ServiceValueDetails2,
                                   ServiceValueDetails3 = c.ServiceValueDetails3,
                                   ThirdPartyCode = c.ThirdPartyCode,
                                   RespCode = a.ResponseCode,
                                   TransactType = c.TransactionType,
                                   ConfigureToken = c.Pin,
                                   ResetToken = c.ServiceCode,
                               }).ToList();

                if (results == null)
                {
                    return null;
                }
                foreach (var search in results)
                {
                    result.Add(search.CustomerID);
                    result.Add(search.CustomerName);
                    result.Add(search.Amount.ToString());
                    result.Add(Convert.ToString(search.ServiceCharge));
                    result.Add(Convert.ToString(search.TrnDate));
                    result.Add(search.Phone);
                    result.Add(search.ReferenceNumber);
                    result.Add(search.ServiceDetails);
                    result.Add(Convert.ToString(search.ServiceID));
                    result.Add(Convert.ToString(search.ServiceValueDetails1));
                    result.Add(Convert.ToString(search.ServiceValueDetails2));
                    result.Add(Convert.ToString(search.ServiceValueDetails3));
                    result.Add(Convert.ToString(search.ThirdPartyCode));
                    result.Add(search.RespCode);
                    result.Add(Convert.ToString(search.TransactType));
                }
                return result;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<AdminUser> getAllBankAdminUsers()
        {
            try
            {
                List<string> result = new List<string>();
                var results = (from a in db.Users
                               join c in db.Banks on a.BankFk equals c.ID
                               select new AdminUser
                               {
                                   firstname = a.firstname,
                                   lastname = a.lastname,
                                   Phone = a.Phone,
                                   Email = a.Email,
                                   Date = a.Date.ToString(),
                                   Name = c.Name,
                                   id = a.id,
                              }).ToList();

                if (results == null)
                {
                    return null;
                }
                return results;
                 //foreach (var search in results)
                 //{
                 //    result.Add(search.firstname);
                 //    result.Add(search.lastname);
                 //    result.Add(search.Phone);
                 //    result.Add(Convert.ToString(search.Name));
                 //    result.Add(Convert.ToString(search.Date));
                 //    result.Add(search.Email);

                 //}
                 //return result.ToList();
                

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        //
        public dynamic getDailyTransactionLogByDate(int Userfk,DateTime fromDate, DateTime toDate,int MerchantFk)
        {
            try
            {
                /*  07-August-22020  */
                var results = db.GetSuccesfulTransLog4PartnersByDate(Userfk, fromDate, toDate,MerchantFk).ToList();
                //var results = "";

              return results.ToList();
          }
          catch (Exception ex)
          {
              return null;
          }
      }

      public dynamic getDailyTransactionLog(int Userfk, DateTime fromDate, DateTime toDate,int Flag,int MerchantFk)
      {
          try
          {

              dynamic results = "";
              if (Flag == 1)
              {
                    /*  07-August-22020  */
                  results = db.GetSuccesfulTransLog4PartnersByDate(Userfk, fromDate, toDate,MerchantFk).Take(10).ToList();
            }
                else if (Flag == 0)
                {
                      /*  07-August-22020  */
                    results = db.GetSuccesfulTransLog4PartnersByDate(Userfk, fromDate, toDate,MerchantFk).ToList();
                }
               
                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /*  public List<SaleMarginRep> getSaleMarginReport(DateTime fromDate, DateTime toDate)
          {
              try
              {
                  double pecent = 0.5;
                  double pecents = 0.1;
                  var groups = from r in db.TransactionLogs
                               where r.TrnDate >= fromDate && r.TrnDate <= toDate
                               group r by r.TrnDate into row
                               select new SaleMarginRep
                               {
                      TrasactionDate = (DateTime)row.Key,
                      Transaction = row.Where(x => x.ID > 0).Select(x => x.ID).Count(),
                      TotalLoanAmount = (Int32)row.Sum(x => x.Amount),
                      SalesMargin = pecent * (Int32)row.Sum(x=>x.Amount) - (Int32)row.Sum(x => x.Amount),
                      SalesMarginBank = pecents * (Int32)row.Sum(x => x.Amount) - (Int32)row.Sum(x => x.Amount),
                               };
                  return groups.ToList();
              }
              catch(Exception ex)
              {
                  return null;
              }
              }*/


        public dynamic getSaleMarginReport(int UserFk,DateTime fromDate, DateTime toDate,int Flag)
            {
            try
            {
                dynamic results = "";
                if (Flag == 1)
                {
                     results = db.GetSalesMarginLog4PartnersByDate(UserFk, fromDate, toDate).Take(10).ToList();
                }
                else if(Flag == 0)
                {
                    results = db.GetSalesMarginLog4PartnersByDate(UserFk, fromDate, toDate).ToList();
                    
                }

                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //
       


    /*    public List<transacLog> getTransactionLogByDate(int UserFk,DateTime fromDate, DateTime toDate)
        {
            try
            {
              var results = (from a in db.GetTransactionLog4PartnersByDate(UserFk,fromDate,toDate)
                               select new transacLog
                               {
                                   Amount = a.Amount.Value,
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   MerchantName = a.MarchantName,
                                   ReferenceNumber = a.ReferenceNumber,
                                   Token = a.Token,
                                   MeterType = a.MeterType,
                                   TransactionType = a.TransactionType,
                                   TrnDate = a.TrnDate.Value,
                                   Status = a.Status,
                               }).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        */

        public dynamic getTransactionLogByDate(int UserFk, DateTime fromDate, DateTime toDate,int Flag,int MerchantFK)
        {
            try
            {
              
                dynamic results = "";
                if (Flag == 1)
                {
                      /*  07-August-22020  */
                  results = db.GetTransactionLog4PartnersByDate(UserFk, fromDate, toDate,MerchantFK).Take(10).ToList();

                }
                else if (Flag == 0)
                {
                  results = db.GetTransactionLog4PartnersByDate(UserFk, fromDate, toDate,MerchantFK).ToList();
                }

                return results;
                
            }
            catch (Exception ex)
            {
                return null;
            }
        }

      /*  public List<transacLog> getTransactionLogByDate(int UserFk,DateTime fromDate, DateTime toDate)
        {
            try
            {
              var results = (from a in db.GetTransactionLog4PartnersByDate(UserFk,fromDate,toDate)
                               select new transacLog
                               {
                                   Amount = a.Amount.Value,
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   MerchantName = a.MarchantName,
                                   ReferenceNumber = a.ReferenceNumber,
                                   Token = a.Token,
                                   MeterType = a.MeterType,
                                   TransactionType = a.TransactionType,
                                   TrnDate = a.TrnDate.Value,
                                   Status = a.Status,
                               }).ToList();

                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       */
        public List<transacLog> getTransactionLog(int UserFk)
        {
            try
            {
               
                var res = db.GetTransactionLog4Partners(UserFk).Take(10).ToList();
                var results = (from a in db.GetTransactionLog4Partners(UserFk)
                              
                               select new transacLog
                               {
                                   Amount = (double)a.Amount,
                                   CustomerID = a.CustomerID,
                                   CustomerName = a.CustomerName,
                                   MerchantName = a.MarchantName,
                                   ReferenceNumber = a.ReferenceNumber,
                                   Token = a.Token,
                                   MeterType = a.MeterType,
                                   TransactionType = a.TransactionType,
                                   TrnDate = (DateTime)a.TrnDate,
                                   Status = a.Status,
                               }).Take(10).ToList();

                return results;
              

                
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // Conv Lend/Vend Report
       // Expression<Func<int, int>> calc = int i => i* 10;

       

        public dynamic LendTransaction(int UserFk, DateTime fromDate, DateTime toDate,int Flag)
        {
            try
            {
                dynamic results = "";
                if (Flag == 1)
                {
                     results = db.GetLendTransLog4PartnersByDate(UserFk, fromDate, toDate).Take(10).ToList();
                }
                else if (Flag == 0)
                {
                    results = db.GetLendTransLog4PartnersByDate(UserFk, fromDate, toDate).ToList();
                }

                return results;
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public dynamic LendTransactionByDate(int UserFk,DateTime fromDate, DateTime toDate)
        {
            
                 
            try
            {
                var results =  db.GetLendTransLog4PartnersByDate(UserFk, fromDate, toDate).ToList();
                              

                return results.ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public dynamic CheckTransaction(int UserFk, DateTime fromDate, DateTime toDate, string Custid)
        {
            try
            {
                dynamic results = "";
                    results = db.GetTransactionByCustomerIDandDateRange(UserFk,fromDate,toDate,Custid).ToList();
               
                return results;

            }
            catch (Exception ex)
            {
                //Web
                return null;
            }
        }

       
   public dynamic DashboardSummary(int UserFk, DateTime fromDate, DateTime toDate, int Flag)
        {
            try
            {
                dynamic results = "";
                if (Flag == 1)
                {
                    /* 7-August-2020 */
                   results = db.GetPartnerSummaryDetails(fromDate,toDate, UserFk).Take(10).ToList();
                }
                else if (Flag == 0)
                {
                     results = db.GetPartnerSummaryDetails(fromDate,toDate, UserFk).ToList();
                }
                return results;

            }
            catch (Exception ex)
            {
                //Web
                return null;
            }
        }



        public dynamic VendTransaction(int UserFk,DateTime fromDate, DateTime toDate,int Flag)
        {
            try
            {
                dynamic results = "";
                if (Flag == 1)
                {
                    results = db.GetVendTransLog4PartnersByDate(UserFk, fromDate, toDate).Take(10).ToList();
                }
                else if (Flag == 0)
                {
                    results = db.GetVendTransLog4PartnersByDate(UserFk, fromDate, toDate).ToList();
                }
                

                return results;

            }
            catch (Exception ex)
            {
                //Web
                return null;
            }
        }


        public dynamic VendTransactionByDate(int UserFk,DateTime fromDate, DateTime toDate)
        {


            try
            {

                var results = db.GetVendTransLog4PartnersByDate(UserFk, fromDate, toDate).ToList();
                              

                return results.ToList();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        //

        /*   public double CalculateConvFee(double val,int trantype)
              {
               try
               {
                   double value = 0;
                   if (trantype == 3)
                   {
                        value = 10 / 100 * val;
                   }
                   if (trantype == 2)
                   {
                       value = 8 / 100 * val;
                   }

                   value = val - value;
                   return value;
               }
               catch(Exception ex)
               {
                   return 0;
               }
           }

           public double Calculate2ConvFee(double val,int trantype)
           {
               try
               {
                   double value = 0;
                   if (trantype == 3)
                   {
                       value = 10 / 100 * val;
                   }
                   if (trantype == 2)
                   {
                       value = 8 / 100 * val;
                   }
                   value = val - value;
                   return value;
               }
               catch (Exception ex)
               {
                   return 0;
               }
           }*/
        public string getXml(string RefNum)
        {
            try
            {
                var CustTransac = db.CustomerTransactions.Find(RefNum);
                if (CustTransac != null)
                {
                    return CustTransac.CusPay2response;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string GetCustTransac(string RefNum)
        {
            try
            {
                var CustTransac = db.CustomerTransactions.Find(RefNum);
                if (CustTransac != null)
                {
                    return CustTransac.ReferenceNumber;
                }
                if(CustTransac == null)
                {
                    return null;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TransactionLog GetTransactionLog(string RefNum)
        {
            try
            {
                var CustTransac = db.TransactionLogs.Find(RefNum);
                if (CustTransac != null)
                {
                    return CustTransac;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool Validate(string value)
        {
            try
            {
                var user = (from a in db.Users
                            where a.Email == value
                            select a).FirstOrDefault();
                if (user != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool ValidateRole(string value)
        {
            try
            {
                var role = (from a in db.Roles
                            where a.RoleName == value
                            select a).FirstOrDefault();
                if (role != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       public bool GetCustomer(string val1 , string val2)
        {
            try
            {
         var cust = (from a in db.PaymentLogs
         where a.CustomerID == val1 && a.ReferenceNumber == val2
         select a).FirstOrDefault();
           if(cust != null)
            {
              return true;
            }
              return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public bool ValidatePage(string value)
        {
            try
            {
                var pag = (from a in db.Pags
                            where a.PageName == value
                            select a).FirstOrDefault();
                if (pag != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool validateRefNum(string value)
        {
            try
            {
                var Transac = db.CustomerTransactions.Find(value);
                if (Transac != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       

            public bool CheckTranToken(string value)
            {
            try
            {
                var Trxtoken = (from a in db.PaymentLogs
                           where a.TrxToken  == value select a).FirstOrDefault();
                if (Trxtoken != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
       

    }
    public bool ValidateCusTrancLog(string value)
        {
            try
            {
                var Transac = db.TransactionLogs.Find(value);
                if (Transac != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool ValidatePayTrancLog(string value)
        {
            try
            {
                var Transac = db.PaymentLogs.Find(value);
                if (Transac != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool ValidateCusTranc(string value)
        {
            try
            {
                //var Transac = (from a in db.CustomerTransactions
                //where a.ReferenceNumber == value
                // select a).FirstOrDefault();
                var Transac = db.CustomerTransactions.Find(value);
                if (Transac != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ValidateAgent(string ContactEmail)
        {
            try
            {
                var ContactMail = db.Agents.Find(ContactEmail);
                var ContactMail0 = (from a in db.Users
                                    where a.Email == ContactEmail
                                    select a).FirstOrDefault();
                if (ContactMail == null && ContactMail0 == null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool ValidatePassword(string Password, string ConfirmPass)
        {
            try
            {
                string value = Password;
                string value1 = ConfirmPass;
                if (value == value1)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //public bool ValidateRole(string value)
        //{
        //    try
        //    {
        //        var rol = (from a in db.Roles
        //                   where a.RoleName == value
        //                   select a).FirstOrDefault();

        //        if (rol != null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        //public bool ValidatePage(string value)
        //{
        //    try
        //    {
        //        var pag = (from a in db.Pags
        //                   where a.PageName == value
        //                   select a).FirstOrDefault();

        //        if (pag != null)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}
        public List<Role> GetAllRoles()
        {
            try
            {
                var Role = (from a in db.Roles

                            select a).ToList();

                if (Role == null)
                {
                    return null;
                }
                return Role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string CheckCustomerEligiblity(string CustomerID, int MerchantFk)
        {
            try
            {
                string ResponseMsg = "";
                var Response = (from a in db.EligibleCustomers
                                where a.CustomerID == CustomerID && a.Merchant_Fk
                                == MerchantFk
                                select a).FirstOrDefault();
                if (Response == null)
                {
                    ResponseMsg = "Customer is Not Available";
                }
                if (Response != null)
                {
                    if (Response.CreditLimit > 0)
                    {

                        ResponseMsg = "Customer is Eligible To Borrow";
                    }

                }
                return ResponseMsg;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool ValidateCustomer(string value)
        {
            try
            {
                var customer = (from a in db.Customers
                                where a.ContactEmail == value
                                select a).FirstOrDefault();

                if (customer != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public List<int> buildAllRoleList()
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in db.Roles select a.RoleId ).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }

        }



        public List<int> buildAllBankRoleList()
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in db.Roles where a.isVissible == 2 select a.RoleId).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public List<int> buildPagesList(Pag pages)
        {
            try
            {

                List<int> pageids = new List<int>();
                pageids = (from a in db.PageAuthentications where a.PageName == pages.PageName select a.RoleId.Value).ToList();

                if (pageids == null)
                {
                    return null;
                }
                return pageids;


            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public List<int> buildNamesList(User users)
        {
            try
            {

                List<int> roleids = new List<int>();
                roleids = (from a in db.UserRoles where a.UserId == users.id select a.RoleId.Value).ToList();

                if (roleids == null)
                {
                    return null;
                }
                return roleids;


            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public int selectRolesByName(UserRole userrole)
        {

            try
            {
                var rol = (from a in db.UserRoles where a.RoleId == userrole.RoleId && a.UserId == userrole.UserId select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.RoleId);
                int roleid = user;
                userrole.RoleId = roleid;
                return user;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }

        }
       //public List<User> findAllUser()
       // {
       //     try
       //     {
       //         var Users = (from a in db.Users select a.Email).FirstOrDefault();

       //         if (Users == null)
       //         {
       //             return null;
       //         }
       //         return Users;
       //     }
       //     catch(Exception ex)
       //     {
       //         return null;
       //     }
       // }
        public int selectUserID(User users)
        {

            try
            {
                var emails = (from a in db.Users where a.Email == users.Email select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.id = Convert.ToInt16(emails.id);
                return users.id;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }


        public int selectPageID(Pag pages)
        {

            try
            {
                var curpage = (from a in db.Pags where a.PageName == pages.PageName select a).FirstOrDefault();

                if (curpage == null)
                { 
               
                    return 0;
                }
                curpage.id = Convert.ToInt16(curpage.id);
                return curpage.id;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }
        public int GetUserIdByEmail(string email)
        {

            try
            {
                var emails = (from a in db.Users where a.Email == email select a).FirstOrDefault();

                if (emails == null)
                {
                    return 0;
                }
                users.id = Convert.ToInt16(emails.id);
                return users.id;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public bool loggedIn(string username, string password)
        {
            try
            {
                var Loggedin = (from a in db.Users
                                where a.Email == username && a.pasword == password
                                select a).FirstOrDefault();

                if (Loggedin != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                //return ex.Message.ToString();
                return false;
            }
        }


        public bool CheckServiceType(string CustomerIdLabel, string CustomerId)
        {
            try
            {
                var Loggedin = (from a in db.CustomerServices
                                where a.CustomerIDLabel == CustomerIdLabel && a.CustomerID == CustomerId
                                select a).FirstOrDefault();

                if (Loggedin != null)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public int selectPageRolesByName(PageAuthentication _pa)
        {

            try
            {
                var rol = (from a in db.PageAuthentications where a.RoleId == _pa.RoleId && a.PageName == _pa.PageName select a).FirstOrDefault();

                if (rol == null)
                {
                    return 0;
                }


                int user = Convert.ToInt32(rol.RoleId);
                int roleid = user;
                userrole.RoleId = roleid;
                return user;
            }
            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
                return 0;
            }


        }

        public string getSelectedPage(int value)
        {
            try
            {
                var page = (from a in db.Pags
                            where a.id == value
                            select a).FirstOrDefault();

                if (page != null)
                {
                    return page.PageName;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetPhone(string value)
        {
            try
            {
                var CustTrans = (from a in db.PaymentLogs
                                 where a.ReferenceNumber == value
                                 select a.CustomerPhoneNumber).FirstOrDefault();

                if (CustTrans != null)
                {
                    return CustTrans;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CustomerTransaction PayLoads(string value)
        {
            try
            {
                var CustTrans = (from a in db.CustomerTransactions
                                 where a.ReferenceNumber == value
                                 select a).FirstOrDefault();

                if (CustTrans != null)
                {
                    return CustTrans;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public TransactionLog PayLoad(string value)
        {
            try
            {
                var CustTrans = (from a in db.TransactionLogs
                                 where a.ReferenceNumber == value
                                 select a).FirstOrDefault();

                if (CustTrans != null)
                {
                    return CustTrans;
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<CustomerService> SearchCustomerType(int userid, string search, int? i)
        {
            try
            {
                List<CustomerService> listCustomer = (db.CustomerServices.Where(x => x.CustomerID.StartsWith(search) || search == null && x.Customer_FK == userid).ToList());

                //List<CustomerService> listCustomer = db.CustomerServices.ToList(); 

                return listCustomer;
            }
            catch (Exception ex)
            {
                return null;

            }


        }

        public Double Balance(int userid)
        {
            try
            {
                // float sum = (from row in db.AgentAccounts
                //  select sum(row.Credit));
                var Balance = (from p in db.AgentAccounts
                               where p.AgentUser_FK == userid
                               group p by p.AgentUser_FK into g
                               select new BalanceType
                               {
                                   //AgentAccount = g.Key,
                                   CreditSum = g.Sum(p => (double)p.Credit),
                                   DebitSum = g.Sum(p => (double)p.Debit),

                               }).FirstOrDefault();
                var NewBalance = Balance.CreditSum - Balance.DebitSum;
                return NewBalance;

            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public Double GetWalletBalance(int userid)
        {
            try
            {
                // float sum = (from row in db.AgentAccounts
                //  select sum(row.Credit));
                var Balance = (from p in db.AgentAccounts
                               where p.AgentUser_FK == userid
                               group p by p.AgentUser_FK into g
                               select new BalanceType
                               {
                                   //AgentAccount = g.Key,
                                   CreditSum = g.Sum(p => (double)p.Credit),
                                   DebitSum = g.Sum(p => (double)p.Debit),

                               }).FirstOrDefault();
                var NewBalance = Balance.CreditSum - Balance.DebitSum;
                return NewBalance;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        // I Did this today ...
        public Double WalletBalance(int userid)
        {
            try
            {
                // float sum = (from row in db.AgentAccounts
                //  select sum(row.Credit));
                var Balance = (from p in db.CustomerWallets
                               where p.User_FK == userid
                               group p by p.User_FK into g
                               select new BalanceType
                               {
                                   //AgentAccount = g.Key,
                                   CreditSum = g.Sum(p => (double)p.Credit),
                                   DebitSum = g.Sum(p => (double)p.Debit),

                               }).FirstOrDefault();
                var NewBalance = Balance.CreditSum - Balance.DebitSum;
                return NewBalance;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public class BalanceType
        {
            public double DebitSum { get; set; }

            public double CreditSum { get; set; }

        }

        public List<StartimesService> GetAllServices()
        {
            try
            {
                var Services = (from a in db.StartimesServices select a).ToList();

                if (Services == null)
                {
                    return null;
                }
                return Services;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public List<StartimeServiceList> GetAllServicesBySat(int id)
        {
            try
            {
                var ServiceList = (from a in db.StartimeServiceLists where a.StartimeServiceFk == id select a).ToList();

                if (ServiceList == null)
                {
                    return null;
                }
                return ServiceList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public int getSatid(string satName)
        {
            try
            {
                var ServiceID = (from a in db.StartimesServices where a.Name == satName select a.id).FirstOrDefault();

                if (ServiceID == 0)
                {
                    return 0;
                }
                return ServiceID;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int getMerchantFk(string refnum)
        {
            try
            {
                var MercFk = (from a in db.TransactionLogs where a.ReferenceNumber == refnum select a.Merchant_FK).FirstOrDefault();

                if (MercFk == 0)
                {
                    return 0;
                }
                return (int)MercFk;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        
        public List<Role> getUserRoles(List<int> roleids)
        {
            //  here i am 
            try
            {
                var roles = (from p in db.Roles where roleids.Contains((int)(p.RoleId)) select p).ToList();
                if (roles == null)
                {
                    return null;
                }

                return roles;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        public List<Merchant> getAllMerchant()
        {
            try
            {
             var Merchant = db.Merchants;

                if (Merchant == null)
                {
                    return null;
                }

                return Merchant.ToList();
            }
            catch (Exception ex)
            {

              
                return null;
            }


        }


        public dynamic GetAllPatners()
        {
            try
            {
                var transactions = (from a in db.Users
                                    join c in db.AgentUsers on a.id equals c.User_FK
                                   
                                     select new Partner
                                     {
                                         firstname = a.firstname,
                                         Email = c.EmailAddress,
                                         PartnerFk = c.PartnerFk.ToString(),
                                         userFk = a.id,
                                     }).ToList();
                                    
                return transactions;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        public List<Merchant> getBankMerchant()
        {
            try
            {
             var Merchant = (from x in db.Merchants where x.IsVisible == 1 select x).ToList();

                if (Merchant == null)
                {
                    return null;
                }

                return Merchant.ToList();
            }
            catch (Exception ex)
            {
              return null;
            }


        }
        public User getUserID(string email)
        {
            try
            {
                var users = (from a in db.Users where a.Email == email select a).FirstOrDefault();

                if (users == null)
                {
                    return null;
                }

                return users;
            }
            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        public List<PaymentLog> getAllTransactionSum(int id)
        {
            try
            {
                List<string> result = new List<string>();
                // var transactions = (from  c in db.PaymentLogs 
                // where c.ID == id select c).ToList();
                var transactions = (from a in db.TransactionLogs
                                    join c in db.PaymentLogs on a.ID equals c.ID
                                    where c.ResponseCode == "00" || c.ResponseDescription == "successful"
                                    select c).ToList();


                if (transactions == null)
                {
                    return null;
                }

                return transactions;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public List<PaymentLog> getCustomerTransactionSum(int id)
        {
            try
            {
                List<string> result = new List<string>();
                // var transactions = (from  c in db.PaymentLogs 
                // where c.ID == id select c).ToList();
                var transactions = (from a in db.TransactionLogs
                join c in db.PaymentLogs on a.ID equals c.ID
                where a.Customer_FK == id select c).ToList();


                if (transactions == null)
                {
                    return null;
                }

                return transactions;
            }
            catch (Exception ex)
            {
                return null;
            }

        }
        public List<string> getCustomerTransactionRecords(int id)
          {
              try
              {
                  List<string> result = new List<string>();
                  var transactions = (from a in db.TransactionLogs
                                      join c in db.PaymentLogs on a.ID equals c.ID
                                      where a.ID == id

                                      select new Report
                                      {
                                          Amount = a.Amount.ToString(),
                                          CustomerID = a.CustomerID,
                                          CustomerName = a.CustomerName,
                                          Customer_FK = a.Customer_FK.ToString(),
                                          Merchant_FK = a.Merchant_FK.ToString(),
                                          ReferenceNumber = a.ReferenceNumber,
                                          ServiceCharge = a.ServiceCharge.ToString(),
                                          ServiceDetails = a.ServiceDetails,
                                          TransactionType = a.TransactionType.ToString(),
                                         TrnDate
                                          = a.TrnDate.ToString(),
                                          TrxToken = a.TrxToken,
                                          ValueDate = a.ValueDate,
                                          ValueTime = a.ValueTime,
                                          ResponseCode = c.ResponseCode.ToString(),
                                          Description = c.ResponseDescription,
                                      }).ToList();


                  if (transactions == null)
                  {
                      return null;
                  }
                  foreach (var search in transactions)
                  {
                      result.Add(search.CustomerID);
                      result.Add(search.CustomerName);
                      result.Add(search.Amount.ToString());
                      result.Add(Convert.ToString(search.ServiceCharge));
                      result.Add(Convert.ToString(search.TrnDate));
                      result.Add(search.ResponseCode);
                      result.Add(search.ReferenceNumber);
                      result.Add(search.ServiceDetails);
                      result.Add(search.TrnDate);
                      result.Add(search.Description);

                  }
                  return result.ToList();
              }
              catch (Exception ex)
              {
                  return null;
              }

          }


        public  User getUserProfile(int id)
        {
            try
            {

                var users = (from a in db.Users where a.id == id select a).FirstOrDefault();
                if(users == null)
                {
                    return null;
                }
                return users;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Role getRole(int id)
        {
            try
            {

                var role = (from a in db.Roles where a.RoleId == id select a).FirstOrDefault();
                if (role == null)
                {
                    return null;
                }
                return role;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public Pag getPage(int id)
        {
            try
            {

                var pag = (from a in db.Pags where a.id == id select a).FirstOrDefault();
                if (pag == null)
                {
                    return null;
                }
                return pag;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       
       /* public string getIndeQuery()
        {
            try
            {
                GlobalTransactEntitiesData gb = new GlobalTransactEntitiesData();
                 int id = 0;
                    // getRecord();
                    var Data = (from a in gb.TransactionLogs
                                    join c in gb.PaymentLogs on a.ID equals c.ID
                                    where a.Customer_FK == mc.id
                                    orderby c.TrnDate descending
                                    select new Paytv.Report
                                    {
                                        Amount = a.Amount.ToString(),
                                        CustomerID = a.CustomerID,
                                        CustomerName = a.CustomerName,
                                        Customer_FK = a.Customer_FK.ToString(),
                                        Merchant_FK = a.Merchant_FK.ToString(),
                                        ReferenceNumber = a.ReferenceNumber,
                                        ServiceCharge = a.ServiceCharge.ToString(),
                                        ServiceDetails = a.ServiceDetails,
                                        TransactionType = a.TransactionType.ToString(),
                                        TrnDate = a.TrnDate.ToString(),
                                        TrxToken = a.TrxToken,
                                        ValueDate = a.ValueDate,
                                        ValueTime = a.ValueTime,
                                        ResponseCode = c.ResponseCode.ToString(),
                                        Description = c.ResponseDescription,
                                    }).ToList().Take(10);
                
              
            }
            catch (Exception ex)
            {
              
                return null;
            }
        }
        */

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

        public class transacLog
        {
            public double Amount { get; set; }
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string ReferenceNumber { get; set; }

            public string Token { get; set; }

            public string MeterType { get; set; }
          
            public string TransactionType { get; set; }
            public DateTime TrnDate { get; set; }
           
            public string Status { get; set; }

            public string MerchantName { get; set; }

            public double ServiceCharge { get; set; }

            public string Phone { get; set; }

            public string ServiceDetails { get; set; }

            public int ServiceID { get; set; }

            public string ServiceValueDetails1 { get; set; }

            public string ServiceValueDetails2 { get; set; }

            public string ServiceValueDetails3 { get; set; }

            public string ThirdPartyCode { get; set; }





        }


        public class AdminUser
        {
            public int id { get; set; }
            public string firstname { get; set; }
            public string lastname { get; set; }
            public string Phone { get; set; }
           
            public string Email { get; set; }

            public string Date { get; set; }

            public string Name { get; set; }


        }

        public class ConvFeeReport
        {
            public int id { get; set; }
            public DateTime TrasactionDate { get; set; }
            public string ReferenceNum { get; set; }
            public string CustomerId { get; set; }

            public string CustomerName { get; set; }

            public double Amount { get; set; }

            public string TransactionType { get; set; }

            public double Convfee { get; set; }

            public double bankshare { get; set; }

            public double GplShare { get; set; }


        }

        public class SaleMarginRep
        {
            public int id { get; set; }
            public DateTime TrasactionDate { get; set; }
            public int Transaction { get; set; }
            public double TotalLoanAmount { get; set; }
            public double SalesMargin { get; set; }
            public double SalesMarginBank { get; set; }

        }

        public class Partner
        {
            public string firstname { get; set; }

            public string Email { get; set; }

            public string PartnerFk { get; set; }

            public int userFk { get; set; }
        }

        public class MarketChannel
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public int IsVisible { get; set; }
            public int ListOrderID { get; set; }
        }




    }
}

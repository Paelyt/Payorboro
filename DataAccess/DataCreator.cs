using Microsoft.ApplicationBlocks.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DataAccess
{
   public class DataCreator
    {
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        CustomerTransaction _ct = new CustomerTransaction();
        CustomerWallet _cW = new CustomerWallet();
        PaymentLog _pL = new PaymentLog();
        string connectionStrings = ConfigurationManager.AppSettings["ConnectionString"];

        public int insertMarketChannel(string Mc, int id)
        {
            try
            {
              
                
                    int ds = SqlHelper.ExecuteNonQuery(connectionStrings, CommandType.StoredProcedure,
                    "GetAllMarketChannels", new SqlParameter("@User_FK", id),
                    new SqlParameter("@MarketingChannel_FK", Mc),
                    new SqlParameter("@DateCreated", DateTime.Today),
                     new SqlParameter("@ValueDate", DateTime.Today.ToString()),
                        new SqlParameter("@ValueTime", DateTime.Today.ToString()),
                    new SqlParameter("@IsVisible", 1)
                    );

                return ds;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public int insertMarketChannels(MarketingDetail MC)
        {
            try
            {

                db.MarketingDetails.Add(MC);
                db.SaveChanges();
                return MC.ID;
            }
            catch (Exception ex)
            {
                //WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }
        public string InsertCustomerWallet(TransactionLog _tL)
        {
            try
            {
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);
                
                if (original != null)
                {
                    _cW.Credit = 0;
                    _cW.Debit = original.Amount;
                    _cW.CustomerID = original.CustomerID;
                    _cW.Narration = "Pay With Wallet";
                    _cW.RefNumber = original.ReferenceNumber;
                    _cW.ValueDate = original.ValueDate;
                    _cW.User_FK = original.Customer_FK;
                    db.CustomerWallets.Add(_cW);
                    db.SaveChanges();
                 }
                InsertPayLog(_tL);
                //updateTransactionLog(_tL);
                return original.ID.ToString();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public string UpdateTransaction(TransactionLog _tL)
        {
            try
            {

                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.ThirdPartyCode = _tL.ThirdPartyCode;
                    original.ServiceValueDetails2 = _tL.ServiceValueDetails2;
                    original.ServiceValueDetails3 = _tL.ServiceValueDetails3;
                  
                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string InsertPayLog(TransactionLog _tL)
        {
            try
            {
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                   
                    _pL.Amount = original.Amount;
                    _pL.CustomerID = original.CustomerID;
                    _pL.CustomerEmail = original.CustomerEmail;
                    _pL.CustomerPhoneNumber = original.CustomerPhone;
                    _pL.ReferenceNumber = original.ReferenceNumber;
                    _pL.ResponseCode = "00";
                    _pL.ResponseDescription = "successful";
                    _pL.TrnDate = original.TrnDate;
                    _pL.ValueDate = original.ValueDate;
                    _pL.ValueTime = original.ValueTime;
                    _pL.IsPaid = 2;

                    db.PaymentLogs.Add(_pL);
                    db.SaveChanges();

                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string Customertransaction(CustomerTransaction _tL)
        {
            try
            {
                var original = db.CustomerTransactions.Find(_tL.ReferenceNumber);

                if (original != null)
                {

                   

                    db.CustomerTransactions.Add(_tL);
                    db.SaveChanges();

                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string updateTransactionLogs(TransactionLog _tL)
        {
            try
            {

                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.ServiceValueDetails1 = _tL.ServiceValueDetails1;
                    original.ServiceValueDetails2 = _tL.ServiceValueDetails2;
                    original.ServiceValueDetails3 = _tL.ServiceValueDetails3;
                    original.ThirdPartyCode = _tL.ThirdPartyCode;
                    original.Pin = _tL.Pin;
                    original.ServiceCode = _tL.ServiceCode;
                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UpdatePaymentLog(string refnum)
        {
            try
            {

                var original = db.PaymentLogs.Find(refnum);

                if (original != null)
                {
                   
                    original.ResponseCode = "00";
                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        

        public string UpdateCustTransactionLogx(CustomerTransaction _cL)
        {
            try
            {
                 _cL.TransactionType = 3;
                db.CustomerTransactions.Add(_cL);
                db.SaveChanges();


                 return _cL.ReferenceNumber;
            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdateTransactionLogResBody(TransactionLog _tL)
        {
            try
            {

                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                   
                    original.ResponseBody = _tL.ResponseBody;
                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string UpdateTransactionLogx(TransactionLog _tL)
        {
            try
            {

                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.Voucher = _tL.Voucher;
                    original.ServiceValueDetails2 = _tL.ServiceValueDetails2;
                    original.ServiceValueDetails3 = _tL.ServiceValueDetails3;
                    _tL.TransactionType = 00;
                    original.ResponseBody = _tL.ResponseBody;
                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string updateTransactionLog(TransactionLog _tL)
        {
            try
            {
                
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.PatnerRefNumber = _tL.PatnerRefNumber;
                    original.PatnerResponseCode = _tL.PatnerResponseCode;

                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string updateDashRequeryTransactLog(TransactionLog _pL)
        {
            try
            {

                var original = db.TransactionLogs.Find(_pL.ReferenceNumber);

                if (original != null)
                {
                    original.ServiceValueDetails1 = _pL.ServiceValueDetails1;
                    original.ServiceValueDetails2 = _pL.ServiceValueDetails2;
                    original.ServiceValueDetails3 = _pL.ServiceValueDetails3;

                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }


        public string updateDashRequeryPaymentLog(PaymentLog _pL)
        {
            try
            {

                var original = db.PaymentLogs.Find(_pL.ReferenceNumber);

                if (original != null)
                {
                    original.ResponseCode = "00";
                    original.ResponseDescription = "Successful";
                    original.TrxToken = _pL.TrxToken;

                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }



        public string updateRequeryPaymentLog(PaymentLog _pL)
        {
            try
            {

                var original = db.PaymentLogs.Find(_pL.ReferenceNumber);

                if (original != null)
                {
                    original.ResponseCode = "00";
                    original.ResponseDescription = "Successful";

                    db.SaveChanges();
                }
                return original.ID.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        // public void UpdatePassword(User users)
        //{
        //    try
        //    {
        //        var original = db.Users.Find(users.id);

        //        if (original != null)
        //        {
        //            original.pasword = users.pasword;
        //            original.confirmPassword = users.confirmPassword;
        //            db.SaveChanges();
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.Message.ToString();
        //    }
        //}
        public void UpdateUsers(User users)
        {
            try
            {
                var original = db.Users.Find(users.id);

                if (original != null)
                {
                    original.ResetPassword = users.ResetPassword;
                    original.DateTim = users.DateTim;
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
        public void UpdatePassword(User user)
        {
            try
            {
             
                var original = db.Users.Find(user.id);
               

                if (original != null)
                {



                    original.pasword = user.pasword;
                    original.confirmPassword = user.confirmPassword;


                    db.SaveChanges();
                }

            }


            catch (Exception ex)
            {

               // WebLog.Log(ex.Message.ToString());
            }

        }

        public void InsertUserRoles(UserRole userrole)
        {

            try
            {
                db.UserRoles.Add(userrole);
                db.SaveChanges();


            }

            catch (Exception ex)
            {

               // WebLog.Log(ex.Message.ToString());
            }

        }

        

            public string UpdateCustomerServiceList(int id)
        {
            try
            {

                var original = db.CustomerServices.Find(id);


                if (original != null)
                {



                    original.isVissible = 0;

                    db.SaveChanges();
                }

                return original.ID.ToString();
            }


            catch (Exception ex)
            {
                return null;
                // WebLog.Log(ex.Message.ToString());
            }

        }

        public void updatelastID(User user)
        {
            try
            {

                var original = db.Users.Find(user.id);


                if (original != null)
                {



                    original.MyReferalCode = user.MyReferalCode;
                   
                    db.SaveChanges();
                }

            }


            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
            }

        }

        
            public int InsertServiceList(CustomerService cs)
            {

            try
            {
                db.CustomerServices.Add(cs);
                db.SaveChanges();

                return cs.ID;

            }

            catch (Exception ex)
            {
                return 0;
                // WebLog.Log(ex.Message.ToString());
            }

        }

        public int CreateUser(User users)
        {

            try
            {
                db.Users.Add(users);
                db.SaveChanges();

                return users.id;
               
            }

            catch (Exception ex)
            {
                return 0;
                // WebLog.Log(ex.Message.ToString());
            }

        }

        
        //    public int InsertPatnerDetails(PartnersRefDetail partner)
        //{
        //    try
        //    {
        //        db.PartnersRefDetails.Add(partner);
        //        db.SaveChanges();

        //        return partner.ID;
        //    }
        //    catch(Exception ex)
        //    {
        //        return 0;
        //    }
        //}
        public int CreateRole(Role roles)
        {

            try
            {
                db.Roles.Add(roles);
                db.SaveChanges();

                return roles.RoleId;

            }

            catch (Exception ex)
            {
                return 0;
                // WebLog.Log(ex.Message.ToString());
            }

        }


        public int CreatePages(Pag pages)
        {

            try
            {
                db.Pags.Add(pages);
                db.SaveChanges();

                return pages.id;

            }

            catch (Exception ex)
            {
                return 0;
                // WebLog.Log(ex.Message.ToString());
            }

        }

        public void InsertPageRoles(PageAuthentication pagerole)
        {

            try
            {
                db.PageAuthentications.Add(pagerole);
                db.SaveChanges();


            }

            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
            }

        }


        public void InitiateTransacLog(TransactionLog _tL)
        {

            try
            {
                db.TransactionLogs.Add(_tL);
                db.SaveChanges();


            }

            catch (Exception ex)
            {

                //WebLog.Log(ex.Message.ToString());
            }

        }

        public void InitiatePaymentLog(PaymentLog _pL)
        {

            try
            {
                db.PaymentLogs.Add(_pL);
                db.SaveChanges();


            }

            catch (Exception ex)
            {

                 //WebLog.Log(ex.Message.ToString());
            }

        }
        public void InitiateCustomerTransaction(CustomerTransaction _ct)
        {

            try
            {
                db.CustomerTransactions.Add(_ct);
                db.SaveChanges();


            }

            catch (Exception ex)
            {

                // WebLog.Log(ex.Message.ToString());
            }

        }
        public void ArchieveCustomerServices(int id)
        {
            try
            {
                var original = db.CustomerServices.Find(id);

                if (original != null)
                {
                   
                    original.isVissible = 0;
                    db.SaveChanges();
                }

            }


            catch (Exception ex)
            {

               
            }

        }

        public string UpdateTransacLog(TransactionLog _tL)
        {

            try
            {
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.TrxToken = _tL.TrxToken;
                    original.PatnerRefNumber = _tL.ReferenceNumber;
                  
                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string UpdateCustomerTransacRecord(dynamic Resp, TransactionLog _tL)
        {

            try
            {
                CustomerTransaction ct = new CustomerTransaction();
                ct.CusPay2response = Convert.ToString(Resp);
                ct.Amount = Resp?.amount;
                ct.CustomerID = Resp?.accountNumber;
                ct.CustomerName = Resp?.customerName;
                ct.TrnDate = _tL.TrnDate;
                ct.Merchant_FK = _tL.Merchant_FK;
                ct.ReferenceNumber = _tL.ReferenceNumber;
                db.CustomerTransactions.Add(ct);
                db.SaveChanges();
                return ct.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UpdateTransacRecord(dynamic Resp, TransactionRecord _tL)
        {

            try
            {
                var original = db.TransactionRecords.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.TransactionStatus_FK = Resp?.respCode;
                    original.ServiceValueDetails1 = Resp?.value;
                    original.ServiceValueDetails2 = Resp?.units;
                    original.ServiceValueDetails3 = Resp?.unitsType;
                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string UpdateTransacLogs(dynamic Resp,TransactionLog _tL)
        {

            try
            {
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                    original.ServiceValueDetails1 = Resp?.units;
                    original.ServiceValueDetails2 = Resp?.value;
                    original.ServiceValueDetails3 = Resp?.customerAdddress;
                    original.PatnerRefNumber = _tL.ReferenceNumber;

                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdateTransac(TransactionLog _tL)
        {

            try
            {
                var original = db.TransactionLogs.Find(_tL.ReferenceNumber);

                if (original != null)
                {
                   
                    original.PatnerRefNumber = _tL.PatnerRefNumber;

                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /* public string UpdatePaymentLog(PaymentLog _pL)
         {
             string Trxtoken = _pL.TrxToken;
             string ResponseCode = _pL.ResponseCode;
             string RespDescrip = _pL.ResponseDescription;
             try
             {
                 var original = db.PaymentLogs.Find(_pL.TrxToken);
                 var nullval = original;
                 db.PaymentLogs.Add(_pL);
                 db.PaymentLogs.Attach(_pL);
                 db.Entry(_pL).State = System.Data.Entity.EntityState.Modified;
                 db.SaveChanges();
                 return _pL.ID.ToString();

            }
             catch (Exception ex)
             {
                 return null;
             }
         }*/
        public string UpdatePaymentLog(PaymentLog _pL)
        {
            string Trxtoken = _pL.TrxToken;
            string ResponseCode = _pL.ResponseCode;
            string RespDescrip = _pL.ResponseDescription;
            try
            {
                var original = db.PaymentLogs.Find(_pL.ReferenceNumber);
               
                if (original != null)
                {
                    original.TrxToken = _pL.TrxToken;
                    original.TrnDate = _pL.TrnDate;
                    original.ResponseCode = _pL.ResponseCode;
                    original.ResponseDescription = _pL.ResponseDescription;

                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdatePaymentLogs(PaymentLog _pL)
        {
            string Trxtoken = _pL.TrxToken;
            string ResponseCode = _pL.ResponseCode;
            string RespDescrip = _pL.ResponseDescription;
            try
            {
                var original = db.PaymentLogs.Find(_pL.ReferenceNumber);

                if (original != null)
                {
                   // original.TrxToken = _pL.TrxToken;
                    original.TrnDate = _pL.TrnDate;
                    original.ResponseCode = _pL.ResponseCode;
                    original.ResponseDescription = _pL.ResponseDescription;

                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string UpdateCustomerTransac(CustomerTransaction _ct)
        {
            try
            {
                var original = db.CustomerTransactions.Find(_ct.ReferenceNumber);
               
                if (original != null)
                {
                    original.CusPay2response = _ct.CusPay2response;
                  
                  
                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public string UpdateProfile(User users)
        {
            try
            {
                var original = db.Users.Find(users.id);

                if (original != null)
                {
                    original.firstname = users.firstname;
                    original.lastname = users.lastname;
                    original.Email = users.Email;
                    original.Phone = users.Phone;
                    original.pasword = users.pasword;
                    original.confirmPassword = users.confirmPassword;
                    
                    db.SaveChanges();
                }
                return original.id.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdateProfiles(User users)
        {
            try
            {
                var original = db.Users.Find(users.id);

                if (original != null)
                {
                    original.firstname = users.firstname;
                    original.lastname = users.lastname;
                    original.Email = users.Email;
                    original.Phone = users.Phone;
                   

                    db.SaveChanges();
                }
                return original.id.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdatePages(Pag pages)
        {
            try
            {
                var original = db.Pags.Find(pages.id);

                if (original != null)
                {
                    original.PageName = pages.PageName;
                    original.PageUrl = pages.PageUrl;
                   

                    db.SaveChanges();
                }
                return original.id.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }



        public string UpdateRole(Role roles)
        {
            try
            {
                var original = db.Roles.Find(roles.RoleId);

                if (original != null)
                {
                    original.RoleName = roles.RoleName;
                   


                    db.SaveChanges();
                }
                return original.RoleId.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public string UpdateCustomerService(CustomerService _cs)
        {
            try
            {
                var original = db.CustomerServices.Find(_cs.ID);

                if (original != null)
                {
                  original.CustomerIDLabel = _cs.CustomerIDLabel;
                    original.CustomerID = _cs.CustomerID;
                    original.OtherLabel = _cs.OtherLabel;
                    original.OtherLabelID = _cs.OtherLabelID;
                    original.PackageType = _cs.PackageType;
                    original.PackageTypeIDLabel = _cs.PackageTypeIDLabel;
                    db.SaveChanges();
                }
                return original.ID.ToString();

            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}

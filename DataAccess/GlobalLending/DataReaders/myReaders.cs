using DataAccess;
//using GlobalLending.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GlobalLending.DataReader
{
    public class myReaders
    {
        // private GlobalTransactEntities globalTransaction = new GlobalTransactEntities();
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        Loan loan = new Loan();
        LoanBank loanbank = new LoanBank();
        LoanSocial loansocial = new LoanSocial();
        LoanEmployeeInfo loanempInfo = new LoanEmployeeInfo();
       
        public List<Loan> selectAllLoan()
        {

            try
            {
                var Loan = (from r in globalTransaction.Loans select r).ToList();

                if (Loan == null)
                {
                    return null;
                }

                return Loan;
            }
            catch (Exception ex)
            {

               // WebLog.Log(ex.Message.ToString());
                return null;
            }



        }
    }
}
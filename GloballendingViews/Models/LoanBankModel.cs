using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GloballendingViews.Models
{
    public class LoanBankModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Loan_Fk")]
        public int Loan_Fk { get; set; }

        [EnumDataType(typeof(BankName))]
        public BankName BankName { get; set; }

        [Display(Name = "Account Name")]
        public string AccountName { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "BVN")]
        public string Bvn { get; set; }
       
    }

    public enum BankName
 {
        Smile = 0,
        GTB = 1,
        FIRSTBANK = 2,
        STERLINGBANK = 3
 }
}
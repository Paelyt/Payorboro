using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GloballendingViews.Models
{
    public class LoanModel
    {
        public LoanModel()
        {
            this.LoanBanks = new HashSet<LoanBank>();
            this.LoanBanks1 = new HashSet<LoanBank>();
            this.LoanEmployeeInfoes = new HashSet<LoanEmployeeInfo>();
            this.LoanSocials = new HashSet<LoanSocial>();
        }
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Customer_Fk")]
        public int Customer_Fk { get; set; }

        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }

        [Display(Name = "Tenor")]
        public int Tenor { get; set; }

        [Display(Name = "Interest Rate")]
        public int InterestRate { get; set; }

        [Display(Name = "Monthly Salary")]
        public decimal MonthlySalary { get; set; }

        [Display(Name = "Primary PhoneNumber")]
        public string PrimaryPhoneNumber { get; set; }

        [Display(Name = "Secondary PhoneNumber")]
        public string SecondaryPhoneNumber { get; set; }

        [Display(Name = "Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MMM/yyyy}")]
        public string DateOfBirth { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }

        [Display(Name = "Primary EmailAddress")]
        public string PrimaryEmailAddress { get; set; }

        [Display(Name = "Secondary EmailAddress")]
        public string SecondaryEmailAddress { get; set; }

        [Display(Name = "Other Income")]
        public decimal OtherIncome { get; set; }
        public ICollection<LoanBank> LoanBanks { get; private set; }
        public ICollection<LoanBank> LoanBanks1 { get; private set; }
        public ICollection<LoanEmployeeInfo> LoanEmployeeInfoes { get; private set; }
        public ICollection<LoanSocial> LoanSocials { get; private set; }
    }
    public partial class LoanBank
    {
        public int ID { get; set; }
        public int Loan_Fk { get; set; }
        public string BankName { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string Bvn { get; set; }

        public virtual LoanModel Loan { get; set; }
        public virtual LoanModel Loan1 { get; set; }
    }

    public partial class LoanEmployeeInfo
    {
        public int ID { get; set; }
        public int Loan_Fk { get; set; }
        public string EmployeeName { get; set; }
        public string OfficeAddress { get; set; }
        public string EmploymentStatus { get; set; }
        public string EmploymentDuration { get; set; }

        public virtual LoanModel Loan { get; set; }
    }

    public partial class LoanSocial
    {
        public int ID { get; set; }
        public int Loan_Fk { get; set; }
        public string Social { get; set; }

        public virtual LoanModel Loan { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GloballendingViews.Models
{
    public class LoanEmployeeModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "Loan FK")]
        public int Loan_Fk { get; set; }

        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }

        [Display(Name = "Office Address")]
        public string OfficeAddress { get; set; }

        [Display(Name = "Employment Status")]
        public string EmploymentStatus { get; set; }

        [Display(Name = "Employment Duration")]
        public string EmploymentDuration { get; set; }

    }
}
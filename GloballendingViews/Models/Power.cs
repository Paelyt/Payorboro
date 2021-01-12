using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GloballendingViews.Models
{
    public class  Power
    {
        [Display(Name = "CustomerID")]
        public string CustomerID { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "CustomerName")]
        public string CustomerName { get; set; }

        [Display(Name = "CustomerAddress")]
        public string CustomerAddress { get; set; }
        
             [Display(Name = "BusinessDistrict")]
        public string BusinessDistrict { get; set; }

        [Display(Name = "BusniessDistrict")]
        public string BusniessDistrict { get; set; }

        [Display(Name = "AccountNumber")]
        public string AccountNumber { get; set; }

        [Display(Name = "MinimumAmount")]
        public string MinimumAmount { get; set; }

        [Display(Name = "Phone")]
        public string Phone { get; set; }

        [Display(Name = "Othername")]
        public string Othername { get; set; }

        public string Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseDesrciption { get; set; }

        public string ThirdPartyCode { get; set; }

        public string CustReference { get; set; }

        public string PaymentType { get; set; }

        public string Receiver { get; set; }

        public string OutStandingAmount { get; set; }

        public string AccountType { get; set; }

        public string AgentID { get; set; }

        public string AgentKey { get; set; }

        public string Email { get; set; }

        public string Merchant_fk { get; set; }

        public string Amount { get; set; }

        public string ConvFee { get; set; }

        public string TransactionType { get; set; }

        public Double Borovalue { get; set; }

        // Added This 10-june-2019
        public string paymethod { get; set; }

    }
}
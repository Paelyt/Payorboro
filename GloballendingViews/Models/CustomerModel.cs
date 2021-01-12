using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace GloballendingViews.Models
{
    public class CustomerModel
    {
        [Display(Name = "ID")]
        public int ID { get; set; }

        [Display(Name = "FirstName")]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        public string LastName { get; set; }

        [Display(Name = "Contact Address")]
        public string ContactAddress { get; set; }

        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Value Date")]
        public string ValueDate { get; set; }

        [Display(Name = "Value Time")]
        public string ValueTime { get; set; }

        [Display(Name = "Amount borrowed")]
        public double Amountborrowed { get; set; }

        [Display(Name = "Amount Paid")]
        public double AmountPaid { get; set; }

        [Display(Name = "Outstanding Amount")]
        public double OutstandingAmount { get; set; }

        [Display(Name = "IsVisible")]
        public string IsVisible { get; set; }

        [Display(Name = "User_FK")]
        public int User_FK { get; set; }
    }

    public class CustomerEligibility
    {
        [Display(Name = "CustomerId")]
        public string CustomerId { get; set; }

        [Display(Name = "MerchantFK")]
        public int MerchantFk { get; set; }
    }

    public class Merchant
    {
        [Display(Name = "Id")]
        public int ID { get; set; }
        [Display(Name = "MarchantName")]
        public string MarchantName { get; set; }
        [Display(Name = "MarchantCode")]
        public string MarchantCode { get; set; }
        [Display(Name = "ContactName")]
        public string ContactName { get; set; }
        [Display(Name = "ContactEmail")]
        public string ContactEmail { get; set; }
        [Display(Name = "ContactPhoneNumber")]
        public string ContactPhoneNumber { get; set; }
        [Display(Name = "ServiceCharge")]
        public double ServiceCharge { get; set; }
        [Display(Name = "MerchantServiceType_FK")]
        public int MerchantServiceType_FK { get; set; }
        [Display(Name = "IsVisible")]
        public int IsVisible { get; set; }
    }


    public class CustomerServices
    {
        [Display(Name = "Id")]
        public int ID { get; set; }

        [Display(Name = "Customer_FK")]
        public int Customer_FK { get; set; }

        [Display(Name = "CustomerIDLabel")]
        public string CustomerIDLabel { get; set; }

        [Display(Name = "CustomerID")]
        public string CustomerID { get; set; }

        [Display(Name = "PackageTypeIDLabel")]
        public string PackageTypeIDLabel { get; set; }

        [Display(Name = "PackageType")]
        public string PackageType { get; set; }

        [Display(Name = "OtherLabelID")]
        public string OtherLabelID { get; set; }

        [Display(Name = "OtherLabel")]
        public string OtherLabel { get; set; }

        [Display(Name = "DateCreated")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "ValueDate")]
        public string ValueDate { get; set; }

        [Display(Name = "ValueTime")]
        public string ValueTime { get; set; }

        [Display(Name = "Merchant_Fk")]
        public int Merchant_Fk { get; set; }

        [Display(Name = "merchant")]
        public Merchant merchant { get; set; }
        [Display(Name = "isVissible")]
        public int isVissible { get; set; }
    }
}
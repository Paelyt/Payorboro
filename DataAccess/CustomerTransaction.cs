//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataAccess
{
    using System;
    using System.Collections.Generic;
    
    public partial class CustomerTransaction
    {
        public int ID { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public Nullable<int> Customer_FK { get; set; }
        public Nullable<double> Amount { get; set; }
        public Nullable<double> ServiceCharge { get; set; }
        public Nullable<System.DateTime> TrnDate { get; set; }
        public Nullable<int> TransactionType { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public string ServiceDetails { get; set; }
        public Nullable<int> Merchant_FK { get; set; }
        public string ReferenceNumber { get; set; }
        public string CusPay2response { get; set; }
    }
}

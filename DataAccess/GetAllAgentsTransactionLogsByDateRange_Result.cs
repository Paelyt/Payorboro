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
    
    public partial class GetAllAgentsTransactionLogsByDateRange_Result
    {
        public int ID { get; set; }
        public string CustomerId { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public string MarchantName { get; set; }
        public string ValueDate { get; set; }
        public Nullable<System.DateTime> TrnDate { get; set; }
        public string PatnerRefNumber { get; set; }
        public string CustomerPhone { get; set; }
        public string MerchantName { get; set; }
        public string BillerName { get; set; }
        public Nullable<int> Merchant_FK { get; set; }
        public Nullable<int> IsPaid { get; set; }
        public string TransactionType { get; set; }
        public Nullable<double> Amount { get; set; }
        public string TransactionStatus { get; set; }
        public string Channel { get; set; }
        public Nullable<double> TransactionCharge { get; set; }
        public Nullable<double> AmountDue { get; set; }
        public string ReferenceNumber { get; set; }
        public string AgengtRefNumber { get; set; }
        public string TrxToken { get; set; }
        public string ServiceValueDetails1 { get; set; }
        public string ServiceValueDetails2 { get; set; }
        public string ServiceValueDetails3 { get; set; }
        public string ServiceDetails { get; set; }
    }
}

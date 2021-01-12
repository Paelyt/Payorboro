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
    
    public partial class TransactionLog
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
        public Nullable<int> IsPaid { get; set; }
        public Nullable<int> Merchant_FK { get; set; }
        public string MerchantName { get; set; }
        public string ReferenceNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public string TrxToken { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public string Voucher { get; set; }
        public string Pin { get; set; }
        public string ServiceCode { get; set; }
        public string PatnerRefNumber { get; set; }
        public string PatnerResponseCode { get; set; }
        public string AccountNumber { get; set; }
        public string AssignedTranxId { get; set; }
        public Nullable<double> CommissionEarned { get; set; }
        public Nullable<double> ConvenienceFee { get; set; }
        public string CreatedBy { get; set; }
        public string CustomerIDAddress { get; set; }
        public string CustomerIDLabel { get; set; }
        public string ResponseBody { get; set; }
        public string ResponseDescription { get; set; }
        public Nullable<int> TransactionStatus_FK { get; set; }
        public string ServiceValueDetails1 { get; set; }
        public string ServiceValueDetails2 { get; set; }
        public string ServiceValueDetails3 { get; set; }
        public string ThirdPartyCode { get; set; }
        public Nullable<int> IsVisible { get; set; }
        public Nullable<System.DateTime> DateModified { get; set; }
    }
}

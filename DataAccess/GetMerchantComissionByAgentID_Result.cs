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
    
    public partial class GetMerchantComissionByAgentID_Result
    {
        public int ID { get; set; }
        public int Agent_UserFK { get; set; }
        public Nullable<int> Merchant_FK { get; set; }
        public int VendingComission { get; set; }
        public int LendingComission { get; set; }
        public int IsVisible { get; set; }
        public string Audit { get; set; }
        public string trnDate { get; set; }
        public string ValueDate { get; set; }
        public string MerchantName { get; set; }
        public string MerchantCode { get; set; }
    }
}

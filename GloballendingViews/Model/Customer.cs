//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GloballendingViews.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class Customer
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactAddress { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public string ValueDate { get; set; }
        public string ValueTime { get; set; }
        public Nullable<double> Amountborrowed { get; set; }
        public Nullable<double> AmountPaid { get; set; }
        public Nullable<double> OutstandingAmount { get; set; }
        public string IsVisible { get; set; }
        public int User_FK { get; set; }
    }
}
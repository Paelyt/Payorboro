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
    
    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            this.UserRoles = new HashSet<UserRole>();
        }
    
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string Phone { get; set; }
        public string pasword { get; set; }
        public string confirmPassword { get; set; }
        public string Email { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string ResetPassword { get; set; }
        public Nullable<System.DateTime> DateTim { get; set; }
        public Nullable<int> isVissibles { get; set; }
        public string Referal { get; set; }
        public string MyReferalCode { get; set; }
        public Nullable<int> ReferralLevel { get; set; }
        public Nullable<long> BankFk { get; set; }
    
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual Bank Bank { get; set; }
    }
}

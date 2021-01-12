﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GlobalTransactEntities1 : DbContext
    {
        public GlobalTransactEntities1()
            : base("name=GlobalTransactEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<CustomerService> CustomerServices { get; set; }
        public virtual DbSet<Loan> Loans { get; set; }
        public virtual DbSet<LoanBank> LoanBanks { get; set; }
        public virtual DbSet<LoanEmployeeInfo> LoanEmployeeInfoes { get; set; }
        public virtual DbSet<LoanSocial> LoanSocials { get; set; }
        public virtual DbSet<Merchant> Merchants { get; set; }
        public virtual DbSet<Pag> Pags { get; set; }
        public virtual DbSet<PageAuthentication> PageAuthentications { get; set; }
        public virtual DbSet<pageHeader> pageHeaders { get; set; }
        public virtual DbSet<PaymentLog> PaymentLogs { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<TransactionLog> TransactionLogs { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<CustomerLedger> CustomerLedgers { get; set; }
        public virtual DbSet<CustomerTransaction> CustomerTransactions { get; set; }
        public virtual DbSet<MerchantServiceType> MerchantServiceTypes { get; set; }
    }
}

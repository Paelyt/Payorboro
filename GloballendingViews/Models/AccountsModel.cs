using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAccess;

namespace GloballendingViews.Models
{
    public class AccountsModel
    {
       
        public int id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string Phone { get; set; }
        public string pasword { get; set; }
        public string confirmPassword { get; set; }
        public string Email { get; set; }
        public DateTime Date { get; set; }
        public string ResetPassword { get; set; }
        public DateTime DateTim { get; set; }
        public int isVissibles { get; set; }

        public string Referal { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
       
        //public virtual ICollection<UserRole> UserRoles { get; set; }

    }

    public class Role
    {       
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int isVissible { get; set; }
        public DateTime Date { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }


    }

    public class Page
    {

        public int id { get; set; }
        public string PageName { get; set; }
        public string PageUrl { get; set; }
        public int RoleId { get; set; }
        public int isvisible { get; set; }
        public int pageHeader { get; set; }

        public virtual ICollection<Pag> Pag1 { get; set; }
        public virtual Pag Pag2 { get; set; }
    }

    public partial class UserRole
    {
        public int id { get; set; }
        public int UserId { get; set; }
        public bool RoleId { get; set; }
        public  int IsVissible { get; set; }
        public DateTime dates { get; set; }

        //public virtual Role Role { get; set; }
        //public virtual User User { get; set; }
    }

    public partial class PageRole
    {
        public int id { get; set; }
        public string PageName { get; set; }
        public int RoleId { get; set; }
        public string isVissible { get; set; }

        public virtual Role Role { get; set; }
    }

    public class GetAssignRoles
    {
        public int userid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }
        public string email { get; set; }

        
    }

    public class UnGetAssignRoles
    {
       
        public int Roleid { get; set; }
        public string Rolename { get; set; }
        


    }

    public class GetAssignPages
    {
        public int pageid { get; set; }
        public string Roleid { get; set; }
        public string Rolename { get; set; }
     


    }

    public class getAllUserAndRoles
    {
       public int userid { get; set; }
       public int roleid { get; set; }
       public string rolename { get; set; }
       public string email { get; set; }
       public int id { get; set; }
    }


    public class getAllPagesAndRoles
    {
       // public int pageid { get; set; }
        public int roleid { get; set; }
        public string rolename { get; set; }
        public string pageName { get; set; }
        public int id { get; set; }


        
                                  
                                 
                                 
    }


}
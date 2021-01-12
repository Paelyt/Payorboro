using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using DataAccess;

namespace GloballendingViews.Models
{
    public class RoleModel
    {
        [Display(Name = "RoleId")]
        public int RoleId { get; set; }

        [Display(Name = "RoleName")]
        public string RoleName { get; set; }

        [Display(Name = "isVissible")]
        public int isVissible { get; set; }

        [Display(Name = "Date")]
        public DateTime Date { get; set; }
       // public virtual ICollection<UserRole> UserRoles
       //{
       //     get;
       //     set;
       // }


    }
}
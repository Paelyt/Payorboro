using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GloballendingViews.Models;
namespace GloballendingViews.ViewModels
{
    public class LoanViewModel
    {
        public LoanModel LoanModel
        {
            get;
            set;
        }

        public LoanBankModel LoanBankModel
        {
            get;
            set;
        }

        public LoanEmployeeModel LoanEmployeeModel
        {
            get;
            set;
        }

        public LoanSocialModel LoanSocialModel
        {
            get;
            set;

        }

        public AccountsModel AccountsModel
        {
            get;
            set;
        }

        public RoleModel RoleModel
        {
            get;
            set;
        }

        public Page PageModel
        {
            get;
            set;
        }

        public UserRole UserRole
        {
            get;
            set;

        }

        public PageRole PageRole
        {
            get;
            set;
        }

        public CustomerEligibility CustomerEligility
        {
            get;
            set;
        }

        public CustomerModel CustomerModel
        {
            get;
            set;
        }

        public CustomerServices CustomerServices
        {
            get;
            set;
        }

        public Merchant merchant
        {
            get;
            set;

        }
        public IEnumerable<GetAssignRoles> GetAssignRoless
        {
            get;
            set;
        }
        public IEnumerable<UnGetAssignRoles> UnGetAssignRoless
        {
            get;
            set;
        }

         public IEnumerable<Classes.Paytv.Report> Report
        {
            get;
            set;
        }
        public IEnumerable<GetAssignPages> GetAssignPagess
        {
            get;
            set;
        }

        public IEnumerable<getAllUserAndRoles> getAllUserAndRoless
        {
            get;
            set;
        }

        public IEnumerable<getAllPagesAndRoles> getAllPagesAndRoless
        {
            get;
            set;
        }

        public AgentModel AgentModel
        {
            get;
            set;
        }

        public Power powerModel
           {
            get;
            set;

        }

        public Classes.Internetserviceprovider.InternetServiceObj InternetServiceModel
        {
            get;
            set;
        }
        //public IEnumerable<Classes.Paytv.Menus> Menus
        //{
        //    get;
        //    set;
        //}
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using GloballendingViews.Models;
using GloballendingViews.ViewModels;
//using GloballendingViews.Model;
using DataAccess;
using GloballendingViews.HelperClasses;

namespace GloballendingViews.Controllers
{
    public class LoanController : Controller
    {
        // GlobalTransactEntities1 gte = new GlobalTransactEntities1();
        //Loan ln = new Loan();
        GlobalTransactEntitiesData gte = new GlobalTransactEntitiesData();
        DataAccess.Loan lo = new DataAccess.Loan();
        DataAccess.DataReaders DataReaders = new DataAccess.DataReaders();
        User users = new DataAccess.User();

        Loan ln = new DataAccess.Loan();
        // GET: Loan
        public ActionResult Index()
        {
          
            GLobalClient gc = new GLobalClient();
            ViewBag.listLoan = gc.findAll();
            return View();
        }

        public ActionResult Delete(int Id)
        {

            GLobalClient cc = new GLobalClient();
            cc.Delete(Id);
            return RedirectToAction("Index");


        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {

            GLobalClient cc = new GLobalClient();
            LoanViewModel cvm = new LoanViewModel();
            cvm.LoanModel = cc.find(Id);
            return View("Edit", cvm);


        }

        [HttpPost]
        public ActionResult Edit(LoanViewModel cvm)
        {
            GLobalClient gc = new GLobalClient();
            gc.Edit(cvm.LoanModel);
            return RedirectToAction("Index");


        }
        [HttpGet]
        public ActionResult Apply(LoanViewModel lvm)
        {
            
            return View();
            
        }

        public int LoanID()
        {
            gte.Loans.Single();

            return 0;
        }

        public bool ValidatePassword(string Password,string ConfirmPass)
        {
            try
            {
                string value = Password;
                string value1 = ConfirmPass;
                if (value == value1)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [HttpPost]
        public ActionResult Create(LoanViewModel lvm)
        {
            GLobalClient cc = new GLobalClient();
           
            if(lvm != null)
           {
            lo.ContactAddress = lvm.LoanModel.ContactAddress;
                /*gte.Loans.Add(lo);
                gte.SaveChanges();
                int id = lo.ID;*/
                 cc.Create(lvm.LoanModel);
                 lvm.LoanModel.ID = ln.ID;
                 lvm.LoanModel.ID = 1;
                 lvm.LoanBankModel.Loan_Fk = lvm.LoanModel.ID;
                 lvm.LoanEmployeeModel.Loan_Fk = lvm.LoanModel.ID;
                 lvm.LoanSocialModel.Loan_Fk = lvm.LoanModel.ID;
                 cc.CreateBank(lvm.LoanBankModel);
                 cc.CreateLoanEmployee(lvm.LoanEmployeeModel);
                 cc.CreateSocail(lvm.LoanSocialModel);
                using (GlobalTransactEntitiesData g = new GlobalTransactEntitiesData())
                {
                   List<DataAccess.Loan> Loan  = g.Loans.ToList();
                }
                TempData["Message"] = "Loan Application Succesful";
          //   return RedirectToAction("Apply");
           }
            else  
           {
            TempData["Message"] = "loan Application Failed";
           }
             return View("Apply", lvm);
           }


        [HttpPost]
        public ActionResult CreateUser(LoanViewModel lvm)
        {
            GLobalClient cc = new GLobalClient();

            if (lvm != null)
            {
                string value = lvm.AccountsModel.Email;
                string password = lvm.AccountsModel.pasword;
                string confirmpass = lvm.AccountsModel.confirmPassword;
                bool validatePass = ValidatePassword(password, confirmpass);
                var EncrypPassword = new CryptographyManager().ComputeHash(password, HashName.SHA256);
                password = EncrypPassword;
                lvm.AccountsModel.confirmPassword = password;
                lvm.AccountsModel.pasword = password;
                lvm.AccountsModel.Email = value;
               
                if (validatePass == false)
                {
                    TempData["Message"] = "Password And Confirm Password Must Match";
                }
                else if (validatePass == true)
                {
                    bool val = DataReaders.Validate(value);
                    if (val == true)
                    {
                        TempData["Message"] = "User Already Exist";
                        return RedirectToAction("Signup", "index", new { area = "" });
                    }
                    else if (val == false)
                    {
                        lvm.AccountsModel.Date = DateTime.Now;
                        lvm.AccountsModel.DateTim = DateTime.Today;
                        cc.CreateUser(lvm.AccountsModel);
                        TempData["Message"] = "User Created Succesfully";
                        return RedirectToAction("Signup", "index", new { area = "" });
                    }

                }
            }
            else
            {
                TempData["Message"] = "Error Creating User";
            }
            return RedirectToAction("Signup", "index", new { area = "" });
        }


        [HttpPost]
        public ActionResult CreateRole(LoanViewModel lvm)
        {
            GLobalClient cc = new GLobalClient();

            if (lvm != null)
            {
                string value = lvm.RoleModel.RoleName;
                bool val = DataReaders.ValidateRole(value);
                if (val == true)
                {
                TempData["Message"] = "Role Already Exist";
         return RedirectToAction("Roles", "index", new { area = "" });
                }
                else if (val == false)
               {
               lvm.RoleModel.Date = DateTime.Now;
               cc.CreateRole(lvm.RoleModel);
               TempData["Message"] = "Role Created Succesfully";
               return RedirectToAction("Roles", "index", new { area = ""
               });
                }

                }
            
            else
            {
                TempData["Message"] = "Error Creating Role";
            }
            return RedirectToAction("Roles", "index", new { area = "" });
        }



        [HttpPost]
        public ActionResult CreatePage(LoanViewModel lvm)
        {
            GLobalClient cc = new GLobalClient();

            if (lvm != null)
            {
                string value = lvm.PageModel.PageName;
                bool val = DataReaders.ValidatePage(value);
                if (val == true)
                {
                    TempData["Message"] = "Page Already Exist";
                    return RedirectToAction("Page", "index", new { area = "" });
                }
                else if (val == false)
                {
                 lvm.PageModel.isvisible = 1;
                    lvm.PageModel.pageHeader = 1;
                 cc.CreatePage(lvm.PageModel);
                 TempData["Message"] = "Page Created Succesfully";
                return RedirectToAction("Page", "index", new
                    {
                        area = ""
                    });
                }

            }

            else
            {
                TempData["Message"] = "Error Creating Page";
            }
            return RedirectToAction("Page", "index", new { area = "" });
        }
    }
}
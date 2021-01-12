using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using GloballendingViews.Models;
using GloballendingViews.ViewModels;
using DataAccess;
using PagedList.Mvc;
using PagedList;
namespace GloballendingViews.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        DataAccess.DataReaders DataReaders = new DataAccess.DataReaders();
        DataAccess.DataCreator DataCreators = new DataAccess.DataCreator();
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        GLobalClient gc = new GLobalClient();
        Customer customer = new Customer();
        CustomerServices _cs = new CustomerServices();
        string Email = "tolutl@yahoo.com";
        LogginHelper LoggedInuser = new LogginHelper();

        public ActionResult Index()
      {
            try
            {
                CustomerClient cc = new CustomerClient();
                ViewBag.listCustomer = cc.findAll();
                return View();
            }
            catch(Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }
      }
        [HttpGet]
        public ActionResult CheckEligibility()
        {
            try
            {
                ViewBag.listMerchant = gc.findAllMerchant();
                return View();
            }
            catch (Exception ex)
            {
                
                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }

        [HttpPost]
        public ActionResult Eligibility(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                string strDDLValue = form["ddlMerchantName"].ToString();
                CustomerEligibility _CE = new CustomerEligibility();
                _CE.CustomerId = lvm.CustomerEligility.CustomerId;
                lvm.CustomerEligility.MerchantFk = Convert.ToInt16(strDDLValue);
                _CE.MerchantFk = lvm.CustomerEligility.MerchantFk;
                var merchantFK = Convert.ToInt16(_CE.MerchantFk);
                var CustomerEligibility = DataReaders.CheckCustomerEligiblity(_CE.CustomerId, merchantFK);
                ViewBag.Message = CustomerEligibility;
                
                TempData["Message"] = CustomerEligibility; 
                
                return RedirectToAction("CheckEligibility");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }


        }
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                CustomerClient cc = new CustomerClient();
                ViewBag.listCustomer = cc.findAll();
                return View("Create");
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        [HttpPost]
        public ActionResult Create(CustomerViewModel cvm)
        {
            try
            {
                string user = LoggedInuser.LoggedInUser();
                cvm.CustomerModel.DateCreated = DateTime.Now;
                customer.DateCreated = DateTime.Now;
                customer.IsVisible = "1";
                customer.OutstandingAmount = 0;
                customer.ValueDate = DateTime.Now.Date.ToString();
                customer.ValueTime = DateTime.Now.TimeOfDay.ToString();
                cvm.CustomerModel.DateCreated = DateTime.Now;
                cvm.CustomerModel.IsVisible = "1";
                cvm.CustomerModel.OutstandingAmount = 0;
                cvm.CustomerModel.ValueDate = DateTime.Now.Date.ToString();
                cvm.CustomerModel.ValueTime = DateTime.Now.TimeOfDay.ToString();
                cvm.CustomerModel.User_FK = DataReaders.GetUserIdByEmail(Email);
                // cvm.CustomerModel.User_FK = 1;
                if (cvm.CustomerModel.ContactEmail != null)
                {
                    var customerRecord = DataReaders.ValidateCustomer(cvm.CustomerModel.ContactEmail);
                    if (customerRecord == false)
                    {

                        gc.CreateCustomer(cvm.CustomerModel);
                        TempData["Message"] = "Customer Added Succesfully";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Message"] = "Error Please Try Again! ";
                        return View("create");
                    }
                }
                return View("create");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult CreateService()
        {
            try {
                GLobalClient gc = new GLobalClient();
                ViewBag.listMerchant = gc.findAllMerchant();
                return View("CreateService");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        [HttpPost]
        public ActionResult CreateService(LoanViewModel lvm,FormCollection form)
        {
            try
            {
                var LoggedInuser = new LogginHelper();
                string user = LoggedInuser.LoggedInUser();
                if (user != "")
                {
                    var userid = DataReaders.GetUserIdByEmail(user);
           GLobalClient gc = new GLobalClient();
           ViewBag.listMerchant = gc.findAllMerchant();
           lvm.CustomerServices.Customer_FK = userid;
           lvm.CustomerServices.DateCreated = DateTime.Now;
           lvm.CustomerServices.ValueDate = DateTime.Now.ToString();
           lvm.CustomerServices.Merchant_Fk = Convert.ToInt16(form["ID"]);
           lvm.CustomerServices.ValueTime = "1";
           lvm.CustomerServices.isVissible = 1;
           lvm.CustomerServices.OtherLabel = "other";
            gc.CreateCustomerService(lvm.CustomerServices);
           TempData["Message"] = "New Service Added";
                    return View("CreateService");
                }
                else
                {
                    return View("CreateService");
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
                
            }
        }

        public ActionResult DeleteCustomerService(int Id, LoanViewModel lvm)
        {
            try {
                CustomerServices cs = new CustomerServices();
                GLobalClient gc = new GLobalClient();
                //This is for The Api
                //gc.DeleteCustomerService(_cs, Id);

                _cs.isVissible = 0;
                DataCreators.ArchieveCustomerServices(Id);
                return RedirectToAction("AllCustomerService");
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult Delete(int Id)
        {
            try {
                CustomerClient cc = new CustomerClient();
                cc.Delete(Id);
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult Edit(string Email)
        {

            try {
                LoanViewModel lvm = new LoanViewModel();
                lvm.CustomerModel = gc.FindCustomer(Email);
                return View("Edit", lvm);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpGet]
        public ActionResult AllCustomerService(string search,int? i)
        {
            try
            {
               // search = "12";
              //  int i = 0;
              
                string user = LoggedInuser.LoggedInUser();
                if (user != null)
                {
                   
        int userid = DataReaders.GetUserIdByEmail(user);
        GLobalClient gc = new GLobalClient();
                    //ViewBag.listCustomerService = gc.GetCustomerSevicesByUser(userid);
                    // List<CustomerService> listCustomer = (db.CustomerServices.Where(x => x.CustomerID.StartsWith(search) || search == null && x.Customer_FK == userid && x.isVissible==1).ToList());
                    List<CustomerService> listCustomer = (db.CustomerServices.ToList());
                    int pageSize = 2;
                    int pageNumber = (i ?? 1);
    ViewBag.listCustomerService = listCustomer;
  var onePageOfCustomers = listCustomer.ToPagedList(pageNumber, pageSize); 

           ViewBag.onePageOfCustomers = onePageOfCustomers;
                //    return View(listCustomer.ToPagedList(pageNumber,pageSize));
                    // return View("AllCustomerService");


                    var query = (db.CustomerServices.Where(x => x.CustomerID.StartsWith(search) || search == null && x.Customer_FK == userid && x.isVissible == 1).
             Select(t => new CustomerServices
             {

                 CustomerID = t.CustomerID,
                 CustomerIDLabel = t.CustomerIDLabel,
                 PackageTypeIDLabel = t.PackageTypeIDLabel,
                 PackageType = t.PackageType,
                 OtherLabelID = t.OtherLabelID


             }).OrderBy(t => t.CustomerID).ToPagedList(pageNumber, pageSize));


                    return View(query);
                }
               
                else
                {
                    return RedirectToAction("index", "index", new { area = "" });
                    
                }
            }
            catch(System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = string.Format("{0}:{1}", validationErrors.Entry.Entity.ToString(), validationError.ErrorMessage);
                        //raise a new exception inserting the current one as the InnerException
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                // WebLog.Log(ex.Message.ToString());
                 return null;
            }
        }

        [HttpPost]
        public ActionResult Edit(CustomerViewModel cvm)
        {

            try
            {
                if (cvm.CustomerModel.ContactEmail != null)
                {
    gc.Edit(cvm.CustomerModel);
    TempData["Message"] = "Customer Added Succesfully";
                return RedirectToAction("Index");

                }
                return View("create");
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }


        [HttpGet]
        public ActionResult EditCustomerService(int id)
        {
            try
            {
                GLobalClient gc = new GLobalClient();
                ViewBag.listMerchant = gc.findAllMerchant();
                LoanViewModel lvm = new LoanViewModel();
                lvm.CustomerServices = gc.FindCustomerService(id);
                return View("EditCustomerService", lvm);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        [HttpPost]
        public ActionResult EditCustomerService(LoanViewModel lvm)
        {
            try
            {
                GLobalClient gc = new GLobalClient();
                ViewBag.listMerchant = gc.findAllMerchant();
         //lvm.CustomerServices = gc.FindCustomerService(lvm);
                CustomerService _cs = new CustomerService();
                _cs.ID = lvm.CustomerServices.ID;
                _cs.CustomerID =lvm.CustomerServices.CustomerID ;
                _cs.CustomerIDLabel = lvm.CustomerServices.CustomerIDLabel;
                _cs.PackageTypeIDLabel = lvm.CustomerServices.PackageTypeIDLabel;
                _cs.PackageType = lvm.CustomerServices.PackageType;
                _cs.OtherLabel = lvm.CustomerServices.OtherLabel;
                _cs.OtherLabelID = lvm.CustomerServices.OtherLabelID;


                var UpdateCustomerService = DataCreators.UpdateCustomerService(_cs);
                // return View("AllCustomerService");
                return RedirectToAction("AllCustomerService", "Customer", new { area = "" });

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
using DataAccess;
using GloballendingViews.Classes;
using GloballendingViews.HelperClasses;
using GloballendingViews.Models;
using GloballendingViews.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.IO;
using System.Data;

namespace GloballendingViews.Controllers
{
    public class HomeController : Controller
    {
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        User users = new User();
        DataAccess.DataReaders DataReaders = new DataAccess.DataReaders();
        DataAccess.DataCreator DataCreators = new DataCreator();
        string val = "";
        Pag pages = new Pag();
        DataAccess.UserRole userroles = new DataAccess.UserRole();
        PageAuthentication _pa = new PageAuthentication();
        string user = "";
        LoanViewModel lvm = new LoanViewModel();
        Utility utils = new Utility();


       
        public ActionResult RecurringBenefit()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult Index()
        {
            try
            {
               // TempData["LogMsg"] = TempData["LogMsg"];
               return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult ResetPassword()
        {
            try
            {
                string value = Request.QueryString["value"];
                TempData["value"] = value;
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(FormCollection form, LoanViewModel lvm)
        {
            try
            {
                if (lvm.AccountsModel.pasword != "" && lvm.AccountsModel.confirmPassword != "")
                {
                    string value = Convert.ToString(form["value"]);
                    string password = lvm.AccountsModel.pasword;
                    string rpassword = lvm.AccountsModel.confirmPassword;

                    ResetPasswords(value, lvm);
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecoverPassword(LoanViewModel lvm)
        {
            try
            {
                if (lvm.AccountsModel.Email != null)
                {
                    string Email = lvm.AccountsModel.Email;
                    sendLink(Email);
                }
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult LogOut()
        {
            try
            {
                FormsAuthentication.SignOut();
                Session.Clear();
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult HowtoPay()
        {
            try
            {
                return View();
             
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        

        public ActionResult Signup()
        {
            try
            {
                // Referal Link Update
                // ViewBag.channel = DataReaders.GetMarketChannelList();
                ViewBag.channel = db.MarketingChannels;
                 LoanViewModel lvm = new LoanViewModel();
                string refCode = Request.QueryString["refCode"];
                
                if (refCode != "" || refCode != null)
                {
                    ViewBag.Referal = refCode;
                
                }
                // referal Link Update  
                string userID = "";
                if (Session["id"] == null)
                {
                    userID = "";
                }
                else
                {
                    userID = Session["id"].ToString();
                }
                if (userID == ConfigurationManager.AppSettings["PaelytEmail"])
                    userID = "";

                if (userID.Length < 1)
                {
                    return View();
                }
                else
                {
                    //return View("Index");
                    return RedirectToAction("Index", "Home");
                    //return View();
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public void SendEmail(LoanViewModel lvm)
        {
            try
            {
                var tAlert = new TransactionAlert
                {
                    PhoneNumber = lvm.AccountsModel.Phone,
                    Sender = "PayOrBoro",
                    Message = $"Dear {lvm.AccountsModel.firstname?.ToUpper()} {lvm.AccountsModel.lastname?.ToUpper()}, {ConfigurationManager.AppSettings["SmsWelcomeMessage"]}"
                };

                // var smsResponse = NotificationService.SendSms(tAlert);

                var bodyTxt = System.IO.File.ReadAllText(Server.MapPath("~/EmailNotifications/WelcomeEmailNotification.html"));
                bodyTxt = bodyTxt.Replace("$MerchantName", $"{lvm.AccountsModel.firstname} {lvm.AccountsModel.lastname}");
                var msgHeader = $"Welcome to PayorBoro";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, lvm.AccountsModel.Email, null, null);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
            }
        }
        [HttpGet]
        public ActionResult Signin()
        {
            
            string userID = "";

            if (Session["id"] == null)
            {
                userID = "";
            }
            else
            {
                userID = Session["id"].ToString();
            }
            if (userID == ConfigurationManager.AppSettings["PaelytEmail"])
                userID = "";
             
            if (userID.Length < 1)
            {
                return View();
            }
            else
            {

                // return RedirectToAction("Index", "Paytv");
                return RedirectToAction("Index", "Home");
                //return View();
            }

        }

        [HttpPost]
        public ActionResult Logins(LoanViewModel lvm)
        {
            try
            {
                users.Email = lvm.AccountsModel.Email;
                users.pasword = lvm.AccountsModel.pasword;
                var EncrypPassword = new CryptographyManager().ComputeHash(users.pasword, HashName.SHA256);
                users.pasword = EncrypPassword;

                var valid = DataReaders.loggedIn(users.Email, EncrypPassword);

                WebLog.Log("Email" + users.Email + EncrypPassword);
                WebLog.Log("Valid1" + valid);
                if (valid == true)
                {
                    WebLog.Log("Valid2" + valid);
                    if (users.Email != null)
                    {
                        WebLog.Log("users.Email 2" + users.Email);
                        Session["id"] = users.Email;
                        Session["User"] = Session["id"];
                        var LoggedInuser = new LogginHelper();
                        user = LoggedInuser.LoggedInUser();
                        // return View("index");

                        // string LoginPage = ConfigurationManager.AppSettings["DefaultLogin"];
                        // return RedirectToAction("Index", "Dashboard");
                        // This is For Banks 
                        var bankFk = DataReaders.getBankByUser(users.Email);
                        /* if(bankFk.BankFk != null || bankFk.BankFk != 0)
                         {
                             return RedirectToAction("DashboardSummary", "Dashboard");
                         }
                         else if(bankFk .BankFk== null || bankFk.BankFk == 0)
                          return RedirectToAction("Dashboard", "Dashboard");
                          */
                        if (bankFk.BankFk == null || bankFk.BankFk == 0)
                        {
                            TempData["Flg"] = "0";
                            return RedirectToAction("Dashboard", "Dashboard");
                        }
                        else if (bankFk.BankFk != null || bankFk.BankFk != 0)
                        {
                        
                            return RedirectToAction("DashboardSummary", "Dashboard");
                        }
                        // return Redirect(LoginPage);
                    }
                    else

                        TempData["message"] = "Invalid User Try Again";
                    // ViewBag.Message ="Try Again!";
                    return View("Signin");
                }
                else
                {
                    WebLog.Log("Valid3" + valid);
                    TempData["message"] = "User Does Not Exist";
                    ViewBag.Message = "User Does Not Exist";
                    return View("Signin");
                }
}
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [HttpGet]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(FormCollection form)
        {
            try
            {
                var Name = Convert.ToString(form["name"]);
                var Issue = Convert.ToString(form["issue"]); 
                var Email = Convert.ToString(form["email"]);
                var Phone = Convert.ToString(form["phone"]);
                var Enquiry = Convert.ToString(form["enquiry"]);
                var bodyTxt = System.IO.File.ReadAllText(Server.MapPath("~/EmailNotifications/EnquiryEmailNotification.html"));
                bodyTxt = bodyTxt.Replace("$Name", $"{Name}");
                bodyTxt = bodyTxt.Replace("$Issue", $"{Issue}");
                bodyTxt = bodyTxt.Replace("$Email", $"{Email}");
                bodyTxt = bodyTxt.Replace("$Phone", $"{Phone}");
                bodyTxt = bodyTxt.Replace("$Enquiry", $"{Enquiry}");
               
                var msgHeader = $"PayorBoro Feedback From" + " " + Name;
                WebLog.Log("resetLink: " + Enquiry);

                WebLog.Log("bodyTxt:" + bodyTxt);
                var CustomerCareEmail = ConfigurationManager.AppSettings["CustomerCare"];
                //var CustomerCareEmail = "tolutl@yahoo.com";
                var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, CustomerCareEmail, null, null);

                if (sendMail == true)
                {
                    TempData["SucMsg"] = "Mail Receive ";
                }
                else
                {
                    TempData["SucMsg"] = "Falied ! Please Try Again ! ";
                }
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            return View();
        }

        public ActionResult Roles()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }

        public ActionResult Page()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult UserRole()
        {
            try
            {
                var value = 13;
                users.id = value;
                GLobalClient gc = new GLobalClient();
                ViewBag.listUser = gc.findAllUser();
                //getAllUsersRoles(value);
                var result = (from a in db.UserRoles
                              join c in db.Roles on a.RoleId equals c.RoleId
                              join b in db.Users on a.UserId equals b.id
                              where a.UserId == users.id

                              select new Models.GetAssignRoles { userid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.Email }).ToList();

                var userroles = DataReaders.buildNamesList(users);
                var roles = DataReaders.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = (from p in db.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();



                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };


                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult UserRole(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                int roleid = Convert.ToInt16(form["roleid"]);
                int userid = Convert.ToInt16(form["id"]);
                userroles.RoleId = roleid;
                userroles.UserId = userid;
                userroles.RoleId = DataReaders.selectRolesByName(userroles);
                if (userroles.RoleId != 0)
                {
                    TempData["message"] = "User Already Have This Role";
                    return View("");


                }
                else

                {
                    userroles.RoleId = roleid;
                    userroles.UserId = userid;
                    userroles.IsVissible = 1;
                    userroles.dates = DateTime.Now;

                    DataCreators.InsertUserRoles(userroles);
                    TempData["message"] = "User n Roles Created Succesfully";
                    lvm.getAllUserAndRoless = getAllUsersAndRoles();

                }
                return View("");
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;

            }
        }

        public List<getAllUserAndRoles> getAllUsersAndRoles()
        {
            //---- Create object of our entities class.
            try
            {
                LoanViewModel lvm = new LoanViewModel();
                var user = "tolutl@yahoo.com";
                users.Email = user;
                users.id = DataReaders.selectUserID(users);

                var results = (from a in db.UserRoles
                               join c in db.Roles on a.RoleId equals c.RoleId
                               join b in db.Users on a.UserId equals b.id
                               //where a.UserId == users.id
                               select new Models.getAllUserAndRoles { userid = b.id, roleid = c.RoleId, rolename = c.RoleName, email = b.Email, id = a.id }).ToList();
                var model = new LoanViewModel
                {
                    getAllUserAndRoless = results.ToList(),

                };
                lvm.getAllUserAndRoless = results;
                return lvm.getAllUserAndRoless.ToList();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult PagesRoles(int value)
        {

            try
            {
                // int userid = 14;
                var val = "13";
                string rolename = "";
                pages.id = value;
                // pages.PageName = val;

                var result = (from a in db.PageAuthentications
                              join c in db.Roles on a.RoleId equals c.RoleId
                              join b in db.Pags on a.PageName equals b.PageName
                              where b.id == pages.id
                              select new Models.GetAssignPages { pageid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName }).ToList();

                {


                }
                var pageroles = DataReaders.buildPagesList(pages);
                var roles = DataReaders.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = (from p in db.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();
                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };


                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public List<Models.Role> GetItems()
        {
            try
            {
                var list = new List<Models.Role>();
                var Dacess = new DataAccess.DataReaders();
                var num = Dacess.GetAllRoles();
                return list;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult CreateUser(LoanViewModel lvm,FormCollection form)
        {
            GLobalClient cc = new GLobalClient();
            try
            {
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
                    lvm.AccountsModel.Referal = Convert.ToString(form["Referal"]);

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
                            // return RedirectToAction("Signup", "Home", new { area = "" });
                            ViewBag.channel = db.MarketingChannels;
                            return View("Signup");
                        }
                        else if (val == false)
                        {
                            lvm.AccountsModel.Date = DateTime.Now;
                            lvm.AccountsModel.DateTim = DateTime.Today;
                            lvm.AccountsModel.isVissibles = 1;
                            // This is For the Api 
                            // cc.CreateUser(lvm.AccountsModel);
                            User users = new DataAccess.User();
                            users.confirmPassword = lvm.AccountsModel.confirmPassword;
                            users.pasword = lvm.AccountsModel.pasword;
                            users.Email = lvm.AccountsModel.Email;
                            users.Date = lvm.AccountsModel.Date;
                            users.DateTim = lvm.AccountsModel.DateTim;
                            users.isVissibles = lvm.AccountsModel.isVissibles;
                            users.firstname = lvm.AccountsModel.firstname;
                            users.lastname = lvm.AccountsModel.lastname;
                            users.Phone = lvm.AccountsModel.Phone;
                            users.Referal = lvm.AccountsModel.Referal;
                            //string lastid = "0";
                            //users.MyReferalCode = MyUtility.getReferralCode(lastid);
                            users.ReferralLevel = MyUtility.getRefferalLevel(users.Referal);
                            // users.MyReferalCode = 
                            int id = DataCreators.CreateUser(users);

                            var channellist = Request["checkboxName"];
                            if (channellist != null )
                            {
                                string[] arr = channellist.Split(',');
                                var chanList = removestring(arr);
                                if (arr.Length > 0)
                                {
                                    for (var i = 0; i < arr.Length; i++)
                                    {
                                        string arrc = Convert.ToString(arr[i]);
                                        insertMarketChannel(arrc, id);
                                    }

                                }
                            }


                            if (id != 0)
                            {

                                string lastid = id.ToString();
                                users.MyReferalCode = MyUtility.getReferralCode(lastid);
                                DataCreators.updatelastID(users);

                                DataAccess.UserRole userroles = new DataAccess.UserRole();
                                userroles.UserId = id;
                                userroles.RoleId = Convert.ToInt16(ConfigurationManager.AppSettings["DefaultUser"]);
                                userroles.IsVissible = 1;
                                userroles.dates = DateTime.Now;
                                DataCreators.InsertUserRoles(userroles);
                                TempData["Message"] = "User Created Succesfully";

                              

                                //For LoggedIn User
                                Session["id"] = lvm.AccountsModel.Email;
                                Session["User"] = Session["id"];
                                var LoggedInuser = new LogginHelper();
                                user = LoggedInuser.LoggedInUser();
                                SendEmail(lvm);

                                string Regpage = ConfigurationManager.AppSettings["DefaultRegister"];

                                return Redirect(Regpage);

                                //return RedirectToAction("Index", "Dashboard");
                                //  return RedirectToAction("Dashboard", "Dashboard");
                            }
                            else
                            {
                                TempData["Message"] = "Registration Not Succesful Please Try Again!";
                                ViewBag.channel = db.MarketingChannels;
                            }
                        }

                    }
                }
                else
                {
                    TempData["Message"] = "Error Creating User";
                    ViewBag.channel = db.MarketingChannels;
                }
                return RedirectToAction("Signup", "Home", new { area = ""
                });
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        /*  public void insertMarketChannel(string Mc, int id)
          {
              try
              {
                 int res = DataCreators.insertMarketChannel(Mc,id);

              }
              catch(Exception ex)
              {
                  WebLog.Log(ex.Message.ToString());

              }
          }*/

        public int insertMarketChannel(string arrs, int userfk)
        {
            try
            {
                var chan = arrs.Before("?");
                var id = arrs.After("?");
                MarketingDetail MD = new MarketingDetail();
                MD.User_FK = userfk;
                MD.MarketingChannel_FK = Convert.ToInt16(id);
                MD.IsVisible = 1;
                MD.DateCreated = DateTime.Today;
                MD.ValueDate = "";
                MD.ValueTime = "";
                var Mgtchannel = DataCreators.insertMarketChannels(MD);

                return Mgtchannel;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return 0;
            }
        }

        public string removestring(string[] channelist)
        {
            try
            {
                string arrc = "";
                List<string> vc = new List<string>();
                char[] mychar = { '?', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
                if (channelist.Length > 0)
                {
                    for (var i = 0; i < channelist.Length; i++)
                    {
                        arrc = Convert.ToString(channelist[i]);
                        arrc = arrc.TrimEnd(mychar);

                        vc.Add(arrc);
                    }

                }
                string ListChannel = string.Join(", ", vc.ToArray());
                return (ListChannel);

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public bool ValidatePassword(string Password, string ConfirmPass)
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

        [HttpGet]
        public ActionResult PageRole()
        {
            try
            {
                var value = "";
                string Pageid = Request.QueryString["value"];
                if (Pageid == "" || Pageid == null)
                {

                    Pageid = "2016";
                }
                GLobalClient gc = new GLobalClient();
                ViewBag.listPage = gc.findAllPages();
                //var value = 2016;
                value = Pageid;
                pages.id = Convert.ToInt16(value);
                var pag = "";
                var selectedItem = DataReaders.getSelectedPage(pages.id);
                if (selectedItem != null)
                {
                    pag = selectedItem;
                    pages.PageName = pag;
                }

                var result = (from a in db.PageAuthentications
                              join c in db.Roles on a.RoleId equals c.RoleId
                              join b in db.Pags on a.PageName equals b.PageName
                              //  where b.id == pages.id
                              where b.id == pages.id
                              select new Models.GetAssignPages { pageid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName }).ToList();


                var pageroles = DataReaders.buildPagesList(pages);
                var roles = DataReaders.buildAllRoleList();
                List<int> pagerole = pageroles;
                List<int> role = roles;
                var newList = roles.Except(pageroles);

                var pageurl = (from p in db.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();
                var model = new LoanViewModel
                {
                    GetAssignPagess = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };


                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpPost]
        public ActionResult PageRole(LoanViewModel lvm, FormCollection form)
        {
            try
            {
                var pag = "";
                int roleid = Convert.ToInt16(form["RoleId"]);
                int pageid = Convert.ToInt16(form["id"]);
                var selectedItem = DataReaders.getSelectedPage(pageid);
                if (selectedItem != null)
                {
                    pag = selectedItem;

                }
                _pa.RoleId = roleid;
                _pa.PageName = pag;
                _pa.RoleId = DataReaders.selectPageRolesByName(_pa);
                if (_pa.RoleId != 0)
                {
                    TempData["message"] = "Page Already Have This Role";
                    return RedirectToAction("PageRole");
                }
                else
                {
                    _pa.RoleId = roleid;
                    _pa.PageName = pag;
                    _pa.isVissible = "1";


                    DataCreators.InsertPageRoles(_pa);
                    TempData["message"] = "Page Role Created Succesfully";
                    return RedirectToAction("PageRole");
                }

            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult Indexs()
        {
            try
            {
                string rolename = "";

                users.id = 13;
                users.Email = "";
                var result = (from a in db.UserRoles
                              join c in db.Roles on a.RoleId equals c.RoleId
                              join b in db.Users on a.UserId equals b.id
                              where a.UserId == users.id

                              select new Models.GetAssignRoles { userid = b.id, Roleid = c.RoleId.ToString(), Rolename = c.RoleName, email = b.Email }).ToList();

                ViewBag.AssignedRoles = result;
                ViewData["AssignedRoles"] = result;

                var userroles = DataReaders.buildNamesList(users);
                var roles = DataReaders.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = (from p in db.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new Models.UnGetAssignRoles { Roleid = p.RoleId, Rolename = p.RoleName }).ToList();


                ViewData["UnAssignedRoles"] = newList;
                //ViewBag.UnAssignedRoles = newList;
                ViewData["UnAssignedRolesurl"] = pageurl;
                //  ViewBag.UnAssignedRolesurl = pageurl;

                var model = new LoanViewModel
                {
                    GetAssignRoless = result.ToList(),
                    UnGetAssignRoless = pageurl.ToList(),
                };

                return View(model);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }

        }
        public void getAllUsersRoles(int value)
        {

            try
            {
                // int userid = 14;
                string rolename = "";

                users.id = value;
                users.Email = "";

                var result = (from a in db.UserRoles
                              join c in db.Roles on a.RoleId equals c.RoleId
                              join b in db.Users on a.UserId equals b.id
                              where a.UserId == users.id
                              select new

                              {
                                  userid = b.id,
                                  roleid = c.RoleId,
                                  rolename = c.RoleName,
                                  email = b.Email,

                              }).ToList();
                {

                    /*CheckBoxList1.DataValueField = "userid";
                      CheckBoxList1.DataTextField = "rolename";
                      CheckBoxList1.DataSource = result.ToList();
                      CheckBoxList1.DataBind();*/

                    ViewBag.AssignedRoles = result;
                    ViewData["AssignedRoles"] = result;
                    Int32 i = 0;
                    foreach (var items in result.ToList())
                    {

                        if (i <= result.Count)
                        {

                            if (items.roleid != 0)
                            {
                                //       CheckBoxList1.Items[i].Selected = true;
                                //        CheckBoxList1.Items[i].Enabled = false;

                            }

                            i = i + 1;
                        }
                    }
                }


                var userroles = DataReaders.buildNamesList(users);
                var roles = DataReaders.buildAllRoleList();
                List<int> userrole = userroles;
                List<int> role = roles;
                var newList = roles.Except(userroles);

                var pageurl = (from p in db.Roles
                               where newList.Contains((int)(p.RoleId))
                               select new

                               {
                                   rolename = p.RoleName,
                                   roleid = p.RoleId,

                               }).ToList();

                /* chkRoleList.DataSource = pageurl.ToList();
                 chkRoleList.DataValueField = "roleid";
                 chkRoleList.DataTextField = "rolename";

                 chkRoleList.DataBind();
                 */
                ViewData["UnAssignedRoles"] = newList;
                //ViewBag.UnAssignedRoles = newList;
                ViewData["UnAssignedRolesurl"] = pageurl;
                //  ViewBag.UnAssignedRolesurl = pageurl;



            }
            catch (Exception ex)
            {

                WebLog.Log(ex.Message.ToString());

            }


        }

        public ActionResult Terms()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }



        public ActionResult Faqs()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult Tcpower()
        {
            try
            {
                return View();
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public ActionResult Tcdata()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ActionResult Tccable()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public ActionResult Faq()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult faqpower()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        public ActionResult faqpaytv()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult PowerSteps()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult PaytvSteps()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        [HttpGet]
        public ActionResult AirtimeDataSteps()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult StepsToPay()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }


        [HttpGet]
        public ActionResult faqairtimedata()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult TermsLink()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        public ActionResult AllUserRoles()
        {
            try
            {
                LoanViewModel lvm = new LoanViewModel();
                lvm.getAllUserAndRoless = getAllUsersAndRoles();
                return View(lvm);
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.ToString());
                return null;
            }
        }

        public void ResetPasswords(string value, LoanViewModel lvm)
        {
            try
            {
                // if (Request.QueryString["value"] != null)
                if (value != "")
                {
                    // value = Request.QueryString["value"];
                    var result = DataReaders.checkValue(value);

                    if (result != null)
                    {
                        DateTime dtCreate = result.DateTim.Value;
                        DateTime dtNow = Classes.Utility.GetCurrentDateTime();
                        DateTime dtExp = dtCreate.AddMinutes(15);
                        if (dtNow > dtExp)
                        {
                            TempData["message"] = "Password Reset Link Expired";
                        }
                        else
                        {
                            var user = DataReaders.getUsers(value);
                            users.id = user.id;

                            UpdatePassword(lvm);
                        }


                    }
                    else
                    {
                        var id = DataReaders.getUsers(value);
                        TempData["message"] = "Invalid key please try again.";
                        return;
                    }

                }

                else
                {

                    TempData["message"] = "Invalid Url";
                    return;
                }

            }
            catch (Exception ex)
            {
                Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }
        }


        public void UpdatePassword(LoanViewModel lvm)
        {
            try
            {
                // LoanViewModel lvm = new LoanViewModel();
                users.pasword = lvm.AccountsModel.pasword;
                users.confirmPassword = lvm.AccountsModel.confirmPassword;
                var EncrypPassword = new CryptographyManager().ComputeHash(users.pasword, HashName.SHA256);
                users.pasword = EncrypPassword;
                string value = "";
                users.ResetPassword = value;
                DataCreators.UpdatePassword(users);
                TempData["message"] = "Password Successfully Update.";
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }

        }

        public void sendLink(String Email)
        {
            try
            {
                //LoanViewModel lvm = new LoanViewModel();
                //String email = lvm.AccountsModel.Email;
                var result = DataReaders.checkEmail(Email);
                WebLog.Log("Email +" + result);
                if (result != null)
                {
                    WebLog.Log("Email +" + result);
                    users.Email = result;
                    WebLog.Log("Email +" + users.Email);
                    users.id = DataReaders.selectUserIDs(users);
                    WebLog.Log("Email +" + users.id);
                    if (users.id != 0)
                    {
                        string encrypt = "";
                        try
                        {
                            encrypt = $"tK_{ Classes.Utility.RandomString(56).ToUpper()}" + users.id;
                            users = DataReaders.getUser(Email);
                            WebLog.Log("users +" + users.Email);
                            string resetLink = ConfigurationManager.AppSettings["ResetPasswordLink"] + encrypt;
                            WebLog.Log("resetLink +" + resetLink);
                            string resetLink1 = "Click The Following Link:<a href='" + resetLink + "'>'" + resetLink + "'</a> to change your password";
                            WebLog.Log("resetLink1 +" + resetLink1);
                            WebLog.Log("resetLink: " + resetLink);
                            var bodyTxt = System.IO.File.ReadAllText(Server.MapPath("~/EmailNotifications/ResetPasswordEmailNotification.html"));
                            bodyTxt = bodyTxt.Replace("$MerchantName", $"{users.firstname} {users.lastname}");
                            bodyTxt = bodyTxt.Replace("$Message", $"{resetLink1}");
                            var msgHeader = $"Reset Your Password";
                            WebLog.Log("resetLink: " + resetLink);

                            WebLog.Log("bodyTxt:" + bodyTxt);

                            var sendMail = NotificationService.SendMail(msgHeader, bodyTxt, users.Email, null, null);
                            users.ResetPassword = encrypt;
                            users.DateTim = Classes.Utility.GetCurrentDateTime();
                            DataCreators.UpdateUsers(users);
                            TempData["message"] = "Check Your Email For Password Reset Link";
                        }
                        catch (Exception ex)
                        {
                            ex.Message.ToString();
                            WebLog.Log(ex.Message.ToString());
                        }
                    }
                    else
                    {
                        TempData["message"] = "Please Try Again";
                    }
                }
                else
                {
                    TempData["message"] = "Account Does Not Exist";
                }
            }
            catch (Exception ex)
            {
                //Response.Write(ex.Message.ToString());
                WebLog.Log(ex.Message.ToString());
            }
        }

    }
}
public class MarketChannel
{
    public int ID { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int IsVisible { get; set; }
    public int ListOrderID { get; set; }
}
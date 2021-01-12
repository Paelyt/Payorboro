using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GloballendingViews.Models;
using DataAccess;
using GloballendingViews.ViewModels;
using System.Collections.Generic;
using GloballendingViews.HelperClasses;
using System.Web.Security;

namespace GloballendingViews.Controllers
{
    public class IndexController : Controller
    {
        public GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();
        User users = new User();
        DataAccess.DataReaders DataReaders = new DataAccess.DataReaders();
        DataAccess.DataCreator DataCreators = new DataCreator();
        string val = "";
        Pag pages = new Pag();
        DataAccess.UserRole userroles = new DataAccess.UserRole();
        PageAuthentication _pa = new PageAuthentication();

        // GET: Index
        public ActionResult Index()
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

        public ActionResult Signin()
        {
            try {
                return View();
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }

        public ActionResult Signup()
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

        public ActionResult Roles()
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

        public ActionResult Page()
        {
            try {
                return View();
            }
            catch(Exception ex)
            {
                
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
        [HttpGet]
        public ActionResult UserRole()
        {
            try {
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
            catch(Exception ex)
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
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
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
        public ActionResult PageRole(LoanViewModel lvm , FormCollection form)
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
            try {
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
            catch(Exception ex)
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


        public ActionResult AllUserRoles()
        {
            try {
                LoanViewModel lvm = new LoanViewModel();
                lvm.getAllUserAndRoless = getAllUsersAndRoles();
                return View(lvm);
            }
            catch(Exception ex)
            {
                WebLog.Log(ex.ToString());
                return null;
            }
        }

        [HttpPost]
        public ActionResult Login(LoanViewModel lvm)
        {
        try
        {
         users.Email = lvm.AccountsModel.Email;
         users.pasword = lvm.AccountsModel.pasword;
         var EncrypPassword = new CryptographyManager().ComputeHash(users.pasword, HashName.SHA256);
         users.pasword = EncrypPassword;

         var valid = DataReaders.loggedIn(users.Email,EncrypPassword);

         if (valid == true)
         {
         if (users.Email != null)
         {
        Session["id"] = users.Email;
        Session["User"] = "Welcome " + Session["id"];
        var LoggedInuser = new LogginHelper();
        string user = LoggedInuser.LoggedInUser();
               return View("index");
        }
          else
        {
         ViewBag.Message = "Invalid Details ! Try Again";
         return View("Signin");
        }

               
            }
                return View("Signin");
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
                return RedirectToAction("Index", "Index");

            }
            catch(Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
            }
       }
}
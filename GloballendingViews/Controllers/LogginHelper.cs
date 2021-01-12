using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace GloballendingViews.Controllers
{
    public class LogginHelper
    {

        
        public string LoggedInUser()
        {
            try
            {
    string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();
     

          return sessionUserId;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool getCurrentUser()
        {
            try {
                string sessionUserId = HttpContext.Current.Session["id"].ToString().Trim();

                if (sessionUserId != null)
                {

                    return true;
                }
                else
                {


                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public string getDefaultUser()
        {
            try
            {
                HttpContext.Current.Session["id"] = ConfigurationManager.AppSettings["PaelytEmail"];
                string sessionUserId = HttpContext.Current.Session["id"].ToString();
                return sessionUserId;
            }
            catch (Exception ex)
            {
                WebLog.Log(ex.Message.ToString());
                return null;
            }
        }
    }
}
using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using GloballendingViews.Classes;
namespace GloballendingViews.Classes
{
   
    [WebService(Namespace = "www.XMLWebServiceSoapHeaderAuth.net")
      ]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class servicelm : System.Web.Services.WebService
    {
        public AuthSoapHd spAuthenticationHeader;

        public servicelm()
        {
        }

        public class AuthSoapHd : SoapHeader
        {
            public string CALLCENTER_USERNAME;
            public string CALLCENTER_PASSWORD;
        }

        [WebMethod, SoapHeader("spAuthenticationHeader")]
        public string HelloWorld()
        {
            if (spAuthenticationHeader.CALLCENTER_USERNAME == "StarCallCenter" &&
              spAuthenticationHeader.CALLCENTER_PASSWORD == "StarCallCenter")
            {
                return "User Name : " + spAuthenticationHeader.CALLCENTER_USERNAME + " and " +
                  "Password : " + spAuthenticationHeader.CALLCENTER_PASSWORD;
            }
            else
            {
                return "Access Denied";
            }
        }
    }
}
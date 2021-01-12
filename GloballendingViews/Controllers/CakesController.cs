using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GloballendingViews.Controllers
{
    public class CakesController : ApiController
    {
        [HttpGet]
        public IHttpActionResult GetAllFlavour()
        {
          //  IList<Flavour> flavour = new List<Flavour>();

            List<string> AuthorList = new List<string>();

            AuthorList.Add("Vanilla");
            AuthorList.Add("chocolate");
            AuthorList.Add("rainbow");
            AuthorList.Add("topin");
            AuthorList.Add("bacon");
            AuthorList.Add("Happy Birthday");


            if (AuthorList.Count == 0)
            {
                return NotFound();
            }

            return Ok(AuthorList);
        }
    }

    public class Flavour
    {
        public string name { get; set; }
    }
}

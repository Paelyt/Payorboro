using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using DataAccess;

namespace GlobalLending.Controllers
{
    public class CustomerServicesController : ApiController
    {
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        // GET: api/CustomerServices
        public IQueryable<CustomerService> GetCustomerServices()
        {
            return db.CustomerServices;
        }

        // GET: api/CustomerServices/5
        [ResponseType(typeof(CustomerService))]
        public IHttpActionResult GetCustomerService(int id)
        {
            CustomerService customerService = db.CustomerServices.Find(id);
            if (customerService == null)
            {
                return NotFound();
            }

            return Ok(customerService);
        }

        // PUT: api/CustomerServices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCustomerService(int id, CustomerService customerService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerService.ID)
            {
                return BadRequest();
            }

            db.Entry(customerService).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerServiceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/CustomerServices
        [ResponseType(typeof(CustomerService))]
        public IHttpActionResult PostCustomerService(CustomerService customerService)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CustomerServices.Add(customerService);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = customerService.ID }, customerService);
        }

        // DELETE: api/CustomerServices/5
        [ResponseType(typeof(CustomerService))]
        public IHttpActionResult DeleteCustomerService(int id)
        {
            CustomerService customerService = db.CustomerServices.Find(id);
            if (customerService == null)
            {
                return NotFound();
            }

            db.CustomerServices.Remove(customerService);
            db.SaveChanges();

            return Ok(customerService);
        }



        public IHttpActionResult DeleteCustomerServices(CustomerService customerService)
        {
          customerService = db.CustomerServices.Find(customerService.ID);
         if(customerService != null)
            {
           customerService.isVissible = 0;
           if (customerService == null)
            {
                return NotFound();
                }
                if (customerService.ID == 0)
                {
                    return BadRequest();
                }

                db.Entry(customerService).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Ok(customerService);
           
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CustomerServiceExists(int id)
        {
            return db.CustomerServices.Count(e => e.ID == id) > 0;
        }


        // GET: api/CustomerServices/5
        [ResponseType(typeof(CustomerService))]
        public IHttpActionResult GetCustomerServiceByUser(int id)
        {
            try
            {
                var customerService = (from a in db.CustomerServices
                                       where a.Customer_FK == id && a.isVissible== 1
                                       select a).ToList();
                if (customerService == null)
                {
                    return NotFound();
                }

                return Ok(customerService);
            }
            catch(Exception ex)
            {
                return null;
            }
        }


        [HttpPost]
        public string Upload(int id)
        {
            CustomerService customerService = db.CustomerServices.Find(id);
            // var customerService = (from a in db.CustomerServices
            //   where a.Customer_FK == id 
            //  select a).ToList();
            if (customerService == null)
            {
                return null;
            }

            return null;
        }
    }
}
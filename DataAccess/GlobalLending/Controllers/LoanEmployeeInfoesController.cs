using DataAccess;
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
//using GlobalLending.Model;

namespace GlobalLending.Controllers
{
    public class LoanEmployeeInfoesController : ApiController
    {
       // private GlobalTransactEntities db = new GlobalTransactEntities();
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        // GET: api/LoanEmployeeInfoes
        public IQueryable<LoanEmployeeInfo> GetLoanEmployeeInfoes()
        {
            return db.LoanEmployeeInfoes;
        }

        // GET: api/LoanEmployeeInfoes/5
        [ResponseType(typeof(LoanEmployeeInfo))]
        public IHttpActionResult GetLoanEmployeeInfo(int id)
        {
            LoanEmployeeInfo loanEmployeeInfo = db.LoanEmployeeInfoes.Find(id);
            if (loanEmployeeInfo == null)
            {
                return NotFound();
            }

            return Ok(loanEmployeeInfo);
        }

        // PUT: api/LoanEmployeeInfoes/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLoanEmployeeInfo(int id, LoanEmployeeInfo loanEmployeeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loanEmployeeInfo.ID)
            {
                return BadRequest();
            }

            db.Entry(loanEmployeeInfo).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanEmployeeInfoExists(id))
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

        // POST: api/LoanEmployeeInfoes
        [ResponseType(typeof(LoanEmployeeInfo))]
        public IHttpActionResult PostLoanEmployeeInfo(LoanEmployeeInfo loanEmployeeInfo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LoanEmployeeInfoes.Add(loanEmployeeInfo);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = loanEmployeeInfo.ID }, loanEmployeeInfo);
        }

        // DELETE: api/LoanEmployeeInfoes/5
        [ResponseType(typeof(LoanEmployeeInfo))]
        public IHttpActionResult DeleteLoanEmployeeInfo(int id)
        {
            LoanEmployeeInfo loanEmployeeInfo = db.LoanEmployeeInfoes.Find(id);
            if (loanEmployeeInfo == null)
            {
                return NotFound();
            }

            db.LoanEmployeeInfoes.Remove(loanEmployeeInfo);
            db.SaveChanges();

            return Ok(loanEmployeeInfo);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoanEmployeeInfoExists(int id)
        {
            return db.LoanEmployeeInfoes.Count(e => e.ID == id) > 0;
        }
    }
}
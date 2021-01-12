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
using DataAccess;
using GlobalLending.DataProvider;

namespace GlobalLending.Controllers
{
    public class LoansController : ApiController
    {
     //private GlobalTransactEntities db = new GlobalTransactEntities();
     DataProvider.myReaders myreader = new DataProvider.myReaders();
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

      /*  public HttpResponseMessage GetAllLoans()
        {
            try
            {
                var loan = myreader.selectAllLoan();
                return null;
               
            }
            catch (Exception ex)
            {
                return null;
            }
        }*/
        // GET: api/Loans
        public IQueryable<DataAccess.Loan> GetLoans()
        {
            return db.Loans;
        }

        // GET: api/Loans/5
        [ResponseType(typeof(DataAccess.Loan))]
        public IHttpActionResult GetLoan(int id)
        {
            DataAccess.Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return NotFound();
            }

            return Ok(loan);
        }

        // PUT: api/Loans/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLoan(int id, DataAccess.Loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loan.ID)
            {
                return BadRequest();
            }

            db.Entry(loan).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanExists(id))
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

        // POST: api/Loans
        [ResponseType(typeof(DataAccess.Loan))]
        public IHttpActionResult PostLoan(DataAccess.Loan loan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Loans.Add(loan);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = loan.ID }, loan);
        }

        // DELETE: api/Loans/5
        [ResponseType(typeof(DataAccess.Loan))]
        public IHttpActionResult DeleteLoan(int id)
        {
            DataAccess.Loan loan = db.Loans.Find(id);
            if (loan == null)
            {
                return NotFound();
            }

            db.Loans.Remove(loan);
            db.SaveChanges();

            return Ok(loan);
         }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoanExists(int id)
        {
            return db.Loans.Count(e => e.ID == id) > 0;
        }
    }
}
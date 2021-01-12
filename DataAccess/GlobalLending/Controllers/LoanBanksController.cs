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
    public class LoanBanksController : ApiController
    {
        //   private GlobalTransactEntities db = new GlobalTransactEntities();
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();


        // GET: api/LoanBanks
        public IQueryable<LoanBank> GetLoanBanks()
        {
            return db.LoanBanks;
        }

        // GET: api/LoanBanks/5
        [ResponseType(typeof(LoanBank))]
        public IHttpActionResult GetLoanBank(int id)
        {
            LoanBank loanBank = db.LoanBanks.Find(id);
            if (loanBank == null)
            {
                return NotFound();
            }

            return Ok(loanBank);
        }

        // PUT: api/LoanBanks/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLoanBank(int id, LoanBank loanBank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loanBank.ID)
            {
                return BadRequest();
            }

            db.Entry(loanBank).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanBankExists(id))
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

        // POST: api/LoanBanks
        [ResponseType(typeof(LoanBank))]
        public IHttpActionResult PostLoanBank(LoanBank loanBank)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LoanBanks.Add(loanBank);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = loanBank.ID }, loanBank);
        }

        // DELETE: api/LoanBanks/5
        [ResponseType(typeof(LoanBank))]
        public IHttpActionResult DeleteLoanBank(int id)
        {
            LoanBank loanBank = db.LoanBanks.Find(id);
            if (loanBank == null)
            {
                return NotFound();
            }

            db.LoanBanks.Remove(loanBank);
            db.SaveChanges();

            return Ok(loanBank);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoanBankExists(int id)
        {
            return db.LoanBanks.Count(e => e.ID == id) > 0;
        }
    }
}
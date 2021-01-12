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
    public class LoanSocialsController : ApiController
    {
        // private GlobalTransactEntities db = new GlobalTransactEntities();
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        // GET: api/LoanSocials
        public IQueryable<LoanSocial> GetLoanSocials()
        {
            return db.LoanSocials;
        }

        // GET: api/LoanSocials/5
        [ResponseType(typeof(LoanSocial))]
        public IHttpActionResult GetLoanSocial(int id)
        {
            LoanSocial loanSocial = db.LoanSocials.Find(id);
            if (loanSocial == null)
            {
                return NotFound();
            }

            return Ok(loanSocial);
        }

        // PUT: api/LoanSocials/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutLoanSocial(int id, LoanSocial loanSocial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != loanSocial.ID)
            {
                return BadRequest();
            }

            db.Entry(loanSocial).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LoanSocialExists(id))
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

        // POST: api/LoanSocials
        [ResponseType(typeof(LoanSocial))]
        public IHttpActionResult PostLoanSocial(LoanSocial loanSocial)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LoanSocials.Add(loanSocial);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = loanSocial.ID }, loanSocial);
        }

        // DELETE: api/LoanSocials/5
        [ResponseType(typeof(LoanSocial))]
        public IHttpActionResult DeleteLoanSocial(int id)
        {
            LoanSocial loanSocial = db.LoanSocials.Find(id);
            if (loanSocial == null)
            {
                return NotFound();
            }

            db.LoanSocials.Remove(loanSocial);
            db.SaveChanges();

            return Ok(loanSocial);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LoanSocialExists(int id)
        {
            return db.LoanSocials.Count(e => e.ID == id) > 0;
        }
    }
}
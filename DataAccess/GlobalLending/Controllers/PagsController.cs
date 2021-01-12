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
    public class PagsController : ApiController
    {
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        // GET: api/Pags
        public IQueryable<Pag> GetPags()
        {
            return db.Pags;
        }

        // GET: api/Pags/5
        [ResponseType(typeof(Pag))]
        public IHttpActionResult GetPag(int id)
        {
            Pag pag = db.Pags.Find(id);
            if (pag == null)
            {
                return NotFound();
            }

            return Ok(pag);
        }

        // PUT: api/Pags/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPag(int id, Pag pag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pag.id)
            {
                return BadRequest();
            }

            db.Entry(pag).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PagExists(id))
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

        // POST: api/Pags
        [ResponseType(typeof(Pag))]
        public IHttpActionResult PostPag(Pag pag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Pags.Add(pag);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = pag.id }, pag);
        }

        // DELETE: api/Pags/5
        [ResponseType(typeof(Pag))]
        public IHttpActionResult DeletePag(int id)
        {
            Pag pag = db.Pags.Find(id);
            if (pag == null)
            {
                return NotFound();
            }

            db.Pags.Remove(pag);
            db.SaveChanges();

            return Ok(pag);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PagExists(int id)
        {
            return db.Pags.Count(e => e.id == id) > 0;
        }
    }
}
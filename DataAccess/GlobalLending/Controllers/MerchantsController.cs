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
    public class MerchantsController : ApiController
    {
        private GlobalTransactEntitiesData db = new GlobalTransactEntitiesData();

        // GET: api/Merchants
        public IQueryable<Merchant> GetMerchants()
        {
            return db.Merchants;
        }

        // GET: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult GetMerchant(string id)
        {
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }

            return Ok(merchant);
        }

        // PUT: api/Merchants/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutMerchant(string id, Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != merchant.MarchantName)
            {
                return BadRequest();
            }

            db.Entry(merchant).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MerchantExists(id))
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

        // POST: api/Merchants
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult PostMerchant(Merchant merchant)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Merchants.Add(merchant);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (MerchantExists(merchant.MarchantName))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = merchant.MarchantName }, merchant);
        }

        // DELETE: api/Merchants/5
        [ResponseType(typeof(Merchant))]
        public IHttpActionResult DeleteMerchant(string id)
        {
            Merchant merchant = db.Merchants.Find(id);
            if (merchant == null)
            {
                return NotFound();
            }

            db.Merchants.Remove(merchant);
            db.SaveChanges();

            return Ok(merchant);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MerchantExists(string id)
        {
            return db.Merchants.Count(e => e.MarchantName == id) > 0;
        }
    }
}
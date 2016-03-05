using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;
using Microsoft.AspNet.Identity;
using Member = cfcusaga.domain.Membership.Member;

namespace Cfcusaga.Web.Controllers
{
    public class MyOrdersController : Controller
    {
        private PortalDbContext db = new PortalDbContext();
        private string _currentUserId;

        public MyOrdersController()
        {
            _currentUserId = User.Identity.GetUserId();
            
        }

        private async Task<cfcusaga.domain.Membership.Member> GetMemberInfoFromAspNetUserId(string currentUserId)
        {
            var entity = await db.Members.FirstOrDefaultAsync(c => c.AspNetUserId == currentUserId);
            if (entity != null)
            {
                var aMember = new Member()
                {
                    Id = entity.Id,
                    LastName = entity.LastName,
                    Firstname = entity.FirstName,
                    Gender = entity.Gender
                };
                return aMember;
            }
            return null;
        }

        // GET: MyOrders
        public async Task<ActionResult> Index()
        {
            var member = await GetMemberInfoFromAspNetUserId(_currentUserId);
            var orders = db.Orders.Include(o => o.Member);
            return View(await orders.ToListAsync());
        }

        // GET: MyOrders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: MyOrders/Create
        public ActionResult Create()
        {
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName");
            return View();
        }

        // POST: MyOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,SaveInfo,CheckNumber,Notes,AspNetUserId,MemberId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // GET: MyOrders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // POST: MyOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,SaveInfo,CheckNumber,Notes,AspNetUserId,MemberId")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // GET: MyOrders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: MyOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

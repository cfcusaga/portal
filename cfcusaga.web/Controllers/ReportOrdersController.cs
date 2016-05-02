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
using cfcusaga.domain.Helpers;
using Cfcusaga.Web.Models;
using PagedList;
using Order = cfcusaga.data.Order;

namespace Cfcusaga.Web.Controllers
{
    public class ReportOrdersController : Controller
    {
        private PortalDbContext _db = new PortalDbContext();

        // GET: ReportOrders
        public async Task<ActionResult> Index(int eventId, int? page, int? pageSize)
        {
            var orders = from o in _db.Orders.AsNoTracking()
                join od in _db.OrderDetails.AsNoTracking() on o.OrderId equals od.OrderId
                join i in _db.Items.AsNoTracking() on od.ItemId equals i.ID
                join e in _db.Events.AsNoTracking() on i.EventId equals e.Id
                where e.Id == eventId
                         select new OrderItems
                         {
                             Id = $"{o.OrderId}/{od.OrderDetailId}",
                             OrderId = o.OrderId,
                             OrderDateUtc = o.OrderDate,
                             OrderDetailId = od.OrderDetailId,
                             ItemId = i.ID,
                             ItemName = i.Name,
                             CategoryId = i.CatagoryID,
                             Firstname = od.Firstname,
                             Lastname = od.Lastname,
                             TshirtSize = od.TshirtSize,
                             Allergies = od.Allergies,
                             City = o.City,
                             State = o.State,
                             ZipCode = o.PostalCode,
                             Phone = o.Phone
                         }
            ;
            var pageNumber = (page ?? 1);
            pageSize = (pageSize ?? 20);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize.Value));



            //var items = from i in _db.Items.AsNoTracking()
            //            join e in _db.Events.AsNoTracking() on i.EventId equals e.Id
            //            join c in _db.Catagories.AsNoTracking() on i.CatagoryID equals c.ID
            //            where i.EventId == eventId.Value
            //            orderby c.SortOrder ascending
            //            select new Item
            //            {
            //                Name = i.Name,
            //                Price = i.Price,
            //                Id = i.ID,
            //                CatagorieId = i.CatagoryID,
            //                ItemPictureUrl = i.ItemPictureUrl,
            //                InternalImage = i.InternalImage,
            //                EventName = e.Name,
            //                EventId = (int)i.EventId
            //            };
        }

        // GET: ReportOrders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: ReportOrders/Create
        public ActionResult Create()
        {
            ViewBag.MemberId = new SelectList(_db.Members, "Id", "FirstName");
            return View();
        }

        // POST: ReportOrders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,SaveInfo,CheckNumber,Notes,AspNetUserId,MemberId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.MemberId = new SelectList(_db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // GET: ReportOrders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.MemberId = new SelectList(_db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // POST: ReportOrders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "OrderId,OrderDate,Username,FirstName,LastName,Address,City,State,PostalCode,Country,Phone,Email,Total,SaveInfo,CheckNumber,Notes,AspNetUserId,MemberId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(order).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.MemberId = new SelectList(_db.Members, "Id", "FirstName", order.MemberId);
            return View(order);
        }

        // GET: ReportOrders/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await _db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: ReportOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Order order = await _db.Orders.FindAsync(id);
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }


}

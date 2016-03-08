using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Helpers;
using Cfcusaga.Web.Models;

namespace Cfcusaga.Web.Controllers
{
    public class ReportsController : Controller
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
                         orderby o.OrderId
                         select new OrderItems
                         {
                             OrderId = o.OrderId,
                             OrderDate = o.OrderDate,
                             OrderDetailId = od.OrderDetailId,
                             ItemId = i.ID,
                             ItemName = i.Name,
                             CategoryId = i.CatagoryID,
                             Firstname = od.Firstname,
                             Lastname = od.Lastname,
                             OrderByLastname = o.LastName,
                             OrderByFirstname = o.FirstName,
                             TshirtSize = od.TshirtSize,
                             Allergies = od.Allergies,
                             City = o.City,
                             State = o.State,
                             ZipCode = o.PostalCode,
                             Phone = o.Phone,
                             Price = od.UnitPrice
                         }
            ;
            var pageNumber = (page ?? 1);
            pageSize = (pageSize ?? 20);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize.Value));

        }

        // GET: Reports/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Reports/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reports/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reports/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Reports/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reports/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}

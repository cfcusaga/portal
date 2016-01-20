using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.data;
using Cfcusaga.Web.Models;
using Cfcusaga.Web.ViewModels;

namespace  Cfcusaga.Web.Controllers
{
    public class AnalyticsController : Controller
    {
        private PortalDbContext db = new PortalDbContext();
        AnalyticsViewModel vm = new AnalyticsViewModel();

        // GET: Analytics
        public async Task<ActionResult> Index()
        {
            var data = (from orders in db.Orders
                       group orders by orders.OrderDate into dateGroup
                       select new OrderDateGroup()
                       {
                            OrderDate = dateGroup.Key,
                            OrderCount = dateGroup.Count()
                       }).Take(10);

            var allData = (from orders in db.Orders
                        group orders by orders.OrderDate into dateGroup
                        select new OrderDateGroup()
                        {
                            OrderDate = dateGroup.Key,
                            OrderCount = dateGroup.Count()
                        });

            
            vm.OrderData = await data.ToListAsync();
            vm.OrderDataForToday = await allData.ToListAsync();

            // Commenting out LINQ to show how to do the same thing in SQL.
            //IQueryable<EnrollmentDateGroup> = from student in db.Students
            //           group student by student.EnrollmentDate into dateGroup
            //           select new EnrollmentDateGroup()
            //           {
            //               EnrollmentDate = dateGroup.Key,
            //               StudentCount = dateGroup.Count()
            //           };

            // SQL version of the above LINQ code.
            //string query = "SELECT OrderDate, COUNT(*) AS StudentCount "
            //    + "FROM Person "
            //    + "WHERE Discriminator = 'Student' "
            //    + "GROUP BY EnrollmentDate";
            //IEnumerable<Order> data = db.Database.SqlQuery<Order>(query);


            return View(vm);
        }

        // GET: Analytics/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            cfcusaga.data.Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Analytics/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Analytics/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(cfcusaga.data.Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(order);
        }

        // GET: Analytics/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Analytics/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(cfcusaga.data.Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(order);
        }

        // GET: Analytics/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Analytics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public JsonResult GetDataAsJson()
        {
            var data = GetData();
            return Json(data, JsonRequestBehavior.AllowGet);
            //return data;
        }

        private List<OrderDateGroup> GetData()
        {
            var allData = (from orders in db.Orders
                           group orders by orders.OrderDate into dateGroup
                           select new OrderDateGroup()
                           {
                               OrderDate = dateGroup.Key,
                               OrderCount = dateGroup.Count()
                           });


            var OrderData = allData.ToList();

            return OrderData;
        }

        public dynamic StopsByMonth()
        {
            var resultSet = GetData();


            return resultSet;
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

using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Events;

namespace  Cfcusaga.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private readonly IEventServices _svc;
        private PortalDbContext _db = new PortalDbContext();


        public EventsController(IEventServices svc)
        {
            _svc = svc;
        }

        // GET: Catagories
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.OrgIdSortParm = sortOrder == "orgId" ? "orgId_desc" : "orgId";
            ViewBag.DateSortParm = sortOrder == "startDate" ? "startDate_desc" : "startDate";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //return View(_svc.ToPagedList());
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            //return View(await _db.Events.ToListAsync());
            return View(await _svc.GetOpenEvents(sortOrder, searchString, pageSize, pageNumber));
        }

        // GET: Catagories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //var anEvent = await _db.Events.FindAsync(id);
            var anEvent = await _svc.GetEventDetails(id);
            if (anEvent == null)
            {
                return HttpNotFound();
            }
            return View(anEvent);
        }

        // GET: Catagories/Create
         [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Catagories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(cfcusaga.domain.Events.Event newEvent)
        {
            if (ModelState.IsValid)
            {
                await _svc.SaveChangesAsync(newEvent);
                return RedirectToAction("Index");
            }

            return View(newEvent);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public async Task<ActionResult> Create([Bind(Include = "ID,Name")] cfcusaga.data.Event anEvent)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Events.Add(anEvent);
        //        await _db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(anEvent);
        //}


        // GET: Catagories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anEvent = await _db.Events.FindAsync(id);
            if (anEvent == null)
            {
                return HttpNotFound();
            }
            return View(anEvent);
        }

        // POST: Catagories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name")] cfcusaga.data.Event anEvent)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(anEvent).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(anEvent);
        }

        // GET: Catagories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anEvent = await _db.Events.FindAsync(id);
            if (anEvent == null)
            {
                return HttpNotFound();
            }
            return View(anEvent);
        }

        // POST: Catagories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var catagorie = await _db.Events.FindAsync(id);
            _db.Events.Remove(catagorie);
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

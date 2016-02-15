using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using cfcusaga.domain.Events;

namespace  Cfcusaga.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private const string EventIdSessionKey = "EventId";

        public static int? GetSessionEventId(HttpContextBase context)
        {
            if (context.Session != null)
            {
                var o = context.Session[EventIdSessionKey];
                if (o != null) return int.Parse(o.ToString());
            }
            return null;
        }

        public static void SetSessionEventId(HttpContextBase httpContext, int id)
        {
            var httpSessionStateBase = httpContext.Session;
            if (httpSessionStateBase != null) httpSessionStateBase[EventIdSessionKey] = id.ToString();
        }

        private readonly IEventServices _svc;

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

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(await _svc.GetOpenEvents(sortOrder, searchString, pageSize, pageNumber));
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var anEvent = await _svc.GetEventDetails(id);
            if (anEvent == null)
            {
                return HttpNotFound();
            }
            return View(anEvent);
        }

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

        // GET: Catagories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var anEvent = await _svc.GetEventDetails(id);
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
        public async Task<ActionResult> Edit(cfcusaga.domain.Events.Event anEvent)
        {
            if (ModelState.IsValid)
            {
                var updatedEvent = await _svc.UpdateEvent(anEvent);
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
            var anEvent = await _svc.GetEventDetails(id);
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
            await _svc.DeleteEvent(id);

            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        public ActionResult EventItems(int id)
        {
            //SetSessionEventId(this.HttpContext, id);
            return RedirectToAction("Register", "Items", new {eventId = id});
        }



    }
}

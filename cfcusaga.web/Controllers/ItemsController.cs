using System;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cfcusaga.domain.Events;

namespace Cfcusaga.Web.Controllers
{
    public class ItemsController : Controller
    {
        private readonly IEventServices _svc;
        private int? _eventId;

        public ItemsController(IEventServices svc)
        {
            _svc = svc;
        }

        public RedirectToRouteResult Register(int eventId)
        {
            EventsController.SetSessionEventId(this.HttpContext, eventId);
            return RedirectToAction("Index", new {eventId});
        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            _eventId = EventsController.GetSessionEventId(this.HttpContext);

            //TODO: place this in session
            var anEvent = await _svc.GetEventDetails(_eventId);
            ViewBag.EventName = anEvent.Name;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var pageSize = 10;
            var pageNumber = (page ?? 1);

            var items = await _svc.GetEventItems(_eventId, sortOrder, searchString,  pageSize, pageNumber);

            return View( items);

        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _svc.GetEventItemDetails(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CatagorieId = new SelectList(_svc.GetItemCategories(), "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(cfcusaga.domain.Events.Item item)
        {
            if (ModelState.IsValid)
            {
                var eventId = EventsController.GetSessionEventId(this.HttpContext);
                item.EventId = eventId.HasValue ? eventId.Value : 0;
                await _svc.AddEventItem(item);
                return RedirectToAction("Index");
            }

            ViewBag.CatagorieId = new SelectList(_svc.GetItemCategories(), "ID", "Name");
            return View(item);
        }

         [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _svc.GetEventItemDetails(id);

            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatagorieId = new SelectList(_svc.GetItemCategories(), "ID", "Name");
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(cfcusaga.domain.Events.Item item)
        {
            if (ModelState.IsValid)
            {
                await _svc.UpdateEventItemAsync(item);
                return RedirectToAction("Index");
            }
            //ViewBag.CatagorieId = new SelectList(_db.Catagories, "ID", "Name", item.CatagorieId);
            ViewBag.CatagorieId = new SelectList(_svc.GetItemCategories(), "ID", "Name");
            return View(item);
        }

        // GET: Items/Delete/5
         [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var item = await _db.Items.FindAsync(id);
            var item = await _svc.GetEventItemDetails(id);

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            await _svc.DeleteEventItem(id);
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            var item = await _svc.GetEventItemDetails(id);

            byte[] photoBack = item.InternalImage;

            return File(photoBack, "image/png");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}

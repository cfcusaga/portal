using System;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Events;
using Item = Cfcusaga.Web.Models.Item;

namespace Cfcusaga.Web.Controllers
{
    public class ItemsController : Controller
    {
        private readonly PortalDbContext _db;
        private readonly EventServices _svc;

        public ItemsController(PortalDbContext db, EventServices svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET: Items
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            //return View(_svc.ToPagedList());
            int pageSize = 3;
            int pageNumber = (page ?? 1);

            var items = await _svc.GetItems(sortOrder, searchString,  pageSize, pageNumber);

            return View( items);


            //var items = db.Items.Include(i => i.Catagorie);
            //return View(await items.ToListAsync());
        }

        // GET: Items/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var item = await _db.Items.FindAsync(id);
            var item = await _svc.GetItemDetails(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // GET: Items/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {

            //ViewBag.CatagorieId = new SelectList(_db.Catagories, "ID", "Name");
            ViewBag.CatagorieId = new SelectList(_svc.GetCatagories(), "ID", "Name");
            return View();
        }

        // POST: Items/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create(cfcusaga.data.Item item)
        {
            if (ModelState.IsValid)
            {
                _db.Items.Add(item);
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CatagorieId = new SelectList(_db.Catagories, "ID", "Name", item.CatagorieId);
            return View(item);
        }

        // GET: Items/Edit/5
         [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            ViewBag.CatagorieId = new SelectList(_db.Catagories, "ID", "Name", item.CatagorieId);
            return View(item);
        }

        // POST: Items/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(item).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CatagorieId = new SelectList(_db.Catagories, "ID", "Name", item.CatagorieId);
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
            var item = await _db.Items.FindAsync(id);
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var item = await _db.Items.FindAsync(id);
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public async Task<ActionResult> RenderImage(int id)
        {
            var item = await _db.Items.FindAsync(id);

            byte[] photoBack = item.InternalImage;

            return File(photoBack, "image/png");
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

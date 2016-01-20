using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.data;

namespace  Cfcusaga.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        private PortalDbContext db = new PortalDbContext();

        // GET: Catagories
        public async Task<ActionResult> Index()
        {
            //return View(await db.Events.ToListAsync());
            return View(await db.Events.ToListAsync());
        }

        // GET: Catagories/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catagorie = await db.Events.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
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
        public async Task<ActionResult> Create([Bind(Include = "ID,Name")] cfcusaga.data.Event catagorie)
        {
            if (ModelState.IsValid)
            {
                db.Events.Add(catagorie);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(catagorie);
        }

        // GET: Catagories/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catagorie = await db.Events.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
        }

        // POST: Catagories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,Name")] Catagory catagorie)
        {
            if (ModelState.IsValid)
            {
                db.Entry(catagorie).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(catagorie);
        }

        // GET: Catagories/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var catagorie = await db.Events.FindAsync(id);
            if (catagorie == null)
            {
                return HttpNotFound();
            }
            return View(catagorie);
        }

        // POST: Catagories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var catagorie = await db.Events.FindAsync(id);
            db.Events.Remove(catagorie);
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

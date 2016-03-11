using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cfcusaga.data;
using Microsoft.AspNet.Identity;
using Member = cfcusaga.domain.Membership.Member;

namespace Cfcusaga.Web.Controllers
{
    public class EventRegistrationsController : Controller
    {
        private PortalDbContext db = new PortalDbContext();

        // GET: EventRegistrations
        public async Task<ActionResult> Index()
        {
            var currentUserId = User.Identity.GetUserId();
            var member = await GetMemberInfoFromAspNetUserId(currentUserId);
            var eventRegistrations = db.EventRegistrations.Include(e => e.Event).Include(e => e.Member).Include(e => e.Order).Where(e => e.Order.MemberId == member.Id);
            return View(await eventRegistrations.ToListAsync());
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


        // GET: EventRegistrations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var eventRegistration = await db.EventRegistrations.FindAsync(id);
            if (eventRegistration == null)
            {
                return HttpNotFound();
            }
            return View(eventRegistration);
        }

        // GET: EventRegistrations/Create
        public ActionResult Create()
        {
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name");
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName");
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username");
            return View();
        }

        // POST: EventRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,EventId,OrderId,MemberId,CreationDate,ModifiedDate")] EventRegistration eventRegistration)
        {
            if (ModelState.IsValid)
            {
                db.EventRegistrations.Add(eventRegistration);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", eventRegistration.EventId);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", eventRegistration.MemberId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", eventRegistration.OrderId);
            return View(eventRegistration);
        }

        // GET: EventRegistrations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRegistration eventRegistration = await db.EventRegistrations.FindAsync(id);
            if (eventRegistration == null)
            {
                return HttpNotFound();
            }
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", eventRegistration.EventId);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", eventRegistration.MemberId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", eventRegistration.OrderId);
            return View(eventRegistration);
        }

        // POST: EventRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,EventId,OrderId,MemberId,CreationDate,ModifiedDate")] EventRegistration eventRegistration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventRegistration).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.EventId = new SelectList(db.Events, "Id", "Name", eventRegistration.EventId);
            ViewBag.MemberId = new SelectList(db.Members, "Id", "FirstName", eventRegistration.MemberId);
            ViewBag.OrderId = new SelectList(db.Orders, "OrderId", "Username", eventRegistration.OrderId);
            return View(eventRegistration);
        }

        // GET: EventRegistrations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventRegistration eventRegistration = await db.EventRegistrations.FindAsync(id);
            if (eventRegistration == null)
            {
                return HttpNotFound();
            }
            return View(eventRegistration);
        }

        // POST: EventRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            EventRegistration eventRegistration = await db.EventRegistrations.FindAsync(id);
            db.EventRegistrations.Remove(eventRegistration);
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

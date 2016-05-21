using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Orders;
using PagedList;
using Order = cfcusaga.data.Order;

namespace Cfcusaga.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IShoppingCartService _svc;
        private PortalDbContext db = new PortalDbContext();

        public OrdersController(IShoppingCartService svc)
        {
            _svc = svc;
        }

        // GET: Orders
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            InitIndexViewBag(sortOrder);

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                if (Request.UrlReferrer != null && Request.UrlReferrer.ToString().Contains("Details"))
                {
                    var sessionFilter = this.HttpContext.Session?["Orders.Filter"];
                    searchString = sessionFilter as string;

                    var sessionSort = this.HttpContext.Session?["Orders.SortBy"];
                    sortOrder = sessionSort as string;
                }
                else
                {
                    searchString = currentFilter;
                }
                //return o as CurrentRegistrationInfo;
                
            }

            ViewBag.CurrentFilter = searchString;

            var orders = from o in db.Orders
                        select o;

            orders = FilterBy(searchString, orders);
            orders = SortItemsBy(sortOrder, orders);

            var httpSessionStateBase = this.HttpContext.Session;
            if (httpSessionStateBase != null) httpSessionStateBase["Orders.Filter"] = searchString;
            if (httpSessionStateBase != null) httpSessionStateBase["Orders.SortBy"] = sortOrder;

            int pageSize = 20;
            int pageNumber = (page ?? 1);
            return View(orders.ToPagedList(pageNumber, pageSize));

            //return View(await db.Orders.ToListAsync());
        }

        private void InitIndexViewBag(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrderIdSortParm = sortOrder == "OrderId" ? "OrderId_desc" : "OrderId";
            ViewBag.OrderDateSortParm = sortOrder == "OrderDate" ? "OrderDate_desc" : "OrderDate";
            ViewBag.FirstNameSortParm = sortOrder == "Firstname" ? "Firstname_desc" : "Firstname";
            ViewBag.LastNameSortParm = sortOrder == "Lastname" ? "Lastname_desc" : "Lastname";
            ViewBag.TotalSortParm = sortOrder == "Total" ? "Total_desc" : "Total";

            ViewBag.CitySortParm = sortOrder == "City" ? "City_desc" : "City";
            ViewBag.StateSortParm = sortOrder == "State" ? "State_desc" : "State";
            ViewBag.ZipCodeSortParm = sortOrder == "Zip" ? "Zip_desc" : "Zip";
            ViewBag.OrderStatusSortParm = sortOrder == "OrderStatus" ? "OrderStatus_desc" : "OrderStatus";
        }

        private static IQueryable<Order> SortItemsBy(string sortOrder, IQueryable<Order> orders)
        {
            switch (sortOrder)
            {
                case "OrderId":
                    orders = orders.OrderBy(s => s.OrderId);
                    break;
                case "OrderId_desc":
                    orders = orders.OrderByDescending(s => s.OrderId);
                    break;
                case "OrderDate":
                    orders = orders.OrderBy(s => s.OrderDate);
                    break;
                case "OrderDate_desc":
                    orders = orders.OrderByDescending(s => s.OrderDate);
                    break;
                case "Firstname_desc":
                    orders = orders.OrderByDescending(s => s.FirstName);
                    break;
                case "Firstname":
                    orders = orders.OrderBy(s => s.FirstName);
                    break;
                case "Lastname":
                    orders = orders.OrderBy(s => s.LastName);
                    break;
                case "Lastname_desc":
                    orders = orders.OrderByDescending(s => s.LastName);
                    break;
                case "Total":
                    orders = orders.OrderBy(s => s.Total);
                    break;
                case "Total_desc":
                    orders = orders.OrderByDescending(s => s.Total);
                    break;
                case "City":
                    orders = orders.OrderBy(s => s.City);
                    break;
                case "City_desc":
                    orders = orders.OrderByDescending(s => s.City);
                    break;
                case "State":
                    orders = orders.OrderBy(s => s.State);
                    break;
                case "State_desc":
                    orders = orders.OrderByDescending(s => s.State);
                    break;
                case "Zip":
                    orders = orders.OrderBy(s => s.PostalCode);
                    break;
                case "Zip_desc":
                    orders = orders.OrderByDescending(s => s.PostalCode);
                    break;
                case "OrderStatus":
                    orders = orders.OrderBy(s => s.OrderStatus.Name);
                    break;
                case "OrderStatus_desc":
                    orders = orders.OrderByDescending(s => s.OrderStatus.Name);
                    break;
                default: // Name ascending 
                    orders = orders.OrderByDescending(s => s.OrderId);
                    break;
            }
            return orders;
        }

        private static IQueryable<Order> FilterBy(string searchString, IQueryable<Order> orders)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                var statesLists = new List<string> { "AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DC", "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "MA", "MD", "ME", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY" };
                if (searchString.Length == 2 && statesLists.Contains(searchString.ToUpper()))
                {
                    orders = orders.Where(s => s.State.ToString().Contains(searchString.ToUpper()));
                }
                else
                {
                    orders = orders.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                               || s.OrderId.ToString().Contains(searchString.ToUpper())
                                               || s.FirstName.ToString().Contains(searchString.ToUpper())
                                               || s.City.ToString().Contains(searchString.ToUpper())
                                               || s.State.ToString().Contains(searchString.ToUpper())
                                               || s.OrderStatus.Name.ToString().Contains(searchString.ToUpper())
                        );
                }

            }
            return orders;
        }

        // GET: Orders/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var order = await db.Orders.FindAsync(id);
            var orderDetails = db.OrderDetails.Where(x => x.OrderId == id );
            var orderDiscounts = db.OrderDiscounts.Include(x => x.Order).Include(y => y.Discount1).Where(x => x.OrderId == id);

            order.OrderDetails = await orderDetails.ToListAsync();
            order.OrderDiscounts = await orderDiscounts.ToListAsync();

            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
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

        // GET: Orders/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var order = await db.Orders.FindAsync(id);
            var order = await _svc.GetOrderById(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.OrderStatusID = new SelectList(_svc.GetOrderStatuses(), "ID", "Name");
            return View(order);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(cfcusaga.domain.Orders.Order order)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(order).State = EntityState.Modified;
                //await db.SaveChangesAsync();
                await _svc.SaveOrder(order);
                return RedirectToAction("Details", new { id = order.OrderId });
                //return RedirectToAction("Register", "Items", new {eventId = id});
            }
            ViewBag.OrderStatusID = new SelectList(_svc.GetOrderStatuses(), "ID", "Name");
            return View(order);
        }

        // GET: Orders/Delete/5
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

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var order = await db.Orders.FindAsync(id);
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

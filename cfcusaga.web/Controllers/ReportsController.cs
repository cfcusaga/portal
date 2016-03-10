using System.Linq;
using System.Threading.Tasks;
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
        public async Task<ActionResult> Index(int? page, string sortOrder, string currentFilter, string searchString)
        {
            var eventId = 7;

            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
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

            var orders = from o in _db.Orders.AsNoTracking()
                         join od in _db.OrderDetails.AsNoTracking() on o.OrderId equals od.OrderId
                         join i in _db.Items.AsNoTracking() on od.ItemId equals i.ID
                         join e in _db.Events.AsNoTracking() on i.EventId equals e.Id
                         where e.Id == eventId
                         orderby o.OrderId descending 
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
            var pageSize = 10;
            var pageNumber = (page ?? 1);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize));

        }

    }
}

using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Helpers;
using Cfcusaga.Web.Models;
using Enums = cfcusaga.domain.Enums;

namespace Cfcusaga.Web.Controllers
{
   
    public class ReportsController : Controller
    {
        private PortalDbContext _db = new PortalDbContext();

        // GET: ReportOrders
        [Authorize(Roles = "SuperUser, Admin")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            const int eventId = 7;
            InitIndexViewBag(sortOrder);
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
                         //orderby o.OrderId descending 
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
                             BirthDate = od.BirthDate,
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

            orders = FilterItemsBy(searchString, orders);
            orders = SortItemsBy(sortOrder, orders);
            const int pageSize = 30;
            var pageNumber = (page ?? 1);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize));

        }

        [AllowAnonymous]
        [HttpGet()]
        [Route("registrations")]
        public async Task<ActionResult> Registrations(string sortOrder, string currentFilter, string searchString, int? page)
        {
            const int eventId = 7;
            InitIndexViewBag(sortOrder);
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
                         && i.CatagoryID == (int)CategoryTypeEnum.Registration
                         //orderby o.OrderId descending 
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
                             BirthDate = od.BirthDate,
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

            orders = FilterItemsBy(searchString, orders);
            orders = SortItemsBy(sortOrder, orders);
            const int pageSize = 50;
            var pageNumber = (page ?? 1);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize));

        }

        private void InitIndexViewBag(string sortOrder)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrderIdSortParm = sortOrder == "OrderId" ? "OrderId_desc" : "OrderId";
            ViewBag.OrderDateSortParm = sortOrder == "OrderDate" ? "OrderDate_desc" : "OrderDate";
            ViewBag.ItemNameSortParm = sortOrder == "ItemName" ? "ItemName_desc" : "ItemName";
            ViewBag.FullNameSortParm = sortOrder == "FullName" ? "FullName_desc" : "FullName";
            ViewBag.AgeOnEventDateSortParm = sortOrder == "AgeOnEventDate" ? "AgeOnEventDate_desc" : "AgeOnEventDate";

            ViewBag.TshirtSortParm = sortOrder == "Tshirt" ? "Tshirt_desc" : "Tshirt";
            ViewBag.OrderedBySortParm = sortOrder == "OrderedBy" ? "OrderedBy_desc" : "OrderedBy";
            ViewBag.CitySortParm = sortOrder == "City" ? "City_desc" : "City";
            ViewBag.StateSortParm = sortOrder == "State" ? "State_desc" : "State";
            ViewBag.ZipSortParm = sortOrder == "Zip" ? "Zip_desc" : "Zip";
        }

        private static IQueryable<OrderItems> FilterItemsBy(string searchString, IQueryable<OrderItems> orders)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(s => s.Lastname.ToUpper().Contains(searchString.ToUpper())
                                           || s.OrderId.ToString().Contains(searchString.ToUpper())
                                           || s.Firstname.ToString().Contains(searchString.ToUpper())
                                           || s.City.ToString().Contains(searchString.ToUpper())
                                           || s.State.ToString().Contains(searchString.ToUpper())
                                           || s.ItemName.ToString().Contains(searchString.ToUpper())
                                           || s.TshirtSize.ToString().Contains(searchString.ToUpper())
                    );
            }
            return orders;
        }

        private IQueryable<OrderItems> SortItemsBy(string sortOrder, IQueryable<OrderItems> orders)
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
                case "ItemName":
                    orders = orders.OrderBy(s => s.ItemName);
                    break;
                case "ItemName_desc":
                    orders = orders.OrderByDescending(s => s.ItemName);
                    break;
                case "FullName":
                    orders = orders.OrderBy(s => s.Lastname).ThenBy(f => f.Firstname);
                    break;
                case "FullName_desc":
                    orders = orders.OrderByDescending(s => s.Lastname).ThenBy(f => f.Firstname);
                    break;
                case "Age":
                    orders = orders.OrderBy(s => s.BirthDate);
                    break;
                case "Age_desc":
                    orders = orders.OrderByDescending(s => s.BirthDate);
                    break;
                case "Tshirt":
                    orders = orders.OrderBy(s => s.TshirtSize);
                    break;
                case "Tshirt_desc":
                    orders = orders.OrderByDescending(s => s.TshirtSize);
                    break;
                case "OrderedBy":
                    orders = orders.OrderBy(s => s.OrderByLastname).ThenBy(f => f.OrderByFirstname);
                    break;
                case "OrderedBy_desc":
                    orders = orders.OrderByDescending(s => s.OrderByLastname).ThenBy(f => f.OrderByFirstname);
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
                    orders = orders.OrderBy(s => s.ZipCode);
                    break;
                case "Zip_desc":
                    orders = orders.OrderByDescending(s => s.ZipCode);
                    break;
                default: // Name ascending 
                    orders = orders.OrderByDescending(s => s.OrderId);
                    break;
            }
            return orders;
        }
    }
}

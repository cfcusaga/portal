using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using cfcusaga.data;
using cfcusaga.domain.Helpers;
using Cfcusaga.Web.Models;
using Enums = cfcusaga.domain.Enums;

namespace Cfcusaga.Web.Controllers
{
   
    public class ReportsController : Controller
    {
        private PortalDbContext _db = new PortalDbContext();

        // GET: ReportOrders test
        [Authorize(Roles = "SuperUser, Admin")]
        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
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

            var orders = RetrieveReportIndexItems(sortOrder, searchString);
            const int pageSize = 30;
            var pageNumber = (page ?? 1);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize));

        }

        private IQueryable<OrderItems> RetrieveReportIndexItems(string sortOrder, string searchString)
        {
            const int eventId = 7;
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
                    OrderTotal = o.Total,
                    OrderDetailId = od.OrderDetailId,
                    ItemId = i.ID,
                    ItemName = i.Name,
                    CategoryId = i.CatagoryID,
                    Firstname = od.Firstname,
                    Lastname = od.Lastname,
                    BirthDate = od.BirthDate,
                    Gender = od.Gender,
                    OrderByLastname = o.LastName,
                    OrderByFirstname = o.FirstName,
                    TshirtSize = od.TshirtSize,
                    Allergies = od.Allergies,
                    City = o.City,
                    State = o.State,
                    ZipCode = o.PostalCode,
                    Phone = o.Phone,
                    Email = o.Email,
                    Price = od.UnitPrice,
                    Address = o.Address,
                }
                ;

            orders = FilterItemsBy(searchString, orders);
            orders = SortItemsBy(sortOrder, orders);
            return orders;
        }

        [AllowAnonymous]
        [HttpGet()]
        //[Route("registrations")]
        public async Task<ActionResult> Registrations(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
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

            var orders = RetrieveOrderDetailsReportItems(sortOrder, searchString);
            const int pageSize = 50;
            var pageNumber = (page ?? 1);
            return View(await orders.ToPagedListAsync(pageNumber, pageSize));

        }


        [Authorize(Roles = "SuperUser, Admin")]
        public void ExportClientsListToExcel(string sortOrder, string currentFilter)
        {
            var grid = new System.Web.UI.WebControls.GridView();

            var reportItems = RetrieveOrderDetailsReportItems(sortOrder, currentFilter);
            grid.DataSource = reportItems.ToList();
            
            grid.DataBind();

            Response.ClearContent();
            Response.AddHeader("content-disposition", "attachment; filename=Exported_Diners.xls");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();

        }

        private IQueryable<OrderItems> RetrieveOrderDetailsReportItems(string sortOrder, string searchString)
        {
            const int eventId = 7;
            var orders = from o in _db.Orders.AsNoTracking()
                join od in _db.OrderDetails.AsNoTracking() on o.OrderId equals od.OrderId
                join i in _db.Items.AsNoTracking() on od.ItemId equals i.ID
                join e in _db.Events.AsNoTracking() on i.EventId equals e.Id
                where e.Id == eventId
                      && i.CatagoryID == (int) CategoryTypeEnum.Registration
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
            return orders;
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
                                           || s.ZipCode.ToString().Contains(searchString.ToUpper())
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

        [Authorize(Roles = "SuperUser, Admin")]
        public void DownloadReportIndex(string sortOrder, string currentFilter)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            grid.AutoGenerateColumns = false;
            var reportItems = RetrieveReportIndexItems(sortOrder, currentFilter);
            grid.DataSource = reportItems.Select(o => new ReportIndexReport()
            {
                CheckNumber = "",
                OrderId = o.OrderId,
                OrderDate = o.OrderDate,
                OrderTotal = o.OrderTotal,
                OrderByLastname = o.OrderByLastname,
                OrderByFirstname = o.OrderByFirstname,
                ItemName = o.ItemName,
                ItemPrice = o.Price,
                ItemId = o.ItemId,
                Lastname = o.Lastname,
                Firstname = o.Firstname,
                Gender = o.Gender,
                DateOfBirth = o.BirthDate,
                EventDate = new DateTime(2016, 6, 18),
                Allergies = o.Allergies,
                TshirtSize = o.TshirtSize,
                Phone = o.Phone,
                Email = o.Email,
                Address = o.Address,
                City = o.City,
                State = o.State,
                Zip = o.ZipCode,
            }).ToList();
            grid.Columns.Add(new BoundField() { DataField = "CheckNumber", HeaderText = "CheckNumber" });
            grid.Columns.Add(new BoundField() { DataField = "OrderId", HeaderText = "OrderId" });
            grid.Columns.Add(new BoundField() { DataField = "OrderDate", HeaderText = "OrderDate", DataFormatString = "{0:d}" });
            //grid.Columns.Add(new BoundField() { DataField = "OrderedBy", HeaderText = "OrderedBy" });
            //grid.Columns.Add(new TemplateField() {HeaderText = ""}); 
            grid.Columns.Add(new BoundField() { DataField = "Lastname", HeaderText = "Lastname" });
            grid.Columns.Add(new BoundField() { DataField = "Firstname", HeaderText = "Firstname" });
            grid.Columns.Add(new BoundField() { DataField = "Gender", HeaderText = "Gender" });
            grid.Columns.Add(new BoundField() { DataField = "DateOfBirth", HeaderText = "DateOfBirth", DataFormatString = "{0:d}" });
            grid.Columns.Add(new BoundField() { DataField = "AgeOnEventDate", HeaderText = "AgeDuringEvent" });
            grid.Columns.Add(new BoundField() { DataField = "Allergies", HeaderText = "Allergies" });
            grid.Columns.Add(new BoundField() { DataField = "TshirtSize", HeaderText = "TshirtSize" });
            grid.Columns.Add(new BoundField() { DataField = "ItemPrice", HeaderText = "ItemPrice" });
            grid.Columns.Add(new BoundField() { DataField = "ItemType", HeaderText = "ItemType" });
            grid.Columns.Add(new BoundField() { DataField = "OrderTotal", HeaderText = "OrderTotal" });

            //grid.Columns.Add(new BoundField() { DataField = "ItemName", HeaderText = "ItemName" });


            grid.Columns.Add(new BoundField() { DataField = "Phone", HeaderText = "Phone" });
            grid.Columns.Add(new BoundField() { DataField = "Email", HeaderText = "Email" });
            grid.Columns.Add(new BoundField() { DataField = "Address", HeaderText = "Address" });
            grid.Columns.Add(new BoundField() { DataField = "City", HeaderText = "City" });
            grid.Columns.Add(new BoundField() { DataField = "State", HeaderText = "State" });
            grid.Columns.Add(new BoundField() { DataField = "Zip", HeaderText = "Zip" });

            grid.RowDataBound += GridView1_RowDataBound;
            grid.RowCreated += GridView1_RowCreated;
            grid.DataBind();

            Response.ClearContent();
            var fileName = $"2016CfcKidsFamilyConfRegistrationExport{DateTime.Now.ToString("MM-dd-yyyy")}.xls";
            Response.AddHeader("content-disposition", $"attachment; filename={fileName}");
            Response.ContentType = "application/excel";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);

            Response.Write(sw.ToString());

            Response.End();
        }

        private int orderId;
        private int rowIndex = 1;
        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                orderId = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "OrderId").ToString());
            }
        }

        protected void GridView1_RowCreated(object sender, GridViewRowEventArgs e)
        {
            bool newRow = false;
            if ((orderId > 0) && (DataBinder.Eval(e.Row.DataItem, "OrderId") != null))
            {
                if (orderId != Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "OrderId").ToString()))
                    newRow = true;
            }
            if ((orderId > 0) && (DataBinder.Eval(e.Row.DataItem, "OrderId") == null))
            {
                newRow = true;
                rowIndex = 0;
            }
            if (newRow)
            {
                AddNewRow(sender, e);
            }
        }
        public void AddNewRow(object sender, GridViewRowEventArgs e)
        {
            GridView GridView1 = (GridView)sender;
            GridViewRow NewTotalRow = new GridViewRow(0, 0, DataControlRowType.DataRow, DataControlRowState.Insert);
            NewTotalRow.Font.Bold = true;
            NewTotalRow.BackColor = System.Drawing.Color.Aqua;
            TableCell HeaderCell = new TableCell();
            HeaderCell.Height = 10;
            HeaderCell.HorizontalAlign = HorizontalAlign.Center;
            HeaderCell.ColumnSpan = 19;
            NewTotalRow.Cells.Add(HeaderCell);
            GridView1.Controls[0].Controls.AddAt(e.Row.RowIndex + rowIndex, NewTotalRow);
            rowIndex++;
        }
    }

    public class ReportIndexReport
    {
        public string CheckNumber { get; set; }
        public int OrderId { get; set; }

        public string OrderedBy => $"{OrderByLastname},{OrderByFirstname}";

        [Browsable(false)]
        public object OrderByFirstname { get; set; }

        [Browsable(false)]
        public object OrderByLastname { get; set; }

        [Browsable(false)]
        public DateTime OrderDate { get; set; }
        public string TshirtSize { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public decimal OrderTotal { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        //public string RegistrationType { get; set; }
        public string Lastname { get; set; }
        public string Firstname { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Allergies { get; set; }
        public string Address { get; set; }

        [Browsable(false)]
        public int ItemId { get; set; }

        public int? AgeOnEventDate => DateOfBirth?.Age(EventDate);

        [Browsable(false)]
        public DateTime EventDate { get; set; }

        public string ItemType
        {
            get
            {
                switch (ItemId)
                {
                    case 3:
                        return "Kids";
                    case 4:
                        return "Youth";
                    case 5:
                        return "Parent";
                    case 6:
                        return "Room";
                    case 8:
                        return "BedSpace";
                    case 7:
                        return "TShirt";
                    default:
                        return "";
                }
            }
        }
    }

}

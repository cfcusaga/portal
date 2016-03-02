using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Events;
using cfcusaga.domain.Orders;
using Cfcusaga.Web.Extensions;
using Cfcusaga.Web.Models;
using Cfcusaga.Web.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Enums = cfcusaga.domain.Enums;
using Member = cfcusaga.domain.Membership.Member;

namespace Cfcusaga.Web.Controllers
{
    public class CurrentRegistrationInfo
    {
        public string Lastname { get; set; }
        public string Firsname { get; set; }
        public bool IsSelfSelected { get; set; }
        public string Gender { get; set; }
    }
    public class ItemRegistrationsController : Controller
    {
        private const string CurrentRegistrationInfoKey = "CurrentRegistrationInfo";

        public static CurrentRegistrationInfo GetSessionCurrentRegistrationInfo(HttpContextBase context)
        {
            var o = context.Session?[CurrentRegistrationInfoKey];
            return o as CurrentRegistrationInfo;
        }

        public static void SetSessionCurrentRegistrationInfo(HttpContextBase httpContext, string lastname, string firstname)
        {
            var httpSessionStateBase = httpContext.Session;
            var info = new CurrentRegistrationInfo()
            {
                Lastname = lastname,
                Firsname = firstname,
                IsSelfSelected = false,
                Gender = string.Empty
            };
            if (httpSessionStateBase != null) httpSessionStateBase[CurrentRegistrationInfoKey] = info;
        }

        public static void SetSessionCurrentRegistrationInfo(HttpContextBase httpContext, string lastname, string firstname, bool isSelf, string gender)
        {
            var httpSessionStateBase = httpContext.Session;
            var info = new CurrentRegistrationInfo()
            {
                Lastname = lastname,
                Firsname = firstname,
                IsSelfSelected =  true,
                Gender = gender
            };
            if (httpSessionStateBase != null) httpSessionStateBase[CurrentRegistrationInfoKey] = info;
        }

        public static List<SelectListItem> GetTShirtSizesList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem() { Text = "Adult Small", Value = "AdultSmall" });
            list.Add(new SelectListItem() { Text = "Adult Medium", Value = "AdultMedium" });
            list.Add(new SelectListItem() { Text = "Adult Large", Value = "AdultLarge" });
            list.Add(new SelectListItem() { Text = "Adult X-Large", Value = "AdultXLarge" });
            return list;
        }


        private readonly IEventServices _svc;
        private readonly IShoppingCartService _cartSvc;
        private readonly PortalDbContext _db;



        public ItemRegistrationsController()
        {
        }

        //public AccountController(ApplicationUserManager userManager)
        //{
        //    UserManager = userManager;
        //}

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        public ItemRegistrationsController(PortalDbContext db, IEventServices svc, IShoppingCartService cartSvc, ApplicationUserManager userManager)
        {
            _db = db;
            _svc = svc;
            _cartSvc = cartSvc;
            UserManager = userManager;
            //}
        }

        // GET: ItemRegistrations
        public async Task<ActionResult> Index()
        {
            return View(await _db.CartItemRegistrations.ToListAsync());
        }

        // GET: ItemRegistrations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var itemRegistration = await _db.CartItemRegistrations.FindAsync(id);
            if (itemRegistration == null)
            {
                return HttpNotFound();
            }
            var eventId = EventsController.GetSessionEventId(this.HttpContext);

            if (eventId == null)
            {
                if (itemRegistration.Cart.Item.EventId != null)
                {
                    eventId = itemRegistration.Cart.Item.EventId;
                    EventsController.SetSessionEventId(this.HttpContext, eventId.Value);
                }

            }

            var anEvent = await _svc.GetEventDetails(eventId);
            ViewBag.Title = anEvent.Name;

            var cartItem = await _db.Carts.FindAsync(itemRegistration.CartID);

            var item = await _db.Items.FindAsync(cartItem.ItemId);
            ViewBag.IsShirtIncluded = item.IsShirtIncluded ?? false;
            ViewBag.IsRequireBirthDate = item.IsRequireBirthDateInfo ?? false;
            ViewBag.SubTitle = item.Name;


            return View(itemRegistration);
        }

        // GET: ItemRegistrations/Create
        public async Task<ActionResult> Create(int itemId)
        {
            //ViewBag.RelationToMemberTypeId = new SelectList(_svc.GetRelationToMemberTypes(), "ID", "Name");

            var eventId= EventsController.GetSessionEventId(this.HttpContext);
            var anEvent = await _svc.GetEventDetails(eventId);
            ViewBag.Title = anEvent.Name;
            var item = await _db.Items.FindAsync(itemId);
            ViewBag.SubTitle = item.Name;
            ViewBag.ItemId = itemId;
            ViewBag.IsRequireBirthDate = item.IsRequireBirthDateInfo ?? false;
            ViewBag.IsRequireParentWaiver = item.IsRequireParentWaiver ?? false;
            ViewBag.IsShirtIncluded = item.IsShirtIncluded ?? false;

            var list = GetTShirtSizesList();
            ViewBag.TshirtSizes = list;


            var sessionInfo = ItemRegistrationsController.GetSessionCurrentRegistrationInfo(this.HttpContext);
            var newRegistratinItem = new ItemRegistrationModel();
            if (item.IsRequireParentWaiver != null && item.IsRequireParentWaiver.Value)
            {
                var relationTypes = _svc.GetRelationToMemberTypesRequiresParentWaiver();
                newRegistratinItem.RelationToMemberTypes = new SelectList(relationTypes, "Id", "Name");
            }
            else
            {
                var relationTypes = _svc.GetRelationToMemberTypesAdults().ToList();
                IEnumerable<RelationToMemberType> selectListItems = null;
                //newRegistratinItem.RelationToMemberTypes = new SelectList(relationTypes, "Id", "Name");
                if (sessionInfo != null && sessionInfo.IsSelfSelected)
                {
                    selectListItems = relationTypes.Where(m => m.Name.ToUpper() != "SELF");
                    newRegistratinItem.Gender = sessionInfo.Gender=="F"?"M":"F";//TODO: need to make gender an Enum
                }
                else
                {
                    selectListItems = relationTypes.ToList();
                }
                var relationTypesList = new SelectList(selectListItems, "Id", "Name");
                newRegistratinItem.RelationToMemberTypes = relationTypesList;
            }

            
            if (sessionInfo != null)
            {
                newRegistratinItem.LastName = sessionInfo.Lastname;
            }

            return View(newRegistratinItem);
        }



        // POST: ItemRegistrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,CartID,MemberId,LastName,FirstName,BirthDate,RelationToMemberTypeId,Gender,Notes,Allergies,TshirtSize")] CartItemRegistration itemRegistration)
        {
            if (ModelState.IsValid)
            {
                //TODO:
                var itemId = Convert.ToInt32(Request.QueryString["itemId"]);

                var foundItem = await _cartSvc.GetItem(itemId);

                var cart = ShoppingCart.GetCart(this.HttpContext, _cartSvc);
                int count;
                var anItem =  cart.AddToCart(foundItem, out count);

                ItemRegistrationsController.SetSessionCurrentRegistrationInfo(this.HttpContext, itemRegistration.LastName, string.Empty);
                //var eventId = EventsController.GetSessionEventId(this.HttpContext);

                itemRegistration.CartID = anItem.Id;

                if (itemRegistration.RelationToMemberTypeId != null && itemRegistration.RelationToMemberTypeId == (int)Enums.RelationToMe.Self)
                {
                    ItemRegistrationsController.SetSessionCurrentRegistrationInfo(this.HttpContext, itemRegistration.LastName, itemRegistration.FirstName, true, itemRegistration.Gender);
                    var user = await UserManager.FindByIdAsync(User.Identity.GetUserId());
                    if (user != null)
                    {
                        var aMember = await GetMember(user.Id);
                        if (aMember != null)
                        {
                            itemRegistration.MemberId = aMember.Id;
                            itemRegistration.LastName = aMember.LastName;
                            itemRegistration.FirstName = aMember.Firstname;
                            itemRegistration.Gender = aMember.Gender;
                        }
                    }
                }


                _db.CartItemRegistrations.Add(itemRegistration);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Items" );
            }

            return View();
        }

        private async Task<Member> GetMember(string id)
        {
            var entity = await _db.Members.FirstOrDefaultAsync(m => m.AspNetUserId == id);
            if (entity != null)
            {
                return new Member
                {
                    Id = entity.Id,
                    LastName =  entity.LastName,
                    Firstname =  entity.FirstName,
                    Gender = entity.Gender,
                    BirthDate = entity.BirthDate,
                    Email = entity.Email,
                    Phone = entity.Phone
                };
            }
            return null;
        }

        // GET: ItemRegistrations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var itemRegistration = await _db.CartItemRegistrations.FindAsync(id);
            if (itemRegistration == null)
            {
                return HttpNotFound();
            }

            //ViewBag.RelationToMemberTypeId = new SelectList(_svc.GetRelationToMemberTypes(), "Id", "Name");

            var eventId = EventsController.GetSessionEventId(this.HttpContext);
            if (eventId == null)
            {
                if (itemRegistration.Cart.Item.EventId != null)
                {
                    eventId = itemRegistration.Cart.Item.EventId;
                    EventsController.SetSessionEventId(this.HttpContext, eventId.Value);
                }

            }
            var anEvent = await _svc.GetEventDetails(eventId);
            ViewBag.Title = anEvent.Name;
            var cartItem = await _db.Carts.FindAsync(itemRegistration.CartID);

            var item = await _db.Items.FindAsync(cartItem.ItemId);
            ViewBag.SubTitle = item.Name;
            ViewBag.ItemId = item.ID;
            ViewBag.IsRequireBirthDate = item.IsRequireBirthDateInfo ?? false;
            ViewBag.IsRequireParentWaiver = item.IsRequireParentWaiver ?? false;
            ViewBag.IsShirtIncluded = item.IsShirtIncluded ?? false;

            var sessionInfo = ItemRegistrationsController.GetSessionCurrentRegistrationInfo(this.HttpContext);
            //IEnumerable relationTypes;
            SelectList relationTypesList = null;
            if (item.IsRequireParentWaiver != null && item.IsRequireParentWaiver.Value)
            {
                var relationTypes = _svc.GetRelationToMemberTypesRequiresParentWaiver();
                relationTypesList = new SelectList(relationTypes, "Id", "Name");
            }
            else
            {
                 //relationTypes = _svc.GetRelationToMemberTypesAdults();
                var relationTypes = _svc.GetRelationToMemberTypesAdults().ToList();
                IEnumerable<RelationToMemberType> selectListItems = null;
                //newRegistratinItem.RelationToMemberTypes = new SelectList(relationTypes, "Id", "Name");
                if (sessionInfo != null && sessionInfo.IsSelfSelected)
                {
                    selectListItems = relationTypes.Where(m => m.Name.ToUpper() != "SELF");
                }
                else
                {
                    selectListItems = relationTypes.ToList();
                }
                relationTypesList = new SelectList(selectListItems, "Id", "Name");
            }

            var list = GetTShirtSizesList();
            ViewBag.TshirtSizes = list;

            var itemRegModel = new ItemRegistrationModel()
            {
                LastName = itemRegistration.LastName,
                MemberId = itemRegistration.MemberId,
                FirstName = itemRegistration.FirstName,
                BirthDate = itemRegistration.BirthDate,
                RelationToMemberTypeId = itemRegistration.RelationToMemberTypeId,
                Gender = itemRegistration.Gender,
                TshirtSize = itemRegistration.TshirtSize,
                Allergies = itemRegistration.Allergies,
                ID = itemRegistration.ID,
                CartID = itemRegistration.CartID,
                RelationToMemberTypes = relationTypesList //new SelectList(relationTypes, "Id", "Name")
        };

            return View(itemRegModel);
        }

        // POST: ItemRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CartID,MemberId,LastName,FirstName,BirthDate,RelationToMemberTypeId,Gender,Allergies,TshirtSize")] ItemRegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                var cartItem = await _db.Carts.FindAsync(model.CartID);
                var item = await _db.Items.FindAsync(cartItem.ItemId);

                var entity = _db.CartItemRegistrations.Find(model.ID);

                entity.LastName = model.LastName;
                entity.FirstName = model.FirstName;
                if (item.IsRequireBirthDateInfo.HasValue && item.IsRequireBirthDateInfo.Value)
                {
                    entity.BirthDate = model.BirthDate;
                }
                
                entity.RelationToMemberTypeId = model.RelationToMemberTypeId;
                entity.Gender = model.Gender;
                entity.Allergies = model.Allergies;
                if (model.TshirtSize != null)
                {
                    entity.TshirtSize = model.TshirtSize;
                }
                _db.Entry(entity).State = EntityState.Modified;

                await _db.SaveChangesAsync();
                return RedirectToAction("Index", "ShoppingCart");
            }
            return View(model);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,CartID,MemberId,LastName,FirstName,BirthDate,RelationToMemberType,Gender,Notes,Allergies")] CartItemRegistration itemRegistration)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Entry(itemRegistration).State = EntityState.Modified;
        //        await _db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(itemRegistration);
        //}

        // GET: ItemRegistrations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var itemRegistration = await _db.CartItemRegistrations.FindAsync(id);
            if (itemRegistration == null)
            {
                return HttpNotFound();
            }
            return View(itemRegistration);
        }

        // POST: ItemRegistrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var itemRegistration = await _db.CartItemRegistrations.FindAsync(id);
            _db.CartItemRegistrations.Remove(itemRegistration);
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

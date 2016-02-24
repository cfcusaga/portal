﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain.Events;
using cfcusaga.domain.Orders;
using Cfcusaga.Web.Models;
using Cfcusaga.Web.ViewModel;
using Cfcusaga.Web.ViewModels;

namespace Cfcusaga.Web.Controllers
{
    public class ItemRegistrationsController : Controller
    {
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

        public ItemRegistrationsController(PortalDbContext db, IEventServices svc, IShoppingCartService cartSvc)
        {
            _db = db;
            _svc = svc;
            _cartSvc = cartSvc;
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
            ViewBag.SubTitle = item.Name;


            return View(itemRegistration);
        }

        // GET: ItemRegistrations/Create
        public async Task<ActionResult> Create(int itemId)
        {
            ViewBag.RelationToMemberTypeId = new SelectList(_svc.GetRelationToMemberTypes(), "ID", "Name");

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
            return View();
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
                cfcusaga.domain.Orders.Cart anItem =  cart.AddToCart(foundItem, out count);
                

                var eventId = EventsController.GetSessionEventId(this.HttpContext);
                itemRegistration.CartID = anItem.Id;
                _db.CartItemRegistrations.Add(itemRegistration);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index", "Items" );
            }

            return View(itemRegistration);
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
                RelationToMemberTypes = new SelectList(_svc.GetRelationToMemberTypes(), "Id", "Name")
        };

            return View(itemRegModel);
        }

        // POST: ItemRegistrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,CartID,MemberId,LastName,FirstName,BirthDate,RelationToMemberTypeId,Gender,Allergies")] ItemRegistrationModel model)
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

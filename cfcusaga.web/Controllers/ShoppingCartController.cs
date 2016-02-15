using Cfcusaga.Web.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain;
using Cfcusaga.Web.Models;
using Cfcusaga.Web.ViewModels;

namespace Cfcusaga.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        //PortalDbContext storeDB = new PortalDbContext();
        private readonly IShoppingCartService _svc;

        public ShoppingCartController(IShoppingCartService svc)
        {
             _svc = svc;
        }

        //
        // GET: /ShoppingCart/
        public async Task<ActionResult> Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);

            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = await cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            // Return the view
            return View(viewModel);
        }
        //
        // GET: /Store/AddToCart/5
        [HttpPost]
        public async Task<ActionResult> AddToCart(int id)
        {
            // Retrieve the item from the database

            var foundItem = await _svc.GetItem(id);

            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);
            int count = cart.AddToCart(foundItem);

            var results = new ShoppingCartRemoveViewModel()
            {
                Message = Server.HtmlEncode(foundItem.Name) +
                          " has been added to your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = count,
                DeleteId = id
            };
            return Json(results);

            //        var addedItem = storeDB.Items
            //.Single(item => item.ID == id);
            //        // Add it to the shopping cart
            //        var cart = ShoppingCart.GetCart(this.HttpContext);

            //        int count = cart.AddToCart(addedItem);

            //        // Display the confirmation message
            //        var results = new ShoppingCartRemoveViewModel
            //        {
            //            Message = Server.HtmlEncode(addedItem.Name) +
            //                " has been added to your shopping cart.",
            //            CartTotal = cart.GetTotal(),
            //            CartCount = cart.GetCount(),
            //            ItemCount = count,
            //            DeleteId = id
            //        };
            //        return Json(results);

            // Go back to the main store page for more shopping
            // return RedirectToAction("Index");
        }
        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public async Task<ActionResult> RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);

            // Get the name of the item to display confirmation

            // Get the name of the album to display confirmation
            //string itemName = storeDB.Items
            //    .Single(item => item.ID == id).Name;
            var anItem = await _svc.GetItem(id);
            string itemName = anItem.Name;
            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = "One (1) "+ Server.HtmlEncode(itemName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }
        //
        // GET: /ShoppingCart/CartSummary
        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);

            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}
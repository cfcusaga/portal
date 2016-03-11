using Cfcusaga.Web.ViewModel;
using System.Threading.Tasks;
using System.Web.Mvc;
using cfcusaga.domain.Orders;
using Cfcusaga.Web.Models;
using Cfcusaga.Web.ViewModels;

namespace Cfcusaga.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
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
                CartTotal = cart.GetTotal(),
                CartDiscounts = await cart.GetCartDiscounts()
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
        }

        [HttpPost]
        public async Task<ActionResult> AddItemToCart(cfcusaga.data.Item item)
        {
            // Retrieve the item from the database

            var foundItem = await _svc.GetItem(item.ID);
            foundItem.TshirtSize = item.TshirtSize;
            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);
            int count = cart.AddToCart(foundItem);

            return RedirectToAction("Index", "Items");
            //var results = new ShoppingCartRemoveViewModel()
            //{
            //    Message = Server.HtmlEncode(foundItem.Name) +
            //              " has been added to your shopping cart.",
            //    CartTotal = cart.GetTotal(),
            //    CartCount = cart.GetCount(),
            //    ItemCount = count,
            //    DeleteId = item.ID
            //};
            //return Json(results);
        }

        [HttpPost]
        public ActionResult UpdateItemInCart(CartItemViewModel item)
        {
            var cart = ShoppingCart.GetCart(this.HttpContext, _svc);
            var foundItem =  _svc.GetCartItem(cart.ShoppingCartId, item.Id);
            if (item.CategoryId == (int) CategoryTypeEnum.Tshirt)
            {
                if (foundItem.TshirtSize != item.TshirtSize)
                {
                    foundItem.TshirtSize = item.TshirtSize;
                    cart.UpdateCartItem(foundItem);
                }
            }

            if (item.ReferringUrl != null)
            {
                return Redirect(item.ReferringUrl.ToString());
            }
            return RedirectToAction("Index", "Items");
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
            //var anItem = await _svc.GetItem(id);
            var anItem = _svc.GetCartItem(cart.ShoppingCartId, id);

            string itemName = anItem.ItemName;
            // Remove from cart
            int itemCount = await cart.RemoveFromCart(id);

            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
                Message = "One (1) " + Server.HtmlEncode(itemName) +
                    " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }



        //[HttpPost]
        //public async Task<ActionResult> RemoveFromCart(int id)
        //{
        //    // Remove the item from the cart
        //    var cart = ShoppingCart.GetCart(this.HttpContext, _svc);

        //    // Get the name of the item to display confirmation

        //    // Get the name of the album to display confirmation
        //    //string itemName = storeDB.Items
        //    //    .Single(item => item.ID == id).Name;
        //    var anItem = await _svc.GetItem(id);
        //    string itemName = anItem.Name;
        //    // Remove from cart
        //    int itemCount = cart.RemoveFromCart(id);

        //    // Display the confirmation message
        //    var results = new ShoppingCartRemoveViewModel
        //    {
        //        Message = "One (1) "+ Server.HtmlEncode(itemName) +
        //            " has been removed from your shopping cart.",
        //        CartTotal = cart.GetTotal(),
        //        CartCount = cart.GetCount(),
        //        ItemCount = itemCount,
        //        DeleteId = id
        //    };
        //    return Json(results);
        //}
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cfcusaga.data;
using cfcusaga.domain.Orders;
using Cfcusaga.Web.Extensions;

namespace Cfcusaga.Web.Models
{
    public class ShoppingCart
    {
        
        private IShoppingCartService _svc;
        public string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context, IShoppingCartService svc)
        {
            var cart = new ShoppingCart(svc);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }
       
        public ShoppingCart(IShoppingCartService svc)
        {
            _svc = svc;
        }



        public int AddToCart(cfcusaga.domain.Events.Item item)
        {

            try
            {
                //var cartItem = _svc.GetCartItem(ShoppingCartId, item.Id);

                //if (cartItem == null)
                //{

                //    cartItem = new cfcusaga.domain.Orders.Cart
                //    {
                //        ItemId = item.Id,
                //        CartId = ShoppingCartId,
                //        Count = 1,
                //        DateCreated = DateTime.Now
                //    };
                //    _svc.AddItemToCart(cartItem);
                //    //_svc.SaveChanges();
                //}
                //else
                //{
                //    // If the item does exist in the cart, 
                //    // then add one to the quantity
                //    cartItem.Count++;
                //    _svc.AddCountToItem(ShoppingCartId, item.Id);
                //}

                var cartItem = new cfcusaga.domain.Orders.Cart
                {
                    ItemId = item.Id,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _svc.AddItemToCart(cartItem);

                //_svc.SaveChanges();
                return cartItem.Count;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public cfcusaga.domain.Orders.Cart AddToCart(cfcusaga.domain.Events.Item item, out int count)
        {

            try
            {
                //var cartItem = _svc.GetCartItem(ShoppingCartId, item.Id);

                //if (cartItem == null)
                //{

                //    cartItem = new cfcusaga.domain.Orders.Cart
                //    {
                //        ItemId = item.Id,
                //        CartId = ShoppingCartId,
                //        Count = 1,
                //        DateCreated = DateTime.Now
                //    };
                //    _svc.AddItemToCart(cartItem);
                    

                //}
                //else
                //{
                //    // If the item does exist in the cart, 
                //    // then add one to the quantity
                //    cartItem.Count++;
                //    _svc.AddCountToItem(ShoppingCartId, item.Id);
                //}
                var cartItem = new cfcusaga.domain.Orders.Cart
                {
                    ItemId = item.Id,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                _svc.AddItemToCart(cartItem);

                int id = cartItem.Id;
                count = cartItem.Count;
                return cartItem;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int RemoveFromCart(int id)
        {
            return _svc.RemoveFromCart(ShoppingCartId, id);
        }

        public void EmptyCart()
        {
            _svc.EmptyCart(ShoppingCartId);
        }

        public async Task<List<cfcusaga.domain.Orders.Cart>> GetCartItems()
        {
            return await _svc.GetCartItems(ShoppingCartId);
        }

        public int GetCount()
        {
            int? count = _svc.GetCount(ShoppingCartId);
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            decimal? total = _svc.GetTotal(ShoppingCartId);
            return total ?? decimal.Zero;
        }

        public async Task<cfcusaga.domain.Orders.Order> CreateOrder(cfcusaga.domain.Orders.Order order)
        {
            try
            {
                decimal orderTotal = 0;
                order.OrderDetails = new List<cfcusaga.domain.Orders.OrderDetail>();

                var cartItems = await GetCartItems();
                // Iterate over the items in the cart, 
                // adding the order details for each
                foreach (var item in cartItems)
                {
                    var js = item.ToJson();
                    var orderDetail = new cfcusaga.domain.Orders.OrderDetail
                    {
                        ItemId = item.ItemId,
                        OrderId = order.OrderId,
                        UnitPrice = item.ItemPrice,
                        Quantity = item.Count,
                        CartId = item.Id,
                        RegistrationDetail = js
                    };
                    // Set the order total of the shopping cart
                    orderTotal += (item.Count * item.ItemPrice);
                    order.OrderDetails.Add(orderDetail);
                    _svc.AddOrderDetails(orderDetail);
                    // _svc.RemoveItemRegistration(item.Id);
                }
                // Set the order's total to the orderTotal count
                order.Total = orderTotal;

                //TODO: Clean the 
                //order.OrderDetails

                _svc.SaveChanges();
                // Empty the shopping cart
                EmptyCart();
                // Return the OrderId as the confirmation number
                return order;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContextBase context)
        {
            if (context.Session[CartSessionKey] == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session[CartSessionKey] =
                        context.User.Identity.Name;
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session[CartSessionKey] = tempCartId.ToString();
                }
            }
            return context.Session[CartSessionKey].ToString();

        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            _svc.MigrateCart(userName, ShoppingCartId);

        }
    }
}
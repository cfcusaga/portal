﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;
using cfcusaga.domain;

namespace Cfcusaga.Web.Models
{
    public class ShoppingCart
    {
        //ApplicationDbContext storeDB = new ApplicationDbContext();
        PortalDbContext portalDB = new PortalDbContext();
        private IShoppingCartService _svc;
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context, IShoppingCartService svc)
        {
            var cart = new ShoppingCart(svc);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        //public static ShoppingCart GetCart(Controller controller)
        //{
        //    return GetCart(controller.HttpContext);
        //}

       
        public ShoppingCart(IShoppingCartService svc)
        {
            _svc = svc;
        }

        //public ShoppingCart()
        //{
        //}

        public int AddToCart(cfcusaga.domain.Events.Item item)
        {
            // Get the matching cart and item instances
            //var cartItem = portalDB.Carts.SingleOrDefault(
            //    c => c.CartId == ShoppingCartId
            //    && c.ItemId == item.Id);

            try
            {
                var cartItem = _svc.GetCartItem(ShoppingCartId, item.Id);

                if (cartItem == null)
                {
                    // Create a new cart item if no cart item exists
                    //cartItem = new cfcusaga.data.Cart
                    //{
                    //    ItemId = item.Id,
                    //    CartId = ShoppingCartId,
                    //    Count = 1,
                    //    DateCreated = DateTime.Now
                    //};
                    //portalDB.Carts.Add(cartItem);

                    cartItem = new cfcusaga.domain.Events.Cart
                    {
                        ItemId = item.Id,
                        CartId = ShoppingCartId,
                        Count = 1,
                        DateCreated = DateTime.Now
                    };
                    _svc.AddItemToCart(cartItem);

                }
                else
                {
                    // If the item does exist in the cart, 
                    // then add one to the quantity
                    cartItem.Count++;
                }
                // Save changes
                //portalDB.SaveChanges();

                _svc.SaveChanges();
                return cartItem.Count;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public int RemoveFromCart(int id)
        {


            // Get the cart

            var cartItem = portalDB.Carts.Single(
                cart => cart.CartId == ShoppingCartId
                && cart.ItemId == id);


            int itemCount = 0;

            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    portalDB.Carts.Remove(cartItem);
                }
                // Save changes
                portalDB.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            _svc.EmptyCart(ShoppingCartId);
            //var cartItems = portalDB.Carts.Where(
            //    cart => cart.CartId == ShoppingCartId);

            //foreach (var cartItem in cartItems)
            //{
            //    portalDB.Carts.Remove(cartItem);
            //}
            //// Save changes
            //portalDB.SaveChanges();
        }

        public async Task<List<cfcusaga.domain.Events.Cart>> GetCartItems()
        {
            return await _svc.GetCartItems(ShoppingCartId);
            //return portalDB.Carts.Where(
            //    cart => cart.CartId == ShoppingCartId).ToList();
        }

        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in portalDB.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }

        public decimal GetTotal()
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in portalDB.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Item.Price).Sum();

            return total ?? decimal.Zero;
        }

        public async Task<cfcusaga.domain.Orders.Order> CreateOrder(cfcusaga.domain.Orders.Order order)
        {
            decimal orderTotal = 0;
            //order.OrderDetails = new List<cfcusaga.data.OrderDetail>();
            order.OrderDetails = new List<cfcusaga.domain.Orders.OrderDetail>();

            var cartItems = await  GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new cfcusaga.domain.Orders.OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.ItemPrice,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.ItemPrice);
                order.OrderDetails.Add(orderDetail);
                //portalDB.OrderDetails.Add(orderDetail);
                _svc.AddOrderDetails(orderDetail);
            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            //portalDB.SaveChanges();
            _svc.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order;
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
            var shoppingCart = portalDB.Carts.Where(
                c => c.CartId == ShoppingCartId);

            foreach (cfcusaga.data.Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            portalDB.SaveChanges();
        }
    }
}
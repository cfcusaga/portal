﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using cfcusaga.data;

namespace Cfcusaga.Web.Models
{
    public partial class ShoppingCart
    {
        //ApplicationDbContext storeDB = new ApplicationDbContext();
        PortalDbContext portalDB = new PortalDbContext();
        string ShoppingCartId { get; set; }
        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContextBase context)
        {
            var cart = new ShoppingCart();
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller)
        {
            return GetCart(controller.HttpContext);
        }

        public int AddToCart(cfcusaga.data.Item item)
        {
            // Get the matching cart and item instances
            var cartItem = portalDB.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.ItemId == item.ID);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new cfcusaga.data.Cart
                {
                    ItemId = item.ID,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                portalDB.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, 
                // then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            portalDB.SaveChanges();

            return cartItem.Count;
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
            var cartItems = portalDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId);

            foreach (var cartItem in cartItems)
            {
                portalDB.Carts.Remove(cartItem);
            }
            // Save changes
            portalDB.SaveChanges();
        }

        public List<cfcusaga.data.Cart> GetCartItems()
        {
            return portalDB.Carts.Where(
                cart => cart.CartId == ShoppingCartId).ToList();
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

        public cfcusaga.data.Order CreateOrder(cfcusaga.data.Order order)
        {
            decimal orderTotal = 0;
            order.OrderDetails = new List<cfcusaga.data.OrderDetail>();

            var cartItems = GetCartItems();
            // Iterate over the items in the cart, 
            // adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new cfcusaga.data.OrderDetail
                {
                    ItemId = item.ItemId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Item.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Item.Price);
                order.OrderDetails.Add(orderDetail);
                portalDB.OrderDetails.Add(orderDetail);

            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;

            // Save the order
            portalDB.SaveChanges();
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
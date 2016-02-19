﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cfcusaga.data;
using Item = cfcusaga.domain.Events.Item;

namespace cfcusaga.domain.Orders
{
    public interface IShoppingCartService
    {
        //Item GetItem(int id);
        Cart GetCartItem(string shoppingCartId, int id);
        void AddItemToCart(Cart cartItem);
        void SaveChanges();
        Task<Item> GetItem(int id);
        //ShoppingCart GetCart(HttpContextBase httpContext);
        Order GetOrderByIdentity(string name);
        Task SaveChangesAsync();
        Task AddOrder(Order order);
        void AddOrderDetails(OrderDetail detail);
        void EmptyCart(string shoppingCartId);
        Task<List<Cart>> GetCartItems(string shoppingCartId);
        bool IsValidOrder(int id, string userName);
        //object GetCartItem(int id);
        int RemoveFromCart(string shoppingCartId, int id);
        int? GetCount(string shoppingCartId);
        decimal? GetTotal(string shoppingCartId);
        void MigrateCart(string userName, string shoppingCartId);
        void AddCountToItem(string shoppingCartId, int id);
    }

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly PortalDbContext _db;

        public ShoppingCartService(PortalDbContext db)
        {
            _db = db;
        }

        public async Task<Item> GetItem(int id)
        {
            try
            {
                var entitity = await _db.Items.FirstOrDefaultAsync(c => c.ID == id);
                if (entitity == null) return null;
                var item = new Item()
                {
                    ItemPictureUrl = entitity.ItemPictureUrl,
                    Name = entitity.Name,
                    Id = entitity.ID,
                    EventId = entitity.EventId ?? 0,
                    CatagorieId = entitity.CatagoryID,
                    Price = entitity.Price,
                    InternalImage = entitity.InternalImage,
                    EventName = entitity.Event.Name
                };
                return item;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public Order GetOrderByIdentity(string name)
        {
            var anOrder = _db.Orders.Select(o => new Order()
            {
                OrderDate = o.OrderDate,
                Address = o.Address,
                City = o.City,
                Country = o.Country,
                Email = o.Email,
                FirstName = o.FirstName,
                LastName = o.LastName,
                OrderId = o.OrderId,
                Phone = o.Phone,
                PostalCode = o.PostalCode,
                State = o.State,
                Total = o.Total,
                Username = o.Username
            }).FirstOrDefault(x => x.Username == name);
            return anOrder;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }

        public async Task AddOrder(Order order)
        {
            var entity = new data.Order
            {
                //Experation = order.Experation,
                FirstName = order.FirstName,
                LastName = order.LastName,
                OrderDate = DateTime.Now,
                Total = order.Total,
                Address = order.Address,
                City = order.City,
                Country = order.Country,
                Email = order.Email,
                Phone = order.Phone,
                PostalCode = order.PostalCode,
                State = order.State,
                Username = order.Username
            };
            _db.Orders.Add(entity);
            await _db.SaveChangesAsync();
            order.OrderId = entity.OrderId;
        }

        public void AddOrderDetails(OrderDetail detail)
        {
            //portalDB.OrderDetails.Add(detail);
            var entity = new data.OrderDetail
            {
                ItemId = detail.ItemId,
                OrderId = detail.OrderId,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity
            };
            // Set the order total of the shopping cart
            _db.OrderDetails.Add(entity);
        }

        public void EmptyCart(string shoppingCartId)
        {
            var cartItems = _db.Carts.Where(
                cart => cart.CartId == shoppingCartId);

            foreach (var cartItem in cartItems)
            {
                _db.Carts.Remove(cartItem);
            }
            // Save changes
            _db.SaveChanges();
        }

        public async Task<List<Cart>> GetCartItems(string shoppingCartId)
        {
            //(from op in db.ObjectPermissions
            // join pg in db.Pages on op.ObjectPermissionName equals page.PageName
            // where pg.PageID == page.PageID
            // select new
            // {
            //     PermissionName = pg,
            //     ObjectPermission = op
            // }).SingleOrDefault();
            //return _db.Carts.Where(
            //    cart => cart.CartId == shoppingCartId).ToList();


            //return _db.Carts.Select(c=> new domain.Events.Cart()
            //{
            //    CartId = c.CartId,
            //    ID = c.ID,
            //    DateCreated = c.DateCreated,
            //    ItemId = c.ItemId
            //})
            //.Where(
            //    cart => cart.CartId == shoppingCartId).ToList();


            return await (from c in _db.Carts
                join i in _db.Items on c.ItemId equals i.ID
                where c.CartId == shoppingCartId
                select new Cart()
                {
                    CartId = c.CartId,
                    Id = c.ID,
                    DateCreated = c.DateCreated,
                    ItemId = c.ItemId,
                    ItemName = i.Name,
                    ItemPrice = i.Price,
                    Count = c.Count
                }).ToListAsync();
        }

        public bool IsValidOrder(int id, string userName)
        {
            return _db.Orders.Any(
                o => o.OrderId == id &&
                o.Username == userName);
        }

        public int RemoveFromCart(string shoppingCartId, int id)
        {
            try
            {

                var itemRegistratin = _db.CartItemRegistrations.FirstOrDefault(c => c.CartID == id);

                if (itemRegistratin != null)
                {
                    _db.CartItemRegistrations.Remove(itemRegistratin);
                }



                var cartItem = _db.Carts
                    .FirstOrDefault(
                        c => c.CartId == shoppingCartId
                             && c.ID == id);

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
                        _db.Carts.Remove(cartItem);
                    }
                    // Save changes
                    _db.SaveChanges();
                }
                return itemCount;
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
            finally
            {
                _db.SaveChanges();
            }
        }

        public int? GetCount(string shoppingCartId)
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _db.Carts
                          where cartItems.CartId == shoppingCartId
                          select (int?)cartItems.Count).Sum();
            return count;
        }

        public decimal? GetTotal(string shoppingCartId)
        {
            // Multiply item price by count of that item to get 
            // the current price for each of those items in the cart
            // sum all item price totals to get the cart total
            decimal? total = (from cartItems in _db.Carts
                              where cartItems.CartId == shoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Item.Price).Sum();
            return total;
        }

        public void MigrateCart(string userName, string shoppingCartId)
        {
            var shoppingCart = _db.Carts.Where(
                c => c.CartId == shoppingCartId);

            foreach (cfcusaga.data.Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            _db.SaveChanges();
        }

        public  void AddCountToItem(string shoppingCartId, int id)
        {
            var aCart = _db.Carts.SingleOrDefault(
                        c => c.CartId == shoppingCartId
                        && c.ItemId == id);
            if (aCart != null) aCart.Count++;
             _db.SaveChanges();
        }


        //public Cart GetCart(HttpContextBase httpContext)
        //{
        //    throw new NotImplementedException();
        //}

        public Cart GetCartItem(string shoppingCartId, int id)
        {
            try
            {
                var aCart = _db.Carts
            .Select(c => new Cart()
            {
                Id = c.ID,
                CartId = c.CartId,
                DateCreated = c.DateCreated,
                ItemId = c.ItemId,
                Count = c.Count,
            })
            .SingleOrDefault(
            c => c.CartId == shoppingCartId
            && c.Id == id);

                return aCart;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void AddItemToCart(Cart cartItem)
        {
            var entity = _db.Set<data.Cart>().Create();

            entity.CartId = cartItem.CartId;
            entity.Count = cartItem.Count;
            entity.DateCreated = DateTime.Now.ToUniversalTime();
            entity.ItemId = cartItem.ItemId;
            _db.Carts.Add(entity);
            _db.SaveChanges();
            cartItem.Id = entity.ID;
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}

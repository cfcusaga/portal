using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using cfcusaga.data;
using Item = cfcusaga.domain.Events.Item;
using Cart = cfcusaga.domain.Events.Cart;
using Order = cfcusaga.domain.Orders.Order;
using OrderDetail = cfcusaga.domain.Orders.OrderDetail;

namespace cfcusaga.domain
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
        void AddOrder(Order order);
        void AddOrderDetails(OrderDetail detail);
        void EmptyCart(string shoppingCartId);
        Task<List<Cart>> GetCartItems(string shoppingCartId);
        bool IsValidOrder(int id, string userName);
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
                var dbItem = await _db.Items.FirstOrDefaultAsync(c => c.ID == id);
                if (dbItem == null) return null;
                var item = new Item()
                {
                    ItemPictureUrl = dbItem.ItemPictureUrl,
                    Name = dbItem.Name,
                    Id = dbItem.ID,
                    EventId = dbItem.EventId ?? 0,
                    CatagorieId = dbItem.CatagoryID,
                    Price = dbItem.Price,
                    InternalImage = dbItem.InternalImage,
                    EventName = dbItem.Event.Name
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

        public void AddOrder(Order order)
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
            _db.SaveChangesAsync();
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
                select new domain.Events.Cart()
                {
                    CartId = c.CartId,
                    ID = c.ID,
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

        public Cart GetCart(HttpContextBase httpContext)
        {
            throw new NotImplementedException();
        }

        public Cart GetCartItem(string shoppingCartId, int id)
        {
            try
            {
                var aCart = _db.Carts
            .Select(c => new Cart()
            {
                ID = c.ID,
                CartId = c.CartId,
                DateCreated = c.DateCreated,
                ItemId = c.ItemId,
                Count = c.Count,
            })
            .SingleOrDefault(
            c => c.CartId == shoppingCartId
            && c.ItemId == id);

                return aCart;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void AddItemToCart(Cart cartItem)
        {
            var item = _db.Set<data.Cart>().Create();

            item.CartId = cartItem.CartId;
            item.Count = cartItem.Count;
            item.DateCreated = DateTime.Now.ToUniversalTime();
            item.ID = cartItem.ID;
            item.ItemId = cartItem.ItemId;
            //Mapper.Map(cartItem, item);
            _db.Carts.Add(item);
        }

        public void SaveChanges()
        {
            _db.SaveChanges();
        }
    }
}

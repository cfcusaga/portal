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
            _db.SaveChangesAsync();
        }
    }
}

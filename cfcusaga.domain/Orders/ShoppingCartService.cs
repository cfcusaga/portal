using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using cfcusaga.data;
using cfcusaga.domain.Membership;
using Cfcusaga.Web.Extensions;
using Item = cfcusaga.domain.Events.Item;
using Member = cfcusaga.domain.Membership.Member;

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
        //Task AddOrder(Order order);
        Task SaveOrderDetails(OrderDetail detail);
        void EmptyCart(string shoppingCartId);
        Task<List<Cart>> GetCartItems(string shoppingCartId);
        bool IsValidOrder(int id, string userName);
        //object GetCartItem(int id);
        Task<int> RemoveFromCart(string shoppingCartId, int id);
        int? GetCount(string shoppingCartId);
        decimal? GetTotal(string shoppingCartId);
        void MigrateCart(string userName, string shoppingCartId);
        void AddCountToItem(string shoppingCartId, int id);
        void RemoveItemRegistration(int id);
        Task AddMemberDetails(Member aMember);
        void UpdateCartItem(Cart foundItem);
        Task<int> AddEventRegistrations(Member aMember, Order order, Cart item);
        Task<int> AddOrder(Order order, List<Cart> cartItems, string shoppingCartId, string currentUserId);
        Task<List<CartDiscount>> GetCartDiscounts(string shoppingCartId);
    }

    public class ShoppingCartService : IShoppingCartService
    {
        private readonly PortalDbContext _db;
        private readonly IMembershipService _memSvc;

        public ShoppingCartService(PortalDbContext db, IMembershipService memSvc)
        {
            _db = db;
            _memSvc = memSvc;
        }

        public PortalDbContext DbContext
        {
            get { return _db; }
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
                Username = o.Username,
                CheckNumber = o.CheckNumber,
                Notes = o.Notes
            }).FirstOrDefault(x => x.Username == name);
            return anOrder;
        }

        public Task SaveChangesAsync()
        {
            return _db.SaveChangesAsync();
        }


        public async Task<int> AddOrder(Order order, List<Cart> cartItems, string shoppingCartId, string currentUserId)
        {
            int memberId = 0;
            int fatherMeberId = 0;
            int motherMemberId = 0;
            List<int> childMemberIds = new List<int>();
            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    decimal orderTotal = 0;
                    var dbOrder = CreateOrder(order);
                    dbOrder.AspNetUserId = currentUserId;
                    _db.Orders.Add(dbOrder);
                    await _db.SaveChangesAsync();
                    order.OrderId = dbOrder.OrderId;
                    //List<OrderDetail> orderDetails;
                    foreach (var item in cartItems)
                    {
                        var js = item.ToJson();
                        
                         var orderDetail = CreateOrderDetail(order, item);
                        order.OrderDetails.Add(orderDetail);
                        // Set the order total of the shopping cart
                        orderTotal += (item.Count * item.ItemPrice);
                        
                        //order.OrderDetails.Add(orderDetail);
                        await SaveOrderDetails(orderDetail);

                        cfcusaga.domain.Membership.Member aMember = null;
                        if (item.CategoryId == (int)Enums.CategoryTypeEnum.Registration && !item.MemberId.HasValue)
                        {
                            aMember = CreateMember(item);
                            if (item.RelationToMemberTypeId == (int)Enums.RelationToMe.Self)
                            {
                                if (aMember.AspNetUserId == null)
                                {
                                    aMember.AspNetUserId = currentUserId;
                                }
                            }
                            await AddMemberDetails(aMember);
                            if (item.RelationToMemberTypeId == (int) Enums.RelationToMe.Self || item.RelationToMemberTypeId == (int)Enums.RelationToMe.Spouse)
                            {
                                memberId = aMember.Id;
                                if (item.Gender == "M")
                                {
                                    fatherMeberId = memberId;
                                }
                                if (item.Gender == "F")
                                {
                                    motherMemberId = memberId;
                                }

                                //UpdateCurrentUserMemberId(aMember.Id);
                            }
                            else if (item.RelationToMemberTypeId == (int)Enums.RelationToMe.Child)
                            {
                                childMemberIds.Add(aMember.Id);
                            }
                        }
                        if (item.CategoryId == (int)Enums.CategoryTypeEnum.Registration && aMember != null)
                        {
                            await AddEventRegistrations(aMember, order, item);
                        }
                        // _svc.RemoveItemRegistration(item.Id
                    }
                    foreach (var childMemberId in childMemberIds)
                    {
                        UpdateChildMemberParentIds(childMemberId, motherMemberId, fatherMeberId);
                    }
                    UpdateHeadOfFamily(fatherMeberId, motherMemberId);
                    UpdateSpouseId(motherMemberId, fatherMeberId);
                    // Set the order's total to the orderTotal count
                    order.Total = orderTotal;

                    var member = await  GetMemberInfoFromAspNetUserId(currentUserId);
                    if (member != null)
                    {
                        dbOrder.MemberId = member.Id;
                    }

                    var discounts = await GetCartDiscounts(shoppingCartId);
                    decimal discountsTotal = 0;
                    foreach (var discount in discounts)
                    {
                        var orderDiscount = new OrderDiscount()
                        {
                            OrderId = order.OrderId,
                            DiscountId = discount.DiscountId,
                            Name = discount.Name,
                            Discount = discount.Discount,
                            Quantity =  discount.Quantity
                        };
                        order.OrderDiscounts.Add(orderDiscount);
                        await AddOrderDiscounts(order, discount);
                         discountsTotal = (discount.Discount*discount.Quantity);
                    }

                    //TODO: Clean the 
                    //order.OrderDetails
                    order.Total = order.Total - discountsTotal;
                    dbOrder.Total = order.Total;
                    await SaveChangesAsync();
                    // Empty the shopping cart
                    EmptyCart(shoppingCartId);


                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
            return memberId;
        }

        private async Task AddOrderDiscounts(Order order, CartDiscount discount)
        {
            try
            {
                var entity = new data.OrderDiscount()
                {
                    OrderId = order.OrderId,
                    DiscountId = discount.DiscountId,
                    Discount = discount.Discount,
                    Quantity = discount.Quantity
                };
                // Set the order total of the shopping cart
                _db.OrderDiscounts.Add(entity);
                await _db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<List<CartDiscount>> GetCartDiscounts(string shoppingCartId)
        {
            try
            {
                return await(from c in _db.CartDiscounts
                             join d  in _db.Discounts on c.DiscountId equals d.Id
                             where c.CartId == shoppingCartId
                             select new CartDiscount()
                             {
                                 Id = c.Id,
                                 Name = d.Name,
                                 DiscountId = c.DiscountId,
                                 Discount = c.Discount,
                                 Quantity = c.Quantity
                             }).ToListAsync();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        private async Task<Member> GetMemberInfoFromAspNetUserId(string currentUserId)
        {
            var entity = await _db.Members.FirstOrDefaultAsync(c => c.AspNetUserId == currentUserId);
            if (entity != null)
            {
                var aMember = new Member()
                {
                    Id = entity.Id,
                    LastName = entity.LastName,
                    Firstname = entity.FirstName,
                    Gender = entity.Gender
                };
                return aMember;
            }
            return null;
        }

        private void UpdateSpouseId(int memberId, int spouseId)
        {
            if (spouseId <= 0) return;
            var entity = _db.Members.FirstOrDefault(c => c.Id == memberId);
            if (entity != null)
            {
                entity.SpouseMemberId = spouseId;
            }
            _db.SaveChanges();
        }

        private void UpdateHeadOfFamily(int fatherMeberId, int motherMemberId)
        {
            if (fatherMeberId <= 0) return;
            var entity = _db.Members.FirstOrDefault(c => c.Id == fatherMeberId);
            if (entity != null)
            {
                entity.IsHeadOfFamily = true;
                if (motherMemberId > 0)
                {
                    entity.SpouseMemberId = motherMemberId;
                }
            }
            _db.SaveChanges();
        }

        private void UpdateChildMemberParentIds(int childMemberId, int motherMemberId, int fatherMeberId)
        {
            var entity = _db.Members.FirstOrDefault(c => c.Id == childMemberId);
            if (entity != null)
            {
                if (motherMemberId > 0)
                    entity.MotherMemberId = motherMemberId;
                if (fatherMeberId > 0) 
                    entity.FatherMemberId = fatherMeberId;
                
            }
            _db.SaveChanges();
        }

        //private void UpdateCurrentUserMemberId(int id)
        //{
        //    var manager =
        //        new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        //    var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
        //    var ctx = store.Context;
        //    var currentUser = manager.FindById(User.Identity.GetUserId());

        //    currentUser.Address = order.Address;
        //    currentUser.City = order.City;
        //    currentUser.Country = order.Country;
        //    currentUser.State = order.State;
        //    currentUser.Phone = order.Phone;
        //    currentUser.PostalCode = order.PostalCode;
        //    currentUser.FirstName = order.FirstName;

        //    //Save this back
        //    //http://stackoverflow.com/questions/20444022/updating-user-data-asp-net-identity
        //    //var result = await UserManager.UpdateAsync(currentUser);
        //    await ctx.SaveChangesAsync();
        //}

        private static Member CreateMember(Cart item)
        {
            return new cfcusaga.domain.Membership.Member
            {
                LastName = item.Lastname,
                Firstname = item.Firstname,
                BirthDate = item.BirthDate ?? item.BirthDate,
                Gender = item.Gender,
                Phone = item.Phone,
                Email = item.Email
            };
        }

        private static OrderDetail CreateOrderDetail(Order order, Cart item)
        {
            var tShirtSize = item.CategoryId == (int) Enums.CategoryTypeEnum.Registration ? item.RegistrationTshirtSize : item.TshirtSize;
            var orderDetail = new cfcusaga.domain.Orders.OrderDetail
            {
                ItemId = item.ItemId,
                OrderId = order.OrderId,
                UnitPrice = item.ItemPrice,
                Quantity = item.Count,
                CartId = item.Id,
                Lastname = item.Lastname,
                Firstname = item.Firstname,
                Gender = item.Gender,
                BirthDate = item.BirthDate,
                Allergies = item.Allergies,
                TshirtSize = tShirtSize,
                CategoryId = item.CategoryId,
                ItemName = item.ItemName,
                RegistrationDetail = item.ToJson()
            };
            return orderDetail;
        }


        private static data.Order CreateOrder(Order order)
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
                Username = order.Username,
                CheckNumber = order.CheckNumber,
                Notes = order.Notes,
                IsAgreeToWaiver = order.IsAgreeToWaiver
            };
            return entity;
        }

        public async Task SaveOrderDetails(OrderDetail detail)
        {
            //portalDB.OrderDetails.Add(detail);
            var entity = new data.OrderDetail
            {
                ItemId = detail.ItemId,
                OrderId = detail.OrderId,
                UnitPrice = detail.UnitPrice,
                Quantity = detail.Quantity,
                RegistrationDetail = detail.RegistrationDetail,
                Lastname = detail.Lastname,
                Firstname = detail.Firstname,
                Gender = detail.Gender,
                BirthDate = detail.BirthDate,
                Allergies = detail.Allergies,
                TshirtSize = detail.TshirtSize
            };
            // Set the order total of the shopping cart
            _db.OrderDetails.Add(entity);
            await _db.SaveChangesAsync();
            detail.OrderDetailId = entity.OrderDetailId;
        }

        public void EmptyCart(string shoppingCartId)
        {
            try
            {
                var cartItems = _db.Carts.Where(
            cart => cart.CartId == shoppingCartId).ToList();

                foreach (var cartItem in cartItems)
                {
                    var reg = _db.CartItemRegistrations.FirstOrDefault(r => r.CartID == cartItem.ID);
                    if (reg!=null)
                        _db.CartItemRegistrations.Remove(reg);
                    _db.Carts.Remove(cartItem);
                }
                // Save changes
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        public async Task<List<Cart>> GetCartItems(string shoppingCartId)
        {
            try
            {
                return await (from c in _db.Carts
                              join i in _db.Items on c.ItemId equals i.ID
                              join r in _db.CartItemRegistrations on c.ID equals  r.CartID
                              into tmpTbl from tmp in tmpTbl.DefaultIfEmpty()
                              where c.CartId == shoppingCartId
                              select new Cart()
                              {
                                  CartId = c.CartId,
                                  Id = c.ID,
                                  TshirtSize = c.TshirtSize,
                                  CategoryId = i.CatagoryID,
                                  DateCreated = c.DateCreated,
                                  ItemId = c.ItemId,
                                  ItemName = i.Name,
                                  ItemPrice = i.Price,
                                  EventId = i.EventId,
                                  Count = c.Count,
                                  ItemRegistrationId = (tmp == null) ? (int?) null : tmp.ID,
                                  MemberId = (tmp == null) ? null : tmp.MemberId,
                                  Lastname = (tmp == null) ? "" : tmp.LastName,
                                  Firstname = (tmp == null) ? "" : tmp.FirstName,
                                  BirthDate = (tmp == null) ? null : tmp.BirthDate,
                                  Gender = (tmp == null) ? null : tmp.Gender,
                                  Notes = (tmp == null) ? null : tmp.Notes,
                                  Allergies = (tmp == null) ? null : tmp.Allergies,
                                  RegistrationTshirtSize = (tmp == null) ? null : tmp.TshirtSize,
                                  RelationToMemberTypeId = (short) ((tmp == null) ? 0 : tmp.RelationToMemberTypeId),
                              }).ToListAsync();
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
                throw;
            }
        }

        public bool IsValidOrder(int id, string userName)
        {
            return _db.Orders.Any(
                o => o.OrderId == id &&
                o.Username == userName);
        }

        public async Task<int> RemoveFromCart(string shoppingCartId, int id)
        {
            using (var dbContextTransaction = _db.Database.BeginTransaction())
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


                        await RemoveItemDiscount(cartItem.ItemId, shoppingCartId);

                        await _db.SaveChangesAsync();

                        dbContextTransaction.Commit();
                    }
                    return itemCount;
                }
                catch (Exception ex)
                {
                    var msg = ex.Message;
                    dbContextTransaction.Rollback();
                    throw;
                }

            }


        }


        private async Task RemoveItemDiscount(int itemId, string shoppingCartId)
        {
            var itemDiscount = await _db.Discounts.FirstOrDefaultAsync(d => d.ItemId == itemId);
            if (itemDiscount != null)
            {
                var cartDiscount =
                    await
                        _db.CartDiscounts.FirstOrDefaultAsync(
                            c => c.DiscountId == itemDiscount.Id && c.CartId == shoppingCartId);
                if (cartDiscount != null)
                {
                    if (itemDiscount.DiscountBeginAtNthItem != null)
                        cartDiscount.Quantity = cartDiscount.Quantity - 1;
                    if (cartDiscount.Quantity == 0)
                    {
                        _db.CartDiscounts.Remove(cartDiscount);
                    }
                }
                
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
            decimal? cartTotal = (from cartItems in _db.Carts
                              where cartItems.CartId == shoppingCartId
                              select (int?)cartItems.Count *
                              cartItems.Item.Price).Sum();

            decimal? totalDiscount = (from cartDiscounts in _db.CartDiscounts
                              where cartDiscounts.CartId == shoppingCartId
                              select (int?)cartDiscounts.Quantity *
                              cartDiscounts.Discount).Sum();
            if (totalDiscount.HasValue)
                return (cartTotal - totalDiscount);
            return cartTotal;

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

        public void RemoveItemRegistration(int id)
        {
            var itemRegistratin = _db.CartItemRegistrations.FirstOrDefault(c => c.CartID == id);

            if (itemRegistratin != null)
            {
                _db.CartItemRegistrations.Remove(itemRegistratin);
            }
        }



        public async Task AddMemberDetails(Member item)
        {
            try
            {
                var aMember = _db.Set<data.Member>().Create();
                aMember.LastName = item.LastName;
                aMember.FirstName = item.Firstname;
                aMember.BirthDate = item.BirthDate ?? item.BirthDate;
                aMember.Gender = item.Gender;
                aMember.Phone = item.Phone;
                aMember.Email = item.Email;
                aMember.AspNetUserId = item.AspNetUserId;
                aMember.DateCreated = DateTime.Now;
                aMember.DateModified = DateTime.Now;
                _db.Members.Add(aMember);
                await _db.SaveChangesAsync();
                item.Id = aMember.Id;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }

        public void UpdateCartItem(cfcusaga.domain.Orders.Cart cartItem)
        {
            var entity = _db.Carts.FirstOrDefault(c => c.ID == cartItem.Id);
            if (entity != null) entity.TshirtSize = cartItem.TshirtSize;
            _db.SaveChanges();
        }

        public async Task<int> AddEventRegistrations(Member aMember, Order order, Cart item)
        {
            var eventReg = _db.Set<data.EventRegistration>().Create();
            if (item.EventId != null) eventReg.EventId = item.EventId.Value;
            eventReg.MemberId = aMember.Id;
            eventReg.OrderId = order.OrderId;
            eventReg.CreationDate = DateTime.Now;
            eventReg.ModifiedDate = DateTime.Now;
            _db.EventRegistrations.Add(eventReg);
            await _db.SaveChangesAsync();
            return eventReg.ID;
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
            entity.TshirtSize = cartItem.TshirtSize;
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

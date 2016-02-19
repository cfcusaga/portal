using System;
using System.Collections;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using cfcusaga.data;
using cfcusaga.domain.Helpers;
using PagedList;

namespace cfcusaga.domain.Events
{
    public interface IEventServices
    {
        Task<IPagedList<Event>> GetOpenEvents(string sortOrder, string searchString, int pageSize, int pageNumber);
        Task<int> SaveChangesAsync(Event newEvent);
        Task<Event> GetEventDetails(int? id);
        Task<Event> UpdateEvent(Event anEvent);
        Task DeleteEvent(int id);
        Task<Item> GetEventItems(int id);
        IEnumerable GetItemCategories();
        Task<int> AddEventItem(Item newItem);
        Task<IPagedList<Item>> GetEventItems(int? eventId, string sortOrder, string searchString, int pageSize, int pageNumber);
        Task<Item> GetEventItemDetails(int? id);
        Task UpdateEventItemAsync(Item item);
        //Task<Item> FindEventItemAsync(int? id);
        Task DeleteEventItem(int id);
        IEnumerable GetRelationToMemberTypes();
    }

    public class EventServices : IEventServices
    {
        private readonly PortalDbContext _db;

        public EventServices(PortalDbContext db)
        {
            _db = db;
        }

        public async Task<IPagedList<Event>> GetOpenEvents(string sortOrder, string searchString, int pageSize, int pageNumber)
        {
            var items = from i in _db.Events
                        select new Event
                        {
                            Id = i.Id,
                            Name = i.Name,
                            Description = i.Description,
                            StartDate = i.StartDate,
                            EndDate = i.EndDate,
                            OrgId = i.OrgId,
                        };
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                         || s.Description.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "startDate":
                    items = items.OrderBy(s => s.StartDate);
                    break;
                case "startDate_desc":
                    items = items.OrderByDescending(s => s.StartDate);
                    break;
                case "orgId":
                    items = items.OrderBy(s => s.OrgId);
                    break;
                case "orgId_desc":
                    items = items.OrderByDescending(s => s.OrgId);
                    break;
                default: // Name ascending 
                    items = items.OrderBy(s => s.Name);
                    break;
            }

            return await items.ToPagedListAsync(pageNumber, pageSize);
        }

        public async Task<int> SaveChangesAsync(Event newEvent)
        {
            var item = _db.Set<data.Event>().Create();

            //Mapper.Map(newEvent, item);
            item.Description = newEvent.Description;
            item.EndDate = newEvent.EndDate;
            item.StartDate = newEvent.StartDate;
            item.Name = newEvent.Name;
            item.OrgId = newEvent.OrgId;
            item.CreateDate = DateTime.Now;
            item.ModifiedDate = DateTime.Now;
            _db.Events.Add(item);
            await _db.SaveChangesAsync();
            return item.Id;
        }

        public async Task<Event> GetEventDetails(int? id)
        {
            var anEvent = await _db.Events.Select(e => new Event()
            {
                Name = e.Name,
                Id = e.Id,
                EndDate = e.EndDate,
                StartDate = e.StartDate,
                OrgId = e.OrgId,
                Description = e.Description
            }).FirstOrDefaultAsync(e => e.Id == id);
            //Mapper.Map(dbEvent, anEvent);
            return anEvent;
        }

        public async Task<Event> UpdateEvent(Event anEvent)
        {
            var entity = await _db.Events.FindAsync(anEvent.Id);

            //TODO: Need to find a better to implement this using AutoMapper
            //Mapper.Map(anEvent, entity);

            entity.Name = anEvent.Name;
            entity.Description = anEvent.Description;
            entity.StartDate = anEvent.StartDate;
            entity.EndDate = anEvent.EndDate;
            entity.OrgId = anEvent.OrgId;
            entity.ModifiedDate = DateTime.Now.ToUniversalTime();
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return await GetEventDetails(anEvent.Id);
        }

        public async Task DeleteEvent(int id)
        {
            var entity = await _db.Events.FindAsync(id);
            _db.Events.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public Task<Item> GetEventItems(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddEventItem(Item newItem)
        {
            var item = _db.Set<data.Item>().Create();

            //Mapper.Map<Item, data.Item>(newItem, item);
            item.Description = newItem.Description;
            item.IsShirtIncluded = newItem.IsShirtIncluded;
            item.IsRequireBirthDateInfo = newItem.IsRequireBirthDateInfo;
            item.IsRequireParentWaiver = newItem.IsRequireParentWaiver;
            item.IsRequireRegistrationInfo = newItem.CatagorieId == 2;// newItem.IsRequireRegistrationInfo;
            item.Name = newItem.Name;
            item.CatagoryID = newItem.CatagorieId;
            item.Price = newItem.Price;
            item.ItemPictureUrl = newItem.ItemPictureUrl;
            item.InternalImage = newItem.InternalImage;

            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return item.ID;
        }

        public async Task<Item> GetEventItemDetails(int? id)
        {
            var item = await _db.Items.Select(x => new Item()
            {
                Id = x.ID,
                Name =  x.Name,
                Price = x.Price,
                ItemPictureUrl = x.ItemPictureUrl,
                CatagorieId = x.CatagoryID,
                IsShirtIncluded = x.IsShirtIncluded??false,
                IsRequireRegistrationInfo =x.IsRequireRegistrationInfo ?? false,
                IsRequireParentWaiver = x.IsRequireParentWaiver ?? false,
                IsRequireBirthDateInfo = x.IsRequireBirthDateInfo ?? false,
                Description = x.Description,
                EventId = (int) x.EventId
            }).FirstOrDefaultAsync(i => i.Id == id);
            //Mapper.Map(entity, item);
            return item;
        }

        public async Task UpdateEventItemAsync(Item item)
        {
            var entity = await _db.Items.FindAsync(item.Id);

            //TODO: Need to find a better to implement this using AutoMapper

            entity.Name = item.Name;
            //entity.CatagoryID = item.CatagorieId;
            //entity.EventId = item.EventId;
            entity.ItemPictureUrl = item.ItemPictureUrl;
            entity.Price = item.Price;
            entity.IsShirtIncluded = item.IsShirtIncluded;
            entity.IsRequireBirthDateInfo = item.IsRequireBirthDateInfo;
            entity.Description = item.Description;
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEventItem(int id)
        {
            var item = await _db.Items.FindAsync(id);
            _db.Items.Remove(item);
            await _db.SaveChangesAsync();
        }

        public IEnumerable GetRelationToMemberTypes()
        {
            return _db.RelationToMemberTypes;
        }

        public async Task<IPagedList<Item>> GetEventItems(int? eventId, string sortOrder, string searchString, int pageSize, int pageNumber)
        {
            var items = from i in _db.Items
                join e in _db.Events on i.EventId equals e.Id
                        where i.EventId == eventId.Value
                select new Item
                {
                    Name = i.Name,
                    Price = i.Price,
                    Id = i.ID,
                    CatagorieId = i.CatagoryID,
                    ItemPictureUrl = i.ItemPictureUrl,
                    InternalImage = i.InternalImage,
                    EventName = e.Name,
                    EventId = (int) i.EventId
                };
            if (!string.IsNullOrEmpty(searchString))
            {
                items = items.Where(s => s.Name.ToUpper().Contains(searchString.ToUpper())
                                         || s.EventName.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    items = items.OrderByDescending(s => s.Name);
                    break;
                case "Price":
                    items = items.OrderBy(s => s.Price);
                    break;
                case "price_desc":
                    items = items.OrderByDescending(s => s.Price);
                    break;
                default: // Name ascending 
                    items = items.OrderBy(s => s.Name);
                    break;
            }
            
            return await items.ToPagedListAsync(pageNumber, pageSize);
        }

        //private static void CreateItemMapper()
        //{
        //    Mapper.CreateMap<Item, data.Item>()
        //        .ForMember(dest => dest.Name,
        //            opts => opts.MapFrom(src => src.Name))
        //        .ForMember(dest => dest.Price,
        //            opts => opts.MapFrom(src => src.Price))
        //        .ForMember(dest => dest.EventId,
        //            opts => opts.MapFrom(src => src.EventId))
        //        .ForMember(dest => dest.CatagoryID,
        //            opts => opts.MapFrom(src => src.CatagorieId))
        //        .ForMember(dest => dest.ItemPictureUrl,
        //            opts => opts.MapFrom(src => src.ItemPictureUrl))
        //        .ForMember(dest => dest.Price,
        //            opts => opts.MapFrom(src => src.Price))
        //        .ForMember(dest => dest.InternalImage,
        //            opts => opts.MapFrom(src => src.InternalImage))
        //        .ReverseMap();
        //}


        public IEnumerable GetItemCategories()
        {
            return _db.Catagories;
        }

    }


}

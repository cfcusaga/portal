using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using cfcusaga.data;
using cfcusaga.domain.Helpers;
using cfcusaga.domain.Mappers;
using PagedList;

namespace cfcusaga.domain.Events
{
    public interface IEventServices
    {
        Task<IPagedList<Event>> GetOpenEvents(string sortOrder, string searchString, int pageSize, int pageNumber);
        Task<int> SaveChangesAsync(Event newEvent);
        Task<Event> GetEventDetails(int? id);
    }

    public class EventServices : IEventServices
    {
        private readonly PortalDbContext _db;
        //private readonly IMappingEngine _mappingEngine;

        //public EventServices(PortalDbContext db, IMappingEngine mappingEngine)
        //{
        //    _db = db;
        //    _mappingEngine = mappingEngine;
        //}

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
            //CreateEventMapper();
            var item = _db.Set<data.Event>().Create();

            Mapper.Map(newEvent, item);
            item.CreateDate = DateTime.Now;
            item.ModifiedDate = DateTime.Now;
            _db.Events.Add(item);
            await _db.SaveChangesAsync();
            return item.Id;
        }

        public async Task<Event> GetEventDetails(int? id)
        {
            var anEvent = new Event();
            var dbEvent = await _db.Events.FindAsync(id);
            Mapper.Map(dbEvent, anEvent);
            return anEvent;
        }

        public async Task<int> AddItem(Item newItem)
        {
            CreateItemMapper();
            var item = _db.Set<cfcusaga.data.Item>().Create();

            //custExisting = Mapper.Map(Of CustomerDTO,  Customer)(custDTO, custExisting)
            Mapper.Map<Item, data.Item>(newItem, item);
            _db.Items.Add(item);
            await _db.SaveChangesAsync();
            return item.ID;
        }


        
        public async Task<IPagedList<Item>> GetItems(string sortOrder, string searchString, int pageSize, int pageNumber)
        {
            //from t1 in db.Table1
            //join t2 in db.Table2 on t1.field equals t2.field
            //select new { t1.field2, t2.field3 }

            var items = from i in _db.Items
                join e in _db.Events on i.EventId equals e.Id
                select new Item
                {
                    Name = i.Name,
                    Price = i.Price,
                    Id = i.ID,
                    CatagorieId = i.CatagorieId,
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
            //CreateItemMapper();
            //var results = Mapper.Map<List<data.Item>, List<Item>>(new List<data.Item>(pagedList));
            //return pagedList.ToPagedList();
        }

        private static void CreateItemMapper()
        {
            Mapper.CreateMap<Item, data.Item>()
                .ForMember(dest => dest.Name,
                    opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(src => src.Price))
                .ForMember(dest => dest.EventId,
                    opts => opts.MapFrom(src => src.EventId))
                .ForMember(dest => dest.CatagorieId,
                    opts => opts.MapFrom(src => src.CatagorieId))
                .ForMember(dest => dest.ItemPictureUrl,
                    opts => opts.MapFrom(src => src.ItemPictureUrl))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(src => src.Price))
                .ForMember(dest => dest.InternalImage,
                    opts => opts.MapFrom(src => src.InternalImage))
                .ReverseMap();
        }

        private static void CreateEventMapper()
        {
            Mapper.CreateMap<Event, data.Event>()
                .ForMember(dest => dest.Id,
                    opts => opts.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                    opts => opts.MapFrom(src => src.Description))
                .ForMember(dest => dest.StartDate,
                    opts => opts.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate,
                    opts => opts.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.OrgId,
                    opts => opts.MapFrom(src => src.OrgId))
                .ReverseMap();
        }
    }


}

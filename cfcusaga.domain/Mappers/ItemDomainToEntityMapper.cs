using AutoMapper;
using cfcusaga.domain.Events;

namespace cfcusaga.domain.Mappers
{
    public class ItemDomainToEntityMapper : Profile
    {
        protected override void Configure()
        {
            Mapper.CreateMap<Item, data.Item>()
                .ForMember(dest => dest.Name,
                    opts => opts.MapFrom(src => src.Name))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(src => src.Price))
                .ForMember(dest => dest.EventId,
                    opts => opts.MapFrom(src => src.EventId))
                .ForMember(dest => dest.CatagoryID,
                    opts => opts.MapFrom(src => src.CatagorieId))
                .ForMember(dest => dest.ItemPictureUrl,
                    opts => opts.MapFrom(src => src.ItemPictureUrl))
                .ForMember(dest => dest.Price,
                    opts => opts.MapFrom(src => src.Price))
                .ForMember(dest => dest.InternalImage,
                    opts => opts.MapFrom(src => src.InternalImage))
                .ReverseMap();
        }
    }
}
using AutoMapper;
using cfcusaga.domain.Events;

namespace cfcusaga.domain.Mappers
{
    public class DomainEventMapper : Profile
    {
        protected override void Configure()
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
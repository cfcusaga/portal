using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
//using Microsoft.Practices.Unity;

namespace cfcusaga.domain.Mappers
{
    public static class AutomapperDomainConfiguration
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new DomainEventMapper());
            });
        }
    }
}
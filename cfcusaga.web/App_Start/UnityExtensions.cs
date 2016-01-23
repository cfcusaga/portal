using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.Mappers;
using Microsoft.Practices.Unity;

namespace Cfcusaga.Web
{
    public static class UnityExtensions
    {
        public static void RegisterAutoMapperType(this IUnityContainer container, LifetimeManager lifetimeManager = null)
        {
            RegisterAutoMapperProfiles(container);

            var profiles = container.ResolveAll<Profile>();
            var autoMapperConfigurationStore = new ConfigurationStore(new TypeMapFactory(), MapperRegistry.Mappers);
            profiles.Each(autoMapperConfigurationStore.AddProfile);

            autoMapperConfigurationStore.AssertConfigurationIsValid();

            container.RegisterInstance<IConfigurationProvider>(autoMapperConfigurationStore, new ContainerControlledLifetimeManager());
            container.RegisterInstance<IConfiguration>(autoMapperConfigurationStore, new ContainerControlledLifetimeManager());

            //container.RegisterType<IMappingEngine, MappingEngine>(lifetimeManager ?? new TransientLifetimeManager(), new InjectionConstructor(typeof(IConfigurationProvider)));
            container.RegisterType<IMappingEngine, MappingEngine>(lifetimeManager ?? new TransientLifetimeManager(), new InjectionConstructor(typeof(IConfigurationProvider)), new InjectionFactory(_ => Mapper.Engine));

        }

        private static void RegisterAutoMapperProfiles(IUnityContainer container)
        {
            IEnumerable<Type> autoMapperProfileTypes = AllClasses.FromAssemblies(AppDomain.CurrentDomain.GetAssemblies())
                           .Where(type => type != typeof(Profile) && typeof(Profile).IsAssignableFrom(type));

            autoMapperProfileTypes.Each(autoMapperProfileType =>
                container.RegisterType(typeof(Profile),
                autoMapperProfileType,
                autoMapperProfileType.FullName,
                new ContainerControlledLifetimeManager(),
                new InjectionMember[0]));
        }
    }
}
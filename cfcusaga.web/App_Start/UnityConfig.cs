using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using AutoMapper.Internal;
using AutoMapper.Mappers;
using cfcusaga.data;
using cfcusaga.domain;
using cfcusaga.domain.Events;
using cfcusaga.domain.Mappers;
using Cfcusaga.Web.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using Unity.Mvc5;
using IConfiguration = Cfcusaga.Web.Configuration.IConfiguration;

namespace Cfcusaga.Web
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            container.RegisterType<UserManager<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<IUserStore<ApplicationUser>, UserStore<ApplicationUser>>(new HierarchicalLifetimeManager());
            container.RegisterType<DbContext, ApplicationDbContext>(new HierarchicalLifetimeManager());

            container.RegisterType<IEventServices, EventServices>();
            container.RegisterType<IShoppingCartService, ShoppingCartService>();

            //configure/bootstrap the domain layer Automappers. this can be use if not using DI container
            //AutomapperDomainConfiguration.Configure();

            // this will register mapper profiles in this executing assembly and register Automapper as ContainerControlledLifetimeManager
            container.RegisterAutoMapperType(new ContainerControlledLifetimeManager()); 

            container.RegisterType<PortalDbContext>(new HierarchicalLifetimeManager());
            
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

    }
}
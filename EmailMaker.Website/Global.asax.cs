﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using CoreDdd.Infrastructure;
using CoreDdd.Nhibernate.Configurations;
using CoreDdd.Nhibernate.Register.Castle;
using CoreDdd.Queries;
using CoreDdd.Register.Castle;
using CoreDdd.Web;
using CoreDdd.Web.ModelBinders;
using CoreIoC;
using EmailMaker.Commands;
using EmailMaker.Controllers;
using EmailMaker.Domain.EventHandlers;
using EmailMaker.Infrastructure;
using EmailMaker.Queries;
using NServiceBus;

namespace EmailMaker.Website
{
    public class UnitOfWorkApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var nserviceBusAssemblies = new[]
                                            {
                                                typeof (IMessage).Assembly,
                                                typeof (Configure).Assembly,
                                            };
            var container = new WindsorContainer();
            Configure
                .With(nserviceBusAssemblies)
                .Log4Net()
                .CastleWindsorBuilder(container)
                .BinarySerializer()
                .MsmqTransport()
                .UnicastBus()
                .LoadMessageHandlers()
                .CreateBus()
                .Start();
            
            container.Install(
                FromAssembly.Containing<ControllerInstaller>(),
                FromAssembly.Containing<QueryExecutorInstaller>(),
                FromAssembly.Containing<CommandHandlerInstaller>(),
                FromAssembly.Containing<EventHandlerInstaller>(),
                FromAssembly.Containing<QueryHandlerInstaller>(),
                FromAssembly.Containing<NhibernateInstaller>(),
                FromAssembly.Containing<EmailMakerNhibernateInstaller>()
                );
            IoC.Initialize(container);

            ControllerBuilder.Current.SetControllerFactory(new IoCControllerFactory());
            ModelBinders.Binders.DefaultBinder = new EnumConverterModelBinder();

            ConfigureNhibernate();
        }

        private void ConfigureNhibernate()
        {
            IoC.Resolve<INhibernateConfigurator>();
        }

        public virtual void Application_BeginRequest()
        {
            GetUnitOfWorkForCurrentThread().BeginTransaction();
        }

        public virtual void Application_EndRequest()
        {
            GetUnitOfWorkForCurrentThread().Commit();
        }

        public virtual void Application_Error()
        {
            GetUnitOfWorkForCurrentThread().Rollback();
        }

        private IUnitOfWork GetUnitOfWorkForCurrentThread()
        {
            return IoC.Resolve<IUnitOfWork>();
        }   
    }
}
namespace deeP.SPAWeb
{
    using Autofac;
    using Autofac.Integration.Mvc;
    using Autofac.Integration.WebApi;
    using deeP.SPAWeb.Services;
    using Owin;
    using System.Reflection;
    using System.Web.Http;
    using System.Web.Mvc;

    public partial class Startup
    {
        /// <summary>
        /// Register types into the Autofac Inversion of Control (IOC) container.
        /// </summary>
        public static void ConfigureContainer(IAppBuilder app)
        {
            IContainer container = CreateContainer();
            app.UseAutofacMiddleware(container);

            // Register MVC Types 
            app.UseAutofacMvc();
        }

        private static IContainer CreateContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
            Assembly assembly = Assembly.GetExecutingAssembly();

            RegisterServices(builder);
            RegisterMvcTypes(builder, assembly);

            IContainer container = builder.Build();

            SetDependencyResolvers(container);

            return container;
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            builder.RegisterType<ConfigurationService>().As<IConfigurationService>().SingleInstance();
            builder.RegisterType<BrowserConfigService>().As<IBrowserConfigService>().InstancePerRequest();
            builder.RegisterType<CacheService>().As<ICacheService>().SingleInstance();
            builder.RegisterType<FeedService>().As<IFeedService>().InstancePerRequest();
            builder.RegisterType<LoggingService>().As<ILoggingService>().SingleInstance();
            builder.RegisterType<ManifestService>().As<IManifestService>().InstancePerRequest();
            builder.RegisterType<OpenSearchService>().As<IOpenSearchService>().InstancePerRequest();
            builder.RegisterType<RobotsService>().As<IRobotsService>().InstancePerRequest();
            builder.RegisterType<SitemapService>().As<ISitemapService>().InstancePerRequest();
            builder.RegisterType<SitemapPingerService>().As<ISitemapPingerService>().InstancePerRequest();
        }

        private static void RegisterMvcTypes(ContainerBuilder builder, Assembly assembly)
        {
            // Register Common MVC Types
            builder.RegisterModule<AutofacWebTypesModule>();

            // Register MVC Filters
            builder.RegisterFilterProvider();

            // Register MVC Controllers
            builder.RegisterControllers(assembly);

            // Register Web Api filters
            builder.RegisterWebApiFilterProvider(GlobalConfiguration.Configuration);

            // Register Web Api model binder provider
            builder.RegisterWebApiModelBinderProvider();

            // Register Web Api model binders
            builder.RegisterWebApiModelBinders(assembly);

            // Register Web Api controllers
            builder.RegisterApiControllers(assembly);
        }

        /// <summary>
        /// Sets the ASP.NET MVC and WebAPI dependency resolvers.
        /// </summary>
        /// <param name="container">The container.</param>
        private static void SetDependencyResolvers(IContainer container)
        {
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalConfiguration.Configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using AutoMapper;
using Caliburn.Micro;
using Microsoft.Extensions.Configuration;
using RSA.DesktopUI.Library.Api;
using RSA.DesktopUI.Library.Models;
using RSA.DesktopUI.Models;
using RSA.DesktopUI.ViewModels;

namespace RSA.DesktopUI
{
    public class Bootstrapper : BootstrapperBase
    {
        private readonly SimpleContainer _container = new SimpleContainer();

        public Bootstrapper()
        {
            Initialize();
        }
        private IMapper ConfigureAutoMapper()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<ProductModel, ProductDisplayModel>();
                cfg.CreateMap<CartItemModel, CartItemDisplayModel>();
            });
            var mapper = config.CreateMapper();
            return mapper;
        }

        private IConfiguration AddConfiguration()
        {
            IConfigurationBuilder builder = 
                new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json");
#if DEBUG
            builder.AddJsonFile("appsettings.Development.json",optional:true, reloadOnChange:true);
#else
            builder.AddJsonFile("appsettings.Production.json",optional:true, reloadOnChange:true);
#endif
            return builder.Build();
        }

        protected override void Configure()
        {
            IMapper mapper = ConfigureAutoMapper();
            _container.Instance(mapper);

            _container.Instance(_container)
                .PerRequest<ISaleEndpoint, SaleEndpoint>()
                .PerRequest<IUserEndpoint, UserEndpoint>()
                .PerRequest<IProductEndpoint, ProductEndpoint>();

            _container
                .Singleton<IWindowManager, WindowManager>()
                .Singleton<IEventAggregator, EventAggregator>()
                .Singleton<ILoggedInUserModel, LoggedInUserModel>()
                .Singleton<IApiHelper, ApiHelper>();

            _container.RegisterInstance(typeof(IConfiguration), "IConfiguration",AddConfiguration());

            //TODO change it later to something more ordinary

            //use reflection to wire up all viewModels to the container
            GetType().Assembly.GetTypes()
                .Where(type => type.IsClass && type.Name.EndsWith("ViewModel"))
                .ToList()
                .ForEach(viewModelType => _container.RegisterPerRequest(
                    viewModelType, viewModelType.ToString(), viewModelType));
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<ShellViewModel>();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }
    }
}

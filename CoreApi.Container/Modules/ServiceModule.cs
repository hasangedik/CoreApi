using System.Reflection;
using Autofac;
using CoreApi.Core.ExternalService.Abstract;
using CoreApi.Core.ExternalService.Concrete;
using CoreApi.Core.Service.Concrete;
using Module = Autofac.Module;

namespace CoreApi.Container.Modules
{
    public class ServiceModule : Module
    {
        private readonly AutoMapper.IMapper _mapper;

        public ServiceModule(AutoMapper.IMapper mapper)
        {
            _mapper = mapper;
        }

        public ServiceModule()
        {
        }

        protected override void Load(ContainerBuilder builder)
        {
            var coreAssemblyType = typeof(UserService).GetTypeInfo();

            if (_mapper != null)
            {
                builder.RegisterAssemblyTypes(coreAssemblyType.Assembly)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope()
                    .WithParameter("mapper", _mapper);
            }
            else
            {
                builder.RegisterAssemblyTypes(coreAssemblyType.Assembly)
                    .AsImplementedInterfaces()
                    .InstancePerLifetimeScope();
            }

            builder
                .RegisterType<Synchronizer>()
                .As<ISynchronizer>()
                .SingleInstance();

            base.Load(builder);
        }
    }
}
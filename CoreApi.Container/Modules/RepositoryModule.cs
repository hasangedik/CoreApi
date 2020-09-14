using System.Reflection;
using Autofac;
using CoreApi.Infrastructure.Context.Persistence;
using Module = Autofac.Module;

namespace CoreApi.Container.Modules
{
    public class RepositoryModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assemblyType = typeof(UserRepository).GetTypeInfo();

            builder.RegisterAssemblyTypes(assemblyType.Assembly)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}

using Autofac;
using Module = Autofac.Module;
using NLayer.Repository;
using NLayer.Service.Mapping;
using System.Reflection;
using NLayer.Repository.Repositories;
using NLayer.Core.Repositories;
using NLayer.Service.Services;
using NLayer.Core.Services;
using NLayer.Repository.UnitOfWorks;
using NLayer.Core.UnitOfWorks;

namespace NLayer.API.Modules
{
    public class RepoServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            #region TEK OLANLARI EKLEMEK İÇİN
            //GENERIC OLANLAR
            builder.RegisterGeneric(typeof(GenericRepository<>)).As(typeof(IGenericRepository<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Service<>)).As(typeof(IService<>)).InstancePerLifetimeScope();


            //GENERİC OLMAYANLAR
            builder.RegisterType<UnitOfWork>().As<IUnitOfWork>().InstancePerLifetimeScope();
            #endregion

            #region ASSEMBLY

            //Üzerinde çalışmış olduğun assembly al;
            //(API KATMANI)
            var apiAssembly = Assembly.GetEntryAssembly();

            //REPOSITORY KATMANI // repo katmanındaki herhangi bir classın adını parametre olarak vereceğiz
            var repoAssembly = Assembly.GetAssembly(typeof(AppDbContext));

            //SERVİCE KATMANI
            var serviceAssembly = Assembly.GetAssembly(typeof(MapProfile));

            // bizim interface lerimizin sonu ve classlarımızın sonu hep Repository ile bitiyor. classları  al ve interfacelerini implement et
            // InstancePerLifetimeScope => Scope
            //REPOSITORY KATMANI
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Repository")).AsImplementedInterfaces().InstancePerLifetimeScope();


            //SERVİCE KATMANI
            builder.RegisterAssemblyTypes(apiAssembly, repoAssembly, serviceAssembly).Where(x => x.Name.EndsWith("Service")).AsImplementedInterfaces().InstancePerLifetimeScope();

            #endregion
        }
    }
}

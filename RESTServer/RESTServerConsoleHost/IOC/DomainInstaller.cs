using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Models;
using RESTServerConsoleHost.Repositories;

namespace RESTServerConsoleHost.IOC
{
    public class DomainInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component
                .For(typeof(IRepository<Person,int>))
                .ImplementedBy(typeof(InMemoryPersonRepository))
                .LifestyleSingleton());

            container.Register(Component
                .For(typeof(IRepository<Account,int>))
                .ImplementedBy(typeof(InMemoryAccountRepository))
                .LifestyleSingleton());
            
            container.Register(Component
                .For(typeof(IRepository<User,int>))
                .ImplementedBy(typeof(InMemoryUserRepository))
                .LifestyleSingleton());
        }
    }
}

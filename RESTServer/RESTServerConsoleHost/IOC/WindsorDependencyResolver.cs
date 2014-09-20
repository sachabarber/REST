using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using RESTServer.IOC;

namespace RESTServerConsoleHost.IOC
{
    public class WindsorDependencyResolver : IDependencyResolver
    {
        private readonly IWindsorContainer container;

        public WindsorDependencyResolver(IWindsorContainer container)
        {
            this.container = container;
        }

        public object GetService(Type serviceType)
        {
            return container.Kernel.HasComponent(serviceType) ? container.Resolve(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return container.Kernel.HasComponent(serviceType) ? 
                container.ResolveAll(serviceType).Cast<object>() : new object[] { };
        }
    }
}

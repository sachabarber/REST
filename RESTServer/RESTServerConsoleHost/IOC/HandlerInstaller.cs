using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Models;
using RESTServer.Handlers;
using RESTServerConsoleHost.Repositories;

namespace RESTServerConsoleHost.IOC
{
    public class HandlerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            foreach (var item in this.GetType().Assembly.GetTypes())
            {
                if (item.GetInterfaces().Any(x => x.FullName.Contains("IHandler")))
                {
                    AddHandler(container, item);
                }


                //if(item.GetInterfaces().Any(x => x.FullName.Contains("IVerbHandler")))
                //{
                //    AddVerbHandler(container, item);
                //}
            }
        }


        private void AddHandler(IWindsorContainer container, Type item)
        {
            container.Register(Component
            .For(typeof(IHandler))
            .ImplementedBy(item)
            .LifestyleTransient());
        }



        //private void AddVerbHandler(IWindsorContainer container, Type item)
        //{

        //    var ints = item.GetInterfaces();
        //    var verbInterface = item.GetInterfaces().Where(x => x.FullName.Contains("IVerbHandler")).Single();
        //    var genericArgs = verbInterface.GenericTypeArguments;



        //    MethodInfo method = typeof(HandlerInstaller).GetMethod("RegisterVerbHandler",
        //        BindingFlags.NonPublic | BindingFlags.Instance);
        //    MethodInfo generic = method.MakeGenericMethod(genericArgs[0], genericArgs[1]);
        //    generic.Invoke(this, new object[] { container, item });

        //}

        //private void RegisterVerbHandler<T, TKey>(IWindsorContainer container, Type concreteType)
        //{
        //    container.Register(Component
        //         .For(typeof(IVerbHandler<T, TKey>))
        //         .ImplementedBy(concreteType)
        //         .LifestyleTransient());

        //    container.Register(Component
        //        .For(typeof(IHandler))
        //        .ImplementedBy(concreteType)
        //        .LifestyleTransient());
        //}

    }


}

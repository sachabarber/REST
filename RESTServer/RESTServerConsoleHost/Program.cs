using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Castle.Windsor;
using Models;
using RESTServer;
using RESTServer.Handlers;
using RESTServerConsoleHost.IOC;

namespace RESTServerConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
            Console.ReadLine();
            
        }



        public void Run()
        {
            //build the container

            Task.Run(() =>
                {

                    IWindsorContainer container = new WindsorContainer();
                    container.Install(new DomainInstaller(), new HandlerInstaller());

                    //run the server
                    HttpServer httpServer = new HttpServer(new WindsorDependencyResolver(container));
                    httpServer.Run(new string[] { @"http://localhost:8001/" });
                });
        }


        
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RESTServer.Handlers;
using RESTServer.IOC;
using RESTServer.Routing;

namespace RESTServer
{
    public class HttpServer
    {

        private readonly HttpListener listener = new HttpListener();
        private readonly int accepts = 4;
        private readonly IDependencyResolver dependencyResolver;
        private Semaphore sem;

        public HttpServer(IDependencyResolver dependencyResolver, int accepts = 4)
        {

            listener.IgnoreWriteExceptions = true;

            // Multiply by number of cores:
            this.accepts = accepts * Environment.ProcessorCount;

            this.dependencyResolver = dependencyResolver;
        }


        public async void Run(params string[] uriPrefixes)
        {
            // Add the server bindings:
            foreach (var prefix in uriPrefixes)
                listener.Prefixes.Add(prefix);
            listener.Start();

            //Accept connections:
            //1. Higher values mean more connections can be maintained yet at a 
            //   much slower average response time; fewer connections will be rejected.
            //2. Lower values mean less connections can be maintained yet at a 
            //   much faster average response time; more connections will be rejected.
            sem = new Semaphore(accepts, accepts);
            await RunServer();

        }


        private async Task RunServer()
        {
            while (true)
            {
                // Fall through until we've initialized all our connection listeners.
                // When the semaphore blocks (its count reaches 0) we wait until a connection occurs, 
                // upon which the semaphore is released and we create another connection "awaiter."
                sem.WaitOne();
                await StartConnectionListener();
            }
        }
 
        private async Task StartConnectionListener()
        {
            // Wait for a connection
            HttpListenerContext context = await listener.GetContextAsync();
 
            // Allow a new connection listener to be set up.
            sem.Release();

            //process the current request
            await ProcessRequest(context);
        }


        private async Task<bool> ProcessRequest(HttpListenerContext context)
        {
            // Setup Chain of Responsibility for route processing
            VerbRouteActioner verbRouteActioner = new VerbRouteActioner();
            DynamicRouteActioner dynamicRouteActioner = new DynamicRouteActioner();
            BadRouteActioner badRouteActioner = new BadRouteActioner();
            verbRouteActioner.Successor = dynamicRouteActioner;
            dynamicRouteActioner.Successor = badRouteActioner;


            var handlers = dependencyResolver.GetServices(typeof(IHandler)).Cast<IHandler>().ToList();


            await verbRouteActioner.ActionRequest(context, handlers);

            return true;
        }
    }
}

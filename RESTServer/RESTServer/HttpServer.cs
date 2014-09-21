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
             //Higher values mean more connections can be maintained yet at a much slower average response time; fewer connections will be rejected.
             //Lower values mean less connections can be maintained yet at a much faster average response time; more connections will be rejected.
            var sem = new Semaphore(accepts, accepts);


            while (true)
            {
                    sem.WaitOne();

                    await listener.GetContextAsync().ContinueWith(async (t) =>
                    {
                        string errMessage;

                        try
                        {
                            sem.Release();
                            var context = await t;
                            await Task.Run(() => ProcessRequest(context));
                            return;
                        }
                        catch (Exception ex)
                        {
                            errMessage = ex.ToString();
                        }

                        await Console.Error.WriteLineAsync(errMessage);
                    });

                
            }
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

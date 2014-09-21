using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RESTServer.Handlers;

namespace RESTServer.Routing
{
    public abstract class RouteActioner
    {
        public abstract Task<bool> ActionRequest(HttpListenerContext context, IList<IHandler> handlers);
        public RouteActioner Successor { set; protected get; }
    }
}

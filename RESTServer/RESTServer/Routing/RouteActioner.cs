using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Handlers;
using RESTServer.Serialization;

namespace RESTServer.Routing
{
    public abstract class RouteActioner
    {
        public abstract Task<bool> ActionRequest(HttpListenerContext context, IEnumerable<IHandler> handlers);
        public RouteActioner Successor { set; protected get; }
    }
}

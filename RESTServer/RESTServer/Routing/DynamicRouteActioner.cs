using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Handlers;

namespace RESTServer.Routing
{
    public class DynamicRouteActioner : RouteActioner
    {
        RestMethodActioner restMethodActioner = new RestMethodActioner();



        public override async Task<bool> ActionRequest(System.Net.HttpListenerContext context, IEnumerable<IHandler> handlers)
        {
            object handler = null;




            return true;

        }


    }
}

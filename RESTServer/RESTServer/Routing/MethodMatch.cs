using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Routing
{
    public class MethodMatch
    {
        public MethodMatch(MethodInfo method, RouteAttribute route)
        {
            Method = method;
            Route = route;
        }

        public MethodInfo Method { get; private set; }
        public RouteAttribute Route { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Utils.Serialization;

namespace RESTServer.Routing
{
    public class RouteResult
    {
        public object Handler { get; set; }
        public SerializationToUse SerializationToUse { get; set; }
    }
}

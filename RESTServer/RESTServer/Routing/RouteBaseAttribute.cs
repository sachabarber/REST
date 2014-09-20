using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Serialization;

namespace RESTServer
{

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class RouteBaseAttribute : System.Attribute
    {

        public RouteBaseAttribute(string urlBase, SerializationToUse serializationToUse)
        {
            this.UrlBase = urlBase;
            this.SerializationToUse = serializationToUse;
        }

        public string UrlBase { get; private set; }
        public SerializationToUse SerializationToUse { get; private set; }
    }
}

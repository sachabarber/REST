using RESTServer.Utils.Serialization;

namespace RESTServer
{
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class RouteAttribute : System.Attribute
    {

        public RouteAttribute(string route, string httpVerb, SerializationToUse serializationToUse = SerializationToUse.UseContentType)
        {
            this.Route = route;
            this.HttpVerb = httpVerb;
            this.SerializationToUse = serializationToUse;
        }

        public string Route { get; private set; }
        public string HttpVerb { get; private set; }
        public SerializationToUse SerializationToUse { get; private set; }
    }
}

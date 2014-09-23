using RESTServer.Routing;
using RESTServer.Utils.Serialization;

namespace RESTServer
{
    /// <summary>
    /// An attribute to specify a dynamic portion of a route that maps
    /// to HttpMethod
    /// Example:
    /// [Route("/GetAllUsers", HttpMethod.Get)]
    /// </summary>
    [System.AttributeUsage(System.AttributeTargets.Method)]
    public class RouteAttribute : System.Attribute
    {

        public RouteAttribute(string route, HttpMethod httpVerb, SerializationToUse serializationToUse = SerializationToUse.UseContentType)
        {
            this.Route = route;
            this.HttpVerb = httpVerb;
            this.SerializationToUse = serializationToUse;
        }

        public string Route { get; private set; }
        public HttpMethod HttpVerb { get; private set; }
        public SerializationToUse SerializationToUse { get; private set; }
    }
}

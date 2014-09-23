using RESTServer.Utils.Serialization;

namespace RESTServer
{
    /// <summary>
    /// An attribute to specify a base portion of a route
    /// to HttpMethod, and also allows the specification of
    /// the serialization to use
    /// Example:
    /// [RouteBase("/users", SerializationToUse.Json)]
    /// </summary>
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

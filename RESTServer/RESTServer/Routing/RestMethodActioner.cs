using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RESTServer.Utils.Serialization;

namespace RESTServer.Routing
{
    public class RestMethodActioner
    {
        private ISerializer xmlPipelineSerializer = new XmlPipelineSerializer();
        private ISerializer jsonPipelineSerializer = new JsonPipelineSerializer();


        public bool IsUrlMatch(string baseRoute, string requestUrl, string httpMethod)
        {
            string restToken = baseRoute.Replace(@"/", "");
            string pattern = string.Format("^(\\/{0}\\/)([1-9]+[0-9]*)$", restToken);
            Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match m = regEx.Match(requestUrl);


            if (httpMethod == "POST")
            {
                return requestUrl == baseRoute;
            }

            if (httpMethod == "GET")
            {
                bool validRoute1 = requestUrl == baseRoute;
                bool validRoute2 = m.Success;
                return validRoute1 || validRoute2;
            }

            if (httpMethod == "PUT" || httpMethod == "DELETE")
            {
                bool validRoute1 = requestUrl == baseRoute;
                bool validRoute2 = m.Success;
                return validRoute1 || validRoute2;
            }

            return false;
        }



        public bool IsGetAll(HttpListenerRequest request)
        {
            return !char.IsDigit(request.RawUrl.Last());

        }

        public async Task<T> ExtractContent<T>(HttpListenerRequest request, SerializationToUse serializationToUse)
        {
            using (StreamReader sr = new StreamReader(request.InputStream))
            {
                string rawData = sr.ReadToEnd();
                switch(serializationToUse)
                {
                    case SerializationToUse.Json:
                        return await DeSerialize<T>(rawData,jsonPipelineSerializer);
                    case SerializationToUse.Xml:
                        return await DeSerialize<T>(rawData, xmlPipelineSerializer);
                    default:
                        return await ObtainDeSerializedItemFromBodyContentType<T>(rawData, request.ContentType);
                }
            }            
        }

        public TKey ExtractId<TKey>(HttpListenerRequest request)
        {
            var cutPoint = request.RawUrl.LastIndexOf(@"/") + 1;
            var rawId = request.RawUrl.Substring(cutPoint);
            return (TKey)Convert.ChangeType(rawId, typeof(TKey));
        }


        public async Task<T> DeSerialize<T>(string rawBodyData, ISerializer serializer)
        {
            return await serializer.Deserialize<T>(rawBodyData);
        }


        public async Task<string> Serialize<T>(T item, ISerializer serializer)
        {
            return await serializer.Serialize<T>(item);
        }


        public ISerializer ObtainSerializer(SerializationToUse serializationToUse, string contentType)
        {
            if (contentType == "application/json" || serializationToUse == SerializationToUse.Json)
            {
                return jsonPipelineSerializer;
            }

            if (contentType == "application/xml" || serializationToUse == SerializationToUse.Xml)
            {
                return xmlPipelineSerializer;
            }

            throw new InvalidOperationException(
                "Can only deserialize using either 'application/json' or 'application/xml' content types");
        }

        private async Task<T> ObtainDeSerializedItemFromBodyContentType<T>(string rawData, string contentType)
        {
            if (contentType == "application/json")
            {
                return await DeSerialize<T>(rawData, jsonPipelineSerializer);
            }

            if (contentType == "application/xml")
            {
                return await DeSerialize<T>(rawData, xmlPipelineSerializer);
            }

            throw new InvalidOperationException(
                "Can only deserialize using either 'application/json' or 'application/xml' content types");
        }


        
    }
}

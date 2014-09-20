using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Serialization;

namespace RESTServer.Routing
{
    public class RestMethodActioner
    {
        private ISerializer xmlPipelineSerializer = new XmlPipelineSerializer();
        private ISerializer jsonPipelineSerializer = new JsonPipelineSerializer();


        public bool IsGetAll(HttpListenerRequest request)
        {
            return request.RawUrl.EndsWith(@"/");
        }

        public T ExtractContent<T>(HttpListenerRequest request, SerializationToUse serializationToUse)
        {
            request.InputStream.Position = 0;
            using (StreamReader sr = new StreamReader(request.InputStream))
            {
                string rawData = sr.ReadToEnd();
                switch(serializationToUse)
                {
                    case SerializationToUse.Json:
                        return DeSerialize<T>(rawData,jsonPipelineSerializer);
                    case SerializationToUse.Xml:
                        return DeSerialize<T>(rawData,xmlPipelineSerializer);
                    default:
                        return ObtainDeSerializedItemFromBodyContentType<T>(rawData,request.ContentType);
                }
            }            
        }

        public TKey ExtractId<TKey>(HttpListenerRequest request)
        {
            var cutPoint = request.RawUrl.LastIndexOf(@"/") + 1;
            var rawId = request.RawUrl.Substring(cutPoint);
            return (TKey)Convert.ChangeType(rawId, typeof(TKey));
        }


        public T DeSerialize<T>(string rawBodyData, ISerializer serializer)
        {
            return (T)serializer.Deserialize<T>(rawBodyData);
        }


        public string Serialize<T>(T item, ISerializer serializer)
        {
            return serializer.Serialize<T>(item);
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

        private T ObtainDeSerializedItemFromBodyContentType<T>(string rawData, string contentType)
        {
            if (contentType == "application/json")
            {
                return DeSerialize<T>(rawData, jsonPipelineSerializer);
            }

            if (contentType == "application/xml")
            {
                return DeSerialize<T>(rawData, xmlPipelineSerializer);
            }

            throw new InvalidOperationException(
                "Can only deserialize using either 'application/json' or 'application/xml' content types");
        }


        
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RESTServer.Handlers;
using RESTServer.Utils.Serialization;

namespace RESTServer.Routing
{
    public class RestMethodActioner
    {
        private ISerializer xmlPipelineSerializer = new XmlPipelineSerializer();
        private ISerializer jsonPipelineSerializer = new JsonPipelineSerializer();


        public async Task<bool> IsUrlMatch(string baseRoute, string requestUrl, string httpMethod)
        {
            return await Task.Run(() =>
            {
                bool result = false;
                string restToken = baseRoute.Replace(@"/", "");
                string pattern = string.Format("^(\\/{0}\\/)([1-9]+[0-9]*)$", restToken);
                Regex regEx = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                Match m = regEx.Match(requestUrl);


                if (httpMethod == "POST")
                {
                    result= requestUrl == baseRoute;
                }

                if (httpMethod == "GET")
                {
                    bool validRoute1 = requestUrl == baseRoute;
                    bool validRoute2 = m.Success;
                    result= validRoute1 || validRoute2;
                }

                if (httpMethod == "PUT" || httpMethod == "DELETE")
                {
                    bool validRoute1 = requestUrl == baseRoute;
                    bool validRoute2 = m.Success;
                    result= validRoute1 || validRoute2;
                }

                return result;
            });
        }

        public async Task<RouteResult> FindHandler(Type handlerTypeRequired,
            HttpListenerContext context, IList<IHandler> handlers, bool isDynamicHandler)
        {

            return await Task.Run(async () =>
            {
                var httpMethod = context.Request.HttpMethod;
                var url = context.Request.RawUrl;
                RouteResult result = new RouteResult();
                foreach (var handler in handlers)
                {
                    if (handler.GetType().GetInterfaces().Any(x => x.Name == handlerTypeRequired.Name))
                    {
                        var routeBase = (RouteBaseAttribute[]) handler.GetType()
                            .GetCustomAttributes(typeof (RouteBaseAttribute), false);

                        if (routeBase.Length > 0)
                        {
                            bool isBaseMatch = url.StartsWith(routeBase[0].UrlBase);
                            bool isUrlMatch = await IsUrlMatch(routeBase[0].UrlBase, url, httpMethod);
                            if (isBaseMatch && (isUrlMatch || isDynamicHandler))
                            {
                                result.SerializationToUse = routeBase[0].SerializationToUse;
                                result.Handler = handler;
                                break;
                            }
                        }
                    }
                }
                return result;

            });

        }


        public async Task<bool> SetResponse<T>(HttpListenerContext context, T result, 
            SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                HttpListenerResponse response = context.Response;
                using (System.IO.Stream output = response.OutputStream)
                {
                    ISerializer serializer = ObtainSerializer(serializationToUse, context.Request.ContentType);
                    string serialized = await serializer.Serialize<T>(result);
                    var buffer = Encoding.UTF8.GetBytes(serialized);
                    output.Write(buffer, 0, buffer.Length);
                    response.StatusCode = 200;
                    response.StatusDescription = Enum.GetName(typeof (HttpStatusCode), HttpStatusCode.OK);
                }
                return true;
            });

        }

        public async Task<bool> SetOkResponse(HttpListenerContext context)
        {
            return await Task.Run(async () =>
            {
                HttpListenerResponse response = context.Response;
                using (System.IO.Stream output = response.OutputStream)
                {
                    response.StatusCode = 200;
                    response.StatusDescription = Enum.GetName(typeof (HttpStatusCode), HttpStatusCode.OK);
                }
                return true;
            });
        }

        public bool IsGetAll(string url)
        {
            return !char.IsDigit(url.Last());
        }

        public async Task<T> ExtractContent<T>(HttpListenerRequest request, 
            SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                using (StreamReader sr = new StreamReader(request.InputStream))
                {
                    string rawData = sr.ReadToEnd();
                    switch (serializationToUse)
                    {
                        case SerializationToUse.Json:
                            return await DeSerialize<T>(rawData, jsonPipelineSerializer);
                        case SerializationToUse.Xml:
                            return await DeSerialize<T>(rawData, xmlPipelineSerializer);
                        default:
                            return await ObtainDeSerializedItemFromBodyContentType<T>(rawData, request.ContentType);
                    }
                }
            });
        }

        public async Task<TKey> ExtractId<TKey>(HttpListenerRequest request)
        {
            return await Task.Run(async () =>
            {
                var cutPoint = request.RawUrl.LastIndexOf(@"/") + 1;
                var rawId = request.RawUrl.Substring(cutPoint);
                return (TKey)Convert.ChangeType(rawId, typeof(TKey));
            });
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RESTServer.Utils.Serialization;

namespace RESTServer.Utils.Client
{
    public class RESTWebClient : WebClient
    {
        private WebRequest request = null;
        private XmlPipelineSerializer xmlPipelineSerializer = new XmlPipelineSerializer();
        private JsonPipelineSerializer jsonPipelineSerializer = new JsonPipelineSerializer();

        protected override WebRequest GetWebRequest(Uri address)
        {
            this.request = base.GetWebRequest(address);

            if (this.request is HttpWebRequest)
            {
                ((HttpWebRequest)this.request).AllowAutoRedirect = false;
            }

            return this.request;
        }

        public async Task<RESTResponse<T>> Get<T>(string url, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                string response = await Task.Run(() => DownloadString(url));
                return await CreateResponse<T>(response, serializationToUse);
            });
        }

        public async Task<RESTResponse<T>> Post<T>(string url, T item, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                byte[] responsebytes = await UploadDataForMethod(url, "POST", item, serializationToUse);
                string responsebody = string.Empty;
                if (serializationToUse == SerializationToUse.Xml)
                {
                    responsebody = Encoding.UTF8.GetString(responsebytes);
                }
                if (serializationToUse == SerializationToUse.Json)
                {
                    responsebody = Encoding.UTF8.GetString(responsebytes);
                }
                return await CreateResponse<T>(responsebody, serializationToUse);
            });
        }

        public async Task<HttpStatusCode> Delete(string url)
        {
            return await Task.Run(async () =>
            {
                var request = WebRequest.Create(url);
                request.Method = "DELETE";
                var response = await request.GetResponseAsync();
                return ((HttpWebResponse) response).StatusCode;
            });
        }


        public async Task<HttpStatusCode> Put<T>(string url, T item, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                await UploadDataForMethod(url, "PUT", item, serializationToUse);
                return await StatusCode();
            });
        }




        private async Task<byte[]> UploadDataForMethod<T>(string url, string httpMethod, T item, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                if (serializationToUse == SerializationToUse.Xml)
                {
                    Headers.Add("Content-Type", "application/xml");
                    var serialized = await xmlPipelineSerializer.SerializeAsBytes(item);
                    return await Task.Run(() => UploadData(url, httpMethod, serialized));
                }
                if (serializationToUse == SerializationToUse.Json)
                {
                    Headers.Add("Content-Type", "application/json");
                    var serialized = await jsonPipelineSerializer.SerializeAsBytes(item);
                    return await Task.Run(() => UploadData(url, httpMethod, serialized));
                }
                throw new InvalidOperationException("You need to specify either Xml or Json serialization");
            });

        }


        private async Task<HttpStatusCode> StatusCode()
        {
            return await Task.Run(() =>
            {
                if (this.request == null)
                {
                    throw (new InvalidOperationException(
                        "Unable to retrieve the status code, maybe you haven't made a request yet."));
                }

                HttpWebResponse response = base.GetWebResponse(this.request) as HttpWebResponse;

                if (response != null)
                {
                    return response.StatusCode;
                }
                throw (new InvalidOperationException(
                    "Unable to retrieve the status code, maybe you haven't made a request yet."));
            });
        }

        private async Task<RESTResponse<T>> CreateResponse<T>(string response, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                if (serializationToUse == SerializationToUse.Xml)
                {
                    return new RESTResponse<T>()
                    {
                        Content = await xmlPipelineSerializer.Deserialize<T>(response),
                        StatusCode = await StatusCode()
                    };
                }
                if (serializationToUse == SerializationToUse.Json)
                {
                    return new RESTResponse<T>()
                    {
                        Content = await jsonPipelineSerializer.Deserialize<T>(response),
                        StatusCode = await StatusCode()
                    };
                }
                throw new InvalidOperationException("You need to specify either Xml or Json serialization");
            });
        }

    }
}

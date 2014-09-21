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


        protected override WebRequest GetWebRequest(Uri address)
        {
            this.request = base.GetWebRequest(address);

            if (this.request is HttpWebRequest)
            {
                ((HttpWebRequest)this.request).AllowAutoRedirect = false;
            }

            return this.request;
        }

        public RESTResponse<T> Get<T>(string url, SerializationToUse serializationToUse)
        {
            string response = DownloadString(url);

            if (serializationToUse == SerializationToUse.Xml)
            {
                return new RESTResponse<T>()
                {
                    Content = xmlPipelineSerializer.Deserialize<T>(response),
                    StatusCode = StatusCode()
                };
            }
            if (serializationToUse == SerializationToUse.Json)
            {
                return new RESTResponse<T>()
                {
                    Content = JsonConvert.DeserializeObject<T>(response),
                    StatusCode = StatusCode()
                };
            }
    
            
            throw new InvalidOperationException("You need to specify either Xml or Json serialization");

        }


        public RESTResponse<T> Post<T>(string url, T item, SerializationToUse serializationToUse)
        {
            if (serializationToUse == SerializationToUse.Xml)
            {
                Headers.Add("Content-Type", "application/xml");
                byte[] responsebytes = UploadData(url, "POST",
                  xmlPipelineSerializer.SerializeAsBytes(item));
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                return new RESTResponse<T>()
                {
                    Content = xmlPipelineSerializer.Deserialize<T>(responsebody),
                    StatusCode = StatusCode()
                };
            }
            if (serializationToUse == SerializationToUse.Json)
            {
                Headers.Add("Content-Type", "application/json");
                byte[] responsebytes = UploadData(url, "POST",
                  Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)));
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                return new RESTResponse<T>()
                {
                    Content = JsonConvert.DeserializeObject<T>(responsebody),
                    StatusCode = StatusCode()
                };
            }


            throw new InvalidOperationException("You need to specify either Xml or Json serialization");
        }

        private HttpStatusCode StatusCode()
        {
            HttpStatusCode result;

            if (this.request == null)
            {
                throw (new InvalidOperationException(
                    "Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            HttpWebResponse response = base.GetWebResponse(this.request) as HttpWebResponse;

            if (response != null)
            {
                result = response.StatusCode;
            }
            else
            {
                throw (new InvalidOperationException(
                    "Unable to retrieve the status code, maybe you haven't made a request yet."));
            }

            return result;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RESTServer.Utils.Serialization
{
    public class JsonPipelineSerializer : ISerializer
    {
        public T Deserialize<T>(string rawBodyData)
        {
            return JsonConvert.DeserializeObject<T>(rawBodyData);
        }

        public string Serialize<T>(T item)
        {
            return JsonConvert.SerializeObject(item);
        }
    }
}

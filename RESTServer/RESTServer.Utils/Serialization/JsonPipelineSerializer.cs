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
        public async Task<T> Deserialize<T>(string rawBodyData)
        {
            return await Task.Run(() => JsonConvert.DeserializeObject<T>(rawBodyData));
        }

        public async Task<Byte[]> SerializeAsBytes<T>(T item)
        {
            return await Task.Run(() => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(item)));
        }
        public async Task<string> Serialize<T>(T item)
        {
            return await Task.Run(() => JsonConvert.SerializeObject(item));
        }
    }
}

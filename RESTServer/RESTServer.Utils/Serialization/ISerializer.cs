using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Utils.Serialization
{
    public interface ISerializer
    {
        Task<T> Deserialize<T>(string rawBodyData);
        Task<string> Serialize<T>(T item);
    }
}

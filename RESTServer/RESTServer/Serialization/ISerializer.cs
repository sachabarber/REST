using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Serialization
{
    public interface ISerializer
    {
        T Deserialize<T>(string rawBodyData);
        string Serialize<T>(T item);
    }
}

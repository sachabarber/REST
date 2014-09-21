using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Utils.Client
{
    public class RESTResponse<T>
    {
        public T Content { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}

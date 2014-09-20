using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Handlers
{
    /// <summary>
    /// Used as a marker interface when you want to supply your own custom routes
    /// by using the 
    /// </summary>
    public interface IDynamicRouteHandler : IHandler
    {
       
    }
}

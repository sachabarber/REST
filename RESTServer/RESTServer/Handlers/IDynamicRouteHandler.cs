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
    /// <typeparam name="T">An intention of the type of REST resource</typeparam>
    /// <typeparam name="TKey">An intention of the type of the Id field of the REST resource</typeparam>
    public interface IDynamicRouteHandler<T,TKey> : IHandler
    {
    }
}

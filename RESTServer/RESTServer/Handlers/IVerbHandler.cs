using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Handlers
{
    public interface IVerbHandler<T, TKey> : IHandler
    {
        Task<T> Get(TKey id);

        Task<IEnumerable<T>> Get();

        Task<T> Post(T item);

        Task<bool> Put(TKey id, T item);

        Task<bool> Delete(TKey id);
    }
}

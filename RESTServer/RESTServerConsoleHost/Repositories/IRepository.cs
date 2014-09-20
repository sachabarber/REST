using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServerConsoleHost.Repositories
{
    public interface IRepository<T, TKey>
    {
        T Get(TKey id);
        IEnumerable<T> GetAll();
        T Add(T item);
        bool Update(T item);
        bool Delete(TKey id);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTServer.Handlers
{
    /// <summary>
    /// A standard REST interface
    /// </summary>
    /// <typeparam name="T">An intention of the type of REST resource</typeparam>
    /// <typeparam name="TKey">An intention of the type of the Id field of the REST resource</typeparam>
    public interface IVerbHandler<T, TKey> : IHandler
    {
        /// <summary>
        /// Gets a REST resource by its Id
        /// </summary>
        Task<T> Get(TKey id);

        /// <summary>
        /// Gets all instances of REST resource
        /// </summary>
        Task<IEnumerable<T>> Get();

        /// <summary>
        /// Add a new REST resource. Where the newly added resource is returned
        /// </summary>
        Task<T> Post(T item);

        /// <summary>
        /// Updates the REST resource identified by its Id, with the new REST resource
        /// </summary>
        Task<bool> Put(TKey id, T item);

        /// <summary>
        /// Deletes a new REST resource by its Id
        /// </summary>
        Task<bool> Delete(TKey id);
    }
}

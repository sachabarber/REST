using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using RESTServer;
using RESTServer.Handlers;
using RESTServerConsoleHost.Repositories;
using RESTServer.Utils.Serialization;

namespace RESTServerConsoleHost.Handlers
{
    [RouteBase("/people", SerializationToUse.Xml)]
    public class PersonHandler : IVerbHandler<Person, int>
    {
        private readonly IRepository<Person,int> personRepository;

        public PersonHandler(IRepository<Person,int> personRepository)
        {
            this.personRepository = personRepository;
        }

        #region IVerbHandler<Person,int> Members

        public async Task<Person> Get(int id)
        {
            return await Task.Run(() => personRepository.Get(id));
        }

        public async Task<IEnumerable<Person>> Get()
        {
            return await Task.Run(() => personRepository.GetAll());
        }

        public async Task<Person> Post(Person item)
        {
            return await Task.Run(() => personRepository.Add(item));
        }

        public async Task<bool> Put(int id, Person item)
        {
            
            return await Task.Run(() =>
            {
                item.Id = id;
                return personRepository.Update(item);
            });
        }

        public async Task<bool> Delete(int id)
        {
            return await Task.Run(() => personRepository.Delete(id));
        }

        #endregion
    }
}

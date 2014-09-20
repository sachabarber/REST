using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RESTServer;
using RESTServer.Handlers;
using RESTServer.Serialization;
using RESTServerConsoleHost.Repositories;

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
            return personRepository.Get(id);
        }

        public async Task<IEnumerable<Person>> Get()
        {
            return personRepository.GetAll();
        }

        public async Task<Person> Post(Person item)
        {
            return personRepository.Add(item);
        }

        public async Task<bool> Put(int id, Person item)
        {
            item.Id = id;
            return personRepository.Update(item);
        }

        public async Task<bool> Delete(int id)
        {
            return personRepository.Delete(id);
        }

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RESTServer.Exceptions;

namespace RESTServerConsoleHost.Repositories
{
    public class InMemoryPersonRepository : IRepository<Person, int>
    {
        private List<Person> people = new List<Person>();


        public InMemoryPersonRepository()
        {
            people.Add(new Person()
            {
                Id = 1,
                FirstName = "Sam",
                LastName = "Beird"
            });

            people.Add(new Person()
            {
                Id = 2,
                FirstName = "John",
                LastName = "Hart"
            });
        }


        #region IRepository<Person> Members

        public Person Get(int id)
        {
            var person = people.SingleOrDefault(x => x.Id == id);
            if (person == null)
                throw new InvalidOperationException("Could not find correct item");
            else
                return person;
        }

        public IEnumerable<Person> GetAll()
        {
            return people;
        }

        public Person Add(Person item)
        {
            int id = people.Max(x => x.Id) + 1;
            item.Id = id;
            people.Add(item);
            return item;
        }

        public bool Update(Person item)
        {
            if (item == null)
            {
                throw new InvalidOperationException("item");
            }
            int index = people.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            people.RemoveAt(index);
            people.Add(item);
            return true;
        }

        public bool Delete(int id)
        {
            if (id < 0)
                throw new InvalidOperationException(
                    "Delete MUST be provided with an Id value >= 0");

            var person = people.SingleOrDefault(x => x.Id == id);
            if (person == null)
                throw new InvalidOperationException("Could not find correct item");
            
            people.Remove(person);
            return true;
        }

        #endregion
    }
}

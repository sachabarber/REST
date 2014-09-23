using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RESTServer.Exceptions;

namespace RESTServerConsoleHost.Repositories
{
    public class InMemoryUserRepository : IRepository<User, int>
    {
        private List<User> users = new List<User>();


        public InMemoryUserRepository()
        {
            users.Add(new User()
            {
                Id = 1,
                UserName = "Charles Bensi"
            });

            users.Add(new User()
            {
                Id = 2,
                UserName = "Tom Henki"
            });
        }


        #region IRepository<User> Members

        public User Get(int id)
        {
            var user = users.SingleOrDefault(x => x.Id == id);
            if (user == null)
                throw new InvalidOperationException("Could not find correct item");
            else
                return user;
        }

        public IEnumerable<User> GetAll()
        {
            return users;
        }

        public User Add(User item)
        {
            int id = users.Max(x => x.Id) + 1;
            item.Id = id;
            users.Add(item);
            return item;
        }

        public bool Update(User item)
        {
            if (item == null)
            {
                throw new InvalidOperationException("item");
            }
            int index = users.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            users.RemoveAt(index);
            users.Add(item);
            return true;

        }

        public bool Delete(int id)
        {
            if (id < 0)
                throw new InvalidOperationException(
                    "Delete MUST be provided with an Id value >= 0");

            var user = users.SingleOrDefault(x => x.Id == id);
            if (user == null)
                throw new InvalidOperationException("Could not find correct item");

            users.Remove(user);
            return true;
        }

        #endregion
    }
}

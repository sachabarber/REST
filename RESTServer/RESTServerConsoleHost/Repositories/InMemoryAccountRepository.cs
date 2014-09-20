using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using RESTServer.Exceptions;

namespace RESTServerConsoleHost.Repositories
{
    public class InMemoryAccountRepository : IRepository<Account, int>
    {
        private List<Account> accounts = new List<Account>();


        public InMemoryAccountRepository()
        {
            accounts.Add(new Account()
            {
                Id = 1,
                AccountNumber = "11558836",
                SortCode = "11-22-44"
            });

            accounts.Add(new Account()
            {
                Id = 2,
                AccountNumber = "12345678",
                SortCode = "22-88-78"
            });
        }


        #region IRepository<Account> Members

        public Account Get(int id)
        {
            var account = accounts.SingleOrDefault(x => x.Id == id);
            if (account == null)
                throw new InvalidOperationException("Could not find correct item");
            else
                return account;
        }

        public IEnumerable<Account> GetAll()
        {
            return accounts;
        }

        public Account Add(Account item)
        {
            int id = accounts.Max(x => x.Id) + 1;
            item.Id = id;
            accounts.Add(item);
            return item;
        }

        public bool Update(Account item)
        {
            if (item == null)
            {
                throw new HttpResponseException("item");
            }
            int index = accounts.FindIndex(p => p.Id == item.Id);
            if (index == -1)
            {
                return false;
            }
            accounts.RemoveAt(index);
            accounts.Add(item);
            return true;

        }

        public bool Delete(int id)
        {
            if (id < 0)
                throw new HttpResponseException(
                    "Delete MUST be provided with an Id value >= 0");

            var account = accounts.SingleOrDefault(x => x.Id == id);
            if (account == null)
                throw new HttpResponseException("Could not find correct item");

            accounts.Remove(account);
            return true;
        }

        #endregion
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
using RESTServer;
using RESTServer.Handlers;
using RESTServer.Utils.Serialization;
using RESTServerConsoleHost.Repositories;

namespace RESTServerConsoleHost.Handlers
{
    [RouteBase("/accounts", SerializationToUse.Json)]
    public class AccountHandler : IVerbHandler<Account, int>
    {
        private readonly IRepository<Account,int> accountRepository;

        public AccountHandler(IRepository<Account,int> accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        #region IVerbHandler<Account,int> Members

        public async Task<Account> Get(int id)
        {
            return await Task.Run(() => accountRepository.Get(id));
        }

        public async Task<IEnumerable<Account>> Get()
        {
            return await Task.Run(() => accountRepository.GetAll());
        }

        public async Task<Account> Post(Account item)
        {
            return await Task.Run(() => accountRepository.Add(item));
        }

        public async Task<bool> Put(int id, Account item)
        {
            return await Task.Run(() =>
            {
                item.Id = id;
                return accountRepository.Update(item);
            });
        }

        public async Task<bool> Delete(int id)
        {
            return await Task.Run(() => accountRepository.Delete(id));
        }

        #endregion
    }
}

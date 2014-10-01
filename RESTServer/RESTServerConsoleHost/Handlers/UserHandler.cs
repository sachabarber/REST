using System.Collections.Generic;
using System.Security.Policy;
using System.Threading.Tasks;
using Models;
using RESTServer;
using RESTServer.Handlers;
using RESTServer.Routing;
using RESTServerConsoleHost.Repositories;
using RESTServer.Utils.Serialization;

namespace RESTServerConsoleHost.Handlers
{
    [RouteBase("/users", SerializationToUse.Json)]
    public class UserHandler : IDynamicRouteHandler<User, int>
    {
        private readonly IRepository<User,int> userRepository;

        public UserHandler(IRepository<User,int> userRepository)
        {
            this.userRepository = userRepository;
        }

        #region IDynamicRouteHandler<User,int> Members


        [Route("/GetUserByTheirId/{0}", HttpMethod.Get)]
        public async Task<User> GetUserByTheirId(int id)
        {
            return await Task.Run(() => userRepository.Get(id));
        }

        [Route("/GetAllUsers", HttpMethod.Get)]
        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await Task.Run(() => userRepository.GetAll());
        }

        [Route("/AddASingleUser", HttpMethod.Post)]
        public async Task<User> AddASingleUser(User item)
        {
            return await Task.Run(() => userRepository.Add(item));
        }

        [Route("/UpdateAUserUsingId/{0}", HttpMethod.Put)]
        public async Task<bool> UpdateTheUserWithId(int id, User item)
        {
            return await Task.Run(() =>
            {
                item.Id = id;
                return userRepository.Update(item);
            });
        }

        [Route("/DeleteUserByTheirId/{0}", HttpMethod.Delete)]
        public async Task<bool> DeleteAUser(int id)
        {
            return await Task.Run(() => userRepository.Delete(id));
        }
   
        #endregion
    }
}

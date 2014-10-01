using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;
using RESTServer.Utils.Client;
using RESTServer.Utils.Serialization;

namespace RESTClientConsoleApp
{
    /// <summary>
    /// Demonstrates how to use the REST framework, along with a 
    /// DynamicRouteHandler server side handler
    /// for custom non standard Urls
    /// </summary>
    public class UserClient
    {
        /// <summary>
        /// Http : GET/{id}
        /// Gets item with the id specified
        /// </summary>
        public async Task GetUser(int id)
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = string.Format("http://localhost:8001/users/GetUserByTheirId/{0}", id);

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json
                    var response = await client.Get<User>(getUrl, SerializationToUse.Json);
                    Console.WriteLine("Http : GET/{id}");
                    Console.WriteLine("Status Code : {0}", response.StatusCode);
                    Console.WriteLine(getUrl);
                    Console.WriteLine(response.Content);
                    Console.WriteLine("=================================");
                }
            });
        }

        /// <summary>
        /// Http : GET
        /// Gets all items in the collection
        /// </summary>
        public async Task GetUsers()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = "http://localhost:8001/users/GetAllUsers";

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<List<User>>(getUrl, SerializationToUse.Json);

                    Console.WriteLine("Http : GET");
                    Console.WriteLine("Status Code : {0}", response.StatusCode);
                    Console.WriteLine(getUrl);
                    Console.WriteLine(response.Content.Count);
                    Console.WriteLine("=================================");
                }
            });
        }

        /// <summary>
        /// Http : POST
        /// </summary>
        public async Task PostUser()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string postUrl = "http://localhost:8001/users/AddASingleUser";
                    User newUser = new User();
                    newUser.UserName = string.Format("UserName_{0}", DateTime.Now.Ticks);

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Post<User>(postUrl, newUser, SerializationToUse.Json);
                    Console.WriteLine("Http : POST");
                    Console.WriteLine("Status Code : {0}", response.StatusCode);
                    Console.WriteLine(postUrl);
                    Console.WriteLine(response.Content);
                    Console.WriteLine("=================================");

                }
            });
        }

        /// <summary>
        /// Http : PUT
        /// </summary>
        public async Task PutUser()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING users/GetUserByTheirId/1");
                    string getUrl = string.Format("http://localhost:8001/users/GetUserByTheirId/{0}", 1);

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<User>(getUrl, SerializationToUse.Json);
                    var user = response.Content;
                    Console.WriteLine(user);

                    string newUserName = string.Format("{0}_Modified_{1}", user.UserName, DateTime.Now.Ticks);
                    user.UserName = newUserName;

                    string putUrl = string.Format("http://localhost:8001/users/UpdateAUserUsingId/{0}", 1);

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var statusCode = await client.Put(putUrl, user, SerializationToUse.Json);
                    Console.WriteLine("Http : PUT");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(putUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING users/GetUserByTheirId/1 again");
                        response = await client.Get<User>(getUrl, SerializationToUse.Json);
                        user = response.Content;
                        Console.WriteLine(user);

                    }
                    else
                    {
                        Console.WriteLine("PUT Failed");
                    }
                    Console.WriteLine("=================================");
                }
            });
        }

        public async Task DeleteUser()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING users");
                    string getUrl = "http://localhost:8001/users/GetAllUsers";

                    //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<List<User>>(getUrl, SerializationToUse.Json);
                    Console.WriteLine("There are currently {0} users", response.Content.Count);

                    string deleteUrl = string.Format("http://localhost:8001/users/DeleteUserByTheirId/{0}", 1);

                    var statusCode = await client.Delete(deleteUrl);
                    Console.WriteLine("Http : DELETE");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(deleteUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING users again");
                        //the server UserHandler [RouteBaseAttribute] is set to return Json, 
                        //so we need to deserialize it as Json 
                        response = await client.Get<List<User>>(getUrl, SerializationToUse.Json);
                        Console.WriteLine("There are currently {0} users", response.Content.Count);

                    }
                    else
                    {
                        Console.WriteLine("DELETE Failed");
                    }
                    Console.WriteLine("=================================");
                }
            });
        }

    }
}

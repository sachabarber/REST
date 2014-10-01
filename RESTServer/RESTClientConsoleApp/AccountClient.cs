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
    /// <c>IVerbHandlerOf<c>Account</c></c> server side handler
    /// for standard REST Urls
    /// </summary>
    public class AccountClient
    {
        /// <summary>
        /// Http : GET/{id}
        /// Gets item with the id specified
        /// </summary>
        public async Task GetAccount(int id)
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = string.Format("http://localhost:8001/accounts/{0}", id);

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json
                    var response = await client.Get<Account>(getUrl, SerializationToUse.Json);
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
        public async Task GetAccounts()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = "http://localhost:8001/accounts";

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<List<Account>>(getUrl, SerializationToUse.Json);

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
        public async Task PostAccount()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string postUrl = "http://localhost:8001/accounts";
                    Account newAccount = new Account();
                    newAccount.SortCode = string.Format("SortCode_{0}", DateTime.Now.Ticks);
                    newAccount.AccountNumber = string.Format("AccountNumber_{0}", DateTime.Now.Ticks);

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Post<Account>(postUrl, newAccount, SerializationToUse.Json);
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
        public async Task PutAccount()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING accounts/1");
                    string getUrl = string.Format("http://localhost:8001/accounts/{0}", 1);

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<Account>(getUrl, SerializationToUse.Json);
                    var account = response.Content;
                    Console.WriteLine(account);

                    string newAccountNumber = string.Format("{0}_Modified_{1}",
                        account.AccountNumber, DateTime.Now.Ticks);
                    account.AccountNumber = newAccountNumber;

                    string putUrl = string.Format("http://localhost:8001/accounts/{0}", 1);

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var statusCode = await client.Put(putUrl, account, SerializationToUse.Json);
                    Console.WriteLine("Http : PUT");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(putUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING accounts/1 again");
                        response = await client.Get<Account>(getUrl, SerializationToUse.Json);
                        account = response.Content;
                        Console.WriteLine(account);

                    }
                    else
                    {
                        Console.WriteLine("PUT Failed");
                    }
                    Console.WriteLine("=================================");
                }
            });
        }

        public async Task DeleteAccount()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING accounts");
                    string getUrl = string.Format("http://localhost:8001/accounts");

                    //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                    //so we need to deserialize it as Json 
                    var response = await client.Get<List<Account>>(getUrl, SerializationToUse.Json);
                    Console.WriteLine("There are currently {0} accounts", response.Content.Count);

                    string deleteUrl = string.Format("http://localhost:8001/accounts/{0}", 1);

                    var statusCode = await client.Delete(deleteUrl);
                    Console.WriteLine("Http : DELETE");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(deleteUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING accounts again");
                        //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                        //so we need to deserialize it as Json 
                        response = await client.Get<List<Account>>("http://localhost:8001/accounts",
                            SerializationToUse.Json);
                        Console.WriteLine("There are currently {0} accounts", response.Content.Count);

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

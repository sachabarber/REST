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
    public class AccountClient
    {
        /// <summary>
        /// Http : GET/{id}
        /// Gets item with the id specified
        /// </summary>
        public void GetAccount(int id)
        {
            using (RESTWebClient client = new RESTWebClient())
            {
                string getUrl = string.Format("http://localhost:8001/accounts/{0}", id);

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json
                var response = client.Get<Account>(getUrl, SerializationToUse.Json);
                Console.WriteLine("Http : GET/{id}");
                Console.WriteLine("Status Code : {0}", response.StatusCode);
                Console.WriteLine(getUrl);
                Console.WriteLine(response.Content);
                Console.WriteLine("=================================");
            }
        }

        /// <summary>
        /// Http : GET
        /// Gets all items in the collection
        /// </summary>
        public void GetAccounts()
        {
            using (RESTWebClient client = new RESTWebClient())
            {
                string getUrl = "http://localhost:8001/accounts";

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json 
                var response = client.Get<List<Account>>(getUrl, SerializationToUse.Json);

                Console.WriteLine("Http : GET");
                Console.WriteLine("Status Code : {0}", response.StatusCode);
                Console.WriteLine(getUrl);
                Console.WriteLine(response.Content.Count);
                Console.WriteLine("=================================");
            }
        }

        /// <summary>
        /// Http : POST
        /// </summary>
        public void PostAccount()
        {
            using (RESTWebClient client = new RESTWebClient())
            {
                string postUrl = "http://localhost:8001/accounts";
                Account newAccount = new Account();
                newAccount.SortCode = string.Format("SortCode_{0}", DateTime.Now.Ticks);
                newAccount.AccountNumber = string.Format("AccountNumber_{0}", DateTime.Now.Ticks);

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json 
                var response = client.Post<Account>(postUrl, newAccount, SerializationToUse.Json);
                Console.WriteLine("Http : POST");
                Console.WriteLine("Status Code : {0}", response.StatusCode);
                Console.WriteLine(postUrl);
                Console.WriteLine(response.Content);
                Console.WriteLine("=================================");

            }
        }

    }
}

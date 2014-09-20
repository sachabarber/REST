using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;

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
            using (WebClient client = new WebClient())
            {
                string getUrl = string.Format("http://localhost:8001/accounts/{0}", id);
                string response = client.DownloadString(getUrl);

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json
                Account account = JsonConvert.DeserializeObject<Account>(response);
                Console.WriteLine("Http : GET/{id}");
                Console.WriteLine(getUrl);
                Console.WriteLine(account);
                Console.WriteLine("=================================");
            }
        }

        /// <summary>
        /// Http : GET
        /// Gets all items in the collection
        /// </summary>
        public void GetAccounts()
        {
            using (WebClient client = new WebClient())
            {
                string getUrl = "http://localhost:8001/accounts";
                string response = client.DownloadString(getUrl);

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json 
                List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(response);
                Console.WriteLine("Http : GET");
                Console.WriteLine(getUrl);
                Console.WriteLine(accounts.Count);
                Console.WriteLine("=================================");
            }
        }
    }
}

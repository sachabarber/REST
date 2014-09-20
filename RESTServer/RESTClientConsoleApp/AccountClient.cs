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



        public void GetAccount(int id)
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(string.Format("http://localhost:8001/accounts/{0}", id));

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json
                Account account = JsonConvert.DeserializeObject<Account>(response);
            }
        }


        public void GetAccounts()
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString("http://localhost:8001/accounts");

                //the server AccountHandler [RouteBaseAttribute] is set to return Json, 
                //so we need to deserialize it as Json 
                List<Account> accounts = JsonConvert.DeserializeObject<List<Account>>(response);
            }
        }

       


        //public void GetUserJson(int id)
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //        string response = client.DownloadString(string.Format("http://localhost:8001/people/{0}",id));
        //        List<Person> people = JsonConvert.DeserializeObject<List<Person>>(response);
        //    }
        //}


        //public void GetUsersJson()
        //{
        //    using (WebClient client = new WebClient())
        //    {
        //        client.Headers[HttpRequestHeader.ContentType] = "application/json";
        //        string response = client.DownloadString("http://localhost:8001/people/");
        //        List<Person> people = JsonConvert.DeserializeObject<List<Person>>(response);
        //    }
        //}



        //POST
        //using (WebClient client = new WebClient())
        //{
        //    System.Collections.Specialized.NameValueCollection reqparm = new System.Collections.Specialized.NameValueCollection();
        //    reqparm.Add("param1", "<any> kinds & of = ? strings");
        //    reqparm.Add("param2", "escaping is already handled");
        //    byte[] responsebytes = client.UploadValues("http://localhost", "POST", reqparm);
        //    string responsebody = Encoding.UTF8.GetString(responsebytes);
        //}


       

    }
}

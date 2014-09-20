using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Models;
using Newtonsoft.Json;

namespace RESTClientConsoleApp
{
    public class PersonClient
    {
        private XmlPipelineSerializer serializer = new XmlPipelineSerializer();


        /// <summary>
        /// Http : GET/{id}
        /// Gets item with the id specified
        /// </summary>
        public void GetPerson(int id)
        {
            using (WebClient client = new WebClient())
            {
                string getUrl = string.Format("http://localhost:8001/people/{0}", id);
                string response = client.DownloadString(getUrl);

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person person = serializer.Deserialize<Person>(response);
                Console.WriteLine("Http : GET/{id}");
                Console.WriteLine(getUrl);
                Console.WriteLine(person);
                Console.WriteLine("=================================");
            }
        }


        /// <summary>
        /// Http : GET
        /// Gets all items in the collection
        /// </summary>
        public void GetPeople()
        {
            using (WebClient client = new WebClient())
            {
                string getUrl = "http://localhost:8001/people";
                string response = client.DownloadString(getUrl);

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                List<Person> people = serializer.Deserialize<List<Person>>(response);
                Console.WriteLine("Http : GET");
                Console.WriteLine(getUrl);
                Console.WriteLine(people.Count);
                Console.WriteLine("=================================");
            }
        }

        /// <summary>
        /// Http : POST
        /// </summary>
        public void PostPerson()
        {
            using (WebClient client = new WebClient())
            {
                string postUrl = "http://localhost:8001/people";
                client.Headers.Add("Content-Type", "application/xml");
                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person newPerson = new Person();
                newPerson.FirstName = string.Format("FirstName_{0}", DateTime.Now.Ticks);
                newPerson.LastName = string.Format("LastName_{0}", DateTime.Now.Ticks);
                byte[] responsebytes = client.UploadData(postUrl, "POST",
                    serializer.Serialize(newPerson));
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person person = serializer.Deserialize<Person>(responsebody);
                Console.WriteLine("Http : POST");
                Console.WriteLine(postUrl);
                Console.WriteLine(person);
                Console.WriteLine("=================================");

            }
        }
    }
}

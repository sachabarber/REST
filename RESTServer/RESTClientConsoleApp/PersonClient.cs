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
using RESTServer.Utils.Client;
using RESTServer.Utils.Serialization;

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
            using (RESTWebClient client = new RESTWebClient())
            {
                string getUrl = string.Format("http://localhost:8001/people/{0}", id);

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                var response = client.Get<Person>(getUrl, SerializationToUse.Xml);
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
        public void GetPeople()
        {
            using (RESTWebClient client = new RESTWebClient())
            {
                string getUrl = "http://localhost:8001/people";

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                var response = client.Get<List<Person>>(getUrl, SerializationToUse.Xml);
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
        public void PostPerson()
        {
            using (RESTWebClient client = new RESTWebClient())
            {
                string postUrl = "http://localhost:8001/people";
                Person newPerson = new Person();
                newPerson.FirstName = string.Format("FirstName_{0}", DateTime.Now.Ticks);
                newPerson.LastName = string.Format("LastName_{0}", DateTime.Now.Ticks);
                
                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                var response = client.Post<Person>(postUrl, newPerson, SerializationToUse.Xml);
                Console.WriteLine("Http : POST");
                Console.WriteLine("Status Code : {0}", response.StatusCode);
                Console.WriteLine(postUrl);
                Console.WriteLine(response.Content);
                Console.WriteLine("=================================");

            }
        }
    }
}

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
    /// <summary>
    /// Demonstrates how to use the REST framework, along with a 
    /// <c>IVerbHandlerOf<c>Person</c></c> server side handler
    /// for standard REST Urls
    /// </summary>
    public class PersonClient
    {
        /// <summary>
        /// Http : GET/{id}
        /// Gets item with the id specified
        /// </summary>
        public async Task GetPerson(int id)
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = string.Format("http://localhost:8001/people/{0}", id);

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var response = await client.Get<Person>(getUrl, SerializationToUse.Xml);
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
        public async Task GetPeople()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string getUrl = "http://localhost:8001/people";

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var response = await client.Get<List<Person>>(getUrl, SerializationToUse.Xml);
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
        public async Task PostPerson()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {
                    string postUrl = "http://localhost:8001/people";
                    Person newPerson = new Person();
                    newPerson.FirstName = string.Format("FirstName_{0}", DateTime.Now.Ticks);
                    newPerson.LastName = string.Format("LastName_{0}", DateTime.Now.Ticks);

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var response = await client.Post<Person>(postUrl, newPerson, SerializationToUse.Xml);
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
        public async Task PutPerson()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING people/1");
                    string getUrl = string.Format("http://localhost:8001/people/{0}", 1);

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var response = await client.Get<Person>(getUrl, SerializationToUse.Xml);
                    var person = response.Content;
                    Console.WriteLine(person);

                    string newLastName = string.Format("{0}_Modified_{1}", person.LastName, DateTime.Now.Ticks);
                    person.LastName = newLastName;

                    string putUrl = string.Format("http://localhost:8001/people/{0}", 1);

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var statusCode = await client.Put(putUrl, person, SerializationToUse.Xml);
                    Console.WriteLine("Http : PUT");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(putUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING people/1 again");
                        response = await client.Get<Person>(getUrl, SerializationToUse.Xml);
                        person = response.Content;
                        Console.WriteLine(person);

                    }
                    else
                    {
                        Console.WriteLine("PUT Failed");
                    }
                    Console.WriteLine("=================================");
                }
            });
        }

        /// <summary>
        /// Http : DELETE
        /// </summary>
        public async Task DeletePerson()
        {
            await Task.Run(async () =>
            {
                using (RESTWebClient client = new RESTWebClient())
                {

                    Console.WriteLine("OBTAINING people");
                    string getUrl = string.Format("http://localhost:8001/people");

                    //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                    //so we need to deserialize it as Xml 
                    var response =
                        await client.Get<List<Person>>("http://localhost:8001/people", SerializationToUse.Xml);
                    Console.WriteLine("There are currently {0} people", response.Content.Count);

                    string deleteUrl = string.Format("http://localhost:8001/people/{0}", 1);

                    var statusCode = await client.Delete(deleteUrl);
                    Console.WriteLine("Http : DELETE");
                    Console.WriteLine("Status Code : {0}", statusCode);
                    Console.WriteLine(deleteUrl);

                    if (statusCode == HttpStatusCode.OK)
                    {
                        Console.WriteLine("OBTAINING people again");
                        //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                        //so we need to deserialize it as Xml 
                        response =
                            await client.Get<List<Person>>("http://localhost:8001/people", SerializationToUse.Xml);
                        Console.WriteLine("There are currently {0} people", response.Content.Count);

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

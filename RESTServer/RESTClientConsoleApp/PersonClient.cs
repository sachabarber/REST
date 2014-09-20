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
       


        public void GetPerson(int id)
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(string.Format("http://localhost:8001/people/{0}",id));

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person person = serializer.Deserialize<Person>(response);
            }
        }


        public void GetPeople()
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString("http://localhost:8001/people");

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                List<Person> people = serializer.Deserialize<List<Person>>(response);
            }
        }


        public void PostPeople()
        {
            using (WebClient client = new WebClient())
            {
                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person newPerson = new Person();
                newPerson.FirstName = string.Format("FirstName_{0}", DateTime.Now.Ticks);
                newPerson.LastName = string.Format("LastName_{0}", DateTime.Now.Ticks);
                string serialized = serializer.Serialize(newPerson);

                NameValueCollection reqParams = new NameValueCollection();
                reqParams.Add("item", serialized);
                byte[] responsebytes = client.UploadValues("http://localhost", "POST", reqParams);
                string responsebody = Encoding.UTF8.GetString(responsebytes);

                //the server PersonHandler [RouteBaseAttribute] is set to return Xml, 
                //so we need to deserialize it as Xml 
                Person person = serializer.Deserialize<Person>(responsebody);

            }
        }


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

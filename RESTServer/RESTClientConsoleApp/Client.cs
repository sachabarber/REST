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
    public class Client
    {

        public static void Main(string[] args)
        {
            PersonClient personClient = new PersonClient();
            AccountClient accountClient = new AccountClient();


            //give server a chance to start
            Thread.Sleep(3000);


            //Test out PersonHandler server side REST handler, expects XML

            //GET
            //personClient.GetPerson(1);
            //personClient.GetPeople();
            //personClient.PostPerson();
            //personClient.GetPeople();
            
            ////Test out PersonHandler server side REST handler, expects JSON

            ////GET
            accountClient.GetAccount(1);
            accountClient.GetAccounts();
            accountClient.PostAccount();
            accountClient.GetAccounts();
            

            Console.ReadLine();

        }


      

       

    }
}

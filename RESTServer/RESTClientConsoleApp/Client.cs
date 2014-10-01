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
            //give server a chance to start
            Thread.Sleep(3000);

            Client c = new Client();
#pragma warning disable 4014
            c.Run();
#pragma warning restore 4014

            Console.ReadLine();
        }


        public async Task Run()
        {
            PersonClient personClient = new PersonClient();
            AccountClient accountClient = new AccountClient();
            UserClient userClient = new UserClient();

            
            //Test out PersonHandler (VerbHandler<T,TKey>) server side REST handler, expects XML

            await personClient.GetPerson(1);
            await personClient.GetPeople();
            await personClient.PostPerson();
            await personClient.PutPerson();
            await personClient.DeletePerson();

            //Test out AccountHandler (VerbHandler<T,TKey>) server side REST handler, expects JSON

            await accountClient.GetAccount(1);
            await accountClient.GetAccounts();
            await accountClient.PostAccount();
            await accountClient.PutAccount();
            await accountClient.DeleteAccount();


            //Test out UserHandler (DynamicRouteHandler<T,TKey>) server side REST handler, expects JSON

            await userClient.GetUser(1);
            await userClient.GetUsers();
            await userClient.PostUser();
            await userClient.PutUser();
            await userClient.DeleteUser();
        }


      

       

    }
}

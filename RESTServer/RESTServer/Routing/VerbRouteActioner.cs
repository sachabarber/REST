using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using RESTServer.Handlers;
using RESTServer.Serialization;

namespace RESTServer.Routing
{
    public class VerbRouteActioner : RouteActioner
    {
        RestMethodActioner restMethodActioner = new RestMethodActioner();





        public override async Task<bool> ActionRequest(HttpListenerContext context, IEnumerable<IHandler> handlers)
        {
            object matchingHandler = null;

            //1. try and find the handler who has same base address as the request url
            //   if we find a handler, go to step 2, otherwise try successor
            //2. find out what verb is being used

            var httpMethod = context.Request.HttpMethod;
            var url = context.Request.RawUrl;
            SerializationToUse serializationToUse = SerializationToUse.UseContentType;
            bool result = false;

            foreach (var handler in handlers)
            {
                var routeBase = (RouteBaseAttribute[])handler.GetType().GetCustomAttributes(typeof(RouteBaseAttribute), false);
                if (routeBase.Length > 0)
                {
                    bool isBaseMatch = url.StartsWith(routeBase[0].UrlBase);
                    bool isUrlMatch = restMethodActioner.IsUrlMatch(routeBase[0].UrlBase, url, httpMethod);
                    if (isBaseMatch && isUrlMatch)
                    {
                        serializationToUse = routeBase[0].SerializationToUse;
                        matchingHandler = handler;
                        break;
                    }
                }
            }

            if (matchingHandler != null)
            {
                //handler is using RouteBase, so fair chance it is a VerbHandler
                var genericArgs = GetVerbHandlerGenericArgs(matchingHandler.GetType());


                MethodInfo method = typeof(VerbRouteActioner).GetMethod("DispatchToHandler", 
                    BindingFlags.NonPublic | BindingFlags.Instance);
               
                MethodInfo generic = method.MakeGenericMethod(genericArgs[0], genericArgs[1]);
                result = await (Task<bool>)generic.Invoke(this, new object[] { context, matchingHandler, httpMethod, serializationToUse });

                return result;

            }
            else
            {
                result = await this.Successor.ActionRequest(context, handlers);
                return result;
            }
        }



        private async Task<bool> DispatchToHandler<T, TKey>(HttpListenerContext context, object handler,
            string httpMethod, SerializationToUse serializationToUse)
        {
            MethodInfo method = typeof(VerbRouteActioner).GetMethod("CreateVerbHandler", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo generic = method.MakeGenericMethod(new Type[] { typeof(T), typeof(TKey) });
            IVerbHandler<T, TKey> actualHandler = (IVerbHandler<T, TKey>)generic.Invoke(this, new object[] { handler });
            var result = false;


            switch (httpMethod)
            {
                case "GET":
                    result = await HandleGet<T, TKey>(actualHandler, context, serializationToUse);
                    break;
                case "PUT":
                    break;
                case "POST":
                    result = await HandlePost<T, TKey>(actualHandler, context, serializationToUse);
                    break;
                case "DELETE":
                    break;
            }
            return result;

        }



        private async Task<bool> HandleGet<T, TKey>(IVerbHandler<T, TKey> actualHandler, HttpListenerContext context, SerializationToUse serializationToUse)
        {

            var result = false;
            if (restMethodActioner.IsGetAll(context.Request))
            {
                var items = await actualHandler.Get();
                result = await SetResponse<List<T>>(context, items.ToList(), serializationToUse);
            }
            else
            {
                TKey id = restMethodActioner.ExtractId<TKey>(context.Request);
                var item = await actualHandler.Get(id);
                result = await SetResponse<T>(context, item, serializationToUse);
            }

            return result;
        }

        private async Task<bool> HandlePost<T, TKey>(IVerbHandler<T, TKey> actualHandler, HttpListenerContext context, SerializationToUse serializationToUse)
        {
            T item = restMethodActioner.ExtractContent<T>(context.Request,serializationToUse);
            T itemAdded = await actualHandler.Post(item);
            bool result = await SetResponse<T>(context, itemAdded, serializationToUse);
            return result;
        }


        private async Task<bool> SetResponse<T>(HttpListenerContext context,T result, SerializationToUse serializationToUse)
        {
            HttpListenerResponse response = context.Response;
            using (System.IO.Stream output = response.OutputStream)
            {
                ISerializer serializer = restMethodActioner.ObtainSerializer(serializationToUse,context.Request.ContentType);
                string serialized = serializer.Serialize<T>(result);
                var buffer = Encoding.UTF8.GetBytes(serialized);
                output.Write(buffer, 0, buffer.Length);
                response.StatusCode = 200;
                response.StatusDescription = "OK";
            }
            return true;
        }




        private IVerbHandler<T, TKey> CreateVerbHandler<T, TKey>(object item)
        {
            Expression convertExpr = Expression.Convert(
                                        Expression.Constant(item),
                                        typeof(IVerbHandler<T, TKey>));

            var x = Expression.Lambda<Func<IVerbHandler<T, TKey>>>(convertExpr).Compile()();
            return x;
        }



        private Type[] GetVerbHandlerGenericArgs(Type item)
        {

            var ints = item.GetInterfaces();
            var verbInterface = item.GetInterfaces().Where(x => x.FullName.Contains("IVerbHandler")).Single();
            return verbInterface.GenericTypeArguments;
        }


        
    }
}

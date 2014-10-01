using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Handlers;
using RESTServer.Utils.Serialization;

namespace RESTServer.Routing
{
    public class VerbRouteActioner : RouteActioner
    {
        RestMethodActioner restMethodActioner = new RestMethodActioner();

        public override async Task<bool> ActionRequest(HttpListenerContext context, IList<IHandler> handlers)
        {
            return await Task.Run(async () =>
            {
                object matchingHandler = null;

                //1. try and find the handler who has same base address as the request url
                //   if we find a handler, go to step 2, otherwise try successor
                //2. find out what verb is being used

                var httpMethod = context.Request.HttpMethod;
                var url = context.Request.RawUrl;
                bool result = false;

                var routeResult = await restMethodActioner.FindHandler(
                    typeof (IVerbHandler<,>), context, handlers, false);

                if (routeResult.Handler != null)
                {
                    //handler is using RouteBase, so fair chance it is a VerbHandler
                    var genericArgs = GetVerbHandlerGenericArgs(routeResult.Handler.GetType());

                    MethodInfo method = typeof (VerbRouteActioner).GetMethod("DispatchToHandler",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    MethodInfo generic = method.MakeGenericMethod(genericArgs[0], genericArgs[1]);
                    result = await (Task<bool>) generic.Invoke(this, new object[]
                    {
                        context, routeResult.Handler, httpMethod, routeResult.SerializationToUse
                    });

                    return result;

                }

                result = await this.Successor.ActionRequest(context, handlers);
                return result;
            });
        }

        private async Task<bool> DispatchToHandler<T, TKey>(HttpListenerContext context, object handler,
            string httpMethod, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                MethodInfo method = typeof (VerbRouteActioner).GetMethod("CreateVerbHandler",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo generic = method.MakeGenericMethod(new[] {typeof (T), typeof (TKey)});
                IVerbHandler<T, TKey> actualHandler =
                    (IVerbHandler<T, TKey>) generic.Invoke(this, new[] {handler});
                var result = false;


                switch (httpMethod)
                {
                    case "GET":
                        result = await HandleGet<T, TKey>(actualHandler, context, serializationToUse);
                        break;
                    case "PUT":
                        result = await HandlePut<T, TKey>(actualHandler, context, serializationToUse);
                        break;
                    case "POST":
                        result = await HandlePost<T, TKey>(actualHandler, context, serializationToUse);
                        break;
                    case "DELETE":
                        result = await HandleDelete<T, TKey>(actualHandler, context, serializationToUse);
                        break;
                }
                return result;
            });
        }

        private async Task<bool> HandleGet<T, TKey>(IVerbHandler<T, TKey> actualHandler, 
            HttpListenerContext context, SerializationToUse serializationToUse)
        {

            return await Task.Run(async () =>
            {
                var result = false;
                if (restMethodActioner.IsGetAll(context.Request.RawUrl))
                {
                    var items = await actualHandler.Get();
                    result = await restMethodActioner.SetResponse<List<T>>(context, items.ToList(),
                        serializationToUse);
                }
                else
                {
                    TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);
                    var item = await actualHandler.Get(id);
                    result = await restMethodActioner.SetResponse<T>(context, item, serializationToUse);
                }

                return result;
            });
        }

        private async Task<bool> HandlePost<T, TKey>(IVerbHandler<T, TKey> actualHandler, HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                T item = await restMethodActioner.ExtractContent<T>(context.Request, serializationToUse);
                T itemAdded = await actualHandler.Post(item);
                bool result = await restMethodActioner.SetResponse<T>(context, itemAdded, serializationToUse);
                return result;
            });
        }
  
        private async Task<bool> HandleDelete<T, TKey>(IVerbHandler<T, TKey> actualHandler, HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);
                bool updatedOk = await actualHandler.Delete(id);
                updatedOk &= await restMethodActioner.SetOkResponse(context);
                return updatedOk;
            });
        }

        private async Task<bool> HandlePut<T, TKey>(IVerbHandler<T, TKey> actualHandler, HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);
                T item = await restMethodActioner.ExtractContent<T>(context.Request, serializationToUse);
                bool updatedOk = await actualHandler.Put(id, item);
                updatedOk &= await restMethodActioner.SetOkResponse(context);
                return updatedOk;
            });
        }


        /// <summary>
        /// Called via Reflection
        /// </summary>
        private IVerbHandler<T, TKey> CreateVerbHandler<T, TKey>(object item)
        {
            Expression convertExpr = Expression.Convert(
                                        Expression.Constant(item),
                                        typeof(IVerbHandler<T, TKey>)
                                     );

            var x = Expression.Lambda<Func<IVerbHandler<T, TKey>>>(convertExpr).Compile()();
            return x;
        }

        private Type[] GetVerbHandlerGenericArgs(Type item)
        {

            var ints = item.GetInterfaces();
            var verbInterface = item.GetInterfaces().Single(x => x.FullName.Contains("IVerbHandler"));
            return verbInterface.GenericTypeArguments;
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using RESTServer.Exceptions;
using RESTServer.Handlers;
using RESTServer.Utils.Serialization;

namespace RESTServer.Routing
{
    public class DynamicRouteActioner : RouteActioner
    {
        RestMethodActioner restMethodActioner = new RestMethodActioner();



        public override async Task<bool> ActionRequest(HttpListenerContext context, IList<IHandler> handlers)
        {
            object matchingHandler = null;

            //1. try and find the handler who has same base address as the request url
            //   if we find a handler, go to step 2, otherwise try successor
            //2. find out what verb is being used

            var httpMethod = context.Request.HttpMethod;
            var url = context.Request.RawUrl;
            SerializationToUse serializationToUse = SerializationToUse.UseContentType;
            bool result = false;

            var routeResult = await restMethodActioner.FindHandler(
                typeof(IDynamicRouteHandler<,>), context, handlers, true);

            if (routeResult.Handler != null)
            {
                //handler is using RouteBase, so fair chance it is a VerbHandler
                var genericArgs = GetDynamicRouteHandlerGenericArgs(routeResult.Handler.GetType());

                MethodInfo method = typeof(DynamicRouteActioner).GetMethod("DispatchToHandler",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                MethodInfo generic = method.MakeGenericMethod(genericArgs[0], genericArgs[1]);
                result = await (Task<bool>)generic.Invoke(this, new object[]
                {
                    context, routeResult.Handler, httpMethod, url, routeResult.SerializationToUse
                });

                return result;

            }

            result = await this.Successor.ActionRequest(context, handlers);
            return result;
        }





        private async Task<bool> DispatchToHandler<T, TKey>(HttpListenerContext context, object handler,
            string httpMethod, string url, SerializationToUse serializationToUse)
        {
            var result = false;


            SerializationToUse actualSerializationToUse = serializationToUse;
            if (serializationToUse == SerializationToUse.UseContentType)
            {
                ObtainSerializationToUseFromContentType(context.Request.ContentType);
            }


            switch (httpMethod)
            {
                case "GET":
                    var method = await ObtainGetMethod<T, TKey>(handler, url);
                    result = await HandleGet<T, TKey>(method, handler, context, actualSerializationToUse);
                    break;
                //case "PUT":
                //    result = await HandlePut<T, TKey>(actualHandler, context, actualSerializationToUse);
                //    break;
                //case "POST":
                //    result = await HandlePost<T, TKey>(actualHandler, context, actualSerializationToUse);
                //    break;
                //case "DELETE":
                //    result = await HandleDelete<T, TKey>(actualHandler, context, actualSerializationToUse);
                //    break;
            }
            return result;
        }

        private async Task<DynamicMethodInfo> ObtainGetMethod<T, TKey>(object handler, string url)
        {
            var possibleGetMethods = from x in handler.GetType().GetMethods()
                                     let attrib = x.GetCustomAttributes(typeof(RouteAttribute), false)
                                     where attrib.Length > 0 && ((RouteAttribute)attrib[0]).HttpVerb == "GET"
                                     select new { Method = x, Route = (RouteAttribute)attrib[0] };

            if (restMethodActioner.IsGetAll(url))
            {
                string attributeUrl = url.Substring(url.LastIndexOf("/"));

                var method = possibleGetMethods.Where(x => x.Route.Route == attributeUrl)
                    .Select(x=>x.Method).FirstOrDefault();

                if (MethodResultIsCorrectType<Task<IEnumerable<T>>>(method))
                {
                    return new DynamicMethodInfo(method,true);
                }

                if (MethodResultIsCorrectType<IEnumerable<T>>(method))
                {
                    return new DynamicMethodInfo(method,false);
                }
                throw new HttpResponseException(string.Format("Incorrect return type for route '{0}'", url));
            }
            else
            {
                string attributeUrl = url.Substring(0,url.LastIndexOf("/"));
                attributeUrl = attributeUrl.Substring(attributeUrl.LastIndexOf("/"));
                attributeUrl = attributeUrl + "/{0}";
                var method = possibleGetMethods.Where(x => x.Route.Route == attributeUrl)
                    .Select(x => x.Method).FirstOrDefault();

                if (MethodResultIsCorrectType<Task<T>>(method)
                    && MethodHasSingleIdParameter<TKey>(method))
                {
                    return new DynamicMethodInfo(method, true);
                }

                if (MethodResultIsCorrectType<T>(method)
                    && MethodHasSingleIdParameter<TKey>(method))
                {
                    return new DynamicMethodInfo(method, false);
                }
                throw new HttpResponseException(string.Format("Incorrect return type for route '{0}'", url));
            }
        }

        private async Task<bool> HandleGet<T, TKey>(DynamicMethodInfo methodInfo, object handler, 
            HttpListenerContext context, SerializationToUse serializationToUse)
        {

            var result = false;
            if (restMethodActioner.IsGetAll(context.Request.RawUrl))
            {
                if (methodInfo.IsTask)
                {
                    var items = await (Task<IEnumerable<T>>)methodInfo.Method.Invoke(handler, null);
                    result = await restMethodActioner.SetResponse<List<T>>(context, 
                        items.ToList(), serializationToUse);
                }
                else
                {
                    var items = (IEnumerable<T>)methodInfo.Method.Invoke(handler, null);
                    result = await restMethodActioner.SetResponse<List<T>>(context,
                        items.ToList(), serializationToUse);
                }
            }
            else
            {
                TKey id = restMethodActioner.ExtractId<TKey>(context.Request);

                if (methodInfo.IsTask)
                {
                    var item = await (Task<T>)methodInfo.Method.Invoke(handler, new object[] { id});
                    result = await restMethodActioner.SetResponse<T>(context,
                        item, serializationToUse);
                }
                else
                {
                    var item = (T)methodInfo.Method.Invoke(handler, null);
                    result = await restMethodActioner.SetResponse<T>(context,
                        item, serializationToUse);
                }
            }
            return result;
        }


        private SerializationToUse ObtainSerializationToUseFromContentType(string contentType)
        {
            if (contentType == "application/json")
            {
                return SerializationToUse.Json;
            }

            if (contentType == "application/xml")
            {
                return SerializationToUse.Xml;
            }

            throw new InvalidOperationException(
                "Can only deserialize using either 'application/json' or 'application/xml' content types");
        }


        private bool MethodHasSingleIdParameter<TKey>(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Count() == 1 && parameters[0].ParameterType == typeof (TKey);
        }


        private bool MethodResultIsCorrectType<T>(MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof (T));
        }


        private Type[] GetDynamicRouteHandlerGenericArgs(Type item)
        {

            var ints = item.GetInterfaces();
            var verbInterface = item.GetInterfaces().Single(x => x.FullName.Contains("IDynamicRouteHandler"));
            return verbInterface.GenericTypeArguments;
        }

    }
}

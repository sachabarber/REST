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
                    typeof (IDynamicRouteHandler<,>), context, handlers, true);

                if (routeResult.Handler != null)
                {
                    //handler is using RouteBase, so fair chance it is a VerbHandler
                    var genericArgs = GetDynamicRouteHandlerGenericArgs(routeResult.Handler.GetType());

                    MethodInfo method = typeof (DynamicRouteActioner).GetMethod("DispatchToHandler",
                        BindingFlags.NonPublic | BindingFlags.Instance);

                    MethodInfo generic = method.MakeGenericMethod(genericArgs[0], genericArgs[1]);
                    result = await (Task<bool>) generic.Invoke(this, new object[]
                    {
                        context, routeResult.Handler, httpMethod, url, routeResult.SerializationToUse
                    });

                    return result;

                }

                result = await this.Successor.ActionRequest(context, handlers);
                return result;
            });
        }

        private async Task<bool> DispatchToHandler<T, TKey>(HttpListenerContext context, object handler,
            string httpMethod, string url, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                var result = false;

                DynamicMethodInfo method = null;
                switch (httpMethod)
                {
                    case "GET":
                        method = await ObtainGetMethod<T, TKey>(handler, url);
                        result = await HandleGet<T, TKey>(method, handler, context, serializationToUse);
                        break;
                    case "PUT":
                        method = await ObtainPutMethod<T, TKey>(handler, url);
                        result = await HandlePut<T, TKey>(method, handler, context, serializationToUse);
                        break;
                    case "POST":
                        method = await ObtainPostMethod<T, TKey>(handler, url);
                        result = await HandlePost<T, TKey>(method, handler, context, serializationToUse);
                        break;
                    case "DELETE":
                        method = await ObtainDeleteMethod<T, TKey>(handler, url);
                        result = await HandleDelete<T, TKey>(method, handler, context, serializationToUse);
                        break;
                }
                return result;
            });
        }


        private async Task<DynamicMethodInfo> ObtainGetMethod<T, TKey>(object handler, string url)
        {
            return await Task.Run(async () =>
            {
                var possibleGetMethods = ObtainPossibleMethodMatches(handler, HttpMethod.Get);

                if (restMethodActioner.IsGetAll(url))
                {
                    string attributeUrl = url.Substring(url.LastIndexOf("/"));

                    var method = possibleGetMethods.Where(x => x.Route.Route == attributeUrl)
                        .Select(x => x.Method).FirstOrDefault();

                    if (MethodResultIsCorrectType<Task<IEnumerable<T>>>(method))
                    {
                        return new DynamicMethodInfo(method, true);
                    }

                    if (MethodResultIsCorrectType<IEnumerable<T>>(method))
                    {
                        return new DynamicMethodInfo(method, false);
                    }
                    throw new HttpResponseException(string.Format(
                        "Incorrect return type/parameters for route '{0}'", url));
                }
                else
                {
                    var method = GetIdMethodMatch(possibleGetMethods, url);

                    if (MethodResultIsCorrectType<Task<T>>(method)
                        && MethodHasSingleCorrectParameterOfType<TKey>(method))
                    {
                        return new DynamicMethodInfo(method, true);
                    }

                    if (MethodResultIsCorrectType<T>(method)
                        && MethodHasSingleCorrectParameterOfType<TKey>(method))
                    {
                        return new DynamicMethodInfo(method, false);
                    }
                    throw new HttpResponseException(string.Format(
                        "Incorrect return type/parameters for route '{0}'", url));
                }
            });
        }

        private async Task<DynamicMethodInfo> ObtainPutMethod<T, TKey>(object handler, string url)
        {
            return await Task.Run(async () =>
            {
                var possiblePutMethods = ObtainPossibleMethodMatches(handler, HttpMethod.Put);

                var method = GetIdMethodMatch(possiblePutMethods, url);

                if (MethodResultIsCorrectType<Task<bool>>(method)
                    && MethodHasCorrectPutParameters<T, TKey>(method))
                {
                    return new DynamicMethodInfo(method, true);
                }

                if (MethodResultIsCorrectType<bool>(method)
                    && MethodHasCorrectPutParameters<T, TKey>(method))
                {
                    return new DynamicMethodInfo(method, false);
                }
                throw new HttpResponseException(string.Format(
                    "Incorrect return type/parameters for route '{0}'", url));
            });

        }

        private async Task<DynamicMethodInfo> ObtainPostMethod<T, TKey>(object handler, string url)
        {
            return await Task.Run(async () =>
            {
                var possiblePostMethods = ObtainPossibleMethodMatches(handler, HttpMethod.Post);

                string attributeUrl = url.Substring(url.LastIndexOf("/"));

                var method = possiblePostMethods.Where(x => x.Route.Route == attributeUrl)
                    .Select(x => x.Method).FirstOrDefault();

                if (MethodResultIsCorrectType<Task<T>>(method)
                    && MethodHasSingleCorrectParameterOfType<T>(method))
                {
                    return new DynamicMethodInfo(method, true);
                }

                if (MethodResultIsCorrectType<T>(method)
                    && MethodHasSingleCorrectParameterOfType<T>(method))
                {
                    return new DynamicMethodInfo(method, false);
                }
                throw new HttpResponseException(string.Format(
                    "Incorrect return type/parameters for route '{0}'", url));
            });

        }

        private async Task<DynamicMethodInfo> ObtainDeleteMethod<T, TKey>(object handler, string url)
        {
            return await Task.Run(async () =>
            {
                var possibleDeleteMethods = ObtainPossibleMethodMatches(handler, HttpMethod.Delete);

                var method = GetIdMethodMatch(possibleDeleteMethods, url);

                if (MethodResultIsCorrectType<Task<bool>>(method)
                    && MethodHasSingleCorrectParameterOfType<TKey>(method))
                {
                    return new DynamicMethodInfo(method, true);
                }

                if (MethodResultIsCorrectType<bool>(method)
                    && MethodHasSingleCorrectParameterOfType<TKey>(method))
                {
                    return new DynamicMethodInfo(method, false);
                }
                throw new HttpResponseException(string.Format(
                    "Incorrect return type/parameters for route '{0}'", url));
            });
        }


        private async Task<bool> HandleGet<T, TKey>(DynamicMethodInfo methodInfo, object handler, 
            HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                var result = false;
                if (restMethodActioner.IsGetAll(context.Request.RawUrl))
                {
                    if (methodInfo.IsTask)
                    {
                        var items = await (Task<IEnumerable<T>>) methodInfo.Method.Invoke(handler, null);
                        result = await restMethodActioner.SetResponse<List<T>>(context,
                            items.ToList(), serializationToUse);
                    }
                    else
                    {
                        var items = (IEnumerable<T>) methodInfo.Method.Invoke(handler, null);
                        result = await restMethodActioner.SetResponse<List<T>>(context,
                            items.ToList(), serializationToUse);
                    }
                }
                else
                {
                    TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);

                    if (methodInfo.IsTask)
                    {
                        var item = await (Task<T>) methodInfo.Method.Invoke(handler, new object[] {id});
                        result = await restMethodActioner.SetResponse<T>(context,
                            item, serializationToUse);
                    }
                    else
                    {
                        var item = (T) methodInfo.Method.Invoke(handler, new object[] {id});
                        result = await restMethodActioner.SetResponse<T>(context,
                            item, serializationToUse);
                    }
                }
                return result;
            });
        }


        private async Task<bool> HandlePut<T, TKey>(DynamicMethodInfo methodInfo, object handler,
            HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                T item = await restMethodActioner.ExtractContent<T>(context.Request, serializationToUse);
                TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);
                var updatedOk = false;

                if (methodInfo.IsTask)
                {
                    updatedOk = await (Task<bool>) methodInfo.Method.Invoke(handler, new object[] {id, item});
                }
                else
                {
                    updatedOk = (bool) methodInfo.Method.Invoke(handler, null);
                }
                updatedOk &= await restMethodActioner.SetOkResponse(context);
                return updatedOk;
            });
        }

        private async Task<bool> HandlePost<T, TKey>(DynamicMethodInfo methodInfo, object handler,
            HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                T itemAdded = default(T);
                T item = await restMethodActioner.ExtractContent<T>(context.Request, serializationToUse);

                if (methodInfo.IsTask)
                {
                    itemAdded = await (Task<T>) methodInfo.Method.Invoke(handler, new object[] {item});
                }
                else
                {
                    itemAdded = (T) methodInfo.Method.Invoke(handler, new object[] {item});
                }

                bool result = await restMethodActioner.SetResponse<T>(context, itemAdded, serializationToUse);
                return result;
            });
        }


        private async Task<bool> HandleDelete<T, TKey>(DynamicMethodInfo methodInfo, object handler,
            HttpListenerContext context, SerializationToUse serializationToUse)
        {
            return await Task.Run(async () =>
            {
                var updatedOk = false;
                TKey id = await restMethodActioner.ExtractId<TKey>(context.Request);

                if (methodInfo.IsTask)
                {
                    updatedOk = await (Task<bool>) methodInfo.Method.Invoke(handler, new object[] {id});
                }
                else
                {
                    updatedOk = (bool) methodInfo.Method.Invoke(handler, new object[] {id});
                }
                updatedOk &= await restMethodActioner.SetOkResponse(context);
                return updatedOk;
            });
        }


        private bool MethodHasSingleCorrectParameterOfType<TKey>(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Count() == 1 && parameters[0].ParameterType == typeof (TKey);
        }

        private bool MethodHasCorrectPutParameters<T,TKey>(MethodInfo method)
        {
            var parameters = method.GetParameters();
            return parameters.Count() == 2 &&
                   parameters[0].ParameterType == typeof (TKey) &&
                   parameters[1].ParameterType == typeof (T);

        }


        private bool MethodResultIsCorrectType<T>(MethodInfo method)
        {
            return method.ReturnType.IsAssignableFrom(typeof (T));
        }

        private IEnumerable<MethodMatch> ObtainPossibleMethodMatches(object handler, HttpMethod httpMethod)
        {
            return from x in handler.GetType().GetMethods()
                   let attrib = x.GetCustomAttributes(typeof(RouteAttribute), false)
                   where attrib.Length > 0 && ((RouteAttribute)attrib[0]).HttpVerb == httpMethod
                   select new MethodMatch(x, (RouteAttribute)attrib[0]);
        }
        private MethodInfo GetIdMethodMatch(IEnumerable<MethodMatch> possibleMatches, string url)
        {
            string attributeUrl = url.Substring(0, url.LastIndexOf("/"));
            attributeUrl = attributeUrl.Substring(attributeUrl.LastIndexOf("/"));
            attributeUrl = attributeUrl + "/{0}";
            return possibleMatches.Where(x => x.Route.Route == attributeUrl)
                .Select(x => x.Method).FirstOrDefault();
        }
        private Type[] GetDynamicRouteHandlerGenericArgs(Type item)
        {

            var ints = item.GetInterfaces();
            var verbInterface = item.GetInterfaces().Single(x => x.FullName.Contains("IDynamicRouteHandler"));
            return verbInterface.GenericTypeArguments;
        }

    }
}

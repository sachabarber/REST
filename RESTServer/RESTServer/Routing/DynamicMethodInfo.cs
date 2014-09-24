using System.Reflection;

namespace RESTServer.Routing
{
    public class DynamicMethodInfo
    {
        public DynamicMethodInfo(MethodInfo method, bool isTask)
        {
            Method = method;
            IsTask = isTask;
        }

        public MethodInfo Method { get; private set; }
        public bool IsTask { get; private set; }
    }
}

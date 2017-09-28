using System;
using System.Linq;
using System.Reflection;

namespace Simp.Rpc.Service
{
    public class ServiceExcuter
    {
        public ServiceExcuter(object instance, RpcMethodInfo method)
        {
            Instance = instance;
            Method = method;
            ArgTypes = method?.RpcParameters.Select(param => param.Type).ToArray();
            ReturnType = method?.RpcReturnType.Type;
        }

        public Type[] ArgTypes { get; private set; }

        public Type ReturnType { get; private set; }

        private object Instance { get; set; }

        private RpcMethodInfo Method { get; set; }


        public object Result { get; private set; }

        public object Excute(object[] args)
        {
            Result = Method.MethodInfo.Invoke(Instance, args);
            return Result;
        }

    }
}
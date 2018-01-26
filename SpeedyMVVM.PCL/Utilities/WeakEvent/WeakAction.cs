using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM;

namespace SpeedyMVVM.Utilities
{
    public class WeakAction
    {

        public WeakReference Instance { get; }
        public MethodInfo Method { get; }
        public Type MethodType { get; }
        
        public Delegate CreateDelegate()
        {
            return Method.CreateDelegate(MethodType, Instance.Target);
        }
    

        public WeakAction(Delegate action)
        {
            Instance = new WeakReference(action.Target);
            Method = action.GetMethodInfo();
            MethodType = Method.GetMethodType();
        }

        public WeakAction(object instance, MethodInfo method)
        {
            Instance = new WeakReference(instance);
            Method = method;
            MethodType = Method.GetMethodType();
        }
    }
}

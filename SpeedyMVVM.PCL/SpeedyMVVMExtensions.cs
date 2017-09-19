using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM
{
    public static class SpeedyMVVMExtensions
    {
        public static ObservableCollection<T>AsObservableCollection<T>(this IEnumerable<T> list)
        {
            return (list != null) ? new ObservableCollection<T>(list) : new ObservableCollection<T>();
        }

        public static T GetCopy<T>(this T S)
        {
            T newObj = Activator.CreateInstance<T>();

            foreach (PropertyInfo i in newObj.GetType().GetRuntimeProperties())
            {
                if (i.CanWrite)
                {
                    object value = S.GetType().GetRuntimeProperty(i.Name).GetValue(S, null);
                    i.SetValue(newObj, value, null);
                }
            }

            return newObj;
        }
    }
}

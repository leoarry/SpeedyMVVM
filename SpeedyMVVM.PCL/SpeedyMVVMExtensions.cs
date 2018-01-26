using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SpeedyMVVM
{
    /* EX PROPERTY FROM EntityFilterViewModel for TypeExplorer
    /// <summary>
    /// List of properties names for <typeparamref name="T"/>.
    /// </summary>
    public ObservableCollection<string> PropertyList
    {
        get
        {
            var pInfo = typeof(T).GetRuntimeProperties();
            List<string> myList = new List<string>();
            myList = pInfo.Where(p => !p.PropertyType.GetTypeInfo().IsSubclassOf(typeof(EntityBase)) &&
                !(p.PropertyType.GetTypeInfo().IsGenericType && p.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(ObservableCollection<>))).Select(p => p.Name).ToList();
            myList.Remove(nameof(EntityBase.EntityStatus));
            return new ObservableCollection<string>(myList);
        }
    }*/

    public static class SpeedyMVVMExtensions
    {
        /// <summary>
        /// Check if the value is the default value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDefault<T>(this T value)
        {
            return value == null || value.Equals(default(T));
        }

        /// <summary>
        /// Check if a collection is null or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        /// <summary>
        /// Convert a collection in an ObservableCollection.
        /// </summary>
        /// <typeparam name="T">Type of the collection to convert from.</typeparam>
        /// <param name="list">Collection to be converted.</param>
        /// <returns>Collection converted in ObservableCollection.</returns>
        public static ObservableCollection<T>ToObservableCollection<T>(this IEnumerable<T> list)
        {
            return (list != null) ? new ObservableCollection<T>(list) : new ObservableCollection<T>();
        }

        /// <summary>
        /// Convert a IEnumerable<KeyValuePair<Tkey,Tvalue>> in ConcurrentDictionary<Tkey,Tvalue>
        /// </summary>
        /// <typeparam name="Tkey">Key type.</typeparam>
        /// <typeparam name="Tvalue">Value type.</typeparam>
        /// <param name="instance">Collection to be converted.</param>
        /// <returns></returns>
        public static ConcurrentDictionary<Tkey,Tvalue>ToConcurrentDictionary<Tkey, Tvalue>(this IEnumerable<KeyValuePair<Tkey,Tvalue>> instance)
        {
            return (instance != null) ? new ConcurrentDictionary<Tkey, Tvalue>(instance) : new ConcurrentDictionary<Tkey, Tvalue>();
        }

        /// <summary>
        /// Convert a collection to a ConcurrentBag.
        /// </summary>
        /// <typeparam name="T">Type of the instance.</typeparam>
        /// <param name="instance">Collection to be converted.</param>
        /// <returns></returns>
        public static ConcurrentBag<T>ToConcurrentBag<T>(this IEnumerable<T> instance)
        {
            return (instance != null) ? new ConcurrentBag<T>(instance) : new ConcurrentBag<T>();
        }

        /// <summary>
        /// Get a deep-copy of an object.
        /// </summary>
        /// <typeparam name="T">Type of the object to copy.</typeparam>
        /// <param name="S">Object to be copied.</param>
        /// <returns>New object copied.</returns>
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

        public static T FromXML<T>(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(stream);
        }

        
        public static bool ToXML<T>(this T obj, Stream stream)
        {
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                return true;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Determinate whenever a type is a Nullable type.
        /// </summary>
        /// <param name="t">Type to be checked.</param>
        /// <returns>Return true if is nullable.</returns>
        public static bool IsNullableType(this Type t)
        {
            return t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        /// <summary>
        /// Determinate whenever a value is a number.
        /// </summary>
        /// <param name="inputString">string to be checked.</param>
        /// <returns>Return true if is a number.</returns>
        public static bool IsNumber<T>(this T input)
        {
            return Regex.IsMatch(input.ToString(), "^[0-9]+$");
        }

        /// <summary>
        /// Return the type of the delegate from a MethodInfo.
        /// </summary>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static Type GetMethodType(this MethodInfo Method)
        {
            Func<Type[], Type> getType;
            var isAction = Method.ReturnType.Equals((typeof(void)));
            var types = Method.GetParameters().Select(p => p.ParameterType);

            if (isAction)
            {
                getType = Expression.GetActionType;
            }
            else
            {
                getType = Expression.GetFuncType;
                types = types.Concat(new[] { Method.ReturnType });
            }

            return getType(types.ToArray());
        }
    }
}

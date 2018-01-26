using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Validation
{
    /// <summary>
    /// Compare two ValidationRule using the property 'PropertyName'.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ValidationRuleEqualityComparer<T> : IEqualityComparer<ValidationRule<T>> where T:ValidableObject
    {
        public bool Equals(ValidationRule<T> x, ValidationRule<T> y)
        {
            return (string.Equals(x.PropertyName, y.PropertyName) && Type.Equals(x.MemberType, y.MemberType));
        }

        public int GetHashCode(ValidationRule<T> obj)
        {
            return obj.GetHashCode();
        }
    }
}

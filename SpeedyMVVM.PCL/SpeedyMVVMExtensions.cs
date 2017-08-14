using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    }
}

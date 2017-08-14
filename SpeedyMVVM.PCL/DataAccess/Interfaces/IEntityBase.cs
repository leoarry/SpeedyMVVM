using SpeedyMVVM.Utilities.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess.Interfaces
{
    /// <summary>
    /// Basic definition of a data entity.
    /// </summary>
    public interface IEntityBase: INotifyPropertyChanged
    {
        int ID { get; set; }
        EntityStatusEnum EntityStatus { get; set; }
    }
}

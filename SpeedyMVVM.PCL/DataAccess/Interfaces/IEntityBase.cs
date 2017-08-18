using System.ComponentModel;

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

using SpeedyMVVM.Navigation.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess.Interfaces
{
    /// <summary>
    /// Define a service to interact with data entities.
    /// </summary>
    public interface IEntityDialogBoxService:IDialogBoxService
    {
        Task<bool?> ShowEntityPickerBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase;
        Task<bool?> ShowEntityDialogBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase;
        Task<bool?> ShowCRUDDialogBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase;
    }
}

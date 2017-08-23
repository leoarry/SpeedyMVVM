using SpeedyMVVM.Navigation.Interfaces;
using System.Collections.ObjectModel;

namespace SpeedyMVVM.DataAccess.Interfaces
{
    /// <summary>
    /// Define a service to interact with data entities.
    /// </summary>
    public interface IEntitiesDialogBoxService:IDialogBoxService
    {
        bool? ShowEntityPickerBox<T>(EntityPickerBoxViewModel<T> myViewModel) where T : EntityBase;
        bool? ShowEntityEditorBox<T>(EntityEditorBoxViewModel<T> myViewModel) where T : EntityBase;
        bool? ShowPrintEntitiesDialog<T>(ObservableCollection<T> myCollection) where T : EntityBase;
    }
}

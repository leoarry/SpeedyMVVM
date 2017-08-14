using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities.Interfaces;
using System.Collections.ObjectModel;

namespace SpeedyMVVM.DataAccess.Interfaces
{
    /// <summary>
    /// Define a service to interact with data entities.
    /// </summary>
    public interface IEntitiesDialogBoxService:IDialogBoxService
    {
        bool? ShowEntityPickerBox<T>(EntityPickerBoxViewModel<T> myViewModel) where T : IEntityBase;
        bool? ShowEntityEditorBox(IPageViewModel myViewModel);
        bool? ShowPrintEntitiesDialog<T>(ObservableCollection<T> myCollection) where T : IEntityBase;
    }
}

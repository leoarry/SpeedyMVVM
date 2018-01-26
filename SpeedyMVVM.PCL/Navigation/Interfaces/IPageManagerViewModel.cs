using System.Collections.ObjectModel;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Navigation
{
    public interface IPageManagerViewModel:IViewModelBase
    {
        RelayCommand<IPageViewModel> ChangePageCommand { get; }
        IPageViewModel CurrentPage { get; set; }
        bool IsExpanded { get; set; }
        ObservableCollection<IPageViewModel> Pages { get; set; }
    }
}
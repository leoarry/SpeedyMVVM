using SpeedyMVVM.Navigation.Models;
using System.Collections.ObjectModel;

namespace SpeedyMVVM.Utilities.Interfaces
{
    public interface IAppRuntimeSettingsService
    {
        int CurrentUserID { get; set; }
        string CurrentUserName { get; set; }
        int CurrentUserLevel { get; set; }
        ObservableCollection<PageSettingModel> PageSettings { get; set; }
    }
}

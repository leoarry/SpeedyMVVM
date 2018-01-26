using SpeedyMVVM.Navigation;
using System.Collections.ObjectModel;

namespace SpeedyMVVM.Utilities
{
    public interface IAppRuntimeSettingsService
    {
        int CurrentUserID { get; set; }
        string CurrentUserName { get; set; }
        int CurrentUserLevel { get; set; }
        ObservableCollection<PageSettingModel> PageSettings { get; set; }
    }
}

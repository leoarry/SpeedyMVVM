using SpeedyMVVM.Models;
using SpeedyMVVM.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

using SpeedyMVVM.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities.Enumerators;
using System.Collections.ObjectModel;

namespace SpeedyMVVM.TestModel.Services
{
    public class EntitiesDialogBoxService : IEntitiesDialogBoxService
    {
        public bool? ShowEntityEditorBox(IPageViewModel myViewModel)
        {
            return true;
        }

        public bool? ShowEntityPickerBox<T>(EntityPickerBoxViewModel<T> myViewModel) where T : IEntityBase
        {
            return true;
        }

        public string ShowInputBox(string message, string title, string iconPath)
        {
            return message;
        }

        public bool? ShowMessageBox(string message, string title, DialogBoxEnum BoxType, DialogBoxIconEnum icon)
        {
            return true;
        }
        
        public bool? ShowPrintEntitiesDialog<T>(ObservableCollection<T> myCollection) where T : IEntityBase
        {
            return true;
        }
    }
}

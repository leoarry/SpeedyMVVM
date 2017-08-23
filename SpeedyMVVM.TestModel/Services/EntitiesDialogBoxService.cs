using SpeedyMVVM.DataAccess.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.Navigation.Interfaces;
using System.Collections.ObjectModel;
using SpeedyMVVM.Navigation.Enumerators;

namespace SpeedyMVVM.TestModel.Services
{
    public class EntitiesDialogBoxService : IEntitiesDialogBoxService
    {
        public bool? ShowEntityEditorBox<T>(EntityEditorBoxViewModel<T> myViewModel) where T : EntityBase
        {
            return true;
        }

        public bool? ShowEntityPickerBox<T>(EntityPickerBoxViewModel<T> myViewModel) where T : EntityBase
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
        
        public bool? ShowPrintEntitiesDialog<T>(ObservableCollection<T> myCollection) where T : EntityBase
        {
            return true;
        }
    }
}

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
    public class EntitiesDialogBoxService : IEntityDialogBoxService
    {
        public async Task<bool?> ShowEntityDialogBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase
        {
            return await Task.Factory.StartNew(()=> { return true; });
        }

        public async Task<bool?> ShowCRUDDialogBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase
        {
            return await Task.Factory.StartNew(() => { return true; });
        }

        public async Task<bool?> ShowEntityPickerBox<T>(CrudDialogViewModel<T> myViewModel) where T : EntityBase
        {
            return await Task.Factory.StartNew(() => { return true; });
        }

        public string ShowInputBox(string message, string title, string iconPath)
        {
            return message;
        }

        public bool? ShowMessageBox(string message, string title, DialogBoxEnum BoxType, DialogBoxIconEnum icon)
        {
            return true;
        }
    }
}

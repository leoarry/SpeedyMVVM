using SpeedyMVVM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM.Navigation;
using System.Collections.ObjectModel;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.TestModel.Services
{
    public class EntitiesDialogBoxService : IDialogBoxService
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

        public bool? ShowViewModel(IViewModelBase viewModel)
        {
            throw new NotImplementedException();
        }

        public bool? ShowDialogBox(IDialogBox dialogBox)
        {
            throw new NotImplementedException();
        }

        Task<string> IDialogBoxService.ShowInputBox(string message, string title, string iconPath)
        {
            throw new NotImplementedException();
        }

        Task<bool?> IDialogBoxService.ShowMessageBox(string message, string title, DialogBoxEnum BoxType, DialogBoxIconEnum icon)
        {
            throw new NotImplementedException();
        }

        Task<bool?> IDialogBoxService.ShowViewModel(IViewModelBase viewModel)
        {
            throw new NotImplementedException();
        }

        Task<bool?> IDialogBoxService.ShowDialogBox(IDialogBox dialogBox)
        {
            throw new NotImplementedException();
        }
    }
}

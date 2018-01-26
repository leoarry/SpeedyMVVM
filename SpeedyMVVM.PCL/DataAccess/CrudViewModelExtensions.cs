using SpeedyMVVM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    public static class CrudViewModelExtensions
    {

        public static ObservableCollection<IRelayCommand> GetEntityCollectionContextMenu<T>(this CrudViewModel<T> instance) where T : EntityBase
        {
            return new ObservableCollection<IRelayCommand> { instance.AddCommand,
                                                             instance.RemoveCommand,
                                                             new RelayCommand(async()=> await instance.PopOutCommandExecute(instance.SelectedItem)),
                                                             new RelayCommand(async()=> await instance.PickEntitiesCommandExecute(e=> true)) };
        }

        /// <summary>
        /// Show a Dialog Box to pick entities from a repository service and add them to Items.
        /// </summary>
        /// <returns>Return TRUE in case of operation successful</returns>
        public static async Task<bool> PickEntitiesCommandExecute<T>(this CrudViewModel<T> viewModel, Expression<Func<T, bool>> expression) where T:EntityBase
        {
            if (viewModel.ServiceContainer != null && viewModel.DialogService != null)
            {
                var vm = new EntityPickerDialogViewModel<T>(viewModel.DataService, string.Format("{0} - Picker", typeof(T).Name));
                if (expression != null)
                {
                    vm.HiddenExpression = expression;
                    await vm.FilterCommandExecute();
                }
                var dialogResult = await viewModel.DialogService.ShowViewModel(vm);
                if (dialogResult == true)
                {
                    foreach (T entity in vm.SelectedItems)
                        viewModel.Items.Add(entity);
                    return true;
                }
                return false;
            }
            return false;
        }

        /// <summary>
        /// Open in a new pop-up the current instance of CRUDViewModel (or the entity 'parameter' when not null) using 'DialogService' when available.
        /// </summary>
        /// <param name="entity">Entity to show on the dialog, if null EntityPickerDialog will create a default instance of 'T'</param>
        /// <param name="popOutAction">Action to perform before close the dialog and set DialogResult=true (default action</param>
        /// <returns>Return TRUE in case operation successful</returns>
        public async static Task<bool> PopOutCommandExecute<T>(this CrudViewModel<T> viewModel, T entity=null, Action popOutAction = null) where T:EntityBase
        {
            bool dialogResult = false;
            if (viewModel.ServiceContainer != null && viewModel.DialogService != null)
            {
                if (entity != null)
                {
                    viewModel.SelectedItem = entity;
                    var vm = new EntityDialogViewModel<T>(viewModel.ServiceContainer, entity, popOutAction);
                    dialogResult = (bool)await viewModel.DialogService.ShowDialogBox(vm);
                }
                else
                {
                    var vm = new CrudDialogViewModel<T>(viewModel.ServiceContainer, typeof(T).Name, "");
                    dialogResult = (bool)await viewModel.DialogService.ShowDialogBox(vm);
                }
            }
            return dialogResult;
        }

        /// <summary>
        /// Populate 'SelectedItems' form parameter (IList).
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Return true when successful.</returns>
        public static async Task<bool> AddSelectionCommandExecute<T>(this CrudViewModel<T> viewModel, object parameter) where T:EntityBase
        {
            return await Task.Factory.StartNew(() =>
            {
                IList myItems = (IList)parameter;
                if (myItems.OfType<T>().Count() > 0)
                {
                    viewModel.SelectedItems = myItems.Cast<T>().ToObservableCollection();
                    return true;
                }
                return false;
            });
        }
    }
}

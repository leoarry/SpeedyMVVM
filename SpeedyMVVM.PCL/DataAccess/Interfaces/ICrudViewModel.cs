using System;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.DataAccess
{
    public interface ICrudViewModel<T>:IViewModelBase where T : EntityBase
    {
        IRepositoryService<T> DataService { get; set; }

        RelayCommand<T> AddCommand { get; }
        RelayCommand<T> RemoveCommand { get; }
        RelayCommand<T> SaveCommand { get; }
        RelayCommand<Expression<Func<T, bool>>> SearchCommand { get; }

        T SelectedItem { get; set; }
        ObservableCollection<T> Items { get; set; }
        ObservableCollection<T> SelectedItems { get; set; }
    }
}
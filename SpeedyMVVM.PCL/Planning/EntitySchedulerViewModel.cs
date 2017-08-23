using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.DataAccess.Interfaces;

namespace SpeedyMVVM.Planning
{
    public class EntitySchedulerViewModel<T>:SchedulerViewModel<T> where T:EntityBase,IPlannable
    {
        #region Fields
        CRUDViewModel<T> _SelectionViewModel;
        #endregion

        #region Properties
        public override PlannedDay<T> SelectedPlannedDay
        {
            get
            {
                return base.SelectedPlannedDay;
            }

            set
            {
                base.SelectedPlannedDay = value;
                SelectionViewModel.Items = SelectedPlannedDay.Items;
            }
        }
        public CRUDViewModel<T> SelectionViewModel
        {
            get { return _SelectionViewModel; }
            set
            {
                if (_SelectionViewModel != value)
                {
                    _SelectionViewModel = value;
                    OnPropertyChanged(nameof(SelectionViewModel));
                }
            }
        }
        #endregion

        #region Methods
        public override void Initialize(ServiceLocator locator)
        {
            if (SelectionViewModel == null)
                SelectionViewModel = new CRUDViewModel<T>(locator);
            if (!SelectionViewModel.IsInitialized)
                SelectionViewModel.Initialize(locator);
            SelectionViewModel.CanSearch = false;
            SelectionViewModel.AddCommand = new RelayCommand<T>(async (item) =>
            {
                item = GetNewItemToAdd();
                await SelectionViewModel.AddCommandExecute(item);
            }, true);
            DataSource = SelectionViewModel.DataService.DataSet;
            base.Initialize(locator);
        }
        #endregion

        #region Costructors
        public EntitySchedulerViewModel() { }
        public EntitySchedulerViewModel(CRUDViewModel<T> viewModel)
        {
            SelectionViewModel = viewModel;
        }
        public EntitySchedulerViewModel(CRUDViewModel<T> viewModel, ServiceLocator locator)
        {
            SelectionViewModel = viewModel;
            Initialize(locator);
        }
        #endregion
    }
}

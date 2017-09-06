using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.DataAccess.Interfaces;
using System.Linq.Expressions;
using SpeedyMVVM.Expressions;

namespace SpeedyMVVM.Planning
{
    public class EntitySchedulerViewModel<T>:SchedulerViewModel<T> where T:EntityBase,IPlannable
    {
        #region Fields
        private Expression<Func<T, bool>> _DataSourceQuery;
        private CrudViewModel<T> _SelectionViewModel;
        #endregion

        #region Properties
        public Expression<Func<T,bool>> DataSourceQuery
        {
            get { return _DataSourceQuery; }
            set
            {
                if (_DataSourceQuery != value)
                {
                    _DataSourceQuery = value;
                    OnPropertyChanged(nameof(DataSourceQuery));
                }
            }
        }
        public CrudViewModel<T> SelectionViewModel
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

        #region Command Executions
        protected override async Task<bool> ComputePlanCommandExecute()
        {
            Expression<Func<T, bool>> query = r => r.PlannedDate >= StartingDate &&
                                                   r.PlannedDate <= EndingDate;
            if (DataSourceQuery != null)
                query = query.And(DataSourceQuery);

            DataSource = await ServiceContainer.GetService<IRepositoryService<T>>()
                .RetrieveCollectionAsync(query);
            return await base.ComputePlanCommandExecute();
        }
        #endregion

        #region Methods
        public override void Initialize(ServiceLocator locator)
        {
            if (SelectionViewModel == null)
                SelectionViewModel = new CrudViewModel<T>(locator);
            if (!SelectionViewModel.IsInitialized)
                SelectionViewModel.Initialize(locator);
            SelectionViewModel.CanSearch = false;
            base.Initialize(locator);
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(SelectedPlannedDay):
                    SelectionViewModel.Items = SelectedPlannedDay.Items;
                    SelectionViewModel.Items.CollectionChanged += SelectedPlannedDay_CollectionChanged;
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }

        protected override void SelectedPlannedDay_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.SelectedPlannedDay_CollectionChanged(sender, e);
            OnPropertyChanged(nameof(SelectionViewModel.Items));
            OnPropertyChanged(nameof(SelectedPlannedDay.Items));
        }
        #endregion

        #region Constructors
        public EntitySchedulerViewModel():base() { }
        public EntitySchedulerViewModel(CrudViewModel<T> viewModel) : base()
        {
            SelectionViewModel = viewModel;
        }
        public EntitySchedulerViewModel(CrudViewModel<T> viewModel, ServiceLocator locator) : base(locator)
        {
            SelectionViewModel = viewModel;
        }
        #endregion
    }
}

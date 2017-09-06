using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SpeedyMVVM.Planning
{
    public abstract class SchedulerViewModel<T>: ViewModelBase, IPageViewModel where T : IPlannable
    {

        #region Fields
        private int _FirstDay;
        private DateTime _StartingDate;
        private DateTime _EndingDate;
        private ObservableCollection<PlannedDayModel<T>> _Plan;
        private PlannedDayModel<T> _SelectedPlannedDay;
        private RelayCommand _ComputePlanCommand;
        private RelayCommand<PlannedDayModel<T>> _SelectPlanCommand;
        private ObservableCollection<T> _DataSource;
        private Expression<Func<T, bool>> _QueryExpression;
        #endregion

        #region IPageViewModel Implementation
        /// <summary>
        /// Path of the icon to show for this view model.
        /// </summary>
        public string IconPath { get; set; }

        /// <summary>
        /// Title of the View Model
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Get/Set View Model visibility.
        /// </summary>
        public bool IsVisible { get; set; }
        #endregion
        
        #region Property
        /// <summary>
        /// First day of the Plan (0==Monday...6==Sunday)
        /// </summary>
        public int FirstDay
        {
            get { return _FirstDay; }
            private set
            {
                if (value != _FirstDay)
                {
                    _FirstDay = value;
                    OnPropertyChanged(nameof(FirstDay));
                }
            }
        }

        /// <summary>
        /// Date when the planning must start from.
        /// </summary>
        public DateTime StartingDate
        {
            get
            {
                return _StartingDate;
            }
            set
            {
                if (_StartingDate != value)
                {
                    _StartingDate = value;
                    OnPropertyChanged(nameof(StartingDate));
                }
            }
        }

        /// <summary>
        /// Date when the planning must finish.
        /// </summary>
        public DateTime EndingDate
        {
            get
            {
                return _EndingDate;
            }
            set
            {
                if (_EndingDate != value)
                {
                    _EndingDate = value;
                    OnPropertyChanged(nameof(EndingDate));
                }
            }
        }

        /// <summary>
        /// Collection of planned day.
        /// </summary>
        public ObservableCollection<PlannedDayModel<T>> Plan
        {
            get { return _Plan; }
            set
            {
                if (_Plan != value)
                {
                    _Plan = value;
                    OnPropertyChanged(nameof(Plan));
                }
            }
        }

        /// <summary>
        /// Selected planned day.
        /// </summary>
        public PlannedDayModel<T> SelectedPlannedDay
        {
            get { return _SelectedPlannedDay; }
            set
            {
                if (_SelectedPlannedDay != value)
                {
                    if (_SelectedPlannedDay != null) { _SelectedPlannedDay.IsSelected = false; }
                    _SelectedPlannedDay = value;
                    _SelectedPlannedDay.IsSelected = true;
                    _SelectedPlannedDay.Items.CollectionChanged += SelectedPlannedDay_CollectionChanged;
                    OnPropertyChanged(nameof(SelectedPlannedDay));
                }
            }
        }
        
        /// <summary>
        /// Collection of IPlannable objects to use to create the Plan.
        /// </summary>
        protected ObservableCollection<T> DataSource { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Command to execute the scheduling calculation
        /// </summary>
        public RelayCommand ComputePlanCommand
        {
            get
            {
                return (_ComputePlanCommand == null) ? _ComputePlanCommand = new RelayCommand(async()=> await ComputePlanCommandExecute(), true) : _ComputePlanCommand;
            }
            set
            {
                if (_ComputePlanCommand != value)
                {
                    _ComputePlanCommand = value;
                    OnPropertyChanged(nameof(ComputePlanCommandExecute));
                }
            }
        }

        /// <summary>
        /// Command to set the 'SelectedPlannedDay' from parameter.
        /// </summary>
        public RelayCommand<PlannedDayModel<T>> SelectPlanCommand
        {
            get { return (_SelectPlanCommand == null) ? 
                    _SelectPlanCommand = new RelayCommand<PlannedDayModel<T>>(SelectPlanCommandExecute, true) : _SelectPlanCommand; }
            set { _SelectPlanCommand = value; }
        }
        #endregion

        #region Commands Executions
        /// <summary>
        /// Create the Plan from 'StartingDate' to 'EndingDate' using the collection 'DataSource'.
        /// </summary>
        protected virtual async Task<bool> ComputePlanCommandExecute()
        {
            if (_StartingDate > _EndingDate)
                throw new ArgumentOutOfRangeException(nameof(StartingDate), "Can't be major than EndingDate!");
            if (_EndingDate < _StartingDate)
                throw new ArgumentOutOfRangeException(nameof(EndingDate), "Can't be major than StartingDate!");

            //Set the first day
            if (_StartingDate.DayOfWeek == DayOfWeek.Sunday) { FirstDay = 6; }
            else { FirstDay = (int)_StartingDate.DayOfWeek - 1; }

            Plan = await Task.Factory.StartNew(() =>
            {
                var myPlan = new ObservableCollection<PlannedDayModel<T>>();

                //Instance the Plan
                DateTime date = _StartingDate;
                while (date <= _EndingDate)
                {
                    var p = new PlannedDayModel<T>(date);
                    p.Items = DataSource;
                    myPlan.Add(p);
                    date = date.AddDays(1);
                }
                SelectedPlannedDay = myPlan.Where(p => p.PlannedDate == _StartingDate).FirstOrDefault();
                return myPlan;
            });
            return true;
        }

        /// <summary>
        /// Set parameter 'plan' as 'SelectedPlannedDay'.
        /// </summary>
        /// <param name="plan">Planned Day to set as Selected.</param>
        public void SelectPlanCommandExecute(PlannedDayModel<T> plan)
        {
            if (plan == null) { return; }
            SelectedPlannedDay = plan;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Event handler for SelectedPlannedDay.Items.CollectionChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void SelectedPlannedDay_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if(e.Action== NotifyCollectionChangedAction.Add)
            {
                foreach(T i in e.NewItems)
                    i.PlannedDate = SelectedPlannedDay.PlannedDate;
            }
        }

        /// <summary>
        /// Initialize the current instance of SchedulerViewModel.
        /// </summary>
        /// <param name="service">Service container with current services.</param>
        public override void Initialize(ServiceLocator service)
        {
            this.ServiceContainer = service;
            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            _StartingDate = date;
            _EndingDate = date.AddDays(6);
            IsInitialized = true;
        }
        #endregion

        #region Constructors
        public SchedulerViewModel()
        {
        }
        public SchedulerViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }
        #endregion
    }
}

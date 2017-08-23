using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedyMVVM.Planning
{
    public class SchedulerViewModel<T>: ViewModelBase, IPageViewModel where T : IPlannable
    {

        #region Fields
        private int _FirstDay;
        private DateTime _StartingDate;
        private ObservableCollection<PlannedDay<T>> _Plan;
        protected PlannedDay<T> _SelectedPlannedDay;
        private RelayCommand<PlannedDay<T>> _SelectPlanCommand;
        private IQueryable<T> _DataSource;
        private int _PlanningLenght;
        #endregion

        #region IPageViewModel Implementation
        public string IconPath { get; set; }
        public string Title { get; set; }
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
            get { return _StartingDate; }
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
        /// Collection of planned day.
        /// </summary>
        public ObservableCollection<PlannedDay<T>> Plan
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
        public virtual PlannedDay<T> SelectedPlannedDay
        {
            get { return _SelectedPlannedDay; }
            set
            {
                if (_SelectedPlannedDay != value)
                {
                    if (_SelectedPlannedDay != null) { _SelectedPlannedDay.IsSelected = false; }
                    _SelectedPlannedDay = value;
                    _SelectedPlannedDay.IsSelected = true;
                    OnPropertyChanged(nameof(SelectedPlannedDay));
                }
            }
        }
        
        /// <summary>
        /// Datasource to query.
        /// </summary>
        public IQueryable<T> DataSource
        {
            get { return _DataSource; }
            set
            {
                if (_DataSource != value)
                {
                    _DataSource = value;
                    OnPropertyChanged(nameof(DataSource));
                }
            }
        }

        /// <summary>
        /// Set the lenght in days of the plan calculation.
        /// </summary>
        public int PlanningLenght
        {
            get { return _PlanningLenght; }
            set
            {
                if (_PlanningLenght != value)
                {
                    _PlanningLenght = value;
                    OnPropertyChanged(nameof(PlanningLenght));
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand<PlannedDay<T>> SelectPlanCommand
        {
            get { return (_SelectPlanCommand == null) ? 
                    _SelectPlanCommand = new RelayCommand<PlannedDay<T>>(SelectPlanCommandExecute, true) : _SelectPlanCommand; }
            set { _SelectPlanCommand = value; }
        }
        #endregion

        #region Commands Executions
        public void SelectPlanCommandExecute(PlannedDay<T> plan)
        {
            if (plan == null) { return; }
            SelectedPlannedDay = plan;
        }
        #endregion

        #region Methods
        protected T GetNewItemToAdd()
        {
            if (SelectedPlannedDay == null) { return default(T); }
            var item = Activator.CreateInstance<T>();
            item.PlannedDate = SelectedPlannedDay.PlannedDate;
            return item;
        }
        /// <summary>
        /// Initialize the current instance of SchedulerViewModel.
        /// </summary>
        /// <param name="service">Service container with current services.</param>
        public override void Initialize(ServiceLocator service)
        {
            this.ServiceContainer = service;
            SelectPlanCommand = new RelayCommand<PlannedDay<T>>(SelectPlanCommandExecute, true);
            SetMonthPlan();
            IsInitialized = true;
        }

        /// <summary>
        /// Initailize the Plan property.
        /// </summary>
        protected virtual async void SetMonthPlan()
        {
            Plan = await Task.Factory.StartNew(() =>
            {
                var myPlan = new ObservableCollection<PlannedDay<T>>();
                var startDate = new DateTime(StartingDate.Year, StartingDate.Month, StartingDate.Day);
                //Set the first day
                if (startDate.DayOfWeek == DayOfWeek.Sunday) { FirstDay = 6; }
                else { FirstDay = (int)startDate.DayOfWeek - 1; }
                //Get the ending date
                DateTime endDate = startDate.AddDays((double)PlanningLenght);
                //Instance the Plan
                while (startDate <= endDate)
                {
                    var p = new PlannedDay<T>(startDate);
                    myPlan.Add(p);
                    startDate = startDate.AddDays(1);
                }

                //Query the datasource
                var query = DataSource.Where(r => r.PlannedDate >= new DateTime(StartingDate.Year, StartingDate.Month, StartingDate.Day)
                                                     && r.PlannedDate <= endDate);
                //Create the month planner list
                foreach (var k in myPlan)
                {
                    k.Items = query.Where(r => r.PlannedDate >= k.PlannedDate &&
                                                   r.PlannedDate < k.PlannedDate.AddDays(1)).AsObservableCollection();
                }
                return myPlan;
            });
        }
        #endregion

        #region Costructors

        #endregion
    }
}

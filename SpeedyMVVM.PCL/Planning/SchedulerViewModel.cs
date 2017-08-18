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
        private int _CurrentYear;
        private ObservableCollection<int> _ListOfYears;
        private MonthsOfTheYear _CurrentMonth;
        private ObservableCollection<PlannedDay<T>> _Plan;
        private PlannedDay<T> _SelectedPlannedDay;
        private RelayCommand<PlannedDay<T>> _SelectPlanCommand;
        private IQueryable<T> _DataSource;
        private PlannerLenghtEnum _PlannerLenght;
        #endregion

        #region IViewModelBase Implementation
        public override void Initialize(ServiceLocator service)
        {
            this.ServiceContainer = service;
            SelectPlanCommand = new RelayCommand<PlannedDay<T>>(SelectPlanCommandExecute, true);
            CurrentYear = DateTime.Now.Year;
            ListOfYears = GetListOfYears();
            CurrentMonth = (MonthsOfTheYear)DateTime.Now.Month;
            SetMonthPlan();
            IsInitialized = true;
        }
        
        #endregion

        #region IPageViewModel Implementation
        public string IconPath { get; set; }
        public string Title { get; set; }
        public bool IsVisible { get; set; }
        #endregion

        #region Events
        public EventHandler<PlannedDay<T>> PlannedDayChanged = delegate { };
        #endregion

        #region Property
        /// <summary>
        /// First day of the Plan (0==Monday...6==Sunday)
        /// </summary>
        public int FirstDay
        {
            get { return _FirstDay; }
            set
            {
                if (value != _FirstDay)
                {
                    _FirstDay = value;
                    OnPropertyChanged(nameof(FirstDay));
                }
            }
        }

        /// <summary>
        /// Selected year for the planner.
        /// </summary>
        public int CurrentYear
        {
            get { return _CurrentYear; }
            set
            {
                if (_CurrentYear != value)
                {
                    _CurrentYear = value;
                    SelectedPlannedDay = null;
                    SetMonthPlan();
                    OnPropertyChanged(nameof(CurrentYear));
                }
            }
        }

        /// <summary>
        /// List of years.
        /// </summary>
        public ObservableCollection<int> ListOfYears
        {
            get { return _ListOfYears; }
            set
            {
                if (_ListOfYears != value)
                {
                    _ListOfYears = value;
                    OnPropertyChanged(nameof(ListOfYears));
                }
            }
        }

        /// <summary>
        /// Selected month for the planner.
        /// </summary>
        public MonthsOfTheYear CurrentMonth
        {
            get { return _CurrentMonth; }
            set
            {
                if (_CurrentMonth != value)
                {
                    _CurrentMonth = value;
                    SetMonthPlan();
                    OnPropertyChanged(nameof(CurrentMonth));
                    SelectedPlannedDay = null;
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
        /// Set the lenght of the plan calculation.
        /// </summary>
        public PlannerLenghtEnum PlannerLenght
        {
            get { return _PlannerLenght; }
            set
            {
                if (_PlannerLenght != value)
                {
                    _PlannerLenght = value;
                    OnPropertyChanged(nameof(PlannerLenght));
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand<PlannedDay<T>> SelectPlanCommand
        {
            get { return _SelectPlanCommand; }
            set { _SelectPlanCommand = value; }
        }
        #endregion

        #region Commands Executions
        protected virtual void AddCommandExecution(T item)
        {
            if (SelectedPlannedDay == null) { return; }
            if (item == null) { item = Activator.CreateInstance<T>(); }
            item.PlannedDate = SelectedPlannedDay.PlannedDate;
        }
        public void SelectPlanCommandExecute(PlannedDay<T> plan)
        {
            if (plan == null) { return; }
            SelectedPlannedDay = plan;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get a list of years starting from the current year.
        /// </summary>
        /// <returns></returns>
        private ObservableCollection<int> GetListOfYears()
        {
            var currYear = _CurrentYear - 9;
            var result = new ObservableCollection<int>();
            for (int i = 0; i <= 10; i++)
            {
                result.Add(currYear);
                currYear++;
            }
            return result;
        }

        /// <summary>
        /// Initailize the Plan property.
        /// </summary>
        protected virtual async void SetMonthPlan()
        {
            Plan = await Task.Factory.StartNew(() =>
            {
                var myPlan = new ObservableCollection<PlannedDay<T>>();
                var startDate = new DateTime(CurrentYear, (int)CurrentMonth, 1);
                //Set the first day
                if (startDate.DayOfWeek == DayOfWeek.Sunday) { FirstDay = 6; }
                else { FirstDay = (int)startDate.DayOfWeek - 1; }
                //Get the ending date
                DateTime endDate;
                if (PlannerLenght == PlannerLenghtEnum.Weekly)
                    endDate = new DateTime(CurrentYear, (int)CurrentMonth, startDate.Day + 7);
                else
                    endDate = new DateTime(CurrentYear, (int)CurrentMonth, DateTime.DaysInMonth(CurrentYear, (int)CurrentMonth));
                
                //Instance the Plan
                while (startDate <= endDate)
                {
                    var p = new PlannedDay<T>(startDate);
                    myPlan.Add(p);
                    startDate = startDate.AddDays(1);
                }

                //Query the datasource
                var query = DataSource.Where(r => r.PlannedDate >= startDate
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
    }
}

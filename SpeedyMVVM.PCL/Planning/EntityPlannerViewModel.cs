using SpeedyMVVM.DataAccess;
using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedyMVVM.Planning
{
    public abstract class EntityPlannerViewModel<T> : ViewModelBase, IPageViewModel where T : EntityBase, IPlannable
    {
        #region Fields
        private int _FirstDay;
        private int _CurrentYear;
        private ObservableCollection<int> _ListOfYears;
        private MonthsOfTheYear _CurrentMonth;
        private ObservableCollection<PlannedDay<T>> _MonthPlan;
        private PlannedDay<T> _SelectedPlan;
        private RelayCommand<PlannedDay<T>> _SelectPlanCommand;
        protected IRepositoryService<T> dataService;
        private CRUDViewModel<T> _SelectedPlanDetails;
        #endregion

        #region IPageViewModel Implementation
        public string IconPath { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
        #endregion

        #region Property
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
        public int CurrentYear
        {
            get { return _CurrentYear; }
            set
            {
                if (_CurrentYear != value)
                {
                    _CurrentYear = value;
                    SelectedPlan = null;
                    SetPlan();
                    OnPropertyChanged(nameof(CurrentYear));
                }
            }
        }
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
        public MonthsOfTheYear CurrentMonth
        {
            get { return _CurrentMonth; }
            set
            {
                if (_CurrentMonth != value)
                {
                    _CurrentMonth = value;
                    SetPlan();
                    OnPropertyChanged(nameof(CurrentMonth));
                    SelectedPlan = null;
                }
            }
        }
        public PlannedDay<T> SelectedPlan
        {
            get { return _SelectedPlan; }
            set
            {
                if (_SelectedPlan != value)
                {
                    if (_SelectedPlan != null) { _SelectedPlan.IsSelected = false; }
                    _SelectedPlan = value;
                    _SelectedPlan.IsSelected = true;
                    _SelectedPlanDetails.Filter.Items = _SelectedPlan.Items;
                    OnPropertyChanged(nameof(SelectedPlan));
                }
            }
        }
        public ObservableCollection<PlannedDay<T>> MonthPlan
        {
            get { return _MonthPlan; }
            set
            {
                if (_MonthPlan != value)
                {
                    _MonthPlan = value;
                    OnPropertyChanged(nameof(MonthPlan));
                }
            }
        }
        public CRUDViewModel<T> SelectedPlanDetails
        {
            get { return _SelectedPlanDetails; }
            set
            {
                if (_SelectedPlanDetails != value)
                {
                    _SelectedPlanDetails = value;
                    OnPropertyChanged(nameof(SelectedPlanDetails));
                    OnPropertyChanged(nameof(MonthPlan));
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
            if (SelectedPlan == null) { return; }
            if (item == null) { item = Activator.CreateInstance<T>(); }
            item.PlannedDate = SelectedPlan.PlannedDate;
            SelectedPlanDetails.AddCommandExecute(item);
        }
        private void SelectPlanCommandExecute(PlannedDay<T> plan)
        {
            if (plan == null) { return; }
            SelectedPlan = plan;
        }
        #endregion

        #region Methods
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
        protected virtual async void SetPlan()
        {
            var myPlan = new ObservableCollection<PlannedDay<T>>();
            var startDate = new DateTime(CurrentYear, (int)CurrentMonth, 1);
            //Set the first day
            if (startDate.DayOfWeek == DayOfWeek.Sunday) { FirstDay = 6; }
            else { FirstDay = (int)startDate.DayOfWeek - 1; }
            //Get the ending date
            var endDate = new DateTime(CurrentYear, (int)CurrentMonth, DateTime.DaysInMonth(CurrentYear, (int)CurrentMonth));
            //Query the DB
            await dataService.SaveChangesAsync();
            var query = await dataService.RetrieveCollectionAsync(r => r.PlannedDate >= startDate
                                                    && r.PlannedDate <= endDate);
            while (startDate <= endDate)
            {
                var p = new PlannedDay<T>(startDate);
                myPlan.Add(p);
                startDate = startDate.AddDays(1);
            }
            //Create the month planner list
            foreach (var k in myPlan)
            {
                var nextDay = k.PlannedDate.AddDays(1);
                var res = query.Where(r => r.PlannedDate >= k.PlannedDate &&
                                           r.PlannedDate < nextDay);
                k.Items = (res != null) ? new ObservableCollection<T>(res) : new ObservableCollection<T>();
            }
            MonthPlan = myPlan;
        }
        #endregion

        public EntityPlannerViewModel(ServiceLocator service)
        {
            base.ServiceContainer = service;
            dataService = service.GetService<IRepositoryService<T>>();
            SelectPlanCommand = new RelayCommand<PlannedDay<T>>(SelectPlanCommandExecute, true);
            _CurrentYear = DateTime.Now.Year;
            _ListOfYears = GetListOfYears();
            _CurrentMonth = (MonthsOfTheYear)DateTime.Now.Month;
            SetPlan();
        }
    }
}

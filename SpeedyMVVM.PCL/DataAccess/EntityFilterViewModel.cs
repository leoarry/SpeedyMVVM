using SpeedyMVVM.Expressions;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Class to filter a collection of <typeparamref name="T"/> using DataService while available.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class EntityFilterViewModel<T> : ViewModelBase where T : EntityBase
    {
        #region Field
        private IRepositoryService<T> _DataService;
        private RelayCommand<ExpressionModel> _AddFilter;
        private RelayCommand _RemoveFilter;
        private RelayCommand<object> _FilterCommand;
        private ObservableCollection<ExpressionModel> _Filters;
        private ExpressionModel _SelectedFilter;
        private ObservableCollection<T> _Items;
        #endregion

        #region Property
        /// <summary>
        /// Collection of filters used to build the Expression.
        /// </summary>
        public ObservableCollection<ExpressionModel> Filters
        {
            get { return _Filters ?? (_Filters = new ObservableCollection<ExpressionModel>()); }
            set { SetProperty(ref _Filters, value); }
        }

        /// <summary>
        /// Selected filter from the collection 'Filters'.
        /// </summary>
        public ExpressionModel SelectedFilter
        {
            get { return _SelectedFilter; }
            set { SetProperty(ref _SelectedFilter, value); }
        }

        /// <summary>
        /// Filtered collection of <typeparamref name="T"/>.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return _Items ?? (_Items = new ObservableCollection<T>()); }
            set { SetProperty(ref _Items, value); }
        }

        /// <summary>
        /// Data service where retrieve data.
        /// </summary>
        public IRepositoryService<T> DataService
        {
            get { return _DataService; }
            set { _DataService = value; }
        }

        /// <summary>
        /// Hidden expression which will be added in AND logic to the 'Filters' generated expression.
        /// </summary>
        public Expression<Func<T, bool>> HiddenExpression { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Add a new filter to the collection 'Filters'.
        /// </summary>
        public RelayCommand<ExpressionModel> AddFilterCommand
        {
            get { return _AddFilter ?? (_AddFilter = new RelayCommand<ExpressionModel>((model) => Filters.Add(model??(new ExpressionModel())), true)); }
        }

        /// <summary>
        /// Remove the 'SelectedFilter' from the 'Filters' collection.
        /// </summary>
        public RelayCommand RemoveFilterCommand
        {
            get { return _RemoveFilter ?? (_RemoveFilter = new RelayCommand(() => Filters.Remove(SelectedFilter))); }
        }

        /// <summary>
        /// Command to execute the query using 'DataService' to retrieve the collection 'Items'.
        /// </summary>
        public RelayCommand<object> FilterCommand
        {
            get
            {
                return _FilterCommand ?? (_FilterCommand = new RelayCommand<object>(async(param)=> await FilterCommandExecute(param), true));
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute the query using 'DataService' to retrieve the collection 'Items'.
        /// </summary>
        public async Task<bool> FilterCommandExecute()
        {
            if (DataService != null)
                Items = await FilterFromDataService();
            else
                Items = await FilterFromCollection(Items);
            return true;
        }

        /// <summary>
        /// Set SelectedFilter.Value as parameter 'value' (if not null) then execute FilterCommandExecute().
        /// </summary>
        /// <param name="value">Value to set for selected Filter</param>
        /// <returns></returns>
        private async Task<bool> FilterCommandExecute(object value)
        {
            if (value != null)
                SelectedFilter.Value = value;
            return await FilterCommandExecute();
        }

        /// <summary>
        /// Return a collection queryied from DataService, filtered using the filters collection and hidden expression.
        /// </summary>
        /// <returns>Result of the query.</returns>
        public async Task<ObservableCollection<T>> FilterFromDataService()
        {
            if (DataService == null)
                return null;
            var myExp = GetExpression();
            var myList = await DataService.RetrieveCollection(myExp);
            return (myList != null) ? myList : new ObservableCollection<T>();
        }

        /// <summary>
        /// Return the collection passed as parameter, filtered using the filters collection and hidden expression.
        /// </summary>
        /// <param name="collection">Collection to filter.</param>
        /// <returns>Filtered collection.</returns>
        public async Task<ObservableCollection<T>> FilterFromCollection(ObservableCollection<T> collection)
        {
            return await Task.Factory.StartNew(() =>
            {
                var myExp = GetExpression();
                return (collection != null) ? collection.AsQueryable().Where(myExp).ToObservableCollection() : new ObservableCollection<T>();
            });
        }

        /// <summary>
        /// Initialize EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services.</param>
        public override void InjectServices(ServiceLocator locator)
        {
            base.InjectServices(locator);
            DataService = GetService<IRepositoryService<T>>();
        }

        /// <summary>
        /// Private method to compute the expression.
        /// </summary>
        /// <returns></returns>
        private Expression<Func<T,bool>> GetExpression()
        {
            Expression<Func<T, bool>> myExp = null;
            if (_SelectedFilter != null && !Filters.Contains(_SelectedFilter))
                Filters.Add(_SelectedFilter);
            if (_Filters != null && _Filters.Count > 0)
            {
                myExp = ExpressionBuilder.GetExpression<T>(_Filters);
                if (HiddenExpression != null)
                    myExp = myExp.And(HiddenExpression);
            }
            else if (HiddenExpression != null)
            {
                myExp = HiddenExpression;
            }
            return myExp;
        }
        #endregion

        #region Methods Overriding
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(SelectedFilter))
                RemoveFilterCommand.IsEnabled = SelectedFilter != null;
            base.OnPropertyChanged(propertyName);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of EntityFilterViewModel
        /// </summary>
        public EntityFilterViewModel()
        {
        }

        /// <summary>
        /// Initialize a new instance of EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services</param>
        /// <param name="expressionModels">ExpressionModels to add to the 'Filters' collection</param>
        public EntityFilterViewModel(ServiceLocator locator, params ExpressionModel[] expressionModels)
        {
            ServiceContainer = locator;
            DataService = locator.GetService<IRepositoryService<T>>();
            if (expressionModels != null)
            {
                _SelectedFilter = expressionModels[0];
                _Filters = expressionModels.ToObservableCollection();
            }
        }

        /// <summary>
        /// Initialize a new instance of EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services</param>
        /// <param name="expressionModels">ExpressionModels to add to the 'Filters' collection</param>
        public EntityFilterViewModel(IRepositoryService<T> dataService, params ExpressionModel[] expressionModels)
        {
            DataService = dataService;
            if (expressionModels != null)
            {
                _SelectedFilter = expressionModels[0];
                _Filters = expressionModels.ToObservableCollection();
            }
        }
        #endregion
    }
}

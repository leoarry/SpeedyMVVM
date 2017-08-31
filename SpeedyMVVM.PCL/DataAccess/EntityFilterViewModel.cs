using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Expressions;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Class to filter a collection of <typeparamref name="T"/> using DataService while available.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class EntityFilterViewModel<T> : ViewModelBase where T : EntityBase
    {
        #region Field
        private RelayCommand _FilterCommand;
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
            get { return (_Filters == null) ? _Filters = new ObservableCollection<ExpressionModel>() : _Filters; }
            set
            {
                if (_Filters != value)
                {
                    _Filters = value;
                    OnPropertyChanged(nameof(Filters));
                }
            }
        }

        /// <summary>
        /// Selected filter from the collection 'Filters'.
        /// </summary>
        public ExpressionModel SelectedFilter
        {
            get { return (_SelectedFilter == null) ? _SelectedFilter = new ExpressionModel() : _SelectedFilter; }
            set
            {
                if (_SelectedFilter != value)
                {
                    _SelectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                }
            }
        }

        /// <summary>
        /// List of properties names for <typeparamref name="T"/>.
        /// </summary>
        public ObservableCollection<string> PropertyList
        {
            get
            {
                var pInfo = typeof(T).GetRuntimeProperties();
                List<string> myList = new List<string>();
                myList = pInfo.Where(p => !p.PropertyType.GetTypeInfo().IsSubclassOf(typeof(EntityBase)) &&
                    !(p.PropertyType.GetTypeInfo().IsGenericType && p.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(ObservableCollection<>))).Select(p => p.Name).ToList();
                myList.Remove(nameof(EntityBase.EntityStatus));
                return new ObservableCollection<string>(myList);
            }
        }

        /// <summary>
        /// Filtered collection of <typeparamref name="T"/>.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return (_Items == null) ? _Items = new ObservableCollection<T>() : _Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }

        /// <summary>
        /// Data service where retrieve data.
        /// </summary>
        public virtual IRepositoryService<T> DataService { get; set; }

        /// <summary>
        /// Hidden expression which will be added in AND logic to the 'Filters' generated expression.
        /// </summary>
        public Expression<Func<T, bool>> HiddenExpression { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Add a new filter to the collection 'Filters'.
        /// </summary>
        public RelayCommand AddFilterCommand
        {
            get { return new RelayCommand(() => Filters.Add(new ExpressionModel())); }
        }

        /// <summary>
        /// Remove the 'SelectedFilter' from the 'Filters' collection.
        /// </summary>
        public RelayCommand RemoveFilterCommand
        {
            get { return new RelayCommand(() => Filters.Remove(SelectedFilter)); }
        }

        /// <summary>
        /// Command to execute the query using 'DataService' to retrieve the collection 'Items'.
        /// </summary>
        public RelayCommand FilterCommand
        {
            get
            {
                return (_FilterCommand == null) ? _FilterCommand = new RelayCommand(async()=> await FilterCommandExecute(), true) : _FilterCommand;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute the query using 'DataService' to retrieve the collection 'Items'.
        /// </summary>
        public virtual async Task<bool> FilterCommandExecute()
        {
            if (DataService != null)
                Items = await GetCollectionFromDataService();
            else
                Items = await FilterCollection(Items);
            return true;
        }

        /// <summary>
        /// Execute the query using 'DataService' to return a collection.
        /// </summary>
        /// <returns>Result of the query.</returns>
        public async Task<ObservableCollection<T>> GetCollectionFromDataService()
        {
            if (DataService == null)
                return null;
            var myExp = GetExpression();
            var myList = await DataService.RetrieveCollectionAsync(myExp);
            return (myList != null) ? myList : new ObservableCollection<T>();
        }

        /// <summary>
        /// Filter the collection passed as parameter.
        /// </summary>
        /// <param name="collection">Collection to filter.</param>
        /// <returns>Filtered collection.</returns>
        public async Task<ObservableCollection<T>> FilterCollection(ObservableCollection<T> collection)
        {
            return await Task.Factory.StartNew(() =>
            {
                var myExp = GetExpression();
                return (collection != null) ? collection.AsQueryable().Where(myExp).AsObservableCollection() : new ObservableCollection<T>();
            });
        }

        /// <summary>
        /// Initialize EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services.</param>
        public override void Initialize(ServiceLocator locator)
        {
            ServiceContainer = locator;
            DataService = locator.GetService<IRepositoryService<T>>();
            IsInitialized = true;
        }

        /// <summary>
        /// Private method to compute the expression.
        /// </summary>
        /// <returns></returns>
        private Expression<Func<T,bool>> GetExpression()
        {
            Expression<Func<T, bool>> myExp = null;
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
        /// <param name="locator">ServiceLocator containing the services.</param>
        public EntityFilterViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }

        /// <summary>
        /// Initialize a new instance of EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services.</param>
        /// <param name="expressionModel">ExpressionModel to add to the 'Filters' collection.</param>
        public EntityFilterViewModel(ServiceLocator locator, ExpressionModel expressionModel)
        {
            Initialize(locator);
            _SelectedFilter = expressionModel;
            _Filters = new ObservableCollection<ExpressionModel>();
            _Filters.Add(_SelectedFilter);
        }

        /// <summary>
        /// Initialize a new instance of EntityFilter using the locator parameter.
        /// </summary>
        /// <param name="locator">ServiceLocator containing the services</param>
        /// <param name="expressionModels">ExpressionModels to add to the 'Filters' collection</param>
        public EntityFilterViewModel(ServiceLocator locator, ObservableCollection<ExpressionModel> expressionModels)
        {
            Initialize(locator);
            _SelectedFilter = expressionModels[0];
            _Filters = expressionModels;
        }
        #endregion
    }
}

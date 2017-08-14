using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Expressions;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Class to filter a collection of <typeparamref name="T"/> using DataService while available.
    /// </summary>
    /// <typeparam name="T">Entity type.</typeparam>
    public class EntityFilterViewModel<T> : ViewModelBase where T : IEntityBase
    {
        #region Field
        private RelayCommand _SearchCommand;
        private ObservableCollection<ExpressionModel> _Filters;
        private ExpressionModel _SelectedFilter;
        private ObservableCollection<T> _Items;
        #endregion

        #region Property
        public ObservableCollection<ExpressionModel> Filters
        {
            get { return _Filters; }
            set
            {
                if (_Filters != value)
                {
                    _Filters = value;
                    OnPropertyChanged(nameof(Filters));
                }
            }
        }
        public ExpressionModel SelectedFilter
        {
            get { return _SelectedFilter; }
            set
            {
                if (_SelectedFilter != value)
                {
                    _SelectedFilter = value;
                    OnPropertyChanged(nameof(SelectedFilter));
                }
            }
        }
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
        public ObservableCollection<T> Items
        {
            get { return _Items; }
            set
            {
                if (_Items != value)
                {
                    _Items = value;
                    OnPropertyChanged(nameof(Items));
                }
            }
        }
        public virtual IRepositoryService<T> DataService { get; set; }
        #endregion

        #region Commands
        public RelayCommand AddFilterCommand
        {
            get { return new RelayCommand(() => Filters.Add(new ExpressionModel())); }
        }
        public RelayCommand RemoveFilterCommand
        {
            get { return new RelayCommand(() => Filters.Remove(SelectedFilter)); }
        }
        public RelayCommand SearchCommand
        {
            get
            {
                return (_SearchCommand == null)? _SearchCommand = new RelayCommand(Search, true) : _SearchCommand;
            }
        }
        #endregion

        #region Methods
        public async void Search()
        {
            Items = await GetResult();
        }
        public async Task<ObservableCollection<T>> GetResult()
        {
            var myExp = ExpressionBuilder.GetExpression<T>(Filters);
            var myList = await DataService.RetrieveCollectionAsync(myExp);
            return (myList != null) ? myList : new ObservableCollection<T>();
        }
        public override void Initialize(ServiceLocator locator)
        {
            DataService = locator.GetService<IRepositoryService<T>>();
            _Filters = new ObservableCollection<ExpressionModel>();
            _Filters.Add(new ExpressionModel());
            _Items = new ObservableCollection<T>();
        }
        #endregion

        #region Costructors
        public EntityFilterViewModel()
        {
            _Filters = new ObservableCollection<ExpressionModel>();
            _Filters.Add(new ExpressionModel());
            _Items = new ObservableCollection<T>();
        }
        public EntityFilterViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }
        public EntityFilterViewModel(ServiceLocator locator, ExpressionModel expressionModel)
        {
            DataService = locator.GetService<IRepositoryService<T>>();
            _Filters = new ObservableCollection<ExpressionModel>();
            _Filters.Add(expressionModel);
            _Items = new ObservableCollection<T>();
        }
        public EntityFilterViewModel(ServiceLocator locator, ObservableCollection<ExpressionModel> expressionModels)
        {
            DataService = locator.GetService<IRepositoryService<T>>();
            _Filters = expressionModels;
            _Items = new ObservableCollection<T>();
        }
        #endregion
    }
}

using System;
using SpeedyMVVM.Utilities;
using System.Collections.ObjectModel;
using SpeedyMVVM.DataAccess.Interfaces;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Class which implement a basic CRUD operation with an EntityBase.
    /// </summary>
    /// <typeparam name="T">Type of Entity.</typeparam>
    public class CRUDViewModel<T> : ViewModelBase where T : IEntityBase
    {
        #region Fields
        private bool _CanSearch;
        private RelayCommand<T> _AddCommand;
        private RelayCommand _SaveCommand;
        private RelayCommand _RemoveCommand;
        private RelayCommand _SearchCommand;
        private IRepositoryService<T> _DataService;
        private EntityFilterViewModel<T> _Filter;
        private ObservableCollection<T> _SelectedItems;
        protected T _SelectedItem;
        #endregion

        #region Properties
        /// <summary>
        /// Filter the collection of CRUDViewModel adding query support.
        /// </summary>
        public EntityFilterViewModel<T> Filter
        {
            get { return (_Filter == null) ? _Filter = new EntityFilterViewModel<T>() : _Filter; }
            set
            {
                if (_Filter != value)
                {
                    _Filter = value;
                    OnPropertyChanged(nameof(Filter));
                }
            }
        }

        /// <summary>
        /// Set 'TRUE' if the filter can execute query.
        /// </summary>
        public bool CanSearch
        {
            get { return _CanSearch; }
            set
            {
                if (_CanSearch != value)
                {
                    _CanSearch = value;
                    OnPropertyChanged(nameof(CanSearch));
                }
            }
        }
        
        /// <summary>
        /// Repository Service where get data from.
        /// </summary>
        public IRepositoryService<T> DataService
        {
            get { return _DataService; }
            set
            {
                if (_DataService != value)
                {
                    _DataService = value;
                    Filter.DataService = value;
                }
            }
        }
        
        /// <summary>
        /// Collection of <typeparamref name="T"/>.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return Filter.Items; }
            set
            {
                Filter.Items = value;
                OnPropertyChanged(nameof(Items));
            }
        }

        /// <summary>
        /// Collection of <typeparamref name="T"/> selected from 'Items'.
        /// </summary>
        public ObservableCollection<T> SelectedItems
        {
            get { return _SelectedItems; }
            set
            {
                if (_SelectedItems != value)
                {
                    _SelectedItems = value;
                    OnPropertyChanged(nameof(SelectedItems));
                }
            }
        }

        /// <summary>
        /// Selected <typeparamref name="T"/> from 'Items' collection.
        /// </summary>
        public virtual T SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (!object.Equals(_SelectedItem,value))
                {
                    _SelectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to add a new <typeparamref name="T"/>.
        /// </summary>
        public RelayCommand<T> AddCommand
        {
            get
            {
                return (_AddCommand == null)?  
                    _AddCommand = new RelayCommand<T>(async(param)=> await AddCommandExecute(param), true) : _AddCommand;
            }
            set { _AddCommand = value; }
        }

        /// <summary>
        /// Command to remove a <typeparamref name="T"/>.
        /// </summary>
        public RelayCommand RemoveCommand
        {
            get
            {
                return (_RemoveCommand == null)? 
                    _RemoveCommand :  _RemoveCommand = new RelayCommand(async()=> await RemoveCommandExecute(), true);
            }
            set { _RemoveCommand = value; }
        }

        /// <summary>
        /// Command to save changes.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return (_SaveCommand == null) ? 
                    _SaveCommand = new RelayCommand(async()=> await SaveCommandExecute(), true) : _SaveCommand;
            }
            set { _SaveCommand = value; }
        }

        /// <summary>
        /// Command to search <typeparamref name="T"/> into 'Items' using 'Filter'
        /// </summary>
        public RelayCommand SearchCommand
        {
            get
            {
                return (_SearchCommand == null) ? 
                    _SearchCommand = new RelayCommand(async()=> await SearchCommandExecute(), true) : _SearchCommand;
            }
            set { _SearchCommand = value; }
        }
        #endregion

        #region Commands Executions
        /// <summary>
        /// Add new entity to 'Items' and through 'DataService' when available.
        /// </summary>
        /// <param name="entity">Entity to add (if null will automatically create a new istance)</param>
        /// <returns>Return TRUE in case of operation successfull</returns>
        public virtual async Task<bool> AddCommandExecute(T entity)
        {
            if (entity == null) { entity = Activator.CreateInstance<T>(); }
            entity.EntityStatus = EntityStatusEnum.Added;
            if(DataService!=null)
                entity = await DataService.AddEntityAsync(entity);
            Items.Add(entity);
            return true;
        }

        /// <summary>
        /// Remove 'SelectedItems' if Count>0 or 'SelectedItem' from 'Items' and through 'DataService' when available.
        /// </summary>
        /// <returns>Return TRUE in case operation successfull</returns>
        public virtual async Task<bool> RemoveCommandExecute()
        {
            bool result = false;
            if (_SelectedItems!=null && _SelectedItems.Count > 0)
            {
                foreach(var e in _SelectedItems)
                {
                    e.EntityStatus = EntityStatusEnum.Deleted;
                    Items.Remove(e);
                    if (DataService != null)
                        await DataService.RemoveEntityAsync(e);
                }
                SelectedItem = default(T);
                result = true;
            }else if (SelectedItem != null)
            {
                SelectedItem.EntityStatus = EntityStatusEnum.Deleted;
                Items.Remove(SelectedItem);
                if (DataService != null)
                    await DataService.RemoveEntityAsync(SelectedItem);
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Persist the collection 'Items' through 'DataService' when available.
        /// </summary>
        /// <returns>Return TRUE in case of operation successfull</returns>
        public virtual async Task<bool> SaveCommandExecute()
        {
            int? result=null;
            if(DataService!=null)
                result= await DataService.SaveChangesAsync();
            if (result == 0)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Retreive 'Items' from 'DataService' if exist, otherwise filter 'Items'.
        /// </summary>
        public virtual async Task<bool> SearchCommandExecute()
        {
            if (CanSearch)
                return await Filter.Search();
            else
                return false;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize CRUDViewModel getting 'DataService' from parameter 'service' and initializing 'Filter'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public override void Initialize(ServiceLocator locator)
        {
            Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(this.Items));
            ServiceContainer = locator;
            _CanSearch = true;
            DataService = ServiceContainer.GetService<IRepositoryService<T>>();
            IsInitialized = true;
        }
        #endregion

        #region Costructors
        /// <summary>
        /// Create a new istance of CRUDViewModel.
        /// </summary>
        public CRUDViewModel()
        {
            Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(this.Items));
        }

        /// <summary>
        /// Create a new istance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public CRUDViewModel(ServiceLocator locator)
        {
            Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(this.Items));
            Initialize(locator);
        }

        /// <summary>
        /// Create a new istance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        /// <param name="canSearch">Enable the viewmodel to execute query.</param>
        public CRUDViewModel(ServiceLocator locator, bool canSearch)
        {
            Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(this.Items));
            Initialize(locator);
            _CanSearch = canSearch;
        }
        #endregion
    }
}

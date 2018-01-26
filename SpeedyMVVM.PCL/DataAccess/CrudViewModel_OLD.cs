using System;
using SpeedyMVVM.Utilities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Collections;
using System.Linq;
using System.ComponentModel;
using SpeedyMVVM.Validation;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Specialized;
using SpeedyMVVM.Navigation;

namespace SpeedyMVVM.DataAccess
{
    /*/// <summary>
    /// Class which implement a basic CRUD operation with an EntityBase.
    /// </summary>
    /// <typeparam name="T">Type of Entity.</typeparam>
    public class CrudViewModel<T> : ViewModelBase where T : EntityBase
    {
        #region Fields
        private List<ValidationRule<T>> _ValidationRules;
        private bool _CanSearch;
        private RelayCommand<T> _AddCommand;
        private RelayCommand<object> _AddSelectionCommand;
        private RelayCommand<T> _RemoveCommand;
        private RelayCommand<T> _PopOutCommand;
        private RelayCommand _FilterCommand;
        private RelayCommand _SaveCommand;
        private ContextMenuItemModel _PickEntitiesCommand;
        private ContextMenuItemModel _RemoveSelectedItemCommand;
        private ContextMenuItemModel _PopOutSelectedItemCommand;
        private IRepositoryService<T> _DataService;
        private IDialogBoxService _DialogService;
        private EntityFilterViewModel<T> _Filter;
        private ObservableCollection<T> _SelectedItems;
        private T _SelectedItem;
        protected ObservableCollection<ContextMenuItemModel> _ContextMenu;
        private Expression<Func<T, bool>> _PickEntitiesExpression;
        #endregion

        #region Properties
        /// <summary>
        /// Get if there are errors on Items or SelectedItem.
        /// </summary>
        public bool IsValid { get { return !HasErrors; } }

        public bool HasErrors { get { return Items.Any(i => i.HasErrors) || (SelectedItem != null) ? SelectedItem.HasErrors : false; } }

        /// <summary>
        /// How to merge the validation rules while SelectedItem.PropertyChnaged
        /// </summary>
        public MergingActionEnum RulesMergingAction { get; set; }

        /// <summary>
        /// List of validation rules to be injected into the SelectedItem.Validator after property changed.
        /// </summary>
        public List<ValidationRule<T>> ValidationRules
        {
            get { return _ValidationRules ?? (_ValidationRules = new List<ValidationRule<T>>()); }
            set { _ValidationRules = value; }
        }
        
        /// <summary>
        /// Filter the collection of CRUDViewModel adding query support.
        /// </summary>
        public EntityFilterViewModel<T> Filter
        {
            get { return _Filter ?? (_Filter = new EntityFilterViewModel<T>()); }
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
        /// Set 'TRUE' if the CrudViewModel can persist entities throw DataService when available (Default=TRUE).
        /// </summary>
        public bool CanSave { get; set; }

        /// <summary>
        /// Set 'TRUE' if the CrudViewModel can add entities throw DataService when available (Default=TRUE).
        /// </summary>
        public bool CanAdd { get; set; }

        /// <summary>
        /// Set 'TRUE' if the filter can execute query using DataService (Default=TRUE).
        /// </summary>
        public bool CanSearch
        {
            get { return _CanSearch; }
            set
            {
                if (_CanSearch != value)
                {
                    _CanSearch = value;
                    //if (_CanSearch && _DataService!=null)
                        //Filter.DataService = DataService;
                    //else
                        //Filter.DataService = null;
                }
            }
        }

        /// <summary>
        /// If 'TRUE' will remove the entity from the DataService (Default=TRUE).
        /// </summary>
        public bool DeleteOnCascade { get; set; }

        /// <summary>
        /// Repository Service where get data from.
        /// </summary>
        public IRepositoryService<T> DataService
        {
            get
            {
                if (ServiceContainer == null)
                    throw new ArgumentNullException(nameof(ServiceLocator), "Can't initialize DataService, can't inject services! Parameter is null.");
                return _DataService ?? (_DataService = ServiceContainer.GetService<IRepositoryService<T>>());
            }
            set
            {
                _DataService = value;
                //if (_CanSearch)
                    //Filter.DataService = value;
            }
        }
        
        /// <summary>
        /// Service for UI dialogs.
        /// </summary>
        public IDialogBoxService DialogService
        {
            get
            {
                if (ServiceContainer == null)
                    throw new ArgumentNullException(nameof(ServiceLocator), "Can't initialize DialogService, can't inject services! Parameter is null.");
                return _DialogService ?? (_DialogService = ServiceContainer.GetService<IDialogBoxService>());
            }
            set { _DialogService = value; }
        }

        /// <summary>
        /// Collection of entities.
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
        /// Collection of entities actually selected from 'Items'.
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
        /// Selected entity from 'Items' collection.
        /// </summary>
        public T SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (!object.Equals(_SelectedItem,value))
                {
                    _SelectedItem = value;
                    if (_SelectedItem != null)
                    {
                        RemoveCommand.IsEnabled = true;
                        PopOutCommand.IsEnabled = true;
                        RemoveSelectedItemCommand.IsEnabled = true;
                        PopOutSelectedItemCommand.IsEnabled = true;
                        if (_SelectedItem.Validator == null)
                            _SelectedItem.GetAsyncValidator().MergeRules(ValidationRules.ToConcurrentBag(), RulesMergingAction);
                        _SelectedItem.Validate();
                    }
                    else
                    {
                        RemoveCommand.IsEnabled = false;
                        PopOutCommand.IsEnabled = false;
                        RemoveSelectedItemCommand.IsEnabled = false;
                        PopOutSelectedItemCommand.IsEnabled = false;
                    }
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        /// <summary>
        /// Collection of commands for the entity.
        /// </summary>
        public ObservableCollection<ContextMenuItemModel> ContextMenu
        {
            get
            {
                if (_ContextMenu == null)
                    SetContextMenu();
                return _ContextMenu;
            }
            set
            {
                if (_ContextMenu != value)
                {
                    _ContextMenu = value;
                    OnPropertyChanged(nameof(ContextMenu));
                }
            }
        }

        /// <summary>
        /// Expression used when executed the command 'PickEntitiesCommand'.
        /// </summary>
        public Expression<Func<T, bool>> PickEntitiesExpression
        {
            get { return _PickEntitiesExpression; }
            set
            {
                if (_PickEntitiesExpression != value)
                {
                    _PickEntitiesExpression = value;
                    OnPropertyChanged(nameof(PickEntitiesExpression));
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
                return _AddCommand ?? (_AddCommand = new RelayCommand<T>(async (param) => await AddCommandExecute(param), true));
            }
            set { _AddCommand = value; }
        }

        /// <summary>
        /// Command to populate 'SelectedItems' form parameter (IList).
        /// </summary>
        public RelayCommand<object> AddSelectionCommand
        {
            get
            {
                return _AddSelectionCommand ?? (_AddSelectionCommand = new RelayCommand<object>(async (param) => await AddSelectionCommandExecute(param), true));
            }
        }

        /// <summary>
        /// Command to remove a <typeparamref name="T"/>.
        /// </summary>
        public RelayCommand<T> RemoveCommand
        {
            get
            {
                return _RemoveCommand ?? (_RemoveCommand = new RelayCommand<T>(async (param) => await RemoveCommandExecute(param)));
            }
            set { _RemoveCommand = value; }
        }

        /// <summary>
        /// Pop-out the current instance of this CRUDViewModel in a new window. 
        /// </summary>
        public RelayCommand<T> PopOutCommand
        {
            get
            {
                return _PopOutCommand ?? (_PopOutCommand = new RelayCommand<T>(async (param) => await PopOutCommandExecute(param)));
            }
            set { _PopOutCommand = value; }
        }

        /// <summary>
        /// Command to search <typeparamref name="T"/> into 'Items' using 'Filter'
        /// </summary>
        public RelayCommand FilterCommand
        {
            get
            {
                return _FilterCommand ?? (_FilterCommand = new RelayCommand(async () => await FilterCommandExecute(), true));
            }
            set { _FilterCommand = value; }
        }
        
        /// <summary>
        /// Command to save changes.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return _SaveCommand ?? (_SaveCommand = new RelayCommand(async () => await SaveCommandExecute(), true));
            }
            set { _SaveCommand = value; }
        }

        /// <summary>
        /// Command to pick entities from a repository service and add them to 'Items'.
        /// </summary>
        public ContextMenuItemModel PickEntitiesCommand
        {
            get
            {
                return _PickEntitiesCommand ?? (_PickEntitiesCommand = new ContextMenuItemModel(
                        async () => await PickEntitiesCommandExecute(PickEntitiesExpression),
                        "Search Items"));
            }
            set { _PickEntitiesCommand = value; }
        }

        /// <summary>
        /// Command to remove the Selected Item from Items.
        /// </summary>
        public ContextMenuItemModel RemoveSelectedItemCommand
        {
            get
            {
                return _RemoveSelectedItemCommand ?? (_RemoveSelectedItemCommand = new ContextMenuItemModel(
                        async () => await RemoveCommandExecute(SelectedItem), "Remove Selected Item"));
            }
            set
            {
                if (_RemoveSelectedItemCommand != value)
                {
                    _RemoveSelectedItemCommand = value;
                    OnPropertyChanged(nameof(RemoveSelectedItemCommand));
                }
            }
        }

        /// <summary>
        /// Command to pop-out Selected Item in a new pop-up.
        /// </summary>
        public ContextMenuItemModel PopOutSelectedItemCommand
        {
            get
            {
                return _PopOutSelectedItemCommand ?? (_PopOutSelectedItemCommand = new ContextMenuItemModel(
                        async () => await PopOutCommandExecute(SelectedItem), "Edit Selected Item"));
            }
            set
            {
                if (_PopOutSelectedItemCommand != value)
                {
                    _PopOutSelectedItemCommand = value;
                    OnPropertyChanged(nameof(PopOutSelectedItemCommand));
                }
            }
        }
        #endregion

        #region Commands Executions
        /// <summary>
        /// Add new entity to 'Items' and through 'DataService' when available.
        /// </summary>
        /// <param name="entity">Entity to add (if null will automatically create a new instance)</param>
        /// <returns>Return TRUE in case of operation successful</returns>
        public virtual async Task<bool> AddCommandExecute(T entity)
        {
            if (entity == null) { entity = Activator.CreateInstance<T>(); }
            entity.EntityStatus = EntityStatusEnum.Added;
            Items.Add(entity);
            if (DataService != null && CanAdd)
                entity = DataService.Add(entity);
            return true;
        }

        /// <summary>
        /// Populate 'SelectedItems' form parameter (IList).
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>Return true when successful.</returns>
        public virtual async Task<bool> AddSelectionCommandExecute(object parameter)
        {
            return await Task.Factory.StartNew(() =>
            {
                IList myItems = (IList)parameter;
                if (myItems.OfType<T>().Count() > 0)
                {
                    SelectedItems = myItems.Cast<T>().ToObservableCollection();
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Remove or 'parameter' if not null or 'SelectedItems' if Count>0 or 'SelectedItem' from 'Items' and through 'DataService' when available.
        /// </summary>
        /// <returns>Return TRUE in case operation successful</returns>
        public virtual async Task<bool> RemoveCommandExecute(T entity)
        {
            if (entity != null)
            {
                entity.EntityStatus = EntityStatusEnum.Deleted;
                if (DeleteOnCascade && DataService != null)
                    DataService.Remove(entity);
                Items.Remove(entity);
                return true;
            }
            else if (_SelectedItems!=null && _SelectedItems.Count > 0)
            {
                foreach(var e in _SelectedItems)
                {
                    e.EntityStatus = EntityStatusEnum.Deleted;
                    if (DeleteOnCascade && DataService != null)
                        DataService.Remove(e);
                    Items.Remove(e);
                }
                SelectedItem = default(T);
                return true;
            }else if (SelectedItem != null)
            {
                SelectedItem.EntityStatus = EntityStatusEnum.Deleted;
                if (DeleteOnCascade && DataService != null)
                    DataService.Remove(SelectedItem);

                Items.Remove(SelectedItem);
                return true;
            }else
                return false;
        }

        /// <summary>
        /// Open in a new pop-up the current instance of CRUDViewModel (or the entity 'parameter' when not null) using 'DialogService' when available.
        /// </summary>
        /// <returns>Return TRUE in case operation successful</returns>
        public virtual bool PopOutCommandExecute(T entity)
        {
            if (ServiceContainer != null && DialogService != null)
            {
                if (entity != null)
                {
                    SelectedItem = entity;
                    var vm = new EntityDialogViewModel<T>(ServiceContainer, typeof(T).Name, "");
                    return (bool) DialogService.ShowViewModel(vm);
                }                  
                else
                {
                    var vm = new EntityDialogViewModel<T>(ServiceContainer, typeof(T).Name, "");
                    return (bool) DialogService.ShowViewModel(vm);
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieve 'Items' from 'DataService' if exist, otherwise filter 'Items'.
        /// </summary>
        /// <returns>Return TRUE in case of operation successful</returns>
        public virtual async Task<bool> FilterCommandExecute()
        {
            return await Filter.FilterCommandExecute();
        }

        /// <summary>
        /// Show a Dialog Box to pick entities from a repository service and add them to Items.
        /// </summary>
        /// <returns>Return TRUE in case of operation successful</returns>
        public virtual async Task<bool> PickEntitiesCommandExecute(Expression<Func<T,bool>> expression)
        {
            if (ServiceContainer != null && DialogService != null)
            {
                var vm = new EntityDialogViewModel<T>(ServiceContainer, string.Format("{0} - Picker", typeof(T).Name), "");
                if (expression != null)
                {
                    vm.ViewModel.Filter.HiddenExpression = expression;
                    //await vm.ViewModel.FilterCommandExecute();
                }
                if (await DialogService.ShowEntityPickerBox(vm) == true)
                {
                    foreach (T entity in vm.ViewModel.SelectedItems)
                        Items.Add(entity);
                    return true;
                }
                else
                    return false;
            }
            return false;
        }

        /// <summary>
        /// Persist the collection 'Items' through 'DataService' when available.
        /// </summary>
        /// <returns>Return TRUE in case of operation successful</returns>
        public virtual async Task<bool> SaveCommandExecute()
        {
            if (DataService != null && CanSave && IsValid)
            {
                if (DataService.Save() == 0)
                {
                    foreach (var i in Items)
                        i.EntityStatus = EntityStatusEnum.Unchanged;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else { return true; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize CRUDViewModel getting 'DataService' from parameter 'service' and initializing 'Filter'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public override void Initialize(ServiceLocator locator)
        {
            if (locator == null)
                throw new ArgumentNullException(nameof(ServiceLocator), "Can't initialize CrudViewModel, can't inject services! Parameter is null.");
            ServiceContainer = locator;
            DataService = locator.GetService<IRepositoryService<T>>();
            DialogService = locator.GetService<IEntityDialogBoxService>();
            IsInitialized = true;
        }

        /// <summary>
        /// Initialize a new instance of ContextMenu containing Add, Remove, Edit and Picker entity commands.
        /// </summary>
        protected virtual void SetContextMenu()
        {
            if(_ContextMenu==null)
                _ContextMenu = new ObservableCollection<ContextMenuItemModel>();
            _ContextMenu.Add(new ContextMenuItemModel(async () => await AddCommandExecute(null), "Add new"));// string.Format("Add new {0}", typeof(T).Name)));
            _ContextMenu.Add(RemoveSelectedItemCommand);
            _ContextMenu.Add(PopOutSelectedItemCommand);
            _ContextMenu.Add(PickEntitiesCommand);
        }

        /// <summary>
        /// Executed when a property of the item 'Filter' change the value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Items):
                    OnPropertyChanged(nameof(this.Items));
                    Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(Items));
                    break;
            }
        }

        protected virtual void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                foreach (T i in e.NewItems)
                {
                    i.GetAsyncValidator().MergeRules(ValidationRules.ToConcurrentBag(), RulesMergingAction);
                    i.Validate();
                }
        }

        /// <summary>
        /// Executed when a property change the value.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed value.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(Filter):
                    Filter.PropertyChanged += Filter_PropertyChanged;
                    break;
            }
            base.OnPropertyChanged(propertyName);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of CRUDViewModel.
        /// </summary>
        public CrudViewModel()
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            Items.CollectionChanged += Items_CollectionChanged;
            _CanSearch = true;
            DeleteOnCascade = true;
            CanAdd = true;
            CanSave = true;
        }
        
        /// <summary>
        /// Create a new instance of CRUDViewModel.
        /// </summary>
        /// <param name="canSearch">Enable the view model to execute query.</param>
        public CrudViewModel(bool canSearch)
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            Items.CollectionChanged += Items_CollectionChanged;
            _CanSearch = canSearch;
            DeleteOnCascade = true;
            CanAdd = true;
            CanSave = true;
        }

        /// <summary>
        /// Create a new instance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public CrudViewModel(ServiceLocator locator)
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            Items.CollectionChanged += Items_CollectionChanged;
            _CanSearch = true;
            DeleteOnCascade = true;
            CanAdd = true;
            CanSave = true;
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        /// <param name="canSearch">Enable the view model to execute query.</param>
        public CrudViewModel(ServiceLocator locator, bool canSearch)
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            Items.CollectionChanged += Items_CollectionChanged;
            DeleteOnCascade = true;
            CanAdd = true;
            CanSave = true;
            _CanSearch = canSearch;
            Initialize(locator);
        }
        #endregion
    }*/
}

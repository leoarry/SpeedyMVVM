﻿using System;
using SpeedyMVVM.Utilities;
using System.Collections.ObjectModel;
using SpeedyMVVM.DataAccess.Interfaces;
using System.Threading.Tasks;
using SpeedyMVVM.Navigation.Models;
using System.Linq.Expressions;
using System.Collections;
using System.Linq;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// Class which implement a basic CRUD operation with an EntityBase.
    /// </summary>
    /// <typeparam name="T">Type of Entity.</typeparam>
    public class CrudViewModel<T> : ViewModelBase where T : EntityBase
    {
        #region Fields
        private bool _CanSearch;
        private RelayCommand<T> _AddCommand;
        private RelayCommand<object> _AddSelectionCommand;
        private RelayCommand<T> _RemoveCommand;
        private RelayCommand<T> _PopOutCommand;
        private RelayCommand _FilterCommand;
        private RelayCommand _PickEntitiesCommand;
        private RelayCommand _SaveCommand;
        private IRepositoryService<T> _DataService;
        private IEntityDialogBoxService _DialogService;
        private EntityFilterViewModel<T> _Filter;
        private ObservableCollection<T> _SelectedItems;
        private T _SelectedItem;
        private ObservableCollection<ContextMenuItemModel> _ContextMenu;
        private Expression<Func<T, bool>> _PickEntitiesExpression;
        private ContextMenuItemModel _RemoveSelectedItemCommand;
        private ContextMenuItemModel _PopOutSelectedItemCommand;
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
        /// Set 'TRUE' if the filter can execute query using DataService.
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
                    if(_CanSearch)
                        Filter.DataService = value;
                }
            }
        }
        
        /// <summary>
        /// Service for UI dialogs.
        /// </summary>
        public IEntityDialogBoxService DialogService
        {
            get
            {
                return (_DialogService == null) ? _DialogService = ServiceContainer.GetService<IEntityDialogBoxService>() : _DialogService;
            }
            set
            {
                if (_DialogService != value)
                {
                    _DialogService = value;
                    OnPropertyChanged(nameof(DialogService));
                }
            }
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
                    RemoveCommand.IsEnabled = true;
                    PopOutCommand.IsEnabled = true;
                    RemoveSelectedItemCommand.Action.IsEnabled = true;
                    PopOutSelectedItemCommand.Action.IsEnabled = true;
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
                return (_AddCommand == null)?  
                    _AddCommand = new RelayCommand<T>(async(param)=> await AddCommandExecute(param), true) : _AddCommand;
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
                return (_AddSelectionCommand == null) ?
                    _AddSelectionCommand = new RelayCommand<object>(async(param)=>await AddSelectionCommandExecute(param), true)
                    : _AddSelectionCommand;
            }
        }

        /// <summary>
        /// Command to remove a <typeparamref name="T"/>.
        /// </summary>
        public RelayCommand<T> RemoveCommand
        {
            get
            {
                return (_RemoveCommand == null) ?
                    _RemoveCommand = new RelayCommand<T>(async (param) => await RemoveCommandExecute(param)) : _RemoveCommand;
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
                return (_PopOutCommand == null) ?
                    _PopOutCommand = new RelayCommand<T>(async (param) => await PopOutCommandExecute(param)) : _PopOutCommand;
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
                return (_FilterCommand == null) ? 
                    _FilterCommand = new RelayCommand(async()=> await FilterCommandExecute(), true) : _FilterCommand;
            }
            set { _FilterCommand = value; }
        }

        /// <summary>
        /// Command to pick entities from a repository service and add them to 'Items'.
        /// </summary>
        public RelayCommand PickEntitiesCommand
        {
            get
            {
                return (_PickEntitiesCommand == null) ?
                    _PickEntitiesCommand = new RelayCommand(async () => await PickEntitiesCommandExecute(PickEntitiesExpression), true) : _PickEntitiesCommand;
            }
            set { _PickEntitiesCommand = value; }
        }

        /// <summary>
        /// Command to save changes.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return (_SaveCommand == null) ?
                    _SaveCommand = new RelayCommand(async () => await SaveCommandExecute(), true) : _SaveCommand;
            }
            set { _SaveCommand = value; }
        }

        /// <summary>
        /// Command to remove the Selected Item from Items.
        /// </summary>
        public ContextMenuItemModel RemoveSelectedItemCommand
        {
            get
            {
                return (_RemoveSelectedItemCommand == null) ? 
                    _RemoveSelectedItemCommand = new ContextMenuItemModel(
                        new RelayCommand(async () => await RemoveCommandExecute(SelectedItem)), "Remove Selected Item") 
                    : _RemoveSelectedItemCommand;
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
                return (_PopOutSelectedItemCommand == null) ?
                    _PopOutSelectedItemCommand = new ContextMenuItemModel(
                        new RelayCommand(async () => await PopOutCommandExecute(SelectedItem)), "Edit Selected Item")
                    : _PopOutSelectedItemCommand;
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
            if(DataService!=null)
                entity = await DataService.AddEntityAsync(entity);
            Items.Add(entity);
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
                    SelectedItems = myItems.Cast<T>().AsObservableCollection();
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
                if (DataService != null)
                    await DataService.RemoveEntityAsync(entity);
                Items.Remove(entity);
                return true;
            }
            else if (_SelectedItems!=null && _SelectedItems.Count > 0)
            {
                foreach(var e in _SelectedItems)
                {
                    e.EntityStatus = EntityStatusEnum.Deleted;
                    if (DataService != null)
                        await DataService.RemoveEntityAsync(e);
                    Items.Remove(e);
                }
                SelectedItem = default(T);
                return true;
            }else if (SelectedItem != null)
            {
                SelectedItem.EntityStatus = EntityStatusEnum.Deleted;
                if (DataService != null)
                    await DataService.RemoveEntityAsync(SelectedItem);

                Items.Remove(SelectedItem);
                return true;
            }else
                return false;
        }

        /// <summary>
        /// Open in a new pop-up the current instance of CRUDViewModel (or the entity 'parameter' when not null) using 'DialogService' when available.
        /// </summary>
        /// <returns>Return TRUE in case operation successful</returns>
        public virtual async Task<bool> PopOutCommandExecute(T entity)
        {
            if (ServiceContainer != null && DialogService != null)
            {
                if (entity != null)
                {
                    var vm = new CrudDialogViewModel<T>(ServiceContainer, typeof(T).Name, "", entity);
                    return (bool)await DialogService.ShowEntityDialogBox(vm);
                }                  
                else
                {
                    var vm = new CrudDialogViewModel<T>(ServiceContainer, typeof(T).Name, "", this);
                    return (bool)await DialogService.ShowCRUDDialogBox(vm);
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
            if (CanSearch)
                return await Filter.FilterCommandExecute();
            else
                return false;
        }

        /// <summary>
        /// Show a Dialog Box to pick entities from a repository service and add them to Items.
        /// </summary>
        /// <returns>Return TRUE in case of operation successful</returns>
        public virtual async Task<bool> PickEntitiesCommandExecute(Expression<Func<T,bool>> expression)
        {
            if (ServiceContainer != null && DialogService != null)
            {
                var vm = new CrudDialogViewModel<T>(ServiceContainer, string.Format("{0} - Picker", typeof(T).Name), "");
                if(expression!=null)
                    vm.ViewModel.Filter.HiddenExpression = expression;
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
            int? result = null;
            if (DataService != null)
                result = await DataService.SaveChangesAsync();
            if (result == 0)
                return true;
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
            ServiceContainer = locator;
            if (locator != null)
                DataService = locator.GetService<IRepositoryService<T>>();
            _CanSearch = true;
            IsInitialized = true;
        }

        /// <summary>
        /// Initialize a new instance of ContextMenu containing Add, Remove, Edit and Picker entity commands.
        /// </summary>
        protected virtual void SetContextMenu()
        {
            if(_ContextMenu==null)
                _ContextMenu = new ObservableCollection<ContextMenuItemModel>();
            _ContextMenu.Add(new ContextMenuItemModel(new RelayCommand(async () => await AddCommandExecute(null), true), "Add new"));// string.Format("Add new {0}", typeof(T).Name)));
            _ContextMenu.Add(RemoveSelectedItemCommand);
            _ContextMenu.Add(PopOutSelectedItemCommand);
            _ContextMenu.Add(new ContextMenuItemModel(PickEntitiesCommand, "Search Items"));
        }

        /// <summary>
        /// Executed when a property of the item 'Filter' change the value.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Filter_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Items":
                    OnPropertyChanged(nameof(this.Items));
                    Filter.Items.CollectionChanged += (obj, args) => OnPropertyChanged(nameof(Items));
                    break;
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
        }
        
        /// <summary>
        /// Create a new instance of CRUDViewModel.
        /// </summary>
        /// <param name="canSearch">Enable the view model to execute query.</param>
        public CrudViewModel(bool canSearch)
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            _CanSearch = canSearch;
        }

        /// <summary>
        /// Create a new instance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public CrudViewModel(ServiceLocator locator)
        {
            Filter.PropertyChanged += Filter_PropertyChanged;
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        /// <param name="canSearch">Enable the view model to execute query.</param>
        public CrudViewModel(ServiceLocator locator, bool canSearch)
        {
            Initialize(locator);
            Filter.PropertyChanged += Filter_PropertyChanged;
            _CanSearch = canSearch;
        }
        #endregion
    }
}

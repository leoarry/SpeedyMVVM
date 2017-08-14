using System;
using System.Collections.Generic;
using SpeedyMVVM.Utilities;
using System.Collections.ObjectModel;
using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Expressions;
using SpeedyMVVM.Utilities.Enumerators;
using SpeedyMVVM.Utilities.Interfaces;

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
        private EntityFilterViewModel<T> _Filter;
        private T _SelectedItem;
        #endregion

        #region Properties
        /// <summary>
        /// Filter the collection of CRUDViewModel adding query support.
        /// </summary>
        public EntityFilterViewModel<T> Filter
        {
            get { return _Filter; }
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
        public virtual IRepositoryService<T> DataService { get; set; }

        /// <summary>
        /// Service for user dialogs interactions.
        /// </summary>
        public virtual IDialogBoxService DialogService { get; set; }

        /// <summary>
        /// Collection of <typeparamref name="T"/>.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return Filter.Items; }
            set { Filter.Items = value; }
        }

        /// <summary>
        /// Selected <typeparamref name="T"/> from 'Items' collection.
        /// </summary>
        public T SelectedItem
        {
            get { return _SelectedItem; }
            set
            {
                if (!_SelectedItem.Equals(value))
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
                return (_AddCommand == null)?  _AddCommand = new RelayCommand<T>(AddCommandExecute, true) : _AddCommand;
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
                return (_RemoveCommand == null)? _RemoveCommand :  _RemoveCommand = new RelayCommand(RemoveCommandExecute, true);
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
                return (_SaveCommand == null) ? _SaveCommand = new RelayCommand(SaveCommandExecute, true) : _SaveCommand;
            }
            set { _SaveCommand = value; }
        }

        /// <summary>
        /// Command to search <typeparamref name="T"/> into 'Items' using 'Filter'
        /// </summary>
        public RelayCommand SearchCommand { get { return Filter.SearchCommand; } }
        #endregion

        #region Commands Executions
        /// <summary>
        /// Add new entity to 'Filter.Items' and through 'DataService' when available.
        /// </summary>
        /// <param name="entity">Entity to add (if null will automatically create a new istance)</param>
        public virtual async void AddCommandExecute(T entity)
        {
            if (entity == null) { entity = Activator.CreateInstance<T>(); }
            entity.EntityStatus = EntityStatusEnum.Added;
            if(DataService!=null)
                entity = await DataService.AddEntityAsync(entity);
            Items.Add(entity);
            if (DialogService != null)
                DialogService.ShowMessageBox(string.Format("Entity ID: {0} added!", entity.ID), "Remove Command", DialogBoxEnum.Ok, DialogBoxIconEnum.Info);
        }

        /// <summary>
        /// Remove 'Filter.Selection' entity from 'Filter.Items' and through 'DataService' when available.
        /// </summary>
        public virtual async void RemoveCommandExecute()
        {
            if (SelectedItem == null) { return; }
            var entity = SelectedItem;
            entity.EntityStatus = EntityStatusEnum.Deleted;
            if (DataService!=null)
                entity = await DataService.RemoveEntityAsync(SelectedItem);
            if (entity != null) { Items.Remove(entity); }
            if (DialogService != null)
                DialogService.ShowMessageBox(string.Format("Entity ID: {0} removed!", entity.ID), "Remove Command", DialogBoxEnum.Ok, DialogBoxIconEnum.Info);
        }

        /// <summary>
        /// Persist the collection 'Filter.Items' through 'DataService' when available.
        /// </summary>
        public virtual void SaveCommandExecute()
        {
            if(DataService!=null)
                DataService.SaveChangesAsync();
            if (DialogService != null)
                DialogService.ShowMessageBox("Data Save Successfully!", "Save Command", DialogBoxEnum.Ok, DialogBoxIconEnum.Info);
        }
        
        #endregion

        #region Methods
        /// <summary>
        /// Initialize CRUDViewModel getting 'DataService' and 'DialogService' from parameter 'service' and initializing 'Filter'.
        /// </summary>
        /// <param name="locator">Service Locator with the current services</param>
        public override void Initialize(ServiceLocator locator)
        {
            ServiceContainer = locator;
            _CanSearch = true;
            DataService = ServiceContainer.GetService<IRepositoryService<T>>();
            DialogService = ServiceContainer.GetService<IDialogBoxService>();
            _Filter = new EntityFilterViewModel<T>(locator);
            IsInitialized = true;
        }
        #endregion

        #region Costructors
        /// <summary>
        /// Create a new istance of CRUDViewModel.
        /// </summary>
        public CRUDViewModel()
        {
            _Filter = new EntityFilterViewModel<T>();
        }

        /// <summary>
        /// Create a new istance of CRUDViewModel and initialize services from parameter 'service'.
        /// </summary>
        /// <param name="locator"Service Locator with the current services></param>
        public CRUDViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }
        #endregion
    }
}

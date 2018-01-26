using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using SpeedyMVVM.Validation;
using System.Linq.Expressions;
using System.Collections.Specialized;
using System.ComponentModel;
using SpeedyMVVM.Navigation;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View model offering the basics functions for CRUD data access using repository services and validation model.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CrudViewModel<T> : ViewModelBase, ICrudViewModel<T> where T:EntityBase
    {
        #region Fields
        private IDialogBoxService _DialogService;
        private List<RepositoryBehaviourEnum> _RepositoryBehaviours;
        private T _SelectedItem;
        private ObservableCollection<T> _SelectedItems;
        private ObservableCollection<T> _Items;
        private RelayCommand<T> _AddCommand;
        private RelayCommand<T> _RemoveCommand;
        private RelayCommand<T> _SaveCommand;
        private RelayCommand<Expression<Func<T,bool>>> _SearchCommand;
        private EntityFilterViewModel<T> _Filter;
        private IValidator<T> _Validator;
        #endregion

        #region Services
        /// <summary>
        /// GET/SET Filter.DataService. In case RepositoryBehaviours is null, add all CRUD behaviours.
        /// </summary>
        public IRepositoryService<T> DataService
        {
            get
            {
                if (Filter.DataService == null && ServiceContainer != null)
                {
                    Filter.DataService = GetService<IRepositoryService<T>>();
                    if (_RepositoryBehaviours == null)
                        _RepositoryBehaviours = new List<RepositoryBehaviourEnum> { RepositoryBehaviourEnum.Create,
                                                                            RepositoryBehaviourEnum.Delete,
                                                                            RepositoryBehaviourEnum.Read,
                                                                            RepositoryBehaviourEnum.Update };
                }
                return Filter.DataService;
            }
            set
            {
                Filter.DataService = value;
                if(_RepositoryBehaviours==null)
                    _RepositoryBehaviours = new List<RepositoryBehaviourEnum> { RepositoryBehaviourEnum.Create,
                                                                            RepositoryBehaviourEnum.Delete,
                                                                            RepositoryBehaviourEnum.Read,
                                                                            RepositoryBehaviourEnum.Update };
            }
        }

        /// <summary>
        /// User dialog service.
        /// </summary>
        public IDialogBoxService DialogService
        {
            get { return _DialogService ?? (_DialogService = GetService<IDialogBoxService>()); }
            set { _DialogService = value; }
        }
        #endregion

        #region Validation Properties
        /// <summary>
        /// Return true when there are not errors.
        /// </summary>
        public bool IsValid { get { return !HasErrors; } }

        /// <summary>
        /// Get if there are errors on Items or SelectedItem.
        /// </summary>
        public bool HasErrors { get { return Items.Any(i => i.HasErrors) || (SelectedItem != null) ? SelectedItem.HasErrors : false; } }

        /// <summary>
        /// Validator
        /// </summary>
        public IValidator<T> Validator
        {
            get { return _Validator; }
            set { _Validator = value; }
        }
        #endregion

        #region Properties      
        /// <summary>
        /// Return true when RepositoryBehaviours contains RepositoryBehaviourEnum.Read.
        /// </summary>
        public bool CanSearch { get { return _RepositoryBehaviours.Contains(RepositoryBehaviourEnum.Read); } }

        /// <summary>
        /// Behaviours for the repository service.
        /// </summary>
        public List<RepositoryBehaviourEnum> RepositoryBehaviours
        {
            get { return _RepositoryBehaviours ?? (_RepositoryBehaviours = new List<RepositoryBehaviourEnum>()); }
            set { _RepositoryBehaviours = value.Distinct().ToList(); }
        }

        /// <summary>
        /// Filter view model to filter the collection.
        /// </summary>
        public EntityFilterViewModel<T> Filter
        {
            get { return _Filter ?? (_Filter=new EntityFilterViewModel<T>()); }
            set
            {
                _Filter = value;
                if (ServiceContainer != null && _Filter != null)
                    _Filter.InjectServices(ServiceContainer);
            }
        }

        /// <summary>
        /// Current item selected from the collection.
        /// </summary>
        public T SelectedItem
        {
            get { return _SelectedItem; }
            set { SetProperty(ref _SelectedItem, value, nameof(SelectedItem)); }
        }

        /// <summary>
        /// Current items selected from the collection.
        /// </summary>
        public ObservableCollection<T> SelectedItems
        {
            get { return _SelectedItems; }
            set { SetProperty(ref _SelectedItems, value, nameof(SelectedItems)); }
        }

        /// <summary>
        /// Collection of items.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return _Items ?? (_Items = new ObservableCollection<T>()); }
            set { SetProperty(ref _Items, value, nameof(Items)); }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to execute the method AddCommandExecute.
        /// </summary>
        public RelayCommand<T> AddCommand
        {
            get { return _AddCommand ?? (_AddCommand = new RelayCommand<T>(AddCommandExecute, true)); }
        }

        /// <summary>
        /// Command to execute the method RemoveCommandExecute.
        /// </summary>
        public RelayCommand<T> RemoveCommand
        {
            get { return _RemoveCommand ?? (_RemoveCommand = new RelayCommand<T>(RemoveCommandExecute)); }
        }

        /// <summary>
        /// Command to execute the method SaveCommandExecute.
        /// </summary>
        public RelayCommand<T> SaveCommand
        {
            get { return _SaveCommand ?? (_SaveCommand = new RelayCommand<T>(SaveCommandExecute, true)); }
        }

        /// <summary>
        /// Command to execute the method SearchCommandExecute.
        /// </summary>
        public RelayCommand<Expression<Func<T, bool>>> SearchCommand
        {
            get { return _SearchCommand ?? (_SearchCommand = new RelayCommand<Expression<Func<T, bool>>>(SearchCommandExecute, true)); }
        }
        #endregion

        #region Commands Executions
        /// <summary>
        /// add a new entity (or the entity passed as parameter) to the collection.
        /// </summary>
        /// <param name="entity">Entity to add.</param>
        public void AddCommandExecute(T entity)
        {
            if (entity == null) { entity = Activator.CreateInstance<T>(); }
            entity.EntityStatus = EntityStatusEnum.Added;
            Items.Add(entity);
            if (DataService != null && _RepositoryBehaviours.Contains(RepositoryBehaviourEnum.Create))
                entity = DataService.Add(entity);
        }

        /// <summary>
        /// Remove the entity passed as parameter or all the SelectedItems if not null or the SelectedItem if not null from the collection and data service when available.
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public void RemoveCommandExecute(T entity)
        {
            bool deleteOnCascade = _RepositoryBehaviours.Contains(RepositoryBehaviourEnum.Delete);
            if (entity != null)
            {
                entity.EntityStatus = EntityStatusEnum.Deleted;
                if (deleteOnCascade && DataService != null)
                    DataService.Remove(entity);
                Items.Remove(entity);
            }
            else if (_SelectedItems != null && _SelectedItems.Count > 0)
            {
                foreach (var e in _SelectedItems)
                {
                    e.EntityStatus = EntityStatusEnum.Deleted;
                    if (deleteOnCascade && DataService != null)
                        DataService.Remove(e);
                    Items.Remove(e);
                }
                SelectedItem = default(T);
            }
            else if (SelectedItem != null)
            {
                SelectedItem.EntityStatus = EntityStatusEnum.Deleted;
                if (deleteOnCascade && DataService != null)
                    DataService.Remove(SelectedItem);
                Items.Remove(SelectedItem);
            }
            else
                return;
        }

        /// <summary>
        /// Persist datas throw repository service.
        /// </summary>
        /// <param name="entity">Entity to pass to the data service.</param>
        public void SaveCommandExecute(T entity)
        {
            if (DataService != null && IsValid && _RepositoryBehaviours.Contains(RepositoryBehaviourEnum.Update))
            {
                if (DataService.Save(entity) == 0)
                    foreach (var i in Items)
                        i.EntityStatus = EntityStatusEnum.Unchanged;
            }
        }

        /// <summary>
        /// Filter the Items collection using dataservice when available.
        /// </summary>
        /// <param name="expression">Expression to query</param>
        public async void SearchCommandExecute(Expression<Func<T, bool>> expression)
        {
            if (!_RepositoryBehaviours.Contains(RepositoryBehaviourEnum.Read))
                return;
            else if (expression != null && DataService != null)
                Items = await DataService.RetrieveCollection(expression);
            else if(DataService != null)
                Items = await _Filter.FilterFromDataService();
            else
                Items = await _Filter.FilterFromCollection(Items);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute when the SelectedItem property change.
        /// </summary>
        protected virtual Task OnSelectedItemChanged()
        {
            return Task.Factory.StartNew(async() =>
            {
                bool isEnabled = _SelectedItem != null || (_SelectedItems != null && _SelectedItems.Count > 0);
                RemoveCommand.IsEnabled = isEnabled;

                if (_SelectedItem != null)
                {
                    if (_SelectedItem.Validator == null && _Validator != null)
                        _SelectedItem.Validator = _Validator;
                    else if (_SelectedItem.Validator != null &&
                             _SelectedItem.Validator.GetType() == typeof(IValidator<T>) &&
                             _Validator != null)
                        ((IValidator<T>)_SelectedItem.Validator).MergeRules(_Validator.RulesCollection);
                    await _SelectedItem.ValidateAsync();
                }
            });
        }

        /// <summary>
        /// Execute on property changed.
        /// </summary>
        /// <param name="propertyName"></param>
        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (propertyName == nameof(SelectedItem))
                OnSelectedItemChanged();
            base.OnPropertyChanged(propertyName);
        }

        public override void InjectServices(ServiceLocator locator)
        {
            base.InjectServices(locator);
            DataService = GetService<IRepositoryService<T>>();
            DialogService = GetService<IDialogBoxService>();
        }
        #endregion

        #region Constructors
        public CrudViewModel()
        {
        }

        public CrudViewModel(ObservableCollection<T> items)
        {
            Items = items;
        }

        public CrudViewModel(ServiceLocator locator, params RepositoryBehaviourEnum[] repositoryBehaviours)
        {
            InjectServices(locator);
            if (repositoryBehaviours != null && repositoryBehaviours.Count() > 0)
                _RepositoryBehaviours = repositoryBehaviours.Distinct().ToList();
            else if (repositoryBehaviours == null)
                _RepositoryBehaviours = new List<RepositoryBehaviourEnum> { RepositoryBehaviourEnum.Create,
                                                                            RepositoryBehaviourEnum.Delete,
                                                                            RepositoryBehaviourEnum.Read,
                                                                            RepositoryBehaviourEnum.Update };
        }

        public CrudViewModel(IRepositoryService<T> repositoryService, params RepositoryBehaviourEnum[] repositoryBehaviours)
        {
            DataService = repositoryService;
            if (repositoryBehaviours != null && repositoryBehaviours.Count() > 0)
                _RepositoryBehaviours = repositoryBehaviours.Distinct().ToList();
            else if(repositoryBehaviours == null)
                _RepositoryBehaviours = new List<RepositoryBehaviourEnum> { RepositoryBehaviourEnum.Create,
                                                                            RepositoryBehaviourEnum.Delete,
                                                                            RepositoryBehaviourEnum.Read,
                                                                            RepositoryBehaviourEnum.Update };
        }

        public CrudViewModel(IRepositoryService<T> repositoryService, IDialogBoxService dialogService, params RepositoryBehaviourEnum[] repositoryBehaviours)
        {
            DataService = repositoryService;
            DialogService = dialogService;
            if (repositoryBehaviours != null && repositoryBehaviours.Count() > 0)
                _RepositoryBehaviours = repositoryBehaviours.Distinct().ToList();
            else if (repositoryBehaviours == null)
                _RepositoryBehaviours = new List<RepositoryBehaviourEnum> { RepositoryBehaviourEnum.Create,
                                                                            RepositoryBehaviourEnum.Delete,
                                                                            RepositoryBehaviourEnum.Read,
                                                                            RepositoryBehaviourEnum.Update };
        }
        #endregion
    }
}

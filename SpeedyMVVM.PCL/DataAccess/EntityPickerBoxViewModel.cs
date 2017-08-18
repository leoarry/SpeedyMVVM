using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View model to select entities from a list filtered using repository services.
    /// </summary>
    /// <typeparam name="T">Type of entity</typeparam>
    public class EntityPickerBoxViewModel<T> : ViewModelBase, IDialogBox where T : IEntityBase
    {

        #region IDialogBox Implementation
        /// <summary>
        /// Icon to show on the dialog box.
        /// </summary>
        public string IconPath
        {
            get { return _IconPath; }
            set
            {
                if (_IconPath != value)
                {
                    _IconPath = value;
                    OnPropertyChanged(nameof(IconPath));
                }
            }
        }

        /// <summary>
        /// Title of the dialog box.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title != value)
                {
                    _Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        /// <summary>
        /// TRUE when the dialog box must be visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    _IsVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }

        /// <summary>
        /// Result of the dialog (fire DialogResultChanged event on value changed).
        /// </summary>
        public bool? DialogResult
        {
            get { return _DialogResult; }
            set
            {
                if (_DialogResult != value)
                {
                    _DialogResult = value;
                    OnPropertyChanged(nameof(DialogResult));
                    DialogResultChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Raised when 'DialogResult' changed.
        /// </summary>
        public event EventHandler<bool?> DialogResultChanged;
        #endregion

        #region Fields
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private bool _CanSearch=true;
        private EntityFilterViewModel<T> _Filter;
        private ObservableCollection<T> _SelectedItems;
        private RelayCommand<object> _AddSelectionCommand;
        #endregion

        #region Property
        /// <summary>
        /// Set 'TRUE' if the filter can execute query.
        /// </summary>
        public bool CanSearch
        {
            get { return _CanSearch; }
            set
            {
                if (value != _CanSearch)
                {
                    _CanSearch = value;
                    SearchCommand.IsEnabled = value;
                    OnPropertyChanged(nameof(CanSearch));
                }
            }
        }

        /// <summary>
        /// Filter the 'Items' collection adding query support.
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
        /// Collection of entities.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return Filter.Items; }
            set
            {
                Filter.Items = value;
                OnPropertyChanged(nameof(Filter));
            }
        }

        /// <summary>
        /// Selected entities from 'Items'.
        /// </summary>
        public ObservableCollection<T> SelectedItems
        {
            get { return _SelectedItems; }
            set
            {
                if (value != _SelectedItems)
                {
                    _SelectedItems = value;
                    OnPropertyChanged(nameof(SelectedItems));
                }
            }
        }

        /// <summary>
        /// Repository service where get datas from.
        /// </summary>
        public IRepositoryService<T> DataService
        {
            get { return Filter.DataService; }
            set { Filter.DataService = value; }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to populate 'SelectedItems' form parameter (IList).
        /// </summary>
        public RelayCommand<object> AddSelectionCommand
        {
            get
            {
                if (_AddSelectionCommand == null)
                {
                    _AddSelectionCommand = new RelayCommand<object>(AddSelectionExecute, true);
                }
                return _AddSelectionCommand;
            }
        }

        /// <summary>
        /// Command to set TRUE 'DialogResult'.
        /// </summary>
        public RelayCommand ConfirmCommand { get { return new RelayCommand(() => DialogResult = true, true); } }

        /// <summary>
        /// Command to set FALSE 'DialogResult'.
        /// </summary>
        public RelayCommand DeclineCommand { get { return new RelayCommand(() => DialogResult = false, true); } }

        /// <summary>
        /// Command to execute the query.
        /// </summary>
        public RelayCommand SearchCommand
        {
            get { return Filter.SearchCommand; }
        }
        #endregion

        #region Commands Execution
        /// <summary>
        /// Populate 'SelectedItems' form parameter (IList).
        /// </summary>
        /// <param name="parameter"></param>
        public void AddSelectionExecute(object parameter)
        {
            try
            {
                IList myItems = (IList)parameter;
                if (myItems.OfType<T>().Count() > 0)
                {
                    SelectedItems = new ObservableCollection<T>(myItems.Cast<T>());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize EntityPickerBoxViewModel using locator for services.
        /// </summary>
        /// <param name="locator">Container with services.</param>
        public override void Initialize(ServiceLocator locator)
        {
            ServiceContainer = locator;
            _Filter = new EntityFilterViewModel<T>(locator);
            IsInitialized = true;
        }
        #endregion

        #region Costructors
        /// <summary>
        /// Create a new instance of EntityPickerBoxViewModel.
        /// </summary>
        public EntityPickerBoxViewModel()
        {
            Filter.Items.CollectionChanged += (obj, arg) => OnPropertyChanged(nameof(EntityPickerBoxViewModel<T>.Items));
        }

        /// <summary>
        /// Create a new instance of EntityPickerBoxViewModel and Initialize it using locator for services.
        /// </summary>
        /// <param name="locator">Container with services.</param>
        public EntityPickerBoxViewModel(ServiceLocator locator)
        {
            Filter.Items.CollectionChanged += (obj, arg) => OnPropertyChanged(nameof(EntityPickerBoxViewModel<T>.Items));
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityPickerBoxViewModel and Initialize it using locator for services.
        /// </summary>
        /// <param name="locator">Container with services.</param>
        /// <param name="canSearch">Enable the search comand.</param>
        public EntityPickerBoxViewModel(ServiceLocator locator, bool canSearch)
        {
            Filter.Items.CollectionChanged += (obj, arg) => OnPropertyChanged(nameof(EntityPickerBoxViewModel<T>.Items));
            CanSearch = canSearch;
            Initialize(locator);
        }
        #endregion
    }
}

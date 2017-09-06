using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View model to pop-up a CRUDViewModel or an entity in a new dialog box.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public class CrudDialogViewModel<T> : ViewModelBase, IDialogBox where T : EntityBase
    {

        #region Fields
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private CrudViewModel<T> _ViewModel;
        #endregion

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
                }
                DialogResultChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Raised when 'DialogResult' changed.
        /// </summary>
        public event EventHandler<bool?> DialogResultChanged;
        #endregion
        
        #region Properties
        /// <summary>
        /// CRUD View Model to expose in the pop-up.
        /// </summary>
        public CrudViewModel<T> ViewModel
        {
            get
            {
                return (_ViewModel == null) ? _ViewModel = new CrudViewModel<T>() : _ViewModel;
            }
            set
            {
                if (_ViewModel != value)
                {
                    _ViewModel = value;
                    OnPropertyChanged(nameof(ViewModel));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to persist changes using 'DataService'.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get { return new RelayCommand(async () => DialogResult = await ViewModel.SaveCommandExecute(), true); }
        }

        /// <summary>
        /// Command to populate 'SelectedItems' form parameter (IList).
        /// </summary>
        public RelayCommand<object> AddSelectionCommand
        {
            get { return new RelayCommand<object>(async (param) => DialogResult = await ViewModel.AddSelectionCommandExecute(param), true); }
        }

        /// <summary>
        /// Command to execute the query.
        /// </summary>
        public RelayCommand SearchCommand
        {
            get { return ViewModel.FilterCommand; }
        }

        /// <summary>
        /// Confirmation command (DialogResult = true).
        /// </summary>
        public RelayCommand ConfirmCommand { get { return new RelayCommand(() => DialogResult = true, true); } }

        /// <summary>
        /// Declination command (DialogResult = false).
        /// </summary>
        public RelayCommand DeclineCommand { get { return new RelayCommand(DeclineCommandExecute, true); } }
        #endregion

        #region Methods
        /// <summary>
        /// Void executed by Decline Command (DialogResult = false).
        /// </summary>
        protected virtual void DeclineCommandExecute() { DialogResult = false; }

        /// <summary>
        /// Void executed by Confirm Command (DialogResult = true).
        /// </summary>
        protected virtual void ConfirmCommandExecute() { DialogResult = true; }

        /// <summary>
        /// Initialize EntityEditor using 'locator'.
        /// </summary>
        /// <param name="locator"></param>
        public override void Initialize(ServiceLocator locator)
        {
            this.ServiceContainer = locator;
            if (!_ViewModel.IsInitialized)
                _ViewModel.Initialize(locator);
            IsInitialized = true;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of EntityEditor.
        /// </summary>
        public CrudDialogViewModel()
        {
            _ViewModel = new CrudViewModel<T>();
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services</param>
        public CrudDialogViewModel(ServiceLocator locator)
        {
            _ViewModel = new CrudViewModel<T>();
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        public CrudDialogViewModel(ServiceLocator locator, string title, string iconPath)
        {
            _Title = title;
            _IconPath = iconPath;
            _ViewModel = new CrudViewModel<T>();
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        /// <param name="selection">Entity to pop-up.</param>
        public CrudDialogViewModel(ServiceLocator locator, string title, string iconPath, T selection)
        {
            _Title = title;
            _IconPath = iconPath;
            _ViewModel = new CrudViewModel<T>();
            _ViewModel.SelectedItem = selection;
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        /// <param name="viewModel">CRUD View Model to pop-up.</param>
        public CrudDialogViewModel(ServiceLocator locator, string title, string iconPath, CrudViewModel<T> viewModel)
        {
            _Title = title;
            _IconPath = iconPath;
            _ViewModel = viewModel;
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        /// <param name="viewModel">CRUD View Model to pop-up.</param>
        public CrudDialogViewModel(string title, string iconPath, CrudViewModel<T> viewModel)
        {
            _Title = title;
            _IconPath = iconPath;
            _ViewModel = viewModel;
        }
        #endregion
    }

}

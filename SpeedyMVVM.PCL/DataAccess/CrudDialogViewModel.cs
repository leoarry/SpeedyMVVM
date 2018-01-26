using SpeedyMVVM.Navigation;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View model to pop-up a CRUDViewModel in a new dialog box.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public class CrudDialogViewModel<T> : ViewModelBase, ICrudViewModel<T>, IDialogBox where T : EntityBase
    {

        #region Fields
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private RelayCommand _ConfirmCommand;
        private RelayCommand _DeclineCommand;
        private ICrudViewModel<T> _viewModel;
        #endregion

        #region IDialogBox Implementation
        /// <summary>
        /// Icon to show on the dialog box.
        /// </summary>
        public string IconPath
        {
            get { return _IconPath; }
            set { SetProperty(ref _IconPath, value); }
        }

        /// <summary>
        /// Title of the dialog box.
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }

        /// <summary>
        /// TRUE when the dialog box must be visible.
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set { SetProperty(ref _IsVisible, value); }
        }

        /// <summary>
        /// Result of the dialog (fire DialogResultChanged event on value changed).
        /// </summary>
        public bool? DialogResult
        {
            get { return _DialogResult; }
            set
            {
                SetProperty(ref _DialogResult, value);
                WeakEventManager.Default.RaiseEvent(this, value, nameof(DialogResultChanged));
            }
        }

        /// <summary>
        /// Raised when 'DialogResult' changed.
        /// </summary>
        public event EventHandler<bool?> DialogResultChanged
        {
            add
            {
                WeakEventManager.Default.AddEventHandler(this, nameof(DialogResultChanged), value);
            }
            remove
            {
                WeakEventManager.Default.RemoveEventHandler(this, nameof(DialogResultChanged), value);
            }
        }
        #endregion

        #region ICrudViewModel Implementation
        public IRepositoryService<T> DataService
        {
            get
            {
                return _viewModel.DataService;
            }

            set
            {
                _viewModel.DataService = value;
            }
        }

        public RelayCommand<T> AddCommand
        {
            get
            {
                return _viewModel.AddCommand;
            }
        }

        public RelayCommand<T> RemoveCommand
        {
            get
            {
                return _viewModel.RemoveCommand;
            }
        }

        public RelayCommand<T> SaveCommand
        {
            get
            {
                return _viewModel.SaveCommand;
            }
        }

        public RelayCommand<Expression<Func<T, bool>>> SearchCommand
        {
            get
            {
                return _viewModel.SearchCommand;
            }
        }

        public T SelectedItem
        {
            get
            {
                return _viewModel.SelectedItem;
            }

            set
            {
                _viewModel.SelectedItem = value;
            }
        }

        public ObservableCollection<T> Items
        {
            get
            {
                return _viewModel.Items;
            }

            set
            {
                _viewModel.Items = value;
            }
        }

        public ObservableCollection<T> SelectedItems
        {
            get
            {
                return _viewModel.SelectedItems;
            }

            set
            {
                _viewModel.SelectedItems = value;
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Confirmation command (DialogResult = true).
        /// </summary>
        public RelayCommand ConfirmCommand
        {
            get { return _ConfirmCommand ?? (_ConfirmCommand = new RelayCommand(() => DialogResult = true, true)); }
        }

        /// <summary>
        /// Declination command (DialogResult = false).
        /// </summary>
        public RelayCommand DeclineCommand
        {
            get { return _DeclineCommand ?? (_DeclineCommand = new RelayCommand(() => DialogResult = false, true)); }
        }        
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of EntityEditor.
        /// </summary>
        public CrudDialogViewModel(ICrudViewModel<T> viewModel = null)
        {
            if (viewModel == null)
                _viewModel = new CrudViewModel<T>();
            else
                _viewModel = viewModel;
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        public CrudDialogViewModel(string title, string iconPath, ICrudViewModel<T> viewModel = null)
        {
            _Title = title;
            _IconPath = iconPath;
            if (viewModel == null)
                _viewModel = new CrudViewModel<T>();
            else
                _viewModel = viewModel;
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services</param>
        public CrudDialogViewModel(ServiceLocator locator, ICrudViewModel<T> viewModel = null)
        {
            InjectServices(locator);
            if (viewModel == null)
                _viewModel = new CrudViewModel<T>();
            else
                _viewModel = viewModel;
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        public CrudDialogViewModel(ServiceLocator locator, string title, string iconPath, ICrudViewModel<T> viewModel = null)
        {
            _Title = title;
            _IconPath = iconPath;
            InjectServices(locator);
            if (viewModel == null)
                _viewModel = new CrudViewModel<T>();
            else
                _viewModel = viewModel;
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        /// <param name="items">Entities collection to pop-up.</param>
        public CrudDialogViewModel(ServiceLocator locator, string title, string iconPath, IEnumerable<T> items, ICrudViewModel<T> viewModel = null)
        {
            _Title = title;
            _IconPath = iconPath;
            Items = items.ToObservableCollection();
            InjectServices(locator);
            if (viewModel == null)
                _viewModel = new CrudViewModel<T>();
            else
                _viewModel = viewModel;
        }

        ~CrudDialogViewModel()
        {
            WeakEventManager.Default.RemoveSource(this);
        }
        #endregion

    }

}

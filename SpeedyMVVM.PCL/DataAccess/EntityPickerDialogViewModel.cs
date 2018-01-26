using SpeedyMVVM.Navigation;
using SpeedyMVVM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// ViewModel to pick entities from a RepositoryService.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityPickerDialogViewModel<T>:EntityFilterViewModel<T>, IDialogBox where T:EntityBase
    {
        #region Field
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private List<T> _SelectedItems;
        private RelayCommand<object> _AddSelectionCommand;
        private RelayCommand _ConfirmCommand;
        private RelayCommand _DeclineCommand;
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

        #region Properties
        /// <summary>
        /// Items currently selected from the user.
        /// </summary>
        public List<T> SelectedItems
        {
            get { return _SelectedItems ?? (_SelectedItems = new List<T>()); }
            set { _SelectedItems = value; }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Confirmation command (DialogResult = true).
        /// </summary>
        public RelayCommand ConfirmCommand { get { return _ConfirmCommand ?? (_ConfirmCommand = new RelayCommand(() => DialogResult = true, true)); } }

        /// <summary>
        /// Declination command (DialogResult = false).
        /// </summary>
        public RelayCommand DeclineCommand { get { return _DeclineCommand ?? (_DeclineCommand = new RelayCommand(() => DialogResult = false, true)); } }

        /// <summary>
        /// Add the selected items passed as parameter to the list SelectedItems.
        /// </summary>
        public RelayCommand<object> AddSelectionCommand
        {
            get
            {
                return _AddSelectionCommand ?? (_AddSelectionCommand = new RelayCommand<object>((obj) => 
                {
                    IList myItems = (IList)obj;
                    if (myItems.OfType<T>().Count() > 0)
                        SelectedItems = myItems.Cast<T>().ToList();
                }, true));
            }
        }
        #endregion


        #region Methods

        #endregion

        #region Constructors
        public EntityPickerDialogViewModel():base()
        { }

        public EntityPickerDialogViewModel(ServiceLocator locator, string title = "", string iconPath = ""):base(locator)
        {
            InjectServices(locator);
            _Title = title;
            _IconPath = iconPath;
        }

        public EntityPickerDialogViewModel(IRepositoryService<T> dataService, string title = "", string iconPath = "") : base(dataService)
        {
            DataService = dataService;
            _Title = title;
            _IconPath = iconPath;
        }

        ~EntityPickerDialogViewModel()
        {
            WeakEventManager.Default.RemoveSource(this);
        }
        #endregion
    }
}

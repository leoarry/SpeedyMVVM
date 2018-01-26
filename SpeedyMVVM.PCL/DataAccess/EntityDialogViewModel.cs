using SpeedyMVVM.Navigation;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View Model representing a dialog to process a single entity. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityDialogViewModel<T>:ViewModelBase, IDialogBox where T:EntityBase
    {
        #region Field
        private IRepositoryService<T> _DataService;
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private T _SelectedItem;
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

        public IRepositoryService<T> DataService
        {
            get { return _DataService; }
            set { _DataService = value; }
        }

        public T SelectedItem
        {
            get { return _SelectedItem ?? (_SelectedItem = Activator.CreateInstance<T>()); }
            set { SetProperty(ref _SelectedItem, value); }
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

        #region Methods
        public override void InjectServices(ServiceLocator locator)
        {
            DataService = locator.GetService<IRepositoryService<T>>();
            base.InjectServices(locator);
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of EntityDialogViewModel.
        /// </summary>
        public EntityDialogViewModel()
        {
        }

        /// <summary>
        /// Create a new instance of EntityDialogViewModel.
        /// </summary>
        /// <param name="locator">Container of services.</param>
        /// <param name="item">Item to manage. If NULL and confirmAction IS NULL will create a new instance of T and add it throw dataservice if IsValid.</param>
        /// <param name="confirmAction">Action to perform before return DialogResult = true. If null and item NOT NULL, the default action will be 'DataService.Save(SelectedItem)'.</param>
        public EntityDialogViewModel(ServiceLocator locator, T item = null, Action confirmAction = null)
        {
            InjectServices(locator);
            if (item == null && confirmAction == null)
                ConfirmCommand.CombineAction(async() => 
                {
                    await _SelectedItem.ValidateAsync();
                    if (SelectedItem.IsValid)
                    {
                        DataService.Add(SelectedItem);
                        DataService.Save();
                    }
                }, true);
            else if (item != null && confirmAction == null)
            {
                _SelectedItem = item;
                ConfirmCommand.CombineAction(() => 
                {
                    if (SelectedItem.IsValid)
                        DataService.Save(SelectedItem);
                }, true);
            }
            else
            {
                _SelectedItem = item;
                ConfirmCommand.CombineAction(confirmAction, true);
            }            
        }

        /// <summary>
        /// Create a new instance of EntityDialogViewModel.
        /// </summary>
        /// <param name="dataService">Repository service where get datas from.</param>
        /// <param name="item">Item to manage. If NULL and confirmAction IS NULL will create a new instance of T and add it throw dataservice if Item.IsValid.</param>
        /// <param name="confirmAction">Action to perform before return DialogResult = true. If null and item NOT NULL, the default action will be 'DataService.Save(SelectedItem)'.</param>
        public EntityDialogViewModel(IRepositoryService<T> dataService, T item = null, Action confirmAction = null)
        {
            DataService = dataService;
            if (item == null && confirmAction == null)
                ConfirmCommand.CombineAction(() =>
                {
                    if (SelectedItem.IsValid)
                    {
                        DataService.Add(SelectedItem);
                        DataService.Save();
                    }
                }, true);
            else if (item != null && confirmAction == null)
            {
                _SelectedItem = item;
                ConfirmCommand.CombineAction(() =>
                {
                    if (SelectedItem.IsValid)
                        DataService.Save(SelectedItem);
                }, true);
            }
            else
            {
                _SelectedItem = item;
                ConfirmCommand.CombineAction(confirmAction, true);
            }
        }

        ~EntityDialogViewModel()
        {
            WeakEventManager.Default.RemoveSource(this);
        }
        #endregion
    }
}

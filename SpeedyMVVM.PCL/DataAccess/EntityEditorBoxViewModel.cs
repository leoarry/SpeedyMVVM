using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Threading.Tasks;

namespace SpeedyMVVM.DataAccess
{
    /// <summary>
    /// View model to edit an entity and persist the datas using services.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    public class EntityEditorBoxViewModel<T> : ViewModelBase, IDialogBox where T : IEntityBase
    {

        #region Fields
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private T _Selection;
        private RelayCommand _SaveCommand;
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
                    DialogResultChanged?.Invoke(this, value);
                }
            }
        }

        /// <summary>
        /// Raised when 'DialogResult' changed.
        /// </summary>
        public event EventHandler<bool?> DialogResultChanged;
        #endregion
        
        #region Properties
        /// <summary>
        /// Item to edit.
        /// </summary>
        public T Selection
        {
            get { return _Selection; }
            set
            {
                if (!_Selection.Equals(value))
                {
                    _Selection = value;
                    OnPropertyChanged(nameof(Selection));
                }
            }
        }

        /// <summary>
        /// Repository service where retreive datas from.
        /// </summary>
        public IRepositoryService<T> DataService { get; set; }
        #endregion

        #region Commands
        /// <summary>
        /// Command to persist changes using 'DataService'.
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return (_SaveCommand == null) ? 
                    _SaveCommand = new RelayCommand(async () => await SaveCommandExecution(), true) : _SaveCommand;
            }
            set { _SaveCommand = value; }
        }
        #endregion

        #region Commands Execution
        /// <summary>
        /// Persist changes using 'DataService'.
        /// </summary>
        /// <returns>0 if successfully.</returns>
        public async Task<int> SaveCommandExecution()
        {
            if (DataService != null)
                return await DataService.SaveChangesAsync();
            else
                return int.MaxValue;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Initialize EntityEditor using 'locator'.
        /// </summary>
        /// <param name="locator"></param>
        public override void Initialize(ServiceLocator locator)
        {
            this.ServiceContainer = locator;
            DataService = locator.GetService<IRepositoryService<T>>();
        }
        #endregion

        #region Costructors
        /// <summary>
        /// Create a new instance of EntityEditor.
        /// </summary>
        public EntityEditorBoxViewModel() { }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services</param>
        public EntityEditorBoxViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of EntityEditor and initialize it using 'locator'. 
        /// </summary>
        /// <param name="locator">Service Locator containing services.</param>
        /// <param name="title">Title of the dialog box.</param>
        /// <param name="iconPath">Icon of the dialog box.</param>
        /// <param name="selection">Entity to edit.</param>
        public EntityEditorBoxViewModel(ServiceLocator locator, string title, string iconPath, T selection)
        {
            _Title = title;
            _IconPath = iconPath;
            _Selection = selection;
            Initialize(locator);            
        }
        #endregion
    }

}

using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Navigation.Models
{
    /// <summary>
    /// Provide a Context Menu Item.
    /// </summary>
    public class ContextMenuItemModel : ObservableObject
    {
        #region Fields
        private RelayCommand _Action;
        private string _Name;
        private string _Icon;
        #endregion

        #region Property
        /// <summary>
        /// Action to perform.
        /// </summary>
        public RelayCommand Action
        {
            get { return _Action; }
            set
            {
                if (_Action != value)
                {
                    _Action = value;
                    OnPropertyChanged(nameof(Action));
                }
            }
        }

        /// <summary>
        /// Name of the control.
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        /// <summary>
        /// Icon path for the control.
        /// </summary>
        public string Icon
        {
            get { return _Icon; }
            set
            {
                if (_Icon != value)
                {
                    _Icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }
        #endregion

        #region Costructors
        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        public ContextMenuItemModel(RelayCommand cmd)
        {
            _Action = cmd;
        }

        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd' and the Name 'text' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        /// <param name="text">Name for the control.</param>
        public ContextMenuItemModel(RelayCommand cmd, string text)
        {
            _Action = cmd;
            _Name = text;
        }

        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd', the Name 'text' and the Icon 'image' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        /// <param name="text">Name for the control.</param>
        /// <param name="image">Icon for the control.</param>
        public ContextMenuItemModel(RelayCommand cmd, string text, string image)
        {
            _Action = cmd;
            _Name = text;
            _Icon = image;
        }
        #endregion
    }
}

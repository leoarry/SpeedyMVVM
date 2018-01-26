using SpeedyMVVM.Navigation;
using SpeedyMVVM.Utilities;
using System;

namespace SpeedyMVVM.Navigation
{
    public class DialogBoxViewModel : ViewModelBase, IDialogBox
    {

        #region Fields
        private string _IconPath;
        private string _Title;
        private bool _IsVisible;
        private bool? _DialogResult;
        private string _Message;
        private string _TextInput;
        private bool _IsInputBox;
        private bool _DeclineCommandVisibility;
        private bool _CancelCommandVisibility;
        private DialogBoxEnum _DialogBoxType;
        private RelayCommand _ConfirmCommand;
        private RelayCommand _DeclineCommand;
        private RelayCommand _CancelCommand;
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

        #region Property
        /// <summary>
        /// Message to show on the dialog box.
        /// </summary>
        public string Message
        {
            get { return _Message; }
            set
            {
                if (value != _Message)
                {
                    _Message = value;
                    OnPropertyChanged(nameof(Message));
                }
            }
        }

        /// <summary>
        /// Text insert by the user
        /// </summary>
        public string TextInput
        {
            get { return _TextInput; }
            set
            {
                if (value != _TextInput)
                {
                    ConfirmCommand.IsEnabled = !string.IsNullOrEmpty(value);
                    _TextInput = value;
                    OnPropertyChanged(nameof(TextInput));
                }
            }
        }

        /// <summary>
        /// If TRUE the dialog box is an InputBox.
        /// </summary>
        public bool IsInputBox
        {
            get { return _IsInputBox; }
            set
            {
                if (value != _IsInputBox)
                {
                    _IsInputBox = value;
                    OnPropertyChanged(nameof(IsInputBox));
                }
            }
        }

        /// <summary>
        /// Get/Set the visibility of the decline button.
        /// </summary>
        public bool DeclineCommandVisibility
        {
            get { return _DeclineCommandVisibility; }
            set
            {
                if (_DeclineCommandVisibility != value)
                {
                    _DeclineCommandVisibility = value;
                    OnPropertyChanged(nameof(DeclineCommandVisibility));
                }
            }
        }

        /// <summary>
        /// Get/Set the visibility of the cancel button.
        /// </summary>
        public bool CancelCommandVisibility
        {
            get { return _CancelCommandVisibility; }
            set
            {
                if (_CancelCommandVisibility != value)
                {
                    _CancelCommandVisibility = value;
                    OnPropertyChanged(nameof(CancelCommandVisibility));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to set TRUE 'DialogResult'.
        /// </summary>
        public RelayCommand ConfirmCommand
        {
            get { return _ConfirmCommand ?? (_ConfirmCommand = new RelayCommand(() => DialogResult = true, true)); }
        }

        /// <summary>
        /// Command to set FALSE 'DialogResult'.
        /// </summary>
        public RelayCommand DeclineCommand
        {
            get { return _DeclineCommand ?? (_DeclineCommand = new RelayCommand(() => DialogResult = false, true)); }
        }

        /// <summary>
        /// Command to set NULL 'DialogResult'.
        /// </summary>
        public RelayCommand CancelCommand
        {
            get { return _CancelCommand ?? (_CancelCommand = new RelayCommand(() => DialogResult = null, true)); }
        }
        #endregion

        #region Methods
        private void OnNewDialogBoxViewModel()
        {
            _IsVisible = false;
            switch (_DialogBoxType)
            {
                case DialogBoxEnum.InputBox:
                    _IsInputBox = true;
                    _CancelCommandVisibility = true;
                    ConfirmCommand.Label = "OK";
                    CancelCommand.Label = "Cancel";
                    ConfirmCommand.IsEnabled = false;
                    break;
                case DialogBoxEnum.Ok:
                    ConfirmCommand.Label = "OK";
                    break;
                case DialogBoxEnum.OkCancel:
                    _CancelCommandVisibility = true;
                    ConfirmCommand.Label = "OK";
                    CancelCommand.Label = "Cancel";
                    break;
                case DialogBoxEnum.YesCancel:
                    _CancelCommandVisibility = true;
                    ConfirmCommand.Label = "Yes";
                    CancelCommand.Label = "Cancel";
                    break;
                case DialogBoxEnum.YesNo:
                    _DeclineCommandVisibility = true;
                    ConfirmCommand.Label = "Yes";
                    DeclineCommand.Label = "No";
                    break;
                case DialogBoxEnum.YesNoCancel:
                    _DeclineCommandVisibility = true;
                    _CancelCommandVisibility = true;
                    ConfirmCommand.Label = "Yes";
                    DeclineCommand.Label = "No";
                    CancelCommand.Label = "Cancel";
                    break;
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of DialogBoxViewModel.
        /// </summary>
        public DialogBoxViewModel()
        {
            _IsVisible = false;
        }

        /// <summary>
        /// Create a new instance of DialogBoxViewModel.
        /// </summary>
        /// <param name="dialogBoxType">Type of dialog box.</param>
        /// <param name="message">Message to show on the dialog box.</param>
        /// <param name="title">Title of the pop-up window.</param>
        /// <param name="iconPath">Icon path for the pop-up window.</param>
        public DialogBoxViewModel(DialogBoxEnum dialogBoxType, string message = null, string title = null, string iconPath = null)
        {
            _DialogBoxType = dialogBoxType;
            _Title = title;
            _Message = message;
            _IconPath = iconPath;
            OnNewDialogBoxViewModel();
        }
        #endregion

    }
}

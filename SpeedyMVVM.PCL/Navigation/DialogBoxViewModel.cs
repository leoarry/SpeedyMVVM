using SpeedyMVVM.Navigation.Enumerators;
using SpeedyMVVM.Navigation.Interfaces;
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
        private bool _ConfirmCommandVisibility;
        private bool _DeclineCommandVisibility;
        private bool _CancelCommandVisibility;
        private string _ConfirmCommandText;
        private string _DeclineCommandText;
        private string _CancelCommandText;
        private DialogBoxEnum _DialogBoxType;
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
        /// Get/Set the visibility of the confirmation button.
        /// </summary>
        public bool ConfirmCommandVisibility
        {
            get { return _ConfirmCommandVisibility; }
            set
            {
                if (_ConfirmCommandVisibility != value)
                {
                    _ConfirmCommandVisibility = value;
                    OnPropertyChanged(nameof(ConfirmCommandVisibility));
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

        /// <summary>
        /// Get/Set the text of the confirmation button.
        /// </summary>
        public string ConfirmCommandText
        {
            get { return _ConfirmCommandText; }
            set
            {
                if (_ConfirmCommandText != value)
                {
                    _ConfirmCommandText = value;
                    OnPropertyChanged(nameof(ConfirmCommandText));
                }
            }
        }

        /// <summary>
        /// Get/Set the text of the decline button. 
        /// </summary>
        public string DeclineCommandText
        {
            get { return _DeclineCommandText; }
            set
            {
                if (_DeclineCommandText != value)
                {
                    _DeclineCommandText = value;
                    OnPropertyChanged(nameof(DeclineCommandText));
                }
            }
        }

        /// <summary>
        /// Get/Set the text of the cancel button.
        /// </summary>
        public string CancelCommandText
        {
            get { return _CancelCommandText; }
            set
            {
                if (_CancelCommandText != value)
                {
                    _CancelCommandText = value;
                    OnPropertyChanged(nameof(CancelCommandText));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to set TRUE 'DialogResult'.
        /// </summary>
        public RelayCommand ConfirmCommand { get { return new RelayCommand(() => DialogResult = true, true); } }

        /// <summary>
        /// Command to set FALSE 'DialogResult'.
        /// </summary>
        public RelayCommand DeclineCommand { get { return new RelayCommand(() => DialogResult = false, true); } }

        /// <summary>
        /// Command to set NULL 'DialogResult'.
        /// </summary>
        public RelayCommand CancelCommand { get { return new RelayCommand(() => DialogResult = null, true); } }
        #endregion

        #region Methods
        public override void Initialize(ServiceLocator locator)
        {
            _IsVisible = false;
            switch (_DialogBoxType)
            {
                case DialogBoxEnum.InputBox:
                    _IsInputBox = true;
                    _ConfirmCommandVisibility = true;
                    _CancelCommandVisibility = true;
                    _ConfirmCommandText = "OK";
                    _CancelCommandText = "CANCEL";
                    break;
                case DialogBoxEnum.Ok:
                    _ConfirmCommandVisibility = true;
                    _ConfirmCommandText = "OK";
                    break;
                case DialogBoxEnum.OkCancel:
                    _ConfirmCommandVisibility = true;
                    _CancelCommandVisibility = true;
                    _ConfirmCommandText = "OK";
                    _CancelCommandText = "CANCEL";
                    break;
                case DialogBoxEnum.YesCancel:
                    _ConfirmCommandVisibility = true;
                    _CancelCommandVisibility = true;
                    _ConfirmCommandText = "YES";
                    _CancelCommandText = "CANCEL";
                    break;
                case DialogBoxEnum.YesNo:
                    _ConfirmCommandVisibility = true;
                    _DeclineCommandVisibility = true;
                    _ConfirmCommandText = "YES";
                    _DeclineCommandText = "NO";
                    break;
                case DialogBoxEnum.YesNoCancel:
                    _ConfirmCommandVisibility = true;
                    _DeclineCommandVisibility = true;
                    _CancelCommandVisibility = true;
                    _ConfirmCommandText = "YES";
                    _DeclineCommandText = "NO";
                    _CancelCommandText = "CANCEL";
                    break;
            }
            IsInitialized = true;
        }
        #endregion

        #region Costructors
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
        public DialogBoxViewModel(DialogBoxEnum dialogBoxType, string message)
        {
            _DialogBoxType = dialogBoxType;
            _Message = message;
            Initialize(null);
        }

        /// <summary>
        /// Create a new instance of DialogBoxViewModel.
        /// </summary>
        /// <param name="dialogBoxType">Type of dialog box.</param>
        /// <param name="message">Message to show on the dialog box.</param>
        /// <param name="title">Title of the popup window.</param>
        public DialogBoxViewModel(DialogBoxEnum dialogBoxType, string message, string title)
        {
            _DialogBoxType = dialogBoxType;
            _Title = title;
            _Message = message;
        }

        /// <summary>
        /// Create a new instance of DialogBoxViewModel.
        /// </summary>
        /// <param name="dialogBoxType">Type of dialog box.</param>
        /// <param name="message">Message to show on the dialog box.</param>
        /// <param name="title">Title of the popup window.</param>
        /// <param name="iconPath">Icon path for the popup window.</param>
        public DialogBoxViewModel(DialogBoxEnum dialogBoxType, string message, string title, string iconPath)
        {
            _DialogBoxType = dialogBoxType;
            _Title = title;
            _Message = message;
            _IconPath = iconPath;
        }
        #endregion

    }
}

using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.ViewModels
{
    public class DialogBoxViewModel : ObservableObject
    {

        #region Fields
        private string _Title;
        private string _Message;
        private string _Input;
        private string _IconPath;
        private bool? _DialogResult;
        private bool _IsInputBox;
        private bool _IsVisible;
        #endregion

        #region Property
        public string Title
        {
            get { return _Title; }
            set
            {
                if (value != _Title)
                {
                    _Title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
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
        public string Input
        {
            get { return _Input; }
            set
            {
                if (value != _Input)
                {
                    _Input = value;
                    OnPropertyChanged(nameof(Input));
                }
            }
        }
        public string IconPath
        {
            get { return _IconPath; }
            set
            {
                if (value != _IconPath)
                {
                    _IconPath = value;
                    OnPropertyChanged(nameof(IconPath));
                }
            }
        }
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
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (value != _IsVisible)
                {
                    _IsVisible = value;
                    OnPropertyChanged(nameof(IsVisible));
                }
            }
        }
        public bool? DialogResult
        {
            get { return _DialogResult; }
            set
            {
                if (value != _DialogResult)
                {
                    _DialogResult = value;
                    OnPropertyChanged(nameof(DialogResult));
                }
            }
        }
        #endregion

        #region Costructors
        public DialogBoxViewModel(string message, string title, string iconPath, string input)
        {
            _IsVisible = false;
            _Title = title;
            _Message = message;
            _Input = input;
            _IconPath = iconPath;
        }
        public DialogBoxViewModel(string message, string title, string iconPath)
        {
            _IsVisible = false;
            _Title = title;
            _Message = message;
            _IconPath = iconPath;
        }
        public DialogBoxViewModel(string message, string title)
        {
            _IsVisible = false;
            _Title = title;
            _Message = message;
        }
        public DialogBoxViewModel(string message)
        {
            _IsVisible = false;
            _Message = message;
        }
        public DialogBoxViewModel()
        {
            _IsVisible = false;
        }
        #endregion

    }
}

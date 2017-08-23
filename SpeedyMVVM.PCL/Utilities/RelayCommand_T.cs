using System;
using System.Windows.Input;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Generic definition of the Relay Command class.
    /// </summary>
    /// <typeparam name="T">Target type of the relay command.</typeparam>
    public class RelayCommand<T> : ICommand
    {
        #region Fields
        private readonly Action<T> _handler;
        private bool _isEnabled;
        #endregion

        #region Costructors
        /// <summary>
        /// Create an instance of RelayCommand. By default, the command will be disable.
        /// </summary>
        /// <param name="handler">Action to perform.</param>
        public RelayCommand(Action<T> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// Create an instance of RelayCommand.
        /// </summary>
        /// <param name="handler">Action to perform.</param>
        /// <param name="enable">Enable the command.</param>
        public RelayCommand(Action<T> handler, bool enable)
        {
            _handler = handler;
            IsEnabled = enable;
        }
        #endregion

        #region Properties and Events
        /// <summary>
        /// Raised when the property 'IsEnabled' change the current status.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// The command is Enabled to execute the action.
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// The command can execute the action.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Execute the stored action.
        /// </summary>
        /// <param name="parameter">Object to pass to the delegate.</param>
        public void Execute(object parameter)
        {
            _handler((T)parameter);
        }
        #endregion
    }
}

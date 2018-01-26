using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeedyMVVM.Utilities
{
    /// <summary>
    /// Generic definition of the Relay Command class.
    /// </summary>
    /// <typeparam name="T">Target type of the relay command.</typeparam>
    public class RelayCommand<T> : IRelayCommand
    {
        #region Fields
        private Action<T> _handler;
        private bool _isEnabled;
        private string _Label;
        private string _Icon;
        #endregion

        #region Costructors
        /// <summary>
        /// Create an instance of RelayCommand.
        /// </summary>
        /// <param name="handler">Action to perform.</param>
        /// <param name="enable">Enable the command.</param>
        /// <param name="label">String label for the command.</param>
        /// <param name="iconPath">Icon path for the command</param>
        public RelayCommand(Action<T> handler, bool enable=false, string label=null, string iconPath=null)
        {
            _handler = handler;
            IsEnabled = enable;
            _Label = label;
            _Icon = iconPath;
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
        /// Name of the control.
        /// </summary>
        public string Label
        {
            get { return _Label; }
            set { _Label = value; }
        }

        /// <summary>
        /// Icon path for the control.
        /// </summary>
        public string Icon
        {
            get { return _Icon; }
            set { _Icon = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// The command can execute the action.
        /// </summary>
        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        /// <summary>
        /// Execute the stored action.
        /// </summary>
        /// <param name="parameter">Object to pass to the delegate.</param>
        public void Execute(object parameter = null)
        {
            _handler((T)parameter);
        }

        /// <summary>
        /// Execute the stored action.
        /// </summary>
        /// <param name="parameter">Object to pass to the delegate.</param>
        public Task ExecuteAsync(object parameter = null)
        {
            return Task.Factory.StartNew(() => _handler((T)parameter));
        }

        /// <summary>
        /// Combine the action passed as parameter with the current action handled by the command.
        /// </summary>
        /// <param name="newAction">New action to combine.</param>
        /// <param name="newActionFirst">If true will combine first the new action then the actual handled action of the command.</param>
        public void CombineAction(Action<T> newAction, bool newActionFirst = false)
        {
            if (newActionFirst)
                _handler = (Action<T>)Delegate.Combine(newAction, _handler);
            else
                _handler = (Action<T>)Delegate.Combine(_handler, newAction);
        }

        /// <summary>
        /// Replace the current action handled by the command with the new action passed as parameter.
        /// </summary>
        /// <param name="newAction">New action that will replace the actual action handled by the command</param>
        public void ReplaceAction(Action<T> newAction)
        {
            _handler = newAction;
        }
        #endregion
    }
}

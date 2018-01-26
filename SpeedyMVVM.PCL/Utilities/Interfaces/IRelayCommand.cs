using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SpeedyMVVM.Utilities
{
    public interface IRelayCommand:ICommand
    {
        /// <summary>
        /// Return a value which tell whenever the command is enabled or not.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// String representing the name or the description of the command.
        /// </summary>
        string Label { get; set; }

        /// <summary>
        /// String representing the icon path of the command.
        /// </summary>
        string Icon { get; set; }

        /// <summary>
        /// Execute the stored action.
        /// </summary>
        /// <param name="parameter">Object to pass to the delegate.</param>
        /// <returns></returns>
        Task ExecuteAsync(object parameter = null);
    }
}

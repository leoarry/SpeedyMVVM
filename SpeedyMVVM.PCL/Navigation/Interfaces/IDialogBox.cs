using SpeedyMVVM.Utilities.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Navigation.Interfaces
{
    public interface IDialogBox : IPageViewModel
    {
        /// <summary>
        /// Result of the dialog (fire DialogResultChanged event on value changed).
        /// </summary>
        bool? DialogResult { get; set; }

        /// <summary>
        /// Raised when 'DialogResult' changed.
        /// </summary>
        event EventHandler<bool?> DialogResultChanged;
    }
}

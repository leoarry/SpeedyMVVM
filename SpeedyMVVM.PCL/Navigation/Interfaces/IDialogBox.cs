using System;

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

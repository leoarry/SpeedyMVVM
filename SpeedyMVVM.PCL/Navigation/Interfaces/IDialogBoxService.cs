using SpeedyMVVM.Utilities;
using System.Threading.Tasks;

namespace SpeedyMVVM.Navigation
{
    /// <summary>
    /// Define a service for basic dialog boxes.
    /// </summary>
    public interface IDialogBoxService
    {
        Task<string> ShowInputBox(string message, string title, string iconPath);
        Task<bool?> ShowMessageBox(string message, string title, DialogBoxEnum BoxType, DialogBoxIconEnum icon);
        Task<bool?> ShowViewModel(IViewModelBase viewModel);
        Task<bool?> ShowDialogBox(IDialogBox dialogBox);
    }
}

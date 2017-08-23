using SpeedyMVVM.Navigation.Enumerators;

namespace SpeedyMVVM.Navigation.Interfaces
{
    /// <summary>
    /// Define a service for basic dialog boxes.
    /// </summary>
    public interface IDialogBoxService
    {
        string ShowInputBox(string message, string title, string iconPath);
        bool? ShowMessageBox(string message, string title, DialogBoxEnum BoxType, DialogBoxIconEnum icon);
    }
}

using SpeedyMVVM.Utilities.Enumerators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities.Interfaces
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

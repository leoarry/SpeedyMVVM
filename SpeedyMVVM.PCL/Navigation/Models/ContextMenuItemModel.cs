using SpeedyMVVM.Utilities;
using System;

namespace SpeedyMVVM.Navigation
{
    /// <summary>
    /// Provide a Context Menu Item.
    /// </summary>
    public class ContextMenuItemModel : RelayCommand
    {
        #region Fields
        private string _Name;
        private string _Icon;
        #endregion

        #region Property
        /// <summary>
        /// Name of the control.
        /// </summary>
        public string Name { get; set; }
        #endregion

        #region Costructors
        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        public ContextMenuItemModel(Action cmd) : base(cmd) { }

        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd' and the Name 'text' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        /// <param name="text">Name for the control.</param>
        public ContextMenuItemModel(Action cmd, string text):base(cmd)
        {
            _Name = text;
        }

        /// <summary>
        /// Initialize a new ContextMenuItemModel with the Action 'cmd', the Name 'text' and the Icon 'image' passed as parameter.
        /// </summary>
        /// <param name="cmd">Action to perform.</param>
        /// <param name="text">Name for the control.</param>
        /// <param name="image">Icon for the control.</param>
        public ContextMenuItemModel(Action cmd, string text, string image):base(cmd)
        {
            _Name = text;
            _Icon = image;
        }
        #endregion
    }
}

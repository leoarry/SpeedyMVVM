using SpeedyMVVM.Utilities;
using System;

namespace SpeedyMVVM.Navigation
{
    /// <summary>
    /// Interface to identify a page.
    /// </summary>
    public interface IPageViewModel:IViewModelBase
    {
        #region Properties
        /// <summary>
        /// Title of the page.
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// Path of the page image.
        /// </summary>
        string IconPath { get; set; }
        /// <summary>
        /// The view of the page is visible or not to the current user.
        /// </summary>
        bool IsVisible { get; set; }
        #endregion
    }
}

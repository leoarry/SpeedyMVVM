using SpeedyMVVM.Utilities.Interfaces;

namespace SpeedyMVVM.Navigation.Interfaces
{
    /// <summary>
    /// Interface to identify a page.
    /// </summary>
    public interface IPageViewModel:IViewModelBase
    {
        #region Properties
        /// <summary>
        /// Name of the page.
        /// </summary>
        string Name { get; set; }
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

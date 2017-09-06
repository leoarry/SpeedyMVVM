﻿using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Navigation.Models;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.Utilities.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System;

namespace SpeedyMVVM.Navigation
{
    /// <summary>
    /// Define a container if pages
    /// </summary>
    public class PageManagerViewModel : ViewModelBase
    {
        #region Fields
        private bool _IsExpanded;
        private bool _AutoSetVisibility;
        private RelayCommand<IPageViewModel> _ChangePageCommand;
        private IPageViewModel _CurrentPageViewModel;
        private ObservableCollection<IPageViewModel> _PageViewModels;
        #endregion

        #region Properties
        /// <summary>
        /// TRUE when the collection of pages is visible.
        /// </summary>
        public bool IsExpanded
        {
            get { return _IsExpanded; }
            set
            {
                if (value != _IsExpanded)
                {
                    _IsExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                }
            }
        }

        /// <summary>
        /// If TRUE will run SetPagesVisibility() on initializing. 
        /// </summary>
        public bool AutoSetVisibility
        {
            get { return _AutoSetVisibility; }
            set
            {
                if (_AutoSetVisibility != value)
                {
                    _AutoSetVisibility = value;
                    OnPropertyChanged(nameof(AutoSetVisibility));
                }
            }
        }

        /// <summary>
        /// Collection of pages.
        /// </summary>
        public ObservableCollection<IPageViewModel> Pages
        {
            get
            {
                return (_PageViewModels == null) ? 
                    _PageViewModels = new ObservableCollection<IPageViewModel>() : _PageViewModels;
            }
            set
            {
                if (_PageViewModels != value)
                {
                    _PageViewModels = value;
                    OnPropertyChanged(nameof(Pages));
                }
            }
        }

        /// <summary>
        /// Current page.
        /// </summary>
        public IPageViewModel CurrentPage
        {
            get
            {
                return _CurrentPageViewModel;
            }
            set
            {
                if (_CurrentPageViewModel != value)
                {
                    if (_CurrentPageViewModel is PageManagerViewModel)
                        ((PageManagerViewModel)_CurrentPageViewModel).IsExpanded = false;
                    _CurrentPageViewModel = value;
                    if (!_CurrentPageViewModel.IsInitialized)
                        _CurrentPageViewModel.Initialize(ServiceContainer);
                    OnPropertyChanged(nameof(CurrentPage));
                }
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Command to change page.
        /// </summary>
        public RelayCommand<IPageViewModel> ChangePageCommand
        {
            get
            {
                return (_ChangePageCommand == null) ? 
                    _ChangePageCommand = new RelayCommand<IPageViewModel>(p => ChangeViewModel(p), true) : _ChangePageCommand;
            }
        }
        #endregion

        #region Command Executions
        /// <summary>
        /// Set the parameter 'viewModel' as current page.
        /// </summary>
        /// <param name="viewModel">Page to show.</param>
        public void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!Pages.Contains(viewModel))
                Pages.Add(viewModel);

            CurrentPage = Pages
                .FirstOrDefault(vm => vm == viewModel);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Set the visibility for the pages of 'Pages' based on the AccessLevel of the current user.
        /// </summary>
        public virtual void SetPagesVisibility()
        {
            var ser = ServiceContainer.GetService<IAppRuntimeSettingsService>();
            if (ser == null)
                return;
            var pageDataService = ServiceContainer.GetService<IRepositoryService<PageSettingModel>>();
            foreach (var p in Pages)
            {
                if (!ser.PageSettings.Any(ps => ps.PageName == p.Title))
                {
                    var page = new PageSettingModel { PageName = p.Title, AccessLevel = 0 };
                    pageDataService.AddEntityAsync(page);
                    ser.PageSettings.Add(page);
                    pageDataService.SaveChangesAsync();
                }
                var pLevel = ser.PageSettings.Where(ps => ps.PageName == p.Title).FirstOrDefault();
                if (ser.CurrentUserLevel > pLevel.AccessLevel) { p.IsVisible = false; }
                else { p.IsVisible = true; }

                if (p.IsVisible && p.GetType() == typeof(PageManagerViewModel))
                    ((PageManagerViewModel)p).SetPagesVisibility();
            }
        }

        /// <summary>
        /// Initialize the current instance of PageManagerViewModel using the parameter 'locator'.
        /// </summary>
        /// <param name="locator">Service container with the current services</param>
        public override void Initialize(ServiceLocator locator)
        {
            ServiceContainer = locator;
            if (AutoSetVisibility)
                SetPagesVisibility();
            IsInitialized = true;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new instance of PageManagerViewModel.
        /// </summary>
        public PageManagerViewModel()
        {
        }

        /// <summary>
        /// Create a new instance of PageManagerViewModel.
        /// </summary>
        /// <param name="autoSetVisibility">If true, will run SetPagesVisibility() after initialize.</param>
        public PageManagerViewModel(bool autoSetVisibility)
        {
            _AutoSetVisibility = autoSetVisibility;
        }

        /// <summary>
        /// Create a new instance of PageManagerViewModel.
        /// </summary>
        /// <param name="locator">Service container with current services.</param>
        public PageManagerViewModel(ServiceLocator locator)
        {
            Initialize(locator);
        }

        /// <summary>
        /// Create a new instance of PageManagerViewModel.
        /// </summary>
        /// <param name="locator">Service container with current services.</param>
        /// <param name="autoSetVisibility">If true, will run SetPagesVisibility() after initialize.</param>
        public PageManagerViewModel(ServiceLocator locator, bool autoSetVisibility)
        {
            _AutoSetVisibility = autoSetVisibility;
            Initialize(locator);
        }
        #endregion
    }
}

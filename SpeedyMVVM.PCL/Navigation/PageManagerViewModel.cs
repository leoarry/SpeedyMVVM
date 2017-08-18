using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.Utilities.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedyMVVM.Navigation
{
    public abstract class PageManagerViewModelBase : ViewModelBase
    {
        #region Fields
        private bool _IsExpanded;
        private RelayCommand<IPageViewModel> _changePageCommand;
        private IPageViewModel _currentPageViewModel;
        private ObservableCollection<IPageViewModel> _pageViewModels;
        #endregion

        #region Properties
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
        public ObservableCollection<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new ObservableCollection<IPageViewModel>();

                return _pageViewModels;
            }
            set
            {
                if (_pageViewModels != value)
                {
                    _pageViewModels = value;
                    OnPropertyChanged(nameof(PageViewModels));
                }
            }
        }
        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    if (_currentPageViewModel is PageManagerViewModelBase)
                    {
                        var p = _currentPageViewModel as PageManagerViewModelBase;
                        p.IsExpanded = false;
                    }
                    _currentPageViewModel = value;
                    OnPropertyChanged(nameof(CurrentPageViewModel));
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand<IPageViewModel> ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand<IPageViewModel>(p => ChangeViewModel(p), true);
                }
                return _changePageCommand;
            }
        }
        #endregion

        #region Command Executions
        public void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }
        #endregion

        #region Methods
        public void SetPagesVisibility()
        {
            var ser = ServiceContainer.GetService<IAppRuntimeSettingsService>();
            var pageDataService = ServiceContainer.GetService<IRepositoryService<PageSettingModel>>();
            foreach (var p in PageViewModels)
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
            }
            var pageManagers = PageViewModels.OfType<PageManagerViewModelBase>();
            if (pageManagers.Count() > 0)
            {
                foreach (var p in pageManagers)
                {
                    p.SetPagesVisibility();
                }
            }
        }
        #endregion

        public PageManagerViewModelBase(ServiceLocator service)
        {
            ServiceContainer = service;
        }
    }
}

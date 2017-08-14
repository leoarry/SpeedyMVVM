using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Expressions;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedyMVVM.DataAccess
{
    public class EntityPickerBoxViewModel<T> : ViewModelBase, IPageViewModel where T : IEntityBase
    {

        #region IPageViewModel Implementation
        public string IconPath
        {
            get { return _IconPath; }
            set
            {
                if (_IconPath != value)
                {
                    _IconPath = value;
                    OnPropertyChanged(nameof(IconPath));
                }
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public bool IsVisible { get; set; }
        #endregion

        #region Fields
        private bool _CanSearch;
        private bool? _DialogResult;
        private string _name;
        private string _IconPath;
        private EntityFilterViewModel<T> _EntityFilter;
        private ObservableCollection<T> _SelectedItems;
        private RelayCommand<object> _AddSelectionCommand;
        private RelayCommand _AddEntityCommand;
        #endregion

        #region Property
        public bool CanSearch
        {
            get { return _CanSearch; }
            set
            {
                if (value != _CanSearch)
                {
                    _CanSearch = value;
                    OnPropertyChanged(nameof(CanSearch));
                }
            }
        }
        public bool? DialogResult
        {
            get { return _DialogResult; }
            set
            {
                if (_DialogResult != value)
                {
                    _DialogResult = value;
                    OnPropertyChanged(nameof(DialogResult));
                }
            }
        }
        public EntityFilterViewModel<T> EntityFilter
        {
            get { return _EntityFilter; }
            set
            {
                if (_EntityFilter != value)
                {
                    _EntityFilter = value;
                    OnPropertyChanged(nameof(EntityFilter));
                }
            }
        }
        public ObservableCollection<T> SelectedItems
        {
            get { return _SelectedItems; }
            set
            {
                if (value != _SelectedItems)
                {
                    _SelectedItems = value;
                    OnPropertyChanged(nameof(SelectedItems));
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand<object> AddSelectionCommand
        {
            get
            {
                if (_AddSelectionCommand == null)
                {
                    _AddSelectionCommand = new RelayCommand<object>(AddSelectionExecute, true);
                }
                return _AddSelectionCommand;
            }
        }
        public RelayCommand AddEntityCommand
        {
            get
            {
                if (_AddEntityCommand == null)
                {
                    _AddEntityCommand = new RelayCommand(AddEntityExecute, true);
                }
                return _AddEntityCommand;
            }
        }
        #endregion

        #region Commands Execution
        public void AddSelectionExecute(object parameter)
        {
            try
            {
                IList myItems = (IList)parameter;
                if (myItems.OfType<T>().Count() > 0)
                {
                    SelectedItems = new ObservableCollection<T>(myItems.Cast<T>());
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        private void AddEntityExecute()
        {
            /*var dataService = ServiceContainer.GetService<IRepositoryService<T>>();
            var dialogService = ServiceContainer.GetService<IDialogBoxService>();
            dynamic newItem = Activator.CreateInstance(typeof(T));
            var vm = new EntityEditorViewModelBase<T>(Name, ImagePath, newItem);
            if (dialogService.ShowEntityBox<T>(vm) == true)
            {
                dataService.AddEntityAsync(vm.Selection);
                dataService.SaveDB();
                EntityFilter.Search(null);
            }*/
        }
        #endregion

        #region Costructors
        public EntityPickerBoxViewModel(string Name, string ImagePath, ServiceLocator service)
        {
            _name = Name;
            _IconPath = ImagePath;
            _CanSearch = true;
            base.ServiceContainer = service;
            _EntityFilter = new EntityFilterViewModel<T>(service);
        }
        public EntityPickerBoxViewModel(string Name, ServiceLocator service)
        {
            _name = Name;
            _CanSearch = true;
            base.ServiceContainer = service;
            _EntityFilter = new EntityFilterViewModel<T>(service);
        }
        public EntityPickerBoxViewModel(ServiceLocator service)
        {
            _CanSearch = true;
            base.ServiceContainer = service;
            _EntityFilter = new EntityFilterViewModel<T>(service);
        }
        public EntityPickerBoxViewModel(string Name, ServiceLocator service, List<ExpressionModel> Filters)
        {
            _CanSearch = true;
            _name = Name;
            base.ServiceContainer = service;
            _EntityFilter = new EntityFilterViewModel<T>(service, Filters);
            _EntityFilter.Search(null);
        }

        public override void Initialize(ServiceLocator service)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

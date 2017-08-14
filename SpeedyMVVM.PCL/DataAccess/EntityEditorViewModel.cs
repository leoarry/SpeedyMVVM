using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Navigation.Interfaces;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.Utilities.Interfaces;
using System;

namespace SpeedyMVVM.DataAccess
{
    public class EntityEditorViewModelBase<T> : ViewModelBase, IPageViewModel where T : IEntityBase
    {
        #region IPageViewModel Implementation
        public string IconPath
        {
            get { return _iconPath; }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
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
        private bool? _DialogResult;
        private string _name;
        private string _iconPath;
        private T _Selection;
        private RelayCommand _SaveCommand;
        private IRepositoryService<T> dataService;
        private IDialogBoxService dialogService;
        #endregion

        #region Property
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
        public T Selection
        {
            get { return _Selection; }
            set
            {
                if (!_Selection.Equals(value))
                {
                    _Selection = value;
                    OnPropertyChanged(nameof(Selection));
                }
            }
        }
        #endregion

        #region Commands
        public RelayCommand SaveCommand
        {
            get
            {
                if (_SaveCommand == null) { _SaveCommand = new RelayCommand(SaveCommandExecution, true); }
                return _SaveCommand;
            }
            set { _SaveCommand = value; }
        }
        #endregion

        #region Commands Execution
        public void SaveCommandExecution()
        {
            dataService.SaveChanges();
        }

        public override void Initialize(ServiceLocator service)
        {
            throw new NotImplementedException();
        }
        
        #endregion

        #region Costructors
        public EntityEditorViewModelBase(ServiceLocator service, string name, string imagePath, T selection)
        {
            _name = name;
            _iconPath = imagePath;
            _Selection = selection;
            dataService = service.GetService<IRepositoryService<T>>();
            dialogService = service.GetService<IDialogBoxService>();
            this.ServiceContainer = service;
        }
        #endregion
    }

}

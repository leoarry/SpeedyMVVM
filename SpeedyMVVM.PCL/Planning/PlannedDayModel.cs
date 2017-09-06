using SpeedyMVVM.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace SpeedyMVVM.Planning
{
    public class PlannedDayModel<T> : ObservableObject where T : IPlannable
    {

        #region Fields
        private DateTime _PlannedDate;
        private bool _IsSelected;
        private ObservableCollection<T> _Items;
        private T _Selection;
        #endregion

        #region Property
        public DateTime PlannedDate
        {
            get { return _PlannedDate; }
            set
            {
                if (_PlannedDate != value)
                {
                    _PlannedDate = value;
                    OnPropertyChanged(nameof(PlannedDate));
                }
            }
        }
        public int DayOfTheMonth
        {
            get { return PlannedDate.Day; }
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    _IsSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }
        public ObservableCollection<T> Items
        {
            get
            {
                return _Items= _Items.Where(r => r.PlannedDate >= PlannedDate &&
                                                 r.PlannedDate < PlannedDate.AddDays(1))
                                     .AsObservableCollection(); ;
            }
            set
            {
                if (value != _Items)
                {
                    _Items = value;
                    OnPropertyChanged(nameof(Items));
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

        #region Constructors
        public PlannedDayModel(DateTime plannedDate, ObservableCollection<T> items)
        {
            _PlannedDate = plannedDate;
            _Items = items;
        }

        public PlannedDayModel(DateTime plannedDate)
        {
            _PlannedDate = plannedDate;
            _Items = new ObservableCollection<T>();
        }
        #endregion
    }
}

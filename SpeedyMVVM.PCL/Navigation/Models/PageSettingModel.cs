using SpeedyMVVM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedyMVVM.Navigation.Models
{
    public class PageSettingModel : EntityBase
    {
        #region Fields
        private string _PageName;
        private int _AccessLevel;
        #endregion

        #region Property
        public string PageName
        {
            get { return _PageName; }
            set
            {
                if (value != _PageName)
                {
                    _PageName = value;
                    OnPropertyChanged(nameof(PageName));
                }
            }
        }
        public int AccessLevel
        {
            get { return _AccessLevel; }
            set
            {
                if (value != _AccessLevel)
                {
                    _AccessLevel = value;
                    OnPropertyChanged(nameof(AccessLevel));
                }
            }
        }
        #endregion
    }
}

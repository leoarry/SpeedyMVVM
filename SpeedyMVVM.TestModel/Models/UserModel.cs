using SpeedyMVVM.DataAccess;
using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SpeedyMVVM.TestModel.Models
{
    public class UserModel: EntityBase
    {
        #region Fields
        private string _Name;
        private string _Surname;
        private string _NickName;
        private string _Password;
        private int _TypeOfUse;
        private int _AccessLevel;
        #endregion

        #region Property
        public string Name
        {
            get { return _Name; }
            set
            {
                if (value != _Name)
                {
                    _Name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }
        public string Surname
        {
            get { return _Surname; }
            set
            {
                if (value != _Surname)
                {
                    _Surname = value;
                    OnPropertyChanged(nameof(Surname));
                }
            }
        }
        public string NickName
        {
            get { return _NickName; }
            set
            {
                if (value != _NickName)
                {
                    _NickName = value;
                    OnPropertyChanged(nameof(NickName));
                }
            }
        }
        public string Password
        {
            get { return _Password; }
            set
            {
                if (value != _Password)
                {
                    _Password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }
        public int TypeOfUser
        {
            get { return _TypeOfUse; }
            set
            {
                if (value != _TypeOfUse)
                {
                    _TypeOfUse = value;
                    OnPropertyChanged(nameof(TypeOfUser));
                }
            }
        }
        public int AccessLevel
        {
            get { return _AccessLevel; }
            set
            {
                if (_AccessLevel != value)
                {
                    _AccessLevel = value;
                    OnPropertyChanged(nameof(AccessLevel));
                }
            }
        }
        
        #endregion
    }
}

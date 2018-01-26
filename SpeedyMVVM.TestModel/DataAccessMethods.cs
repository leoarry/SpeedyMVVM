using SpeedyMVVM.DataAccess;
using SpeedyMVVM.Navigation;
using SpeedyMVVM.TestModel.Models;
using SpeedyMVVM.TestModel.Services;
using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpeedyMVVM.TestModel
{
    public static class DataAccessMethods
    {
        public static List<UserModel> GetListOfUser()
        {
            var list = new List<UserModel>();
            list.Add(new UserModel
            { Name = "Patrick", Surname = "Jefferson", AccessLevel = 1000, NickName = "Gentleman" });
            list.Add(new UserModel
            { Name = "Arnold", Surname = "Jhonson", AccessLevel = 0, NickName = "Rock" });
            list.Add(new UserModel
            { Name = "Paul", Surname = "Gibson", AccessLevel = 100, NickName = "GuitarMan" });
            return list;
        }

        public static ServiceLocator GetServiceLocator()
        {
            var locator = new ServiceLocator();
            locator.RegisterService<IRepositoryService<UserModel>>(new RepositoryService<UserModel> { List = GetListOfUser() });
            locator.RegisterService<IDialogBoxService>(new EntitiesDialogBoxService());
            return locator;
        }
    }
}

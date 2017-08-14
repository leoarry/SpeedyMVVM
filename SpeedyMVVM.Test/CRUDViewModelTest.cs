using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.TestModel.Models;
using SpeedyMVVM.TestModel;
using SpeedyMVVM.DataAccess.Interfaces;
using SpeedyMVVM.Utilities;
using SpeedyMVVM.TestModel.Services;

namespace SpeedyMVVM.Test
{
    [TestClass]
    public class CRUDViewModelTest
    {
        ServiceLocator locator= DataAccessMethods.GetServiceLocator();

        [TestMethod]
        public void AddEntity_Test()
        {
            var userToAdd = new UserModel { Name = "Donald", Surname = "Duck", AccessLevel = 0 };
            var viewModel = new CRUDViewModel<UserModel>();

            viewModel.AddCommandExecute(userToAdd);

            var retrievedUser = viewModel.Filter.Items.Where(u => u.Name == userToAdd.Name).FirstOrDefault();

            Assert.AreEqual(userToAdd, retrievedUser);
        }

        [TestMethod]
        public void RemoveEntity_Test()
        {
            var userToRemove = new UserModel { Name = "Donald", Surname = "Duck", AccessLevel = 0 };
            var viewModel = new CRUDViewModel<UserModel>();

            viewModel.Filter.Items.Add(userToRemove);

            viewModel.RemoveCommandExecute();

            var retrievedUser = viewModel.Filter.Items.Where(u => u.Name == userToRemove.Name).First();
            
            Assert.IsNull(retrievedUser);
        }

        [TestMethod]
        public void Initialize_Test()
        {

        }
    }
}

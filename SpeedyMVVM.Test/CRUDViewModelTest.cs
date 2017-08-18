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
using System.Threading.Tasks;

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

            viewModel.AddCommandExecute(userToAdd).Wait();

            var retrievedUser = viewModel.Items.Where(u => u.Name == userToAdd.Name).FirstOrDefault();

            Assert.AreEqual(userToAdd, retrievedUser);
        }

        [TestMethod]
        public void RemoveEntity_Test()
        {
            var userToRemove = new UserModel { Name = "Donald", Surname = "Duck", AccessLevel = 0 };
            var viewModel = new CRUDViewModel<UserModel>();
            
            viewModel.SelectedItem = userToRemove;
            viewModel.Items.Add(viewModel.SelectedItem);
            viewModel.RemoveCommandExecute().Wait();
            
            Assert.IsTrue(viewModel.Items.Where(u => u.Name == "Donald").Count()==0);
        }

        [TestMethod]
        public void Search_Test()
        {
            var viewModel = new CRUDViewModel<UserModel>();
            var listOfUsers = new List<UserModel>
            {
                new UserModel {Name="Donald", Surname="Duck" },
                new UserModel {Name="Foxy", Surname="Lady" },
                new UserModel {Name="Peter", Surname="Griffin" },
                new UserModel {Name="Fernanda", Surname="Lopez" }
            };

            viewModel.Items = listOfUsers.AsObservableCollection();
            viewModel.Filter.HiddenExpression = u => u.Name.StartsWith("F");
            
            viewModel.SearchCommandExecute().Wait();
            Assert.IsTrue(viewModel.Items.Where(u => u.Name.StartsWith("F")).Count() == 2);        }
    }
}

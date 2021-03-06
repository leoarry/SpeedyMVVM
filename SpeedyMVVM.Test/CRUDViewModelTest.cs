﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.TestModel.Models;
using SpeedyMVVM.TestModel;
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
            var viewModel = new CrudViewModel<UserModel>();

            viewModel.AddCommand.Execute(userToAdd);

            var retrievedUser = viewModel.Items.Where(u => u.Name == userToAdd.Name).FirstOrDefault();

            Assert.AreEqual(userToAdd, retrievedUser);
        }

        [TestMethod]
        public void RemoveEntity_Test()
        {
            var userToRemove = new UserModel { Name = "Donald", Surname = "Duck", AccessLevel = 0 };
            var viewModel = new CrudViewModel<UserModel>();
            
            viewModel.SelectedItem = userToRemove;
            viewModel.Items.Add(viewModel.SelectedItem);
            viewModel.RemoveCommand.Execute(null);
            
            Assert.IsTrue(viewModel.Items.Where(u => u.Name == "Donald").Count()==0);
        }

        [TestMethod]
        public void Search_Test()
        {
            var viewModel = new CrudViewModel<UserModel>();
            var listOfUsers = new List<UserModel>
            {
                new UserModel {Name="Donald", Surname="Duck" },
                new UserModel {Name="Foxy", Surname="Lady" },
                new UserModel {Name="Peter", Surname="Griffin" },
                new UserModel {Name="Fernanda", Surname="Lopez" }
            };

            viewModel.Items = listOfUsers.ToObservableCollection();
            viewModel.Filter.HiddenExpression = u => u.Name.StartsWith("F");
            
            viewModel.SearchCommand.Execute();
            Assert.IsTrue(viewModel.Items.Where(u => u.Name.StartsWith("F")).Count() == 2);        }
    }
}

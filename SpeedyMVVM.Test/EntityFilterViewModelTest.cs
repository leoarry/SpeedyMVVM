using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.DataAccess;
using SpeedyMVVM.TestModel.Models;
using SpeedyMVVM.Expressions;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Linq.Expressions;

namespace SpeedyMVVM.Test
{
    /// <summary>
    /// Descrizione del riepilogo per EntityFilterViewModel_Test
    /// </summary>
    [TestClass]
    public class EntityFilterViewModelTest
    {
        [TestMethod]
        public void GetPredicateTest()
        {
            var Filters = new ObservableCollection<ExpressionModel>();
            Filters.Add(new ExpressionModel
            {
                PropertyName = nameof(UserModel.Name),
                Operator = ExpressionOperatorEnum.StartsWith,
                Value = "P"
            });
            Filters.Add(new ExpressionModel
            {
                PropertyName = nameof(UserModel.Name),
                Operator = ExpressionOperatorEnum.StartsWith,
                Value = "F",
                ConcatOperator = ExpressionConcatEnum.Or
            });

            Expression<Func<UserModel, bool>> comparingExpression = item => item.Name.StartsWith("P") || item.Name.StartsWith("F");

            Assert.AreEqual(comparingExpression.ToString(), ExpressionBuilder.GetExpression<UserModel>(Filters).ToString());
        }
        
        [TestMethod]
        public void Search_Test()
        {
            var filter = new EntityFilterViewModel<UserModel>();
            filter.Items = new List<UserModel>
            {
                new UserModel {Name="Donald", Surname="Duck" },
                new UserModel {Name="Foxy", Surname="Lady" },
                new UserModel {Name="Peter", Surname="Griffin" },
                new UserModel {Name="Fernanda", Surname="Lopez" }
            }.ToObservableCollection();

            filter.Filters.Add(new ExpressionModel
            {
                PropertyName = nameof(UserModel.Name),
                Operator = ExpressionOperatorEnum.StartsWith,
                Value = "P"
            });
            filter.Filters.Add( new ExpressionModel
            {
                PropertyName=nameof(UserModel.Name),
                Operator= ExpressionOperatorEnum.StartsWith,
                Value="F",
                ConcatOperator = ExpressionConcatEnum.Or
            });
            filter.FilterCommandExecute().Wait();
            Assert.IsTrue(filter.Items.Count == 3);
        }
    }
}

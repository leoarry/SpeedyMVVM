using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.Validation;
using System.Linq;
using SpeedyMVVM.DataAccess;
using System.Threading;
using System.Linq.Expressions;

namespace SpeedyMVVM.Test
{
    /// <summary>
    /// Summary description for ValidatorTest
    /// </summary>
    [TestClass]
    public class ValidatorTest
    {
        public class TestClass : EntityBase
        {
            private string _StringProp;
            public string StringProp
            {
                get { return _StringProp; }
                set
                {
                    if (_StringProp != value)
                    {
                        _StringProp = value;
                        OnPropertyChanged(nameof(StringProp));
                    }
                }
            }

            private int _IntProp;
            public int IntProp
            {
                get { return _IntProp; }
                set
                {
                    if (_IntProp != value)
                    {
                        _IntProp = value;
                        OnPropertyChanged(nameof(IntProp));
                    }
                }
            }
            public int? RequiredProp { get; set; }

            private TestClass _NestedTestClass;
            public TestClass NestedTestClass
            {
                get { return _NestedTestClass ?? (_NestedTestClass = new TestClass()); }
                set
                {
                    if (_NestedTestClass != value)
                    {
                        _NestedTestClass = value;
                        OnPropertyChanged(nameof(NestedTestClass));
                    }
                }
            }

            public object TestClassFunction() { return new object(); }
            
        }

        public ValidatorTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestValidationRule()
        {
            var obj = new TestClass();
            //obj.GetValidator().Property<TestClass>(p => p.StringProp).IsRequired().HasMaxLength(2);
            var val = obj.GetAsyncValidator();
            //obj.Property(p => p.StringProp).IsRequired().HasMaxLength(2);
            val.Property(p => p.RequiredProp).IsRequired().IsGreaterThan(1);
            val.Property(p => p.NestedTestClass).IsRequired();
            val.Property(p => p.StringProp).IsRequired();
            //obj.Property(p => p.NestedTestClass.RequiredProp).IsRequired().IsGreaterThan(1).AddRuleAction(tc=> tc.StringProp!= "bellaaaaa");
            obj.RequiredProp = 0;
            //obj.NestedTestClass.IntProp = 2;
            //obj.StringProp = "l";
            //obj.IntProp = 6;
            obj.Validate();
            Thread.Sleep(1000);
            string msg=", ";
            foreach (string s in obj.GetErrors(nameof(obj.StringProp)))
                msg += string.Concat(s,", ");
            foreach (string s in obj.GetErrors(nameof(obj.RequiredProp)))
                msg += string.Concat(s, ", ");
            foreach (string s in obj.GetErrors("NestedTestClass"))
                msg += string.Concat(s, ", ");

            Assert.IsFalse(obj.HasErrors, string.Concat(obj.HasErrors.ToString(), msg));
            
        }

        [TestMethod]
        public void GetFullMemberNameTest()
        {
            int prova = 2;

            Expression<Func<TestClass,object>> f = tc => tc.StringProp == "TEST" && tc.IntProp>prova;

            string result = NameReaderExtensions.GetFullMemberName(f);

            Assert.Fail(result);
        }

        [TestMethod]
        public void TestValidationCrudViewModel()
        {
            var vm = new CrudViewModel<TestClass>();
            //vm.ValidationRules = new List<ValidationRule<TestClass>>();
            vm.Validator.Property(p=> p.StringProp).IsRequired().HasMaxLength(2);
            vm.Validator.Property(p => p.IntProp).IsGreaterThan(2);

            var obj = new TestClass();

            vm.SelectedItem = obj;
            vm.SelectedItem.StringProp = "l";
            vm.SelectedItem.IntProp = 3;

            Thread.Sleep(1000);
            string msg = ", ";
            foreach (string s in vm.SelectedItem.GetErrors(nameof(vm.SelectedItem.StringProp)))
                msg += string.Concat(s, ", ");
            foreach (string s in vm.SelectedItem.GetErrors(nameof(vm.SelectedItem.IntProp)))
                msg += string.Concat(s, ", ");

            msg = string.Format("{0}, Is valid: {1}", msg, vm.IsValid.ToString());

            Assert.IsFalse(vm.SelectedItem.HasErrors, msg);
        }

        [TestMethod]
        public void TestValidationCrudViewModelMultiItems()
        {
            var vm = new CrudViewModel<TestClass>();
            var obj1 = new TestClass();
            var obj2 = new TestClass();
            //Set validations
            vm.Validator.Property(p => p.StringProp).IsRequired().HasMaxLength(2);
            vm.Validator.Property(p => p.IntProp).IsGreaterThan(2);
            //Add Items to VM
            vm.Items.Add(obj1);
            vm.Items.Add(obj2);

            vm.SelectedItem = obj1;
            //vm.SelectedItem.StringProp = "object 1";
            //vm.SelectedItem.IntProp = 5;

            vm.SelectedItem = obj2;
            vm.SelectedItem.StringProp = "o2";
            vm.SelectedItem.IntProp = 3;

            Thread.Sleep(4000);
            string msg = ", OBJ1: ";
            foreach (string s in obj1.GetErrors(nameof(TestClass.StringProp)))
                msg += string.Concat(s, ", ");
            foreach (string s in obj1.GetErrors(nameof(TestClass.IntProp)))
                msg += string.Concat(s, ", ");

            msg += ", OBJ2: ";
            foreach (string s in obj2.GetErrors(nameof(TestClass.StringProp)))
                msg += string.Concat(s, ", ");
            foreach (string s in obj2.GetErrors(nameof(TestClass.IntProp)))
                msg += string.Concat(s, ", ");

            msg = string.Format("{0}, Is valid: {1}", msg, vm.IsValid.ToString());

            Assert.IsTrue(vm.IsValid, msg);
        }
    }
}

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Test
{
    [TestClass]
    public class RelayCommandTest
    {
        [TestMethod]
        public void RelayCommandCombine_ReturnFirstAction()
        {
            int result = 0;
            var cmd = new RelayCommand<int>((x) => result = result + x);
            cmd.CombineAction((x) => result=result-x);

            cmd.Execute(10);

            Assert.IsTrue(result == 0, string.Format("result = {0}", result));
        }
    }
}

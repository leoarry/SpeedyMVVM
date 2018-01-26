using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Test
{
    [TestClass]
    public class MessengerTest
    {
        public class myClass
        {
            public int number { get; set; }
            public string text { get; set; }

            public void TestMsg(myMessage message)
            {
                text = message.message;
            }
        }

        public class myMessage
        {
            public myClass sender { get; set; }
            public string message { get; set; }
            public int number { get; set; }
        }

        [TestMethod]
        public void SendMessageBetweenRegisteredObjects()
        {
            var msg = new Messenger();
            var result = "changed!";

            myClass c1 = new myClass { number = 1, text = "c1" };
            myClass c2 = new myClass { number = 2, text = "c2" };
            msg.Register<myMessage>(c1.TestMsg);
            msg.Register<myMessage>(c2.TestMsg);
            msg.Send(c1, new myMessage { sender = c1, message = result });

            Assert.IsTrue(c2.text == result && c1.text!=result, c1.text + " " + c2.text);
        }

        [TestMethod]
        public void SendMessageBetweenRegisteredObjectsUsingTargetCondition()
        {
            var msg = new Messenger();
            var result = "changed!";

            myClass c1 = new myClass { number = 1, text = "c1" };
            myClass c2 = new myClass { number = 2, text = "c2" };
            myClass c3 = new myClass { number = 3, text = "c3" };
            msg.Register<myMessage>(c1.TestMsg, 1);
            msg.Register<myMessage>(c2.TestMsg, 3);
            msg.Register<myMessage>(c2.TestMsg, c1);

            msg.Send(c1, new myMessage { sender = c1, message = result, number = 1 },true , (m,tkn)=> int.Parse(tkn.ToString()) == m.number);

            Assert.IsTrue(c1.text == result && c2.text != result && c3.text != result, c1.text + " " + c2.text + " " + c3.text);
        }

        
    }
}

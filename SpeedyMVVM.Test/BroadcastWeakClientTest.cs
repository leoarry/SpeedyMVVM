using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.Services;
using SpeedyMVVM.Utilities;

namespace SpeedyMVVM.Test
{
    [TestClass]
    public class BroadcastWeakClientTest
    {

        class BroadcastCallBack : IBroadcastServiceCallBack
        {
            public string Message;
            
            public void NotifyClientMessage(string userName, object sender, object message, object[] clientsToNotify = null)
            {
                throw new NotImplementedException();
            }

            public void NotifyClientMessage(AppToken proxyToken, object sender, object message, object[] clientsToNotify)
            {
                Message = ((myMessage)message).message;
            }

            public void NotifyClientTextMessage(string senderUser, string message)
            {
                throw new NotImplementedException();
            }

            public void NotifyOnlineUsersChanged(string server, List<string> users)
            {
                throw new NotImplementedException();
            }

            public void NotifyServiceMessage(HostNotification notification, object args = null)
            {
                throw new NotImplementedException();
            }

            public void NotifyServiceMessage(string proxy, int faultCode, string message = "", object args = null)
            {
                throw new NotImplementedException();
            }
        }

        class myMessage
        {
            public string message;
        }


        [TestMethod]
        public void TestSendMessage()
        {
            var message = new myMessage { message = "message" };
            var callBack = new BroadcastCallBack();
            var weakClient = new BroadcastWeakClient("client1");

            weakClient.AddMessage(typeof(myMessage),"message");
            
            Assert.IsTrue(weakClient.ContainsMessageOfType(message.GetType()));
        }
    }
}

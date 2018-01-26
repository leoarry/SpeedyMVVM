using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedyMVVM.Utilities;
using System.Threading;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace SpeedyMVVM.Test
{
    [TestClass]
    public class WeakEventManagerTest
    {
        private class testEventArgs : EventArgs
        {
            public object Result;

            public testEventArgs(object result)
            {
                Result = result;
            }
        }

        private class testClass : ObservableObject
        {
            public bool PropertyChangedHandled { get; set; }
            public event EventHandler<testEventArgs> ResultChanged
            {
                add
                {
                    WeakEventManager.Default.AddEventHandler(this, nameof(ResultChanged), value);
                }
                remove
                {
                    WeakEventManager.Default.RemoveEventHandler(this, nameof(ResultChanged), value);
                }
            }

            int _number;
            public int number
            {
                get { return _number; }
                set
                {
                    SetProperty(ref _number, value);
                    WeakEventManager.Default.RaiseEvent(this, new testEventArgs(_number), nameof(ResultChanged));
                }
            }
            string _message;
            public string message
            {
                get { return _message; }
                set
                {
                    SetProperty(ref _message, value);
                    WeakEventManager.Default.RaiseEvent(this, new testEventArgs(_message), nameof(ResultChanged));
                }
            }

            public void HandleEvent(object sender, testEventArgs args)
            {
                if (args.Result.GetType() == typeof(string))
                {
                    _message = args.Result.ToString();
                }
                if (args.Result.GetType() == typeof(int))
                {
                    _number = (int)args.Result;
                }
            }

            public void HandlePropertyChangedEvent(object sender, PropertyChangedEventArgs e)
            {
                PropertyChangedHandled = true;
            }

            public testClass()
            {
            }
            public testClass(int result)
            {
                _number = result;
            }
            ~testClass()
            {
                WeakEventManager.Default.RemoveSource(this);
            }


        }
        [TestMethod]
        public void TestResultChanged()
        {
            var t = new testClass(10);
            var result = 0;

            t.ResultChanged += (obj, args) => result = t.number;

            t.number = 5;

            Assert.AreEqual(t.number, result);
        }

        [TestMethod]
        public void TestAddRemoveListeners()
        {
            WeakEventManager.ResetDefault();
            string result = "";
            var source = new testClass();
            var listenerA = new testClass();
            var listenerB = new testClass();

            source.ResultChanged += listenerA.HandleEvent;
            source.ResultChanged += listenerB.HandleEvent;

            result = string.Format("{0}Source added : {1}, ", result, WeakEventManager.Default.ContainsSource(source));
            result = string.Format("{0}ListenerA added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerB));

            source.number = 5;

            source.ResultChanged -= listenerA.HandleEvent;
            source.ResultChanged -= listenerB.HandleEvent;

            result = string.Format("{0}ListenerA removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerB));

            result = string.Format("{0}source = {1}, listenerA = {2}, listenerB = {3}", result, source.number, listenerA.number, listenerB.number);

            Assert.Fail(result);
        }

        [TestMethod]
        public void TestAddRemoveListenersExternally()
        {
            WeakEventManager.ResetDefault();
            string result = "";
            var source = new testClass();
            var listenerA = new testClass();
            var listenerB = new testClass();

            WeakEventManager.Default.AddEventHandler<testClass, testEventArgs>(source, nameof(source.ResultChanged), listenerA.HandleEvent);
            WeakEventManager.Default.AddEventHandler<testClass, testEventArgs>(source, nameof(source.ResultChanged), listenerB.HandleEvent);

            result = string.Format("{0}Source added : {1}, ", result, WeakEventManager.Default.ContainsSource(source));
            result = string.Format("{0}ListenerA added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerB));

            source.number = 5;

            WeakEventManager.Default.RemoveSource(source);
            WeakEventManager.Default.RemoveListener<testClass>(listenerA, nameof(testClass.ResultChanged));
            WeakEventManager.Default.RemoveListener(listenerB);

            result = string.Format("{0}Source removed : {1}, ", result, !WeakEventManager.Default.ContainsSource(source));
            result = string.Format("{0}ListenerA removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerB));

            result = string.Format("{0}source = {1}, listenerA = {2}, listenerB = {3}", result, source.number, listenerA.number, listenerB.number);

            Assert.Fail(result);
        }

        [TestMethod]
        public void TestAddRemoveMultiSourcesAndListeners()
        {
            WeakEventManager.ResetDefault();
            string result = "";
            var sourceA = new testClass();
            var sourceB = new testClass();
            var listenerA = new testClass();
            var listenerB = new testClass();

            WeakEventManager.Default.AddEventHandler<testClass, testEventArgs>(sourceA, nameof(sourceA.ResultChanged), listenerA.HandleEvent);
            WeakEventManager.Default.AddEventHandler<testClass, testEventArgs>(sourceB, nameof(sourceB.ResultChanged), listenerA.HandleEvent);

            WeakEventManager.Default.AddEventHandler<testClass, testEventArgs>(sourceA, nameof(sourceA.ResultChanged), listenerB.HandleEvent);

            result = string.Format("{0}SourceA added : {1}, ", result, WeakEventManager.Default.ContainsSource(sourceA));
            result = string.Format("{0}ListenerA added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB added : {1}, ", result, WeakEventManager.Default.ContainsListener(listenerB));

            sourceA.number = 5;

            WeakEventManager.Default.RemoveEventHandler<testClass, testEventArgs>(sourceA, nameof(sourceA.ResultChanged), listenerB.HandleEvent);
            WeakEventManager.Default.RemoveListener(listenerA);

            result = string.Format("{0}SourceA removed : {1}, ", result, !WeakEventManager.Default.ContainsSource(sourceA));
            result = string.Format("{0}ListenerA removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerA));
            result = string.Format("{0}ListenerB removed : {1}, ", result, !WeakEventManager.Default.ContainsListener(listenerB));

            result = string.Format("{0}source = {1}, listenerA = {2}, listenerB = {3}", result, sourceA.number, listenerA.number, listenerB.number);

            Assert.Fail(result);
        }

        [TestMethod]
        public void TestPropertyChanged()
        {
            WeakEventManager.ResetDefault();
            var source = new testClass();
            var listenerA = new testClass();
            var listenerB = new testClass();

            source.PropertyChanged += listenerA.HandlePropertyChangedEvent;
            source.PropertyChanged += listenerB.HandlePropertyChangedEvent;
            
            source.number = 5;

            Assert.IsTrue(listenerA.PropertyChangedHandled && listenerB.PropertyChangedHandled);
        }
        
    }
}

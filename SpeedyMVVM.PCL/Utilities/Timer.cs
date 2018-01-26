using SpeedyMVVM.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SpeedyMVVM.Utilities
{
    public delegate void TimerCallBack(object state);

    public class Timer : CancellationTokenSource
    {
        #region Fields
        private bool _IsRunning;
        private int _WaitTime;
        private TimerCallBack callBack;
        #endregion

        #region Properties
        public int WaitTime
        {
            get { return _WaitTime; }
            set { _WaitTime = value; }
        }

        public bool IsRunning
        {
            get { return _IsRunning; }
        }
        #endregion
        
        

        public async Task Start()
        {
            _IsRunning = true;
            while (IsRunning)
            {
                await Task.Factory.StartNew(async (obj) =>
                {
                    var tuple = (Tuple<TimerCallBack, Timer, int, bool>)obj;
                    while (true)
                    {
                        if (IsCancellationRequested)
                            break;
                        tuple.Item1?.Invoke(tuple.Item2);
                        await Task.Delay(tuple.Item3);
                    }
                },Tuple.Create(callBack, this, _WaitTime, IsRunning));
                _IsRunning = false;
            }
        }

        public void Stop()
        {
            _IsRunning = false;
        }

        public Timer(int waitTime)
        {
            WaitTime = waitTime;
        }

    }

    internal sealed class Timer3 : CancellationTokenSource
    {
        internal Timer3(Action<object> callback, object state, int millisecondsDueTime, int millisecondsPeriod, bool waitForCallbackBeforeNextPeriod = false)
        {
            //Contract.Assert(period == -1, "This stub implementation only supports dueTime.");

            Task.Delay(millisecondsDueTime, Token).ContinueWith(async (t, s) =>
            {
                var tuple = (Tuple<Action<object>, object>)s;

                while (!IsCancellationRequested)
                {
                    if (waitForCallbackBeforeNextPeriod)
                        tuple.Item1(tuple.Item2);
                    else
                        Task.Run(() => tuple.Item1(tuple.Item2));

                    await Task.Delay(millisecondsPeriod, Token).ConfigureAwait(false);
                }

            }, Tuple.Create(callback, state), CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnRanToCompletion, TaskScheduler.Default);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Cancel();

            base.Dispose(disposing);
        }
    }
}

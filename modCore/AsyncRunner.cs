using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace modCore
{
	class AsyncRunner : IDisposable
	{
        public ManualResetEvent resetEvent;
        public List<Action> actions;
        public List<Action> Actions {
            get {
                lock (actions) {
                    return actions;
                }
            }
        }
        public bool run;
        public string threadName;
        public List<Action> _currentActions;
        private Thread thread;
        int x = 0;

        public AsyncRunner(string _name) {
            threadName = _name;
            resetEvent = new ManualResetEvent(false);
            actions = new List<Action>();
            _currentActions = new List<Action>();
            run = true;
            thread = new Thread(new ThreadStart(Run));
            thread.Start();
        }

        public void AddAsyncTask(Action e) {
            lock (actions) {
                actions.Add(e);
                //resetEvent.Set();
            }
        }

        public void Run() {
            while (run) {
                resetEvent.WaitOne(50);
                try {
                    if (actions.Count > 0) {
                        lock (actions) {
                            _currentActions.Clear();
                            _currentActions.AddRange(actions);
                            actions.Clear();
                        }

                        for (int i = 0; i < _currentActions.Count; i++) {
                            try {
                                _currentActions[i]();
                                _currentActions[i] = null;
                                //ConsoleWpr.LogDebug(threadName + ": function Called.");
                            }
                            catch (Exception e) {
                                ConsoleWpr.LogError("message: " + e.Message + ", thread: " + threadName);
                                _currentActions[i] = null;
                            }
                        }
                    }
                }
                catch (Exception e) {
                    ConsoleWpr.LogError("\nMessage: " + e.Message + "\nFunction: Run\nThread: " + threadName);
                }
                resetEvent.Reset();
            }
        }

        public void Dispose() {
            ConsoleWpr.LogDebug("Dispose called in thread " + threadName);
            run = false;
            thread.Abort();
            for (int i = 0; i < actions.Count; i++) {
                //Actions[i] = null;
            }
            Actions.Clear();
            actions = null;
        }
	}
}

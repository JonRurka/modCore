using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Linq;

namespace modCore {
    public class Loom : MonoBehaviour {
        public static int maxThreads = 8;
        static int numThreads;

        private static Loom _current;
        private int _count;
        public static Loom Current {
            get {
                Initialize();
                return _current;
            }
        }

        void Awake() {
            _current = this;
            initialized = true;
        }

        static bool initialized;

        static void Initialize() {
            if (!initialized) {

                if (!Application.isPlaying)
                    return;
                initialized = true;
                var g = new GameObject("Loom");
                _current = g.AddComponent<Loom>();
            }

        }

        private List<Action> _actions = new List<Action>();
        private Dictionary<string, AsyncRunner> _AsynAction = new Dictionary<string, AsyncRunner>();

        public struct DelayedQueueItem {
            public float time;
            public Action action;
        }

        private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

        List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

        public static void QueueOnMainThread(Action action) {
            QueueOnMainThread(action, 0f);
        }

        public static void QueueOnMainThread(Action action, float time) {
            if (time != 0) {
                if (Current._delayed != null) {
                    lock (Current._delayed) {
                        Current._delayed.Add(new DelayedQueueItem { time = Time.time + time, action = action });
                    }
                }
            }
            else {
                if (Current._actions != null) {
                    lock (Current._actions) {
                        Current._actions.Add(action);
                    }
                }
            }
        }

        public static void AddAsyncThread(string thread) {
            if (Current._AsynAction != null){
                lock (Current._AsynAction) {
                    try {
                        if (!Current._AsynAction.ContainsKey(thread)) {
                            //ConsoleWpr.LogDebug("Created thread: " + thread);
                            AsyncRunner _runner = new AsyncRunner(thread);
                            Current._AsynAction.Add(thread, _runner);
                        }
                    }
                    catch (Exception e) {
                        ConsoleWpr.LogError("\nMessage: " + e.Message + "\nFunction: AddAsyncThread\nThread: " + thread);
                    }
                }
            }
        }

        public static void QueueAsyncTask(string thread, Action e) {
            lock (Current._AsynAction) {
                try {
                    if (Current._AsynAction.ContainsKey(thread)) {
                        Current._AsynAction[thread].AddAsyncTask(e);
                    }
                    else
                        ConsoleWpr.LogError("failed to locate thread " + thread);
                }
                catch (Exception ex) {
                    ConsoleWpr.LogError("\nMessage: " + ex.Message + "\nFunction: QueueAsyncTask\nThread: " + thread);
                }
            }
        }

        public static bool ThreadExists(string thread) {
            return Current._AsynAction.ContainsKey(thread);
        }

        public static Thread RunAsync(Action a) {
            Initialize();
            while (numThreads >= maxThreads) {
                Thread.Sleep(1);
            }
            Interlocked.Increment(ref numThreads);
            ThreadPool.QueueUserWorkItem(RunAction, a);
            a = null;
            return null;
        }

        private static void RunAction(object action) {
            try {
                ((Action)action)();
            }
            catch {
            }
            finally {
                Interlocked.Decrement(ref numThreads);
            }

        }

        void OnDisable() {
            if (_current == this) {

                _current = null;
            }
        }

        List<Action> _currentActions = new List<Action>();

        // Update is called once per frame
        void Update() {
            lock (_actions) {
                _currentActions.Clear();
                _currentActions.AddRange(_actions);
                _actions.Clear();
            }
            for (int i = 0; i < _currentActions.Count; i++) {
                _currentActions[i]();
                _currentActions[i] = null;
            }

            if (Input.GetKey(KeyCode.Alpha5)) {
                foreach (string thread in _AsynAction.Keys) {
                    if (_AsynAction[thread].Actions.Count > 0) {
                        CoreConsole.LogDebug(_AsynAction[thread].threadName + ": functions detected: " + _AsynAction[thread].Actions.Count + ", " + _AsynAction[thread]._currentActions.Count);
                    }
                }
            }

            lock (_delayed) {
                _currentDelayed.Clear();
                _currentDelayed.AddRange(_delayed.Where(d => d.time <= Time.time));
                foreach (var item in _currentDelayed)
                    _delayed.Remove(item);
            }
            foreach (var delayed in _currentDelayed) {
                delayed.action();
            }
        }

        void OnApplicationQuit() {
            if (_actions != null) {
                _actions.Clear();
                _actions = null;
            }
            if (_AsynAction != null) {
                foreach (AsyncRunner runner in _AsynAction.Values) {
                    runner.Dispose();
                }
                _AsynAction.Clear();
                _AsynAction = null;
            }
            if (_currentActions != null) {
                _currentActions.Clear();
                _currentActions = null;
            }
            if (_delayed != null) {
                _delayed.Clear();
                _delayed = null;
            }
            if (_currentDelayed != null) {
                _currentDelayed.Clear();
                _currentDelayed = null;
            }
        }
    }
}
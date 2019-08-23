using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TomskGO.Managers
{
    /// <summary>
    /// Especial structure for the app-related tasks
    /// </summary>
    class AppTask
    {
        /// <summary>
        /// Actual method to execute
        /// </summary>
        private Action Action { get; set; }
        /// <summary>
        /// Cancellation companion
        /// </summary>
        private CancellationTokenSource TokenSource { get; set; }
        /// <summary>
        /// Determines, whether task is repeatable (running in a loop)
        /// </summary>
        public bool IsRepeatable { get; set; }
        /// <summary>
        /// Determines, whether task is currently running
        /// </summary>
        public bool IsRunning { get; set; }
        /// <summary>
        /// Determines, whether task should be executed in UI thread, or in a background
        /// </summary>
        public bool IsUIRelated { get; set; }
        /// <summary>
        /// Task ID
        /// </summary>
        public uint ID { get; set; }

        public AppTask(Action action, CancellationTokenSource tokenSource, uint id, bool isRepeatable = false, bool isUIRelated = false)
        {
            ID = id;
            Action = action;
            IsRepeatable = isRepeatable;
            IsUIRelated = isUIRelated;
            TokenSource = tokenSource;
        }

        /// <summary>
        /// Starts the task's execution
        /// </summary>
        public void RunTask()
        {
            TokenSource = new CancellationTokenSource();
            if (IsRepeatable)
            {
                IsRunning = true;
                Device.BeginInvokeOnMainThread(() => Device.StartTimer(TimeSpan.FromSeconds(5), TimerFunc));
            }
            else
            {
                Task.Run(() =>
                 {
                     if (IsUIRelated)
                     {
                         Device.BeginInvokeOnMainThread(() =>
                         {
                             IsRunning = true;
                             Action.Invoke();
                             IsRunning = false;
                         });
                     }
                     else
                     {
                         IsRunning = true;
                         Action.Invoke();
                         IsRunning = false;
                     }
                 }, TokenSource.Token).ContinueWith((t) => TaskManager.Instance.UnregisterTask(ID));
            }
        }

        /// <summary>
        /// Loop-related method
        /// </summary>
        /// <returns>If "True" - execution continues within the loop, if "False" - execution stops</returns>
        private bool TimerFunc()
        {
            if (!IsRunning)
            {
                TaskManager.Instance.UnregisterTask(ID);
                return false;
            }

            Task.Run(() =>
            {
                if (IsUIRelated)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Action.Invoke();
                    });
                }
                else
                {
                    Action.Invoke();
                }
            }, TokenSource.Token);

            return IsRunning;
        }

        /// <summary>
        /// Stop the current task
        /// </summary>
        public void StopTask()
        {
            TokenSource?.Cancel();
            IsRunning = false;
        }
    }

    /// <summary>
    /// Enumeration, which helps determine, what kind of tasks should be paused
    /// </summary>
    enum PauseKind
    {
        IfRepeating,
        IfUIRelated,
        None
    }

    /// <summary>
    /// Task manager, which has all the abilities to operate the tasks
    /// </summary>
    class TaskManager
    {
        #region Singleton
        private static readonly Lazy<TaskManager> _instance = new Lazy<TaskManager>(() => new TaskManager());

        public static TaskManager Instance => _instance.Value;

        private TaskManager() { }
        #endregion

        private readonly object _lockObj = new object();

        /// <summary>
        /// Global index with limitation of number tasks equals to the limit of unsigned integer type
        /// </summary>
        private uint GlobalIndex { get; set; } = 0;
        /// <summary>
        /// Actual tasks container
        /// </summary>
        private Dictionary<uint, AppTask> Tasks { get; set; } = new Dictionary<uint, AppTask>();

        /// <summary>
        /// Currently running tasks
        /// </summary>
        private List<uint> RunningTasks { get; set; } = new List<uint>();
        /// <summary>
        /// Currently paused tasks
        /// </summary>
        private List<uint> PausedTasks { get; set; } = new List<uint>();

        /// <summary>
        /// Registers task and adds it to the queue
        /// </summary>
        /// <param name="task">Action delegate</param>
        /// <param name="runImmediately">Should be run immediately</param>
        /// <param name="isRepeatable">Should be in a loop</param>
        /// <param name="isUIRelated">Should be executed in the UI thread</param>
        /// <returns></returns>
        public uint RegisterTask(Action task, bool runImmediately = false, bool isRepeatable = false, bool isUIRelated = false)
        {
            lock (_lockObj)
            {
                var currentTaskIndex = GlobalIndex;
                var tokenSource = new CancellationTokenSource();
                var localTask = new AppTask(task, tokenSource, currentTaskIndex, isRepeatable, isUIRelated);

                Tasks.Add(currentTaskIndex, localTask);

                if (runImmediately)
                {
                    RunTask(currentTaskIndex);
                }

                GlobalIndex++;

                return currentTaskIndex;
            }
        }

        /// <summary>
        /// Runs task
        /// </summary>
        /// <param name="id">Task ID</param>
        public void RunTask(uint id)
        {
            if (Tasks.ContainsKey(id))
            {
                Tasks[id].RunTask();
                RunningTasks.Add(id);
            }
        }
        
        /// <summary>
        /// Restarts task (effective for the repeatable tasks)
        /// </summary>
        /// <param name="id">Task ID</param>
        public void RestartTask(uint id)
        {
            StopTask(id);
            RunTask(id);
        }
        
        /// <summary>
        /// Stops task
        /// </summary>
        /// <param name="id">Task ID</param>
        public void StopTask(uint id)
        {
            lock (_lockObj)
            {
                if (Tasks.ContainsKey(id)) Tasks[id].StopTask();
                if (RunningTasks.Contains(id)) RunningTasks.Remove(id);
                if (PausedTasks.Contains(id)) PausedTasks.Remove(id);
            }
        }

        /// <summary>
        /// Stops all tasks in queue
        /// </summary>
        public void StopAllTasks()
        {
            for(uint id = 0; id < Tasks.Count; id++)
            {
                if (!Tasks.ContainsKey(id)) continue;
                StopTask(id);
            }
        }

        /// <summary>
        /// Pauses tasks
        /// </summary>
        /// <param name="stopKind">Task kind</param>
        public void PauseAllRunningTasks(PauseKind stopKind = PauseKind.None)
        {
            for(int i = 0; i < RunningTasks.Count; i++)
            {
                var index = RunningTasks[i];
                if (Tasks.ContainsKey(index))
                {
                    var task = Tasks[index];
                    if (task.IsRepeatable && stopKind == PauseKind.IfRepeating) task.StopTask();
                    if (task.IsUIRelated && stopKind == PauseKind.IfUIRelated) task.StopTask();
                    if (stopKind == PauseKind.None) task.StopTask();
                    RunningTasks.RemoveAt(i);
                    PausedTasks.Add(index);
                }
            }
        }

        /// <summary>
        /// Resumes tasks
        /// </summary>
        public void RunAllPausedTasks()
        {
            for (int i = 0; i < PausedTasks.Count; i++)
            {
                var index = PausedTasks[i];
                if (Tasks.ContainsKey(index))
                {
                    var task = Tasks[index];
                    task.RunTask();
                    PausedTasks.RemoveAt(i);
                    RunningTasks.Add(index);
                }
            }
        }

        /// <summary>
        /// Unregisters task and removes it from the queue
        /// </summary>
        /// <param name="id"></param>
        public void UnregisterTask(uint id)
        {
            if (Tasks.ContainsKey(id))
            {
                StopTask(id);
                Tasks[id] = null;
                Tasks.Remove(id);
            }
        }
    }
}
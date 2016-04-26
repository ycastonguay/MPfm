// Copyright � 2011-2013 Yanick Castonguay
//
// This file is part of Sessions.
//
// Sessions is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// Sessions is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Sessions. If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Sessions.Core.Tasks
{
    // The consumer service needs to have a generic report event, or a wrapper around it
    public class TaskServiceBase<TTask, TReport> where TTask : RequestTaskBase 
                                                 where TReport : RequestTaskProgressBase
    {
        public delegate void RequestServiceReportProgressDelegate();

        public const int SleepTimeBetweenLoopIterations = 50;
        public const int MaxNumberOfRequests = 20;
        #if MACOSX
        public const int MaximumNumberOfTasks = 1;
        #else
        public const int MaximumNumberOfTasks = 2;
        #endif

        private readonly object _lockerTasks = new object();
        private readonly TaskServiceHandler<TTask, TReport> _taskServiceHandler;
        private readonly List<TTask> Tasks;
        protected int NumberOfTasksRunning { get; private set; }

        public event RequestServiceReportProgressDelegate ReportProgress;

//        public event WaveFormRenderingService.GenerateWaveFormEventHandler GenerateWaveFormBitmapEndedEvent;
        public int Count { get { return Tasks.Count; } }

        public TaskServiceBase(TaskServiceHandler<TTask, TReport> taskServiceHandler)
        {
            Tasks = new List<TTask>(); // todo: use queue instead?
            _taskServiceHandler = taskServiceHandler;
            _taskServiceHandler.ReportProgress += RequestHandlerWrapper_ReportProgress;

            StartTaskProcessLoop();
        }

        private void RequestHandlerWrapper_ReportProgress(TReport report)
        {
            Console.WriteLine("[RequestServiceBase] - RequestHandlerWrapper_ReportProgress");
            lock (_lockerTasks)
            {
                NumberOfTasksRunning--;
            }

            if (ReportProgress != null)
            {
                ReportProgress();
            }
        }

        public void Flush()
        {
            lock (_lockerTasks)
            {
                Tasks.Clear();
            }
        }

        protected virtual bool ShouldAddTaskToList(TTask task)
        {
            return true;
        }

        protected virtual void CleanupTaskList()
        {
            // Remove the oldest request from the list if we hit the maximum 
            if(Tasks.Count > MaxNumberOfRequests)
                Tasks.RemoveAt(0);
        }

        public Task RequestTask(TTask task)
        {
            var taskToReturn = Task.Factory.StartNew(() =>
            {
                lock (_lockerTasks)
                {
                    bool addToList = ShouldAddTaskToList(task);
                    if(addToList)
                    {
                        Tasks.Add(task);
                        CleanupTaskList();                                

                        Console.WriteLine("[RequestServiceBase] - RequestTask - Pulsing...");
                        Monitor.Pulse(_lockerTasks);     
                    }
                }
            });
            return taskToReturn;
        }

        public void StartTaskProcessLoop()
        {
            var thread = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
//                    Console.WriteLine("[RequestServiceBase] - ProcessLoop - Requests.Count: {0} NumberOfTasksRunning: {1}", Requests.Count, NumberOfTasksRunning);
                    var tasksToProcess = new List<TTask>();
                    int taskCount = 0;
                    lock (_lockerTasks)
                    {
                        while (Tasks.Count > 0 && NumberOfTasksRunning < MaximumNumberOfTasks)
                        {
                            //int index = 0; // FIFO
                            int index = Tasks.Count - 1; // LIFO
                            NumberOfTasksRunning++;
                            var task = Tasks[index];
                            tasksToProcess.Add(task);
                            Tasks.RemoveAt(index);
                        }
                        taskCount = Tasks.Count;
                    }

                    foreach (var task in tasksToProcess)
                    {
//                        Console.WriteLine("[RequestServiceBase] - ProcessLoop - Processing request - NumberOfTasksRunning: {0}", NumberOfTasksRunning);
                        // ThreadQueueWorkItem will manage a thread pool
                        _taskServiceHandler.ProcessTask(task);
                    }

                    if (taskCount > 0)
                    {
                        Console.WriteLine("[RequestServiceBase] - ProcessLoop - Sleeping...");
                        Thread.Sleep(SleepTimeBetweenLoopIterations);
                    }
                    else
                    {
                        lock (_lockerTasks)
                        {
                            Console.WriteLine("[RequestServiceBase] - ProcessLoop - Waiting...");
                            Monitor.Wait(_lockerTasks);
                            Console.WriteLine("[RequestServiceBase] - ProcessLoop - Woken up!");
                        }
                    }
                }
            }));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }
    }

    public abstract class TaskServiceHandler<TTask, TReport> where TTask : RequestTaskBase 
                                                             where TReport : RequestTaskProgressBase
    {
        public delegate void ReportProgressDelegate(TReport report);
        public event ReportProgressDelegate ReportProgress;
        public abstract void ProcessTask(TTask task);
    }

    public class RequestTaskBase
    {
    }

    public class RequestTaskProgressBase
    {
    }

    //
    //
    //

    public class ExampleTask : RequestTaskBase
    {
    }

    public class ExampleTaskProgress : RequestTaskProgressBase
    {
    }

    public class ExampleTaskServiceHandler : TaskServiceHandler<ExampleTask, ExampleTaskProgress>
    {
        public override void ProcessTask(ExampleTask task)
        {
        }
    }

    public class ExampleTaskService : TaskServiceBase<ExampleTask, ExampleTaskProgress>
    {
        public ExampleTaskService(TaskServiceHandler<ExampleTask, ExampleTaskProgress> taskServiceHandler) : base(taskServiceHandler)
        {
        }

        protected override bool ShouldAddTaskToList(ExampleTask task)
        {
            return base.ShouldAddTaskToList(task);
        }

        protected override void CleanupTaskList()
        {
            base.CleanupTaskList();
        }
    }
}
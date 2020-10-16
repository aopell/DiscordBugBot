using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordBugBot.Tools
{
    public class AsyncEventExecutor : IDisposable
    {
        private readonly CancellationTokenSource terminationSource = new CancellationTokenSource();
        private readonly Thread executorThread;

        private readonly ISet<Task> pendingTasks = new HashSet<Task>();
        // used as a mutex around pendingTasks
        private readonly SemaphoreSlim pendingTasksGuard = new SemaphoreSlim(1);
        // used as a task counter to ensure we don't try to wait on an empty set
        private readonly SemaphoreSlim pendingTasksCount = new SemaphoreSlim(0);

        // the invoking thread of this event handler is unspecified
        public event EventHandler<Task> TaskFaulted;

        public AsyncEventExecutor()
        {
            executorThread = new Thread(RunTask);
            executorThread.Start();
        }

        private void HandleCompletedTask(Task t)
        {
            // notify non-success
            if (!t.IsCompletedSuccessfully)
            {
                TaskFaulted?.Invoke(this, t);
            }
        }

        private void RunTask()
        {
            CancellationToken token = terminationSource.Token;

            while (!token.IsCancellationRequested)
            {
                Task<Task> whenDone;

                // ensure we will have a task ready to wait on
                // (since we are the only thread to remove from the set, this is okay)
                pendingTasksCount.Wait(token);

                pendingTasksGuard.Wait(token);
                try
                {
                    // assign the task in a lock, but do not await it in here
                    // most of the time is expected to be spent on the Wait
                    // WhenAny makes an internal copy, which is why we don't
                    // need a longer lock
                    whenDone = Task.WhenAny(pendingTasks);
                }
                finally
                {
                    pendingTasksGuard.Release();
                }

                // WhenAny is guaranteed not to fault, the inner task (however) may
                whenDone.Wait(token);
                Task completed = whenDone.Result;

                HandleCompletedTask(completed);

                // remove from wait queue
                pendingTasksGuard.Wait(token);
                try
                {
                    Debug.Assert(pendingTasks.Remove(completed), "Recently completed task unexpectedly not found in task queue");
                }
                finally
                {
                    pendingTasksGuard.Release();
                }
            }
        }

        public async Task Enqueue(Task t)
        {
            // don't bother going through waitqueue if we're complete already
            // if we complete relatively early after this if statement, that's okay
            // waitqueue logic should handle it okay
            if ((t ?? throw new ArgumentNullException(nameof(t))).IsCompleted)
            {
                HandleCompletedTask(t);
                return;
            }

            await pendingTasksGuard.WaitAsync().ConfigureAwait(false);
            try
            {
                if (!pendingTasks.Add(t))
                {
                    // exception will cause us not to release pendingTasksCount, which is good
                    // we only want to reach that line if the Add completes and actually adds a new item
                    throw new ArgumentException("The given task is already in this wait queue.");
                }
            }
            finally
            {
                pendingTasksGuard.Release();
            }

            // signal that a new task is available
            pendingTasksCount.Release();
        }

        /// <summary>
        /// Creates a delegate function returning a task which completes when the given underlying handler
        /// has been enqueued for handling on this <see cref="AsyncEventExecutor"/>.
        /// Intended for use as an event handling delegate directly in lieu of <paramref name="underlyingHandler"/>
        /// in situations where a blocking-task event handler is unacceptable.
        /// </summary>
        public Func<Task> CreateEventHandler(Func<Task> underlyingHandler)
        {
            return () => Enqueue(underlyingHandler());
        }

        /// <see cref="CreateEventHandler(Func{Task})"/>
        public Func<T1, Task> CreateEventHandler<T1>(Func<T1, Task> underlyingHandler)
        {
            return a => Enqueue(underlyingHandler(a));
        }

        /// <see cref="CreateEventHandler(Func{Task})"/>
        public Func<T1, T2, Task> CreateEventHandler<T1, T2>(Func<T1, T2, Task> underlyingHandler)
        {
            return (a, b) => Enqueue(underlyingHandler(a, b));
        }

        /// <see cref="CreateEventHandler(Func{Task})"/>
        public Func<T1, T2, T3, Task> CreateEventHandler<T1, T2, T3>(Func<T1, T2, T3, Task> underlyingHandler)
        {
            return (a, b, c) => Enqueue(underlyingHandler(a, b, c));
        }

        /// <see cref="CreateEventHandler(Func{Task})"/>
        public Func<T1, T2, T3, T4, Task> CreateEventHandler<T1, T2, T3, T4>(Func<T1, T2, T3, T4, Task> underlyingHandler)
        {
            return (a, b, c, d) => Enqueue(underlyingHandler(a, b, c, d));
        }

        public void Dispose()
        {
            terminationSource.Cancel();
            executorThread.Join();
            terminationSource.Dispose();
            pendingTasksGuard.Dispose();
        }
    }
}

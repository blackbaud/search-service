using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace QueryEngine.Utils.Threading
{
    public class WorkerTask
    {
        private bool _running;

        private readonly BlockingCollection<ITask> _requestQueue;

        public WorkerTask(BlockingCollection<ITask> queue)
        {
            _requestQueue = queue;
            _running = true;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Shutdown()
        {
            _running = false;
        }

        public void Execute()
        {
            Console.WriteLine("Starting thread '{0}' at {1}", Task.CurrentId, DateTime.Now);

            while (_running)
            {
                try
                {
                    if (_requestQueue.Count == 0)
                        Thread.Sleep(2000);

                    if (_requestQueue.IsAddingCompleted) continue;

                    var worker = _requestQueue.Take();
                    if (worker != null)
                    {
                        worker.Execute();
                    }
                }
                catch (InvalidOperationException exception)
                {
                    //Eat this exception - The requestQueue was marked for adding complete after the check was performed
                    //Console.WriteLine("Warning Only - {0}", exception.Message);
                }
            }
            Console.WriteLine("Task Processor shutdown complete - {0} TaskID({1})", DateTime.Now, Task.CurrentId);
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace QueryEngine.Utils.Threading
{
    public class TaskProcessor
    {
        private BlockingCollection<ITask> _requestQueue { get; set; }
        private readonly int _threadCount;
        private readonly Task[] _tasks;
        private readonly WorkerTask[] _workers;
        private readonly int _queueCapacity;
        private readonly CancellationTokenSource _cancellationToken;

        public TaskProcessor(int numberOfThreads, int queueCapacity)
        {
            _threadCount = numberOfThreads;
            _tasks = new Task[numberOfThreads];
            _workers = new WorkerTask[numberOfThreads];
            _queueCapacity = queueCapacity;
            _requestQueue = CreateBlockingQueue(queueCapacity);
            _cancellationToken = new CancellationTokenSource();
        }

        private static BlockingCollection<ITask> CreateBlockingQueue(int capacity)
        {
            return capacity > 0 ? new BlockingCollection<ITask>(capacity) : new BlockingCollection<ITask>();
        }

        public void Start()
        {
            for (var i = 0; i < _threadCount; i++)
            {
                var idx = i;
                _workers[idx] = new WorkerTask(_requestQueue);
                _tasks[idx] = new Task(() => _workers[idx].Execute());
                _tasks[idx].Start();
            }
        }

        public void Shutdown()
        {
            Console.WriteLine("Shutting down the Task Processor. Please Wait... - {0}", DateTime.Now);
            _requestQueue.CompleteAdding();
            for (var i = 0; i < _workers.Count(); i++)
            {
                var idx = i;
                _workers[idx].Shutdown();
            }
            Task.WaitAll(_tasks);
        }

        public void Enqueue(ITask task)
        {
            _requestQueue.Add(task);
        }

        public int GetCapacity()
        {
            return _queueCapacity;
        }

        public int GetQueueDepth()
        {
            return _requestQueue.Count;
        }
    }
}

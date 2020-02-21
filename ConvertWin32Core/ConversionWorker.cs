using Swan.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ConvertWin32Core
{
    class ConversionWorker
    {
        public static ConversionWorker Instance { get; } = new ConversionWorker();

        static ConversionWorker() { }
        private ConversionWorker() { }

        private readonly ConcurrentQueue<ConversionTask> TaskQueue_ = new ConcurrentQueue<ConversionTask>();
        private bool Running_ { get; set; }
        private bool Working_ { get; set; }

        public event Action OnTaskStart;
        public event Action OnTaskEnd;

        public TaskCompletionSource<bool> Enqueue(ConversionTask task)
        {
            $"Enqueue input={task.InputFile} output={task.OutputFile} type={task.Type.ToString()}".Info();
            TaskQueue_.Enqueue(task);
            return task.Promise;
        }

        public int QueueLength
        {
            get
            {
                return TaskQueue_.Count + (Working_ ? 1 : 0);
            }
        }

        public void Start()
        {
            if (Running_)
            {
                "Conversion worker is already running".Info();
                return;
            }

            Running_ = true;
            new Thread(Run).Start();
            "Conversion worker is starting".Info();
        }

        public void Stop()
        {
            Running_ = false;
            "Conversion worker is stopping".Info();
        }

        private void Run()
        {
            while (Running_)
            {
                if (!TaskQueue_.TryDequeue(out var task))
                {
                    Thread.Sleep(500);
                    continue;
                }

                Working_ = true;

                $"Convert input={task.InputFile} output={task.OutputFile} type={task.Type.ToString()}".Info();
                OnTaskStart.Invoke();

                try
                {
                    switch (task.Type)
                    {
                        case ConversionType.PDF:
                            Converter.ConvertToPDF(task.InputFile, task.OutputFile);
                            break;
                        case ConversionType.PNG:
                            Converter.ConvertToPNG(task.InputFile, task.OutputFile);
                            break;
                    }
                    task.Promise.SetResult(true);
                }
                catch (Exception e)
                {
                    e.Error("ConversionWorker", "Cannot convert due to");
                    task.Promise.SetResult(false);
                }
                OnTaskEnd.Invoke();

                Working_ = false;
            }
            "Conversion worker is end".Info();
        }
    }
}

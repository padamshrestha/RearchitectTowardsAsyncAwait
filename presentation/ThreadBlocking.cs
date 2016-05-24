using System;
using System.Threading;
using System.Threading.Tasks;
using AsyncDolls;
using NUnit.Framework;

namespace RearchitectTowardsAsyncAwait
{
    [TestFixture]
    public class ThreadBlocking
    {
        [Test]
        public async Task Locks()
        {
            //var locker = new object();

            //lock (locker)
            //{
            //    await Task.Yield();
            //}
        }

        [Test]
        public async Task Event()
        {
            MyEvent += MyEventHandler;

            try
            {
                OnMyEvent();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught: { ex.Message } ");
            }

            await Task.Delay(100);
        }

        event EventHandler MyEvent = delegate { };

        async void MyEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Inside MyEventHandler");
            await Task.Yield();
            Console.WriteLine("About to throw inside MyEventHandler");
            throw new InvalidOperationException();
        }

        protected virtual void OnMyEvent()
        {
            MyEvent(this, EventArgs.Empty);
        }

        [Test]
        public async Task AsyncEvent()
        {
            MyAsyncEvent += MyAsyncEventHandler;

            try
            {
                await OnMyAsyncEvent();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Caught: { ex.Message } ");
            }
        }

        event AsyncEventHandler MyAsyncEvent = delegate { return Task.CompletedTask; };

        public delegate Task AsyncEventHandler(object sender, EventArgs e);

        async Task MyAsyncEventHandler(object sender, EventArgs e)
        {
            Console.WriteLine("Inside MyAsyncEventHandler");
            await Task.Yield();
            Console.WriteLine("About to throw inside MyAsyncEventHandler");
            throw new InvalidOperationException();
        }

        protected virtual Task OnMyAsyncEvent()
        {
            return MyAsyncEvent(this, EventArgs.Empty);
        }

        [Test]
        public async Task SemaphoreUsage()
        {
            var semaphore = new Semaphore(1, 1);

            var tasks = new Task[3];
            for (int i = 0; i < 3; i++)
            {
                var i1 = i;
                tasks[i] = Task.Run(() =>
                {
                    $"{ i1 } Entering wait".Output();
                    semaphore.WaitOne();
                    Thread.Sleep(1000);
                    $"{ i1 } Continue".Output();
                    semaphore.Release();
                });
            }

            await Task.WhenAll(tasks);
        }
    }
}
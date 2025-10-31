using System.Threading.Channels;
using RTS.Service.Connector.Interfaces;

namespace RTS.Service.Connector.Infrastructure
{
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<string> _queue;

        public BackgroundTaskQueue()
        {
            _queue = Channel.CreateBounded<string>(new BoundedChannelOptions(5) // bounded so we don't use too much memory if Tracelink fires too fast
            {
                SingleReader = true, // only one consumer can process items
                SingleWriter = false, // multiple producers can add items
                FullMode = BoundedChannelFullMode.Wait // producer has to wait if full 
            });
        }

        public void Enqueue(string orderNumber)
        {
            _queue.Writer.TryWrite(orderNumber);
        }

        public async Task<string> DequeueAsync(CancellationToken token)
        { 
            var orderNumber = await _queue.Reader.ReadAsync(token);
            return orderNumber;
        }
    }
}

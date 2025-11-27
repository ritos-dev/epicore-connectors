using FluentAssertions;
using RTS.Service.Connector.Infrastructure.BackgroundWorker;

namespace RTS.Service.Connector.Test.Infrastructure.BackgroundWorker
{
    public class BackgroundTaskQueueTests
    {
        private readonly BackgroundTaskQueue _sut;

        public BackgroundTaskQueueTests()
        {
            _sut = new BackgroundTaskQueue();
        }

        [Fact]
        public async Task EnqueueAsync_ShouldAddItemToQueue_WhenQueueIsNotFull()
        {
            // Arrange
            var orderNumber = "ORD-123";
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

            // Act
            await _sut.EnqueueAsync(orderNumber, CancellationToken.None);
            var result = await _sut.DequeueAsync(cts.Token);

            // Assert
            result.Should().Be(orderNumber);
        }

        [Fact]
        public async Task DequeueAsync_ShouldReturnItems_InFifoOrder()
        {
            // Arrange
            var firstOrder = "ORD-1";
            var secondOrder = "ORD-2";
            var thirdOrder = "ORD-3";
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            // Act
            await _sut.EnqueueAsync(firstOrder, CancellationToken.None);
            await _sut.EnqueueAsync(secondOrder, CancellationToken.None);
            await _sut.EnqueueAsync(thirdOrder, CancellationToken.None);

            var result1 = await _sut.DequeueAsync(cts.Token);
            var result2 = await _sut.DequeueAsync(cts.Token);
            var result3 = await _sut.DequeueAsync(cts.Token);

            // Assert
            result1.Should().Be(firstOrder);
            result2.Should().Be(secondOrder);
            result3.Should().Be(thirdOrder);
        }

        [Fact]
        public void DequeueAsync_ShouldWait_WhenQueueIsEmpty()
        {
            // Arrange
            var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

            // Act
            Func<Task> act = async () => await _sut.DequeueAsync(cts.Token);

            // Assert
            act.Should().ThrowAsync<OperationCanceledException>().WithMessage("The operation was canceled.");
        }

        [Fact]
        public async Task DequeueAsync_ShouldComplete_WhenItemIsAddedAfterWaitStarts()
        {
            // Arrange
            var orderNumber = "ASYNC-TEST";
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            // Act
            var consumeTask = _sut.DequeueAsync(cts.Token);
            await _sut.EnqueueAsync(orderNumber, CancellationToken.None); 
            var result = await consumeTask;

            // Assert
            result.Should().Be(orderNumber);
            consumeTask.IsCompletedSuccessfully.Should().BeTrue();
        }

        [Fact]
        public async Task EnqueueAsync_ShouldBlock_WhenQueueIsFull()
        {
            // Arrange
            // Queue capacity is 5.
            for (int i = 0; i < 5; i++)
            {
                await _sut.EnqueueAsync($"Item-{i}", CancellationToken.None);
            }

            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2));

            // Act
            // Attempt to add the 6th item. 
            var producingTask = _sut.EnqueueAsync("Item-6", cts.Token);

            // Assert
            await Task.Delay(50);
            producingTask.IsCompleted.Should().BeFalse("Queue is full");
        }

        [Fact]
        public async Task DequeueAsync_Should_Resume_QueueProcess()
        {
            // Arrange
            // Queue capacity is 5.
            for (int i = 0; i < 5; i++)
            {
                await _sut.EnqueueAsync($"Item-{i}", CancellationToken.None);
            }

            // Act
            // Dequeue one item to make space
            await _sut.DequeueAsync(CancellationToken.None);
            var producingTask = _sut.EnqueueAsync("Item-#", CancellationToken.None);

            // Assert
            producingTask.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public async Task EnqueueAsync_ShouldCancel_WhenQueueIsFullAndTokenIsCancelled()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                await _sut.EnqueueAsync($"Item-{i}", CancellationToken.None);
            }

            var cts = new CancellationTokenSource();

            // Act
            var producingTask = _sut.EnqueueAsync("Item-Will-Be-Cancelled", cts.Token);
            producingTask.IsCompleted.Should().BeFalse();
            cts.Cancel();

            // Assert
            Func<Task> act = async () => await producingTask.AsTask();
            await act.Should().ThrowAsync<OperationCanceledException>().WithMessage("The operation was canceled.");
        }
    }
}
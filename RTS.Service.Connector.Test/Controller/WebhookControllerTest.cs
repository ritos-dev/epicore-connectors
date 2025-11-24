using Microsoft.AspNetCore.Mvc;
using Moq;
using RTS.Service.Connector.Controllers;
using RTS.Service.Connector.Interfaces;
using Microsoft.Extensions.Logging;

namespace RTS.Service.Connector.Test.Controller
{
    public class WebhookControllerTest
    {
        private readonly Mock<IBackgroundTaskQueue> _queueMock;
        private readonly Mock<ILogger<TracelinkWebhookController>> _loggerMock;
        private readonly TracelinkWebhookController _controller;

        public WebhookControllerTest()
        {
            _queueMock = new Mock<IBackgroundTaskQueue>();
            _loggerMock = new Mock<ILogger<TracelinkWebhookController>>();
            _controller = new TracelinkWebhookController(
                _queueMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public void ReceiveOrder_ValidOrderNumber_EnqueuesOrderAndReturnsAccepted()
        {
            // Arrange
            var orderNumber = "ORD123";

            // Act
            var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None); 

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _queueMock.Verify(q => q.EnqueueAsync(orderNumber, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ReceiveOrder_NullOrderNumber_StillEnqueuesAndReturnsAccepted()
        {
            // Arrange
            string? orderNumber = null;

            // Act
            var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _queueMock.Verify(q => q.EnqueueAsync(orderNumber, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ReceiveOrder_EmptyOrderNumber_EnqueuesAndReturnsAccepted()
        {
            // Arrange
            var orderNumber = string.Empty;

            // Act
            var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _queueMock.Verify(q => q.EnqueueAsync(orderNumber, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ReceiveOrder_WhitespaceOrderNumber_EnqueuesAndReturnsAccepted()
        {
            // Arrange
            var orderNumber = " ";

            // Act
            var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None);

            // Assert
            Assert.IsType<AcceptedResult>(result);
            _queueMock.Verify(q => q.EnqueueAsync(orderNumber, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void ReceiveOrder_EnqueueThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var orderNumber = "ORD123";
            _queueMock.Setup(q => q.EnqueueAsync(orderNumber, CancellationToken.None)).Throws(new System.Exception("Queue error"));

            // Act
            var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void ReceiveOrder_MultipleCalls_EnqueuesEachOrder()
        {
            var orderNumbers = new[] { "ORD1", "ORD2", "ORD3" };

            foreach (var orderNumber in orderNumbers)
            {
                var result = _controller.ReceiveOrder(orderNumber, CancellationToken.None);
                Assert.IsType<AcceptedResult>(result);
            }

            foreach (var orderNumber in orderNumbers)
            {
                _queueMock.Verify(q => q.EnqueueAsync(orderNumber, CancellationToken.None), Times.Once);
            }
        }
    }
}

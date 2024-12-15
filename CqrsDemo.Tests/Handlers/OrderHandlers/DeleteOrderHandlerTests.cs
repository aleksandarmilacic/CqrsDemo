using Xunit;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Services.OrderServices;
using MediatR;

namespace CqrsDemo.Tests.Handlers.OrderHandlers
{
    public class DeleteOrderHandlerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly DeleteOrderHandler _handler;

        public DeleteOrderHandlerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _handler = new DeleteOrderHandler(_orderServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidId_DeletesOrder()
        {
            // Arrange
            var command = new DeleteOrderCommand(Guid.NewGuid());
            _orderServiceMock.Setup(s => s.DeleteAsync(command.Id)).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(Unit.Value, result); // Ensure the handler returns Unit.Value
            _orderServiceMock.Verify(s => s.DeleteAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var command = new DeleteOrderCommand(Guid.NewGuid());
            _orderServiceMock.Setup(s => s.DeleteAsync(command.Id))
                             .ThrowsAsync(new KeyNotFoundException("Entity not found"));

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            _orderServiceMock.Verify(s => s.DeleteAsync(command.Id), Times.Once);
        }

        [Fact]
        public async Task Handle_ServiceThrowsException_ThrowsInvalidOperationException()
        {
            // Arrange
            var command = new DeleteOrderCommand(Guid.NewGuid());
            _orderServiceMock.Setup(s => s.DeleteAsync(command.Id))
                             .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.Handle(command, CancellationToken.None));

            _orderServiceMock.Verify(s => s.DeleteAsync(command.Id), Times.Once);
        }
    }
}

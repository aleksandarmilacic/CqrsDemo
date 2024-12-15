using Moq;
using AutoMapper;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Domain.Entities.Order;
using CqrsDemo.Application.Services.OrderServices;
using Xunit;

namespace CqrsDemo.Tests.Handlers.OrderHandlers
{
    public class UpdateOrderHandlerTests
    {
        private readonly Mock<IOrderService> _orderServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly UpdateOrderHandler _handler;

        public UpdateOrderHandlerTests()
        {
            _orderServiceMock = new Mock<IOrderService>();
            _mapperMock = new Mock<IMapper>();
            _handler = new UpdateOrderHandler(_orderServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUpdate_ReturnsOrderDTO()
        {
            // Arrange
            var command = new UpdateOrderCommand(Guid.NewGuid(), "Updated Order", 200.0m);

            var mappedOrder = new Order(command.Name, command.Price) { Id = command.Id };
            var updatedOrderDTO = new OrderDTO { Id = command.Id, Name = command.Name, Price = command.Price };

            // Mock the mapping behavior
            _mapperMock.Setup(m => m.Map<Order>(command)).Returns(mappedOrder);

            // Mock the service update logic
            _orderServiceMock.Setup(s => s.UpdateAsync(command.Id, mappedOrder))
                             .ReturnsAsync(updatedOrderDTO);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedOrderDTO, result);

            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _orderServiceMock.Verify(s => s.UpdateAsync(command.Id, mappedOrder), Times.Once);
        }

        [Fact]
        public async Task Handle_EntityNotFound_ThrowsKeyNotFoundException()
        {
            // Arrange
            var command = new UpdateOrderCommand(Guid.NewGuid(), "Updated Order", 200.0m);

            var mappedOrder = new Order(command.Name, command.Price) { Id = command.Id };

            // Mock the service to throw KeyNotFoundException when entity is not found
            _orderServiceMock.Setup(s => s.UpdateAsync(command.Id, mappedOrder))
                             .ThrowsAsync(new KeyNotFoundException($"Entity with ID {command.Id} not found."));

            // Mock the mapping behavior
            _mapperMock.Setup(m => m.Map<Order>(command)).Returns(mappedOrder);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _orderServiceMock.Verify(s => s.UpdateAsync(command.Id, mappedOrder), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new UpdateOrderCommand(Guid.NewGuid(), "Updated Order", 200.0m);

            // Mock the mapping to return null (invalid mapping case)
            _mapperMock.Setup(m => m.Map<Order>(command)).Returns((Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));

            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _orderServiceMock.Verify(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<Order>()), Times.Never);
        }
    }
}

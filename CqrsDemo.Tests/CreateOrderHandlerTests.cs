using System.Threading.Tasks;
using System.Threading;
using System;
using Moq;
using AutoMapper;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Services.OrderServices;
using CqrsDemo.Application.Commands.Order;

namespace CqrsDemo.Tests
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<OrderService> _serviceMock;
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<OrderService>();
            _handler = new CreateOrderHandler(_serviceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsOrderDTO()
        {
            // Arrange
            var command = new CreateOrderCommand("Sample Order", 100.0m);
            var orderEntity = new Order { Id = command.Id, Name = command.Name, Price = command.Price };
            var orderDto = new OrderDTO { Id = command.Id, Name = command.Name, Price = command.Price };

            _mapperMock.Setup(m => m.Map<Order>(command)).Returns(orderEntity);
            _serviceMock.Setup(s => s.CreateAsync(orderEntity)).ReturnsAsync(orderDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(orderDto, result);
            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _serviceMock.Verify(s => s.CreateAsync(orderEntity), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidCommand_ThrowsException()
        {
            // Arrange
            var command = new CreateOrderCommand(Guid.NewGuid(), "Sample Order", 100.0m);

            _mapperMock.Setup(m => m.Map<Order>(command)).Returns((Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));

            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _serviceMock.Verify(s => s.CreateAsync(It.IsAny<Order>()), Times.Never);
        }
    }
}

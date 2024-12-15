using Xunit;
using Moq;
using AutoMapper;
using CqrsDemo.Application.Commands.Order;
using CqrsDemo.Application.Handlers.Commands.Order;
using CqrsDemo.Application.Models.DTOs.Order;
using CqrsDemo.Application.Services;
using CqrsDemo.Domain.Entities.Order;
using System;
using System.Threading;
using System.Threading.Tasks;
using CqrsDemo.Application.Services.OrderServices;

namespace CqrsDemo.Tests
{
    public class CreateOrderHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IOrderService> _serviceMock; // Mock the interface
        private readonly CreateOrderHandler _handler;

        public CreateOrderHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _serviceMock = new Mock<IOrderService>();
            _handler = new CreateOrderHandler(_serviceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsOrderDTO()
        {
            // Arrange
            var command = new CreateOrderCommand("Sample Order", 100.0m);
            var orderEntity = new Order(command.Name, command.Price);
            var orderDto = new OrderDTO { Id = Guid.NewGuid(), Name = command.Name, Price = command.Price };

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
        public async Task Handle_InvalidCommand_ThrowsArgumentNullException()
        {
            // Arrange
            var command = new CreateOrderCommand("Sample Order", 100.0m);

            _mapperMock.Setup(m => m.Map<Order>(command)).Returns((Order)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _handler.Handle(command, CancellationToken.None));

            _mapperMock.Verify(m => m.Map<Order>(command), Times.Once);
            _serviceMock.Verify(s => s.CreateAsync(It.IsAny<Order>()), Times.Never);
        }
    }
}

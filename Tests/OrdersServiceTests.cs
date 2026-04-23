using AutoMapper;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Repository;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DTOs;
using System;

namespace Tests
{
    public class OrdersServiceTests
    {
        private readonly Mock<IOrdersRepository> _mockRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IProductsService> _mockProductsService;
        private readonly Mock<ILogger<OrdersService>> _mockLogger;
        private readonly OrdersService _ordersService;

        public OrdersServiceTests()
        {
            _mockRepository = new Mock<IOrdersRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockProductsService = new Mock<IProductsService>();
            _mockLogger = new Mock<ILogger<OrdersService>>();

            _ordersService = new OrdersService(
                _mockRepository.Object,
                _mockMapper.Object,
                _mockProductsService.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task CreateOrder_HappyPath_SumIsCorrect()
        {
            // Arrange
            var itemsDto = new List<OrderItemDTO> { new OrderItemDTO(0, 1, 0, 2) };
            var orderDto = new OrderDTO(0, DateOnly.FromDateTime(DateTime.Now), 100, 1, itemsDto);

            var orderEntity = new Order
            {
                OrderSum = 100,
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2 } }
            };

            // Setup Mappers: Since CreateOrder uses _mapper, we must mock its behavior
            _mockMapper.Setup(m => m.Map<OrderDTO, Order>(It.IsAny<OrderDTO>())).Returns(orderEntity);
            _mockMapper.Setup(m => m.Map<Order, OrderDTO>(It.IsAny<Order>())).Returns(orderDto);

            // Mock product service to return price 50 (50 * 2 = 100)
            _mockProductsService.Setup(s => s.GetProductById(1))
                .ReturnsAsync(new ProductDTO(1, "Laptop", 50, 1, "Tech", "Gaming Laptop"));

            _mockRepository.Setup(r => r.CreateOrder(It.IsAny<Order>())).ReturnsAsync(orderEntity);

            // Act
            var result = await _ordersService.CreateOrder(orderDto);

            // Assert
            Assert.Equal(100, result.OrderSum);
            _mockRepository.Verify(r => r.CreateOrder(It.IsAny<Order>()), Times.Once);
        }

        [Fact]
        public async Task CreateOrder_WhenSumIsIncorrect_ShouldUpdateSumAndLogWarning()
        {
            // Arrange
            // User sent sum 50, but actual calculation (100 * 2) should be 200
            var itemsDto = new List<OrderItemDTO> { new OrderItemDTO(0, 1, 0, 2) };
            var initialOrderDto = new OrderDTO(0, DateOnly.FromDateTime(DateTime.Now), 50, 10, itemsDto);

            var orderEntity = new Order
            {
                UserId = 10,
                OrderSum = 50,
                OrderItems = new List<OrderItem> { new OrderItem { ProductId = 1, Quantity = 2 } }
            };

            _mockMapper.Setup(m => m.Map<OrderDTO, Order>(initialOrderDto)).Returns(orderEntity);

            // Use 'with' expression to create the expected returned DTO (Immutability)
            var expectedReturnedDto = initialOrderDto with { OrderSum = 200 };
            _mockMapper.Setup(m => m.Map<Order, OrderDTO>(orderEntity)).Returns(expectedReturnedDto);

            _mockProductsService.Setup(s => s.GetProductById(1))
                .ReturnsAsync(new ProductDTO(1, "Laptop", 100, 1, "Tech", "Gaming Laptop"));

            _mockRepository.Setup(r => r.CreateOrder(orderEntity)).ReturnsAsync(orderEntity);

            // Act
            var result = await _ordersService.CreateOrder(initialOrderDto);

            // Assert
            // 1. Verify internal Entity sum was updated before repository call
            Assert.Equal(200, orderEntity.OrderSum);

            // 2. Verify returned DTO has the corrected sum
            Assert.Equal(200, result.OrderSum);

            // 3. Verify Logger was called with warning level and correct message
            _mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("unmatched sum")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}
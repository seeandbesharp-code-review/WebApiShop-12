using AutoMapper;
using Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Org.BouncyCastle.Crypto;
using Repository;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using DTOs;

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
        public async Task OrderSumValidation_HappyPath_SumIsCorrect()
        {
            // Arrange
            var order = new Order
            {
                OrderSum = 100, 
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 }
                }
            };

            _mockProductsService.Setup(s => s.GetProductById(1))
                .ReturnsAsync(new ProductDTO(1, "Laptop", 50, 1, "Tech", "Gaming Laptop"));

            // Act
            await _ordersService.orderSumValidation(order);

            // Assert
            Assert.Equal(100, order.OrderSum); 
        }

        [Fact]
        public async Task OrderSumValidation_UnhappyPath_SumIsIncorrect_ShouldUpdateSumAndLogWarning()
        {
            // Arrange
            var order = new Order
            {
                UserId = 10,
                OrderSum = 50, 
                OrderItems = new List<OrderItem>
                {
                    new OrderItem { ProductId = 1, Quantity = 2 }
                }
            };

            _mockProductsService.Setup(s => s.GetProductById(1))
                .ReturnsAsync(new ProductDTO(1, "Laptop", 100, 1, "Tech", "Gaming Laptop"));

            // Act
            await _ordersService.orderSumValidation(order);

            // Assert
            Assert.Equal(200, order.OrderSum);

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
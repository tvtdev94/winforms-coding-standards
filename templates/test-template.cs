// Template: xUnit Unit Test with Moq
// Replace: YourClass, YourService, YourRepository

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace YourNamespace.Tests
{
    /// <summary>
    /// Unit tests for YourClass.
    /// </summary>
    public class YourClassTests
    {
        private readonly Mock<IYourRepository> _mockRepository;
        private readonly Mock<ILogger<YourService>> _mockLogger;
        private readonly YourService _service;

        public YourClassTests()
        {
            // Arrange - Setup mocks
            _mockRepository = new Mock<IYourRepository>();
            _mockLogger = new Mock<ILogger<YourService>>();
            _service = new YourService(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ValidId_ReturnsEntity()
        {
            // Arrange
            var expectedEntity = new YourEntity { Id = 1, Name = "Test" };
            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(expectedEntity);

            // Act
            var result = await _service.GetByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_InvalidId_ReturnsNull()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((YourEntity?)null);

            // Act
            var result = await _service.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SaveAsync_ValidEntity_CallsRepository()
        {
            // Arrange
            var entity = new YourEntity { Id = 0, Name = "New Entity" };
            _mockRepository
                .Setup(r => r.SaveAsync(It.IsAny<YourEntity>()))
                .ReturnsAsync(true);

            // Act
            var result = await _service.SaveAsync(entity);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.SaveAsync(entity), Times.Once);
        }

        [Fact]
        public async Task SaveAsync_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange, Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.SaveAsync(null!));
        }

        [Fact]
        public async Task DeleteAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.DeleteAsync(1))
                .ReturnsAsync(true);

            // Act
            var result = await _service.DeleteAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            // Arrange
            var entities = new List<YourEntity>
            {
                new YourEntity { Id = 1, Name = "Entity 1" },
                new YourEntity { Id = 2, Name = "Entity 2" }
            };
            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(entities);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}

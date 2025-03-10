﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using FootScout_Vue.WebAPI.Repositories.Classes;

namespace FootScout_Vue.WebAPI.IntegrationTests.Repositories
{
    public class PlayerPositionRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly PlayerPositionRepository _playerPositionRepository;

        public PlayerPositionRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _playerPositionRepository = new PlayerPositionRepository(_dbContext);
        }

        [Fact]
        public async Task GetPlayerPositions_ReturnsAllPlayerPositions()
        {
            // Arrange & Act
            var result = await _playerPositionRepository.GetPlayerPositions();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.Count());
        }

        [Fact]
        public async Task GetPlayerPositionCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _playerPositionRepository.GetPlayerPositionCount();

            // Assert
            Assert.Equal(15, result);
        }

        [Fact]
        public async Task GetPlayerPositionName_ReturnsCorrectName()
        {
            // Arrange
            var positionId = 15;

            // Act
            var result = await _playerPositionRepository.GetPlayerPositionName(positionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Striker", result);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsTrue_WhenPositionExists()
        {
            // Arrange
            var positionName = "Striker";

            // Act
            var result = await _playerPositionRepository.CheckPlayerPositionExists(positionName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckPlayerPositionExists_ReturnsFalse_WhenPositionDoesNotExist()
        {
            // Arrange
            var positionName = "Trainer";

            // Act
            var result = await _playerPositionRepository.CheckPlayerPositionExists(positionName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CreatePlayerPosition_AddsNewPosition_WhenPositionDoesNotExist()
        {
            // Arrange
            var newPosition = new PlayerPosition { PositionName = "NewPosition" };

            // Act
            await _playerPositionRepository.CreatePlayerPosition(newPosition);

            // Assert
            var result = await _dbContext.PlayerPositions.FindAsync(16);
            Assert.NotNull(result);
            Assert.Equal("NewPosition", result.PositionName);

            _dbContext.PlayerPositions.Remove(result);
            await _dbContext.SaveChangesAsync();
        }
    }
}
﻿using AutoMapper;
using FootScout_Vue.WebAPI.Controllers;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FootScout_Vue.WebAPI.UnitTests.Controllers
{
    // Testy jednostkowe dla metod kontrolerów związanych z ulubionymi ogłoszeniami
    public class FavoritePlayerAdvertisementControllerTests
    {
        private readonly Mock<IFavoritePlayerAdvertisementRepository> _mockFavoritePlayerAdvertisementRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly FavoritePlayerAdvertisementController _favoritePlayerAdvertisementController;

        public FavoritePlayerAdvertisementControllerTests()
        {
            _mockFavoritePlayerAdvertisementRepository = new Mock<IFavoritePlayerAdvertisementRepository>();
            _mockMapper = new Mock<IMapper>();
            _favoritePlayerAdvertisementController = new FavoritePlayerAdvertisementController(_mockFavoritePlayerAdvertisementRepository.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task AddToFavorites_ValidDto_ReturnsOkResultWithFavoritePlayerAdvertisement()
        {
            // Arrange
            var dto = new FavoritePlayerAdvertisementCreateDTO { PlayerAdvertisementId = 1, UserId = "pepguardiola" };
            var favoritePlayerAdvertisement = new FavoritePlayerAdvertisement { Id = 1, PlayerAdvertisementId = 1, UserId = "pepguardiola" };

            _mockMapper.Setup(m => m.Map<FavoritePlayerAdvertisement>(dto)).Returns(favoritePlayerAdvertisement);
            _mockFavoritePlayerAdvertisementRepository.Setup(repo => repo.AddToFavorites(favoritePlayerAdvertisement)).Returns(Task.CompletedTask);

            // Act
            var result = await _favoritePlayerAdvertisementController.AddToFavorites(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFavorite = Assert.IsType<FavoritePlayerAdvertisement>(okResult.Value);
            Assert.Equal(favoritePlayerAdvertisement.Id, returnedFavorite.Id);
            Assert.Equal(favoritePlayerAdvertisement.PlayerAdvertisementId, returnedFavorite.PlayerAdvertisementId);
            Assert.Equal(favoritePlayerAdvertisement.UserId, returnedFavorite.UserId);
        }

        [Fact]
        public async Task AddToPlayerFavorites_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            FavoritePlayerAdvertisementCreateDTO dto = null;

            // Act
            var result = await _favoritePlayerAdvertisementController.AddToFavorites(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid dto data.", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteFromPlayerFavorites_ValidId_ReturnsNoContent()
        {
            // Arrange
            var favoritePlayerAdvertisementId = 1;

            _mockFavoritePlayerAdvertisementRepository.Setup(repo => repo.DeleteFromFavorites(favoritePlayerAdvertisementId)).Returns(Task.CompletedTask);

            // Act
            var result = await _favoritePlayerAdvertisementController.DeleteFromFavorites(favoritePlayerAdvertisementId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_ExistingFavorite_ReturnsOkResultWithFavoriteId()
        {
            // Arrange
            var playerAdvertisementId = 1;
            var userId = "pepguardiola";
            var favoriteId = 1;

            _mockFavoritePlayerAdvertisementRepository.Setup(repo => repo.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId)).ReturnsAsync(favoriteId);

            // Act
            var result = await _favoritePlayerAdvertisementController.CheckPlayerAdvertisementIsFavorite(playerAdvertisementId, userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedFavoriteId = Assert.IsType<int>(okResult.Value);
            Assert.Equal(favoriteId, returnedFavoriteId);
        }
    }
}
﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using FootScout_Vue.WebAPI.Repositories.Classes;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.IntegrationTests.Repositories
{
    public class FavoriteClubAdvertisementRepositoryTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly FavoriteClubAdvertisementRepository _favoriteClubAdvertisementRepository;
        public FavoriteClubAdvertisementRepositoryTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _favoriteClubAdvertisementRepository = new FavoriteClubAdvertisementRepository(_dbContext);
        }

        [Fact]
        public async Task AddToFavorites_AddsAdToFavoritesToDB()
        {
            // Arrange
            var newFavoriteAd = new FavoriteClubAdvertisement
            {
                ClubAdvertisementId = 1,
                UserId = "pepguardiola"
            };

            // Act
            await _favoriteClubAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var result = await _dbContext.FavoriteClubAdvertisements
                .FirstOrDefaultAsync(ca => ca.ClubAdvertisementId == 1 && ca.UserId == "pepguardiola");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.ClubAdvertisementId);
            Assert.Equal("pepguardiola", result.UserId);

            _dbContext.FavoriteClubAdvertisements.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteFromFavorites_DeleteClubAdFromFavorites()
        {
            // Arrange
            var newFavoriteAd = new FavoriteClubAdvertisement
            {
                ClubAdvertisementId = 2,
                UserId = "pepguardiola"
            };
            await _favoriteClubAdvertisementRepository.AddToFavorites(newFavoriteAd);

            var favResult = await _dbContext.FavoriteClubAdvertisements
                .FirstOrDefaultAsync(pa => pa.ClubAdvertisementId == 2 && pa.UserId == "pepguardiola");

            // Act
            await _favoriteClubAdvertisementRepository.DeleteFromFavorites(favResult.Id);

            // Assert
            var result = await _dbContext.FavoriteClubAdvertisements.FindAsync(favResult.Id);
            Assert.Null(result);
        }

        [Fact]
        public async Task CheckClubAdvertisementIsFavorite_CheckIfClubAdvertisementIsCheckedAsFavoriteForUser()
        {
            // Arrange
            var clubAdvertisementId = 1;
            var userId = "leomessi";

            // Act
            var result = await _favoriteClubAdvertisementRepository.CheckClubAdvertisementIsFavorite(clubAdvertisementId, userId);

            // Assert
            Assert.Equal(1, result);
        }
    }
}
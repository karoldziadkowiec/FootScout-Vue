﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using FootScout_Vue.WebAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FootScout_Vue.WebAPI.IntegrationTests.Controllers
{
    public class FavoritePlayerAdvertisementControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public FavoritePlayerAdvertisementControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
        {
            _fixture = fixture;
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // new DbContext using DatabaseFixture
                    services.AddDbContext<AppDbContext>(options =>
                    {
                        options.UseSqlServer(_fixture.DbContext.Database.GetDbConnection().ConnectionString);
                    });
                });
            }).CreateClient();

            var userTokenJWT = _fixture.UserTokenJWT;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userTokenJWT);
        }

        [Fact]
        public async Task AddToFavorites_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var favoriteDto = new FavoritePlayerAdvertisementCreateDTO
            {
                PlayerAdvertisementId = 1,
                UserId = "leomessi"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-advertisements/favorites", favoriteDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<FavoritePlayerAdvertisement>();
            Assert.NotNull(result);
            Assert.Equal(favoriteDto.PlayerAdvertisementId, result.PlayerAdvertisementId);
            Assert.Equal(favoriteDto.UserId, result.UserId);

            var favoritePlayerAdvertisement = await _fixture.DbContext.FavoritePlayerAdvertisements.FirstOrDefaultAsync(fpa => fpa.PlayerAdvertisementId == 1 && fpa.UserId == "leomessi");
            _fixture.DbContext.FavoritePlayerAdvertisements.Remove(favoritePlayerAdvertisement);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AddToFavorites_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            FavoritePlayerAdvertisementCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/player-advertisements/favorites", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task DeleteFromFavorites_ReturnsNoContent_WhenDeletionIsSuccessful()
        {
            // Arrange
            var favoriteDto = new FavoritePlayerAdvertisementCreateDTO
            {
                PlayerAdvertisementId = 1,
                UserId = "leomessi"
            };

            var postResponse = await _client.PostAsJsonAsync("/api/player-advertisements/favorites", favoriteDto);
            var favoritePlayerAdvertisement = await _fixture.DbContext.FavoritePlayerAdvertisements.FirstOrDefaultAsync(fpa => fpa.PlayerAdvertisementId == 1 && fpa.UserId == "leomessi");

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/player-advertisements/favorites/{favoritePlayerAdvertisement.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_ReturnsOk_WithFavoriteId()
        {
            // Arrange
            int playerAdvertisementId = 1;
            string userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/player-advertisements/favorites/check/{playerAdvertisementId}/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var favoriteId = await response.Content.ReadAsStringAsync();
            Assert.NotEmpty(favoriteId);
        }

        [Fact]
        public async Task CheckPlayerAdvertisementIsFavorite_ReturnsOk_WhenNotFavorited()
        {
            // Arrange
            int playerAdvertisementId = 9999;
            string userId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/player-advertisements/favorites/check/{playerAdvertisementId}/{userId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var favoriteId = await response.Content.ReadAsStringAsync();
            Assert.Equal("0", favoriteId);
        }
    }
}
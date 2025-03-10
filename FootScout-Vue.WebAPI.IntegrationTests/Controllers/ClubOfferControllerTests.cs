﻿using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using FootScout_Vue.WebAPI.DbManager;
using Microsoft.Extensions.DependencyInjection;
using FootScout_Vue.WebAPI.Models.DTOs;

namespace FootScout_Vue.WebAPI.IntegrationTests.Controllers
{
    public class ClubOfferControllerTests : IClassFixture<WebApplicationFactory<Program>>, IClassFixture<DatabaseFixture>
    {
        private readonly HttpClient _client;
        private readonly DatabaseFixture _fixture;

        public ClubOfferControllerTests(WebApplicationFactory<Program> factory, DatabaseFixture fixture)
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
        public async Task GetClubOffer_ReturnsOk_WhenClubOfferExists()
        {
            // Arrange
            var clubOfferId = 1;

            // Act
            var response = await _client.GetAsync($"/api/club-offers/{clubOfferId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubOffer = await response.Content.ReadFromJsonAsync<ClubOffer>();
            Assert.Equal(clubOfferId, clubOffer.Id);
        }

        [Fact]
        public async Task GetClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubOfferId = 9999;

            // Act
            var response = await _client.GetAsync($"/api/club-offers/{clubOfferId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllClubOffers_ReturnsOk_WhenClubOffersExists()
        {
            // Arrange & Act
            var response = await _client.GetAsync($"/api/club-offers");

            // Assert
            response.EnsureSuccessStatusCode();
            var clubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(clubOffers);
        }

        [Fact]
        public async Task GetActiveClubOffers_ReturnsOk_WhenActiveClubOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-offers/active");

            // Assert
            response.EnsureSuccessStatusCode();
            var activeClubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.NotEmpty(activeClubOffers);
        }

        [Fact]
        public async Task GetActiveClubOffersCount_ReturnsOk_WithCorrectCount()
        {
            // Arrange
            var expectedCount = 2;

            // Act
            var response = await _client.GetAsync("/api/club-offers/active/count");

            // Assert
            response.EnsureSuccessStatusCode();
            var countString = await response.Content.ReadAsStringAsync();
            int actualCount = int.Parse(countString);
            Assert.Equal(expectedCount, actualCount);
        }

        [Fact]
        public async Task GetInactiveClubOffers_ReturnsOk_WhenInactiveClubOffersExist()
        {
            // Act
            var response = await _client.GetAsync("/api/club-offers/inactive");

            // Assert
            response.EnsureSuccessStatusCode();
            var inactiveClubOffers = await response.Content.ReadFromJsonAsync<IEnumerable<ClubOffer>>();
            Assert.Empty(inactiveClubOffers);
        }

        [Fact]
        public async Task CreateClubOffer_ReturnsOk_WhenDataIsValid()
        {
            // Arrange
            var offerDto = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 232,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-offers", offerDto);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ClubOffer>();
            Assert.NotNull(result);
            Assert.Equal(offerDto.PlayerPositionId, result.PlayerPositionId);
            Assert.Equal(offerDto.ClubName, result.ClubName);
            Assert.Equal(offerDto.League, result.League);
            Assert.Equal(offerDto.Region, result.Region);
            Assert.Equal(offerDto.Salary, result.Salary);
            Assert.Equal(offerDto.ClubMemberId, result.ClubMemberId);

            var clubOffer = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.PlayerPositionId == offerDto.PlayerPositionId && co.ClubName == offerDto.ClubName && co.League == offerDto.League && co.Region == offerDto.Region && co.Salary == offerDto.Salary && co.ClubMemberId == offerDto.ClubMemberId);
            _fixture.DbContext.ClubOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task CreateClubOffer_ReturnsBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            ClubOfferCreateDTO invalidDto = null;

            // Act
            var response = await _client.PostAsJsonAsync("/api/club-offers", invalidDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
        }

        [Fact]
        public async Task UpdateClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var offerToUpdate = await _fixture.DbContext.ClubOffers.FirstAsync();
            offerToUpdate.ClubName = "Inter Mediolan";

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/{offerToUpdate.Id}", offerToUpdate);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task UpdateClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubOfferId = 2;
            var offerToUpdate = await _fixture.DbContext.ClubOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/{clubOfferId}", offerToUpdate);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteClubOffer_RemovesClubOffer()
        {
            // Arrange
            var newOffer = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 233,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response = await _client.PostAsJsonAsync("/api/club-offers", newOffer);
            var clubOffer = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.PlayerPositionId == newOffer.PlayerPositionId && co.ClubName == newOffer.ClubName && co.League == newOffer.League && co.Region == newOffer.Region && co.Salary == newOffer.Salary && co.ClubMemberId == newOffer.ClubMemberId);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/club-offers/{clubOffer.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }

        [Fact]
        public async Task AcceptClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var newOffer = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 237,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response1 = await _client.PostAsJsonAsync("/api/club-offers", newOffer);
            var offerToAccept = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.Salary == 237);

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/club-offers/accept/{offerToAccept.Id}", offerToAccept);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.Salary == 237);
            _fixture.DbContext.ClubOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task AcceptClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubOfferId = 2;
            var offerToAccept = await _fixture.DbContext.ClubOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/accept/{clubOfferId}", offerToAccept);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RejectClubOffer_ReturnsNoContent_WhenClubOfferExists()
        {
            // Arrange
            var newOffer = new ClubOfferCreateDTO
            {
                PlayerAdvertisementId = 1,
                PlayerPositionId = 12,
                ClubName = "Juventus Turyn",
                League = "Serie A",
                Region = "Italy",
                Salary = 238,
                AdditionalInformation = "no info",
                ClubMemberId = "pepguardiola"
            };
            var response1 = await _client.PostAsJsonAsync("/api/club-offers", newOffer);
            var offerToReject = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.Salary == 238);

            // Act
            var response2 = await _client.PutAsJsonAsync($"/api/club-offers/reject/{offerToReject.Id}", offerToReject);

            // Assert
            response2.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, response2.StatusCode);

            var clubOffer = await _fixture.DbContext.ClubOffers.FirstOrDefaultAsync(co => co.Salary == 238);
            _fixture.DbContext.ClubOffers.Remove(clubOffer);
            await _fixture.DbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task RejectClubOffer_ReturnsNotFound_WhenClubOfferDoesNotExist()
        {
            // Arrange
            var clubOfferId = 2;
            var offerToReject = await _fixture.DbContext.ClubOffers.FirstAsync();

            // Act
            var response = await _client.PutAsJsonAsync($"/api/club-offers/reject/{clubOfferId}", offerToReject);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetClubOfferStatusId_ReturnsOk_WhenClubOfferExists()
        {
            // Arrange
            var clubOfferId = 2;
            var clubMemberId = "pepguardiola";

            // Act
            var response = await _client.GetAsync($"/api/club-offers/status/{clubOfferId}/{clubMemberId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var offerStatusId = await response.Content.ReadFromJsonAsync<int>();
            Assert.Equal(2, offerStatusId);
        }

        [Fact]
        public async Task ExportClubOffersToCsv_ReturnsFileResult()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/club-offers/export");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("text/csv", response.Content.Headers.ContentType.MediaType);
            Assert.Equal("club-offers.csv", response.Content.Headers.ContentDisposition.FileName);
        }
    }
}
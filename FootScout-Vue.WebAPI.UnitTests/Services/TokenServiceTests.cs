﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Services.Classes;
using FootScout_Vue.WebAPI.UnitTests.TestManager;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FootScout_Vue.WebAPI.UnitTests.Services
{
    public class TokenServiceTests : TestBase
    {
        [Fact]
        public async Task CreateTokenJWT_SetsCorrectExpirationDate()
        {
            // Arrange
            var options = GetDbContextOptions("CreateTokenJWT_SetsCorrectExpirationDate");
            var user = new User { Id = "userId" };

            using (var dbContext = new AppDbContext(options))
            {
                var configuration = CreateConfiguration();
                var mockUserManager = new Mock<UserManager<User>>(
                    Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
                mockUserManager.Setup(um => um.GetRolesAsync(It.IsAny<User>()))
                    .ReturnsAsync(new List<string>());

                var _tokenService = new TokenService(configuration, mockUserManager.Object);

                // Act
                var token = await _tokenService.CreateTokenJWT(user);

                // Assert
                Assert.NotNull(token);
                Assert.True(token.ValidTo > DateTime.Now);
                Assert.True(token.ValidTo <= DateTime.Now.AddDays(1));
            }
        }
    }
}
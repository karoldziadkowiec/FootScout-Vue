﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.IntegrationTests.TestManager;
using FootScout_Vue.WebAPI.Services.Classes;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FootScout_Vue.WebAPI.IntegrationTests.Services
{
    public class ChatServiceTests : IClassFixture<DatabaseFixture>
    {
        private readonly AppDbContext _dbContext;
        private readonly ChatService _chatService;

        public ChatServiceTests(DatabaseFixture fixture)
        {
            _dbContext = fixture.DbContext;
            _chatService = new ChatService(_dbContext);
        }

        [Fact]
        public async Task GetChatById_ReturnsCorrectChat()
        {
            // Arrange
            var userId = 1;

            // Act
            var result = await _chatService.GetChatById(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal("leomessi", result.User1Id);
        }

        [Fact]
        public async Task GetChats_ReturnsAllChats()
        {
            // Arrange &  Act
            var result = await _chatService.GetChats();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetChatCount_ReturnsCorrectCount()
        {
            // Arrange & Act
            var result = await _chatService.GetChatCount();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task GetChatIdBetweenUsers_ReturnsCorrectChatId()
        {
            // Arrange
            var user1Id = "leomessi";
            var user2Id = "pepguardiola";

            // Act
            var result = await _chatService.GetChatIdBetweenUsers(user1Id, user2Id);

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CreateChat_AddsNewChat()
        {
            // Arrange
            var newChat = new Chat
            {
                User1Id = "pepguardiola",
                User2Id = "admin0"
            };

            // Act
            await _chatService.CreateChat(newChat);

            var result = await _dbContext.Chats.FindAsync(newChat.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("pepguardiola", result.User1Id);
            Assert.Equal("admin0", result.User2Id);

            _dbContext.Chats.Remove(result);
            await _dbContext.SaveChangesAsync();
        }

        [Fact]
        public async Task DeleteChat_RemovesChatAndMessages()
        {
            // Arrange
            _dbContext.Chats.Add(new Chat
            {
                User1Id = "pepguardiola",
                User2Id = "unknown9"
            });
            await _dbContext.SaveChangesAsync();

            var chatToDelete = await _dbContext.Chats
                .FirstOrDefaultAsync(c => c.User1Id == "pepguardiola" && c.User2Id == "unknown9");

            // Act
            await _chatService.DeleteChat(chatToDelete.Id);

            var chatResult = await _dbContext.Chats
                .FirstOrDefaultAsync(c => c.User1Id == "pepguardiola" && c.User2Id == "unknown9");

            var messagesResult = await _dbContext.Messages.Where(m => m.ChatId == chatToDelete.Id).ToListAsync();

            // Assert
            Assert.Null(chatResult);
            Assert.Empty(messagesResult);
        }

        [Fact]
        public async Task ExportChatsToCsv_ReturnsValidCsvStream()
        {
            // Arrange & Act
            var csvStream = await _chatService.ExportChatsToCsv();
            var csvContent = Encoding.UTF8.GetString(csvStream.ToArray());

            // Assert
            Assert.NotEmpty(csvContent);
            Assert.Contains("Chat Id,User1 E-mail,User1 First Name,User1 Last Name,User2 E-mail,User2 First Name,User2 Last Name", csvContent);
            Assert.Contains("1,lm10@gmail.com,Leo,Messi,pg8@gmail.com,Pep,Guardiola", csvContent);
        }
    }
}
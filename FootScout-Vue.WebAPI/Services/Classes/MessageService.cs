﻿using FootScout_Vue.WebAPI.DbManager;
using FootScout_Vue.WebAPI.Entities;
using FootScout_Vue.WebAPI.Models.DTOs;
using FootScout_Vue.WebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FootScout_Vue.WebAPI.Services.Classes
{
    // Serwis z zaimplementowanymi metodami związanymi z wiadomościami
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _dbContext;

        public MessageService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Zwróć wszystkie wiadomości
        public async Task<IEnumerable<Message>> GetAllMessages()
        {
            return await _dbContext.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        // Zwróć liczbę wszystkich wiadomości
        public async Task<int> GetAllMessagesCount()
        {
            return await _dbContext.Messages.CountAsync();
        }

        // Zwróć wszystkie wiadomości dla konkretnego czatu
        public async Task<IEnumerable<Message>> GetMessagesForChat(int chatId)
        {
            return await _dbContext.Messages
                .Include(m => m.Chat)
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ChatId == chatId)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
        }

        // Zwróć liczbę wiadomości dla konkretnego czatu
        public async Task<int> GetMessagesForChatCount(int chatId)
        {
            return await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .CountAsync();
        }

        // Zwróć datę ostatniej wiadomości dla konkretnego czatu
        public async Task<DateTime> GetLastMessageDateForChat(int chatId)
        {
            return await _dbContext.Messages
                .Where(m => m.ChatId == chatId)
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.Timestamp)
                .FirstOrDefaultAsync();
        }

        // Wyślij (utwórz) nową wiadomość
        public async Task<Message> SendMessage(MessageSendDTO dto)
        {
            var message = new Message
            {
                ChatId = dto.ChatId,
                SenderId = dto.SenderId,
                ReceiverId = dto.ReceiverId,
                Content = dto.Content,
                Timestamp = DateTime.Now
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return message;
        }

        // Usuń konkretną wiadomość
        public async Task DeleteMessage(int messageId)
        {
            var message = await _dbContext.Messages.FindAsync(messageId);

            if (message == null)
                throw new ArgumentException($"No message found with ID {messageId}");

            _dbContext.Messages.Remove(message);
            await _dbContext.SaveChangesAsync();
        }
    }
}
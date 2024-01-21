using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository(ApplicationDbContext context, IHubContext<ChatHub> hub) : IMessageRepository
    {
        private readonly ApplicationDbContext _context = context;
        private readonly IHubContext<ChatHub> _hub = hub;

        public async Task<List<MessageDto>> GetConversation(int page, int limit, string receiverId, string senderId)
        {
            return await _context.Messages
                .Where(x => (x.ReceiverId == receiverId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == receiverId))
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new MessageDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Timestamp = x.Timestamp,
                    SenderId = x.SenderId,
                    ReceiverId = x.ReceiverId
                })
                .Reverse()
                .ToListAsync();
        }

        public async Task<int> GetMessageNotificationCount(string userId) => await _context.Messages.Where(m => m.ReceiverId == userId && !m.IsRead).CountAsync();

        public async Task<List<UserDto>> GetRecentChats(string userId)
        {
            var recentUsers = await _context.Messages
                .Where(x => x.ReceiverId == userId || x.SenderId == userId)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => x.ReceiverId == userId ? x.SenderId : x.ReceiverId)
                .Distinct()
                .ToListAsync();

            var users = await _context.Users
                .Where(x => recentUsers.Contains(x.Id))
                .Select(x => new UserDto
                {
                    Id = x.Id,
                    UserName = x.UserName ?? string.Empty,
                    ProfilePicture = x.ProfilePicture,
                    LastLogin = x.LastLogin,
                    UnreadMessages = _context.Messages.Count(m => m.ReceiverId == userId && m.SenderId == x.Id && !m.IsRead)
                })
                .OrderByDescending(u => _context.Messages
                .Where(m => (m.SenderId == userId && m.ReceiverId == u.Id) || (m.SenderId == u.Id && m.ReceiverId == userId))
                .OrderByDescending(m => m.Timestamp)
                .Select(m => m.Timestamp)
                .FirstOrDefault())
                .ToListAsync();

            return users;
        }

        public async Task<List<Message>> GetUserMessages(string id) => await _context.Messages.Where(m => m.ReceiverId == id).ToListAsync();

        public async Task<int> ReadMessages(string senderId, string receiverId)
        {
            // The idea its when opening a conversation all the messages are marked as read
            var messages = await _context.Messages.Where(m => m.SenderId == senderId && m.ReceiverId == receiverId && !m.IsRead).ToListAsync();

            foreach (var message in messages)
            {
                message.IsRead = true;
            }

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected == 0)
            {
                return 0;
            }

            await UpdateUserLastLogin(receiverId);

            return rowsAffected;
        }

        public async Task<MessageDto> Send(string senderId, string receiverId, string message)
        {
            bool receiverExists = await _context.Users.AnyAsync(u => u.Id == receiverId);
            User? sender = await _context.Users.FindAsync(senderId);

            if (!receiverExists || sender is null)
            {
                return null;
            }

            var newMessage = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = message,
                Timestamp = DateTimeOffset.UtcNow,
                IsRead = false
            };

            _context.Messages.Add(newMessage);
            sender.LastLogin = DateTimeOffset.UtcNow;

            int rowsAffected = await _context.SaveChangesAsync();

            if (rowsAffected == 0)
            {
                return null;
            }

            MessageDto messageDto = new()
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                Timestamp = newMessage.Timestamp,
                SenderId = newMessage.SenderId,
                ReceiverId = newMessage.ReceiverId
            };

            // Call SendMessage from chathub here instead of the next line
            await _hub.Clients.User(receiverId).SendAsync("ReceiveMessage", messageDto);

            await UpdateUserLastLogin(receiverId);

            return messageDto;
        }

        public async Task<bool> UpdateUserLastLogin(string userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user is null)
            {
                return false;
            }

            user.LastLogin = DateTimeOffset.UtcNow;

            int rowsAffected = await _context.SaveChangesAsync();

            return rowsAffected > 0;
        }
    }
}

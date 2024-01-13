using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MessageRepository(ApplicationDbContext context) : IMessageRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<MessageDto>> GetConversation(int page, int limit, string receiverId, string senderId)
        {
            return await _context.Messages
                .Where(x => (x.ReceiverId == receiverId && x.SenderId == senderId) || (x.ReceiverId == senderId && x.SenderId == receiverId))
                .OrderBy(x => x.Timestamp)
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
                .ToListAsync();
        }

        public async Task<List<UserDto>> GetRecentChats(string userId)
        {
            return await _context.Messages
                .Where(x => x.ReceiverId == userId || x.SenderId == userId)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new UserDto
                {
                    Id = x.SenderId == userId ? x.Receiver.Id : x.Sender.Id,
                    UserName = x.SenderId == userId ? x.Receiver.UserName : x.Sender.UserName,
                    ProfilePicture = x.SenderId == userId ? x.Receiver.ProfilePicture : x.Sender.ProfilePicture,
                    LastLogin = x.SenderId == userId ? x.Receiver.LastLogin : x.Sender.LastLogin,
                })
                .Distinct()
                .ToListAsync();
        }

        public async Task<List<Message>> GetUserMessages(string id) => await _context.Messages.Where(m => m.ReceiverId == id).ToListAsync();

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

            return new MessageDto
            {
                Id = newMessage.Id,
                Content = newMessage.Content,
                Timestamp = newMessage.Timestamp,
            };
        }
    }
}

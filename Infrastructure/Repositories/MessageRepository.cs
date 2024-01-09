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
                .OrderByDescending(x => x.Timestamp)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(x => new MessageDto
                {
                    Id = x.Id,
                    Content = x.Content,
                    Timestamp = x.Timestamp,
                    Sender = new UserDto
                    {
                        Id = x.Sender.Id,
                        UserName = x.Sender.UserName,
                        ProfilePicture = x.Sender.ProfilePicture
                    },
                    Receiver = new UserDto
                    {
                        Id = x.Receiver.Id,
                        UserName = x.Receiver.UserName,
                        ProfilePicture = x.Receiver.ProfilePicture
                    }
                })
                .ToListAsync();
        }

        public async Task<List<UserDto>> GetRecentChats(string userId)
        {
            // People that have messaged the user
            var senders = await _context.Messages
                 .Where(x => x.ReceiverId == userId && x.SenderId != userId)
                 .OrderByDescending(x => x.Timestamp)
                 .Select(x => new UserDto
                 {
                     Id = x.Sender.Id,
                     UserName = x.Sender.UserName,
                     ProfilePicture = x.Sender.ProfilePicture
                 })
                 .Distinct()
                 .ToListAsync();

            // People that the user has messaged
            var receivers = await _context.Messages
                .Where(x => x.SenderId == userId && x.ReceiverId != userId)
                .OrderByDescending(x => x.Timestamp)
                .Select(x => new UserDto
                {
                    Id = x.Receiver.Id,
                    UserName = x.Receiver.UserName,
                    ProfilePicture = x.Receiver.ProfilePicture
                })
                .Distinct()
                .ToListAsync();

            // Sort the combined list of "senders" and "receivers" based on the timestamp of their most recent message.
            return
                [.. senders.Concat(receivers).OrderByDescending(
                    user => _context.Messages // Retrieve the timestamp of the most recent message for each user
                    .Where(x => (x.ReceiverId == user.Id && x.SenderId == userId) || (x.ReceiverId == userId && x.SenderId == user.Id))
                    .OrderByDescending(x => x.Timestamp)
                    .Select(x => x.Timestamp)
                    .FirstOrDefault()
                    )];
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

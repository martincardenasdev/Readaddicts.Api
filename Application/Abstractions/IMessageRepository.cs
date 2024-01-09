using Domain.Dto;
using Domain.Entities;

namespace Application.Abstractions
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetUserMessages(string id);
        Task<MessageDto> Send(string senderId, string receiverId, string message);
        Task<List<MessageDto>> GetConversation(int page, int limit, string receiverId, string senderId);
        Task<List<UserDto>> GetRecentChats(string userId);
    }
}

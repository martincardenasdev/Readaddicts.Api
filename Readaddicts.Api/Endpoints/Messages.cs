using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace Readaddicts.Api.Endpoints
{
    internal class RegisterMessagesEndpoints : IMinimalEndpoint
    {
        public void MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            var messages = routeBuilder.MapGroup("/api/v1/messages");

            messages.MapPost("send/{receiverId}", Messages.CreateMessage).RequireAuthorization(); // Dont actually hit this. Use SignalR instead
            messages.MapGet("", Messages.GetUserMessages).RequireAuthorization();
            messages.MapGet("conversation/{receiverId}", Messages.GetConversation).RequireAuthorization();
            messages.MapGet("recent-chats", Messages.GetRecentChats).RequireAuthorization();
            messages.MapPatch("read-messages/{senderId}", Messages.ReadMessages).RequireAuthorization();
            messages.MapGet("notification-count", Messages.GetMessageNotificationCount).RequireAuthorization();
        }
    }
    public static class Messages
    {
        private static string GetUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
        public static async Task<Results<Ok<MessageDto>, BadRequest>> CreateMessage(IMessageRepository messageRepository, ClaimsPrincipal user, string receiverId, string message)
        {
            MessageDto? newMessage = await messageRepository.Send(GetUserId(user), receiverId, message);

            if (newMessage is null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(newMessage);
        }
        public static async Task<Results<Ok<List<Message>>, NotFound>> GetUserMessages(IMessageRepository messageRepository, ClaimsPrincipal user)
        {
            List<Message>? messages = await messageRepository.GetUserMessages(GetUserId(user));

            if (messages is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(messages);
        }
        public static async Task<Results<Ok<List<MessageDto>>, NotFound>> GetConversation(IMessageRepository messageRepository, ClaimsPrincipal user, string receiverId, int page, int limit)
        {
            List<MessageDto>? messages = await messageRepository.GetConversation(page, limit, GetUserId(user), receiverId);

            if (messages is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(messages);
        }
        public static async Task<Results<Ok<List<UserDto>>, NotFound>> GetRecentChats(IMessageRepository messageRepository, ClaimsPrincipal user)
        {
            List<UserDto>? users = await messageRepository.GetRecentChats(GetUserId(user));

            if (users is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(users);
        }
        public static async Task<Results<Ok<int>, BadRequest>> ReadMessages(IMessageRepository messageRepository, ClaimsPrincipal user, string senderId)
        {
            int messagesRead = await messageRepository.ReadMessages(senderId, GetUserId(user));

            if (messagesRead == 0)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(messagesRead);
        }
        public static async Task<Results<Ok<int>, BadRequest>> GetMessageNotificationCount(IMessageRepository messageRepository, ClaimsPrincipal user)
        {
            int count = await messageRepository.GetMessageNotificationCount(GetUserId(user));

            return TypedResults.Ok(count);
        }
    }
}

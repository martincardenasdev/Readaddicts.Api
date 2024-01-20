using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Security.Claims;

namespace ReadaddictsNET8.Endpoints
{
    public static class Messages
    {
        public static void AddMessagesEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder messages = routes.MapGroup("/api/v1/messages");

            messages.MapPost("send/{receiverId}", CreateMessage).RequireAuthorization(); // Dont actually hit this. Use SignalR instead
            messages.MapGet("", GetUserMessages).RequireAuthorization();
            messages.MapGet("conversation/{receiverId}", GetConversation).RequireAuthorization();
            messages.MapGet("recent-chats", GetRecentChats).RequireAuthorization();
            messages.MapPatch("read-messages/{senderId}", ReadMessages).RequireAuthorization();
        }

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
        public static async Task<Results<Ok, BadRequest>> ReadMessages(IMessageRepository messageRepository, ClaimsPrincipal user, string senderId)
        {
            bool success = await messageRepository.ReadMessages(senderId, GetUserId(user));

            if (!success)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
    }
}

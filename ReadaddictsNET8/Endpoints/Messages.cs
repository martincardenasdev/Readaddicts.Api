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
            RouteGroupBuilder comments = routes.MapGroup("/api/v1/messages");

            comments.MapPost("send/{receiverId}", CreateMessage).RequireAuthorization();
            comments.MapGet("", GetUserMessages).RequireAuthorization();
            comments.MapGet("conversation/{receiverId}", GetConversation).RequireAuthorization();
            comments.MapGet("recent-chats", GetRecentChats).RequireAuthorization();
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
    }
}

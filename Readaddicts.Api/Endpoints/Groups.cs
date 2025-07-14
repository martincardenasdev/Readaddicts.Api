using Application.Abstractions;
using Domain.Common;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Readaddicts.Api.Endpoints
{
    internal class RegisterGroupsEndpoints : IMinimalEndpoint
    {
        public void MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            var groups = routeBuilder.MapGroup("/api/v1/groups");

            groups.MapGet("/all", Groups.GetGroups);
            groups.MapGet("/{id}", Groups.GetGroup);
            groups.MapGet("/{id}/posts", Groups.GetPostsByGroup).RequireAuthorization();
            groups.MapPost("/create", Groups.CreateGroup).RequireAuthorization().DisableAntiforgery();
            groups.MapPatch("/{id}", Groups.UpdateGroup).RequireAuthorization();
            groups.MapDelete("/{id}", Groups.DeleteGroup).RequireAuthorization();
            groups.MapPost("/{id}/join", Groups.JoinGroup).RequireAuthorization();
            groups.MapPost("/{id}/leave", Groups.LeaveGroup).RequireAuthorization();
        }
    }
    public static class Groups
    {
        private static string GetUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
        public static async Task<Ok<DataCountPagesDto<IEnumerable<GroupDto>>>> GetGroups([FromServices] IGroupRepository groupRepository, int page, int limit)
        {
            var groups = await groupRepository.GetGroups(page, limit);

            return TypedResults.Ok(groups);
        }
        public static async Task<Results<Ok<GroupDto>, NotFound>> GetGroup(ClaimsPrincipal user, [FromServices] IGroupRepository groupRepository, string id)
        {
            GroupDto? group = await groupRepository.GetGroup(GetUserId(user), id);

            if (group is null)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Ok(group);
        }
        public static async Task<Ok<DataCountPagesDto<IEnumerable<PostDto>>>> GetPostsByGroup([FromServices] IGroupRepository groupRepository, string id, ClaimsPrincipal user, int page, int limit)
        {
            var posts = await groupRepository.GetPostsByGroup(id, GetUserId(user), page, limit);

            return TypedResults.Ok(posts);
        }
        public static async Task<Results<Ok<string>, BadRequest>> CreateGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, [FromForm] Group group, [FromForm] IFormFile? picture)
        {
            string groupId = await groupRepository.CreateGroup(GetUserId(user), group, picture);

            if (groupId is null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(groupId);
        }
        public static async Task<Results<Ok, BadRequest>> UpdateGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id, [FromForm] Group group, [FromForm] IFormFile? picture)
        {
            bool updated = await groupRepository.UpdateGroup(id, GetUserId(user), group, picture);

            if (!updated)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
        public static async Task<Results<Ok, BadRequest>> DeleteGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id)
        {
            bool deleted = await groupRepository.DeleteGroup(id, GetUserId(user));

            if (!deleted)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
        public static async Task<Results<Ok<OperationResult<UserDto>>, BadRequest<OperationResult<UserDto>>>> JoinGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id)
        {
            var joinResult = await groupRepository.JoinGroup(GetUserId(user), id);

            if (!joinResult.Success)
            {
                return TypedResults.BadRequest(joinResult);
            }

            return TypedResults.Ok(joinResult);
        }
        public static async Task<Results<Ok<OperationResult<UserDto>>, BadRequest<OperationResult<UserDto>>>> LeaveGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id)
        {
            var leaveResult = await groupRepository.LeaveGroup(GetUserId(user), id);

            if (!leaveResult.Success)
            {
                return TypedResults.BadRequest(leaveResult);
            }

            return TypedResults.Ok(leaveResult);
        }
    }
}


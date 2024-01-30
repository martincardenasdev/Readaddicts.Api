using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ReadaddictsNET8.Endpoints
{
    public static class Groups
    {
        public static void AddGroupsEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder groups = routes.MapGroup("/api/v1/groups");

            groups.MapGet("/all", GetGroups);
            groups.MapGet("{id}", GetGroup);
            groups.MapGet("{id}/posts", GetPostsByGroup).RequireAuthorization();
            groups.MapPost("/create", CreateGroup).RequireAuthorization().DisableAntiforgery();
            groups.MapPatch("{id}", UpdateGroup).RequireAuthorization();
            groups.MapDelete("{id}", DeleteGroup).RequireAuthorization();
            groups.MapPost("{id}/join", JoinGroup).RequireAuthorization();
            groups.MapPost("{id}/leave", LeaveGroup).RequireAuthorization();
        }

        private static string GetUserId(ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier);
        public static async Task<Ok<DataCountPagesDto<IEnumerable<GroupDto>>>> GetGroups([FromServices] IGroupRepository groupRepository, int page, int limit)
        {
            var groups = await groupRepository.GetGroups(page, limit);

            return TypedResults.Ok(groups);
        }
        public static async Task<Results<Ok<GroupDto>, NotFound>> GetGroup([FromServices] IGroupRepository groupRepository, string id)
        {
            GroupDto? group = await groupRepository.GetGroup(id);

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
        public static async Task<Results<Ok, BadRequest>> JoinGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id)
        {
            bool joined = await groupRepository.JoinGroup(GetUserId(user), id);

            if (!joined)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
        public static async Task<Results<Ok, BadRequest>> LeaveGroup([FromServices] IGroupRepository groupRepository, ClaimsPrincipal user, string id)
        {
            bool left = await groupRepository.LeaveGroup(GetUserId(user), id);

            if (!left)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok();
        }
    }
}


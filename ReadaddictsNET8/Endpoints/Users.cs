using Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;

namespace ReadaddictsNET8.Endpoints
{
    public static class Users
    {
        public static void AddUsersEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder users = routes.MapGroup("/api/v1/users");

            users.MapPost("/register", Register);
            users.MapPost("/add-roles", AddRoles);
            users.MapDelete("/delete", Delete);
        }

        public static async Task<Results<Ok<User>, BadRequest<IEnumerable<string>>>> Register(User user, string roleName, UserManager<User> userManager)
        {
            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email
            };

            var result = await userManager.CreateAsync(newUser, user.PasswordHash);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, roleName);

                return TypedResults.Ok(newUser);
            }

            var errors = result.Errors.Select(error => error.Description);

            return TypedResults.BadRequest(errors);
        }
        public static async Task<Results<Ok, BadRequest<IEnumerable<string>>>> AddRoles (RoleManager<IdentityRole> roleManager)
        {
            var admin = new IdentityRole("Admin");
            var moderator = new IdentityRole("Moderator");
            var user = new IdentityRole("User");

            var result = await roleManager.CreateAsync(admin);
            var result2 = await roleManager.CreateAsync(moderator);
            var result3 = await roleManager.CreateAsync(user);

            if (result.Succeeded && result2.Succeeded && result3.Succeeded)
            {
                return TypedResults.Ok();
            }

            var errors = result.Errors.Select(error => error.Description);
            return TypedResults.BadRequest(errors);
        }
        public static async Task<Results<Ok, NotFound, BadRequest<IEnumerable<string>>>> Delete(string userId, UserManager<User> userManager)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            var result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                return TypedResults.Ok();
            }

            var errors = result.Errors.Select(error => error.Description);
            return TypedResults.BadRequest(errors);
        }
    }
}

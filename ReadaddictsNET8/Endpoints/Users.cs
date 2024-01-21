using Application.Abstractions;
using Domain.Dto;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ReadaddictsNET8.Endpoints
{
    public static class Users
    {
        public static void AddUsersEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder users = routes.MapGroup("/api/v1/users");

            users.MapPost("/register", Register);
            users.MapPost("/login", Login);
            users.MapPost("/add-roles", AddRoles);
            users.MapPatch("/update", Update).DisableAntiforgery();
            users.MapPatch("/update-password", UpdatePassword).RequireAuthorization();
            users.MapDelete("/delete", Delete).RequireAuthorization();
            users.MapGet("/{username}", GetUser);
            users.MapGet("/id/{id}", GetUserById);
            users.MapGet("/current", GetCurrentUser).RequireAuthorization();
            users.MapPost("/logout", Logout).RequireAuthorization();
            users.MapPost("/refresh", UpdateLastLogin).RequireAuthorization();
        }

        public static async Task<Results<Ok<User>, BadRequest<IEnumerable<string>>>> Register(User user, string roleName, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                LastLogin = DateTimeOffset.UtcNow
            };

            var result = await userManager.CreateAsync(newUser, user.PasswordHash);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, roleName);
                await signInManager.SignInAsync(newUser, true);

                return TypedResults.Ok(newUser);
            }

            var errors = result.Errors.Select(error => error.Description);

            return TypedResults.BadRequest(errors);
        }
        public static async Task<Results<Ok, BadRequest<IEnumerable<string>>>> AddRoles(RoleManager<IdentityRole> roleManager)
        {
            var admin = new IdentityRole("Admin");
            var moderator = new IdentityRole("Mode rator");
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
        public static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult>> Login(string username, string password, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            var result = await signInManager.PasswordSignInAsync(username, password, true, false);

            if (result.Succeeded)
            {
                User? user = await signInManager.UserManager.FindByNameAsync(username);
                user!.LastLogin = DateTimeOffset.UtcNow;

                context.Update(user);

                await context.SaveChangesAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    ProfilePicture = user.ProfilePicture
                };

                return TypedResults.Ok(userDto);
            }

            return TypedResults.Unauthorized();
        }
        public static async Task<Results<Ok, BadRequest<IEnumerable<string>>, NotFound, UnauthorizedHttpResult>> Update([FromForm] User newUser, [FromForm] string password, [FromForm] IFormFile? profilePicture, ClaimsPrincipal user, UserManager<User> userManager, [FromServices] ICloudinaryRepository cloudinary)
        {
            // Still need to add profile picture change
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            User userToUpdate = await userManager.FindByIdAsync(userId);

            if (userToUpdate is null)
            {
                return TypedResults.NotFound();
            }

            bool validatePassword = await userManager.CheckPasswordAsync(userToUpdate, password);

            if (!validatePassword)
            {
                return TypedResults.Unauthorized();
            }

            if (!string.IsNullOrWhiteSpace(newUser.Biography))
            {
                userToUpdate.Biography = newUser.Biography;
            }

            if (!string.IsNullOrWhiteSpace(newUser.Email))
            {
                IdentityResult updateEmail = await userManager.SetEmailAsync(userToUpdate, newUser.Email);

                if (!updateEmail.Succeeded)
                {
                    IEnumerable<string> error = updateEmail.Errors.Select(error => error.Description);
                    return TypedResults.BadRequest(error);
                }
            }

            if (profilePicture is not null)
            {
                var (imageUrl, _, _) = await cloudinary.Upload(profilePicture, 300, 300); // All profile pictures will be 300x300
                userToUpdate.ProfilePicture = imageUrl;
            }

            var result = await userManager.UpdateAsync(userToUpdate);

            if (result.Succeeded)
            {
                return TypedResults.Ok();
            }

            IEnumerable<string> errors = result.Errors.Select(error => error.Description);
            return TypedResults.BadRequest(errors);
        }
        public static async Task<Results<Ok, BadRequest<IEnumerable<string>>, NotFound>> UpdatePassword(ClaimsPrincipal user, string currentPassword, string newPassword, UserManager<User> userManager)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            User userToUpdate = await userManager.FindByIdAsync(userId);

            if (userToUpdate is null)
            {
                return TypedResults.NotFound();
            }

            var result = await userManager.ChangePasswordAsync(userToUpdate, currentPassword, newPassword);

            if (result.Succeeded)
            {
                return TypedResults.Ok();
            }

            var errors = result.Errors.Select(error => error.Description);
            return TypedResults.BadRequest(errors);
        }
        public static async Task<Results<Ok<UserDto>, NotFound>> GetUser(string username, UserManager<User> userManager, ApplicationDbContext context)
        {
            User? user = await userManager.FindByNameAsync(username);

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                LastLogin = user.LastLogin,
                Biography = user.Biography,
                TierName = context.Tiers.Find(user.TierId)?.Name
            };

            return TypedResults.Ok(userDto);
        }
        public static async Task<Results<Ok<UserDto>, NotFound>> GetUserById(string id, UserManager<User> userManager, ApplicationDbContext context)
        {
            User? user = await userManager.FindByIdAsync(id);

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            var userDto = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                ProfilePicture = user.ProfilePicture,
                LastLogin = user.LastLogin,
                Biography = user.Biography,
                TierName = context.Tiers.Find(user.TierId)?.Name
            };

            return TypedResults.Ok(userDto);
        }
        public static async Task<Results<Ok<UserDto>, NotFound>> GetCurrentUser(ClaimsPrincipal user, UserManager<User> userManager)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            User? currentUser = await userManager.FindByIdAsync(userId);

            if (currentUser is null)
            {
                return TypedResults.NotFound();
            }

            var userDto = new UserDto
            {
                Id = currentUser.Id,
                UserName = currentUser.UserName,
                ProfilePicture = currentUser.ProfilePicture
            };

            return TypedResults.Ok(userDto);
        }
        public static async Task<Results<Ok, UnauthorizedHttpResult>> Logout(SignInManager<User> signInManager)
        {
            await signInManager.SignOutAsync();

            return TypedResults.Ok();
        }
        public static async Task<Results<Ok, UnauthorizedHttpResult>> UpdateLastLogin(ClaimsPrincipal user, ApplicationDbContext context)
        {
            string userId = user.FindFirst(ClaimTypes.NameIdentifier).Value;

            User? currentUser = await context.Users.FindAsync(userId);

            if (currentUser is null)
            {
                return TypedResults.Unauthorized();
            }

            currentUser.LastLogin = DateTimeOffset.UtcNow;

            await context.SaveChangesAsync();

            return TypedResults.Ok();
        }
    }
}

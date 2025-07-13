using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Readaddicts.Api.Endpoints
{
    public static class Antiforgery
    {
        public static void AddAntiForgeryEndpoints(this IEndpointRouteBuilder routes)
        {
            RouteGroupBuilder antiforgery = routes.MapGroup("/api/v1/antiforgery");

            antiforgery.MapGet("/token", GetToken).RequireAuthorization();
        }

        public static async Task<ContentHttpResult> GetToken(IAntiforgery antiforgery, HttpContext httpContext)
        {
            // Send header RequestVerificationToken
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            var xsrfToken = tokens.RequestToken;
            return TypedResults.Content(xsrfToken, "text/plain");
        }
    }
}

using Application.Abstractions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Readaddicts.Api.Endpoints
{
    internal class RegisterAntiForgeryEndpoints : IMinimalEndpoint
    {
        public void MapRoutes(IEndpointRouteBuilder routeBuilder)
        {
            var antiForgery = routeBuilder.MapGroup("/api/v1/antiforgery");

            antiForgery.MapGet("/token", Antiforgery.GetToken).RequireAuthorization();
        }
    }
    public static class Antiforgery
    {
        public static async Task<ContentHttpResult> GetToken(IAntiforgery antiforgery, HttpContext httpContext)
        {
            // Send header RequestVerificationToken
            var tokens = antiforgery.GetAndStoreTokens(httpContext);
            var xsrfToken = tokens.RequestToken;
            return TypedResults.Content(xsrfToken, "text/plain");
        }
    }
}

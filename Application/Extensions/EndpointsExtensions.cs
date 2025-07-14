using Application.Abstractions;
using Microsoft.AspNetCore.Routing;

namespace Application.Extensions
{
    public static class EndpointsExtensions
    {
        public static void AddMinimalEndpoints(this IEndpointRouteBuilder routeBuilder)
        {
            var endpointDefinitions = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => typeof(IMinimalEndpoint).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .Select(Activator.CreateInstance)
                .Cast<IMinimalEndpoint>();

            foreach (var endpoint in endpointDefinitions)
                endpoint.MapRoutes(routeBuilder);
        }
    }
}

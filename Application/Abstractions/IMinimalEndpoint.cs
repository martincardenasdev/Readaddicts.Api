using Microsoft.AspNetCore.Routing;

namespace Application.Abstractions
{
    public interface IMinimalEndpoint
    {
        void MapRoutes(IEndpointRouteBuilder routeBuilder);
    }
}

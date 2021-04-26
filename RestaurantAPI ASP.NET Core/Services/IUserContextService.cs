using System.Security.Claims;

namespace RestaurantAPI_ASP.NET_Core.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        int? GetUserId { get; }
    }
}
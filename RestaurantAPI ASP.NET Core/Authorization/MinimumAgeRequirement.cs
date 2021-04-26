using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI_ASP.NET_Core
{
    internal class MinimumAgeRequirement:IAuthorizationRequirement
    {
        public int MinimumAge { get; }

        public MinimumAgeRequirement(int v)
        {
            MinimumAge = v;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestaurantAPI_ASP.NET_Core.Authorization
{
    class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private ILogger<MinimumAgeRequirementHandler> _logger;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);
            if(dateOfBirth.AddDays(requirement.MinimumAge) <= DateTime.Today)
            {
                _logger.LogInformation("Succedded");
                context.Succeed(requirement);
            }


            return Task.CompletedTask;
        }
    }
}
